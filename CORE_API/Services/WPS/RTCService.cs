using ContextMapper;
using CORE_API.General;
using CORE_API.Models.WPS;
using CORE_API.Services.FNGL;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using TransactionManager = CORE_API.Services.FNGL.TransactionManager;

namespace CORE_API.Services.WPS
{
    public class RTCService : CastService<RTC_Master>
    {

        dbEngine _dbEngine = new dbEngine();
        private readonly TransactionManager _transactionManager;
        private object conn;

        public RTCService()
        {
            _transactionManager = new TransactionManager();
        }

        #region Get ALL RTC Master Record
        public DTO GetAllRTC(DateTime? Received_On)
        {
            try
            {
                string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();
                string[] paramater = { "Received_On" };
                DataTable Returnlist = ExecuteStoreProc("SPGetAllRTC", paramater, Connection, Received_On);

                return this.SuccessResponse(Returnlist);
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message, "PAF");
                return this.BadResponse(ex.Message);
            }
        }
        #endregion

        #region Get GetRTCdetailbyId
        public DTO GetRTCdetailbyId(int Id)
        {
            try
            {
                if (Id == 0)
                    return BadResponse("Please Insert Correct Infromation");

                var returnRTCdetailsbyid = (from t1 in _dbEngine._WPSContext.RTC_Details
                                            join t2 in _dbEngine._WPSContext.WPS_RTC_Returncodes on t1.Return_Code_Reason equals t2.statuscode into tempstatus
                                            where t1.MasterId == Id
                                            from t3 in tempstatus.DefaultIfEmpty()

                                            select new
                                            {
                                                t1.SIF_WPS_Referrence,
                                                t1.Employer_Unique_Id,
                                                t1.Emp_Personal_Number,
                                                t1.Amount_Returned,
                                                t1.Return_Code_Reason,
                                                Reasonstatus = t3.shortcode,
                                                t1.SIF_Filename

                                            }).ToList();




                if (returnRTCdetailsbyid != null && returnRTCdetailsbyid.Count == 0)
                    return this.BadResponse("Record Not Found");

                return this.SuccessResponse(returnRTCdetailsbyid);
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message, "PAF");
                return this.BadResponse(ex.Message);
            }
        }
        #endregion

        #region update Status
        public DTO ProccedClaimed(proccedClaimedModel model)
        {
            try
            {
                if (model.Id != 0)
                {
                    var obj = _dbEngine._WPSContext.RTC_Masters.Where(o => o.Id == model.Id).FirstOrDefault();

                    if (obj == null)
                        return this.BadResponse("Record Not Found");

                    string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsetting"].ToString();
                    string WPSConnection = System.Configuration.ConfigurationManager.ConnectionStrings["WPSFileProcessing"].ToString();
                    var transactiontype = _transactionManager.GetTransactionType(Enums.TransactionTypes.RTCClaimed);

                    string queryToUpdateStatus = "update WPSFileProcessing.dbo.RTC_Master" +
                         " set RTC_rejected_status = @Claimed , Modifyon = @Modifyon , Modifyby = @Modifyby OUTPUT INSERTED.Id " +
                        " where Id = @Id and RTC_rejected_status = @NotClaimed";

                    string queryToMakeTransactions = "INSERT INTO CoreDb.dbo.FNGL_Transactions_HI (TransTypeId, CompanyId, CreditAmount,TransDate,TransDesc,TransReference) " +
                        "output INSERTED.TransId VALUES (@TransTypeId, @CompanyId, @CreditAmount,@TransDate,@TransDesc,@TransReference)";

                    string queryToUpdateCompanybalance = "update CoreDb.dbo.SMAM_CompanyMst_ST set Balance += @CreditAmount OUTPUT INSERTED.CompanyId where CompanyID = @CompanyId";

                    using (TransactionScope transactionScope = new TransactionScope())
                    {

                        int TransId = 0;
                        int RTCId = 0;
                        int CompanyId = 0;

                        #region Status update in WPS RTC_Masters
                        using (SqlConnection con = new SqlConnection(WPSConnection))
                        {

                            using (SqlCommand cmd = new SqlCommand(queryToUpdateStatus, con))
                            {
                                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;
                                cmd.Parameters.Add("@Claimed", SqlDbType.VarChar).Value = "Claimed";
                                cmd.Parameters.Add("@NotClaimed", SqlDbType.VarChar).Value = "Not Claimed";
                                cmd.Parameters.Add("@Modifyon", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@Modifyby", SqlDbType.Int).Value = model.UserId;

                                if (con.State == ConnectionState.Closed)
                                    con.Open();

                                object a = cmd.ExecuteScalar();
                                if (a == null)
                                    return BadResponse("Already Claimed");
                                RTCId = (int)a;
                            }
                        }
                        #endregion

                        using (SqlConnection conn = new SqlConnection(Connection))
                        {

                            #region Insert Record FNGL_Transactions_HI
                            conn.Open();
                            if (transactiontype == null)
                            {
                                transactionScope.Dispose();
                                return BadResponse("Transaction Type does not exist we can't make the transfers");
                            }

                            using (SqlCommand cmd = new SqlCommand(queryToMakeTransactions, conn))
                            {
                                cmd.Parameters.Add("@TransTypeId", SqlDbType.Int).Value = transactiontype.TransTypeId;
                                cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = model.CompanyId;
                                cmd.Parameters.Add("@CreditAmount", SqlDbType.Decimal).Value = model.Returned_Amount != null ? model.Returned_Amount : 0M;
                                cmd.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@TransDesc", SqlDbType.VarChar).Value = transactiontype.TransTypeDesc;
                                cmd.Parameters.Add("@TransReference", SqlDbType.VarChar).Value = obj.RTC_Filename;

                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                TransId = (int)cmd.ExecuteScalar();
                                conn.Close();
                            }
                            #endregion

                            #region Update Company Balance
                            using (SqlCommand cmd = new SqlCommand(queryToUpdateCompanybalance, conn))
                            {
                                cmd.Parameters.Add("@CreditAmount", SqlDbType.Int).Value = model.Returned_Amount;
                                cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = model.CompanyId;

                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();

                                object a = cmd.ExecuteScalar();
                                if (a == null)
                                    return BadResponse("Update Company Balance Field");
                                CompanyId = (int)a;
                                conn.Close();
                            }

                            #endregion

                        }

                        transactionScope.Complete();
                        return SuccessResponse("Successful Claimed");

                    }

                }

                return BadResponse("Please fill the information correctly");

            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }
        }
        #endregion

    }
}