using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Stylelabs.Integration.Reference.ExternalWebTask.Models
{
    public class ExternalWebTaskResource
    {
        public ExternalWebTaskResource()
        {
            Sources = new List<string>();
            Parameters = new Dictionary<string, JToken>();
        }

        [JsonProperty("callback")]
        public string Callback { get; set; }

        [JsonProperty("sources")]
        public IList<string> Sources { get; set; }

        [JsonProperty("parameters")]
        public IDictionary<string, JToken> Parameters { get; set; }
    }
}
