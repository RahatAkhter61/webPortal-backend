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
    public class AssignRolesController : BaseController
    {

        private readonly AssignRolesService _assignrolesSer;

        public AssignRolesController()
        {
            _assignrolesSer = new AssignRolesService();
            
           
        }
        [HttpGet]
        [ActionName("GetRightsbyRoles")]
        public IHttpActionResult GetRightsbyRoles(long RoleId)
        {
            var data = _assignrolesSer.GetRightsbyRoles(RoleId);
            return Ok(data);
        }


        [ActionName("SubmitAssignRights")]
        [HttpPost]
        public IHttpActionResult SubmitAssignRights(List<SMIM_AssignRoles_ST> obj)
        {
            var data = _assignrolesSer.SubmitAssignRights(obj);
            return Ok(data);

        }

        //[HttpDelete]
        //[ActionName("DeleteRights")]
        //public IHttpActionResult DeleteRights(long Id)
        //{
        //    var data = _rightsSer.DeleteRights(Id);
        //    return Ok(data);
        //}
    }
}
