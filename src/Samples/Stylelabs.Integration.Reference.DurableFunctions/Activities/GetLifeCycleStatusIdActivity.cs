using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Stylelabs.Integration.Reference.DurableFunctions.Helpers;
using Stylelabs.Integration.Reference.TrainingFunctions.Logging;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Linq;
using Stylelabs.M.Sdk.WebApiClient;
using System.Linq;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.DurableFunctions.Activities
{
    public static class GetLifeCycleStatusIdActivity
    {
        [FunctionName("GetLifeCycleStatusId")]
        public static async Task<long> GetLifeCycleStatusId([ActivityTrigger] string name, TraceWriter log)
        {
            MClient.Logger = new TraceWriterLogger(log);

            var lifeCycleQuery = Query.CreateIdsQuery(entities =>
                    (from e in entities
                     where e.DefinitionName == "M.Final.LifeCycle.Status" && e.Property("StatusValue") == name
                     select e).Take(1));

            var lifeCycleQueryResult = await MConnector.Client.Querying.Query(lifeCycleQuery);
            return lifeCycleQueryResult.Ids.First();
        }
    }
}
