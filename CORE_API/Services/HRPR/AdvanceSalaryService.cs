using ContextMapper;
using CORE_API.General;
using CORE_API.Models.AdvanceSalary;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CORE_API.Services.HRPR
{
    public class AdvanceSalaryService : CastService<AdvanceSalary>
    {

        dbEngine db = new dbEngine();
        public DTO GetAdvanceSalary(int? pageNo, int limit, int UserId)
        {

            string[] paramater = { "pageNumber", "NumberofRecord", "UserId" };
            DataTable getAllLoanApprovals = ExecuteSp("SP_GetAllLoanApprovals", paramater, pageNo, limit, UserId);

            int totatRecord = getAllLoanApprovals.Rows.Count;

            if (totatRecord == 0)
                return BadResponse("No Record Found");

            return SuccessResponse(getAllLoanApprovals);


        }
        public DTO GetAdvanceSalaryDetailbyId(int advanceSalaryId)
        {
            var GetAdvanceSalaryDetail = new BLInfo<AdvanceSalaryDetail>().GetQuerable<AdvanceSalaryDetail>().Where(o => o.AdvanceSalaryId == advanceSalaryId).ToList();

            if (GetAdvanceSalaryDetail.Count == 0)
                return BadResponse("Record Not Available");

            return SuccessResponse(GetAdvanceSalaryDetail);
        }

        public DTO GetEmployeeSalaryInformation(string personalNo)
        {

            string[] paramater = { "PersonalNo" };
            DataTable employeeSalaryInformation = ExecuteSp("SP_GetEmployeeSalaryInformation", paramater, personalNo);

            int totatRecords = employeeSalaryInformation.Rows.Count;

            if (totatRecords == 0)
                return BadResponse("No Record Found");

            return SuccessResponse(employeeSalaryInformation);

        }

        public DTO UpdateAdvanceSalaryStatus(List<AdvanceSalaryModel> model)
        {

            var statusTypelist = db._dbContext.StatusTypes.ToList();
            var statuslist = db._dbContext.Status.ToList();
            var advanceSalaryRecords = db._dbContext.AdvanceSalaries.ToList();

            if (model.Count == 0)
                return BadResponse("Record Not Found");

            if (statusTypelist == null)
                return BadResponse("Advance Salary Status Type Not Available");

            if (statuslist == null)
                return BadResponse("Advance Salary Status Not Available");

            if (advanceSalaryRecords == null && advanceSalaryRecords.Count == 0)
                return BadResponse("No Record Found Advance Salary");




            // Salary Status Type
            var statusSalaryType = statusTypelist.Where(o => o.Name.Equals(Enums.AdvanceSalaryStatus.AdvanceSalary.Replace(" ", String.Empty))).FirstOrDefault();
            if (statusSalaryType == null)
                return BadResponse("Advance Salary Status Type Not Available");


            // Payment Status Type
            var statusPaymentType = statusTypelist.Where(o => o.Name.Equals(Enums.AdvanceSalaryStatus.PaymentType)).FirstOrDefault();
            if (statusPaymentType == null)
                return BadResponse("Payment Salary Status Type Not Available");


            // Advance Salary Status
            var paymentStatus = statuslist.Where(o => o.TypeId == statusPaymentType.StatustypeId && o.Name.Equals(Enums.AdvanceSalaryStatus.AdvanceSalary)).FirstOrDefault();
            if (paymentStatus == null)
                return BadResponse("Payment Status Not Available");

            // Alert Msg
            var alertMsg = new BLInfo<SMAL_Alerts_ST>().GetQuerable<SMAL_Alerts_ST>().Where(o => o.Title.Equals(Enums.AlertSmsType.ADSAL)).FirstOrDefault();
            if (alertMsg == null)
                return BadResponse("Alert Message Not Found.");

            var advanceCharges = (from t1 in statusTypelist
                                  join t2 in statuslist on t1.StatustypeId equals t2.TypeId
                                  where t1.Name.Equals(Enums.AdvanceSalaryStatus.AdvanceCharges) && t2.Name.Equals(Enums.AdvanceSalaryStatus.AdvanceFee)

                                  select new
                                  {
                                      t2.StatusId
                                  }).FirstOrDefault();

            if (advanceCharges == null)
                return BadResponse("Charges Status Not Availabe");


            // Advance Salary Pending Status
            var pendingStatus = statuslist.Where(o => o.TypeId == statusSalaryType.StatustypeId && o.Name.Equals(Enums.AdvanceSalaryStatus.Pending)).FirstOrDefault();

            foreach (var item in model)
            {
                List<AdvanceSalary> advaceSalaryobject = new List<AdvanceSalary>();
                List<AdvanceSalaryDetail> AdvanceSalaryDetail = new List<AdvanceSalaryDetail>();
                List<AdvanceSalaryStatustracking> AdvanceSalaryStatustracking = new List<AdvanceSalaryStatustracking>();
                List<AdvanceSalaryCharge> AdvanceSalaryCharge = new List<AdvanceSalaryCharge>();
                List<AdvanceSalaryDetailInfo> advanceSalaryDeatilInfo = new List<AdvanceSalaryDetailInfo>();

                string sixDigts = "100000";
                long lastAddedNumber = 0;
                string lastTransactionReferencenumber = string.Empty;

                var advanceSalary = advanceSalaryRecords.Where(o => o.AdvanceSalaryId == item.advanceSalaryId).FirstOrDefault();
                var salaryStatus = statuslist.Where(o => o.StatusId == item.statustype).Select(o => o.Name).FirstOrDefault();


                var lastReferenceNo = db._dbContext.AdvanceSalaryDetails.OrderByDescending(a => a.AdvanceSalaryDetailId).FirstOrDefault();

                if (lastReferenceNo != null && lastReferenceNo.TransactionReferencenumber != null && lastReferenceNo.TransactionReferencenumber.Equals("999999"))
                    lastAddedNumber = long.Parse(Common.GenerateCifNumber(sixDigts));

                else if (lastReferenceNo != null && lastReferenceNo.TransactionReferencenumber != null)
                {
                    lastTransactionReferencenumber = lastReferenceNo.TransactionReferencenumber.ToString();
                    lastAddedNumber = long.Parse(Common.GenerateCifNumber(lastTransactionReferencenumber));
                }
                else
                {
                    lastTransactionReferencenumber = lastReferenceNo == null ? sixDigts : lastReferenceNo.TransactionReferencenumber.ToString();
                    lastAddedNumber = long.Parse(Common.GenerateCifNumber(lastTransactionReferencenumber));
                }


                if (item.RoleId == 1 && salaryStatus.Equals(Enums.AdvanceSalaryStatus.Approve))
                {

                    #region Advance Salary Detail Entry Credit
                    var salaryDetailCreditEntry = new AdvanceSalaryDetail();
                    salaryDetailCreditEntry.AdvanceSalaryId = advanceSalary.AdvanceSalaryId;
                    salaryDetailCreditEntry.DebitAmount = 0M;
                    salaryDetailCreditEntry.CreditAmount = item.RequestAmount != null ? item.RequestAmount : 0M;
                    salaryDetailCreditEntry.TypeId = paymentStatus.StatusId;
                    salaryDetailCreditEntry.StatusId = pendingStatus.StatusId;
                    salaryDetailCreditEntry.JobStatus = 1;
                    salaryDetailCreditEntry.Mode = "C";
                    salaryDetailCreditEntry.TransactionReferencenumber = Convert.ToString(lastAddedNumber);
                    salaryDetailCreditEntry.CreatedBy = item.CreatedBy;
                    salaryDetailCreditEntry.CreatedOn = DateTime.Now;

                    AdvanceSalaryDetail.Add(salaryDetailCreditEntry);
                    db._dbContext.AdvanceSalaryDetails.InsertAllOnSubmit(AdvanceSalaryDetail);
                    #endregion

                    #region Advance Salary Detail Entry Debit
                    var salaryDetailDebitEntry = new AdvanceSalaryDetail();
                    salaryDetailDebitEntry.AdvanceSalaryId = advanceSalary.AdvanceSalaryId;
                    salaryDetailDebitEntry.DebitAmount = item.AmountFee != null ? item.AmountFee : 0M;
                    salaryDetailDebitEntry.CreditAmount = 0M;
                    salaryDetailDebitEntry.TypeId = paymentStatus.StatusId;
                    salaryDetailDebitEntry.StatusId = pendingStatus.StatusId;
                    salaryDetailDebitEntry.JobStatus = 1;
                    salaryDetailDebitEntry.Mode = "D";
                    salaryDetailDebitEntry.TransactionReferencenumber = Convert.ToString(++lastAddedNumber);
                    salaryDetailDebitEntry.CreatedBy = item.CreatedBy;
                    salaryDetailDebitEntry.CreatedOn = DateTime.Now;

                    AdvanceSalaryDetail.Add(salaryDetailDebitEntry);
                    db._dbContext.AdvanceSalaryDetails.InsertAllOnSubmit(AdvanceSalaryDetail);
                    db._dbContext.SubmitChanges();
                    #endregion

                    int advanceSalaryDetailId = AdvanceSalaryDetail.OrderByDescending(o => o.AdvanceSalaryDetailId).FirstOrDefault().AdvanceSalaryDetailId;

                    #region Entry Salary Charges
                    var salaryChargesEntry = new AdvanceSalaryCharge();
                    salaryChargesEntry.AdvanceSalaryId = advanceSalary.AdvanceSalaryId;
                    salaryChargesEntry.AdvanceSalaryDetailId = advanceSalaryDetailId;
                    salaryChargesEntry.Amount = item.AmountFee;
                    salaryChargesEntry.AdvanceChargesStatusId = Convert.ToInt32(advanceCharges.StatusId);
                    salaryChargesEntry.CreatedBy = item.CreatedBy;
                    salaryChargesEntry.CreatedOn = DateTime.Now;

                    AdvanceSalaryCharge.Add(salaryChargesEntry);
                    db._dbContext.AdvanceSalaryCharges.InsertAllOnSubmit(AdvanceSalaryCharge);
                    #endregion

                    db._dbContext.SubmitChanges();
                }

                if (salaryStatus.Equals(Enums.AdvanceSalaryStatus.SendtoBank))
                    advanceSalary.LoanPayCompanyId = item.LoanPayCompanyId;



                List<SMAL_AlertQueue_HI> AlertQueue = new List<SMAL_AlertQueue_HI>();
                if (salaryStatus.Equals(Enums.AdvanceSalaryStatus.Reject))
                {

                    #region Add Record SMAL_AlertQueue_HI
                    var getEmpDtl = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(o => o.EmpId == advanceSalary.EmpId).FirstOrDefault();

                    var AlertQueueModel = new SMAL_AlertQueue_HI();
                    AlertQueueModel.SendTo = getEmpDtl.MobileNo;
                    AlertQueueModel.SMSBody = alertMsg.SMSBody;
                    AlertQueueModel.EmpId = advanceSalary.EmpId;
                    AlertQueueModel.AddedOn = DateTime.Now;
                    AlertQueueModel.AlertId = alertMsg.AlertId;
                    AlertQueueModel.SentOn = null;
                    AlertQueueModel.EmpId = getEmpDtl.EmpId;
                    AlertQueue.Add(AlertQueueModel);

                    db._dbContext.SMAL_AlertQueue_HIs.InsertAllOnSubmit(AlertQueue);
                    #endregion

                    #region Add Record Advance Salary Details Info
                    var getAdvanceSalaryDtlInfo = db._dbContext.AdvanceSalaryDetailInfos.Where(o => o.AdvanceSalaryId == item.advanceSalaryId).FirstOrDefault();
                    if (getAdvanceSalaryDtlInfo != null)
                    {
                        getAdvanceSalaryDtlInfo.ReasonofRejection = item.ReasonofRejection;
                        getAdvanceSalaryDtlInfo.ModifyBy = item.CreatedBy;
                        getAdvanceSalaryDtlInfo.ModifyOn = DateTime.Now;
                        db._dbContext.SubmitChanges();
                    }
                    else
                    {
                        var advanceSalaryDtlInfo = new AdvanceSalaryDetailInfo();
                        advanceSalaryDtlInfo.AdvanceSalaryId = item.advanceSalaryId;
                        advanceSalaryDtlInfo.ReasonofRejection = item.ReasonofRejection;
                        advanceSalaryDtlInfo.CreatedBy = item.CreatedBy;
                        advanceSalaryDtlInfo.CreatedOn = DateTime.Now;
                        advanceSalaryDeatilInfo.Add(advanceSalaryDtlInfo);
                        db._dbContext.AdvanceSalaryDetailInfos.InsertAllOnSubmit(advanceSalaryDeatilInfo);
                    }
                    #endregion
                }


                #region Advance Salary Status Tracking
                var advanceSalaryTracking = new AdvanceSalaryStatustracking();
                advanceSalaryTracking.AdvanceSalaryId = advanceSalary.AdvanceSalaryId;
                advanceSalaryTracking.StatusId = advanceSalary.AdvanceStatusId;
                advanceSalaryTracking.CreatedBy = item.CreatedBy;
                advanceSalaryTracking.CreatedOn = DateTime.Now;
                #endregion


                #region Update Advance Salary
                if (item.RoleId == 1 && salaryStatus.Equals(Enums.AdvanceSalaryStatus.Approve))
                    advanceSalary.ApprovalDate = DateTime.Now;

                advanceSalary.AdvanceStatusId = item.statustype;
                advanceSalary.ModifiedOn = DateTime.Now;
                advanceSalary.ModifiedBy = item.CreatedBy;

                AdvanceSalaryStatustracking.Add(advanceSalaryTracking);
                advaceSalaryobject.Add(advanceSalary);

                db._dbContext.AdvanceSalaryStatustrackings.InsertAllOnSubmit(AdvanceSalaryStatustracking);
                db._dbContext.SubmitChanges();
                #endregion
            }

            return SuccessResponse("Update Successfully");

        }

        public DTO GetAllPreviousAdvanceSalaries(int empId)
        {

            var previousAdvanceSalaries = (from t1 in db._dbContext.AdvanceSalaries
                                           join t2 in db._dbContext.Status on t1.RecoveryStatus equals t2.StatusId
                                           where t1.EmpId == empId && t1.IsComplete == true && t2.Name.Equals(Enums.AdvanceSalaryStatus.FullyRecieve)

                                           select new
                                           {

                                               PaidOnTime = t1.PaidOnTime == true ? "Yes" : "No",
                                               t1.lastRecievingDate,
                                               t1.DateOfDisbursement,

                                           }).ToList();

            return SuccessResponse(previousAdvanceSalaries);

        }

        public DTO GetAdvanceSalaryStatus(int UserId)
        {

            var roleName = (from t1 in db._dbContext.SMIM_UserMst_STs
                            join t2 in db._dbContext.SMIM_Roles_STs on t1.RoleId equals t2.RoleId
                            where t1.UserId == UserId &&
                            t2.RoleName.Equals(Enums.AdvanceSalaryStatus.ComplianceView)
                            select new
                            {
                                t2.RoleName,
                            }).FirstOrDefault();

            var advanceSalarystatusType = Common.GetStatusType(Enums.StatusTypes.AdvanceSalary);

            var advanceSalaryStatus = Common.GetStatusByTypeId(advanceSalarystatusType.StatustypeId).Select(o => new
            {
                o.StatusId,
                o.Name,
                o.TypeId,

            }).ToList();

            if (advanceSalaryStatus.Count == 0)
                return BadResponse("Advance Salary Status Not Available");

            var response = new
            {
                AdvanceSalaryStatus = advanceSalaryStatus,
                roleName = roleName,

            };

            return SuccessResponse(response);
        }

        public DTO EmployeeAdvanceEligibility(int companyId, string customerId, int pageNo = 0, int limit = 100)
        {
            List<EmployeeAdvanceEligibiltyResponseModel> ReturnModel = new List<EmployeeAdvanceEligibiltyResponseModel>();
            int totalRecords = 0;
            var querry = (from t1 in db._dbContext.HRMS_EmployeeMst_STs
                          join t2 in db._dbContext.SMAM_CompanyMst_STs on t1.CompanyId equals t2.CompanyId
                          where t1.CompanyId == companyId
                          select new EmployeeAdvanceEligibiltyResponseModel
                          {
                              EmpName = t1.DisplayName,
                              EmpId = t1.EmpId,
                              CompanyName = t2.BusinessName,
                              CompanyId = t2.CompanyId,
                              MobileNo = t1.MobileNo,
                              CompanyCreatedDate = t2.CreatedOn,
                              CardCreatedDate = t1.CreatedOn,
                              EmiratesId = t1.HRMS_EmpDocs_STs.FirstOrDefault(x => x.DocId == 6 && x.IsActive == 'Y') != null ? t1.HRMS_EmpDocs_STs.FirstOrDefault(x => x.DocId == 6 && x.IsActive == 'Y').DocNo : "",
                              CustomerNo = t1.CustomerId
                          }).ToList();

            totalRecords = querry.Count;
            var getEmpData = !string.IsNullOrEmpty(customerId) ? querry.Where(x => x.CustomerNo == customerId).ToList()
                                                        : querry.ToList().Skip(pageNo).Take(limit).ToList();

            foreach (var item in getEmpData)
            {

                item.Message = Enums.AdvanceEligibiltyStatus.Eligible;
                item.Eligible = Enums.AdvanceEligibiltyStatus.Yes;
                item.totalRecords = totalRecords;
                var account = db._dbEngineContext.FNGL_Accounts_TRs.Where(o => o.CustomerId == item.CustomerNo).FirstOrDefault();
                if (account == null)
                {
                    item.Message = "Account Not Found Against Custumer Id";
                    item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                    ReturnModel.Add(item);
                    continue;
                }
                var employee = db._dbContext.HRMS_EmployeeMst_STs.Where(o => o.CustomerId == account.CustomerId).FirstOrDefault();

                if (employee.IsActive == false && employee.IsArchive == true)
                {
                    item.Message = "Employee Is not Active";
                    item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                    ReturnModel.Add(item);
                    continue;
                }

                decimal eligibleAdvanceAmount = 0;
                decimal averageSalary = 0;

                item.EmpCrn = account.WalletId;
                item.CardCreatedDate = employee.CreatedOn;
                string[] paramater = { "PersonalNo" };
                DataTable dt = ExecuteSp("GetEmployeeLastSalaryMonthWise", paramater, employee.PersonalNo);
                List<SalaryDetail> employeeLastSalaryMonthWise = new List<SalaryDetail>();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var salaryDetail = new SalaryDetail
                        {
                            Year = dt.Rows[i]["Year"].ToString(),
                            Month = dt.Rows[i]["Month"].ToString(),
                            NetSalary = Convert.ToDecimal(dt.Rows[i]["NetSalary"]),
                            SalaryDate = Convert.ToDateTime(dt.Rows[i]["SalaryDate"]),

                        };
                        employeeLastSalaryMonthWise.Add(salaryDetail);
                    }
                }

                var advanceMinSalaryObj = db._dbContext.SystemSettings.FirstOrDefault(x => x.Key.Equals(Enums.SystemSettings.AdvanceMinSalary.ToString()));
                decimal advanceMinSalary = (advanceMinSalaryObj != null && !string.IsNullOrEmpty(advanceMinSalaryObj.Value)) ? Convert.ToDecimal(advanceMinSalaryObj.Value) : 500M;

                var advanceMaxSalaryObj = db._dbContext.SystemSettings.FirstOrDefault(x => x.Key.Equals(Enums.SystemSettings.AdvanceMaxSalary.ToString()));
                decimal advanceMaxSalary = (advanceMaxSalaryObj != null && !string.IsNullOrEmpty(advanceMaxSalaryObj.Value)) ? Convert.ToDecimal(advanceMaxSalaryObj.Value) : 5000M;

                var advanceMaxEligibleObj = db._dbContext.SystemSettings.FirstOrDefault(x => x.Key.Equals(Enums.SystemSettings.MaxAmountOfEligibility.ToString()));
                decimal advanceMaxEligibleAmount = (advanceMaxEligibleObj != null && !string.IsNullOrEmpty(advanceMaxEligibleObj.Value)) ? Convert.ToDecimal(advanceMaxEligibleObj.Value) : 0M;

                var empWhitelist = db._dbContext.HRMS_EmpWhitelistings.Where(o => o.CompanyId == employee.CompanyId && o.EmpId == employee.EmpId).FirstOrDefault();
                if (empWhitelist != null)
                {

                    var whiteListStatus = db._dbContext.Status.Where(o => o.StatusId == empWhitelist.StatusId).FirstOrDefault();

                    if (whiteListStatus == null || whiteListStatus.Name == null || whiteListStatus.Name.Equals(Enums.EmployeeWhitelistingStatus.BlackList))
                    {
                        item.Message = "Employee is Blacklisted";
                        item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                        ReturnModel.Add(item);
                        continue;
                    }

                    int lastMonthSalaryCount = employeeLastSalaryMonthWise.Count();
                    var AdvanceAverageSalary = db._dbContext.SystemSettings.Where(x => x.Key == Enums.SystemSetting.AdvanceAverageSalary.ToString()).Select(o => o.Value).FirstOrDefault();

                    if (lastMonthSalaryCount == 0)
                    {
                        decimal.TryParse(AdvanceAverageSalary, out averageSalary);
                        item.AverageSalary = averageSalary;
                    }


                    else if (lastMonthSalaryCount == 1)
                    {
                        averageSalary = employeeLastSalaryMonthWise.Select(o => (decimal)o.NetSalary).FirstOrDefault();
                        item.AverageSalary = averageSalary;
                    }

                    else if (lastMonthSalaryCount == 2)
                    {
                        averageSalary = employeeLastSalaryMonthWise.Sum(o => (decimal)o.NetSalary) / 2;
                        item.AverageSalary = averageSalary;
                    }


                    else if (lastMonthSalaryCount > 2)
                    {
                        averageSalary = employeeLastSalaryMonthWise.Sum(o => (decimal)o.NetSalary) / 3;
                        var salaryBelowAverageSalary = employeeLastSalaryMonthWise.FirstOrDefault();
                        averageSalary = salaryBelowAverageSalary.NetSalary > averageSalary ? averageSalary : (decimal)salaryBelowAverageSalary.NetSalary;
                        item.AverageSalary = averageSalary;
                    }
                    if (averageSalary < advanceMinSalary || averageSalary > advanceMaxSalary)
                    {
                        decimal.TryParse(AdvanceAverageSalary, out averageSalary);
                        item.AverageSalary = averageSalary;
                    }
                }
                else
                {

                    if (employeeLastSalaryMonthWise.Count() > 2)
                    {
                        DateTime currentDate = Convert.ToDateTime(employeeLastSalaryMonthWise.FirstOrDefault().SalaryDate);
                        int count = 0;

                        foreach (var item2 in employeeLastSalaryMonthWise)
                        {
                            count++;
                            if (int.Parse(item2.Month) != DateTime.Now.AddMonths(-count).Month)
                            {

                                item.Message = "Employee is not eligible for Advance Salary because he dont get three consecutive Salaries";
                                item.Eligible = Enums.AdvanceEligibiltyStatus.No;

                            }
                            if (count != 1)
                            {
                                if (item2.SalaryDate > currentDate.AddDays(-20))
                                {
                                    item.Message = "Employee is not eligible for Advance Salary because he dont have 20 days differrence in salary";
                                    item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                                }
                                else
                                    currentDate = (DateTime)item2.SalaryDate;
                            }
                            if (count == 3)
                                break;
                        }

                        //Average Salary Working
                        averageSalary = employeeLastSalaryMonthWise.Sum(o => (decimal)o.NetSalary) / 3;
                        var salaryBelowAverageSalary = employeeLastSalaryMonthWise.FirstOrDefault();
                        averageSalary = salaryBelowAverageSalary.NetSalary > averageSalary ? averageSalary : (decimal)salaryBelowAverageSalary.NetSalary;
                        item.AverageSalary = averageSalary;

                    }
                    else
                    {
                        item.Message = "Employee is not eligible for Advance Salary because he dont get three consecutive Salaries";
                        item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                        ReturnModel.Add(item);
                        continue;
                    }

                }



                if (averageSalary >= advanceMinSalary && averageSalary <= advanceMaxSalary)
                {
                    eligibleAdvanceAmount = averageSalary / 100 * 50;
                    eligibleAdvanceAmount = eligibleAdvanceAmount >= advanceMaxEligibleAmount ? advanceMaxEligibleAmount : eligibleAdvanceAmount;
                    item.EligibleAMount = eligibleAdvanceAmount;
                }
                else
                {
                    item.Message = "Employee must have min " + advanceMinSalary + " and max " + advanceMaxSalary + " average salary for advance";
                    item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                    ReturnModel.Add(item);
                    continue;
                }

                //Previous History
                var pendingAdvance = db._dbContext.AdvanceSalaries.Where(o => o.EmpId == employee.EmpId && o.Status.Name == "Pending").OrderByDescending(o => o.AdvanceSalaryId).FirstOrDefault();
                if (pendingAdvance != null)
                {
                    item.Message = "You have already applied for a Salary Advance. Please wait for approval";
                    item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                    ReturnModel.Add(item);
                    continue;
                }

                //Rejected
                var rejectedAdvance = db._dbContext.AdvanceSalaries.Where(o => o.EmpId == employee.EmpId && o.Status.Name == "Reject").OrderByDescending(o => o.AdvanceSalaryId).FirstOrDefault();
                if (rejectedAdvance != null)
                {
                    item.Message = "Your request for Salary Advance was rejected. Please try again at a later date.";
                    item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                    ReturnModel.Add(item);
                    continue;
                }

                // Not Return
                var notReturn = db._dbContext.AdvanceSalaries.Where(o => o.EmpId == employee.EmpId && o.IsComplete == false).OrderByDescending(o => o.AdvanceSalaryId).FirstOrDefault();
                if (notReturn != null)
                {
                    item.Message = "Your previous request for Salary Advance was approved. your Due Date for returning is";
                    item.DueDate = notReturn.DueDate;
                    item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                    ReturnModel.Add(item);
                    continue;
                }

                // Not Return On Time
                var notReturnOntime = db._dbContext.AdvanceSalaries.Where(o => o.EmpId == employee.EmpId && o.PaidOnTime == false).Count() > 0;
                if (notReturnOntime)
                {
                    item.Message = "Employee not return previous advance amount on time";
                    item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                    ReturnModel.Add(item);
                    continue;
                }

                // VAT 
                var VAT = db._dbContext.SystemSettings.Where(x => x.Key == Enums.SystemSettings.VAT.ToString()).Select(o => o.Value).FirstOrDefault();
                if (VAT == null)
                {
                    item.Message = "VAT is Not Found in System Setting";
                    item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                    ReturnModel.Add(item);
                    continue;
                }
                decimal VATPercent = decimal.Parse(VAT);


                // Company Load Bracket

                List<CompanyLoanBracket> companyLoanBrackets = null;
                companyLoanBrackets = db._dbContext.CompanyLoanBrackets.Where(o => o.CompanyId == employee.CompanyId).ToList();
                if (companyLoanBrackets.Count() == 0)
                {
                    companyLoanBrackets = db._dbContext.CompanyLoanBrackets.Where(o => o.CompanyId == null).ToList();
                    if (companyLoanBrackets.Count() == 0)
                    {
                        item.Message = "Loan Bracket Not Found Against company";
                        item.Eligible = Enums.AdvanceEligibiltyStatus.No;
                        ReturnModel.Add(item);
                        continue;
                    }
                }

                var companyLoanBracketsdata = companyLoanBrackets.Select(o => new LoanBracketDetails
                {
                    FromAmount = o.FromAmount,
                    ToAmount = o.ToAmount,
                    VATPercent = VATPercent,
                    VATAmount = (o.Fees / 100 * VATPercent),
                    Fees = o.Fees + (o.Fees / 100 * VATPercent)
                }).ToList();

                item.loanBrackets = companyLoanBracketsdata;
                ReturnModel.Add(item);


            }
            if (ReturnModel != null && ReturnModel.Count == 0)
                return BadResponse("No Record Found...!");
            return SuccessResponse(ReturnModel);
        }

        public DTO AddEditAdditinoalQuestionAdvanceSalary(AdditinoalQuestionAdvanceSalaryModel model)
        {


            var returnMsg = string.Empty;
            var advanceDetailInfo = new AdvanceSalaryDetailInfo();

            var checkAdvanceSalary = db._dbContext.AdvanceSalaries.Where(o => o.AdvanceSalaryId == model.AdvanceSalaryId).FirstOrDefault();
            if (checkAdvanceSalary == null)
                return BadResponse("Advance Salary Record Not Found, Agains this Id " + model.AdvanceSalaryId);

            if (model.AdvanceSalaryDetailInfoId > 0)
            {

                var getadvanceSalaryDtlInfo = db._dbContext.AdvanceSalaryDetailInfos.Where(o => o.AdvanceSalarydtlinfoId == model.AdvanceSalaryDetailInfoId).FirstOrDefault();
                if (getadvanceSalaryDtlInfo == null)
                    return BadResponse("Can't Update Record Because Record Not Found.");

                getadvanceSalaryDtlInfo.HomeVisitPlan = model.HomeVisitPlan;
                getadvanceSalaryDtlInfo.Jobleaving = model.JobLeavingPlan;
                getadvanceSalaryDtlInfo.CellNumberSpouse = model.CellNumberSpouse;
                getadvanceSalaryDtlInfo.NameofSpouse = model.NameofSpouse;
                getadvanceSalaryDtlInfo.Purposeofloan = model.PurposeofLoan;
                getadvanceSalaryDtlInfo.UaeReferenceName = model.UAEfriend;
                getadvanceSalaryDtlInfo.UaeReferenceNo = model.CellNumberUaeFriend;
                getadvanceSalaryDtlInfo.otherJobleaving = model.OtherJobleaving;
                getadvanceSalaryDtlInfo.otherPurposeofloan = model.OtherPurposeofloan;
                getadvanceSalaryDtlInfo.ModifyBy = model.CreatedBy;
                getadvanceSalaryDtlInfo.ModifyOn = DateTime.Now;
                returnMsg = "Record Successful Update";
            }
            else
            {
                advanceDetailInfo.AdvanceSalaryId = checkAdvanceSalary.AdvanceSalaryId;
                advanceDetailInfo.HomeVisitPlan = model.HomeVisitPlan;
                advanceDetailInfo.Jobleaving = model.JobLeavingPlan;
                advanceDetailInfo.CellNumberSpouse = model.CellNumberSpouse;
                advanceDetailInfo.NameofSpouse = model.NameofSpouse;
                advanceDetailInfo.Purposeofloan = model.PurposeofLoan;
                advanceDetailInfo.UaeReferenceName = model.UAEfriend;
                advanceDetailInfo.UaeReferenceNo = model.CellNumberUaeFriend;
                advanceDetailInfo.otherJobleaving = model.OtherJobleaving;
                advanceDetailInfo.otherPurposeofloan = model.OtherPurposeofloan;
            }



            if (model.AdvanceSalaryDetailInfoId == 0)
            {
                advanceDetailInfo.CreatedBy = model.CreatedBy;
                advanceDetailInfo.CreatedOn = DateTime.Now;
                db._dbContext.AdvanceSalaryDetailInfos.InsertOnSubmit(advanceDetailInfo);
                returnMsg = "Record Successful Save";
            }

            db._dbContext.SubmitChanges();

            return SuccessResponse(returnMsg);
        }

        public DTO GetAdvanceSalaryDetailInfobyId(int advanceSalaryId)
        {

            
            
            var getAdvanceSalaryDtlInfo = db._dbContext.AdvanceSalaryDetailInfos.Where(o => o.AdvanceSalaryId == advanceSalaryId)
                .Select(o => new
                {
                    o.AdvanceSalarydtlinfoId,
                    o.ReasonofRejection,

                    PurposeofloanValue = o.Purposeofloan != null ? db._dbContext.Status.Where(i => i.StatusId == o.Purposeofloan).Select(i => i.Name).FirstOrDefault() : "",
                    HomeVisitPlanValue = o.HomeVisitPlan != null ? db._dbContext.Status.Where(i => i.StatusId == o.HomeVisitPlan).Select(i => i.Name).FirstOrDefault() : "",
                    JobleavingValue = o.Jobleaving != null ? db._dbContext.Status.Where(i => i.StatusId == o.Jobleaving).Select(i => i.Name).FirstOrDefault() : "",


                    o.Purposeofloan,
                    o.HomeVisitPlan,
                    o.Jobleaving,

                    o.UaeReferenceName,
                    o.UaeReferenceNo,
                    o.NameofSpouse,
                    o.CellNumberSpouse,

                    o.otherJobleaving,
                    o.otherPurposeofloan,
                    CreatedBy = o.CreatedBy != null ? db._dbContext.SMIM_UserMst_STs.Where(i => i.UserId == o.CreatedBy).Select(i => i.UserName).FirstOrDefault() : ""

                }).FirstOrDefault();

            if (getAdvanceSalaryDtlInfo == null)
                return BadResponse("Record Not Available");

            return SuccessResponse(getAdvanceSalaryDtlInfo);

        }

        public DTO GetAdditinoalQuestionStatus()
        {

            var homeVisitPlanStatusType = Common.GetStatusType(Enums.StatusTypes.HomeVisitPlan);
            var purposeofLoanStatusType = Common.GetStatusType(Enums.StatusTypes.PurposeofLoan);
            var jobLeavingPlanStatusType = Common.GetStatusType(Enums.StatusTypes.JobLeavingPlan);

            if (homeVisitPlanStatusType == null && purposeofLoanStatusType == null && jobLeavingPlanStatusType == null)
                return BadResponse("Status Type Not Available");


            var homeVisitPlan = Common.GetStatusByTypeId(homeVisitPlanStatusType.StatustypeId).Select(o => new
            {
                o.StatusId,
                o.Name,
                o.TypeId,

            }).ToList();

            var purposeofLoan = Common.GetStatusByTypeId(purposeofLoanStatusType.StatustypeId).Select(o => new
            {
                o.StatusId,
                o.Name,
                o.TypeId,

            }).ToList();

            var jobLeavingPlan = Common.GetStatusByTypeId(jobLeavingPlanStatusType.StatustypeId).Select(o => new
            {
                o.StatusId,
                o.Name,
                o.TypeId,

            }).ToList();


            var response = new
            {
                homeVisitPlanStatus = homeVisitPlan,
                purposeofLoanStatus = purposeofLoan,
                jobLeavingPlanStatus = jobLeavingPlan,
            };
            return SuccessResponse(response);
        }

    }
}