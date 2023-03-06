using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models.Transaction
{
    public class Transaction
    {
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }

    }

    public class DepositSlipTrsansactionModel : Transaction 
    {
        public int? CompanyId { get; set; }
        public string TransactionDiscription { get; set; }
        public string TransactionReference { get; set; }
        public int SlipId { get; set; }
        public int? TransactionTypeId { get; set; }
        public string TransactionDescription { get; set; }
    }
    public class ChargeTransaction : Transaction
    {
        public int ChargeType { get; set; }
        public decimal? TotalAmount { get; set; }
        public string TransactionType { get; set; }
        public int? CompanyId { get; set; }
        public string TransactionDiscription { get; set; }
        public string TransactionReference { get; set; }
        public int? Sifid { get; set; }
        public bool? IsDeducted { get; set; }
        public decimal? ChargeAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? VatPercent { get; set; }
        public int? CreatedBy { get; set; }
        public int? TransactionId { get; set; }
        public int? CashLoadId { get; set; }
        public int? ProductId { get; set; }
    }

    public struct RefundTransactionModel
    {
        public int Id { get; set; }
        public int DebitId { get; set; }
        public DateTime? DataOfFailure { get; set; }
        public decimal? RefundAmount { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public int DebitRefundStatusId { get; set; }
        public int RefundApprovalStatusId { get; set; }
        public int OldRefundApprovalStatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string TransReference { get; set; }
        public string ProofImagePath { get; set; }
        public int ModifiedBy { get; set; }
    }

}