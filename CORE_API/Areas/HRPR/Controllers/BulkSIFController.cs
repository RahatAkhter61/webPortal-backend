using ContextMapper;
using CORE_API.Areas.HRMS.Models;
using CORE_API.Areas.SMAM.Models;
using CORE_API.General;
using CORE_API.Services.HRMS;
using CORE_API.Services.HRPR;
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
using CORE_API.Areas.General.Controllers;
using System.Web.Http;
using System.Globalization;

namespace CORE_API.Areas.HRPR.Controllers
{
    public class BulkSIFController : BaseController
    {

        private readonly BulkSIFMasterService _bulkMastereSer;
        private readonly BulkSIFDetailService _bulkDetailSer;

        public BulkSIFController()
        {
            _bulkMastereSer = new BulkSIFMasterService();
            _bulkDetailSer = new BulkSIFDetailService();
        }

        [HttpPost]
        [ActionName("GetBulkData")]
        public IHttpActionResult GetBulkData(UnprocesessedParameters objunprocess)
        {

            var data = _bulkDetailSer.GetBulkData(objunprocess);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("UploadBulkSIF")]
        public async Task<IHttpActionResult> UploadBulkSIF()
        {

            try
            {
                var httprequest = HttpContext.Current.Request;
                int ImportedBy = Convert.ToInt32(httprequest["ImportedBy"]);
                int CompanyId = Convert.ToInt32(httprequest["CompanyId"]);
                string IsCards = httprequest["IsCards"].ToString();

                string EstId = httprequest["EstId"].ToString();
                string TotalBalance = httprequest["TotalBalance"].ToString();
                string SifDate = DateTime.Now.ToString(httprequest["SifDate"]).ToString();
                string FileName = httprequest.Files[0].FileName;

                string Month = httprequest["Month"].ToString();
                string Year = httprequest["Year"].ToString();
                string UserId = httprequest["UserId"].ToString();

                //if ((IsCards == "true" && FileName == "KPcards.xlsx") || (IsCards == "false" && FileName == "Othercards.xlsx"))
                //{
                DateTime SalaryDate = Convert.ToDateTime(httprequest["SifDate"]).Date;
                var stream = Request.Content.ReadAsStreamAsync();
                var excel = new ExcelPackage(stream.Result);
                DataTable dt = Common.ExcelPackageToDataTable(excel);

                int TotalEmployee = dt.Rows.Count;

                // var obj = new BLInfo<HRPR_BulkSIFFiles_HI>().GetQuerable<HRPR_BulkSIFFiles_HI>().Where(a => a.FileName == FileName).FirstOrDefault();

                //if (obj == null)
                //{
                int TSifId = _bulkMastereSer.SubmitBulkSIFFiles(FileName, TotalEmployee, CompanyId, Month, Year, UserId);


                List<string> lst = _bulkDetailSer.SaveBulkSIFDetails(dt, ImportedBy, IsCards, CompanyId, SalaryDate, TSifId, TotalBalance, EstId);
                if (lst.Count > 0)
                {
                    return this.BadResponse(lst);
                }
                return this.SuccessResponse(TSifId);
                //}
                //else
                //    return this.BadResponse("File is already uploaded...!");
                //}
                //else
                //    return this.BadResponse("Please upload valid file...!");
            }
            catch (Exception Ex)
            {

                return this.BadResponse(Ex.Message.ToString());
            }
        }



    }
}

