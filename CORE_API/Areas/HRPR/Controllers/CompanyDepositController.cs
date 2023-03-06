using CORE_API.Services.HRPR;
using CORE_MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CORE_API.Areas.General.Controllers;
using System.Web.Http;
using ContextMapper;
using CORE_API.Models.ComapnyWhiteListing;
using System.Threading.Tasks;

namespace CORE_API.Areas.HRPR.Controllers
{
    public class CompanyDepositController : BaseController
    {

        private readonly CompanyDepositService _depSer;
        private readonly AccountTransactionService _accountTranSer;

        public CompanyDepositController()
        {
            _depSer = new CompanyDepositService();
            _accountTranSer = new AccountTransactionService();


        }


        [HttpGet]
        [ActionName("GetCompanyDeposit")]
        public IHttpActionResult GetCompanyDeposit(int CompanyId)
        {
            try
            {
                var data = _depSer.GetCompanyDeposit(CompanyId);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadResponse(ex.Message);
            }


        }

        [ActionName("SubmitDeposit")]
        [HttpPost]
        public IHttpActionResult SubmitDeposit()
        {

            string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/");  // @"E:\SFTP";
            var httprequest = HttpContext.Current.Request;
            string FileName = "";
            int? CompanyId = Convert.ToInt32(httprequest["CompanyId"]);
            HRPR_DepositSlip_TR objdeposit = JsonConvert.DeserializeObject<HRPR_DepositSlip_TR>(httprequest["DepositMst"].ToString());
            var objcomp = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == CompanyId).FirstOrDefault();

            var postedfile = httprequest.Files["Image"];
            var filePath = "";// @"\" + objcomp != null ? objcomp.Name : "";
            if (postedfile != null)
            {
                filePath = @"\" + objcomp != null ? objcomp.Name : "";
                FileName = postedfile.FileName.ToString();
                if (filePath != null)
                {
                    if (!Directory.Exists(basefilepath + filePath))
                    {
                        Directory.CreateDirectory(basefilepath + filePath);
                    }
                }

                filePath = filePath + @"\Company Deposit";

                if (filePath != null)
                {
                    if (!Directory.Exists(basefilepath + filePath))
                    {
                        Directory.CreateDirectory(basefilepath + filePath);
                    }
                }

                filePath = filePath + @"\" + FileName;
                postedfile.SaveAs(basefilepath + filePath);
            }

            var obj = _depSer.SubmitDeposit(objdeposit, filePath);
            return Ok(obj);

        }


        [ActionName("AddCompanyDeposit")]
        [HttpPost]
        public IHttpActionResult AddCompanyDeposit()
        {
            try
            {
                var httprequest = HttpContext.Current.Request;
                var companyDocdetail = httprequest["DepositDetail"];

                if (companyDocdetail == null)
                    return Ok(new CORE_BASE.CORE.DTO { isSuccessful = false, errors = null, data = null });


                HRPR_DepositSlip_TR objcompanyDeposit = JsonConvert.DeserializeObject<HRPR_DepositSlip_TR>(httprequest["DepositDetail"].ToString());
                var response = _depSer.AddCompanyDeposit(objcompanyDeposit, httprequest);

                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = null });
            }
            catch (Exception Ex)
            {
                return this.BadResponse(Ex.Message);
            }
        }

        [HttpDelete]
        [ActionName("DeleteDeposit")]
        public IHttpActionResult DeleteDeposit(long Id)
        {
            var data = _depSer.DeleteDeposit(Id);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetAllDepositSlip")]
        public IHttpActionResult GetAllDepositSlip(DateTime Fromdate, DateTime Todate)
        {
            var data = _depSer.GetAllDepositSlip(Fromdate, Todate);
            return Ok(data);
        }


        [HttpGet]
        [ActionName("CommonStatus")]
        public IHttpActionResult GetCommonStatus()
        {
            try
            {
                var data = _depSer.GetCommonStatus();
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadResponse(ex.Message);

            }

        }

        [HttpPost]
        [ActionName("UpdateDepositSlipStatus")]
        public async Task<IHttpActionResult> UpdateDepositSlipStatusAsync(IList<CompanyDeposite> model)
        {

            try
            {
                var updateDepositSlipStatus = await _depSer.UpdateDepositSlipStatus(model);
                return SuccessResponse(updateDepositSlipStatus);
                     
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);

            }
        }

    }

}