using System.Collections.Generic;
using Newtonsoft.Json;

namespace HEIMS_DOC_TO_JSON
{
    public class DataElementFile
    {
        [JsonProperty]
        public List<DataElement> DataElements {get; set;}
    }
}