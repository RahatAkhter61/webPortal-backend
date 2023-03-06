using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.HRMS
{
    public class EmployeeSalaryService : CastService<HRMS_EmpSalary_ST>
    {

        public EmployeeSalaryService()
        {

        }

        public DTO GetEmployeeSalary(int EmpId)
        {
            try
            {
                var objempsalary = new BLInfo<HRMS_EmpSalary_ST>().GetQuerable<HRMS_EmpSalary_ST>().Where(a => a.EmpId == EmpId).ToList();
                var lst = (from sa in objempsalary
                           select new
                           {
                               sa.EmpId,
                               sa.BasicSalary,
                               sa.Allowances,
                               sa.Travel,
                               sa.Housing,
                               sa.EffFrom,
                               sa.EffTo
                           }).ToList();
                return SuccessResponse(lst);
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }
        public DTO SubmitSalary(IList<HRMS_EmpSalary_ST> item,int EmpId)
        {
            try
            {

                foreach (var obj in item)
                {
                    if (obj.EmpSalId != null)
                        GetByPrimaryKey(obj.EmpSalId);
                    if (obj.EmpSalId == 0 || obj.EmpSalId == null)
                        PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                    {
                        New();
                        Current.CreatedBy = obj.CreatedBy;
                        Current.CreatedOn = DateTime.Now;
                    }

                    Current.EmpId = EmpId;
                    Current.BasicSalary = obj.BasicSalary;
                    Current.Travel = obj.Travel;
                    Current.Housing = obj.Housing;
                    Current.Allowances = obj.Allowances;

                    DateTime? EffFrom = obj.EffFrom;
                    DateTime? EffTo = obj.EffTo;
                    Common.DateHandler(ref EffFrom);
                    Common.DateHandler(ref EffTo);
                    Current.EffFrom = EffFrom;
                    Current.EffTo = EffTo;

                    Save();

                }
                return this.SuccessResponse("Saved Succesfully");
            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return BadResponse(Errors);
                else
                    return BadResponse(Ex.Message);
            }


        }


    }
}
