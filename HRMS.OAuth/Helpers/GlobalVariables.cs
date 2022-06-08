using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HRMS.OAuth.Helpers
{
    public static class GlobalVariables
    {
        public static string goApplicationName { get; set; }
        public static string goIssuer { get; set; }
        public static string goAudienceId { get; set; }
        public static string goAudienceSecret { get; set; }
        public static string goClientId { get; set; }
        public static double goRefreshTokenLifeTime { get; set; }

        public static string GetApplicationConfig(string pConfigurationkey)
        {
            return ConfigurationManager.AppSettings[pConfigurationkey].ToString();
        }
    }
}