using ContextMapper;
using CORE_API.Areas.SMAM.Models;
using CORE_API.General;
using CORE_API.Services.FNGL;
using CORE_API.Services.SMAM;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using TransactionManager = CORE_API.Services.FNGL.TransactionManager;

namespace CORE_API.Services.HRMS
{
    public class CampService : CastService<SMAM_Camp_ST>
    {
        private readonly ProjectLocationService _projSer;
        private readonly TransactionManager _transactionManager;
        public CampService()
        {
            _projSer = new ProjectLocationService();
            _transactionManager = new TransactionManager();

        }


        public DTO SubmitCamp(SMAM_Camp_ST obj, IList<SMAM_CampEmp_ST> objempcamp, DateTime EffFrom, DateTime EffTo)
        {
            try
            {
                if (obj.CampId != 0)
                {
                    GetByPrimaryKey(obj.CampId);
                    Current.ModifiedBy = obj.CreatedBy;
                    Current.ModifiedOn = DateTime.Now;

                }
                if (obj.CampId == 0 || obj.CampId == null)

                    PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                {
                    New();
                    Current.CreatedBy = obj.CreatedBy;
                    Current.CreatedOn = DateTime.Now;
                }

                Current.CountryId = obj.CountryId;
                Current.StateId = obj.StateId;
                Current.Title = obj.Title;
                Current.CompanyId = obj.CompanyId;
                Current.Description = obj.Description;

                Save();
                if (objempcamp.Count() > 0)
                    _projSer.SubmitProjectLocation(objempcamp, Current.CampId, EffFrom, EffTo);
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

        public DTO SP_CampAllocation(SMAM_Camp_ST obj)
        {

            try
            {
                int? CampId = null;
                string[] paramater = { "CampId", "CompanyId", "Title", "Description", "Address", "StateId", "CountryId", "CreatedBy" };
                DataTable dt = ExecuteSp("SP_AddCampAllocation", paramater, obj.CampId, obj.CompanyId, obj.Title, obj.Description, obj.Address, obj.StateId, obj.CountryId, obj.CreatedBy);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        CampId = Convert.ToInt32(row["CampId"]);
                    }
                }

                var jsonResult = new { CampId = CampId };
                return this.SuccessResponse(jsonResult);

            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        public DTO SP_GETCampAllocation(int? UserId)
        {

            try
            {
                string[] paramater = { "UserId" };
                DataTable dt = ExecuteSp("SP_GetAllCampAllocation", paramater, UserId);
                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        public DTO GET_ForCamp(int companyId)
        {
            try
            {
                string[] paramater = { "Companyid" };
                DataTable dt = ExecuteSp("SP_ForCamp", paramater, companyId);
                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        public DTO GET_CampbyEmp(int CampId)
        {
            try
            {
                string[] paramater = { "CampId" };
                DataTable dt = ExecuteSp("GET_SP_EmpbyCamp", paramater, CampId);
                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        public DTO Insert_CampEmp(List<SMAM_CampEmp_ST> obj)
        {
            try
            {
                DataTable dt = null;
                foreach (var item in obj)
                {
                    string[] paramater = { "CampId", "EmpId", "EffFrom", "Effto", "IsActive" };
                    dt = ExecuteSp("SP_InsertNewCampEmp", paramater, item.CampId, item.EmpId, item.EffFrom, item.EffTo, item.IsActive);
                }
                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        public DTO SP_TranferAmountToCompany(TransferAmountToCompany obj)
        {
            try
            {
                string[] paramater = { "UserId", "FromCompanyId", "ToCompanyId", "FromCompanybalnce", "ToCompanybalnce", "Transferamount", "Description" };
                DataTable dt = ExecuteSp("SP_UpdateCompanyToCompanyBalance", paramater, obj.UserId, obj.FromCompanyId, obj.ToCompanyId, obj.FromCompanybalnce, obj.ToCompanybalnce, obj.Transferamount, obj.Description);
                return this.SuccessResponse("Saved Succesfully");
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        public DTO AddTransferOfFunds(TransferOfFundsRequestModel model)
        {
            try
            {

                if (model != null && model.TransferDetails != null && model.TransferDetails.Count > 0)
                {
                    var fundTransferTypeDetail = _transactionManager.GetTransactionType(Enums.TransactionTypes.CompanyFundtransfer);
                    var addingFundsTypeDetail = _transactionManager.GetTransactionType(Enums.TransactionTypes.AddingFunds);
                    var companyCurrentBalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().FirstOrDefault(x => x.CompanyId == model.CompanyId);
                    if (companyCurrentBalance == null || companyCurrentBalance.Balance < model.TransferDetails.Sum(x => x.Amount))
                        return BadResponse("Company balance is not sufficient");
                    string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsetting"].ToString();

                    string queryToAddFunds = "INSERT INTO dbo.CompanyTransferOfFunds (CompanyId, ToCompanyId, TransferAmount,CreatedBy,CreatedOn) " +
                      " output INSERTED.TransferId VALUES (@CompanyId, @ToCompanyId, @TransferAmount,@CreatedBy,@CreatedOn)";
                    string queryToMakeDebit = "INSERT INTO dbo.FNGL_Transactions_HI (TransTypeId, CompanyId, DebitAmount,CreditAmount,TransDate,TransDesc,TransReference,TransferId) " +
                     " output INSERTED.TransId VALUES (@TransTypeId, @CompanyId, @DebitAmount,@CreditAmount,@TransDate,@TransDesc,@TransReference,@TransferId)";
                    string queryToDeductBalance = "update SMAM_CompanyMst_ST set Balance = Balance - @Amount OUTPUT INSERTED.CompanyId where CompanyID = @CompanyId ";

                    string queryToMakeCredit = "INSERT INTO dbo.FNGL_Transactions_HI (TransTypeId, CompanyId, DebitAmount,CreditAmount,TransDate,TransDesc,TransReference,TransferId) " +
                    " output INSERTED.TransId VALUES (@TransTypeId, @CompanyId, @DebitAmount,@CreditAmount,@TransDate,@TransDesc,@TransReference,@TransferId)";
                    var lastAddedTransferOfFundsNumber = _transactionManager.GetLastTransferNumberAdded(Enums.TransactionTypes.CompanyFundtransfer);
                    var lastAddedTransferOfAddingFundsNumber = _transactionManager.GetLastTransferNumberAdded(Enums.TransactionTypes.AddingFunds);

                    using (TransactionScope transactionScope = new TransactionScope())
                    {

                        using (SqlConnection conn = new SqlConnection(Connection))
                        {

                            conn.Open();

                            int transferId, debitTransactionId, creditTransactionId, deductedCompanyId, addedCompanyId;

                            foreach (var item in model.TransferDetails)
                            {
                                transferId = 0;
                                debitTransactionId = 0;
                                creditTransactionId = 0;
                                deductedCompanyId = 0;
                                addedCompanyId = 0;
                                var transferRefernceForFundTransfer = _transactionManager.CreateTransferNumber(lastAddedTransferOfFundsNumber);
                                var transferReferenceNumberForAddingFunds = _transactionManager.CreateTransferNumber(lastAddedTransferOfAddingFundsNumber);
                                #region Create Transfer 

                                using (SqlCommand cmd = new SqlCommand(queryToAddFunds, conn))
                                {
                                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = model.CompanyId;
                                    cmd.Parameters.Add("@ToCompanyId", SqlDbType.Int).Value = item.ToCompanyId;
                                    cmd.Parameters.Add("@TransferAmount", SqlDbType.Decimal).Value = item.Amount;
                                    cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = item.CreatedBy;
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
                                {

                                    return BadResponse("Transaction Type does not exist we can't make the transfers");
                                }

                                using (SqlCommand cmd = new SqlCommand(queryToMakeDebit, conn))
                                {
                                    cmd.Parameters.Add("@TransTypeId", SqlDbType.Int).Value = fundTransferTypeDetail.TransTypeId;
                                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = model.CompanyId;
                                    cmd.Parameters.Add("@DebitAmount", SqlDbType.Decimal).Value = item.Amount;
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
                                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = item.Amount;
                                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = model.CompanyId;
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
                                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = item.ToCompanyId;
                                    cmd.Parameters.Add("@DebitAmount", SqlDbType.Decimal).Value = 0M;
                                    cmd.Parameters.Add("@CreditAmount", SqlDbType.Int).Value = Convert.ToDecimal(item.Amount);
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
                                string queryToAddBalance = "update SMAM_CompanyMst_ST set Balance = Balance + @Amount  OUTPUT INSERTED.CompanyId where CompanyID = @CompanyId";
                                using (SqlCommand cmd = new SqlCommand(queryToAddBalance, conn))
                                {
                                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = item.Amount;
                                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = item.ToCompanyId;
                                    if (conn.State == ConnectionState.Closed)
                                        conn.Open();
                                    addedCompanyId = (int)cmd.ExecuteScalar();

                                }
                                #endregion

                                if (debitTransactionId == 0 || creditTransactionId == 0 || deductedCompanyId == 0 || addedCompanyId == 0)
                                {
                                    if (conn.State == ConnectionState.Closed)
                                        conn.Open();
                                    return BadResponse("SomeThing Went Wrong We Can'nt Make The Transfer");
                                }
                                lastAddedTransferOfFundsNumber = transferRefernceForFundTransfer;
                                lastAddedTransferOfAddingFundsNumber = transferReferenceNumberForAddingFunds;

                            }

                            if (conn.State == ConnectionState.Closed)
                                conn.Open();
                            transactionScope.Complete();
                            conn.Close();
                            conn.Dispose();
                            return SuccessResponse("Fund Transfer Successfully");

                        }

                    }
                }
                return BadResponse("Please fill the information correctly");
            }
            catch (Exception ex)
            {

                return BadResponse(ex.Message);
            }
        }

    }

}
