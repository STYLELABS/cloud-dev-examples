using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Stylelabs.Integration.Reference.TrainingFunctions.Helpers;
using Stylelabs.M.Sdk.WebApiClient.Exceptions;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Functions
{
    public static class SetLifeCycleStatus
    {
        [FunctionName("SetLifeCycleStatus")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "head", "post", Route = "lifecycle/{status}")]HttpRequestMessage req, TraceWriter log, string status)
        {
            // Connectivity check
            if (req.Method == HttpMethod.Head)
                return req.CreateResponse(HttpStatusCode.OK);

            // Extract target id from request header
            var targetId = req.Headers.GetValues("target_id").FirstOrDefault();

            // Get asset
            log.Info($"Loading entity {targetId}.");
            var entity = await MConnector.Client.Entities.Get(long.Parse(targetId), Constants.DefaultCulture);
            if (entity == null || entity.Resource == null) return req.CreateResponse(HttpStatusCode.NotFound);

            // Set lifecycle
            var lifeCycleId = await EntityHelper.GetEntityId(Constants.Definitions.FinalLifeCycleStatus, Constants.Properties.StatusValue, status);
            var lifeCycleRelation = await entity.GetRelation(Constants.Relations.FinalLifeCycleStatusToAsset);
            await lifeCycleRelation.SetParentId(lifeCycleId);

            try
            {
                // Update entity
                log.Info($"Updating entity {targetId}.");
                await MConnector.Client.Entities.Update(entity);
            }
            catch (WebApiValidationException ex)
            {
                // Handle validation errors
                foreach (var failure in ex.Failures)
                {
                    log.Error($"{failure.Key} - {failure.Message}");
                }

                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("Validation error.", Encoding.UTF8, "application/json");
                return response;

            }
            catch (Exception ex)
            {
                // Generic error logging
                log.Error(ex.Message);

                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            
            // Create response
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
