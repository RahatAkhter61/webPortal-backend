using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Areas.PCMS.Model
{
    public class PCMS_Cashload_model
    {
        public int Campanyid { get; set; }
        public int Productid { get; set; }
        public bool Isfreezone { get; set; }
        public int TotalRecords { get; set; }
        public int TotalAmount { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; }
        public DateTime Modifyon { get; set; }
        public DateTime Createdon { get; set; }

       public List<PCMS_Cashload_Detail> Cashload_Detail;

    }



}