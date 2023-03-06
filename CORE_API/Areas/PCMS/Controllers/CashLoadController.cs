using CORE_API.Areas.General.Controllers;
using CORE_API.Areas.PCMS.Model;
using CORE_API.Areas.SMAM.Models;
using CORE_API.Models;
using CORE_API.Services.PCMS;
using CORE_MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Areas.PCMS.Controllers
{
    public class CashLoadController : BaseController
    {
        private readonly CashLoadService _cashser;
        public CashLoadController()
        {
            _cashser = new CashLoadService();
        }

        [HttpGet]
        [ActionName("GetCashLoad")]
        public IHttpActionResult GetCashLoad(int Companyid, int Productid)
        {
            var data = _cashser.GetCashloadinfo(Companyid, Productid);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("SubmitCashload")]
        public IHttpActionResult SubmitCashload()
        {
            var httprequest = HttpContext.Current.Request;
            PCMS_Cashload_Master objCashLoadMaster = JsonConvert.DeserializeObject<PCMS_Cashload_Master>(httprequest["CashLoadMaster"].ToString());
            CalculateMultipleChargeBindingModel chargemModel = JsonConvert.DeserializeObject<CalculateMultipleChargeBindingModel>(httprequest["ChargeModel"].ToString());
            var data = _cashser.SubmitCashload(objCashLoadMaster, chargemModel);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("SubmitExcelDoc")]
        public IHttpActionResult SubmitExcelDoc()
        {
            string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/");
            var httprequest = HttpContext.Current.Request;
            if (httprequest.Files.Count > 0)
            {
                string FileName = "";
                int count = 0;
                foreach (string file in httprequest.Files)
                {
                    var postedfile = httprequest.Files[file];
                    FileName = "Cashload_" + postedfile.FileName + Path.GetExtension(postedfile.FileName);

                    var filePath = @"\" + "KP";
                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }

                    filePath = filePath + @"\Cashload Documents";
                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }

                    filePath = filePath + @"\" + "Cashload_" + postedfile.FileName;
                    postedfile.SaveAs(basefilepath + filePath);

                    return Ok(filePath);
                }
            }
            return Ok("File Not Found");
        }

        [HttpGet]
        [ActionName("GetCashload")]
        public IHttpActionResult GetCashload(int UserId, DateTime Fromdate, DateTime Todate)
        {
            var data = _cashser.GetCashload(UserId, Fromdate, Todate);
            return Ok(data);
        }


        [HttpGet]
        [ActionName("GetCashloaddetailbyId")]
        public IHttpActionResult GetCashloaddetailbyId(int CashloadId)
        {
            var data = _cashser.GetCashloaddetailbyId(CashloadId);
            return Ok(data);
        }



        [HttpGet]
        [ActionName("updateCasload")]
        public async Task<IHttpActionResult> updateCasloadAsync(int CashloadId, int status,int CreatedBy)
        {
            var data = await _cashser.updateCasloadAsync(CashloadId, status, CreatedBy);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetCashloadStatus")]
        public IHttpActionResult GetCashloadStatus()
        {
            var data = _cashser.GetCashloadStatus();
            return Ok(data);
        }

    }
}