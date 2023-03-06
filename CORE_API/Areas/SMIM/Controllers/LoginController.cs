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
    public class LoginController : BaseController
    {

        private readonly LoginService _loginSer;
       // private readonly BaseService _baseSer;
        public LoginController()
        {
            _loginSer = new LoginService();
        //    _baseSer = new BaseService();
        }

        [HttpGet]
        [ActionName("GetUserVerification")]
        public IHttpActionResult GetUserVerification(string UserName,string Password)
        {
           var obj = _loginSer.GetUserVerification(UserName,Password);
            return Ok(obj);
        }

    }
}
