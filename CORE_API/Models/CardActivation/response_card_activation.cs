using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models.CardActivation
{
    public class response_card_activation
    {
        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("exception_details")]
        public Body Body { get; set; }
    }
}