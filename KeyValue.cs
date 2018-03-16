using Newtonsoft.Json;

namespace HEIMS_DOC_TO_JSON
{
    struct KeyValue
    {
        [JsonProperty("Attribute")]
        public string Attr {get; set;}

        [JsonProperty("Value")]
        public string Value {get; set;}
    }
}