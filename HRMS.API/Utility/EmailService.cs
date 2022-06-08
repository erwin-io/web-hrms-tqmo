
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using HtmlAgilityPack;
using MailKit;
using MimeKit;
using Newtonsoft.Json;
using HRMS.API.Helpers;

namespace HRMS.API.Utility
{
    public static class EmailService
    {
        public static bool SendEmailVerificationMailKit(string recieverEmail, string code)
        {
            bool success = false;
            try
            {

                string filePath = HttpContext.Current.Server.MapPath(GlobalVariables.goEmailVerificationTempPath);
                string emailProfilePath = HttpContext.Current.Server.MapPath(GlobalVariables.goEmailTempProfilePath);
                StreamReader str = new StreamReader(filePath);
                string MailText = str.ReadToEnd();
                str.Close();

                MailText = MailText.Replace("[APP_VERIFICATION_HOSTNAME]", GlobalVariables.goAppHostName);
                MailText = MailText.Replace("[APP_NAME]", GlobalVariables.goApplicationName);
                MailText = MailText.Replace("[SUPPORT_EMAIL_ID]", GlobalVariables.goSiteSupportEmail);
                MailText = MailText.Replace("[EMAIL_ID]", recieverEmail);
                MailText = MailText.Replace("[VERIFICATION_CODE]", code);
                MailText = MailText.Replace("[CLIENT_LANDING_PAGE_WEBSITE]", GlobalVariables.goClientLandingPageWebsite.Replace("/", ""));


                byte[] bytes = File.ReadAllBytes(emailProfilePath);
                string base64 = Convert.ToBase64String(bytes);
                //MailText = MailText.Replace("[IMAGE_DATA]", $"data:image/png;base64,{base64}");


                string subject = "Email Verification";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(GlobalVariables.goSiteSupportEmail));
                message.To.Add(new MailboxAddress(recieverEmail));
                message.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = MailText;
                builder.Attachments.Add(emailProfilePath, bytes, new MimeKit.ContentType("image", "jpeg"));
                var contentId = MimeKit.Utils.MimeUtils.GenerateMessageId();
                builder.HtmlBody = builder.HtmlBody.Replace("[IMAGE_DATA]", contentId);
                builder.Attachments.FirstOrDefault().ContentId = contentId;
                message.Body = builder.ToMessageBody();

                using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    //Set HOST server SMTP detail
                    var smtpType = GetEmailType(recieverEmail);
                    if (smtpType == EmailSenderEnums.EmailSMTPTypeEnums.GMAIL)
                    {
                        smtpClient.Connect("smtp.gmail.com", 465, true);
                    }
                    if (smtpType == EmailSenderEnums.EmailSMTPTypeEnums.YAHOO)
                    {
                        smtpClient.Connect("smtp.mail.yahoo.com", 587, true);
                    }
                    if (smtpType == EmailSenderEnums.EmailSMTPTypeEnums.HOTMAIL)
                    {
                        smtpClient.Connect("smtp.live.com", 587, true);
                    }
                    smtpClient.Authenticate(GlobalVariables.goSiteSupportEmail, GlobalVariables.goSiteSupportEmailPassword);
                    smtpClient.SendAsync(message);
                    smtpClient.Disconnect(true);
                }

                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }
        public static bool SendEmailVerification(string recieverEmail, string code)
        {
            bool success = false;
            try
            {
                string filePath = HttpContext.Current.Server.MapPath(GlobalVariables.goEmailVerificationTempPath);
                string emailProfilePath = HttpContext.Current.Server.MapPath(GlobalVariables.goEmailTempProfilePath);
                StreamReader str = new StreamReader(filePath);
                string MailText = str.ReadToEnd();
                str.Close();

                MailText = MailText.Replace("[APP_VERIFICATION_HOSTNAME]", GlobalVariables.goAppHostName);
                MailText = MailText.Replace("[APP_NAME]", GlobalVariables.goApplicationName);
                MailText = MailText.Replace("[SUPPORT_EMAIL_ID]", GlobalVariables.goSiteSupportEmail);
                MailText = MailText.Replace("[EMAIL_ID]", recieverEmail);
                MailText = MailText.Replace("[VERIFICATION_CODE]", code);
                MailText = MailText.Replace("[CLIENT_LANDING_PAGE_WEBSITE]", GlobalVariables.goClientLandingPageWebsite.Replace("/", ""));

                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                   (MailText, null, MediaTypeNames.Text.Html);
                LinkedResource inline = new LinkedResource(emailProfilePath, MediaTypeNames.Image.Jpeg);
                inline.ContentId = Guid.NewGuid().ToString();
                avHtml.LinkedResources.Add(inline);

                Attachment att = new Attachment(emailProfilePath);
                att.ContentDisposition.Inline = true;
                MailText = MailText.Replace("[IMAGE_DATA]", att.ContentId);


                string subject = "Email Verification";

                //Base class for sending email
                MailMessage _mailmsg = new MailMessage();

                //Make TRUE because our body text is html
                _mailmsg.IsBodyHtml = true;

                //Set From Email ID
                _mailmsg.From = new MailAddress(GlobalVariables.goSiteSupportEmail);

                //Set To Email ID
                _mailmsg.To.Add(recieverEmail);

                //Set Subject
                _mailmsg.Subject = subject;

                //Set Body Text of Email 
                _mailmsg.Body = MailText;

                //Set Attachments
                _mailmsg.Attachments.Add(att);


                //Now set your SMTP 
                SmtpClient _smtp = new SmtpClient();

                //Set HOST server SMTP detail
                var smtpType = GetEmailType(recieverEmail);
                if (smtpType == EmailSenderEnums.EmailSMTPTypeEnums.GMAIL)
                {
                    _smtp.Host = "smtp.gmail.com";
                }
                if (smtpType == EmailSenderEnums.EmailSMTPTypeEnums.YAHOO)
                {
                    _smtp.Host = "smtp.mail.yahoo.com";
                }
                if (smtpType == EmailSenderEnums.EmailSMTPTypeEnums.HOTMAIL)
                {
                    _smtp.Host = "smtp.live.com";
                }

                //Set PORT number of SMTP
                _smtp.Port = 587;

                //Set SSL --> True / False
                _smtp.EnableSsl = true;

                //Set Default Credentials --> True / False
                _smtp.UseDefaultCredentials = false;

                //Set Sender UserEmailID, Password
                NetworkCredential _network = new NetworkCredential(GlobalVariables.goSiteSupportEmail, GlobalVariables.goSiteSupportEmailPassword);
                _smtp.Credentials = _network;

                //Send Method will send your MailMessage create above.
                _smtp.Send(_mailmsg);
                success = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return success;
        }
        public static bool SendEmailChangePassword(string recieverEmail, string systemUserId, string code)
        {
            bool success = false;
            try
            {
                string filePath = HttpContext.Current.Server.MapPath(GlobalVariables.goChangePasswordTempPath);
                string emailProfilePath = HttpContext.Current.Server.MapPath(GlobalVariables.goEmailTempProfilePath);
                StreamReader str = new StreamReader(filePath);
                string MailText = str.ReadToEnd();
                str.Close();

                MailText = MailText.Replace("[APP_VERIFICATION_HOSTNAME]", GlobalVariables.goAppHostName);
                MailText = MailText.Replace("[APP_NAME]", GlobalVariables.goApplicationName);
                MailText = MailText.Replace("[SUPPORT_EMAIL_ID]", GlobalVariables.goSiteSupportEmail);
                MailText = MailText.Replace("[SYSTEM_USER_ID]", systemUserId);
                MailText = MailText.Replace("[EMAIL_ID]", recieverEmail);
                MailText = MailText.Replace("[VERIFICATION_CODE]", code);
                MailText = MailText.Replace("[CLIENT_LANDING_PAGE_WEBSITE]", GlobalVariables.goClientLandingPageWebsite.Replace("/", ""));

                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                   (MailText, null, MediaTypeNames.Text.Html);
                LinkedResource inline = new LinkedResource(emailProfilePath, MediaTypeNames.Image.Jpeg);
                inline.ContentId = Guid.NewGuid().ToString();
                avHtml.LinkedResources.Add(inline);

                Attachment att = new Attachment(emailProfilePath);
                att.ContentDisposition.Inline = true;
                MailText = MailText.Replace("[IMAGE_DATA]", att.ContentId);
                string subject = "Change Password";

                //Base class for sending email
                MailMessage _mailmsg = new MailMessage();

                //Make TRUE because our body text is html
                _mailmsg.IsBodyHtml = true;

                //Set From Email ID
                _mailmsg.From = new MailAddress(GlobalVariables.goSiteSupportEmail);

                //Set To Email ID
                _mailmsg.To.Add(recieverEmail);

                //Set Subject
                _mailmsg.Subject = subject;

                //Set Body Text of Email 
                _mailmsg.Body = MailText;

                //Set Attachments
                _mailmsg.Attachments.Add(att);

                //Now set your SMTP 
                SmtpClient _smtp = new SmtpClient();

                //Set HOST server SMTP detail
                var smtpType = GetEmailType(recieverEmail);
                if (smtpType == EmailSenderEnums.EmailSMTPTypeEnums.GMAIL)
                {
                    _smtp.Host = "smtp.gmail.com";
                }
                if (smtpType == EmailSenderEnums.EmailSMTPTypeEnums.YAHOO)
                {
                    _smtp.Host = "smtp.mail.yahoo.com";
                }
                if (smtpType == EmailSenderEnums.EmailSMTPTypeEnums.HOTMAIL)
                {
                    _smtp.Host = "smtp.live.com";
                }

                //Set PORT number of SMTP
                _smtp.Port = 587;

                //Set SSL --> True / False
                _smtp.EnableSsl = true;

                //Set Default Credentials --> True / False
                _smtp.UseDefaultCredentials = false;

                //Set Sender UserEmailID, Password
                NetworkCredential _network = new NetworkCredential(GlobalVariables.goSiteSupportEmail, GlobalVariables.goSiteSupportEmailPassword);
                _smtp.Credentials = _network;

                //Send Method will send your MailMessage create above.
                _smtp.Send(_mailmsg);
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public static EmailSenderEnums.EmailSMTPTypeEnums GetEmailType(string email)
        {
            EmailSenderEnums.EmailSMTPTypeEnums smtpType;
            string output = email.Substring(email.IndexOf('@') + 1);
            if (output.ToUpper().Contains("YAHOO"))
            {
                smtpType = EmailSenderEnums.EmailSMTPTypeEnums.YAHOO;
            }
            else if (output.ToUpper().Contains("HOTMAIL"))
            {
                smtpType = EmailSenderEnums.EmailSMTPTypeEnums.HOTMAIL;
            }
            else
            {
                smtpType = EmailSenderEnums.EmailSMTPTypeEnums.GMAIL;
            }
            return smtpType;
        }

        public static string GenerateVerificationCode()
        {
            Random generator = new Random();
            string verificationCode = generator.Next(0, 100000).ToString("D5");
            return verificationCode;
        }
    }

    public class EmailSenderEnums
    {
        public enum EmailSMTPTypeEnums
        {
            YAHOO,
            GMAIL,
            HOTMAIL
        }
    }
}