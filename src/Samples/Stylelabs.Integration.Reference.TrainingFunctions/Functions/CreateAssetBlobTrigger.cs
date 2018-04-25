using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Stylelabs.Integration.Reference.TrainingFunctions.Helpers;
using Stylelabs.M.Sdk.WebApiClient.Models;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using Stylelabs.M.Sdk.WebApiClient.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Functions
{
    public static class CreateAssetBlobTrigger
    {
        [FunctionName("CreateAssetBlobTrigger")]
        public static void Run([BlobTrigger("hotfolder/{name}", Connection = "StorageConnectionString")]ICloudBlob myBlob, string name, TraceWriter log)
        {
            // Create asset
            var id = CreateAsset(name).Result;

            // Create fetch job
            CreateFetchJob(id, myBlob.Uri.ToString()).Wait();
        }

        private static async Task<long> CreateAsset(string name)
        {
            // Create asset
            var asset = new EntityResourceWrapper(MConnector.Client);

            // Set title
            asset.SetProperty<string>(Constants.Properties.Title, name);

            // Set lifecycle
            var lifeCycleId = await EntityHelper.GetEntityId(Constants.Definitions.FinalLifeCycleStatus, Constants.Properties.StatusValue, "Approved");
            var lifeCycleRelation = await asset.GetRelation(Constants.Relations.FinalLifeCycleStatusToAsset);
            await lifeCycleRelation.SetParentId(lifeCycleId);

            // Set content repo
            var contentRepoId = await EntityHelper.GetEntityId(Constants.Definitions.ContentRepository, Constants.Properties.ClassificationName, "Standard");
            var contentRepoRelation = await asset.GetRelation(Constants.Relations.ContentRepositoryToAsset);
            await contentRepoRelation.SetParentId(contentRepoId);

            // save asset
            return await MConnector.Client.Entities.Create(asset, Constants.Definitions.Asset);
        }

        private static async Task CreateFetchJob(long assetId, string file)
        {
            await MConnector.Client.Jobs.CreateFetchJob(
                new WebFetchJobRequest("This is a fetch job created by a blob trigger.", assetId)
                {
                    Urls = new List<string>() { file }
                });
        }
    }
}
