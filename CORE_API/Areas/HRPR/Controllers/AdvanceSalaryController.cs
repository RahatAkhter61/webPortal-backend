using CORE_API.Areas.General.Controllers;
using CORE_API.General;
using CORE_API.Models;
using CORE_API.Models.AdvanceSalary;
using CORE_API.Services.HRPR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace CORE_API.Areas.HRPR.Controllers
{
    public class AdvanceSalaryController : BaseController
    {
        private readonly AdvanceSalaryService _advanceSalaryService;
        public AdvanceSalaryController()
        {
            _advanceSalaryService = new AdvanceSalaryService();
        }

        [HttpGet]
        [ActionName("GetAdvanceSalary")]
        public IHttpActionResult GetAdvanceSalary(int? pageNo = null, int limit = 0, int UserId = 0)
        {
            try
            {
                var getAdvanceSalaryRecords = _advanceSalaryService.GetAdvanceSalary(pageNo, limit, UserId);
                return Ok(getAdvanceSalaryRecords);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }

        }
        [HttpGet]
        [ActionName("GetEligibleEmployeeByCompanyId")]
        public IHttpActionResult GetEligibleEmployeeByCompanyId(int companyId, int pageNo, int limit, string customerId)
        {
            try
            {
                var getAdvanceSalaryRecords = _advanceSalaryService.EmployeeAdvanceEligibility(companyId, customerId, pageNo, limit);
                return Ok(getAdvanceSalaryRecords);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }

        }

        [HttpGet]
        [ActionName("GetAdvanceSalaryDetailbyId")]
        public IHttpActionResult GetAdvanceSalaryDetailbyId(int AdvanceSalaryId)
        {
            try
            {
                var getAdvanceSalaryDetailId = _advanceSalaryService.GetAdvanceSalaryDetailbyId(AdvanceSalaryId);
                return Ok(getAdvanceSalaryDetailId);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }

        }

        [HttpGet]
        [ActionName("GetEmployeeSalaryInformation")]
        public IHttpActionResult GetEmployeeSalaryInformation(string personalNo)
        {
            try
            {
                var getAdvanceSalaryRecords = _advanceSalaryService.GetEmployeeSalaryInformation(personalNo);
                return Ok(getAdvanceSalaryRecords);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }

        }

        [HttpPost]
        [ActionName("UpdateAdvanceSalaryStatus")]
        public IHttpActionResult UpdateAdvanceSalaryStatus(List<AdvanceSalaryModel> model)
        {
            try
            {
                var updateAdvanceSalaryStatus = _advanceSalaryService.UpdateAdvanceSalaryStatus(model);
                return Ok(updateAdvanceSalaryStatus);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }

        }

        [HttpGet]
        [ActionName("GetAdvanceSalaryStatus")]
        public IHttpActionResult GetAdvanceSalaryStatus(int UserId)
        {
            try
            {
                var statusList = _advanceSalaryService.GetAdvanceSalaryStatus(UserId);
                return Ok(statusList);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpGet]
        [ActionName("GetAllPreviousAdvanceSalaries")]
        public IHttpActionResult GetAllPreviousAdvanceSalaries(int empId)
        {
            try
            {
                var previousAdvanceSalaries = _advanceSalaryService.GetAllPreviousAdvanceSalaries(empId);
                return Ok(previousAdvanceSalaries);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost]
        [ActionName("AddEditAdditinoalQuestionAdvanceSalary")]
        public IHttpActionResult AddEditAdditinoalQuestionAdvanceSalaryAsync(AdditinoalQuestionAdvanceSalaryModel model)
        {
            try
            {
                var verifyAdvanceSalaryDetail = _advanceSalaryService.AddEditAdditinoalQuestionAdvanceSalary(model);
                return Ok(verifyAdvanceSalaryDetail);
            }
            catch (Exception ex)
            {
                var actionValue = JsonConvert.SerializeObject(model);
                Common.SubmitLog(new LogBindingModel
                {
                    Action = "AddEditAdditinoalQuestionAdvanceSalary",
                    Module = "Advance Salary",
                    ActionValue = actionValue,
                    Error = ex.Message.ToString(),
                });

                return BadResponse(ex.Message);
            }
        }


        [HttpGet]
        [ActionName("GetAdvanceSalaryDetailInfobyId")]
        public IHttpActionResult GetAdvanceSalaryDetailInfobyId(int advanceSalaryId)
        {
            try
            {
                var advanceSalaryDetailInfo = _advanceSalaryService.GetAdvanceSalaryDetailInfobyId(advanceSalaryId);
                return Ok(advanceSalaryDetailInfo);
            }
            catch (Exception ex)
            {

                Common.SubmitLog(new LogBindingModel
                {
                    Action = "GetAdvanceSalaryDetailInfobyId",
                    Module = "Advance Salary",
                    ActionValue = "advanceSalaryId " + advanceSalaryId,
                    Error = ex.Message.ToString(),
                });
                return BadResponse(ex.Message);
            }
        }

        [HttpGet]
        [ActionName("GetAdditinoalQuestionStatus")]
        public IHttpActionResult GetAdditinoalQuestionStatus()
        {
            try
            {
                var additinoalQuestionStatus = _advanceSalaryService.GetAdditinoalQuestionStatus();
                return Ok(additinoalQuestionStatus);
            }
            catch (Exception ex)
            {

                Common.SubmitLog(new LogBindingModel
                {
                    Action = "GetAdditinoalQuestionStatus",
                    Module = "Advance Salary",
                    ActionValue = "",
                    Error = ex.Message.ToString(),
                });

                return BadResponse(ex.Message);
            }
        }

    }
}