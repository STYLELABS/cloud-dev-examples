using Newtonsoft.Json;
using System;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Models
{
    public class DeleteEntityMessage
    {
        [JsonProperty("TargetId")]
        public long TargetId { get; set; }

        [JsonProperty("TargetIdentifier")]
        public string TargetIdentifier { get; set; }

        [JsonProperty("TargetLabel")]
        public string TargetLabel { get; set; }

        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("TargetDefinition")]
        public string TargetDefinition { get; set; }

        [JsonProperty("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("EventType")]
        public string EventType { get; set; }
    }
}
