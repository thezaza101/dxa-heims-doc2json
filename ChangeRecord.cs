using Newtonsoft.Json;

namespace HEIMS_DOC_TO_JSON
{
    public struct ChangeRecord
    {
        [JsonProperty("Version")]
        public string Version {get; set;}

        [JsonProperty("RevisionDate")]
        public string RevisionDate {get; set;}

        [JsonProperty("ReportingYear")]
        public string ReportingYear {get; set;}


    }
}