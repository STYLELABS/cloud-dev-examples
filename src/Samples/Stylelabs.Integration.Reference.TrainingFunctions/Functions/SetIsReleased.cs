using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Stylelabs.Integration.Reference.TrainingFunctions.Helpers;
using Stylelabs.Integration.Reference.TrainingFunctions.Mappers;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Functions
{
    public static class SetIsReleased
    {
        [FunctionName("SetIsReleased")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "head", "post", Route = "release")]HttpRequestMessage req, TraceWriter log)
        {
            // Connectivity check
            if (req.Method == HttpMethod.Head)
                return req.CreateResponse(HttpStatusCode.OK);
            
            // Extract message from request body
            var value = await req.Content.ReadAsStringAsync();
            log.Info($"Message is {value}.");

            var message = SaveEntityMessageMapper.Map(value);
            
            // Extract target id from request header
            var targetId = req.Headers.GetValues("target_id").FirstOrDefault();
            var id = long.Parse(targetId);

            // Get entity
            log.Info($"Loading entity {message.TargetId}.");
            var entity = await MConnector.Client.Entities.Get(message.TargetId, Constants.DefaultCulture);
            if (entity == null || entity.Resource == null) return req.CreateResponse(HttpStatusCode.NotFound);

            // Set isReleased
            var releaseDate = entity.GetProperty<DateTime>(Constants.Properties.ReleaseDate);
            var isReleased = releaseDate.ToUniversalTime() <= DateTime.UtcNow;
            entity.SetProperty<bool>(Constants.Properties.IsReleased, isReleased);

            // Update entity
            log.Info($"Updating entity {message.TargetId}.");
            await MConnector.Client.Entities.Update(entity);

            // Create response
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
