using Stylelabs.Integration.Reference.Training.Exercises;
using Stylelabs.Integration.Reference.Training.Logging;
using Stylelabs.Integration.Reference.Training.Tools;
using Stylelabs.M.Sdk.WebApiClient;
using System;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.Training
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns></returns>
        private static async Task Run()
        {
            // Enable logging
            MClient.Logger = new ConsoleLogger();
            
            // Read queries
            await RunReadQueries();

            // Write queries
            await RunWriteQueries();

            // Schema changes
            await ReadQueries.DisplayEntityDefinitionInfo(Constants.EntityDefinitions.Asset.DefinitionName);
            await Schema.Update();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunReadQueries()
        {
            #region Entities Client

            // Request asset id for interaction with the read queries
            Console.WriteLine("Enter the id of the asset:");
            var assetId = long.Parse(Console.ReadLine());
            
            await ReadQueries.DisplayAssetInfoById(assetId);
            await ReadQueries.Download(assetId, Constants.Renditions.Thumbnail);
            await ReadQueries.DisplayAssetMediaEntitiesCount();

            #endregion

            #region Querying Client

            await ReadQueries.DisplayDescriptionByIdQueryFilter(assetId);

            // Request the asset type name and display asset type information
            Console.WriteLine("Enter the name of your asset type:");
            var assetTypeName = Console.ReadLine();
            
            await ReadQueries.ListAssetsByAssetType(assetTypeName);

            await ReadQueries.DisplayTotalAssetCount();

            await ReadQueries.DisplayAssetMediaOverview();
            
            var createdByAdminIds = await ReadQueries.GetCreatedByEntityIds("Administrator");
            Console.WriteLine($"{createdByAdminIds.Count} entities were created by the administrator");

            await ReadQueries.DisplayNewestAsset();

            await ReadQueries.DisplayProcessingJobsInfo();

            #endregion
        }

        private static async Task RunWriteQueries()
        {
            // Create the asset using the specified title
            Console.WriteLine("Enter a value for your asset's title:");
            string assetTitle = Console.ReadLine();
            var assetId = await WriteQueries.CreateAsset(assetTitle);

            // Create the asset type using the given name/label
            Console.WriteLine("Enter the name of your asset type:");
            string assetTypeName = Console.ReadLine();
            var assetTypeId = await WriteQueries.CreateAssetType(assetTypeName);

            // Link the asset to the asset type
            await WriteQueries.LinkAssetToAssetType(assetId, assetTypeId);

            // Request a resource url and create a fetch job
            Console.WriteLine("Enter the url to the desired resource:");
            string resourceUrl = Console.ReadLine();
            await Utilities.CreateFetchJob(assetId, resourceUrl);

            // Output the newly created asset id
            Console.WriteLine("Asset id: {0}", assetId);
            Console.ReadKey();
        }

        private static async Task Examples()
        {
            // Ingestion
            await Ingestion.ImportAssets();

            // Exporter
            await Exporter.ExportEntities(Constants.EntityDefinitions.Asset.DefinitionName, 10);

            // Policies
            await Security.SetupUserGroupPolicies("M.SDK.Training", Constants.EntityDefinitions.Recipe.DefinitionName, Constants.Permissions.CookingPermission);
        }
    }
}