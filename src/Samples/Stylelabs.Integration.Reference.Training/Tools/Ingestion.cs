using Newtonsoft.Json.Linq;
using Stylelabs.M.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Stylelabs.M.Base.Querying.Linq;
using System.Linq;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Sdk.WebApiClient.Wrappers;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using Stylelabs.M.Sdk.WebApiClient.Models;

namespace Stylelabs.Integration.Reference.Training.Tools
{
    public static class Ingestion
    {
        /// <summary>
        /// Imports the assets.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task ImportAssets()
        {
            // Parse the import data
            var importData = Files.AssetImportData;
            var dataArray = JArray.Parse(Encoding.UTF8.GetString(importData), new JsonLoadSettings());

            // Load content repository: standard
            var standardContentRepository = await MConnector.Client.Entities.Get("M.Content.Repository.Standard");

            foreach (var jt in dataArray.Children())
            {
                // Parse entries
                var title = jt.Value<string>("title");
                var description = jt.Value<string>("description");
                var status = jt.Value<string>("status");
                var imageUrl = jt.Value<string>("imageUrl");

                // Create asset
                var assetId = await CreateAsset(title, description, status, standardContentRepository.Resource.Id);
                
                // Create the fetch job
                var fetchJobId = await CreateFetchJob(assetId, imageUrl);
            }
        }

        /// <summary>
        /// Creates the asset.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="assetTypeId">The asset type identifier.</param>
        /// <param name="resourceUrl">The resource URL.</param>
        /// <returns></returns>
        public static async Task<long> CreateAsset(string title, string description, string status, long contentRepositoryId)
        {
            // Initialize the entity resource
            var entity = new EntityResourceWrapper(MConnector.Client);

            // Title
            entity.SetProperty<string>(Constants.EntityDefinitions.Asset.Properties.Title, title);

            // Description
            entity.SetProperty<string>(Constants.EntityDefinitions.Asset.Properties.Description, description, Constants.DefaultCulture);

            // Link to content repository
            var contentRepositoryRelation = await MConnector.Client.Entities.GetRelation(entity, Constants.EntityDefinitions.Asset.Relations.ContentRepositoryToAsset);
            await contentRepositoryRelation.SetParentId(contentRepositoryId);

            // Link the asset to final lifecycle
            var finalLifeCycleStatus = await MConnector.Client.Entities.Get($"M.Final.LifeCycle.Status.{status}");
            var finalLifeCycleRelation = await MConnector.Client.Entities.GetRelation(entity, Constants.EntityDefinitions.Asset.Relations.FinalLifeCycleStatusToAsset);
            await finalLifeCycleRelation.SetParentId(finalLifeCycleStatus.Resource.Id);

            // Process the relation updates
            return await MConnector.Client.Entities.Create(entity, Constants.EntityDefinitions.Asset.DefinitionName);
        }

        /// <summary>
        /// Creates the fetch job.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="resourceUrl">The resource URL.</param>
        /// <returns></returns>
        private static async Task<long> CreateFetchJob(long entityId, string resourceUrl)
        {
            // Validation
            Guard.GreaterThan("entityId", entityId, 0);
            Guard.NotNullOrEmpty("resourceUrl", resourceUrl);

            // Create the fetch job request
            var fjr = new WebFetchJobRequest("File", entityId);
            fjr.Urls.Add(resourceUrl);

            // Create the fetch job
            return await MConnector.Client.Jobs.CreateFetchJob(fjr);
        }
    }
}
