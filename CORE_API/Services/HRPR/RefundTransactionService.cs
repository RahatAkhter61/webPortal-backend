using ContextMapper;
using CORE_API.General;
using CORE_API.Models.Transaction;
using CORE_API.Services.FNGL;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace CORE_API.Services.HRPR
{
    public class RefundTransactionService : CastService<FNGL_RefundTransactionTracking>
    {
        dbEngine db = new dbEngine();

        private readonly TransactionManager _transactionManager;
        public RefundTransactionService()
        {
            _transactionManager = new TransactionManager();
        }

        public DTO GetDebitTransactions(DateTime? FromDate, DateTime? Todate, DateTime? TransactionDate, string ReferenceNo, string WalletId, string MobileNo)
        {
            string[] paramater = { "FromDate", "Todate", "TransactionDate", "ReferenceNo", "WalletId", "MobileNo" };
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();

            DataTable dt = ExecuteStoreProc("SP_DebitTransactions", paramater, connection, FromDate, Todate, TransactionDate, ReferenceNo, WalletId, MobileNo);
            return this.SuccessResponse(dt);
        }

        public DTO GetRefundStatus()
        {
            var refundApprovalStatus = Common.GetStatusType(Enums.StatusTypes.RefundApproval);
            var debitRefundStatus = Common.GetStatusType(Enums.StatusTypes.DebitRefund);

            var debitRefundlist = Common.GetStatusByTypeId(debitRefundStatus.StatustypeId).Select(o => new
            {
                o.Name,
                o.TypeId,
                o.StatusId
            }).ToList();

            if (debitRefundlist == null && debitRefundlist.Count == 0)
                return BadResponse("debit Refund Status Not Found");

            var refundApprovallist = Common.GetStatusByTypeId(refundApprovalStatus.StatustypeId).Select(o => new
            {
                o.Name,
                o.TypeId,
                o.StatusId
            }).ToList();

            if (refundApprovallist == null && refundApprovallist.Count == 0)
                return BadResponse("debit Approval Status Not Found");

            var statusList = new
            {
                debitRefund = debitRefundlist,
                refundApproval = refundApprovallist
            };

            return SuccessResponse(statusList);
        }

        public DTO PostRefundTransaction(RefundTransactionModel model, HttpRequest httpContext)
        {


            var cardDebitReversal = db._dbEngineContext.SRCC_CardDebit_Reversals.Where(o => o.DebitId == model.DebitId).FirstOrDefault();

            if (cardDebitReversal != null)
                return BadResponse("Record Already In Process");


            var statusType = Common.GetStatusType(Enums.StatusTypes.RefundApproval);
            var status = Common.GetStatusByTypeId(statusType.StatustypeId);

            if (status.Count == 0)
                return BadResponse("Status Not Available");

            var RefundApprovalStatusId = status.Where(o => o.Name.Equals(Enums.StatusTypes.Pending)).FirstOrDefault().StatusId;

            if (httpContext.Files.Count == 0)
                return this.BadResponse("Please add Attachment");

            decimal? refundAmount = 0;

            var cardDebitTran = db._dbEngineContext.SRCC_CardDebitTrans_TRs.Where(o => o.DebitId == model.DebitId).FirstOrDefault();
            if (cardDebitTran == null)
                return BadResponse("Record Not Found");

            #region Refund Amount Calculation
            var refundAmountInfo = db._dbEngineContext.SRCC_CardDebitTrans_TRs.Where(o => o.DebitId == model.DebitId).Select(o => new
            {

                DebitAmount = String.Format("{0:.##}", (Convert.ToDecimal(o.DebitAmount) / 100)),
                FeeAmount = o.FeeAmount != null ? String.Format("{0:.##}", (Convert.ToDecimal(o.FeeAmount) / 100)) : "0",

            }).FirstOrDefault();

            decimal? totalAmount = Convert.ToDecimal(refundAmountInfo.DebitAmount) + Convert.ToDecimal(refundAmountInfo.FeeAmount);
            var DebitRefundStatusTitle = db._dbContext.Status.Where(o => o.StatusId == model.DebitRefundStatusId).FirstOrDefault().Name;

            // With Fee
            if (DebitRefundStatusTitle.Equals(Enums.StatusTypes.WithFee))
                refundAmount = totalAmount;

            //// Without Fee
            if (DebitRefundStatusTitle.Equals(Enums.StatusTypes.WithoutFee))
                refundAmount = Convert.ToDecimal(refundAmountInfo.DebitAmount);

            //// Partial Fee
            if (DebitRefundStatusTitle.Equals(Enums.StatusTypes.Partial))
            {
                if (totalAmount >= model.RefundAmount)
                    return this.BadResponse("Total Amount");

                refundAmount = model.RefundAmount;
            }

            #endregion

            #region Document Save
            DTO response;
            response = DocumentSave(httpContext);
            var proofImagePath = (string)response.data;
            #endregion

            if (response.isSuccessful == false)
                return BadResponse(response.errors);

            #region Card Debit Reversal

            SRCC_CardDebit_Reversal currentReversal = new SRCC_CardDebit_Reversal();

            currentReversal.IsCredited = false;
            currentReversal.InitiatedDate = DateTime.Now;
            currentReversal.TransReference = model.TransReference;
            currentReversal.TransDescription = model.Description;
            currentReversal.DateofFailure = model.DataOfFailure;
            currentReversal.DebitId = model.DebitId;
            currentReversal.ProofImagePath = "00 - Portal" + "/" + "DebitRefund_Tracking" + "/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + proofImagePath;
            currentReversal.RefundAmount = refundAmount;
            currentReversal.DebitRefundStatusId = model.DebitRefundStatusId;
            currentReversal.RefundApprovalStatusId = RefundApprovalStatusId;
            currentReversal.CreatedOn = DateTime.Now;
            currentReversal.CreatedBy = model.CreatedBy;

            db._dbEngineContext.SRCC_CardDebit_Reversals.InsertOnSubmit(currentReversal);
            db._dbEngineContext.SubmitChanges();

            #endregion

            #region Add Refund Transaction Trancking
            FNGL_RefundTransactionTracking currentTrancking = new FNGL_RefundTransactionTracking();

            currentTrancking.DebitId = model.DebitId;
            currentTrancking.Description = model.Description;
            currentTrancking.RefundAmount = model.RefundAmount;
            currentTrancking.RefundApprovalStatusId = RefundApprovalStatusId;
            currentTrancking.Createdby = model.CreatedBy;
            currentTrancking.Createdon = DateTime.Now;

            db._dbEngineContext.FNGL_RefundTransactionTrackings.InsertOnSubmit(currentTrancking);
            db._dbEngineContext.SubmitChanges();
            #endregion

            return SuccessResponse("Successfully Save");
        }

        public DTO RefundTransactionTracking(int? debitId)
        {

            var refundTrackingRecords = (from t1 in db._dbEngineContext.SRCC_CardDebit_Reversals
                                         join t2 in db._dbEngineContext.FNGL_RefundTransactionTrackings on t1.DebitId equals t2.DebitId

                                         where t1.DebitId == debitId
                                         select new
                                         {
                                             t1.DateofFailure,
                                             t2.RefundApprovalStatusId,
                                             t1.DebitRefundStatusId,
                                             t1.RefundAmount,
                                             t1.ProofImagePath,
                                             t2.Comment,
                                             t2.Createdby,
                                             t2.Description
                                         }).ToList();

            if (refundTrackingRecords.Count == 0)
                return BadResponse("No Account Transaction Found");


            var trackingRecords = (from t1 in refundTrackingRecords
                                   join t2 in db._dbContext.SMIM_UserMst_STs on t1.Createdby equals t2.UserId

                                   select new
                                   {
                                       t1.DateofFailure,
                                       t1.RefundApprovalStatusId,
                                       t1.DebitRefundStatusId,
                                       t1.RefundAmount,
                                       t1.ProofImagePath,
                                       t1.Comment,
                                       t1.Description,
                                       RequestedBy = t2.UserName,

                                   }).ToList();

            return SuccessResponse(trackingRecords);

        }

        public DTO GetDebitReversal()
        {


            var refundApprovalStatus = Common.GetStatusType(Enums.StatusTypes.RefundApproval).StatustypeId;
            var status = Common.GetStatusByTypeId(refundApprovalStatus).Where(o => o.Name.Equals(Enums.StatusTypes.Pending)).FirstOrDefault();

            if (status == null)
                return BadResponse("Status Not Available...!");

            var debitReversal = (from t1 in db._dbEngineContext.SRCC_CardDebit_Reversals
                                 join t2 in db._dbEngineContext.SRCC_CardDebitTrans_TRs on t1.DebitId equals t2.DebitId
                                 join t3 in db._dbEngineContext.FNGL_Accounts_TRs on t2.AccountId equals t3.AccountId
                                 //  where t1.RefundApprovalStatusId == status.StatusId
                                 select new
                                 {
                                     t1.Id,
                                     t1.DebitId,
                                     t1.TransReference,
                                     t1.TransDescription,
                                     t1.DateofFailure,
                                     t1.ProofImagePath,
                                     t1.RefundAmount,
                                     t1.DebitRefundStatusId,
                                     t1.RefundApprovalStatusId,
                                     t2.Mode,
                                     t3.CustomerId,
                                     t2.Description,
                                     //RefundStatus = status.Name,
                                     t3.WalletId,
                                     t3.AccountName,
                                 }).ToList();


            var debitReversalRecords = (from t1 in debitReversal
                                        join emp in db._dbContext.HRMS_EmployeeMst_STs on t1.CustomerId equals emp.CustomerId
                                        join comp in db._dbContext.SMAM_CompanyMst_STs on emp.CompanyId equals comp.CompanyId
                                        join t4 in db._dbContext.Status on t1.DebitRefundStatusId equals t4.StatusId
                                        join t5 in db._dbContext.Status on t1.RefundApprovalStatusId equals t5.StatusId
                                        select new
                                        {
                                            t1.Id,
                                            t1.DebitId,
                                            t1.TransReference,
                                            t1.TransDescription,
                                            t1.DateofFailure,
                                            t1.ProofImagePath,
                                            t1.RefundAmount,
                                            t1.DebitRefundStatusId,
                                            t1.RefundApprovalStatusId,
                                            t1.Mode,
                                            RefundStatus = t5.Name,
                                            t1.WalletId,
                                            t1.AccountName,
                                            t1.Description,
                                            RefundType = t4.Name,
                                            CompanyName = comp.Name,
                                        }).ToList();


            if (debitReversalRecords.Count == 0)
                return BadResponse("No Record Approval Debit Reversal");

            return SuccessResponse(debitReversalRecords);
        }

        public DTO UpdateRefundStatus(RefundTransactionModel model)
        {
            var cardDebitReversal = db._dbEngineContext.SRCC_CardDebit_Reversals.Where(o => o.Id == model.Id).FirstOrDefault();

            if (cardDebitReversal == null)
                return BadResponse("Record Not Found");


            cardDebitReversal.RefundApprovalStatusId = model.RefundApprovalStatusId;
            cardDebitReversal.ModifiedOn = DateTime.Now;
            cardDebitReversal.ModifiedBy = model.ModifiedBy;
            db._dbEngineContext.SubmitChanges();


            #region Add Refund Transaction Trancking
            FNGL_RefundTransactionTracking currentTrancking = new FNGL_RefundTransactionTracking();

            currentTrancking.DebitId = model.DebitId;
            currentTrancking.Comment = model.Comment;
            currentTrancking.RefundAmount = model.RefundAmount;
            currentTrancking.RefundApprovalStatusId = model.RefundApprovalStatusId;
            currentTrancking.Createdby = model.CreatedBy;
            currentTrancking.Createdon = DateTime.Now;

            db._dbEngineContext.FNGL_RefundTransactionTrackings.InsertOnSubmit(currentTrancking);
            db._dbEngineContext.SubmitChanges();
            #endregion

            return SuccessResponse("Update Successfully");
        }

        private DTO DocumentSave(HttpRequest httprequest)
        {

            //refund_reference_YYYYMMDDHHMMSS
            string basefilepath = ConfigurationManager.AppSettings["DocPath"].ToString();
            string FolderName = ConfigurationManager.AppSettings["FolderName"].ToString();

            if (httprequest.Files.Count == 0)
                return new DTO { isSuccessful = false, errors = "Document Required", data = null };

            else
            {
                string FileName = "";

                foreach (string file in httprequest.Files)
                {
                    var postedfile = httprequest.Files[file];
                    FileName = "Refund_reference_" + DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(postedfile.FileName);

                    var filePath = @"\" + FolderName;

                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }

                    filePath = filePath + @"\DebitRefund_Tracking";
                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }

                    filePath = filePath + @"\" + DateTime.Now.Year;
                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }
                    filePath = filePath + @"\" + DateTime.Now.Month;
                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }
                    filePath = filePath + @"\" + DateTime.Now.Day;
                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }

                    filePath = filePath + @"\" + FileName;
                    postedfile.SaveAs(basefilepath + filePath);

                }

                return new DTO { isSuccessful = true, errors = null, data = FileName };
            }


        }

    }
}