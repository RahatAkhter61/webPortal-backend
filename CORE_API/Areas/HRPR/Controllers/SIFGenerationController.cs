using CORE_API.Areas.SMAM.Models;
using CORE_API.General;
using CORE_API.Services.HRMS;
using CORE_API.Services.HRPR;
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
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using ContextMapper;
using CORE_API.Areas.HRMS.Models;
using CORE_API.Areas.HRPR.Models;
using CORE_API.Models;

namespace CORE_API.Areas.HRPR.Controllers
{
    public class SIFGenerationController : BaseController
    {

        private readonly SIFMasterService _sifMasterSer;
        private readonly SIFDetailService _sifDetailSer;
        private readonly ExportSIFfileMasterService _sifexpMasterSer;
        private readonly ExportSIFfileDetailService _sifexpDetailSer;
        private readonly BulkSIFDetailService _bulksiFDetailSer;
        private readonly AccountTransactionService _accountTransactionService;
        public SIFGenerationController()
        {
            _sifMasterSer = new SIFMasterService();
            _sifDetailSer = new SIFDetailService();
            _sifexpMasterSer = new ExportSIFfileMasterService();
            _sifexpDetailSer = new ExportSIFfileDetailService();
            _bulksiFDetailSer = new BulkSIFDetailService();
            _accountTransactionService = new AccountTransactionService();
        }

