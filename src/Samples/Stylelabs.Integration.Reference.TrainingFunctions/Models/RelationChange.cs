using Newtonsoft.Json;
using System.Collections.Generic;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Models
{
    public class RelationChange
    {
        [JsonProperty("Relation")]
        public string Relation { get; set; }

        [JsonProperty("Role")]
        public string Role { get; set; }

        [JsonProperty("Cardinality")]
        public string Cardinality { get; set; }

        [JsonProperty("NewValues")]
        public ICollection<long> NewValues { get; set; }

        [JsonProperty("RemovedValues")]
        public ICollection<long> RemovedValues { get; set; }
    }
}
