using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using HRMS.Domain.Enumerations;

namespace HRMS.API.Helpers
{
    public static class GlobalFunctions
    {
        public static bool IsValidTimeFormat(this string input)
        {
            TimeSpan dummyOutput;
            return TimeSpan.TryParse(input, out dummyOutput);
        }

        public static Dictionary<string, string> SupportedAudioExtensions()
        {
            Dictionary<string, string> mediaExtensions = new Dictionary<string, string>();
            mediaExtensions.Add("WAV", "audio/vnd.wav");
            mediaExtensions.Add("MID", "audio/mid");
            mediaExtensions.Add("MP3", "audio/mpeg");
            mediaExtensions.Add("OGG", "audio/ogg");
            mediaExtensions.Add("RMA", "audio/vnd.rn-realaudio");
            return mediaExtensions;
        }

        public static Dictionary<string, string> SupportedImageExtensions()
        {
            Dictionary<string, string> mediaExtensions = new Dictionary<string, string>();
            mediaExtensions.Add("PNG", "image/png");
            mediaExtensions.Add("JPG", "image/jpeg");
            mediaExtensions.Add("JPEG", "image/jpeg");
            mediaExtensions.Add("BMP", "image/bmp");
            mediaExtensions.Add("GIF", "image/gif");
            return mediaExtensions;
        }

        public static Dictionary<string, string> SupportedVideoExtensions()
        {
            Dictionary<string, string> mediaExtensions = new Dictionary<string, string>();
            mediaExtensions.Add("AVI", "video/x-msvideo");
            mediaExtensions.Add("MP4", "video/mp4");
            mediaExtensions.Add("WMV", "video/x-ms-wmv");
            return mediaExtensions;
        }
        public static string GetFileRawFormatByExtension(string extenstion = "")
        {
            var mediaExtensios = new Dictionary<string, string>();
            foreach (var m in SupportedAudioExtensions())
            {
                mediaExtensios.Add(m.Key, m.Value);
            }
            foreach (var m in SupportedImageExtensions())
            {
                mediaExtensios.Add(m.Key, m.Value);
            }
            foreach (var m in SupportedVideoExtensions())
            {
                mediaExtensios.Add(m.Key, m.Value);
            }
            return mediaExtensios.FirstOrDefault(x => x.Key == extenstion.ToUpper()).Value;
        }
        public static string GetFileExtensionByFileRawFormat(string type = "")
        {
            var mediaExtensios = new Dictionary<string, string>();
            foreach(var m in SupportedAudioExtensions())
            {
                mediaExtensios.Add(m.Key, m.Value);
            }
            foreach (var m in SupportedImageExtensions())
            {
                mediaExtensios.Add(m.Key, m.Value);
            }
            foreach (var m in SupportedVideoExtensions())
            {
                mediaExtensios.Add(m.Key, m.Value);
            }
            return string.Format(".{0}", mediaExtensios.FirstOrDefault(x => x.Value.ToLower().Contains(type.ToLower()) || type.ToLower().Contains(x.Key.ToLower())).Key.ToLower());
        }
        public static long GetFileTypeByFileExtension(string extenstion = "")
        {
            if (SupportedAudioExtensions().ToList().Any(x => x.Value.ToLower().Contains(extenstion.ToLower()) || extenstion.ToLower().Contains(x.Key.ToLower())))
                return (int)DOC_REPORT_MEDIA_TYPE_ENUMS.AUDIO;
            else if(SupportedVideoExtensions().ToList().Any(x => x.Value.ToLower().Contains(extenstion.ToLower()) || extenstion.ToLower().Contains(x.Key.ToLower())))
                return (int)DOC_REPORT_MEDIA_TYPE_ENUMS.VIDEO;
            else
                return (int)DOC_REPORT_MEDIA_TYPE_ENUMS.IMAGE;
        }
    }
}