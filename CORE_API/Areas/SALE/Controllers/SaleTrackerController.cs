using CORE_API.Areas.General.Controllers;
using CORE_API.Services.PCMS;
using CORE_API.Services.SALE;
using CORE_MODELS;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Http;

namespace CORE_API.Areas.SALE.Controllers
{
    public class SaleTrackerController : BaseController
    {
        private readonly SaleTrackerService _delSer;
        public SaleTrackerController()
        {
            _delSer = new SaleTrackerService();
        }


        [HttpGet]
        [ActionName("AllDealStage")]
        public IHttpActionResult GetDealStage()
        {
            var data = _delSer.GetAllDealStage();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("Saledetails")]
        public IHttpActionResult SaleTracker(int UserId)
        {
            var data = _delSer.GetbyuserId(UserId);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("SubmitSale")]
        public IHttpActionResult SubmitSalestracker(SALES_DealComment DTO)
        {
            try
            {
                var httprequest = HttpContext.Current.Request;
                var result = _delSer.SubmitSalestracker(DTO);
                return Ok(result);
            }
            catch (Exception Ex)
            {
                return this.BadResponse(Ex.Message);
            }

        }

        [HttpPost]
        [ActionName("UpdateSaletracker")]
        public IHttpActionResult UpdateSaletracker(SALES_DealComment DTO)
        {
            try
            {
                var httprequest = HttpContext.Current.Request;

               var rtn = _delSer.UpdateSaletracker(DTO);
                return Ok(rtn);
            }
            catch (Exception Ex)
            {
                return this.BadResponse(Ex.Message);
            }

        }

    }
}