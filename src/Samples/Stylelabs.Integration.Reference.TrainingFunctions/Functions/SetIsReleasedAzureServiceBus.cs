using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Stylelabs.Integration.Reference.TrainingFunctions.Helpers;
using Stylelabs.Integration.Reference.TrainingFunctions.Mappers;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using System;
using System.Threading.Tasks;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Functions
{
    public static class SetIsReleasedAzureServiceBus
    {
        [FunctionName("SetIsReleasedAzureServiceBus")]
        public static async Task Run([ServiceBusTrigger("myqueue", AccessRights.Manage, Connection = "ServiceBusConnection")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            // Parse message
            var message = SaveEntityMessageMapper.Map(myQueueItem);
            
            // Get entity
            var entity = await MConnector.Client.Entities.Get(message.TargetId, Constants.DefaultCulture);
            if (entity == null || entity.Resource == null) return;
            
            // Set isReleased property
            var releaseDate = entity.GetProperty<DateTime>(Constants.Properties.ReleaseDate);
            var isReleased = releaseDate.ToUniversalTime() <= DateTime.UtcNow;
            entity.SetProperty<bool>(Constants.Properties.IsReleased, isReleased);

            // Update entity
            log.Info($"Updating entity {message.TargetId}");
            await MConnector.Client.Entities.Update(entity);
        }
    }
}
