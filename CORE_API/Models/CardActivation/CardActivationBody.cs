using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models.CardActivation
{
    public class CardActivationBody
    {
        public string card_no { get; set; }
        public string pin { get; set; }
        public string request_type { get; set; }
        public string card_type { get; set; }
        public string FIID { get; set; }
    }
}