
using CORE_API.Areas.General.Controllers;
using CORE_API.Services.PCMS;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CORE_API.Areas.PCMS.Controllers
{
    public class ManualTransactionController : BaseController
    {
        private readonly ManualTransactionService _MTService;

        public ManualTransactionController()
        {
            _MTService = new ManualTransactionService();
        }

        // GET: PCMS/ManualTransaction
        [HttpGet]
        [ActionName("GetModelist")]

        public IHttpActionResult GetMode_()
        {
            var data = _MTService.GetModetype();
            return Ok(data);
        }

        [HttpPost]
        [ActionName("SubmitManualTransaction")]
        public IHttpActionResult SubmitManual_Transaction(MTP_Master obj)
        {
            var data = _MTService.SubmitManualTransaction(obj);
            return Ok(data);
        }


        [HttpGet]
        [ActionName("SP_GetMTPTrans")]
        public IHttpActionResult SP_Get_MTPTrans(int UserId)
        {
            var data = _MTService.SP_GetMTPTrans(UserId);
            return Ok(data);
        }


        [HttpGet]
        [ActionName("SP_GetMTPTransdtlbyId")]
        public IHttpActionResult GetMTPTransdtlbyId(int MTPMasterId)
        {
            var data = _MTService.SP_GetMTPTransdtlbyId(MTPMasterId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("MTP_UpdateStatus")]

        public IHttpActionResult MTPUpdateStatus(int MTPMasterId, int Status)
        {
            var data = _MTService.MTPUpdateStatus(MTPMasterId, Status);
            return Ok(data);

        }


        //


        //
    }
}