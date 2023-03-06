using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models.CardActivation
{
    public class CardActivationHeader
    {
        public string msg_id { get; set; }
        public string msg_type { get; set; }
        public string msg_function { get; set; }
        public string src_application { get; set; }
        public string target_application { get; set; }
        public string timestamp { get; set; }
        public string tracking_id { get; set; }
        public string bank_id { get; set; }
    }
}