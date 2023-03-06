using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Models.ComapnyWhiteListing
{
    public class CompanyWhileListing
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int tmlStatus { get; set; }

        public List<CompanyLoanBracketModel> CompanyLoanBracketModel { get; set; }
    }

    public class CompanyLoanBracketModel
    {
        public int? BracketId { get; set; }
        public decimal? FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public decimal? Fees { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class CompanyDeposite
    {
        public int SlipId { get; set; }
        public int Status { get; set; }
        public int CompanyId { get; set; }
        public int UploadedBy { get; set; }
        public decimal? DepositeAmount { get; set; }

    }

    public class ApprovalRequestModel
    {
        public string status { get; set; }
        public string type { get; set; }
        public string approvalId { get; set; }

    }

    public class BaseModel
    {
        public string error { get; set; }
        public string data { get; set; }
    }

}