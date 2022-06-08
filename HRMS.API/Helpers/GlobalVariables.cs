using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HRMS.API.Helpers
{
    public static class GlobalVariables
    {
        public static string goAppHostName { get; set; }
        public static string goOAuthURI { get; set; }
        public static string goApplicationName { get; set; }
        public static bool goEnableSwagger { get; set; }
        public static bool goEnableAPI { get; set; }
        public static string goIssuer { get; set; }
        public static string goAudienceId { get; set; }
        public static string goAudienceSecret { get; set; }
        public static string goClientId { get; set; }
        public static string goDefaultSystemUploadRootDirectory { get; set; }
        public static string goDefaultSystemUserProfilePicPath { get; set; }
        public static string goDefaultItemTypeIconFilePath { get; set; }
        public static string goDefaultItemBrandIconFilePath { get; set; }
        public static string goDefaultItemIconFilePath { get; set; }
        //Email Service
        public static string goEmailVerificationTempPath { get; set; }
        public static string goChangePasswordTempPath { get; set; }
        public static string goForgotPasswordTempPath { get; set; }
        public static string goEmailTempProfilePath { get; set; }
        public static string goSiteSupportEmail { get; set; }
        public static string goSiteSupportEmailPassword { get; set; }
        public static string goClientLandingPageWebsite { get; set; }
        //End Email Service 

        public static string GetApplicationConfig(string pConfigurationkey)
        {
            return ConfigurationManager.AppSettings[pConfigurationkey].ToString();
        }
    }
}