using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Stylelabs.Integration.Reference.ExternalWebTask.Models;
using System.Linq;
using System.Text;

namespace Stylelabs.Integration.Reference.ExternalWebTask.Functions
{
    public static class ExternalWebTaskQueue
    {
        [FunctionName("ExternalWebTaskQueue")]
        public static void Run([QueueTrigger("externalwebtask", Connection = "StorageConnectionString")]string myQueueItem, TraceWriter log)
        {
            // Parse queue item
            var resource = JsonConvert.DeserializeObject<ExternalWebTaskResource>(myQueueItem);

            // Get metadata rendition
            var renditionClient = new RestClient(resource.Sources.First());
            var data = renditionClient.DownloadData(new RestRequest(Method.GET));
            var metadata = JToken.Parse(Encoding.Default.GetString(data));

            // Check resolution
            var width = metadata["File:ImageWidth"].Value<int>();
            var height = metadata["File:ImageHeight"].Value<int>();
            var isLowRes = width < 1000 || height < 1000;

            // POST to callback
            var callbackClient = new RestClient(resource.Callback);
            var request = new RestRequest() { Method = Method.POST, RequestFormat = DataFormat.Json };
            request.AddJsonBody(new { value = new { width, height, isLowRes } });
            callbackClient.Execute(request);
        }
    }
}
