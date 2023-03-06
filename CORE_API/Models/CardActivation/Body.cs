using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models.CardActivation
{
    public class Body
    {
        public string application_name { get; set; }
        public string date_time { get; set; }
        public string status { get; set; } // success
        public string error_code { get; set; } // 000
        public string error_description { get; set; }
        public string transaction_ref_id { get; set; }
    }
}