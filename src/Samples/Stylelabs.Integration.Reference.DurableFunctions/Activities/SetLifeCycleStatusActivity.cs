using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using Stylelabs.Integration.Reference.DurableFunctions.Helpers;
using Stylelabs.Integration.Reference.TrainingFunctions.Logging;
using Stylelabs.M.Sdk.WebApiClient;
using Stylelabs.M.Sdk.WebApiClient.Exceptions;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.DurableFunctions.Activities
{
    public static class SetLifeCycleStatusActivity
    {
        [FunctionName("SetLifeCycleStatus")]
        public static async Task<IList<string>> SetLifeCycleStatus([ActivityTrigger] string value, TraceWriter log)
        {
            MClient.Logger = new TraceWriterLogger(log);

            var obj = JObject.Parse(value);
            var id = obj["id"].Value<long>();

            var entity = await MConnector.Client.Entities.Get(id);

            var lifeCycleId = obj["lifeCycleId"].Value<long>();
            var lifeCycleRelation = await entity.GetRelation("FinalLifeCycleStatusToAsset");
            await lifeCycleRelation.SetParentId(lifeCycleId);

            var result = new List<string>();
            result.Add($"Updating lifecycle for entity {entity.Resource.Id}.");

            try
            {
                await MConnector.Client.Entities.Update(entity);
            }
            catch (WebApiValidationException ex)
            {
                foreach (var failure in ex.Failures)
                {
                    result.Add(string.Concat(failure.Key, " - ", failure.Message));
                }
            }

            return result;
        }
    }
}
