using Stylelabs.M.Sdk.WebApiClient;
using System;

namespace Stylelabs.Integration.Reference.Training.Tools
{
    /// <summary>
    /// Connector class to connect to the M instance and expose the SDK Client
    /// </summary>
    public static class MConnector
    {
        private static readonly Lazy<MClient> _client = new Lazy<MClient>(() =>
            new MClient(AppSettings.OriginAddress, AppSettings.ClientId, AppSettings.ClientSecret, AppSettings.Username, AppSettings.Password)
        );

        public static MClient Client { get { return _client.Value; } }
    }
}
