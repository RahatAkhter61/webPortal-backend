using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace CORE_API.Models.AdvanceSalary
{
    public class AdvanceSalaryModel
    {
        public int advanceSalaryId { get; set; }
        public decimal? RequestAmount { get; set; }
        public decimal? AmountFee { get; set; }
        public int statustype { get; set; }
        public int CreatedBy { get; set; }
        public int RoleId { get; set; }
        public int LoanPayCompanyId { get; set; }

        public string ReasonofRejection { get; set; }

    }
    public class EmployeeAdvanceEligibiltyResponseModel
    {
        public string EmpName { get; set; }
        public string EmpCrn { get; set; }
        public string EmiratesId { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public string MobileNo { get; set; }
        public DateTime? CompanyCreatedDate { get; set; }
        public DateTime? CardCreatedDate { get; set; }
        public decimal? AverageSalary { get; set; }
        public string Message { get; set; }
        public int EmpId { get; set; }
        public string CustomerNo { get; set; }
        public decimal? EligibleAMount { get; set; }
        public string Eligible { get; set; }

        public DateTime? DueDate { get; set; }
        public int totalRecords { get; set; }
        public List<LoanBracketDetails> loanBrackets { get; set; }
    }
    public class SalaryDetail
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public decimal? NetSalary { get; set; }
        public DateTime? SalaryDate { get; set; }
    }
    public class JsonResultModel
    {
        public object Data;
        public bool IsSuccess;
        public HttpStatusCode StatusCode { get; set; }
        public string Responce { get; set; }
    }
    public class LoanBracketDetails
    {
        public decimal? FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public decimal? Fees { get; set; }
        public decimal? VATPercent { get; set; }
        public decimal? VATAmount { get; set; }
    }

    public class AdditinoalQuestionAdvanceSalaryModel
    {

        public int AdvanceSalaryDetailInfoId { get; set; }
        public int AdvanceSalaryId { get; set; }
        public string NameofSpouse { get; set; }
        public string CellNumberSpouse { get; set; }
        public string UAEfriend { get; set; }
        public string CellNumberUaeFriend { get; set; }
        public int PurposeofLoan { get; set; }
        public int JobLeavingPlan { get; set; }
        public int HomeVisitPlan { get; set; }
        public string OtherPurposeofloan { get; set; }
        public string OtherJobleaving { get; set; }

        public int CreatedBy { get; set; }

    }
}