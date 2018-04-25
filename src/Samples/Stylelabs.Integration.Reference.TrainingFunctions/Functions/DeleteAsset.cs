using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Stylelabs.Integration.Reference.TrainingFunctions.Helpers;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Functions
{
    public static class DeleteAsset
    {
        [FunctionName("DeleteAsset")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "head", "get", "post", Route = "delete")]HttpRequestMessage req, TraceWriter log)
        {
            // Allow head request to test connectivity from M.
            if (req.Method == HttpMethod.Head)
                return req.CreateResponse(HttpStatusCode.OK);
            
            // Extract id from request header.
            var id = req.Headers.GetValues("target_id").FirstOrDefault();
            log.Info($"id: {id}");

            await MConnector.Client.Entities.Delete(long.Parse(id));

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
