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
using System.Drawing;
using System.Configuration;
using System.Text;
using CORE_API.Models.ComapnyWhiteListing;

namespace CORE_API.Areas.SMAM.Controllers
{
    public class CompanyWhitelistingController : BaseController
    {
        private readonly CompanyWhitelistingService _companyWL;
        public CompanyWhitelistingController()
        {
            _companyWL = new CompanyWhitelistingService();
        }


        [HttpPost]
        [ActionName("AddEditCompanyWhitelisting")]
        public IHttpActionResult AddEditCompanyWhitelisting(IList<CompanyWhileListing> obj)
        {
            try
            {
                var data = _companyWL.AddEditCompanyWhitelisting(obj);
                return Ok(data);
            }
            catch (Exception Ex)
            {
                return this.BadResponse(Ex.Message);
            }
            
        }


        [HttpGet]
        [ActionName("GetAllCompanyWhitelisting")]
        public IHttpActionResult GetCompanyWhitelisting(int userId ,int? pageNo = null, int limit = 0)
        {
            try
            {
                var getCompanyWhitelisting = _companyWL.GetCompanyWhitelisting(userId,pageNo, limit);
                return Ok(getCompanyWhitelisting);

            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }
        [HttpGet]
        [ActionName("GetAllCompanyNonWhitelisting")]
        public IHttpActionResult GetAllCompanyNonWhitelisting()
        {
            try
            {
                var getAllCompanyNonWhitelisting = _companyWL.GetAllCompanyNonWhitelisting();
                return Ok(getAllCompanyNonWhitelisting);

            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet]
        [ActionName("GetCompanyWhitelistStatus")]
        public IHttpActionResult GetCompanyWhitelistStatus()
        {
            try
            {
                var companyWhiteListStatus = _companyWL.CompanyWhitelistStatus();
                return Ok(companyWhiteListStatus);

            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet]
        [ActionName("GetCompanySalaryBracket")]
        public IHttpActionResult GetCompanySalaryBracket(int companyId)
        {
            try
            {
                var companySalaryBracket = _companyWL.CompanySalaryBracket(companyId);
                return Ok(companySalaryBracket);

            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost]
        [ActionName("updateCompanySalaryBracket")]
        public IHttpActionResult AddEditCompanySalaryBracket()
        {
            try
            {
                var httprequest = HttpContext.Current.Request;
                int companyId = Convert.ToInt16(httprequest["companyId"].ToString());

                IList<CompanyLoanBracketModel> companyLoanBracket = JsonConvert.DeserializeObject<IList<CompanyLoanBracketModel>>(httprequest["CompanyLoanBracket"].ToString());

                var updateCompanySalaryBracket = _companyWL.AddEditCompanySalaryBracket(companyLoanBracket, companyId);

                return Ok(updateCompanySalaryBracket);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


    }
}