
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
    public class CardOperationController : BaseController
    {
        private readonly CardOperationService _CardSer;

        public CardOperationController()
        {
            _CardSer = new CardOperationService();
        }

        [HttpGet]
        [ActionName("ALlCardOperation")]
        public IHttpActionResult SPGetCardOperaion(int? UserId)
        {
            var result = this._CardSer.SPGetCardOperaion(UserId);
            return this.Ok(result);
        }

        [HttpPost]
        [ActionName("SubmitCardOperation")]
        public IHttpActionResult Submit_CardOperation(CardOperationMaster obj)
        {
            var data = _CardSer.SubmitCardOperation(obj);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("UpdateCardOperationStatus")]
        public IHttpActionResult Update_CardOperation(int COMasterId, int StatusId, int EmpId)
        {
            var data = _CardSer.UpdateCardOperation(COMasterId, StatusId, EmpId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetSPCardOPdtlbyId")]
        public IHttpActionResult Get_CardOPdtlbyId(int COMasterId)
        {
            var data = _CardSer.GetCardOPdtlbyId(COMasterId);
            return Ok(data);
        }


    }
}