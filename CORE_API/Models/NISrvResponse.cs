using CORE_API.Models.CardActivation;
using CORE_API.Services.SRCC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models
{
    public class NISrvResponse
    {

        [JsonProperty("response_card_activation")]
        public response_card_activation ResponseCardActivation { get; set; }

    }
}