using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stylelabs.Integration.Reference.TrainingFunctions.Helpers;
using Stylelabs.M.Sdk.WebApiClient.Exceptions;
using Stylelabs.M.Sdk.WebApiClient.Models;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using Stylelabs.M.Sdk.WebApiClient.Wrappers;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Functions
{
    public static class CreateAsset
    {
        [FunctionName("CreateAsset")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "head", "post", Route = "create")]HttpRequestMessage req, TraceWriter log)
        {
            // Connectivity check
            if (req.Method == HttpMethod.Head) return req.CreateResponse(HttpStatusCode.OK);
            
            // Parse request
            var body = await req.Content.ReadAsStringAsync();
            var token = JToken.Parse(body);

            var title = token["title"].Value<string>();
            var image = token["image"].Value<string>();

            // Create entity
            var entity = new EntityResourceWrapper(MConnector.Client);

            // Set Title property
            entity.SetProperty<string>(Constants.Properties.Title, title);
            
            // Set FinalLifeCycleStatusToAsset to 'Created'
            var lifeCycleId = await EntityHelper.GetEntityId(Constants.Definitions.FinalLifeCycleStatus, Constants.Properties.StatusValue, "Created");
            var lifeCycleRelation = await entity.GetRelation(Constants.Relations.FinalLifeCycleStatusToAsset);
            await lifeCycleRelation.SetParentId(lifeCycleId);

            // Set ContentRepositoryToAsset to 'Standard'
            var contentRepoId = await EntityHelper.GetEntityId(Constants.Definitions.ContentRepository, Constants.Properties.ClassificationName, "Standard");
            var contentRepoRelation = await entity.GetRelation(Constants.Relations.ContentRepositoryToAsset);
            await contentRepoRelation.SetParentId(contentRepoId);

            long assetId;

            try
            {
                // Save entity
                assetId = await MConnector.Client.Entities.Create(entity, Constants.Definitions.Asset);
                log.Info($"Entity {assetId} created.");

                var fetchJobId = await MConnector.Client.Jobs.CreateFetchJob(new WebFetchJobRequest("my fetch job", assetId)
                {
                    Urls = new List<string> { image }
                });

                log.Info($"Fetch job {fetchJobId} created.");
            }
            catch (WebApiValidationException ex)
            {
                // Handle validation errors
                foreach (var failure in ex.Failures)
                {
                    log.Error($"{failure.Key} - {failure.Message}");
                }
                throw;
            }

            // Return asset id in response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(new { assetId }), Encoding.UTF8, "application/json");
            return response;
        }
    }
}
