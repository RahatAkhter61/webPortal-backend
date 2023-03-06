using CORE_API.Areas.General.Controllers;
using CORE_API.Models.Transaction;
using CORE_API.Services.HRPR;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Http;

namespace CORE_API.Areas.HRPR.Controllers
{
    public class RefundTransactionController : BaseController
    {
        // GET: HRPR/RefundTransaction
        private readonly RefundTransactionService _refundService;
        public RefundTransactionController()
        {
            _refundService = new RefundTransactionService();
        }

        [HttpGet]
        [ActionName("GetRefundStatus")]
        public IHttpActionResult GetRefundStatus()
        {
            try
            {
                var statusTypeList = _refundService.GetRefundStatus();
                return Ok(statusTypeList);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet]
        [ActionName("GetDebitTransactions")]
        public IHttpActionResult GetDebitTransactions(DateTime? FromDate, DateTime? Todate, DateTime? TransactionDate, string ReferenceNo, string WalletId, string MobileNo)
        {
            try
            {
                var debitTransactionlist = _refundService.GetDebitTransactions(FromDate, Todate, TransactionDate,ReferenceNo,WalletId,MobileNo);
                return Ok(debitTransactionlist);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }

        }

        [HttpPost]
        [ActionName("PostRefundTransaction")]
        public IHttpActionResult PostRefundTransaction()
        {
            try
            {
                var httprequest = HttpContext.Current.Request;
                var requestBody = httprequest["RefundTransactionRequest"];

                RefundTransactionModel model = JsonConvert.DeserializeObject<RefundTransactionModel>(requestBody.ToString());

                var statusTypeList = _refundService.PostRefundTransaction(model, httprequest);
                return Ok(statusTypeList);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet]
        [ActionName("GetDebitReversal")]
        public IHttpActionResult GetDebitReversal()
        {
            try
            {
                var debitReversal = _refundService.GetDebitReversal();
                return Ok(debitReversal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ActionName("UpdateRefundStatus")]
        public IHttpActionResult UpdateRefundStatus(RefundTransactionModel model)
        {
            try
            {
                var updateRefundStatus = _refundService.UpdateRefundStatus(model);
                return Ok(updateRefundStatus);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet]
        [ActionName("RefundTransactionTracking")]
        public IHttpActionResult RefundTransactionTracking(int? debitId)
        {
            try
            {
                var refundTrackingRecord = _refundService.RefundTransactionTracking(debitId);
                return Ok(refundTrackingRecord);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

    }
}