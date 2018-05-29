using Newtonsoft.Json.Linq;
using Stylelabs.M.Sdk.WebApiClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.ValidationTest
{
    public static class MConnector
    {
        private static readonly HttpClient client = new HttpClient();

        private static readonly Lazy<MClient> _client = new Lazy<MClient>(() =>
            new MClient(AppSettings.OriginAddress, AppSettings.ClientId, AppSettings.ClientSecret, AppSettings.Username, AppSettings.Password)
        );

        public static MClient Client { get { return _client.Value; } }

        public static async Task<bool> VerifyConnection(string email)
        {
            string endpoint = AppSettings.VerificationEndpoint;
            var json = new JObject();
            json.Add("email", JToken.FromObject(email));

            HttpResponseMessage response = await client.PostAsync(endpoint, new StringContent(json.ToString()));
            return (response.IsSuccessStatusCode);
        }
    }
}
