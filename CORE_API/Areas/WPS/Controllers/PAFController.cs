using CORE_API.Areas.General.Controllers;
using CORE_API.Models.WPS;
using CORE_API.Services.WPS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CORE_API.Areas.WPS.Controllers
{
    public class PAFController : BaseController
    {

        public readonly PAFService _PAFService;
        public readonly PRCService _PRCService;
        public readonly RTCService _RTCService;

        public PAFController()
        {
            _PAFService = new PAFService();
            _PRCService = new PRCService();
            _RTCService = new RTCService();
        }

        [HttpGet]
        [ActionName("GetAllPAF")]
        public IHttpActionResult GetAllPAF(DateTime? PAFUploadedOn)
        {
            var obj = _PAFService.GetAllPAF(PAFUploadedOn);
            return Ok(obj);
        }

        [HttpGet]
        [ActionName("GetPAFDetailbyId")]
        public IHttpActionResult GetPAFDetailbyId(int PAFId)
        {
            var obj = _PAFService.GetPAFDetailbyId(PAFId);
            return Ok(obj);
        }

        [HttpPost]
        [ActionName("WPSUpdateStatusTopUpRequest")]
        public IHttpActionResult PostUpdateStatusTopUpRequest(List<WPSupdatetopupstatus> model)
        {
            var obj = _PAFService.UpdateStatusTopUpRequest(model);
            return Ok(obj);
        }

        [HttpGet]
        [ActionName("GetPRCDetailbyId")]
        public IHttpActionResult GetPRCDetailbyId(int PAFId)
        {
            var obj = _PRCService.GetPRCDetailbyId(PAFId);
            return Ok(obj);
        }


        [HttpGet]
        [ActionName("GetAllRTC")]
        public IHttpActionResult GetAllRTC(DateTime? Received_On)
        {
            var result = _RTCService.GetAllRTC(Received_On);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("procced_Claimed")]
        public IHttpActionResult ProccedClaimed(proccedClaimedModel model)
        {
            var result = _RTCService.ProccedClaimed(model);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("GetRTCdetailbyId")]
        public IHttpActionResult GetRTCdetailbyId(int Id)
        {
            var result = _RTCService.GetRTCdetailbyId(Id);
            return Ok(result);
        }

    }
}