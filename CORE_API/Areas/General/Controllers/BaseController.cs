using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CORE_BASE.CORE;

namespace CORE_API.Areas.General.Controllers
{
    public class BaseController : ApiController
    {
        public BaseController()
        {

        }
        protected IHttpActionResult SuccessResponse(object data)
        {
            return Ok(new { isSuccessful = true, data = data });
        }

        protected IHttpActionResult BadResponse(object error)
        {
            return Ok(new{isSuccessful = false, errors = error });
        }
    }
}
