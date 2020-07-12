using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Heplers
{
    [JsonObject]
    public class AssetDetails
    {
        [JsonProperty("coinVal")]
        public string CoinVal { get; set; }

        [JsonProperty("sellPercentage")]
        public string SellPercentage { get; set; }

    }
}
