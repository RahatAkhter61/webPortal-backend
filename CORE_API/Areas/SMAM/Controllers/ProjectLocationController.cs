using CORE_API.Areas.SMAM.Models;
using CORE_API.General;
using CORE_API.Services.HRMS;
using CORE_API.Services.SMAM;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using CORE_API.Areas.General.Controllers;
using System.Web.Http;

namespace CORE_API.Areas.SMAM.Controllers
{
    public class ProjectLocationController : BaseController
    {

        private readonly ProjectLocationService _projSer;
        private readonly CampService _campSer;

        public ProjectLocationController()
        {
            _projSer = new ProjectLocationService();
            _campSer = new CampService();

        }


        [HttpGet]
        [ActionName("GetCamp")]
        public IHttpActionResult GetCamp(int? CompanyId)
        {

            var data = _projSer.GetCamp(CompanyId);
            return Ok(data);
        }
        [HttpGet]
        [ActionName("GetCampById")]
        public IHttpActionResult GetCampById(int CampId)
        {

            var data = _projSer.GetCampById(CampId);
            return Ok(data);
        }
        [HttpGet]
        [ActionName("GetEmployeesbycamp")]
        public IHttpActionResult GetEmployeesbycamp(int CampId)
        {

            var data = _projSer.GetEmployeesbycamp(CampId);
            return Ok(data);
        }
        [ActionName("SubmitProjectLocation")]
        [HttpPost]
        public IHttpActionResult SubmitProjectLocation()
        {
            try
            {

                var httprequest = HttpContext.Current.Request;

                SMAM_Camp_ST objcamp = JsonConvert.DeserializeObject<SMAM_Camp_ST>(httprequest["CampMst"].ToString());
                Effective obj = JsonConvert.DeserializeObject<Effective>(httprequest["Effective"].ToString());
                //DateTime EffFrom = Convert.ToDateTime(httprequest["EffFrom"].ToString());
                //DateTime EffTo = Convert.ToDateTime(httprequest["EffTo"].ToString());
                IList<SMAM_CampEmp_ST> objempcamp = JsonConvert.DeserializeObject<IList<SMAM_CampEmp_ST>>(httprequest["EmpCampDetail"].ToString());
                var data = _campSer.SubmitCamp(objcamp, objempcamp, obj.EffFrom, obj.EffTo);
                return Ok(data);


            }
            catch (Exception Ex)
            {

                return this.BadResponse(Ex.Message);
            }


            return Ok("");
        }

    }

}
public class Effective
{
    public DateTime EffFrom { get; set; }
    public DateTime EffTo { get; set; }
}