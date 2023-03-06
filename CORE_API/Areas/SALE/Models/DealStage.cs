using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Areas.SALE.Models
{
    public class DealStage
    {
        public int CategoryId { get; set; }
        public string StageCategory { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? Createdon { get; set; } 
    }
}