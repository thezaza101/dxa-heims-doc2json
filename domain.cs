using System.Collections.Generic;
using Newtonsoft.Json;

namespace HEIMS_DOC_TO_JSON
{
    public class Domain
    {
        [JsonProperty("domain")]
        public string domain {get; set;}

        [JsonProperty("acronym")]
        public string acronym {get; set;}

        [JsonProperty("version")]
        public string version {get; set;}

        [JsonProperty("sourceURL")]
        public string sourceURL {get; set;}




        [JsonProperty("content")]
        public List<OutputDataElement> content {get; set;}
        
    }

    public class OutputDataElement
    {
        [JsonProperty("name")]
        public string Name {get; set;}

        [JsonProperty("domain")]
        public string Domain {get; set;}

        [JsonProperty("status")]
        public string Status {get; set;}


        [JsonProperty("definition")]
        public string Definition {get; set;}

        
        [JsonProperty("guidance")]
        public string guidance {get; set;}

        [JsonProperty("usage")]
        public List<string> usage {get; set;}

        [JsonProperty("datatype")]
        public outputDataType dataType {get; set;}

        [JsonProperty("values")]
        public List<string> values {get; set;}

        [JsonProperty("sourceURL")]
        public string sourceURL {get; set;}

    }

    public class outputDataType
    {
        [JsonProperty("facets")]
        public string facets {get; set;}

        [JsonProperty("type")]
        public string type {get; set;}
    }
}