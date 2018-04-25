using Stylelabs.Integration.Reference.Training.Tools;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Linq;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using Stylelabs.M.Sdk.WebApiClient.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.Training.Exercises
{
    public static class WriteQueries
    {
        public static async Task<long> CreateAssetType(string name)
        {
            // Check if the asset type already exists
            var result = await MConnector.Client.Querying.Query(
                Query.CreateIdsQuery(entities =>
                    from e in entities
                    where e.Property(Constants.EntityDefinitions.AssetType.Properties.Label) == name
                    select e));

            if (result.TotalItems > 0) return result.Ids.Single();

            // Create a new asset type entity resource
            var assetType = new EntityResourceWrapper(MConnector.Client);
            assetType.Resource.EntityDefinition = Constants.EntityDefinitions.AssetType.DefinitionName;

            // Mark the asset type as a root taxonomy item
            assetType.Resource.IsRootTaxonomyItem = true;

            // Save the asset type
            var assetTypeId = await MConnector.Client.Entities.Create(assetType, Constants.EntityDefinitions.AssetType.DefinitionName);

            // Get our created resource
            assetType = await MConnector.Client.Entities.Get(assetTypeId);

            // Set the classification name
            assetType.SetProperty<string>(Constants.EntityDefinitions.AssetType.Properties.Label, name);

            // Update the resource
            await MConnector.Client.Entities.Update(assetType);

            return assetTypeId;
        }

        public static async Task<long> CreateAsset(string title)
        {
            // Create the entity resource
            var asset = new EntityResourceWrapper(MConnector.Client);

            // Set the mandatory title property
            asset.SetProperty<string>(Constants.EntityDefinitions.Asset.Properties.Title, title);

            // Link the asset to content repository: standard
            var standardContentRepository = await MConnector.Client.Entities.Get(Constants.ContentRepositories.Standard);
            var contentRepositoryRelation = await MConnector.Client.Entities.GetRelation(asset, Constants.EntityDefinitions.Asset.Relations.ContentRepositoryToAsset);
            await contentRepositoryRelation.SetParentIds(new List<long>() { standardContentRepository.Resource.Id });

            // Link the asset to final lifecycle: created
            var finalLifeCycleCreated = await MConnector.Client.Entities.Get(Constants.LifeCycleStatus.Created);
            var finalLifeCycleRelation = await MConnector.Client.Entities.GetRelation(asset, Constants.EntityDefinitions.Asset.Relations.FinalLifeCycleStatusToAsset);
            await finalLifeCycleRelation.SetParentId(finalLifeCycleCreated.Resource.Id);

            // Create the asset
            var assetId = await MConnector.Client.Entities.Create(asset, Constants.EntityDefinitions.Asset.DefinitionName);

            // Return a reference to the newly created asset
            return assetId;
        }

        public static async Task LinkAssetToAssetType(long assetId, long assetTypeId)
        {
            // Load the asset
            var asset = await MConnector.Client.Entities.Get(assetId);

            // Get the AssetTypeToAsset relation and link the asset to the given asset type
            var assetTypeToAsset = await asset.GetRelation(Constants.EntityDefinitions.Asset.Relations.AssetTypeToAsset);
            await assetTypeToAsset.SetParentId(assetTypeId);

            // Update the asset
            await MConnector.Client.Entities.Update(asset);
        }
    }
}
