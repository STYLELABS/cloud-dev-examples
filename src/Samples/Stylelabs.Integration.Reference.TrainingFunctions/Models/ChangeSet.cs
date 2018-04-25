using Newtonsoft.Json;
using System.Collections.Generic;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Models
{
    public class ChangeSet
    {
        [JsonProperty("Culture")]
        public string Culture { get; set; }

        [JsonProperty("PropertyChanges")]
        public IList<PropertyChange> PropertyChanges { get; set; }

        [JsonProperty("RelationChanges")]
        public IList<RelationChange> RelationChanges { get; set; }
    }
}
