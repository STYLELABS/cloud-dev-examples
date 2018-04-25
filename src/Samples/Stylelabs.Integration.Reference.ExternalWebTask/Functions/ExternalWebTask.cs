using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.ExternalWebTask.Functions
{
    public static class ExternalWebTask
    {
        [FunctionName("ExternalWebTask")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "externalwebtask")]HttpRequestMessage req, TraceWriter log)
        {
            // Parse request
            var content = await req.Content.ReadAsStringAsync();
            
            // Put on storage queue
            var connection = ConfigurationManager.AppSettings["StorageConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(connection);
            
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("externalwebtask");
            queue.CreateIfNotExists();

            var queueMessage = new CloudQueueMessage(content);
            queue.AddMessage(queueMessage);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
