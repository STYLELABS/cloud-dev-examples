using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stylelabs.M.Base.Web.Api.Models;
using Stylelabs.M.Sdk.WebApiClient.Wrappers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.Training.Tools
{
    public static class Exporter
    {
        private static readonly int BatchSize = 25;

        /// <summary>
        /// Exports the entities.
        /// </summary>
        /// <param name="entityDefinition">The entity definition.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public static async Task ExportEntities(string entityDefinition, int amount)
        {
            int skip = 0;

            while (amount > 0)
            {
                // Load the entities in batches
                var entities = await MConnector.Client.Entities.GetByDefinition(entityDefinition, skip, BatchSize);
                await Export(entities);

                skip += BatchSize;
                amount -= BatchSize;
            }
        }

        /// <summary>
        /// Converts to json.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private static async Task<JObject> ConvertToJson(EntityResourceWrapper entity)
        {
            JObject result = new JObject();

            // System properties
            result.Add(new JProperty("id", entity.Resource.Id));
            result.Add(new JProperty("identifier", entity.Resource.Identifier));
            result.Add(new JProperty("definition", entity.Resource.EntityDefinition.Uri));
            result.Add(new JProperty("uri", entity.Resource.Self.Uri));
            result.Add(new JProperty("version", entity.Resource.Version));
            result.Add(new JProperty("createdon", entity.Resource.CreatedOn));
            result.Add(new JProperty("createdby", entity.Resource.CreatedBy.Uri));
            result.Add(new JProperty("modifiedon", entity.Resource.ModifiedOn));
            result.Add(new JProperty("modifiedby", entity.Resource.ModifiedBy.Uri));

            // Properties
            JArray properties = new JArray();
            foreach (var property in entity.Resource.Properties)
            {
                properties.Add(new JObject(new JProperty(property.Key, property.Value)));
            }
            result.Add(new JProperty("properties", properties));

            // Relations
            JArray relations = new JArray();
            foreach (var relation in entity.Resource.Relations)
            {
                var definitionName = entity.Resource.EntityDefinition.Uri.Split('/').LastOrDefault();
                var definition = await MConnector.Client.EntityDefinitions.Get(definitionName);
                
                var memberGroup = definition.MemberGroups.FirstOrDefault(mg => mg.MemberDefinitions.FirstOrDefault(md=>md.Name.Equals(relation.Key)) != null);
                var relationDefinition = (RelationDefinition)memberGroup.MemberDefinitions.First(rd => rd.Name == relation.Key);
                
                if (relationDefinition.IsNested)
                {
                    var link = (RelationResource)relation.Value;
                    relations.Add(new JObject(new JProperty(relation.Key, link.Self.Uri)));
                }
                else
                {
                    var link = (Link)relation.Value;
                    relations.Add(new JObject(new JProperty(relation.Key, link.Uri)));
                }
            }

            result.Add(new JProperty("relations", relations));

            return result;
        }

        /// <summary>
        /// Exports the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        private static async Task Export(EntityCollectionResourceWrapper entities)
        {
            string exportPath = GenerateExportPath();
            JArray jsonEntities = new JArray();

            // Append to the json object when the file already exists
            if (File.Exists(exportPath))
            {
                var jsonData = File.ReadAllText(exportPath);
                jsonEntities = (JArray)JsonConvert.DeserializeObject(jsonData);
            }

            // Convert the entities to json
            foreach (EntityResourceWrapper entity in entities.Items)
            {
                JObject jsonEntity = await ConvertToJson(entity);
                jsonEntities.Add(jsonEntity);
            }

            // Export the changes to the filesystem
            using (StreamWriter file = new StreamWriter(exportPath))
            {
                await file.WriteLineAsync(jsonEntities.ToString(Newtonsoft.Json.Formatting.Indented));
            }
        }

        /// <summary>
        /// Generates the export path.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private static string GenerateExportPath()
        {
            // Validation
            if (!Directory.Exists(AppSettings.TempDirectory))
                throw new DirectoryNotFoundException($"Directory '{AppSettings.TempDirectory}' does not exist");

            string filename = $"export-{DateTime.UtcNow.ToString("MM-dd-yyyy")}.json";
            string exportPath = Path.Combine(AppSettings.TempDirectory, filename);

            return exportPath;
        }
    }
}