        [HttpGet]
        [ActionName("GetUnProcessedEmployees")]
        public IHttpActionResult GetUnProcessedEmployees(int CompanyId, int DeptId, int StationId, int EmpId, DateTime dt)
        {

            var data = _sifDetailSer.GetUnProcessedEmployees(CompanyId, DeptId, StationId, EmpId, dt);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetUnProcessedEmployeesDatass")]
        public IHttpActionResult GetUnProcessedEmployeesDatass(int? CompanyId, int? DeptId, int? StationId, int? EmpId, DateTime dt)
        {

            var data = _sifDetailSer.GetUnProcessedEmployeesData(CompanyId, DeptId, StationId, EmpId, dt);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetTotalBalance")]
        public IHttpActionResult GetTotalBalance(int CompanyId)
        {

            var data = _sifDetailSer.GetTotalBalance(CompanyId);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("GetUnProcessedEmployeesData")]
        public IHttpActionResult GetUnProcessedEmployeesData()
        {
            var httprequest = HttpContext.Current.Request;
            UnprocesessedParameters obj = JsonConvert.DeserializeObject<UnprocesessedParameters>(httprequest["Proc"].ToString());
            var data = _sifDetailSer.GetUnProcessedEmployeesData(obj);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("GetProcessedEmployeesData")]
        public IHttpActionResult GetProcessedEmployeesData()
        {

            var httprequest = HttpContext.Current.Request;
            UnprocesessedParameters obj = JsonConvert.DeserializeObject<UnprocesessedParameters>(httprequest["Proc"].ToString());
            var data = _sifDetailSer.GetProcessedEmployeesData(obj);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("GetVerifiedEmployeesData")]
        public IHttpActionResult GetVerifiedEmployeesData()
        {
            var httprequest = HttpContext.Current.Request;
            UnprocesessedParameters obj = JsonConvert.DeserializeObject<UnprocesessedParameters>(httprequest["Proc"].ToString());
            var data = _sifDetailSer.GetVerifiedEmployeesData(obj);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("GetLastVerifiedEmployeesData")]
        public IHttpActionResult GetLastVerifiedEmployeesData()
        {
            var httprequest = HttpContext.Current.Request;
            UnprocesessedParameters obj = JsonConvert.DeserializeObject<UnprocesessedParameters>(httprequest["Proc"].ToString());
            var data = _sifDetailSer.GetLastVerifiedEmployeesData(obj);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("GetGenerateSIFData")]
        public IHttpActionResult GetGenerateSIFData()
        {
            var httprequest = HttpContext.Current.Request;
            UnprocesessedParameters obj = JsonConvert.DeserializeObject<UnprocesessedParameters>(httprequest["Proc"].ToString());
            var data = _sifDetailSer.GetGenerateSIFData(obj);
            return Ok(data);
        }

        [ActionName("CalculateCharges")]
        [HttpPost]
        public IHttpActionResult CalculateCharges(CalculateChargeBindingModel model)
        {
            try
            {
                var data = _accountTransactionService.CalculateCharges(model);
                return Ok(data);
            }
            catch (Exception Ex)
            {

                return this.BadResponse(Ex.Message);
            }


            return Ok("");
        }

        [ActionName("SubmitSIF")]
        [HttpPost]
        public IHttpActionResult SubmitSIF()
        {
            try
            {

                var httprequest = HttpContext.Current.Request;

                HRPR_SifMaster_TR objsifmaster = JsonConvert.DeserializeObject<HRPR_SifMaster_TR>(httprequest["SIFMst"].ToString());
                IList<HRPR_SifDetail_TR> objsifDetail = JsonConvert.DeserializeObject<IList<HRPR_SifDetail_TR>>(httprequest["SIFDetail"].ToString());
                var data = _sifMasterSer.SubmitSIFMaster(objsifmaster, objsifDetail);
                return Ok(data);


            }
            catch (Exception Ex)
            {

                return this.BadResponse(Ex.Message);
            }


            return Ok("");
        }

        [ActionName("ExportSIF")]
        [HttpPost]
        public IHttpActionResult ExportSIF(IList<ExportSIF> obj)
        {
            try
            {

                var data = _sifDetailSer.ExportSIF(obj);
                return Ok(data);


            }
            catch (Exception Ex)
            {

                return this.BadResponse(Ex.Message);
            }


            return Ok("");
        }

        [ActionName("UpdateSIFDetail")]
        [HttpPost]
        public IHttpActionResult UpdateSIFDetail(IList<HRPR_SifDetail_TR> obj)
        {
            try
            {
                var data = _sifDetailSer.UpdateSIFDetail(obj);
                return Ok(data);



            }
            catch (Exception Ex)
            {

                return this.BadResponse(Ex.Message);
            }


            return Ok("");
        }

        [ActionName("ProcessRollBack")]
        [HttpPost]
        public IHttpActionResult ProcessRollBack(IList<HRPR_SifDetail_TR> obj)
        {
            try
            {
                var data = _sifDetailSer.ProcessRollBack(obj);
                return Ok(data);



            }
            catch (Exception Ex)
            {

                return this.BadResponse(Ex.Message);
            }


            return Ok("");
        }


        [ActionName("GenerateSIF")]
        [HttpPost]
        public IHttpActionResult GenerateSIF()
        {
            try
            {
                var httprequest = HttpContext.Current.Request;
                string IsCards = "";
                string Isdirect = httprequest["Isdirect"].ToString();
                string Establistmentdid = httprequest["Establistmentdid"].ToString();
                int UserId = Convert.ToInt16(httprequest["UserId"].ToString());

                string Month = httprequest["Month"].ToString();
                string Year = httprequest["Year"].ToString();

                if (Isdirect == "Yes")
                {
                    IsCards = httprequest["IsCards"].ToString();
                }

                HRPR_SifMaster_TR objsifmaster = JsonConvert.DeserializeObject<HRPR_SifMaster_TR>(httprequest["SIFMst"].ToString());
                //IList<HRPR_SifDetail_TR> objsifDetail = JsonConvert.DeserializeObject<IList<HRPR_SifDetail_TR>>(httprequest["SIFDetail"].ToString());
                IList<ExportSIfMapper> objsifDetail = JsonConvert.DeserializeObject<IList<ExportSIfMapper>>(httprequest["SIFDetail"].ToString());
                //CalculateChargeBindingModel chargemModel = JsonConvert.DeserializeObject<CalculateChargeBindingModel>(httprequest["ChargeModel"].ToString());
                CalculateMultipleChargeBindingModel chargemModel = JsonConvert.DeserializeObject<CalculateMultipleChargeBindingModel>(httprequest["ChargeModel"].ToString());

                HRPR_ExportSIFFiles_HI expsifdata = _sifexpMasterSer.SubmitExportSIFFiles(objsifmaster, chargemModel, IsCards, Establistmentdid, UserId, Month, Year);
                var data = _sifexpDetailSer.SubmitSIFDETAIL(objsifDetail, objsifmaster.CompanyID, expsifdata.ExpSifID, expsifdata.FileName, IsCards);

                if (Isdirect == "Yes")
                {
                    IList<HRPR_BulkSIFDetails_HI> objbulsifdetail = JsonConvert.DeserializeObject<IList<HRPR_BulkSIFDetails_HI>>(httprequest["SIFDetail"].ToString());
                    _bulksiFDetailSer.UpdateGenerateSIF(objbulsifdetail, UserId);
                }
                return Ok(data);


            }
            catch (Exception Ex)
            {

                return this.BadResponse(Ex.Message);
            }


            return Ok("");
        }


        [ActionName("SP_GetSIFGenerationlst")]
        [HttpGet]
        public IHttpActionResult GetSIFData(int UserId, DateTime? Date)
        {
            try
            {
                var data = _sifMasterSer.GetSIFData(UserId, Date);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        [ActionName("SP_GetSIFDetailbyId")]
        [HttpGet]
        public IHttpActionResult GetSIFDetailsbyId(int ExpSifId)
        {
            try
            {
                var data = _sifMasterSer.GetSIFDetailsbyId(ExpSifId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }
        [ActionName("WPS_PushDatatoWPSTable")]
        [HttpGet]
        public IHttpActionResult PushDatatoWPSTable(int ExpSifId)
        {
            try
            {
                var data = _sifMasterSer.WPS_PushDatatoWPSTable(ExpSifId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message, "WPS_PushDatatoWPSTable");
                return this.BadResponse(ex.Message);
            }
        }

        [ActionName("SP_WPS_Status")]
        [HttpGet]
        public IHttpActionResult WPS_Status(int ExpSifId)
        {
            try
            {
                var data = _sifMasterSer.WPS_Status(ExpSifId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message, "SP_WPS_Status");
                return this.BadResponse(ex.Message);
            }
        }

        [ActionName("GetSifDetailbyCompanyId")]
        [HttpGet]
        public IHttpActionResult GetSifDetailbyCompanyId(int CompanyId)
        {
            try
            {
                if (CompanyId == 0 && CompanyId != null)
                    return BadResponse("Please Select Company");

                var returnSifDetail = _sifexpMasterSer.GetSifDetailbyCompanyId(CompanyId);

                return Ok(returnSifDetail);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }


        [ActionName("CalculateMultipleCharges")]
        [HttpPost]
        public IHttpActionResult CalculateMultipleCharges(CalculateMultipleChargeBindingModel model)
        {
            try
            {
                var data = _accountTransactionService.CalculateMultipleCharges(model);
                return Ok(data);
            }
            catch (Exception Ex)
            {

                return this.BadResponse(Ex.Message);
            }
            return Ok("");
        }

        [ActionName("AddSifNakClaim")]
        [HttpPost]
        public IHttpActionResult AddSifNakClaim(SifNakClaimedModel obj)
        {
            try
            {
                var sifNakClaim = _sifMasterSer.SifNakClaim(obj);
                return Ok(sifNakClaim);
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message + "Request Model" + obj, "SIFNakClaimed");
                return this.BadResponse(ex.Message);
            }
        }

    }

}



public class ExportSIfMapper
{
    public string RecordType { get; set; }
    public int? ExpSifId { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int? EmpId { get; set; }

    public decimal? Basic { get; set; }

    public decimal? Allowances { get; set; }

    public decimal? NetSalary { get; set; }

    public decimal? Total { get; set; }

    public decimal? LeaveDays { get; set; }

    public string RoutingCode { get; set; }

    public string IBAN { get; set; }

    public string PersonalNo { get; set; }

    public decimal? FixAmount { get; set; }

}