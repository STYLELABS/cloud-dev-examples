using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Filters;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using Stylelabs.M.Sdk.WebApiClient.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.Training.Tools
{
    public static class Security
    {
        #region Policies

        /// <summary>
        /// Setups the user group policies.
        /// </summary>
        /// <param name="userGroupName">Name of the user group.</param>
        /// <param name="definitionName">Name of the definition.</param>
        /// <param name="permission">The permission.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task SetupUserGroupPolicies(string userGroupName, string definitionName, string permission)
        {
            // Ensure that the usergroup exists
            var userGroupId = await GetOrCreateUserGroupId(userGroupName);

            // Get the usergroup policies
            var policies = await MConnector.Client.Policies.Get(userGroupId);

            // Ensure that the definition exists
            var definition = await MConnector.Client.EntityDefinitions.Get(definitionName);
            if (definition == null) throw new InvalidOperationException($"definition '{definitionName}' does not exist");

            // Check if the rule already exists            
            var ruleResult = await policies.GetRulesWhere((d, c, rule) => rule.Definitions.Contains(definition.Self.Uri) && rule.Permissions.Contains(permission));
            if(ruleResult.Count() > 0) return;

            // Add training policy
            await policies.AddRule(
                new[] { definitionName },
                new[] { permission },
                appliesToCreatedByLoggedOnUser: false);
        }

        #endregion

        #region UserGroups

        /// <summary>
        /// Gets or creates the user group with the specified name and returns the id.
        /// </summary>
        /// <param name="userGroupName">Name of the user group.</param>
        /// <returns></returns>
        public static async Task<long> GetOrCreateUserGroupId(string userGroupName)
        {
            // Build query to retrieve the given usergroup
            var query = new Query
            {
                Filter = new CompositeQueryFilter
                {
                    Children = new List<QueryFilter>() {
                        new DefinitionQueryFilter
                        {
                            Name = Constants.EntityDefinitions.UserGroup.DefinitionName
                        },
                        new PropertyQueryFilter
                        {
                            Property = Constants.EntityDefinitions.UserGroup.Properties.GroupName,
                            Operator = ComparisonOperator.Equals,
                            DataType = FilterDataType.String,
                            Value = userGroupName
                        }
                    }
                },
                Skip = 0,
                Take = 1,
                EntityLoadOptions = new EntityLoadOptions() { LoadEntities = false }
            };

            // Attempt to retrieve the usergroup id
            var result = await MConnector.Client.Querying.Query(query);
            
            // Return the usergroup in case it already exists
            if (result.TotalItems.HasValue && result.TotalItems.Value > 0)
                return result.Ids.First();

            // Create the usergroup if it does not exist yet
            var userGroup = new EntityResourceWrapper(MConnector.Client);
            userGroup.SetProperty(Constants.EntityDefinitions.UserGroup.Properties.GroupName, userGroupName);

            return await MConnector.Client.Entities.Create(userGroup, Constants.EntityDefinitions.UserGroup.DefinitionName);
        }

        #endregion
    }
}
