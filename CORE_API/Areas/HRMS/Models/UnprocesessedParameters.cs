using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Areas.HRMS.Models
{
    public class UnprocesessedParameters
    {

        public int? CreatedBy { get; set; }
        public int? CompanyID { get; set; }
        public int? branchId { get; set; }
        public int? departmentId { get; set; }
        public int? empId { get; set; }
        public DateTime? SifDate { get; set; }
        public decimal? TotalEmployees { get; set; }
        public decimal? TotalSalary { get; set; }

        public string employeename { get; set; }


        public UnprocesessedParameters()
        {
            this.CompanyID = 0;
            this.branchId = 0;
            this.departmentId = 0;
            this.empId = 0;
            this.SifDate = DateTime.Now;
            this.TotalEmployees = 0;
            this.TotalSalary = 0;
            this.employeename = "";
        }

        public class UpdateEmployeeArchive
        {
            public int UserId { get; set; }
            public int? EmpId { get; set; }
            public int? CompanyId { get; set; }
            public int? ProductId { get; set; }
            public string EmpCode { get; set; }
            public string WalletId { get; set; }
            public string CustomerId { get; set; }
            public bool? IsActive { get; set; }
            public bool? IsArchive { get; set; }

            public bool? CardActivated { get; set; }

        }


    }

}