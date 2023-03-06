using CORE_API.Areas.General.Controllers;
using CORE_API.Services.FNGL;
using System;
using System.Web.Http;

namespace CORE_API.Areas.FNGL.Controller
{
    public class TransactionController : BaseController
    {
        // GET: FNGL/Transaction
        private readonly TransactionService _transactionService;

        public TransactionController()
        {
            _transactionService = new TransactionService();
        }

        [HttpGet]
        [ActionName("GetAllDebitCreditTransaction")]
        public IHttpActionResult GetAllDebitCreditTransaction(int? pageNo = null, int limit = 0, int? companyId = null)
        {
            try
            {
                var debitCreditTransaction = _transactionService.GetAllDebitCreditTransaction(pageNo, limit, companyId);
                return Ok(debitCreditTransaction);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }



        }
    }


}