using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Linq;
using Stylelabs.M.Sdk.WebApiClient.Models;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using Stylelabs.M.Sdk.WebApiClient.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.DataMigration
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // Create asset
            var assetId = CreateAsset("Marketing Content Hub").Result;

            // Create fetch job
            var fetchJobId = CreateFetchJob("My fetch job", assetId, "https://stylelabs.com/wp-content/uploads/2017/09/logo-marketingcontenthub-600.png").Result;

            // Follow up on fetch job status
            CheckJobStatus(fetchJobId).Wait();

            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }

        private static async Task CheckJobStatus(long fetchJobId)
        {
            Console.WriteLine($"Checking status of fetch job {fetchJobId}.");

            // Wait for 5 seconds
            Thread.Sleep(5000);

            // Get job
            var job = await MConnector.Client.Entities.Get(fetchJobId);
            var condition = job.GetProperty<string>("Job.Condition");
            var status = job.GetProperty<string>("Job.State");
            
            if (!string.IsNullOrEmpty(status) && !string.IsNullOrEmpty(condition))
            {
                // Job has completed successfully
                if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase) && condition.Equals("Success", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Fetch job {fetchJobId} has completed successfully.");
                    return;
                }
            }

            // Check job status again
            await CheckJobStatus(fetchJobId);
        }

        private static async Task<long> CreateFetchJob(string description, long assetId, string url)
        {
            Console.WriteLine($"Creating fetch job '{description}'.");

            // Create fetch job for provided id and URL
            return await MConnector.Client.Jobs.CreateFetchJob(new WebFetchJobRequest(description, assetId)
            {
                Urls = new List<string> { url }
            });
        }

        private static async Task<long> CreateAsset(string title)
        {
            Console.WriteLine($"Creating asset {title}.");

            // Create entity
            var entity = new EntityResourceWrapper(MConnector.Client);

            // Set Title property
            entity.SetProperty<string>("Title", title);

            // Set FinalLifeCycleStatusToAsset
            var lifeCycleId = await GetEntityId("M.Final.LifeCycle.Status", "StatusValue", "Approved");
            var lifeCycleRelation = await entity.GetRelation("FinalLifeCycleStatusToAsset");
            await lifeCycleRelation.SetParentId(lifeCycleId);

            // Set ContentRepositoryToAsset
            var contentRepoId = await GetEntityId("M.Content.Repository", "ClassificationName", "Standard");
            var contentRepoRelation = await entity.GetRelation("ContentRepositoryToAsset");
            await contentRepoRelation.SetParentId(contentRepoId);

            // Save the asset
            return await MConnector.Client.Entities.Create(entity, "M.Asset");
        }

        private static async Task<long> GetEntityId(string definition, string property, string value)
        {
            // Get entity id for provided definition, property and value
            var query = Query.CreateIdsQuery(entities =>
                    (from e in entities
                     where e.DefinitionName == definition && e.Property(property) == value
                     select e).Take(1));

            var result = await MConnector.Client.Querying.Query(query);
            return result.Ids.First();
        }
    }
}
