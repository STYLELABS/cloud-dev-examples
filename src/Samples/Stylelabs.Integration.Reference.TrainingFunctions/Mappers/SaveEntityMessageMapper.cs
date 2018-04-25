using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stylelabs.Integration.Reference.TrainingFunctions.Models;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Mappers
{
    public static class SaveEntityMessageMapper
    {
        public static SaveEntityMessage Map(string value)
        {
            var message = JToken.Parse(value)["saveEntityMessage"];
            return JsonConvert.DeserializeObject<SaveEntityMessage>(message.ToString(), 
                new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}
