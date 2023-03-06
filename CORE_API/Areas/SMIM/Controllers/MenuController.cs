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
    public class MenuController : BaseController
    {

        private readonly MenuService _menuSer;

        public MenuController()
        {
            _menuSer = new MenuService();
           
        }
        [HttpGet]
        [ActionName("GetMenus")]
        public IHttpActionResult GetMenus()
        {
            var data = _menuSer.GetMenus();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetMenusByRole")]
        public IHttpActionResult GetMenusByRole(int RoleId)
       {
            var data = _menuSer.GetMenusByRole(RoleId);
            return Ok(data);
        }
        [HttpGet]
        [ActionName("GetParentMenu")]
        public IHttpActionResult GetParentMenu()
        {
            var data = _menuSer.GetParentMenu();
            return Ok(data);
        }

        [ActionName("SubmitMenu")]
        [HttpPost]
        public IHttpActionResult SubmitMenu(SMIM_Menu_ST obj)
        {
            var data = _menuSer.SubmitMenu(obj);
            return Ok(data);

        }

        [HttpDelete]
        [ActionName("DeleteMenu")]
        public IHttpActionResult DeleteMenu(long Id)
        {
            var data = _menuSer.DeleteMenu(Id);
            return Ok(data);
        }
    }
}
