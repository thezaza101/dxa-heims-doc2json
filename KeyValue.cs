using Newtonsoft.Json;

namespace HEIMS_DOC_TO_JSON
{
    public struct KeyValue
    {
        [JsonProperty("Attribute")]
        public string Attr {get; set;}

        [JsonProperty("Value")]
        public string Value {get; set;}

        public override bool Equals(object obj)
        {
            KeyValue item = (KeyValue)obj;
            return (this.Attr.Equals(item.Attr)&&this.Value.Equals(item.Value));
        }
    }
}