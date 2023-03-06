using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.SMAM
{
    public class ProjectLocationService : CastService<SMAM_CampEmp_ST>
    {

        public ProjectLocationService()
        {

        }

        public DTO GetCamp(int? CompanyId)
        {
            var obj = CompanyId!=null? new BLInfo<SMAM_Camp_ST>().GetQuerable<SMAM_Camp_ST>().Where(a => a.CampId != 0 && a.CompanyId == CompanyId).ToList():
                 new BLInfo<SMAM_Camp_ST>().GetQuerable<SMAM_Camp_ST>().Where(a => a.CampId != 0).ToList(); 
            var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != null).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.CampId,
                           sa.Description,
                           sa.Title,
                           sa.CountryId,
                           sa.CompanyId,
                           Country = sa.SMAM_Country_ST != null ? sa.SMAM_Country_ST.Name : "",
                           State = sa.SMAM_States_ST != null ? sa.SMAM_States_ST.StateName : "",
                           Company = sa.SMAM_CompanyMst_ST != null ? sa.SMAM_CompanyMst_ST.Title : "",
                           sa.Address,
                           sa.StateId,
                           CreatedByName = objUser.Where(a => a.UserId == sa.CreatedBy).FirstOrDefault().UserName,
                           sa.ModifiedBy


                       }).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO GetCampById(int CampId)
        {
            var obj = new BLInfo<SMAM_Camp_ST>().GetQuerable<SMAM_Camp_ST>().Where(a => a.CampId == CampId).ToList();
            var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != null).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.CampId,
                           sa.Description,
                           sa.Title,
                           sa.CountryId,
                           sa.CompanyId,
                           Country = sa.SMAM_Country_ST != null ? sa.SMAM_Country_ST.Name : "",
                           State = sa.SMAM_States_ST != null ? sa.SMAM_States_ST.StateName : "",
                           Company = sa.SMAM_CompanyMst_ST != null ? sa.SMAM_CompanyMst_ST.Title : "",
                           sa.Address,
                           sa.StateId,
                           CreatedByName = objUser.Where(a => a.UserId == sa.CreatedBy).FirstOrDefault().UserName,
                           sa.ModifiedBy


                       }).FirstOrDefault();
            return this.SuccessResponse(lst);
        }


        public DTO GetEmployeesbycamp(int CampId)
        {
            var obj = new BLInfo<SMAM_CampEmp_ST>().GetQuerable<SMAM_CampEmp_ST>().Where(a => a.CampId == CampId).ToList();
            var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.CampEmpId,
                           sa.CampId,
                           sa.EffFrom,
                           sa.EffTo,
                           sa.EmpId,
                           sa.IsActive,
                           EmpStatusId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.EmpStatusId : 0,
                           EmpTypeId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.EmpTypeId : 0,
                           CompanyId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.CompanyId : 0,
                           BankId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.BankId : 0,
                           NationalityId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.NationalityId : 0,
                           PositionId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.PositionId : 0,
                           IBAN = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.IBAN : "",
                           PersonalNo = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.PersonalNo : "",
                           EmpCode = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.EmpCode : "",
                           DisplayName = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DisplayName : "",
                           FirstName = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.FirstName : "",
                           MiddleName = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.MiddleName : "",
                           LastName = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.LastName : "",
                           Employeename = sa.HRMS_EmployeeMst_ST != null ? (sa.HRMS_EmployeeMst_ST.FirstName + " " + sa.HRMS_EmployeeMst_ST.MiddleName + " " + sa.HRMS_EmployeeMst_ST.LastName) : "",
                           GenderText = sa.HRMS_EmployeeMst_ST != null ? (sa.HRMS_EmployeeMst_ST.Gender == 'M' || sa.HRMS_EmployeeMst_ST.Gender == 'm') ? "Male" : (sa.HRMS_EmployeeMst_ST.Gender == 'F' || sa.HRMS_EmployeeMst_ST.Gender == 'f') ? "Female" : "Unknown" : "",
                           Gender = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.Gender : null,
                           DOB = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DOB : null,
                           DOJ = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DOJ : null,
                           CreatedDateString = sa.HRMS_EmployeeMst_ST != null ? (sa.HRMS_EmployeeMst_ST.CreatedOn != null ? sa.HRMS_EmployeeMst_ST.CreatedOn.Value.ToString("dd/MM/yyyy") : null) : "",
                           DOBString = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DOB.Value.ToString("dd/MM/yyyy") : null,
                           DOJString = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DOJ.Value.ToString("dd/MM/yyyy") : null,
                           MaritalStatus = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.MaritalStatus : null,
                           CreatedBy = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.CreatedBy : null,
                           CreatedByName = sa.HRMS_EmployeeMst_ST != null ? (objUser.Where(a => a.UserId == sa.HRMS_EmployeeMst_ST.CreatedBy).FirstOrDefault()?.UserName) : ""

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO SubmitProjectLocation(IList<SMAM_CampEmp_ST> obj,int CampId,DateTime EffFrom,DateTime EffTo)
        {
            try
            {
                foreach (var item in obj)
                {
                    if (item.CampEmpId != 0)
                        GetByPrimaryKey(item.CampEmpId);
                    if (item.CampEmpId == 0 || item.CampEmpId == null)
                        PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                        New();

                    
                    Current.CampId = CampId;
                    Current.EmpId = item.EmpId;
                    Current.IsActive = item.IsActive;
                    DateTime? EffeFrom = EffFrom;
                    DateTime? EffeTo = EffTo;
                    Common.DateHandler(ref EffeFrom);
                    Common.DateHandler(ref EffeTo);
                    Current.EffFrom = EffeFrom;
                    Current.EffTo = EffTo;
                    Current.IsActive = item.IsActive;

                    Save();
                }

                return this.SuccessResponse("Saved Succesfully");


            }

            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return BadResponse(obj);
                else
                    return BadResponse(Ex.Message);
            }
        }

        public DTO DeleteProjectLocation(long Id)
        {
            try
            {
                GetByPrimaryKey(Id);
                if (Current != null)
                    PrimaryKeyValue = Id;

                Delete();
                return this.SuccessResponse("Deleted Successfully.");


            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }
    }


}
