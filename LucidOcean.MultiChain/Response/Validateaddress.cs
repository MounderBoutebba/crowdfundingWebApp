using Newtonsoft.Json;

namespace LucidOcean.MultiChain.Response
{

    public class Validateaddress
    {
        public Validateaddress()
        {

        }

        [JsonProperty("isvalid")]
        public bool isvalid { get; set; }

        [JsonProperty("address")]
        public string address { get; set; }

        [JsonProperty("ismine")]
        public bool ismine { get; set; }

        [JsonProperty("iswatchonly")]
        public bool iswatchonly { get; set; }

        [JsonProperty("isscript")]
        public bool isscript { get; set; }

        [JsonProperty("account")]
        public string account { get; set; }

        [JsonProperty("synchronized")]
        public bool synchronized { get; set; }

        [JsonProperty("startblock")]
        public string startblock { get; set; }
    }
}
