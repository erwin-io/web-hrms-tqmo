
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
    public static class SMSService
    {
        public static bool SendVerification(string reciever, string code)
        {
            bool success = false;
            try
            {
                var body = new ITextMoSendMessageModel()
                {
                    Email = GlobalVariables.goITextMoEmail,
                    Password = GlobalVariables.goITextMoPassword,
                    Recipients = new string[] { reciever },
                    Message = $"{GlobalVariables.goITextMoMessageFormat} {code}",
                    ApiCode = GlobalVariables.goITextMoAPICode,

                    SenderId = GlobalVariables.goITextMoSenderId
                };
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(GlobalVariables.goITextMoAPIURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(body);

                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var response = JsonConvert.DeserializeObject<ITextMoResponseModel>(result);
                    if (response.Error)
                    {
                        throw new Exception(JsonConvert.SerializeObject(response.Message));
                    }
                    else
                        success = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }
    }
    public class ITextMoSendMessageModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string[] Recipients { get; set; }
        public string Message { get; set; }
        public string ApiCode { get; set; }
        public string SenderId { get; set; }
    }
    public class ITextMoResponseModel
    {
        public DateTime DateTime { get; set; }
        public bool Error { get; set; }
        public int TotalSMS { get; set; }
        public int Accepted { get; set; }
        public int TotalCreditUsed { get; set; }
        public int Failed { get; set; }
        public string ReferenceId { get; set; }
        public object Message { get; set; }
    }
}