using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Stylelabs.Integration.Reference.TrainingFunctions.Helpers;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Functions
{
    public static class GetFile
    {
        [FunctionName("GetFile")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "head", "get", Route = "file/{id}/{renditionName}")]HttpRequestMessage req, TraceWriter log, long id, string renditionName)
        {
            // Connectivity check
            if (req.Method == HttpMethod.Head) return req.CreateResponse(HttpStatusCode.OK);

            // Fallback for original
            if (renditionName.Equals("original", StringComparison.OrdinalIgnoreCase)) renditionName = "downloadOriginal";

            // Get the rendition from the entity
            var entity = await MConnector.Client.Entities.Get(id);
            var rendition = entity.GetRendition(renditionName);
            var renditionItem = rendition?.FirstOrDefault();

            // If there's no rendition, return 404.
            if (renditionItem == null) return req.CreateResponse(HttpStatusCode.NotFound);

            // Get a stream to the rendition and return it.
            using (var stream = await renditionItem.Download())
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(StreamToBytes(stream))
                };

                return result;
            }
        }

        private static byte[] StreamToBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
