using System;
using System.Collections.Generic;
using System.Configuration;

namespace Stylelabs.Integration.Reference.Training
{
    public static class AppSettings
    {
        public static Uri OriginAddress
        {
            get { return new Uri(Get<string>(Constants.AppSettings.MOriginAddress)); }
        }

        public static string ClientId
        {
            get { return Get<string>(Constants.AppSettings.MClientId); }
        }

        public static string ClientSecret
        {
            get { return Get<string>(Constants.AppSettings.MClientSecret); }
        }

        public static string Username
        {
            get { return Get<string>(Constants.AppSettings.MUsername); }
        }

        public static string Password
        {
            get { return Get<string>(Constants.AppSettings.MPassword); }
        }

        public static string TempDirectory
        {
            get { return Get<string>(Constants.AppSettings.TempDirectory); }
        }

        private static T Get<T>(string key)
        {
            var result = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(result))
                throw new KeyNotFoundException($"Unable to find '{key}' appsetting.");

            var value = (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
            return value;
        }
    }
}
