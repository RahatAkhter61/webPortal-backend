using CORE_API.Models.CardActivation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models.CardActivation
{
    public class PostObject
    {
        [JsonProperty("NISrvRequest")]
        public NISrvRequest NISrvRequest { get; set; }
    }
}
public class NISrvRequest
{
    [JsonProperty("request_card_activation_set_pin")]
    public request_card_activation request_card_activation { get; set; }

}