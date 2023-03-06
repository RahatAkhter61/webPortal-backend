using ContextMapper;
using CORE_API.Areas.PCMS.Model;
using CORE_API.General;
using CORE_API.Models;
using CORE_API.Models.ComapnyWhiteListing;
using CORE_API.Models.Transaction;
using CORE_API.Services.FNGL;
using CORE_API.Services.HRPR;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using TransactionManager = CORE_API.Services.FNGL.TransactionManager;

namespace CORE_API.Services.PCMS
{
    public class CashLoadService : CastService<PCMS_Cashload_Master>
    {
        dbEngine dbEngine = new dbEngine();

        private readonly AccountTransactionService _accountTran;
        private readonly TransactionManager _transactionManager;

        public CashLoadService()
        {
            _accountTran = new AccountTransactionService();
            _transactionManager = new TransactionManager();
        }
        public DTO GetCashloadinfo(int Companyid, int Productid)
        {
            try
            {
                var Balance = string.Empty;

                var emplist = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(o => o.IsActive == true && o.IsArchive == false).ToList();
                var account = dbEngine._dbEngineContext.FNGL_Accounts_TRs.ToList();

                Balance = Companyid != null ? new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(o => o.CompanyId == Companyid).Select(o => o.Balance).FirstOrDefault().ToString() : "0";



                var result = (from ep in emplist
                              join acc in account on ep.CustomerId equals acc.CustomerId
                              where ep.CompanyId == Companyid && ep.ProductId == Productid
                              select new
                              {

                                  EmpId = ep.EmpId,
                                  PersonalNo = ep.PersonalNo,
                                  AccountName = ep.FirstName?.Trim() + " " + ep.LastName?.Trim(),
                                  Walletid = acc.WalletId,
                                  CustomerId = acc.CustomerId,
                                  Amount = 0,
                                  AccountId = acc.AccountId,
                                  Description = "",
                                  IsActive = ep.IsActive,
                                  IsArchive = ep.IsArchive

                              }).ToList();

                var data = new
                {
                    Data = result,
                    CompanyBalance = Balance,
                };
                return this.SuccessResponse(data);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }

        }
        public DTO SubmitCashload(PCMS_Cashload_Master obj, CalculateMultipleChargeBindingModel chargeModel)
        {
            try
            {

                New();
                Current.Companyid = obj.Companyid;
                Current.Productid = obj.Productid;
                Current.Isfreezone = obj.Isfreezone;
                Current.TotalRecords = obj.PCMS_Cashload_Details.Count;
                Current.TotalAmount = obj.TotalAmount;
                Current.FilePath = obj.FilePath;
                Current.Status = obj.Status;
                Current.Cardloadtype = obj.Cardloadtype;
                Current.Modifyon = obj.Modifyon;
                Current.Createdon = obj.Createdon;
                Current.Createdby = obj.Createdby;
                Current.IsFundtransfer = false;
                Current.PCMS_Cashload_Details = obj.PCMS_Cashload_Details;
                Save();


                foreach (var chargeIds in chargeModel.ChargeIds)
                {
                    var chargeObject = new ChargeTransaction
                    {
                        TotalAmount = obj.TotalAmount,
                        ChargeType = int.Parse($"{chargeIds}"),
                        TransactionType = Enums.TransactionTypes.CompanyCharge,
                        CompanyId = obj.Companyid,
                        CashLoadId = Current.CashloadMasterId,
                        IsDeducted = chargeModel.IsDeducted.Equals("True") ? true : false,
                        CreatedBy = obj.Createdby,
                        ProductId = chargeModel.ProductId

                    };
                    var performCharge = _accountTran.PerformSifCharges(chargeObject);
                }

                return this.SuccessResponse("Saved Successfully");
            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }
        }
        public DTO GetCashloadStatus()
        {
            var rslt = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(a => a.Statusid != 0).ToList();
            //var Cardload = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(a => a.Statusid != 0 ).ToList();
            //var MTP_Trans = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(a => a.Statusid != 0 ).ToList();

            //var rslt = new
            //{
            //    CashloadStatus = Cashload,
            //    Cardloadtype = Cardload,
            //    MTP_Trans = MTP_Trans
            //};
            return this.SuccessResponse(rslt);
        }
        public DTO GetCashload(int UserId, DateTime Fromdate, DateTime Todate)
        {

            try
            {

                var Cashloadlst = (from t1 in dbEngine._dbContext.PCMS_Cashload_Masters
                                   join t2 in dbEngine._dbContext.SMAM_CompanyMst_STs on t1.Companyid equals t2.CompanyId
                                   join t3 in dbEngine._dbContext.SMAM_Products_STs on t1.Productid equals t3.ProductId
                                   join t4 in dbEngine._dbContext.PCMS_Status_CLs on t1.Status equals t4.Statusid into tempstatus
                                   join t6 in dbEngine._dbContext.SMIM_UserMst_STs on t1.Createdby equals t6.UserId into tempUser

                                   from t5 in tempstatus.DefaultIfEmpty()
                                   from t7 in tempUser.DefaultIfEmpty()
                                   where t1.Createdon >= Fromdate && t1.Createdon <= Convert.ToDateTime(Todate).AddDays(1)


                                   select new
                                   {

                                       t1.CashloadMasterId,
                                       t1.Companyid,
                                       CompanyName = t2.Name,
                                       ProductName = t3.Description,
                                       Isfreezone = t1.Isfreezone != false ? "Yes" : "No",
                                       CashloadId = t1.CashloadMasterId,
                                       TotalAmount = t1.TotalAmount,
                                       TotalRecords = t1.TotalRecords,
                                       Cardloadtype = t1.Cardloadtype,
                                       Createdby = t7.UserName,
                                       Createdon = t1.Createdon,
                                       Status = t5.Description,

                                   }).ToList();




                //var Cashload = new BLInfo<PCMS_Cashload_Master>().GetQuerable<PCMS_Cashload_Master>().ToList();
                //var result = (from sa in Cashload
                //              select new
                //              {
                //                  CompanyId = sa.Companyid,
                //                  CashloadId = sa.CashloadMasterId != null ? sa.CashloadMasterId : 0,
                //                  CompanyName = sa.Companyid != 0 ? new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == sa.Companyid).FirstOrDefault().Name : "",
                //                  ProductName = sa.Productid != 0 ? new BLInfo<SMAM_Products_ST>().GetQuerable<SMAM_Products_ST>().Where(a => a.ProductId == sa.Productid).FirstOrDefault().Description : "",
                //                  Isfreezone = sa.Isfreezone != false ? "Yes" : "No",
                //                  TotalAmount = sa.TotalAmount,
                //                  TotalRecords = sa.TotalRecords,
                //                  Cardloadtype = sa.Cardloadtype != null ? sa.Cardloadtype : 0,
                //                  Createdon = sa.Createdon,
                //                  // Status = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(a =>  a.Statusid == sa.Status).FirstOrDefault().Description,
                //                  // Statusid = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(a => a.Statusid == sa.Status).FirstOrDefault().Statusid,
                //                  Status = (sa.Status != 0 || sa.Status != null) ? new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(a => a.Statusid == sa.Status).FirstOrDefault().Description : "",
                //                  Cashload_Details = sa.PCMS_Cashload_Details != null ?

                //                  (from dtl in new BLInfo<PCMS_Cashload_Detail>().GetQuerable<PCMS_Cashload_Detail>().Where(a => a.CashloadMasterId == sa.CashloadMasterId)
                //                   select new
                //                   {
                //                       AccountName = dtl.AccountName,
                //                       PersonalNo = dtl.PersonalNo,
                //                       WalletId = dtl.WalletId,
                //                       Amount = dtl.Amount,
                //                       Description = dtl.Description
                //                   }).ToList() : null

                //              }).OrderByDescending(o => o.CashloadId).ToList();

                if (Cashloadlst.Count == 0)
                    return BadResponse("No Record Found");

                return SuccessResponse(Cashloadlst);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }
        public DTO GetCashloaddetailbyId(int CashloadId)
        {

            try
            {

                var Cashloaddetailslist = new BLInfo<PCMS_Cashload_Detail>().GetQuerable<PCMS_Cashload_Detail>().Where(o => o.CashloadMasterId == CashloadId).Select(dtl => new
                {
                    AccountName = dtl.AccountName,
                    PersonalNo = dtl.PersonalNo,
                    WalletId = dtl.WalletId,
                    Amount = dtl.Amount,
                    Description = dtl.Description
                }).ToList();

                if (Cashloaddetailslist == null || Cashloaddetailslist.Count == 0)
                    return this.BadResponse("No Record Found");

                return SuccessResponse(Cashloaddetailslist);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }

        }
        public async Task<DTO> updateCasloadAsync(int CashloadId, int status, int CreatedBy)
        {
            try
            {

                var statusType = string.Empty;
                List<ApprovalRequestModel> list = new List<ApprovalRequestModel>();

                var cashLoadStatus = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(o => o.Statusid == status).FirstOrDefault();

                if (cashLoadStatus == null)
                    return BadResponse("Status Not Available");

                var cashLoadMaster = new BLInfo<PCMS_Cashload_Master>().GetQuerable<PCMS_Cashload_Master>().Where(o => o.CashloadMasterId == CashloadId).FirstOrDefault();
                
                if (cashLoadMaster == null)
                    return BadResponse("Record Not Found..!");

                if (cashLoadMaster.IsFundtransfer != null && cashLoadMaster.IsFundtransfer == true && cashLoadStatus != null && cashLoadStatus.Type.Equals(Enums.PCMSStatusType.Statustype) && cashLoadStatus.Description.Equals(Enums.PCMSStatus.Pending))
                {
                    var fundTransferTypeDetail = _transactionManager.GetTransactionType(Enums.TransactionTypes.CompanyFundtransfer);
                    var addingFundsTypeDetail = _transactionManager.GetTransactionType(Enums.TransactionTypes.AddingFunds);

                    var companydtl = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().FirstOrDefault(x => x.CompanyId == cashLoadMaster.Companyid);

                    if (companydtl == null)
                        return BadResponse(string.Format("Company not found. Id:{0}", cashLoadMaster.Companyid));

                    if (companydtl.ParentCompanyId == null)
                        return BadResponse(string.Format("Parent Company not found. Id:{0}", cashLoadMaster.Companyid));

                    var parentCompanydtl = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().FirstOrDefault(x => x.CompanyId == companydtl.ParentCompanyId);

                    if (parentCompanydtl == null || parentCompanydtl.Balance == null || parentCompanydtl.Balance < cashLoadMaster.TotalAmount)
                        return BadResponse("Parent Company have not enough balance to load centive cards");

                    string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsetting"].ToString();

                    string queryToAddFunds = "INSERT INTO dbo.CompanyTransferOfFunds (CompanyId, ToCompanyId, TransferAmount,CreatedBy,CreatedOn) " +
                      " output INSERTED.TransferId VALUES (@CompanyId, @ToCompanyId, @TransferAmount,@CreatedBy,@CreatedOn)";

                    string queryToMakeDebit = "INSERT INTO dbo.FNGL_Transactions_HI (TransTypeId, CompanyId, DebitAmount,CreditAmount,TransDate,TransDesc,TransReference,TransferId) " +
                     " output INSERTED.TransId VALUES (@TransTypeId, @CompanyId, @DebitAmount,@CreditAmount,@TransDate,@TransDesc,@TransReference,@TransferId)";

                    string queryToDeductParentCompanyBalance = "update SMAM_CompanyMst_ST set Balance = Balance - @Amount OUTPUT INSERTED.CompanyId where CompanyID = @CompanyId";

                    string queryToMakeCredit = "INSERT INTO dbo.FNGL_Transactions_HI (TransTypeId, CompanyId, DebitAmount,CreditAmount,TransDate,TransDesc,TransReference,TransferId) " +
                   " output INSERTED.TransId VALUES (@TransTypeId, @CompanyId, @DebitAmount,@CreditAmount,@TransDate,@TransDesc,@TransReference,@TransferId)";

                    string queryToAddChildCompanyBalance = "update SMAM_CompanyMst_ST set Balance = Balance + @Amount  OUTPUT INSERTED.CompanyId where CompanyID = @CompanyId";

                    string queryToStatusUpdate = "update PCMS_Cashload_Master set Status = @Status, Modifyon = @Modifyon  OUTPUT INSERTED.CashloadMasterId where CashloadMasterId = @CashloadMasterId";

                    string queryToDeductBalance = "update SMAM_CompanyMst_ST set Balance = Balance - @Amount OUTPUT INSERTED.CompanyId where CompanyID = @CompanyId";

                    var lastAddedTransferOfFundsNumber = _transactionManager.GetLastTransferNumberAdded(Enums.TransactionTypes.CompanyFundtransfer);
                    var lastAddedTransferOfAddingFundsNumber = _transactionManager.GetLastTransferNumberAdded(Enums.TransactionTypes.AddingFunds);

                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        int transferId, debitTransactionId, deductedCompanyId, creditTransactionId, addedCompanyId, cashLoadMasterId;

                        using (SqlConnection conn = new SqlConnection(Connection))
                        {

                            transferId = 0;
                            debitTransactionId = 0;
                            deductedCompanyId = 0;
                            creditTransactionId = 0;
                            addedCompanyId = 0;
                            cashLoadMasterId = 0;

                            var transferRefernceForFundTransfer = _transactionManager.CreateTransferNumber(lastAddedTransferOfFundsNumber);
                            var transferReferenceNumberForAddingFunds = _transactionManager.CreateTransferNumber(lastAddedTransferOfAddingFundsNumber);

                            conn.Open();

                            #region Create Transfer 
                            using (SqlCommand cmd = new SqlCommand(queryToAddFunds, conn))
                            {

                                cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companydtl.ParentCompanyId;
                                cmd.Parameters.Add("@ToCompanyId", SqlDbType.Int).Value = cashLoadMaster.Companyid;
                                cmd.Parameters.Add("@TransferAmount", SqlDbType.Decimal).Value = cashLoadMaster.TotalAmount;
                                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = CreatedBy;
                                cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;

                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                transferId = (int)cmd.ExecuteScalar();
                                conn.Close();
                            }
                            #endregion

                            #region MakeDeditTransaction 
                            if (transferId == 0)
                            {
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();

                                return BadResponse("We cant make transfer Something Went Wrong");
                            }

                            if (fundTransferTypeDetail == null)
                                return BadResponse("Transaction Type does not exist we can't make the transfers");

                            using (SqlCommand cmd = new SqlCommand(queryToMakeDebit, conn))
                            {
                                cmd.Parameters.Add("@TransTypeId", SqlDbType.Int).Value = fundTransferTypeDetail.TransTypeId;
                                cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companydtl.ParentCompanyId;
                                cmd.Parameters.Add("@DebitAmount", SqlDbType.Decimal).Value = cashLoadMaster.TotalAmount;
                                cmd.Parameters.Add("@CreditAmount", SqlDbType.Int).Value = 0M;
                                cmd.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@TransDesc", SqlDbType.NVarChar).Value = fundTransferTypeDetail.TransTypeDesc;
                                cmd.Parameters.Add("@TransReference", SqlDbType.NVarChar).Value = transferRefernceForFundTransfer;
                                cmd.Parameters.Add("@TransferId", SqlDbType.Int).Value = transferId;
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                debitTransactionId = (int)cmd.ExecuteScalar();

                            }
                            #endregion

                            #region Deduct Parent Company balance
                            if (debitTransactionId == 0)
                            {
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();

                                return BadResponse("We cant make transfer Something Went Wrong");
                            }
                            using (SqlCommand cmd = new SqlCommand(queryToDeductBalance, conn))
                            {
                                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = cashLoadMaster.TotalAmount;
                                cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companydtl.ParentCompanyId;
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                deductedCompanyId = (int)cmd.ExecuteScalar();

                            }
                            #endregion

                            #region MakeCreditTransaction
                            if (transferId == 0)
                            {
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();

                                return BadResponse("We cant make transfer Something Went Wrong");
                            }

                            if (addingFundsTypeDetail == null || debitTransactionId == 0 || deductedCompanyId == 0)
                            {
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();

                                return BadResponse("Something weant wrong we can't make the transaction");
                            }

                            using (SqlCommand cmd = new SqlCommand(queryToMakeCredit, conn))
                            {
                                cmd.Parameters.Add("@TransTypeId", SqlDbType.Int).Value = addingFundsTypeDetail.TransTypeId;
                                cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = cashLoadMaster.Companyid;
                                cmd.Parameters.Add("@DebitAmount", SqlDbType.Decimal).Value = 0M;
                                cmd.Parameters.Add("@CreditAmount", SqlDbType.Int).Value = Convert.ToDecimal(cashLoadMaster.TotalAmount);
                                cmd.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@TransDesc", SqlDbType.NVarChar).Value = addingFundsTypeDetail.TransTypeDesc;
                                cmd.Parameters.Add("@TransReference", SqlDbType.NVarChar).Value = transferReferenceNumberForAddingFunds;
                                cmd.Parameters.Add("@TransferId", SqlDbType.Int).Value = transferId;
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                creditTransactionId = (int)cmd.ExecuteScalar();
                                conn.Close();
                            }
                            #endregion

                            #region AddBalanceInChildCompany
                            if (debitTransactionId == 0 || creditTransactionId == 0 || deductedCompanyId == 0)
                            {
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                return BadResponse("We cant make transfer Something Went Wrong");
                            }

                            using (SqlCommand cmd = new SqlCommand(queryToAddChildCompanyBalance, conn))
                            {
                                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = cashLoadMaster.TotalAmount;
                                cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = cashLoadMaster.Companyid;
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                addedCompanyId = (int)cmd.ExecuteScalar();

                            }
                            #endregion

                            #region Update PCMS Status
                            if (debitTransactionId == 0 || creditTransactionId == 0 || deductedCompanyId == 0)
                            {
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                return BadResponse("We cant make transfer Something Went Wrong");
                            }

                            using (SqlCommand cmd = new SqlCommand(queryToStatusUpdate, conn))
                            {
                                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = cashLoadStatus.Statusid;
                                cmd.Parameters.Add("@Modifyon", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@CashloadMasterId", SqlDbType.Int).Value = cashLoadMaster.CashloadMasterId;
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                cashLoadMasterId = (int)cmd.ExecuteScalar();

                            }
                            #endregion

                            if (debitTransactionId == 0 || creditTransactionId == 0 || deductedCompanyId == 0 || addedCompanyId == 0 || cashLoadMasterId == 0)
                            {
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                return BadResponse("SomeThing Went Wrong We Can'nt Make The Transfer");
                            }


                            if (conn.State == ConnectionState.Closed)
                                conn.Open();
                            transactionScope.Complete();
                            conn.Close();
                            conn.Dispose();

                        }
                    }
                }

                else
                {
                    if (cashLoadMaster != null)
                        GetByPrimaryKey(cashLoadMaster.CashloadMasterId);

                    if (PrimaryKeyValue != null)
                    {
                        Current.Status = cashLoadStatus.Statusid;
                        Current.Modifyon = DateTime.Now;
                        Save();
                    }
                }

                return SuccessResponse("Save Successful");
            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }

        }

    }
}