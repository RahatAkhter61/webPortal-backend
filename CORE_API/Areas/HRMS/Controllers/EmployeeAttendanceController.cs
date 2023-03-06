using CORE_API.Areas.General.Controllers;
using CORE_API.Areas.SMAM.Models;
using CORE_API.General;
using CORE_API.Services.HRMS;
using CORE_API.Services.SMAM;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Areas.HRMS.Controllers
{
    public class EmployeeAttendanceController : BaseController
    {

        private readonly EmployeeAttendanceService _empattendanceSer;

        public EmployeeAttendanceController()
        {
            _empattendanceSer = new EmployeeAttendanceService();
        }

        [HttpGet]
        [ActionName("GetEmployeeAttendance")]
        public IHttpActionResult GetEmployeeAttendance()
        {
            var data = _empattendanceSer.GetEmployeeAttendance();
            return Ok(data);
        }
    

        [HttpPost]
        [ActionName("UploadAttendance")]
        public async Task<IHttpActionResult> UploadAttendance()
        {
            //  //await this.Runasync(() => { });

            string ImageName = null;
            var httprequest = HttpContext.Current.Request;
            int CreadtedBy = Convert.ToInt32(httprequest["CreatedBy"]);

            var stream = Request.Content.ReadAsStreamAsync();
            var excel = new ExcelPackage(stream.Result);
            DataTable dt = Common.ExcelPackageToDataTable(excel);

            List<string> lst = _empattendanceSer.SaveEmployeeAttendance(dt, CreadtedBy);
            if (lst.Count > 0)
                return this.BadResponse(lst);
            else
                return this.SuccessResponse("Uploaded Succesfully");

        }

        [HttpDelete]
        [ActionName("DeleteAttendance")]
        public IHttpActionResult DeleteAttendance(long Id)
        {
            var data = _empattendanceSer.DeleteAttendance(Id);
            return Ok(data);
        }

        [ActionName("UpdateEmployeeAttendance")]
        [HttpPost]
        public IHttpActionResult UpdateEmployeeAttendance(IList<HRMS_EmpAttendance_TR> obj)
        {
            var data = _empattendanceSer.UpdateEmployeeAttendance(obj);
            return Ok(data);

        }

        //[ActionName("SubmitEmployee")]
        //[HttpPost]
        //public IHttpActionResult SubmitEmployee()
        //{
        //    try
        //    {
        //        var httprequest = HttpContext.Current.Request;
        //        HRMS_EmployeeMst_ST objemp = JsonConvert.DeserializeObject<HRMS_EmployeeMst_ST>(httprequest["EmployeeMst"].ToString());
        //        IList<HRMS_EmpDocs_ST> objempdoc = JsonConvert.DeserializeObject<IList<HRMS_EmpDocs_ST>>(httprequest["EmployeeDoc"].ToString());
        //        IList<HRMS_EmpCharges_ST> objempcharges = JsonConvert.DeserializeObject<IList<HRMS_EmpCharges_ST>>(httprequest["EmployeeCharges"].ToString());

        //       _empSer.SubmitEmployee(objemp);
        //        _docSer.SubmitDocuments(objempdoc, objemp.EmpId);
        //        _chargesSer.SubmitCharges(objempcharges, objemp.EmpId);



        //        return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = null });
        //    }
        //    catch (Exception Ex)
        //    {

        //        return this.SuccessResponse(Ex.Message);
        //    }


        //    return Ok("");

        //}

        //[ActionName("SubmitEmployeeDocuments")]
        //[HttpPost]
        //public IHttpActionResult SubmitEmployeeDocuments()
        //{

        //    string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/");  // @"E:\SFTP";
        //    var httprequest = HttpContext.Current.Request;
        //    object data = null;
        //    if (httprequest.Files.Count > 0)
        //    {
        //        string FileName = "";
        //        int count = 0;

        //        int? EmployeeId = Convert.ToInt32(httprequest["EmployeeId"]);
        //        string desc = httprequest["Desc"].ToString();
        //        string empname = httprequest["Name"].ToString();

        //        string[] descarr = desc.Split(',');
        //        foreach (string file in httprequest.Files)
        //        {
        //            var postedfile = httprequest.Files[file];


        //            FileName = postedfile.FileName.ToString();


        //            var filePath = @"\" + empname;
                  

        //            if (filePath != null)
        //            {
        //                if (!Directory.Exists(basefilepath + filePath))
        //                {
        //                    Directory.CreateDirectory(basefilepath + filePath);
        //                }
        //            }

        //            filePath = filePath + @"\Employee Documents";
        //            //  // HttpContext.Current.Server.MapPath("~/Company/" + FileName);

        //            if (filePath != null)
        //            {
        //                if (!Directory.Exists(basefilepath + filePath))
        //                {
        //                    Directory.CreateDirectory(basefilepath + filePath);
        //                }
        //            }

        //            filePath = filePath + @"\" + FileName;
        //            postedfile.SaveAs(basefilepath + filePath);
        //            data = _docSer.UpdateDocuments(descarr[count], filePath, FileName, EmployeeId);
        //            count++;

                
        //        }
        //    }
        //    return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = null });

        //}



    }
}


//public class Attachments
//{
//    public object file { get; set; }
//    public int CompanyId { get; set; }
//}