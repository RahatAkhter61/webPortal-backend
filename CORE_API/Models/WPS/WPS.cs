using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models.WPS
{
    public class WPSupdatetopupstatus
    {
        public int PAFMasterId { get; set; }
        public int PAFDetailId { get; set; }
        public string Status { get; set; }
        public int Updatedby { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public class proccedClaimedModel
    {
        public int Id { get; set; }
        public decimal? Returned_Amount { get; set; }
        public int CompanyId { get; set; }
        public string Agent_Referrence { get; set; }
        public int UserId { get; set; }

    }
}