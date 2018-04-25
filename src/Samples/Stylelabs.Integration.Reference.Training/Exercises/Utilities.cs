using Stylelabs.Integration.Reference.Training.Tools;
using Stylelabs.M.Framework.Utilities;
using Stylelabs.M.Sdk.WebApiClient.Models;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.Training.Exercises
{
    public static class Utilities
    {
        /// <summary>
        /// Creates the fetch job for the specified entity.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="resourceUrl">The resource URL.</param>
        /// <returns></returns>
        public static async Task CreateFetchJob(long entityId, string resourceUrl)
        {
            // Validation
            Guard.GreaterThan("entityId", entityId, 0);
            Guard.NotNullOrEmpty("resourceUrl", resourceUrl);

            // Create the fetch job request
            var fjr = new WebFetchJobRequest("File", entityId);
            fjr.Urls.Add(resourceUrl);

            // Create the fetch job
            await MConnector.Client.Jobs.CreateFetchJob(fjr);
        }
    }
}
