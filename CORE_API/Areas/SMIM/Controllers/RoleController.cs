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
    public class RoleController : BaseController
    {

        private readonly RoleService _roleSer;
     
        public RoleController()
        {
            _roleSer = new RoleService();
           
        }
        [HttpGet]
        [ActionName("GetRole")]
        public IHttpActionResult GetRole()
        {
            var data = _roleSer.GetRole();
            return Ok(data);
        }

      
        [ActionName("SubmitRole")]
        [HttpPost]
        public IHttpActionResult SubmitRole(SMIM_Roles_ST obj)
        {
            var data = _roleSer.SubmitRole(obj);
            return Ok(data);

        }

        [HttpDelete]
        [ActionName("DeleteRole")]
        public IHttpActionResult DeleteRole(long Id)
        {
            var data = _roleSer.DeleteRoleData(Id);
            return Ok(data);
        }
    }
}
