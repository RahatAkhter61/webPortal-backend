using ContextMapper;
using CORE_API.Areas.HRMS.Models;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.HRPR
{
    public class BulkSIFDetailService : CastService<HRPR_BulkSIFDetails_HI>
    {
        dbEngine dbEngine = new dbEngine();
        public BulkSIFDetailService()
        { }

        public DTO UpdateGenerateSIF(IList<HRPR_BulkSIFDetails_HI> obj, int userId)
        {
            try
            {
                foreach (var item in obj)
                {

                    GetByPrimaryKey(item.TSifDtlID);


                    Current.SIFGeneratedOn = DateTime.Now;
                    Current.SIFGenerated = userId;

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

        public DTO GetBulkData(UnprocesessedParameters objunprocess)
        {
            try
            {


                var _BulkData = new BLInfo<HRPR_BulkSIFDetails_HI>().GetQuerable<HRPR_BulkSIFDetails_HI>().Where(x => x.CompanyId == objunprocess.CompanyID && x.TFileId == objunprocess.branchId).ToList();

                var objEmp = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(x => x.CompanyId == objunprocess.CompanyID).ToList();
                DateTime startDate = new DateTime(objunprocess.SifDate.Value.Year, objunprocess.SifDate.Value.Month, 1).Date;
                var endDate = startDate.AddMonths(1).AddDays(-1).Date;
                var obj = (from e in _BulkData
                           select new
                           {
                               e.TSifDtlID,
                               e.SifId,
                               Empid = objEmp != null ? objEmp.Where(emp => emp.PersonalNo == e.PersonalNo).FirstOrDefault().NewIfEmpty().EmpId : 0,
                               EmpCode = objEmp != null ? objEmp.Where(emp => emp.PersonalNo == e.PersonalNo).FirstOrDefault().NewIfEmpty().EmpCode : "",
                               PersonalNo = e.PersonalNo,
                               DisplayName = e.EmpName,
                               StartDate = new DateTime(e.Month.Value.Year, e.Month.Value.Month, 1).Date,
                               EndDate = new DateTime(e.Month.Value.Year, e.Month.Value.Month, 1).Date.AddMonths(1).AddDays(-1).Date,
                               Basic = e.Total,
                               FixAmount = e.FixAmount,

                               Allowances = e.VariableAmount,
                               IBAN = e.IBAN,
                               Routing = e.RoutingCode,
                               LeaveDays = e.LeaveDays,
                               e.Total,
                               e.RoutingCode,
                               IsAllow = false,
                               e.IsValid,
                               e.ErrorMessage,
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

        public List<string> SaveBulkSIFDetails(DataTable obj, int ImportedBy, string IsCards, int CompanyId, DateTime SifDate, int TSifId, string TotalBalance, string EstId)
        {

            List<string> lsterror = new List<string>();
            decimal TotalAmount = 0;

            try
            {

                if (PrimaryKeyValue == null)
                    New();
                HRPR_BulkSIFDetails_HI objDet;

                int count = 0;

                decimal? totalFixedAmount = 0M;
                decimal? totalvariableamount = 0M;

                foreach (DataRow item in obj.Rows)
                {
                    objDet = new HRPR_BulkSIFDetails_HI();

                    objDet.ImportedBy = ImportedBy;
                    objDet.ImportedOn = DateTime.Now;
                    objDet.TFileId = TSifId;

                    if (SifDate != null)
                    {
                        DateTime? Date = SifDate;
                        Common.DateHandler(ref Date);
                        objDet.Month = Date;
                    }
                    else
                        objDet.ErrorMessage += "\u2022 Required Salary Month" + Environment.NewLine;

                    if (CompanyId != 0)
                    {
                        var objcomp = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(sa => sa.CompanyId == CompanyId).FirstOrDefault();//.NewIfEmpty().CompanyId;
                        if (objcomp != null)
                        {
                            objDet.EstId = EstId;
                            objDet.CompanyId = CompanyId;
                        }
                    }

                    string Establisment_ID = item["Establisment ID"].ToString();
                    string RoutingCode = item["ROUTING CODE"].ToString();
                    string PersonalNo = item["Personal No"].ToString().Trim();
                    string iban = item["ACCOUNT NO/IBAN"].ToString();
                    string EMPNAME = item["EMP NAME"].ToString();
                    string leavedays = item["NO OF LEAVE DAYS"].ToString();
                    string _varAmount = item["VAR AMT"].ToString();
                    string TotalAmt = item["TOTAL"].ToString();
                    string FixAmt = item["FIX AMOUNT"].ToString();
                    string REMARKS = item["REMARKS"].ToString();
                    string Zero23 = "00000000000000000000000";

                    Regex regex = new Regex("^\\d+$");

                    if (string.IsNullOrEmpty(Establisment_ID))
                    {
                        objDet.ErrorMessage += "\u2022 EstablismentID Should Not be Empty.!" + Environment.NewLine;
                    }

                    if (Establisment_ID.Length != 13)
                    {
                        objDet.ErrorMessage += "\u2022 Establishment must contain 13 Digits" + Environment.NewLine;
                    }

                    if (EstId != Establisment_ID)
                    {
                        objDet.ErrorMessage += "\u2022 Establisment ID Not Match.!" + Environment.NewLine;
                    }

                    #region Other Card 
                    if (IsCards == "false")
                    {
                        if (String.IsNullOrEmpty(RoutingCode))
                            objDet.ErrorMessage += "\u2022 Routing Code Should Not be Empty.!" + Environment.NewLine;

                        if (RoutingCode.Length != 9)
                            objDet.ErrorMessage += "\u2022 Routing Code Should be 9 digits.!" + Environment.NewLine;

                        if (!regex.IsMatch(RoutingCode))
                            objDet.ErrorMessage += "\u2022 Routing Code Allow only Numeric" + Environment.NewLine;

                        if (String.IsNullOrEmpty(iban))
                            objDet.ErrorMessage += "\u2022 IBAN Should Not be Empty.!" + Environment.NewLine;

                        if (iban.Length != 23)
                            objDet.ErrorMessage += "\u2022 IBAN should have 23 Characters" + Environment.NewLine;

                        string StrtAE = iban.Substring(0, 2);

                        if (StrtAE == "AE" && iban.Length == 23)
                        {
                            objDet.IBAN = iban;
                        }
                        else
                        {
                            objDet.IBAN = iban;
                            objDet.ErrorMessage += "\u2022 IBAN Should be Format.!" + Environment.NewLine;
                        }

                        if (string.IsNullOrEmpty(PersonalNo) || PersonalNo.Length != 14)
                        {
                            objDet.PersonalNo = PersonalNo;
                            objDet.ErrorMessage += "\u2022 Personal No Should be 14 digits.!" + Environment.NewLine;
                        }

                        objDet.RoutingCode = RoutingCode;
                        objDet.IBAN = iban;
                        objDet.PersonalNo = PersonalNo;
                    }
                    #endregion

                    #region Kamel Pay Card 
                    if (IsCards == "true")
                    {



                        var objroutcode = new BLInfo<SMAM_Banks_ST>().GetQuerable<SMAM_Banks_ST>().Where(sa => sa.BankId == 1).FirstOrDefault();
                        if (objroutcode == null)
                        {
                            objDet.RoutingCode = objroutcode.RoutingCode;
                            objDet.ErrorMessage += "\u2022 Invalid Routing Code.!" + Environment.NewLine;
                        }
                        else
                        {

                            if (String.IsNullOrEmpty(objroutcode.RoutingCode))
                                objDet.ErrorMessage += "\u2022 Routing Code Should Not be Empty.!" + Environment.NewLine;

                            if (objroutcode.RoutingCode.Length != 9)
                                objDet.ErrorMessage += "\u2022 Routing Code Should be 9 digits.!" + Environment.NewLine;

                            if (!regex.IsMatch(objroutcode.RoutingCode))
                                objDet.ErrorMessage += "\u2022 Routing Code Allow only Numeric" + Environment.NewLine;

                            else
                                objDet.RoutingCode = objroutcode.RoutingCode;
                        }

                        if (PersonalNo != "")
                        {
                            var objPersonalN = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(sa => sa.PersonalNo == PersonalNo && sa.CompanyId == CompanyId).FirstOrDefault();

                            if (objPersonalN == null)
                            {
                                objDet.ErrorMessage += "\u2022 Personal No Not Register.!" + Environment.NewLine;
                                objDet.PersonalNo = PersonalNo.Trim();
                            }

                            if (objPersonalN != null)
                            {
                                if (objPersonalN.IsActive == false)
                                {
                                    objDet.ErrorMessage += "This Employee is Not Active" + Environment.NewLine;
                                }

                                if (objPersonalN.IsArchive == true)
                                {
                                    objDet.ErrorMessage += "This Employee is Archive" + Environment.NewLine;
                                }

                                if (objPersonalN.IsActive == true && objPersonalN.IsArchive == false)
                                {
                                    objDet.PersonalNo = PersonalNo;
                                }

                            }

                            if (iban.Equals(Zero23))
                            {
                                objDet.IBAN = iban;
                            }

                            if (string.IsNullOrEmpty(iban))
                            {
                                objDet.IBAN = iban;
                            }

                            string checkdigit = iban?.Substring(0, 4);
                            if (checkdigit == "4979" && iban.Length == 11)
                            {
                                var WalletId = dbEngine._dbEngineContext.FNGL_Accounts_TRs.Where(o => o.CustomerId == objPersonalN.CustomerId).Select(o => o.WalletId).FirstOrDefault();

                                if (string.IsNullOrEmpty(WalletId))
                                {
                                    objDet.ErrorMessage += "\u2022 WalletId Not Found" + Environment.NewLine;
                                }
                                if (WalletId != iban)
                                {
                                    objDet.IBAN = iban;
                                    objDet.ErrorMessage += "\u2022 Not Match Wallet ID.!" + Environment.NewLine;
                                }
                                objDet.IBAN = iban;
                            }

                        }
                        else
                            objDet.ErrorMessage += "\u2022 Personal No Should Not be Empty.!" + Environment.NewLine;
                    }
                    #endregion




                    objDet.EmpName = EMPNAME;
                    objDet.LeaveDays = decimal.Parse(leavedays != "" ? leavedays : "0");
                    //objDet.VariableAmount = decimal.Parse(_varAmount != "" ? _varAmount : "0");
                    //objDet.FixAmount = decimal.Parse(FixAmt);
                    objDet.Remarks = REMARKS;



                    if (TotalAmt != "")
                    {
                        if (decimal.Parse(TotalAmt) <= 0)
                        {
                            objDet.TotalAmount = decimal.Parse(TotalAmt);
                            objDet.Total = double.Parse(TotalAmt);
                            objDet.ErrorMessage += "\u2022 Total amount should be greater than Zero" + Environment.NewLine;
                        }
                        else
                        {
                            objDet.TotalAmount = decimal.Parse(TotalAmt);
                            objDet.Total = double.Parse(TotalAmt);
                        }
                    }
                    else
                        objDet.ErrorMessage += "\u2022 Total amount Should not be Empty.!" + Environment.NewLine;


                    if (FixAmt != "")
                    {
                        totalFixedAmount += decimal.Parse(FixAmt);

                        if (decimal.Parse(FixAmt) <= 0)
                        {
                            objDet.FixAmount = decimal.Parse(FixAmt);
                            objDet.ErrorMessage += "\u2022 Fixed amount should be greater than Zero" + Environment.NewLine;
                        }
                        else
                        {
                            objDet.FixAmount = decimal.Parse(FixAmt);
                        }
                    }
                    else
                        objDet.ErrorMessage += "\u2022 Fixed amount Should not be Empty.!" + Environment.NewLine;


                    if (_varAmount != "")
                    {
                        objDet.VariableAmount = decimal.Parse(_varAmount);
                        totalvariableamount += decimal.Parse(_varAmount);
                    }
                    else
                        objDet.ErrorMessage += "\u2022 Variable Amount Should not be Empty" + Environment.NewLine;


                    if (Convert.ToDecimal(TotalAmt != "" ? TotalAmt : "0") != (decimal.Parse(FixAmt != "" ? FixAmt : "0") + decimal.Parse(_varAmount != "" ? _varAmount : "0")))
                    {
                        objDet.ErrorMessage += "\u2022 Total amount must be equal to Fixed & Variable amount" + Environment.NewLine;
                    }


                    if (objDet.ErrorMessage != null)
                        objDet.IsValid = false;
                    else
                        objDet.IsValid = true;

                    CurrentBulk.Add(objDet);
                    count++;
                }

                if (Convert.ToDecimal(TotalAmount) > Convert.ToDecimal(TotalBalance))
                {
                    lsterror.Add("Balance Low");
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
                Logger.TraceService(Ex.Message, "BulkSIFDetails");
                lsterror.Add(Ex.Message);
            }

            return lsterror;
        }


    }

}
