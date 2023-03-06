using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models
{
    public class SMSResponse
    {
        public int ErrorCode { get; set; }
        public string Description { get; set; }
        public string OriginatingAddress { get; set; }
        public string DestinationAddress { get; set; }

        public int MessageCount { get; set; }


    }
}