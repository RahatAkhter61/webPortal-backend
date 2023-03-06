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

namespace CORE_API.Services.HRMS
{
    public class EmployeeAttendanceService : CastService<HRMS_EmpAttendance_TR>
    {

        public EmployeeAttendanceService()
        {

        }

        public DTO GetEmployeeAttendance()
        {
            //var obj = new BLInfo<SMAM_Camp_ST>().GetQuerable<SMAM_Camp_ST>().Where(a => a.CampId != 0).ToList();
            //var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != null).ToList();
            //var lst = (from sa in obj
            //           select new
            //           {
            //               sa.CampId,
            //               sa.Description,
            //               sa.Title,
            //               sa.CountryId,
            //               sa.CompanyId,
            //               Country = sa.SMAM_Country_ST != null ? sa.SMAM_Country_ST.Name : "",
            //               State = sa.SMAM_States_ST != null ? sa.SMAM_States_ST.StateName : "",
            //               Company = sa.SMAM_CompanyMst_ST != null ? sa.SMAM_CompanyMst_ST.Title : "",
            //               sa.Address,
            //               sa.StateId,
            //               CreatedByName = objUser.Where(a => a.UserId == sa.CreatedBy).FirstOrDefault().UserName,
            //               sa.ModifiedBy,
            //               EstId = sa.SMAM_CompanyMst_ST != null ? sa.SMAM_CompanyMst_ST.EstId : "",
            //               //EstId = sa. != null ? sa.SMAM_CompanyMst_ST.EstId : ""


            //           }).ToList();
            //return this.SuccessResponse(lst);


            var obj = new BLInfo<HRMS_EmpAttendance_TR>().GetQuerable<HRMS_EmpAttendance_TR>().Where(a => a.EmpAttId != 0).ToList();
            // var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != null).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.EmpAttId,
                           EmpCode = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.EmpCode : "",
                           EstId = sa.SMAM_CompanyMst_ST != null ? sa.SMAM_CompanyMst_ST.EstId : "",
                           CompanyId = sa.SMAM_CompanyMst_ST != null ? sa.SMAM_CompanyMst_ST.CompanyId : 0,
                           EmpId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.EmpId : 0,
                           sa.Present,
                           sa.Leaves,
                           sa.AttendDate,
                           sa.CreatedBy

                       }).ToList();
            return this.SuccessResponse(lst);
        }

        //public DTO GetCampById(int CampId)
        //{
        //    var obj = new BLInfo<SMAM_Camp_ST>().GetQuerable<SMAM_Camp_ST>().Where(a => a.CampId == CampId).ToList();
        //    var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != null).ToList();
        //    var lst = (from sa in obj
        //               select new
        //               {
        //                   sa.CampId,
        //                   sa.Description,
        //                   sa.Title,
        //                   sa.CountryId,
        //                   sa.CompanyId,
        //                   Country = sa.SMAM_Country_ST != null ? sa.SMAM_Country_ST.Name : "",
        //                   State = sa.SMAM_States_ST != null ? sa.SMAM_States_ST.StateName : "",
        //                   Company = sa.SMAM_CompanyMst_ST != null ? sa.SMAM_CompanyMst_ST.Title : "",
        //                   sa.Address,
        //                   sa.StateId,
        //                   CreatedByName = objUser.Where(a => a.UserId == sa.CreatedBy).FirstOrDefault().UserName,
        //                   sa.ModifiedBy


        //               }).FirstOrDefault();
        //    return this.SuccessResponse(lst);
        //}


        //public DTO GetEmployeesbycamp(int CampId)
        //{
        //    var obj = new BLInfo<HRMS_EmpCamp_ST>().GetQuerable<HRMS_EmpCamp_ST>().Where(a => a.CampId == CampId).ToList();
        //    var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != null).ToList();
        //    var lst = (from sa in obj
        //               select new
        //               {
        //                   sa.EmpCampId,
        //                   sa.CampId,
        //                   sa.EffFrom,
        //                   sa.EffTo,
        //                   sa.EmpId,
        //                   sa.IsActive,
        //                   EmpStatusId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.EmpStatusId : 0,
        //                   EmpTypeId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.EmpTypeId : 0,
        //                   CompanyId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.CompanyId : 0,
        //                   BankId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.BankId : 0,
        //                   NationalityId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.NationalityId : 0,
        //                   PositionId = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.PositionId : 0,
        //                   IBAN = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.IBAN : "",
        //                   PersonalNo = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.PersonalNo : "",
        //                   EmpCode = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.EmpCode : "",
        //                   DisplayName = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DisplayName : "",
        //                   FirstName = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.FirstName : "",
        //                   MiddleName = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.MiddleName : "",
        //                   LastName = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.LastName : "",
        //                   Employeename = sa.HRMS_EmployeeMst_ST != null ? (sa.HRMS_EmployeeMst_ST.FirstName + " " + sa.HRMS_EmployeeMst_ST.MiddleName + " " + sa.HRMS_EmployeeMst_ST.LastName) : "",
        //                   GenderText = sa.HRMS_EmployeeMst_ST != null ? (sa.HRMS_EmployeeMst_ST.Gender == 'M' || sa.HRMS_EmployeeMst_ST.Gender == 'm') ? "Male" : (sa.HRMS_EmployeeMst_ST.Gender == 'F' || sa.HRMS_EmployeeMst_ST.Gender == 'f') ? "Female" : "Unknown" : "",
        //                   Gender = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.Gender : null,
        //                   DOB = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DOB : null,
        //                   DOJ = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DOJ : null,
        //                   CreatedDateString = sa.HRMS_EmployeeMst_ST != null ? (sa.HRMS_EmployeeMst_ST.CreatedOn != null ? sa.HRMS_EmployeeMst_ST.CreatedOn.Value.ToString("dd/MM/yyyy") : null) : "",
        //                   DOBString = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DOB.Value.ToString("dd/MM/yyyy") : null,
        //                   DOJString = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.DOJ.Value.ToString("dd/MM/yyyy") : null,
        //                   MaritalStatus = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.MaritalStatus : null,
        //                   CreatedBy = sa.HRMS_EmployeeMst_ST != null ? sa.HRMS_EmployeeMst_ST.CreatedBy : null,
        //                   CreatedByName = sa.HRMS_EmployeeMst_ST != null ? (objUser.Where(a => a.UserId == sa.HRMS_EmployeeMst_ST.CreatedBy).FirstOrDefault().UserName) : ""

        //               }).ToList();
        //    return this.SuccessResponse(lst);
        //}

        public DTO UpdateEmployeeAttendance(IList<HRMS_EmpAttendance_TR> obj)
        {
            try
            {
                foreach (var item in obj)
                {
                    if (item.EmpAttId != 0)
                        GetByPrimaryKey(item.EmpAttId);
                    if (item.EmpAttId == 0 || item.EmpAttId == null)
                        PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                    {
                        New();
                        Current.CreatedBy = item.CreatedBy;
                        Current.CreatedOn = item.CreatedOn;
                    }


                    Current.EmpId = item.EmpId;
                    Current.CompanyId = item.CompanyId;
                    DateTime? AttendDate = item.AttendDate;
                    //DateTime? EffTo = item.EffTo;
                    //Common.DateHandler(ref EffFrom);
                    Common.DateHandler(ref AttendDate);
                    Current.AttendDate = AttendDate;
                    Current.Present = item.Present;
                    Current.Leaves = item.Leaves;


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

        DateTime AttendDate;
        public List<string> SaveEmployeeAttendance(DataTable obj, int CreatedBy)
        {

            List<string> lsterror = new List<string>();
            ////await this.Runasync(() => { });
            try
            {

                if (PrimaryKeyValue == null)
                    New();
                HRMS_EmpAttendance_TR objDet;
                int count = 0;

                foreach (DataRow item in obj.Rows)
                {
                    objDet = new HRMS_EmpAttendance_TR();

                    string EstId = "";
                    //Validations 
                    if (item["EstId"] != "")
                    {
                        EstId = item["EstId"].ToString();
                    }



                    if (item["EstId"] != "")
                    {
                        var objcompany = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(sa => sa.EstId == EstId).FirstOrDefault();//.NewIfEmpty().CompanyId;
                        if (objcompany != null)
                        {
                            int companyId = objcompany.CompanyId;
                            objDet.CompanyId = companyId;
                        }
                        else
                            lsterror.Add("Row # : " + (count + 1) + " Invalid Est Id.");
                    }
                    else
                        lsterror.Add("Row # : " + (count + 1) + " Required : Est Id.");

                    if (item["EmpCode"] != "")
                    {
                        string EmpCode = item["EmpCode"].ToString();
                        var objemployee = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(sa => sa.EmpCode == EmpCode).FirstOrDefault();//.NewIfEmpty().CompanyId;
                        if (objemployee != null)
                        {
                            int EmpId = objemployee.EmpId;
                            objDet.EmpId = EmpId;
                        }
                        else
                            lsterror.Add("Row # : " + (count + 1) + " Invalid Employee.");
                    }
                    else
                        lsterror.Add("Row # : " + (count + 1) + " Required : Emp Code.");


                  

                    if (item["Date"] != "")
                    {
                        //DateTime dt =DateTime.Now.Date;;

                        AttendDate = Convert.ToDateTime(item["Date"]);

                        objDet.AttendDate = AttendDate;

                        if (item["EmpCode"] != "")
                        {
                            string EmpCode = item["EmpCode"].ToString();
                            var objemployee = new BLInfo<HRMS_EmpAttendance_TR>().GetQuerable<HRMS_EmpAttendance_TR>().Where(sa => sa.HRMS_EmployeeMst_ST.EmpCode == EmpCode && sa.AttendDate == AttendDate).FirstOrDefault();//.NewIfEmpty().CompanyId;
                            if (objemployee != null)
                            {
                                lsterror.Add("Row # : " + (count + 1) + " Employee attendance of this month is already marked.");
                            }
                        }
                            


                    }
                    else
                        lsterror.Add("Row # : " + (count + 1) + " Required : Date.");

                    if (item["Present"] != "")
                    {
                        decimal present = decimal.Parse(item["Present"].ToString());
                        objDet.Present = present;
                    }
                    else
                        lsterror.Add("Row # : " + (count + 1) + " Required : Present.");

                    if (item["Leaves"] != "")
                    {
                        decimal Leaves = decimal.Parse(item["Leaves"].ToString());
                        objDet.Leaves = Leaves;
                    }
                    else
                        lsterror.Add("Row # : " + (count + 1) + " Required : Leaves.");


                    objDet.CreatedBy = CreatedBy;

                    CurrentBulk.Add(objDet);
                    count++;
                }
                if (lsterror.Count == 0)
                {
                    SaveBulk();
                   
                }
                else
                    return lsterror;
            }
            catch (Exception Ex)
            {

                lsterror.Add(Ex.Message);
            }

            return lsterror;
        }

        public DTO DeleteAttendance(long Id)
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
