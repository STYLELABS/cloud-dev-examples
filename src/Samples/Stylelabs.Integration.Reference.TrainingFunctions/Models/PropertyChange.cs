using Newtonsoft.Json;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Models
{
    public class PropertyChange
    {
        [JsonProperty("Property")]
        public string Property { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("OriginalValue")]
        public string OriginalValue { get; set; }

        [JsonProperty("NewValue")]
        public string NewValue { get; set; }
    }
}
