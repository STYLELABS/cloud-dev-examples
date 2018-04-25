using Stylelabs.M.Base.Web.Api.Models;

namespace Stylelabs.Integration.Reference.Training.Extensions
{
    public static class EntityDefinitionResourceExtensions
    {
        public static bool Exists(this EntityDefinitionResource resource)
        {
            return (resource.Id != default(long));
        }
    }
}
