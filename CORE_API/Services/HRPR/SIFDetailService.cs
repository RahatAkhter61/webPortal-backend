using ContextMapper;
using CORE_API.Areas.HRMS.Models;
using CORE_API.Areas.HRPR.Models;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.HRPR
{
    public class SIFDetailService : CastService<HRPR_SifDetail_TR>
    {
        public SIFDetailService()
        {

        }

        public DTO GetUnProcessedEmployees(int CompanyId, int DeptId, int StationId, int EmpId, DateTime dt)
        {
            try
            {
                var _ProcessedEmployee = new BLInfo<HRPR_SifDetail_TR>().GetQuerable<HRPR_SifDetail_TR>().Where(x => x.StartDate.Value.Month == dt.Month
                                                                                   && x.StartDate.Value.Year == dt.Year)
                                                           .Select(x => x.EmpId).ToList();
                var objempmst = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>();
                var objempsal = new BLInfo<HRMS_EmpSalary_ST>().GetQuerable<HRMS_EmpSalary_ST>();
                var objpos = new BLInfo<HRMS_Positions_ST>().GetQuerable<HRMS_Positions_ST>();
                var objbank = new BLInfo<SMAM_Banks_ST>().GetQuerable<SMAM_Banks_ST>();



                var obj = (from e in objempmst
                           join S in objempsal on e.EmpId equals S.EmpId
                           join p in objpos on e.PositionId equals p.PositionId
                           join b in objbank on e.BankId equals b.BankId
                           where e.CompanyId == CompanyId
                                              & p.StationId.Value == (StationId > 0 ? StationId : p.StationId.Value)
                                              & p.DeptId == (DeptId > 0 ? DeptId : p.DeptId.Value)
                                               & e.EmpId == (EmpId > 0 ? EmpId : e.EmpId)
                                              //&  e.EmpId == (_EmpID > 0 ? _EmpID : Sifdetails.EmpId)
                                              & !_ProcessedEmployee.Contains(e.EmpId)

                           select new
                           {
                               Empid = e.EmpId,
                               EmpCode = e.EmpCode,
                               DisplayName = e.DisplayName,
                               BasicSalary = S.BasicSalary.Value,
                               Allownce = S.Housing.Value + S.Travel.Value + S.Allowances.Value,
                               IBAN = e.IBAN,
                               Routing = b.RoutingCode,
                           }
                                               ).ToList();



                return this.SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                if (Errors.Count() > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }

        public DTO GetUnProcessedEmployeesData(UnprocesessedParameters objunprocess)
        {
            try
            {

                DateTime? Date = objunprocess.SifDate;
                Common.DateHandler(ref Date);
                var _ProcessedEmployee = new BLInfo<HRPR_SifDetail_TR>().GetQuerable<HRPR_SifDetail_TR>().Where(x => x.StartDate.Value.Month == Date.Value.Month
                                                                                   && x.StartDate.Value.Year == Date.Value.Year)
                                                           .Select(x => x.EmpId).ToList();
                var objempmst = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().ToList();
                var objempsal = new BLInfo<HRMS_EmpSalary_ST>().GetQuerable<HRMS_EmpSalary_ST>().ToList();
                var objpos = new BLInfo<HRMS_Positions_ST>().GetQuerable<HRMS_Positions_ST>().ToList();
                var objbank = new BLInfo<SMAM_Banks_ST>().GetQuerable<SMAM_Banks_ST>().ToList();
                var objempattendance = new BLInfo<HRMS_EmpAttendance_TR>().GetQuerable<HRMS_EmpAttendance_TR>().Where(x => x.AttendDate.Value.Month == Date.Value.Month
                                                                                   && x.AttendDate.Value.Year == Date.Value.Year).ToList();

                DateTime startDate = new DateTime(Date.Value.Year, Date.Value.Month, 1).Date;
                var endDate = startDate.AddMonths(1).AddDays(-1).Date;
                var obj = (from e in objempmst
                           join S in objempsal on e.EmpId equals S.EmpId
                           join p in objpos on e.PositionId equals p.PositionId
                           join b in objbank on e.BankId equals b.BankId
                           join ea in objempattendance on e.EmpId equals ea.EmpId
                           where e.CompanyId == objunprocess.CompanyID
                                               //& p.StationId.Value == (objunprocess.branchId > 0 ? objunprocess.branchId : p.StationId.Value)
                                               //  & p.DeptId == (objunprocess.departmentId > 0 ? objunprocess.departmentId : p.DeptId.Value)
                                               & e.EmpId == (objunprocess.empId > 0 ? objunprocess.empId : e.EmpId)
                                              //&  e.EmpId == (_EmpID > 0 ? _EmpID : Sifdetails.EmpId)
                                              & !_ProcessedEmployee.Contains(e.EmpId)

                           select new
                           {
                               Empid = e.EmpId,
                               EmpCode = e.EmpCode,
                               e.PersonalNo,
                               DisplayName = e.DisplayName,
                               StartDate = startDate,
                               EndDate = endDate,
                               BasicSalaryText = String.Format("{0:0.###}", ((S.BasicSalary / (DateTime.DaysInMonth(objunprocess.SifDate.Value.Year, objunprocess.SifDate.Value.Month))) * (ea.Present + ea.Leaves))),
                               AllowanceText = String.Format("{0:0.###}", (((S.Housing.Value + S.Travel.Value + S.Allowances.Value) / ((DateTime.DaysInMonth(objunprocess.SifDate.Value.Year, objunprocess.SifDate.Value.Month)))) * (ea.Present + ea.Leaves))),
                               BasicSalary = Convert.ToDecimal(string.Format("{0:F2}", (S.BasicSalary / (DateTime.DaysInMonth(objunprocess.SifDate.Value.Year, objunprocess.SifDate.Value.Month))) * (ea.Present + ea.Leaves))),
                               Allowance = Convert.ToDecimal(string.Format("{0:F2}", ((S.Housing.Value + S.Travel.Value + S.Allowances.Value) / ((DateTime.DaysInMonth(objunprocess.SifDate.Value.Year, objunprocess.SifDate.Value.Month)))) * (ea.Present + ea.Leaves))),
                               IBAN = e.IBAN,
                               Routing = b.RoutingCode,
                               LeaveDays = ea.Leaves,
                               IsAllow = false
                           }
                                               ).Select(sh => new
                                               {
                                                   sh.Empid,
                                                   sh.EmpCode,
                                                   sh.PersonalNo,
                                                   sh.DisplayName,
                                                   sh.StartDate,
                                                   sh.EndDate,
                                                   Basic = sh.BasicSalary,
                                                   Allowances = sh.Allowance,
                                                   sh.IBAN,
                                                   RoutingCode = sh.Routing,
                                                   sh.LeaveDays,
                                                   sh.IsAllow,
                                                   NetSalary = Convert.ToDecimal(string.Format("{0:F2}", (sh.BasicSalary + sh.Allowance)))
                                               }).ToList();

             //   var _CompanyTransactions = new BLInfo<FNGL_Transactions_HI>().GetQuerable<FNGL_Transactions_HI>().Where(x => x.CompanyId == objunprocess.CompanyID && !x.EmpId.HasValue).Sum(x => x.DebitAmount - x.CreditAmount);

                decimal? _CompanyBalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == objunprocess.CompanyID).FirstOrDefault().NewIfEmpty().Balance;
                decimal _TotalBalance = (_CompanyBalance.HasValue ? _CompanyBalance.Value : 0);// + (_CompanyTransactions.HasValue ? _CompanyTransactions.Value : 0);

                Dictionary<object, object> dic = new Dictionary<object, object>();


                dic.Add("Unprocess", obj);
                dic.Add("TotalBalance", _TotalBalance);

                return this.SuccessResponse(dic);
            }
            catch (Exception Ex)
            {

                if (Errors.Count() > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }
       
        public DTO GetUnProcessedEmployeesData(int? CompanyId, int? DeptId, int? StationId, int? EmpId, DateTime dt)
        {
            try
            {


                var _ProcessedEmployee = new BLInfo<HRPR_SifDetail_TR>().GetQuerable<HRPR_SifDetail_TR>().Where(x => x.StartDate.Value.Month == dt.Month
                                                                                   && x.StartDate.Value.Year == dt.Year)
                                                           .Select(x => x.EmpId).ToList();
                var objempmst = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().ToList();
                var objempsal = new BLInfo<HRMS_EmpSalary_ST>().GetQuerable<HRMS_EmpSalary_ST>().ToList();
                var objpos = new BLInfo<HRMS_Positions_ST>().GetQuerable<HRMS_Positions_ST>().ToList();
                var objbank = new BLInfo<SMAM_Banks_ST>().GetQuerable<SMAM_Banks_ST>().ToList();
                var objempattendance = new BLInfo<HRMS_EmpAttendance_TR>().GetQuerable<HRMS_EmpAttendance_TR>().Where(x => x.AttendDate.Value.Month == dt.Month
                                                                                   && x.AttendDate.Value.Year == dt.Year).ToList();

                DateTime startDate = new DateTime(dt.Year, dt.Month, 1).Date;
                var endDate = startDate.AddMonths(1).AddDays(-1).Date;
                var obj = (from e in objempmst
                           join S in objempsal on e.EmpId equals S.EmpId
                           join p in objpos on e.PositionId equals p.PositionId
                           join b in objbank on e.BankId equals b.BankId
                           join ea in objempattendance on e.EmpId equals ea.EmpId
                           where e.CompanyId == CompanyId
                                              & p.StationId.Value == (StationId > 0 ? StationId : p.StationId.Value)
                                & p.DeptId == (DeptId > 0 ? DeptId : p.DeptId.Value)
                                               & e.EmpId == (EmpId > 0 ? EmpId : e.EmpId)
                                              //&  e.EmpId == (_EmpID > 0 ? _EmpID : Sifdetails.EmpId)
                                              & !_ProcessedEmployee.Contains(e.EmpId)

                           select new
                           {
                               Empid = e.EmpId,
                               EmpCode = e.EmpCode,
                               e.PersonalNo,
                               DisplayName = e.DisplayName,
                               StartDate = startDate,
                               EndDate = endDate,
                               BasicSalaryText = String.Format("{0:0.###}", ((S.BasicSalary / (DateTime.DaysInMonth(dt.Year, dt.Month))) * (ea.Present + ea.Leaves))),
                               AllowanceText = String.Format("{0:0.###}", (((S.Housing.Value + S.Travel.Value + S.Allowances.Value) / ((DateTime.DaysInMonth(dt.Year, dt.Month)))) * (ea.Present + ea.Leaves))),
                               BasicSalary = Convert.ToDecimal(string.Format("{0:F2}", (S.BasicSalary / (DateTime.DaysInMonth(dt.Year, dt.Month))) * (ea.Present + ea.Leaves))),
                               Allowance = Convert.ToDecimal(string.Format("{0:F2}", ((S.Housing.Value + S.Travel.Value + S.Allowances.Value) / ((DateTime.DaysInMonth(dt.Year, dt.Month)))) * (ea.Present + ea.Leaves))),
                               IBAN = e.IBAN,
                               Routing = b.RoutingCode,
                               LeaveDays = ea.Leaves,
                               IsAllow = false
                           }
                                               ).Select(sh => new
                                               {
                                                   sh.Empid,
                                                   sh.EmpCode,
                                                   sh.PersonalNo,
                                                   sh.DisplayName,
                                                   sh.StartDate,
                                                   sh.EndDate,
                                                   Basic = sh.BasicSalary,
                                                   Allowances = sh.Allowance,
                                                   sh.IBAN,
                                                   RoutingCode = sh.Routing,
                                                   sh.LeaveDays,
                                                   sh.IsAllow,
                                                   NetSalary = Convert.ToDecimal(string.Format("{0:F2}", (sh.BasicSalary + sh.Allowance)))
                                               }).ToList();

              //  var _CompanyTransactions = new BLInfo<FNGL_Transactions_HI>().GetQuerable<FNGL_Transactions_HI>().Where(x => x.CompanyId == CompanyId && !x.EmpId.HasValue).Sum(x => x.DebitAmount - x.CreditAmount);

                decimal? _CompanyBalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == CompanyId).FirstOrDefault().NewIfEmpty().Balance;
                decimal _TotalBalance = (_CompanyBalance.HasValue ? _CompanyBalance.Value : 0);// + (_CompanyTransactions.HasValue ? _CompanyTransactions.Value : 0);


                //decimal? TotalBalance = 0;
                //if (totalcompanybalance.Count() > 0)
                //    TotalBalance = totalcompanybalance.Sum(a => a.DebitAmount);

                Dictionary<object, object> dic = new Dictionary<object, object>();


                dic.Add("Unprocess", obj);
                dic.Add("TotalBalance", _TotalBalance);

                return this.SuccessResponse(dic);
            }
            catch (Exception Ex)
            {

                if (Errors.Count() > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }
        
        public DTO GetTotalBalance(int CompanyId)
        {
            try
            {
                decimal? _CompanyBalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == CompanyId).FirstOrDefault().NewIfEmpty().Balance;
                decimal _TotalBalance = (_CompanyBalance.HasValue ? _CompanyBalance.Value : 0);// + (_CompanyTransactions.HasValue ? _CompanyTransactions.Value : 0);
                return this.SuccessResponse(_TotalBalance);
            }
            catch (Exception Ex)
            {

                if (Errors.Count() > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }
       
        public DTO GetProcessedEmployeesData(UnprocesessedParameters objunprocess)
        {
            try
            {
                DateTime? Date = objunprocess.SifDate;
                Common.DateHandler(ref Date);

                var _ProcessedEmployee = new BLInfo<HRPR_SifDetail_TR>().GetQuerable<HRPR_SifDetail_TR>().Where(x => x.StartDate.Value.Month == Date.Value.Month
                                                                                   && x.StartDate.Value.Year == Date.Value.Year && x.Status == 'P' && x.CompanyID == objunprocess.CompanyID).ToList();


                //objunprocess.CompanyID
                //                             & p.StationId.Value == (objunprocess.branchId > 0 ? objunprocess.branchId : p.StationId.Value)
                //               & p.DeptId == (objunprocess.departmentId > 0 ? objunprocess.departmentId : p.DeptId.Value)
                //                              & e.EmpId == (objunprocess.empId > 0 ? objunprocess.empId : e.EmpId)

                //                             & !_ProcessedEmployee.Contains(e.EmpId

                var obj = (from e in _ProcessedEmployee
                           select new
                           {
                               e.SifDtlId,
                               e.SifId,
                               Empid = e.EmpId,
                               EmpCode = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.EmpCode : "",
                               PersonalNo = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.PersonalNo : "",
                               DisplayName = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.DisplayName : "",
                               StartDate = e.StartDate,
                               EndDate = e.EndDate,
                               Basic = e.Basic,
                               Allowances = e.Allowances,
                               IBAN = e.IBAN,
                               Routing = e.RoutingCode,
                               LeaveDays = e.LeaveDays,
                               e.NetSalary,
                               e.RoutingCode,
                               IsAllow = false,
                               Status = "V"

                           }).ToList();

                return this.SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                if (Errors.Count() > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }

        public DTO GetVerifiedEmployeesData(UnprocesessedParameters objunprocess)
        {
            try
            {
                DateTime? Date = objunprocess.SifDate;
                Common.DateHandler(ref Date);
                var _ProcessedEmployee = new BLInfo<HRPR_SifDetail_TR>().GetQuerable<HRPR_SifDetail_TR>().Where(x => x.StartDate.Value.Month == Date.Value.Month
                                                                                   && x.StartDate.Value.Year == Date.Value.Year && x.Status == 'V'&& x.CompanyID == objunprocess.CompanyID).ToList();
                var obj = (from e in _ProcessedEmployee
                           select new
                           {
                               e.SifDtlId,
                               e.SifId,

                               Empid = e.EmpId,
                               EmpCode = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.EmpCode : "",
                               PersonalNo = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.PersonalNo : "",
                               DisplayName = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.DisplayName : "",
                               StartDate = e.StartDate,
                               EndDate = e.EndDate,
                               Basic = e.Basic,
                               Allowances = e.Allowances,
                               IBAN = e.IBAN,
                               Routing = e.RoutingCode,
                               LeaveDays = e.LeaveDays,
                               e.NetSalary,
                               e.RoutingCode,
                               IsAllow = false,
                               Status = "V"

                           }).ToList();

                return this.SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                if (Errors.Count() > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }
       
        public DTO GetLastVerifiedEmployeesData(UnprocesessedParameters objunprocess)
        {
            try
            {
                var _ProcessedEmployee = new BLInfo<HRPR_SifDetail_TR>().GetQuerable<HRPR_SifDetail_TR>().Where(x => x.Status == 'V' && x.CompanyID == objunprocess.CompanyID).ToList();
                var obj = (from e in _ProcessedEmployee
                           select new
                           {
                               e.SifDtlId,
                               e.SifId,

                               Empid = e.EmpId,
                               EmpCode = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.EmpCode : "",
                               PersonalNo = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.PersonalNo : "",
                               DisplayName = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.DisplayName : "",
                               StartDate = e.StartDate,
                               EndDate = e.EndDate,
                               Basic = e.Basic,
                               Allowances = e.Allowances,
                               IBAN = e.IBAN,
                               Routing = e.RoutingCode,
                               LeaveDays = e.LeaveDays,
                               e.NetSalary,
                               e.RoutingCode,
                               IsAllow = false,


                           }).ToList();

                return this.SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                if (Errors.Count() > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }

        public DTO GetGenerateSIFData(UnprocesessedParameters objunprocess)
        {
            try
            {
                DateTime? Date = objunprocess.SifDate;
                Common.DateHandler(ref Date);
                var _ProcessedEmployee = new BLInfo<HRPR_ExpSifDetail_TR>().GetQuerable<HRPR_ExpSifDetail_TR>().Where(x => x.StartDate.Value.Month == Date.Value.Month
                                                                                   && x.StartDate.Value.Year == Date.Value.Year && x.RecordType == "EDR" && x.HRPR_ExportSIFFiles_HI.CompanyId == objunprocess.CompanyID).ToList();


                var obj = (from e in _ProcessedEmployee
                           select new
                           {
                               Empid = e.EmpId,
                               e.RecordType,
                               PersonalNo = e.HRMS_EmployeeMst_ST != null ? e.HRMS_EmployeeMst_ST.PersonalNo : "",
                               RoutingCode = e.RoutingCode,
                               IBAN = e.IBAN,
                               StartDate = e.StartDate.ToDateTime().ToString("yyyy-MM-dd"),
                               EndDate = e.EndDate.ToDateTime().ToString("yyyy-MM-dd"),
                               SalaryDays = (e.EndDate - e.StartDate).Value.Days + 1,
                               Basic = e.Basic,
                               Allowances = e.Allowances,
                               LeaveDays = e.LeaveDays

                           }).ToList();

                ExportSIF ob = null;
                if (obj.Count() > 0)
                {
                    string EstId = "";
                    var objcom = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == objunprocess.CompanyID).FirstOrDefault();
                    if (objcom != null)
                    {
                        EstId = objcom.EstId.PadLeft(13, '0');
                        EstId = EstId.Substring(EstId.Length - 13);
                    }
                    ob = new ExportSIF
                    {


                        //Empid = 0,
                        RecordType = "SCR",
                        PersonalNo = EstId,// _ProcessedEmployee != null ? _ProcessedEmployee.FirstOrDefault().HRMS_EmployeeMst_ST.SMAM_CompanyMst_ST.EstId.PadLeft(13,0) : "",
                        RoutingCode = _ProcessedEmployee != null ? _ProcessedEmployee.FirstOrDefault().RoutingCode : "",
                        IBAN = _ProcessedEmployee != null ? _ProcessedEmployee.FirstOrDefault().HRPR_ExportSIFFiles_HI.CreatedOn.ToDateTime().ToString("yyyy-MM-dd") : "",
                        StartDate = _ProcessedEmployee != null ? _ProcessedEmployee.FirstOrDefault().HRPR_ExportSIFFiles_HI.CreatedOn.ToDateTime().ToString("HHmm") : "",
                        EndDate = _ProcessedEmployee != null ? (_ProcessedEmployee.FirstOrDefault().HRPR_ExportSIFFiles_HI.CreatedOn.ToDateTime().ToString("MMyyyy")) : "",
                        SalaryDays = _ProcessedEmployee.Count(),
                        Basic = (_ProcessedEmployee != null ? _ProcessedEmployee.Sum(a => a.Basic + a.Allowances) : 0).ToString(),
                        Allowances = "AED",
                        LeaveDays = "COREPAY",
                        // RoutingCode = _ProcessedEmployee != null ? _ProcessedEmployee.FirstOrDefault().RoutingCode : "",

                    };
                }


                //    var finalobj = obj.Union(obj1);

                Dictionary<object, object> dic = new Dictionary<object, object>();


                dic.Add("ExpSifDetail", obj);
                dic.Add("ExpSif", ob);

                return this.SuccessResponse(dic);
            }
            catch (Exception Ex)
            {

                if (Errors.Count() > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }

        public DTO SubmitSIFDETAIL(IList<HRPR_SifDetail_TR> obj, int sifId, int? CompanyId, int? CreatedBy)
        {
            try
            {
                foreach (var item in obj)
                {
                    if (item.SifDtlId != 0)
                        GetByPrimaryKey(item.SifDtlId);
                    if (item.SifDtlId == 0 || item.SifDtlId == null)
                        PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                        New();


                    Current.SifId = sifId;
                    Current.EmpId = item.EmpId;
                    Current.CompanyID = CompanyId;
                    Current.StartDate = item.StartDate;
                    Current.EndDate = item.EndDate;
                    Current.EmpId = item.EmpId;
                    Current.Basic = item.Basic;
                    Current.Allowances = item.Allowances;
                    Current.NetSalary = item.NetSalary;
                    Current.LeaveDays = item.LeaveDays;
                    Current.RoutingCode = item.RoutingCode;
                    Current.IBAN = item.IBAN;
                    Current.Status = 'P';
                    Current.ProcessedBy = CreatedBy;
                    Current.ProcessedOn = DateTime.Now;

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

        public DTO SubmitSIFDETAILANDACCOUN(IList<HRPR_SifDetail_TR> obj)
        {
            try
            {
                foreach (var item in obj)
                {

                    PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                        New();



                    Current.EmpId = item.EmpId;
                    // Current.CompanyID = CompanyId;
                    Current.StartDate = item.StartDate;
                    Current.EndDate = item.EndDate;
                    Current.EmpId = item.EmpId;
                    Current.Basic = item.Basic;
                    Current.Allowances = item.Allowances;
                    Current.NetSalary = item.NetSalary;
                    Current.LeaveDays = item.LeaveDays;
                    Current.RoutingCode = item.RoutingCode;
                    Current.IBAN = item.IBAN;


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

        public DTO UpdateSIFDetail(IList<HRPR_SifDetail_TR> obj)
        {
            try
            {
                decimal? TotalBasic = obj != null ? obj.Sum(a => a.Basic) : 0;
                decimal? TotalAllowances = obj != null ? obj.Sum(a => a.Allowances) : 0;

                decimal? TotalSalary = TotalBasic + TotalAllowances;
                int? cmpid = obj[0].CompanyID;
                decimal? totalamountinacc = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == cmpid).FirstOrDefault().NewIfEmpty().Balance;
                //\new BLInfo<FNGL_Transactions_HI>().GetQuerable<FNGL_Transactions_HI>().Where(a => a.CompanyId == cmpid).ToList().NewIfEmpty().Sum(a => a.DebitAmount);

                if (TotalSalary <= totalamountinacc)
                {


                    foreach (var item in obj)
                    {


                        if (item.SifDtlId != 0)
                            GetByPrimaryKey(item.SifDtlId);

                        if (Current != null)
                            PrimaryKeyValue = item.SifDtlId;
                        if (PrimaryKeyValue != null)
                        {
                            Current.Status = item.Status;
                            Current.VerifiedBy = obj[0].VerifiedBy;
                            Current.VerifiedOn = DateTime.Now;
                            Save();
                        }

                    }
                    return this.SuccessResponse("Saved Successfully.");
                }
                else
                    return this.BadResponse("Salary cannot be greater than Total Amount...!");
            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }

        public DTO ProcessRollBack(IList<HRPR_SifDetail_TR> obj)
        {
            try
            {
                foreach (var item in obj)
                {


                    if (item.SifDtlId != 0)
                        GetByPrimaryKey(item.SifDtlId);

                    if (Current != null)
                        PrimaryKeyValue = item.SifDtlId;
                    if (PrimaryKeyValue != null)
                    {
                        Delete();
                        Save();
                    }

                }
                return this.SuccessResponse("Saved Successfully.");


            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }

        public DTO ExportSIF(IList<ExportSIF> lst)
        {
            try
            {
                string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/");
                string Filename = "";
                if (lst.Count > 0)
                {
                    var sb = new StringBuilder();
                    int count = 0;
                    string text = "";
                    foreach (var item in lst)
                    {

                        sb.AppendLine(item.RecordType + "," + item.PersonalNo + "," + item.RoutingCode + "," + item.IBAN + "," + item.StartDate + "," + item.EndDate + "," + item.SalaryDays + "," + item.Basic + "," + item.Allowances + "," + item.LeaveDays);
                    }




                    var filePath = "";
                    //filePath += @"\Company Documents\";
                    filePath = filePath + @"\SIF Files\";

                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }


                    filePath = filePath + "\\" + DateTime.Now.ToString("dd MMM yyyyy") + "\\";

                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }
                    string originalpath = basefilepath + filePath;
                    Filename = originalpath + lst.LastOrDefault().PersonalNo + lst.LastOrDefault().IBAN.ToDateTime().ToString("yyMMdd") + lst.LastOrDefault().StartDate + DateTime.Now.ToString("ss");     //Guid.NewGuid().ToString();  //lst.LastOrDefault().PersonalNo.ToString();
                    File.WriteAllText(Filename + ".SIF", sb.ToString());
                }
                return this.SuccessResponse(Filename + ".SIF has been generated to " + basefilepath);
            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }

        //public DTO GetCompanyDeposit()
        //{
        //    try
        //    {
        //        var objdeposit = new BLInfo<HRPR_DepositSlip_TR>().GetQuerable<HRPR_DepositSlip_TR>().Where(a => a.SlipId != 0).ToList();
        //        var lst = (from sa in objdeposit
        //                   select new
        //                   {
        //                       sa.SlipId,
        //                       sa.SlipNo,
        //                       sa.CompanyId,
        //                       Company = sa.CompanyId != 0 ? new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == sa.CompanyId).FirstOrDefault().Name : "",
        //                       sa.Amount,
        //                       sa.Date,
        //                       sa.ImgPath,
        //                       DepositDate = sa.Date.Value.ToString("dd/MM/yyyy")

        //                   }).ToList();
        //        return SuccessResponse(lst);
        //    }
        //    catch (Exception Ex)
        //    {
        //        return BadResponse(Ex.Message);
        //    }
        //}
        //public DTO SubmitDeposit(HRPR_DepositSlip_TR obj, string filePath)
        //{
        //    try
        //    {


        //        if (obj.SlipId != null)
        //            GetByPrimaryKey(obj.SlipId);
        //        if (obj.SlipId == 0 || obj.SlipId == null)
        //            PrimaryKeyValue = null;
        //        if (PrimaryKeyValue == null)
        //            New();


        //        Current.CompanyId = obj.CompanyId;
        //        Current.SlipNo = obj.SlipNo;
        //        Current.Amount = obj.Amount;
        //        Current.ImgPath = filePath != "" ? filePath : obj.ImgPath;
        //        if (filePath != "")
        //        {
        //            Current.UploadedBy = obj.UploadedBy;
        //            Current.UploadedOn = DateTime.Now;
        //        }
        //        DateTime? Date = obj.Date;
        //        Common.DateHandler(ref Date);
        //        Current.Date = Date;

        //        Save();
        //        _accountTranSer.SubmitAccountTransanction(Current);


        //        return this.SuccessResponse("Saved Succesfully");
        //    }
        //    catch (Exception Ex)
        //    {
        //        if (Errors.Count > 0)
        //            return BadResponse(Errors);
        //        else
        //            return BadResponse(Ex.Message);
        //    }


        //}

        //public DTO DeleteDeposit(long Id)
        //{
        //    try
        //    {
        //        GetByPrimaryKey(Id);
        //        if (Current != null)
        //            PrimaryKeyValue = Id;

        //        Delete();
        //        _accountTranSer.UpdateAccountTran(Id);

        //        return this.SuccessResponse("Deleted Successfully.");


        //    }
        //    catch (Exception Ex)
        //    {
        //        if (Errors.Count > 0)
        //            return this.BadResponse(Errors);
        //        else
        //            return this.BadResponse(Ex.Message);
        //    }
        //}
    }
}


