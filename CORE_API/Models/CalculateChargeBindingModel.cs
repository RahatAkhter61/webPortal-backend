using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models
{
    public class CalculateChargeBindingModel
    {
        public int? ChargeId { get; set; }
        public int? TotalRecords { get; set; }
        public int? CompanyId { get; set; }
        public string IsDeducted { get; set; }
        public int? ProductId { get; set; }


    }

    public class ChargesResponseModel
    {
        public decimal? VatAmount { get; set; }
        public decimal? ChargeAmount { get; set; }
        public decimal? VatPercent { get; set; }
        public decimal? Currentbalance { get; set; }
        public decimal? totalChargeAmount { get; set; }
        public int? CompanyChargeId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
    }
    public class ManualChargesModel 
    {
        public int? CompanyId { get; set; }
        public int? ProductId { get; set; }
        public int? CompanyChargeId { get; set; }
        public int? Quantity { get; set; }
        public int? SifId { get; set; }
    }

    public class ManualChargesTransactionModel : ManualChargesModel
    {
        public decimal? VatAmount { get; set; }
        public decimal? ChargeAmount { get; set; }
        public decimal? VatPercent { get; set; }
        public decimal? totalChargeAmount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool IsDeducted { get; set; }
        public int? CreatedBy { get; set; }
        public string Description { get; set; }

    }

    public class CalculateMultipleChargeBindingModel
    {
        public List<int?> ChargeIds { get; set; }
        public int? TotalRecords { get; set; }
        public int? CompanyId { get; set; }
        public string IsDeducted { get; set; }
        public int? ProductId { get; set; }

    }

    public class SifNakClaimedModel
    {
        public int ExpSifID { get; set; }
        public decimal? ClaimedAmount { get; set; }

    }

}