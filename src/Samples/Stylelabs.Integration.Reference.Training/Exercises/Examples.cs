using Stylelabs.Integration.Reference.Training.Tools;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Filters;
using Stylelabs.M.Base.Web.Api.Models;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using Stylelabs.M.Sdk.WebApiClient.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.Training.Exercises
{
    public class Examples
    {
        #region Querying

        /// <summary>
        /// Examples of entity queries.
        /// </summary>
        /// <returns></returns>
        public static async Task EntityQueries(int skip = 0, int take = 10)
        {
            // Query by identifier
            var asset = await MConnector.Client.Entities.Get("identifier", Constants.DefaultCulture);

            // Definition query
            var assetsByDefinition = await MConnector.Client.Entities.GetByDefinition(Constants.EntityDefinitions.Asset.DefinitionName);

            // Linq query with paging
            var assets = await MConnector.Client.Querying.Query(
                Query.CreateEntitiesQuery(entities =>
                    (from e in entities
                     where e.DefinitionName == Constants.EntityDefinitions.Asset.DefinitionName
                     select e).Skip(skip).Take(take)));

            // Query through entity definition using Querying
            var qry = new Query
            {
                Filter = new DefinitionQueryFilter { Name = Constants.EntityDefinitions.Asset.DefinitionName },
                Skip = 0,
                Take = 50,
                EntityLoadOptions = new EntityLoadOptions { LoadEntities = false }
            };

            var assetsByDefinitionQuery = await MConnector.Client.Querying.Query(qry);
        }

        /// <summary>
        /// Examples of entity definition queries.
        /// </summary>
        /// <param name="definitionName">Name of the definition.</param>
        /// <returns></returns>
        public static async Task EntityDefinitionQueries(string definitionName = Constants.EntityDefinitions.Asset.DefinitionName)
        {
            // Get single definition
            var definition = await MConnector.Client.EntityDefinitions.Get(definitionName);

            // Get multiple definitions by name
            var definitionsByName = await MConnector.Client.EntityDefinitions.Get(new List<string>() { definitionName });

            // Get all definitions using default paging options
            var definitions = await MConnector.Client.EntityDefinitions.GetAll();
        }

        /// <summary>
        ///  Query a given setting.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="settingName">Name of the setting.</param>
        /// <returns></returns>
        public static async Task SettingQuery(string categoryName, string settingName)
        {
            // Get setting by category/setting name
            var setting = await MConnector.Client.Settings.Get(categoryName, settingName);
        }

        /// <summary>
        /// Query the policies for a given user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static async Task PoliciesQuery(long userId)
        {
            // Get policy by user id
            var policy = await MConnector.Client.Policies.Get(userId);
        }

        #endregion

        #region Schema

        /// <summary>
        /// Creates the training definition.
        /// </summary>
        /// <returns></returns>
        public static async Task CreateTrainingDefinition()
        {
            // Retrieve the training definition
            var trainingDefinition = await MConnector.Client.EntityDefinitions.Get(Constants.EntityDefinitions.Training.DefinitionName);
            if (trainingDefinition != null) return;

            // Create the entity definition
            trainingDefinition = new EntityDefinitionResource();
            trainingDefinition.Name = Constants.EntityDefinitions.Training.DefinitionName;
            trainingDefinition.IsTaxonomyItemDefinition = true;

            // Create the membergroup
            var memberGroup = new MemberGroup() { Name = Constants.EntityDefinitions.Recipe.MemberGroups.Main };
            trainingDefinition.MemberGroups.Add(memberGroup);

            // Create the name property definition
            memberGroup.MemberDefinitions.Add(new StringPropertyDefinition()
            {
                Name = Constants.EntityDefinitions.Training.Properties.Name,
                IncludedInContent = true,
                IncludedInCompletion = true,
                Indexed = true,
                IsMandatory = true
            });

            // Create the description property definition
            memberGroup.MemberDefinitions.Add(new StringPropertyDefinition()
            {
                Name = Constants.EntityDefinitions.Training.Properties.Description,
                IncludedInContent = true,
                Indexed = true,
                ContentType = StringPropertyDefinition.StringContentType.MultiLine
            });

            // Create or update the training definition
            await MConnector.Client.EntityDefinitions.Create(trainingDefinition);
        }

        #endregion

        #region Policies

        /// <summary>
        /// Example on how to setup a user policy.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static async Task SetupUserPolicy(long userId)
        {
            // Setup new user policy
            var policy = new PolicyResourceWrapper(MConnector.Client);
            await policy.SetUserId(userId);

            // Add rule to the policy
            await policy.AddRule(
                new[] { Constants.EntityDefinitions.Training.DefinitionName },
                new[] { Constants.Permissions.TrainingPermission },
                appliesToCreatedByLoggedOnUser: false);

            // Create the user policy
            var policyId = await MConnector.Client.Policies.Create(policy);
        }

        #endregion

        #region Download

        /// <summary>
        /// Downloads the rendition.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="targetLocation">The target location.</param>
        /// <param name="renditionName">Name of the rendition.</param>
        /// <returns></returns>
        public static async Task DownloadRendition(long entityId, string targetLocation, string renditionName = Constants.Renditions.Preview)
        {
            var entity = await MConnector.Client.Entities.Get(entityId);
            var rendition = entity.GetRendition(renditionName);

            await rendition.First().Download(targetLocation);
        }

        #endregion
    }
}
