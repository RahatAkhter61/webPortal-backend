using CORE_API.Services.SRCC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models.CardActivation
{
    public class request_card_activation
    {
        [JsonProperty("header")]
        public CardActivationHeader header { get; set; }
        [JsonProperty("body")]
        public CardActivationBody body { get; set; }
    }
}