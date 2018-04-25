using Stylelabs.Integration.Reference.Training.Tools;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Filters;
using Stylelabs.M.Framework.Utilities;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using Stylelabs.M.Sdk.WebApiClient.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.Training.Exercises
{
    public static class ReadQueries
    {
        #region Entities Client

        /// <summary>
        /// Displays asset information by id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static async Task DisplayAssetInfoById(long id)
        {
            // Load the entity by id using the EntitiesClient 
            var entity = await MConnector.Client.Entities.Get(id);
            if (entity == null || entity.Resource == null)
            {
                Console.WriteLine($"Entity with id '{id}' could not be found");
                return;
            }

            // Title
            var title = entity.GetProperty<string>(Constants.EntityDefinitions.Asset.Properties.Title);
            if (!string.IsNullOrEmpty(title)) Console.WriteLine($"Asset #{entity.Resource.Id} title is '{title}'");

            // Description
            var description = entity.GetProperty<string>(Constants.EntityDefinitions.Asset.Properties.Description, Constants.DefaultCulture);
            if (!string.IsNullOrEmpty(description)) Console.WriteLine($"Asset #{entity.Resource.Id} description is: '{description}'");

            // Asset type
            var assetTypeRelation = await entity.GetRelation(Constants.EntityDefinitions.Asset.Relations.AssetTypeToAsset);
            var assetTypeId = await assetTypeRelation.GetParentId();

            // Load the asset type and display its value when available
            if (assetTypeId.HasValue)
            {
                var assetType = await MConnector.Client.Entities.Get(assetTypeId.Value);
                var assetTypeLabel = assetType.GetProperty<string>(Constants.EntityDefinitions.AssetType.Properties.Label, Constants.DefaultCulture);
                Console.WriteLine($"Asset #{entity.Resource.Id} is linked to asset type '{assetTypeLabel}'");
            }
        }

        /// <summary>
        /// Downloads the rendition of the specified entity.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="renditionName">Name of the rendition.</param>
        /// <param name="targetLocation">The target location.</param>
        /// <returns></returns>
        public static async Task Download(long entityId, string renditionName, string targetLocation = null)
        {
            // Get the entity/rendition for download purposes
            var entity = await MConnector.Client.Entities.Get(entityId);
            var rendition = entity.GetRendition(renditionName);
            if (!rendition.Any()) return;

            // Set a default target location when none was specified
            if (string.IsNullOrEmpty(targetLocation))
            {
                var filename = entity.GetProperty<string>(Constants.EntityDefinitions.Asset.Properties.FileName);
                targetLocation = Path.Combine(AppSettings.TempDirectory, filename);
            }

            // Initiate the download
            await rendition.First().Download(targetLocation);
        }

        /// <summary>
        /// Displays the asset media entities count.
        /// </summary>
        /// <returns></returns>
        public static async Task DisplayAssetMediaEntitiesCount()
        {
            var result = await MConnector.Client.Entities.GetByDefinition(Constants.EntityDefinitions.AssetMedia.DefinitionName, 0, 1);
            Console.WriteLine($"There are {result.TotalItems} entities of definition '{Constants.EntityDefinitions.AssetMedia.DefinitionName}' in the system.");
        }

        #endregion

        #region Querying Client

        /// <summary>
        /// Displays the description by id query filter.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static async Task DisplayDescriptionByIdQueryFilter(long id)
        {
            // Build a query to load the entity by id 
            var qry = new Query
            {
                Filter = new IdQueryFilter { Value = id },
                Skip = 0,
                Take = 1,
                EntityLoadOptions = new EntityLoadOptions
                {
                    LoadEntities = true,
                    PropertiesToLoad = new string[] { Constants.EntityDefinitions.Asset.Properties.Description },
                    RelationsToLoad = new string[0],
                    CulturesToLoad = new string[] { Constants.DefaultCulture.Name }
                }
            };

            // Ececute the query / validation
            var result = await MConnector.Client.Querying.Query(qry);
            if (result.TotalItems == 0) return;
            var entity = result.Items.First();

            // Description
            var description = entity.GetProperty<string>(Constants.EntityDefinitions.Asset.Properties.Description, Constants.DefaultCulture);

            // Display the description
            if (!string.IsNullOrEmpty(description))
                Console.WriteLine($"The description for asset with id '{entity.Resource.Id}' is '{description}'");
        }

        /// <summary>
        /// Gets the asset type by name.
        /// </summary>
        /// <param name="assetTypeName">Name of the asset type.</param>
        /// <returns></returns>
        public static async Task<EntityResourceWrapper> GetAssetTypeByName(string assetTypeName)
        {
            Guard.NotNullOrEmpty("assetTypeName", assetTypeName);

            // Build the query with a property filter
            var qry = new Query
            {
                Filter = new PropertyQueryFilter
                {
                    Property = Constants.EntityDefinitions.AssetType.Properties.Label,
                    Value = assetTypeName,
                    Culture = Constants.DefaultCulture
                },
                Skip = 0,
                Take = 1,
                EntityLoadOptions = new EntityLoadOptions
                {
                    LoadEntities = true,
                    PropertiesToLoad = new string[] { Constants.EntityDefinitions.AssetType.Properties.Label },
                    RelationsToLoad = new string[] { Constants.EntityDefinitions.AssetType.Relations.AssetTypeToAsset }
                }
            };

            // Execute the query
            var result = await MConnector.Client.Querying.Query(qry);
            return result.Items.FirstOrDefault();
        }

        /// <summary>
        /// Lists the type of the assets by asset type.
        /// </summary>
        /// <param name="assetTypeName">Name of the asset type.</param>
        /// <returns></returns>
        public static async Task ListAssetsByAssetType(string assetTypeName)
        {
            var assetType = await GetAssetTypeByName(assetTypeName);
            if (assetType == null)
            {
                Console.WriteLine($"Could not find asset type {assetTypeName}");
                return;
            }

            var qry = new Query()
            {
                Filter = new RelationQueryFilter()
                {
                    ParentId = assetType.Resource.Id,
                    Relation = Constants.EntityDefinitions.AssetType.Relations.AssetTypeToAsset
                },
                EntityLoadOptions = new EntityLoadOptions()
                {
                    LoadEntities = true,
                    PropertiesToLoad = new string[] { Constants.EntityDefinitions.Asset.Properties.Title },
                    RelationsToLoad = new string[0]
                }
            };

            var result = await MConnector.Client.Querying.Query(qry);
            Console.WriteLine($"Found {result.TotalItems} entities linked to asset type {assetTypeName}");

            foreach (var asset in result.Items)
            {
                var title = asset.GetProperty<string>(Constants.EntityDefinitions.Asset.Properties.Title);
                Console.WriteLine(title);
            }
        }

        /// <summary>
        /// Gets the created by entity ids based on the given username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public static async Task<IList<long>> GetCreatedByEntityIds(string username)
        {
            var result = await MConnector.Client.Querying.Query(
                Query.CreateIdsQuery(entities =>
                    from e in entities
                    where e.CreatedByUsername == username
                    select e));

            return result.Ids;
        }

        /// <summary>
        /// Displays an asset media overview.
        /// </summary>
        /// <returns></returns>
        public static async Task DisplayAssetMediaOverview()
        {
            long? totalItems;
            int batchSize = 20;
            int idx = 0;

            do
            {
                // Load the entities by definition
                var result = await MConnector.Client.Entities.GetByDefinition(Constants.EntityDefinitions.AssetMedia.DefinitionName, idx, batchSize);
                totalItems = result.TotalItems;

                foreach (var am in result.Items)
                {
                    // Get the asset media name
                    var name = am.GetProperty<string>(Constants.EntityDefinitions.AssetMedia.Properties.ClassificationName);

                    // Build a query to retrieve the linked assets
                    var query = new Query()
                    {
                        Filter = new RelationQueryFilter()
                        {
                            ParentId = am.Resource.Id,
                            Relation = Constants.EntityDefinitions.AssetMedia.Relations.AssetMediaToAsset
                        },
                        EntityLoadOptions = new EntityLoadOptions()
                        {
                            LoadEntities = false,
                            PropertiesToLoad = new string[0],
                            RelationsToLoad = new string[0]
                        }
                    };

                    // Retrieve the assets and display the count for the asset media
                    var assetsResult = await MConnector.Client.Querying.Query(query);
                    Console.WriteLine($"{name} : {assetsResult.TotalItems}");
                }

                idx += result.Items.Count;
            } while (totalItems.HasValue && totalItems > idx);
        }

        /// <summary>
        /// Displays the total asset count.
        /// </summary>
        /// <returns></returns>
        public static async Task DisplayTotalAssetCount()
        {
            // Build a query to retrieve the asset info
            var query = new Query()
            {
                Filter = new DefinitionQueryFilter()
                {
                    Name = Constants.EntityDefinitions.Asset.DefinitionName
                },
                EntityLoadOptions = new EntityLoadOptions()
                {
                    LoadEntities = false,
                    PropertiesToLoad = new string[0],
                    RelationsToLoad = new string[0]
                }
            };

            // Execute the query and display the count
            var result = await MConnector.Client.Querying.Query(query);
            Console.WriteLine($"Assets : {result.TotalItems}");
        }

        public static async Task DisplayNewestAsset()
        {
            var qry = new Query()
            {
                Filter = new DefinitionQueryFilter()
                {
                    Name = Constants.EntityDefinitions.Asset.DefinitionName
                },
                EntityLoadOptions = new EntityLoadOptions()
                {
                    LoadEntities = true,
                    PropertiesToLoad = new string[] { Constants.EntityDefinitions.Asset.Properties.Title },
                    RelationsToLoad = new string[0]
                },
                Take = 1,
                Sorting = new Sorting[] {
                    new Sorting() {
                        Field = Constants.Properties.CreatedOn,
                        FieldType = SortFieldType.System,
                        Order = QuerySortOrder.Desc
                    }
                }
            };

            var result = await MConnector.Client.Querying.Query(qry);

            foreach (var asset in result.Items)
            {
                var title = asset.GetProperty<string>(Constants.EntityDefinitions.Asset.Properties.Title);
                Console.WriteLine(title);
            }
        }

        public static async Task DisplayProcessingJobsInfo()
        {
            var qry = new Query()
            {
                Filter = new CompositeQueryFilter()
                {
                    Children = new List<QueryFilter>() {
                        new DefinitionQueryFilter()
                        {
                            Name = Constants.EntityDefinitions.Job.DefinitionName
                        },
                        new PropertyQueryFilter()
                        {
                            Property = Constants.EntityDefinitions.Job.Properties.Type,
                            Value = Constants.EntityDefinitions.Job.Types.Processing
                        },
                        new PropertyQueryFilter()
                        {
                            Property = Constants.EntityDefinitions.Job.Properties.Condition,
                            Value = Constants.EntityDefinitions.Job.Conditions.Pending
                        }
                    },
                    CombineMethod = CompositeFilterOperator.And
                }
            };

            var pendingJobResults = await MConnector.Client.Querying.Query(qry);

            qry = new Query()
            {
                Filter = new CompositeQueryFilter()
                {
                    Children = new List<QueryFilter>() {
                        new DefinitionQueryFilter()
                        {
                            Name = Constants.EntityDefinitions.Job.DefinitionName
                        },
                        new PropertyQueryFilter()
                        {
                            Property = Constants.EntityDefinitions.Job.Properties.Type,
                            Value = Constants.EntityDefinitions.Job.Types.Processing
                        },
                        new PropertyQueryFilter()
                        {
                            Property = Constants.EntityDefinitions.Job.Properties.Condition,
                            Value = Constants.EntityDefinitions.Job.Conditions.Success
                        }
                    },
                    CombineMethod = CompositeFilterOperator.And
                }
            };

            var completedJobResults = await MConnector.Client.Querying.Query(qry);

            Console.WriteLine($"Pending processing jobs in the system: {pendingJobResults.Ids.Count}");
            Console.WriteLine($"Completed processing jobs in the system: {completedJobResults.Ids.Count}");
        }

        #endregion

        #region Entity Definitions Client

        /// <summary>
        /// Displays information about the specified entity definition.
        /// </summary>
        /// <param name="definitionName">Name of the definition.</param>
        /// <returns></returns>
        public static async Task DisplayEntityDefinitionInfo(string definitionName)
        {
            // Load the definition
            var definition = await MConnector.Client.EntityDefinitions.Get(definitionName);

            // Loop through the membergroups
            foreach (var mg in definition.MemberGroups)
            {
                // Loop through the definitions
                foreach (var md in mg.MemberDefinitions)
                {
                    // Display info for the non system owned member definitions
                    if (!md.IsSystemOwned)
                        Console.WriteLine($"{mg.Name} : {md.Name}");
                }
            }
        }

        #endregion
    };
}
