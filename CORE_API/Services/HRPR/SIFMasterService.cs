using ContextMapper;
using CORE_API.General;
using CORE_API.Models;
using CORE_API.Services.FNGL;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.HRPR
{
    public class SIFMasterService : CastService<HRPR_SifMaster_TR>
    {
        private readonly SIFDetailService _sifDetail;
        private readonly TransactionManager _transactionManager;
        public SIFMasterService()
        {
            _sifDetail = new SIFDetailService();
            _transactionManager = new TransactionManager();
        }

        public DTO SubmitSIFMaster(HRPR_SifMaster_TR obj, IList<HRPR_SifDetail_TR> objsifDetail)
        {
            try
            {
                if (obj.SifId != 0)
                {
                    GetByPrimaryKey(obj.SifId);

                }
                if (obj.SifId == 0 || obj.SifId == null)

                    PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                {
                    New();
                    Current.CreatedBy = obj.CreatedBy;
                    Current.CreatedOn = DateTime.Now;
                }



                Current.CompanyID = obj.CompanyID;
                Current.SifDate = DateTime.Now;
                DateTime? Date = obj.SifDate;
                Common.DateHandler(ref Date);

                Current.Month = Date != null ? Date.Value.ToString("MM", CultureInfo.InvariantCulture) : "";   //bug not accepting month name
                Current.Year = Date != null ? Date.Value.ToString("yyyy", CultureInfo.CreateSpecificCulture("es")) : "";
                Current.TotalEmployees = obj.TotalEmployees;
                Current.TotalSalary = Convert.ToDecimal(obj.TotalSalary);
                Current.Reason = obj.Reason;

                Save();
                if (objsifDetail.Count() > 0)
                    _sifDetail.SubmitSIFDETAIL(objsifDetail, Current.SifId, Current.CompanyID, obj.CreatedBy);
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

        public DTO GetSIFData(int UserId, DateTime? Date)
        {
            try
            {
                string[] paramater = { "UserId", "Date" };
                DataTable dt = ExecuteSp("SP_GetSIFGenerationlst", paramater, UserId, Date);
                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        public DTO GetSIFDetailsbyId(int ExpSifId)
        {
            try
            {
                string[] paramater = { "ExpSifId" };
                DataTable dt = ExecuteSp("SP_GetSIFDetailbyId", paramater, ExpSifId);
                return this.SuccessResponse(dt);

            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO WPS_PushDatatoWPSTable(int ExpSifId)
        {
            try
            {
                string[] paramater = { "ExpSifId" };
                string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();
                DataTable dt = ExecuteStoreProc("SP_WPS_PushDatatoWPSTable", paramater, Connection, ExpSifId);
                return this.SuccessResponse("Approve Succesfully");
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }

        }

        public DTO WPS_Status(int ExpSifId)
        {
            try
            {
                string[] paramater = { "ExpSifId" };
                string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();
                DataTable dt = ExecuteStoreProc("SP_GetWPS_Status", paramater, Connection, ExpSifId);
                return this.SuccessResponse(dt);

            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }

        }

        public DTO SifNakClaim(SifNakClaimedModel obj)
        {


            dbEngine db = new dbEngine();

            #region HRPR_ExportSIFFiles_HI update claim status 
            var claimed = db._dbContext.HRPR_ExportSIFFiles_HIs.Where(o => o.ExpSifID == obj.ExpSifID).FirstOrDefault();

            if (claimed == null)
                return BadResponse("Export Sif Not Found");

            claimed.Claimstatus = true;
            #endregion

            #region Update Company Balance
            var companyInfo = db._dbContext.SMAM_CompanyMst_STs.Where(o => o.CompanyId == claimed.CompanyId).FirstOrDefault();
            if (companyInfo == null)
                return BadResponse("Company Not Found");

            companyInfo.Balance = companyInfo.Balance != null ? companyInfo.Balance + obj.ClaimedAmount : 0 + obj.ClaimedAmount;
            #endregion

            #region Adding company ledger 

            var transactionType = _transactionManager.GetTransactionTypebyCode(Enums.TransactionTypeCode.sifNakClaimed);
            var lastTransaction = db._dbContext.FNGL_Transactions_HIs.OrderByDescending(o => o.TransId).FirstOrDefault();
            var TransReference = lastTransaction != null ? lastTransaction.TransReference : null;

            var transferRefernceForFundTransfer = _transactionManager.CreateTransferNumber(TransReference);

            FNGL_Transactions_HI model = new FNGL_Transactions_HI();

            model.CompanyId = claimed.CompanyId;
            model.TransTypeId = transactionType.TransTypeId;
            model.DebitAmount = 0M;
            model.TransDate = DateTime.Now;
            model.TransDesc = transactionType.TransTypeDesc;
            model.TransReference = transferRefernceForFundTransfer;
            model.SifId = obj.ExpSifID;
            model.CreditAmount = obj.ClaimedAmount;
            db._dbContext.FNGL_Transactions_HIs.InsertOnSubmit(model);
            #endregion

            db._dbContext.SubmitChanges();
            return SuccessResponse("SIF Claimed Successful");
        }
    }
}
