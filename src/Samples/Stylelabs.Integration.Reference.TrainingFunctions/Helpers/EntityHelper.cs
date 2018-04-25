using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Helpers
{
    public static class EntityHelper
    {
        public static async Task<long> GetEntityId(string definition, string property, string value)
        {
            var query = Query.CreateIdsQuery(entities =>
                    (from e in entities
                     where e.DefinitionName == definition && e.Property(property) == value
                     select e).Take(1));

            var result = await MConnector.Client.Querying.Query(query);
            return result.Ids.First();
        }
    }
}
