using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HEIMS_DOC_TO_JSON
{
    [JsonObject("DataElement")]
    public class DataElement
    {
        [JsonProperty("ElementNumber")]
        public string ElementNumber {get; set;}

        [JsonProperty("Version")]
        public string Version {get; set;}

        [JsonProperty("FirstYear")]
        public string FirstYear {get; set;}

        [JsonProperty("LastYear")]
        public string LastYear {get; set;}

        [JsonProperty("FieldName")]
        public string FieldName {get; set;}

        [JsonProperty("ElementName")]
        public string ElementName {get; set;}

        [JsonProperty("Description")]
        public string Description {get; set;}

        [JsonProperty("CodeFormat")]
        public List<KeyValue> CodeFormat {get; set;}
        
        [JsonProperty("Classification")]
        public List<KeyValue> Classification {get; set;}

        [JsonProperty("CodingNotes")]
        public string CodingNotes {get; set;}

        [JsonProperty("InputFIles")]
        public List<string> InputFiles {get; set;}        

        [JsonProperty("ChangeHistory")]
        public List<ChangeRecord> ChangeHistory {get; set;} 

    }
}