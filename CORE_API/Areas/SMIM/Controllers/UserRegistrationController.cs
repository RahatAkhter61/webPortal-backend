using CORE_API.General;
using CORE_API.Services.SMIM;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CORE_API.Areas.General.Controllers;
using System.Web.Http;

namespace CORE_API.Areas.SMIM.Controllers
{
    public class UserRegistrationController : ApiController
    {

         UserRegistrationService _UserRegSer;

        public UserRegistrationController()
        {
            _UserRegSer = new UserRegistrationService();
           
        }
        [HttpGet]
        [ActionName("GetUsers")]
        public IHttpActionResult GetUsers()
        {
            var data = _UserRegSer.GetUsers();
            return Ok(data);
        }


        [ActionName("SubmitUsers")]
        [HttpPost]
        public IHttpActionResult SubmitUsers(SMIM_UserMst_ST obj)
        {
            var data = _UserRegSer.SubmitUsers(obj);
            return Ok(data);

        }

        [HttpDelete]
        [ActionName("DeleteUser")]
        public IHttpActionResult DeleteUser(long Id)
        {
            var data = _UserRegSer.DeleteUser(Id);
            return Ok(data);
        }
    }
}
