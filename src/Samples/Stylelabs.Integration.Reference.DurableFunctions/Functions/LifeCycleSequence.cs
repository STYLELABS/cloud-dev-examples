using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stylelabs.Integration.Reference.DurableFunctions.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.DurableFunctions.Functions
{
    public static class LifeCycleSequence
    {
        [FunctionName("LifeCycleSequence")]
        public static async Task<List<string>> Run(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            // Parse data
            var data = JObject.Parse(context.GetInput<string>());
            var status = data["status"].Value<string>();

            var output = new List<string>();

            // Retrieve lifecycle status id
            var lifeCycleId = await context.CallActivityAsync<long>("GetLifeCycleStatusId", status);
            output.Add($"Lifecycle id is {lifeCycleId}.");

            // Split ids in batches
            var ids = data["ids"].ToObject<List<long>>();
            var batches = ids.SplitIntoBatches(25);

            foreach (var batch in batches)
            {
                if (!batch.Any()) continue;

                // Process all entries in a single batch in parallel
                var parallelTasks = new List<Task<IList<string>>>();

                foreach (var id in batch)
                {
                    var obj = new JObject()
                    {
                        new JProperty("lifeCycleId", lifeCycleId),
                        new JProperty("id", id)
                    };

                    var value = JsonConvert.SerializeObject(obj);

                    var task = context.CallActivityAsync<IList<string>>("SetLifeCycleStatus", value);
                    parallelTasks.Add(task);
                }

                await Task.WhenAll(parallelTasks);

                parallelTasks.Select(d => d.Result).ToList().ForEach(d => output.AddRange(d));
            }

            return output;
        }
    }
}
