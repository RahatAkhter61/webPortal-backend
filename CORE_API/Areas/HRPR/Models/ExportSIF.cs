using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Areas.HRPR.Models
{
    public class ExportSIF
    {

        public int EmpID { get; set; }
        public string RecordType { get; set; }
        public string PersonalNo { get; set; }
        public string RoutingCode { get; set; }
        public string IBAN { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int SalaryDays { get; set; }
        public string Basic { get; set; }
        public string Allowances { get; set; }
        public string LeaveDays { get; set; }
    }

    public class UpdatePrintedStatus
    {
        public int? UserId { get; set; }
        public int ExportId { get; set; }
        public string WalletID { get; set; }
        public int EmpId { get; set; }
        public string CardStatus { get; set; }
        public int? ExecutivePerson { get; set; }

    }
    public class AssignToExecutive
    {
        public int? EmpId { get; set; }
        public int? UserId { get; set; }
        public int ExportId { get; set; }
        public string WalletID { get; set; }
        public string CardStatus { get; set; }
        public int ExecutivePerson { get; set; }
        public DateTime? ExecutiveDate { get; set; }

    }

    public class updateActivateStatusModel
    {
        public string UserName { get; set; }
        public string WalletId { get; set; }
        public string CardNo { get; set; }
            
    }




}