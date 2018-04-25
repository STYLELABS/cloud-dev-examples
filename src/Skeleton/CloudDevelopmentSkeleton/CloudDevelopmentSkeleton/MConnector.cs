using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Stylelabs Packages
using Stylelabs.M.Sdk.WebApiClient;

namespace CloudDevelopmentSkeleton
{
    public static class MConnector
    {
        private static readonly Lazy<MClient> _client = new Lazy<MClient>(() =>
            new MClient(AppSettings.OriginAddress, AppSettings.ClientId, AppSettings.ClientSecret, AppSettings.Username, AppSettings.Password)
        );

        public static MClient Client { get { return _client.Value; } }
    }
}
