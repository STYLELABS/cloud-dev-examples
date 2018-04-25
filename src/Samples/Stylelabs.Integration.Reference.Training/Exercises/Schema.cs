using Stylelabs.Integration.Reference.Training.Extensions;
using Stylelabs.Integration.Reference.Training.Tools;
using Stylelabs.M.Base.Web.Api.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.Training.Exercises
{
    public static class Schema
    {
        private static string assetDefinitionUri;
        private static string chefDefinitionUri;
        private static string recipeDefinitionUri;

        /// <summary>
        /// Updates the schema/domain model.
        /// </summary>
        /// <returns></returns>
        public static async Task Update()
        {
            // Retrieve the asset definition to setup the associated relation definition
            var assetDefinitionResult = await MConnector.Client.EntityDefinitions.Get(Constants.EntityDefinitions.Asset.DefinitionName);
            assetDefinitionUri = assetDefinitionResult.Self.Uri;

            // Create or update the chef definition
            chefDefinitionUri = await CreateOrUpdateChefDefinition();

            // Create or update the recipe definition
            recipeDefinitionUri = await CreateOrUpdateRecipeDefinition();

            // Update the chef labels
            IDictionary<CultureInfo, IDictionary<string, string>> chefLabels = new Dictionary<CultureInfo, IDictionary<string, string>>()
            {
                {
                    Constants.DefaultCulture,
                    new Dictionary<string, string>() {
                        { Constants.EntityDefinitions.Chef.Relations.ChefToRecipe, "Recipe"}
                    }
                }
            };

            await UpdateLabels(Constants.EntityDefinitions.Chef.DefinitionName, chefLabels);
        }

        /// <summary>
        /// Creates or updates the chef definition.
        /// </summary>
        /// <returns></returns>
        private static async Task<string> CreateOrUpdateChefDefinition()
        {
            // Retrieve the chef definition
            var chefDefinition = await MConnector.Client.EntityDefinitions.Get(Constants.EntityDefinitions.Chef.DefinitionName);

            // Create the entity definition
            if (chefDefinition == null)
            {
                chefDefinition = new EntityDefinitionResource();
                chefDefinition.Name = Constants.EntityDefinitions.Chef.DefinitionName;
                chefDefinition.IsTaxonomyItemDefinition = true;
                chefDefinition.Labels = new Dictionary<string, string>() { { Constants.DefaultCulture.Name, Constants.EntityDefinitions.Chef.DefinitionName } };
            }

            // Get the membergroup
            var memberGroup = chefDefinition.MemberGroups.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Chef.MemberGroups.Main);
            if (memberGroup == null)
            {
                memberGroup = new MemberGroup() { Name = Constants.EntityDefinitions.Chef.MemberGroups.Main };
                chefDefinition.MemberGroups.Add(memberGroup);
            }

            #region Properties

            // Create the Name property definition if it doesn't exist already
            var namePropertyDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Chef.Properties.Name);
            if (namePropertyDefinition == null)
                memberGroup.MemberDefinitions.Add(new StringPropertyDefinition()
                {
                    Name = Constants.EntityDefinitions.Chef.Properties.Name,
                    IncludedInContent = true,
                    IncludedInCompletion = true,
                    Indexed = true,
                    IsMandatory = true
                });



            // Create the bio property definition if it doesn't exist already
            var bioPropertyDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Chef.Properties.Bio);
            if (bioPropertyDefinition == null)
                memberGroup.MemberDefinitions.Add(new StringPropertyDefinition()
                {
                    Name = Constants.EntityDefinitions.Chef.Properties.Bio,
                    IncludedInContent = true,
                    IncludedInCompletion = true,
                    Indexed = true,
                    IsMandatory = true,
                    ContentType = StringPropertyDefinition.StringContentType.Html
                });

            #endregion

            // Set the display template
            chefDefinition.DisplayTemplate = "{" + Constants.EntityDefinitions.Chef.Properties.Name + "}";

            // Create or update the chef definition
            if (!chefDefinition.Exists())
            {
                await MConnector.Client.EntityDefinitions.Create(chefDefinition);
                chefDefinition = await MConnector.Client.EntityDefinitions.Get(Constants.EntityDefinitions.Chef.DefinitionName);
            }
            else
                await MConnector.Client.EntityDefinitions.Update(chefDefinition);

            return chefDefinition.Self.Uri;
        }

        /// <summary>
        /// Creates or updates the recipe definition.
        /// </summary>
        /// <returns></returns>
        private static async Task<string> CreateOrUpdateRecipeDefinition()
        {
            // Retrieve the recipe definition
            var recipeDefinition = await MConnector.Client.EntityDefinitions.Get(Constants.EntityDefinitions.Recipe.DefinitionName);

            // Create the entity definition
            if (recipeDefinition == null)
            {
                recipeDefinition = new EntityDefinitionResource();
                recipeDefinition.Name = Constants.EntityDefinitions.Recipe.DefinitionName;
                recipeDefinition.IsTaxonomyItemDefinition = true;
                recipeDefinition.Labels = new Dictionary<string, string>() { { Constants.DefaultCulture.Name, Constants.EntityDefinitions.Recipe.DefinitionName } };
            }

            // Get the membergroup
            var memberGroup = recipeDefinition.MemberGroups.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Recipe.MemberGroups.Main);
            if (memberGroup == null)
            {
                memberGroup = new MemberGroup() { Name = Constants.EntityDefinitions.Recipe.MemberGroups.Main };
                recipeDefinition.MemberGroups.Add(memberGroup);
            }

            #region Properties

            // Create the Name property definition if it doesn't exist already
            var namePropertyDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Recipe.Properties.Name);
            if (namePropertyDefinition == null)
                memberGroup.MemberDefinitions.Add(new StringPropertyDefinition()
                {
                    Name = Constants.EntityDefinitions.Recipe.Properties.Name,
                    IncludedInContent = true,
                    IncludedInCompletion = true,
                    Indexed = true,
                    IsMandatory = true
                });

            // Create the Description property definition if it doesn't exist already
            var descriptionPropertyDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Recipe.Properties.Description);
            if (descriptionPropertyDefinition == null)
                memberGroup.MemberDefinitions.Add(new StringPropertyDefinition()
                {
                    Name = Constants.EntityDefinitions.Recipe.Properties.Description,
                    IncludedInContent = true,
                    IncludedInCompletion = true,
                    Indexed = true,
                    ContentType = StringPropertyDefinition.StringContentType.MultiLine
                });

            // Create the Categories property definition if it doesn't exist already
            var categoriesPropertyDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Recipe.Properties.Categories);
            if (categoriesPropertyDefinition == null)
            {
                var categoriesDataSource = await GetOrCreateCategoriesDataSource();
                memberGroup.MemberDefinitions.Add(new StringPropertyDefinition()
                {
                    Name = Constants.EntityDefinitions.Recipe.Properties.Categories,
                    DataSource = categoriesDataSource.Self.Uri,
                    MultiValue = true,
                    Labels = new Dictionary<string, string>() { { Constants.DefaultCulture.Name, "Categories" } }
                });
            }

            // Create the Preparation property definition if it doesn't exist already
            var preparationPropertyDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Recipe.Properties.Preparation);
            if (preparationPropertyDefinition == null)
                memberGroup.MemberDefinitions.Add(new StringPropertyDefinition()
                {
                    Name = Constants.EntityDefinitions.Recipe.Properties.Preparation,
                    IncludedInContent = true,
                    IncludedInCompletion = true,
                    Indexed = true,
                    ContentType = StringPropertyDefinition.StringContentType.Html
                });

            // Create the Servings property definition if it doesn't exist already
            var servingsPropertyDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Recipe.Properties.Servings);
            if (servingsPropertyDefinition == null)
                memberGroup.MemberDefinitions.Add(new IntegerPropertyDefinition()
                {
                    Name = Constants.EntityDefinitions.Recipe.Properties.Servings,
                    Indexed = true
                });

            // PublishDate
            var publishDatePropertyDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Recipe.Properties.PublishDate);
            if (publishDatePropertyDefinition == null)
                memberGroup.MemberDefinitions.Add(new DateTimePropertyDefinition()
                {
                    Name = Constants.EntityDefinitions.Recipe.Properties.PublishDate,
                    Indexed = true
                });

            // Create the Vegetarian property definition if it doesn't exist already
            var exclusivePropertyDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Recipe.Properties.Vegetarian);
            if (exclusivePropertyDefinition == null)
                memberGroup.MemberDefinitions.Add(
                    new BooleanPropertyDefinition()
                    {
                        Name = Constants.EntityDefinitions.Recipe.Properties.Vegetarian,
                        Indexed = true
                    });

            #endregion

            #region Relations

            // Create the RecipeToMasterAsset relation if it doesn't exist already
            var recipeToAssetRelationDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Recipe.Relations.RecipeToMasterAsset);
            if (recipeToAssetRelationDefinition == null)
                memberGroup.MemberDefinitions.Add(new RelationDefinition()
                {
                    Name = Constants.EntityDefinitions.Recipe.Relations.RecipeToMasterAsset,
                    AssociatedEntityDefinition = assetDefinitionUri,
                    Cardinality = Cardinality.ManyToMany,
                    InheritsSecurity = true,
                    Labels = new Dictionary<string, string>() { { Constants.DefaultCulture.Name, "Master Asset" } }
                });

            // Create the ChefToRecipe relation if it doesn't exist already
            var chefToRecipeRelationDefinition = memberGroup.MemberDefinitions.FirstOrDefault(x => x.Name == Constants.EntityDefinitions.Chef.Relations.ChefToRecipe);
            if (chefToRecipeRelationDefinition == null)
                memberGroup.MemberDefinitions.Add(new RelationDefinition()
                {
                    Name = Constants.EntityDefinitions.Chef.Relations.ChefToRecipe,
                    AssociatedEntityDefinition = chefDefinitionUri,
                    Cardinality = Cardinality.OneToMany,
                    InheritsSecurity = true,
                    Labels = new Dictionary<string, string>() { { Constants.DefaultCulture.Name, "Chef" } }
                });

            #endregion

            // Set the display template
            recipeDefinition.DisplayTemplate = "{" + Constants.EntityDefinitions.Recipe.Properties.Name + "}";

            // Create or update the recipe definition
            if (!recipeDefinition.Exists())
            {
                await MConnector.Client.EntityDefinitions.Create(recipeDefinition);
                recipeDefinition = await MConnector.Client.EntityDefinitions.Get(Constants.EntityDefinitions.Recipe.DefinitionName);
            }
            else
                await MConnector.Client.EntityDefinitions.Update(recipeDefinition);

            return recipeDefinition.Self.Uri;
        }

        #region Data Sources

        /// <summary>
        /// Gets or creates the categories data source.
        /// </summary>
        /// <returns></returns>
        private static async Task<DataSourceResource> GetOrCreateCategoriesDataSource()
        {
            var categories = await MConnector.Client.DataSources.Get(Constants.DataSources.RecipeCategories);
            if (categories != null) return categories;

            var categoriesResource = new DataSourceResource()
            {
                Name = Constants.DataSources.RecipeCategories,
                Type = DataSourceType.Flat,
                IsSystemOwned = false,
                Values = new List<DataSourceValue>()
                {
                    new DataSourceValue() {
                        Identifier = "Asian",
                        Labels = new Dictionary<CultureInfo, string>() { { Constants.DefaultCulture, "Asian" } }
                    },
                    new DataSourceValue() {
                        Identifier = "Paleo",
                        Labels = new Dictionary<CultureInfo, string>() { { Constants.DefaultCulture, "Paleo" } }
                    },
                    new DataSourceValue() {
                        Identifier = "Southern",
                        Labels = new Dictionary<CultureInfo, string>() { { Constants.DefaultCulture, "Southern" } }
                    }
                }
            };

            // Create and return the categories datasource
            await MConnector.Client.DataSources.Create(categoriesResource);
            return await MConnector.Client.DataSources.Get(Constants.DataSources.RecipeCategories);

            #endregion
        }

        private static async Task UpdateLabels(string entityDefinitionName, IDictionary<CultureInfo, IDictionary<string, string>> labels)
        {
            // Retrieve the entity definition
            var entityDefinition = await MConnector.Client.EntityDefinitions.Get(entityDefinitionName);

            // Update the labels based on the given culture info
            foreach (CultureInfo cultureInfo in labels.Keys)
            {
                IDictionary<string, string> values = labels[cultureInfo];

                foreach (KeyValuePair<string, string> label in values)
                {
                    // Retrieve the membergroup that holds the given memberdefinition
                    var memberGroup = entityDefinition.MemberGroups.FirstOrDefault(mg => 
                                        mg.MemberDefinitions.FirstOrDefault(md => md.Name.Equals(label.Key)) != null);
                    if (memberGroup == null) continue;

                    // Retrieve the actual memberdefinition
                    var memberDefinition = memberGroup.MemberDefinitions.FirstOrDefault(md => md.Name.Equals(label.Key));
                    if (memberDefinition == null) continue;

                    // Update the label
                    memberDefinition.Labels[cultureInfo.Name] = label.Value;
                }
            }

            // Update the entity definition
            await MConnector.Client.EntityDefinitions.Update(entityDefinition);
        }
    }
}
