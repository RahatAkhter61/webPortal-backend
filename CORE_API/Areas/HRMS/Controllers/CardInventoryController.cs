using CORE_API.Areas.General.Controllers;
using CORE_API.Areas.HRPR.Models;
using CORE_API.General;
using CORE_API.Services.HRMS;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace CORE_API.Areas.HRMS.Controllers
{
    public class CardInventoryController : BaseController
    {
        // GET: HRMS/CardInventory
        private readonly USPExportService _uspExportSer;
        private readonly DeliveryService _delSer;


        public CardInventoryController()
        {
            _delSer = new DeliveryService();
            _uspExportSer = new USPExportService();
        }

        [HttpGet]
        [ActionName("Get_CardSentOnPrinted")]
        public IHttpActionResult GetCardSentOnPrinted(int? UserId, DateTime Date)
        {
            var data = _uspExportSer.Get_SPCardSentOnPrinted(UserId, Date);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("UpdatePrintedStatus")]
        public IHttpActionResult Update_PrintedStatus(IList<UpdatePrintedStatus> Obj)
        {

            var data = _uspExportSer.UpdatePrintedStatus(Obj);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("sp_GetCollectedCardsForAssign")]
        public IHttpActionResult GetCollectedCardsForAssign(int? UserId, int CompanyId, int CampId, string WalletId)
        {
            var data = _uspExportSer.CollectedCardsForAssign(UserId, CompanyId, CampId, WalletId);
            return Ok(data);
        }


        [HttpPost]
        [ActionName("AssignToExecutive")]
        public IHttpActionResult AssignToExecutive(IList<AssignToExecutive> obj)
        {

            try
            {
                var result = _uspExportSer.AssignToExecutive(obj);

                if (result.isSuccessful && result.data != "Update Successfully")
                {
                    var rtn = _delSer.updateActivateStatus(result.data);
                    return Ok(rtn);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            

        }

    }
}