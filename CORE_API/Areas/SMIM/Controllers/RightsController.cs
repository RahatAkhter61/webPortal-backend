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
    public class RightsController : BaseController
    {

        private readonly RightsService _rightsSer;

        public RightsController()
        {
            _rightsSer = new RightsService();
           
        }

      //  public 
        //void GenerateClassInstance<T>(T obj) where T : class,new()
        //{
        //    T current = null;
        //    current = Activator.CreateInstance<T>();
        //}
        [HttpGet]
        [ActionName("GetRights")]
        public IHttpActionResult GetRights()
        {

            var data = _rightsSer.GetRights();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetRightsByForm")]
        public IHttpActionResult GetRightsByForm(string FormName)
        {
            var data = _rightsSer.GetRightsByForm(FormName);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetForms")]
        public IHttpActionResult GetForms()
        {
            var data = _rightsSer.GetForms();
            return Ok(data);
        }


        [ActionName("SubmitRights")]
        [HttpPost]
        public IHttpActionResult SubmitRights(SMIM_Rights_ST obj)
        {
            var data = _rightsSer.SubmitRights(obj);
            return Ok(data);

        }

        [HttpDelete]
        [ActionName("DeleteRights")]
        public IHttpActionResult DeleteRights(long Id)
        {
            var data = _rightsSer.DeleteRights(Id);
            return Ok(data);
        }
    }
}
