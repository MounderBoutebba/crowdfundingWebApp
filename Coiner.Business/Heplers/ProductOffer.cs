using Coiner.Business.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Heplers
{
    [JsonObject]
    public class ProductOffer
    {
        [JsonProperty("offerAssetName")]
        public string OfferAssetName { get; set; }

        [JsonProperty("offerQuantity")]
        public decimal OfferQuantity { get; set; }

        [JsonProperty("askAssetName")]
        public string AskAssetName { get; set; }

        [JsonProperty("askQuantity")]
        public decimal AskQuantity { get; set; }

        [JsonProperty("pxCoin")]
        public decimal PxCoin { get; set; }

        [JsonProperty("fromAddress")]
        public string FromAddress { get; set; }

        [JsonProperty("txId")]
        public string TxId { get; set; }

        [JsonProperty("hexBlob")]
        public string HexBlob { get; set; }

        [JsonProperty("action")]
        public OfferTypeEnum Operation { get; set; }

        [JsonProperty("date")]
        public string Date { get; set;}


    }
}
