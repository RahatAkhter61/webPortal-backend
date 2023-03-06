using ContextMapper;
using CORE_API.Areas.HRPR.Models;
using CORE_API.General;
using CORE_API.Services.SMAM;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.HRMS
{
    public class USPExportService : CastService<HRMS_USPExport_HI>
    {
        dbEngine db = new dbEngine();
        public USPExportService()
        {

        }
        public DTO UpdateUSPExport(int ExportId)
        {
            try
            {
                if (ExportId != 0)
                    GetByPrimaryKey(ExportId);
                if (ExportId == 0 || ExportId == null)
                    PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                {
                    New();
                }

                Current.CardDeliveredOn = DateTime.Now;
                Current.CardDeliveredStatus = 2;

                Save();
                AddUSPExport(Current);
                return SuccessResponse("Saved Successfully");
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }

        public DTO UpdateUSPStatus(int ExportId)
        {
            try
            {
                if (ExportId != 0)
                    GetByPrimaryKey(ExportId);
                if (ExportId == 0 || ExportId == null)
                    PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                {
                    New();
                }

                Current.CardDeliveredOn = DateTime.Now;
                Current.CardDeliveredStatus = 1;

                Save();
                return SuccessResponse("Saved Successfully");
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }

        public DTO UpdateTermsCondition(int ExportId, string IsAgree, string IsAutoDelivery)
        {
            try
            {
                if (ExportId != 0)
                    GetByPrimaryKey(ExportId);
                Current.AgreeTerms = IsAgree == "true" ? true : false;
                Current.AutoDelivery = IsAutoDelivery == "true" ? true : false;
                Save();
                return SuccessResponse("Saved Successfully");
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }

        public DTO UpdateMobileNo(string MobileNo, int ExpoortId)
        {
            try
            {
                if (ExpoortId != 0)
                    GetByPrimaryKey(ExpoortId);

                bool flag = true;
               
                var getExport = new BLInfo<HRMS_USPExport_HI>().GetQuerable<HRMS_USPExport_HI>().Where(a => a.ExportId==ExpoortId).FirstOrDefault();
                var getAccount = getExport!=null ? db._dbEngineContext.FNGL_Accounts_TRs.FirstOrDefault(x=> x.WalletId==getExport.WalletId) : null;
                var getEmpId = getAccount != null ? db._dbContext.HRMS_EmployeeMst_STs.FirstOrDefault(x=> x.CustomerId==getAccount.CustomerId) : null;
                if (getEmpId != null)
                {
                    var isMobileAlreadyExistInEmpMst = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(a => a.EmpId != getEmpId.EmpId && a.MobileNo == MobileNo).FirstOrDefault();
                    if (isMobileAlreadyExistInEmpMst != null)
                    {
                        Current.MobileNo = "971500000000";
                        flag = false;
                        Logger.TraceService("Mobile no updated with default Number Exist In HRMS_EmployeeMst_ST " + MobileNo, "DeliveryLogInfo");
                    }
                    var getEmployeeEmiratesId = new BLInfo<HRMS_EmpDocs_ST>().GetQuerable<HRMS_EmpDocs_ST>().Where(a => a.EmpId == getEmpId.EmpId && a.DocId == 6).FirstOrDefault();
                    if (getEmployeeEmiratesId != null)
                    {
                        var checkUserLogin = db._dbEngineContext.SRCC_UserLogin_ST.FirstOrDefault(x => x.EmiratesId != getEmployeeEmiratesId.DocNo && x.MobileNo == MobileNo);
                        if (checkUserLogin != null)
                        {
                            Current.MobileNo = "971500000000";
                            flag = false;
                            Logger.TraceService("Mobile no updated with default Number Exist In SRCC_UserLogin_ST " + MobileNo, "DeliveryLogInfo");
                        }
                    }
                }
                if(flag)
                    Current.MobileNo = MobileNo;

                Save();
                Logger.TraceService("Mobile no updated " + MobileNo, "DeliveryLogInfo");
                return SuccessResponse("Updated Mobile no Successfully");
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }

        public DTO AddUSPExport(HRMS_USPExport_HI obj)
        {
            try
            {

                PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                {
                    New();
                }
                Current.WalletId = obj.WalletId;
                Current.CustomerName = obj.CustomerName;
                Current.Address1 = obj.Address1;
                Current.Address2 = obj.Address2;
                Current.MobileNo = obj.MobileNo;
                Current.CardProgramName = obj.CardProgramName;
                Current.CardPurchase = obj.CardPurchase;
                Current.Contactless = obj.Contactless;
                Current.Withdrawl = obj.Withdrawl;
                Current.Apptransaction = obj.Apptransaction;

                Current.CardId = obj.CardId;
                Current.SentToUSP = obj.SentToUSP;
                Current.SentOn = obj.SentOn;
                Current.BatchId = obj.BatchId;
                Current.CardPrintedBatch = obj.CardPrintedBatch;
                Current.CardPrintedDate = obj.CardPrintedDate;
                Current.CardCollectedBatch = obj.CardCollectedBatch;
                Current.CardCollectedBy = obj.CardCollectedBy;
                Current.CardCollectedOn = obj.CardCollectedOn;


                Current.CardAssignToExecBatch = null;
                Current.CardAssignToExec = null;
                Current.CardAssignToExecOn = null;
                Current.CardDeliveredBatch = null;
                Current.CardDeliveredOn = null;
                Current.CardDeliveredStatus = null;
                Current.ExportId = 0;
                Save();


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

        public DTO Get_SPCardSentOnPrinted(int? UserId, DateTime Date)
        {
            try
            {
                string[] paramater = { "UserId", "Date" };
                DataTable dt = ExecuteSp("SP_CardSentOnPrinted", paramater, UserId, Date);
                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO UpdatePrintedStatus(IList<UpdatePrintedStatus> obj)
        {
            try
            {

                int CardPrintedBatch = (new Random()).Next(100, 1000);
                DateTime CardPrintedDate = DateTime.Now;

                int BatchId = 0;
                int FailedEmployees = 0;

                //string[] paramate = { "BatchNo", "FileName", "CompanyId", "TotalRecords", "CreatedBy"};
                //DataTable dt1 = ExecuteSp("SP_InsertEmpbatch", paramate, CardPrintedBatch, null, null, obj.Count, obj[0].UserId);

                foreach (var item in obj)
                {
                    GetByPrimaryKey(item.ExportId);
                    if (PrimaryKeyValue != null)
                    {
                        string[] paramater = { "ExportId", "EmpId", "BatchId", "WalletId", "CardBatchNo", "CardDate", "CreatedBy", "CardStatus", "ExecutivePerson" };
                        DataTable dt = ExecuteSp("SP_UpdateCardStatus", paramater, item.ExportId, item.EmpId, BatchId, item.WalletID, CardPrintedBatch, CardPrintedDate, item.UserId, item.CardStatus, item.ExecutivePerson);

                        if (dt != null)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                FailedEmployees = Convert.ToInt32(row["FailedEmployees"]);
                                BatchId = Convert.ToInt32(row["BatchId"]);
                            }
                        }

                    }

                }

                return this.SuccessResponse("Update Successfully");

            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO CollectedCardsForAssign(int? UserId, int CompanyId, int CampId, string WalletId)
        {
            try
            {
                string[] paramater = { "UserId", "CompanyId", "CampId", "WalletId" };
                DataTable dt = ExecuteSp("sp_GetCollectedCardsForAssign", paramater, UserId, CompanyId, CampId, WalletId);
                return this.SuccessResponse(dt);

            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }

        }

        public DTO AssignToExecutive(IList<AssignToExecutive> obj)
        {
            try
            {

                int CardBatchNo = (new Random()).Next(100, 1000);
                int BatchId = 0;
                List<int> FailedRecord = new List<int>();

                var arrayList = new List<updateActivateStatusModel>();
                if (obj.Count == 0) { return this.BadResponse("No Record Found..!"); }

                // UPDATE CARD STATUS IN (CORE SYSTEM)
                foreach (var item in obj)
                {
                    GetByPrimaryKey(item.ExportId);
                    if (PrimaryKeyValue == null) { }

                    else
                    {
                        string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();
                        string[] paramater = { "ExportId", "EmpId", "BatchId", "WalletId", "CardBatchNo", "CardDate", "CreatedBy", "CardStatus", "ExecutivePerson" };
                        DataTable dt = ExecuteStoreProc("SP_UpdateCardStatus", paramater, Connection, item.ExportId, item.EmpId, BatchId, item.WalletID, CardBatchNo, item.ExecutiveDate, item.UserId, item.CardStatus, item.ExecutivePerson);

                        if (dt != null)
                        {
                            foreach (DataRow row in dt.Rows)
                            {

                                int _RecordField = Convert.ToInt32(row["FailedEmployees"]);

                                if (_RecordField > 0)
                                {
                                    FailedRecord.Add(_RecordField);
                                }
                                else
                                {
                                    BatchId = Convert.ToInt32(row["BatchId"]);
                                    var jsonResult = new updateActivateStatusModel()
                                    {
                                        CardNo = Convert.ToString(row["CardNo"]),
                                        WalletId = Convert.ToString(row["WalletId"]),
                                        UserName = Convert.ToString(row["UserName"]),
                                    };
                                    
                                    arrayList.Add(jsonResult);
                                }

                            }
                        }
                    }
                }

                var result = arrayList.Where(o => o.UserName != string.Empty).ToList();
                if (result.Count > 0)
                {
                    return this.SuccessResponse(result);
                }
                return this.SuccessResponse("Update Successfully");

            }
            catch (Exception ex)
            {
                Logger.TraceService($"ERROR" + ex.Message, "AssignToExecutive");
                return this.BadResponse(ex.Message);
            }
        }

    }

}
