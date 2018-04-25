using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stylelabs.Integration.Reference.TrainingFunctions.Models;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Mappers
{
    public static class DeleteEntityMessageMapper
    {
        public static DeleteEntityMessage Map(string value)
        {
            var message = JToken.Parse(value)["deleteEntityMessage"];
            return JsonConvert.DeserializeObject<DeleteEntityMessage>(message.ToString(),
                new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}
