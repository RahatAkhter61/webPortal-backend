using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CORE_MODELS;
namespace CORE_API.Areas.SMAM.Models
{
    public class CompanyModel
    {

        //  public SMAM_CompanyMst_ST CompanyMst;
        public List<SMAM_CompanyDocs_ST> CompanyDoc;
        public List<SMAM_CompanyCharges_ST> CompanyCharges;
    }


    public class TransferAmountToCompany
    {
        public int UserId { get; set; }
        public int FromCompanyId { get; set; }
        public int ToCompanyId { get; set; }
        public decimal? FromCompanybalnce { get; set; }
        public decimal? ToCompanybalnce { get; set; }
        public decimal? Transferamount { get; set; }
        public string Description { get; set; }
    }

    public class TransferOfFundsRequestModel 
    {
        public int? CompanyId { get; set; }
       public List<TransferDetail> TransferDetails { get; set; }
    }

    public class TransferDetail 
    {
        public int? TransferId { get; set; }
        public int? ToCompanyId { get; set; }
        public decimal? Amount { get; set; }
        public int CreatedBy { get; set; }
    }

}