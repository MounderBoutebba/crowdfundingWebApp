﻿/*=====================================================================
Authors: Lucid Ocean PTY (LTD)
Copyright © 2017 Lucid Ocean PTY (LTD). All Rights Reserved.

License: Dual MIT / Lucid Ocean Wave Business License v1.0
Please refer to http://www.lucidocean.co.za/wbl-license.html for restrictions and freedoms.
The full license will also be found on the root of the main source-code directory.
=====================================================================*/
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LucidOcean.MultiChain.Response
{
    public class TxOutResponse
    {

        [JsonProperty("bestblock")]
        public string BestBlock { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty("scriptPubKey")]
        public ScriptPubKeyResponse ScriptPubKey { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("coinbase")]
        public bool Coinbase { get; set; }

        [JsonProperty("assets")]
        public List<TxAssetResponse> Assets { get; set; } = new List<TxAssetResponse>();

        [JsonProperty("permissions")]
        public List<object> Permissions { get; set; } = new List<object>();
    }
}
