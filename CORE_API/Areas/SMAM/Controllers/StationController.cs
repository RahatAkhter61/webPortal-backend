using CORE_API.General;
using CORE_API.Services.SMAM;
using CORE_API.Services.SMIM;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using CORE_API.Areas.General.Controllers;
using System.Web.Http;

namespace CORE_API.Areas.SMAM.Controllers
{
    public class StationController : BaseController
    {

        private readonly StationService _stationSer;
        private readonly DepartmentService _depSer;
        public StationController()
        {
            _stationSer = new StationService();
            _depSer = new DepartmentService();
           
        }
        [HttpGet]
        [ActionName("GetStations")]
        public IHttpActionResult GetStations()
        {
            var data = _stationSer.GetStations();
            return Ok(data);
        }

        [ActionName("SubmitStation")]
        [HttpPost]
        public IHttpActionResult SubmitStation()
        {
            try
            {
                var httprequest = HttpContext.Current.Request;
                SMAM_Stations_ST objstation = JsonConvert.DeserializeObject<SMAM_Stations_ST>(httprequest["Station"].ToString());
                IList<SMAM_Departments_ST> objdepts = JsonConvert.DeserializeObject<IList<SMAM_Departments_ST>>(httprequest["Departments"].ToString());


                int statId =_stationSer.SubmitStation(objstation);
                int? createdby = objstation.CreatedBy;
                _depSer.SubmitDepartment(objdepts, createdby, statId);
                



                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = null });
            }
            catch (Exception Ex)
            {

                return this.SuccessResponse(Ex.Message);
            }


            return Ok("");

        }

        //[HttpDelete]
        //[ActionName("DeleteRole")]
        //public IHttpActionResult DeleteRole(long Id)
        //{
        //    var data = _roleSer.DeleteRoleData(Id);
        //    return Ok(data);
        //}
    }
}
