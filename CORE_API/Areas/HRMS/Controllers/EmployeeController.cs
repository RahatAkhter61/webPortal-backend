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
using System.Collections;
using static CORE_API.Areas.HRMS.Models.UnprocesessedParameters;
using ContextMapper;

namespace CORE_API.Areas.HRMS.Controllers
{
    public class EmployeeController : BaseController
    {
        // master_new Branch

        private readonly CompanyService _companySer;
        private readonly EmployeeService _empSer;
        private readonly EmployeeDocumentService _docSer;
        private readonly EmployeeChargesService _chargesSer;
        private readonly EmployeeSalaryService _empSalarySer;
        public EmployeeController()
        {
            _empSer = new EmployeeService();
            _companySer = new CompanyService();
            _docSer = new EmployeeDocumentService();
            _chargesSer = new EmployeeChargesService();
            _empSalarySer = new EmployeeSalaryService();

        }


        [HttpGet]
        [ActionName("GetEmployeeStatus")]
        public IHttpActionResult GetEmployeeStatus()
        {

            var data = _empSer.GetEmployeeStatus();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetEmployeeType")]
        public IHttpActionResult GetEmployeeType()
        {

            var data = _empSer.GetEmployeeType();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetPositions")]
        public IHttpActionResult GetPoBsitions()
        {

            var data = _empSer.GetPositions();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetEmployeeProduct")]
        public IHttpActionResult GetEmployeeProduct()
        {

            var data = _empSer.GetEmployeeProduct();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetNationality")]
        public IHttpActionResult GetNationality()
        {
            var data = _empSer.GetNationality();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetBank")]
        public IHttpActionResult GetBank()
        {
            var data = _empSer.GetBank();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetEmployee")]
        public IHttpActionResult GetEmployee()
        {
            var data = _empSer.GetEmployee();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetEmployeeById")]
        public IHttpActionResult GetEmployeeById(int EmpId)
        {
            var data = _empSer.GetEmployeeById(EmpId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetEmployeebyCompany")]
        public IHttpActionResult GetEmployeebyCompany(int? CompanyId)
        {
            var data = _empSer.GetEmployeebyCompany(CompanyId);
            return Ok(data);
        }


        [HttpGet]
        [ActionName("GetEmployeeDocuments")]
        public IHttpActionResult GetEmployeeDocuments(int EmpId)
        {
            var data = _empSer.GetEmployeeDocuments(EmpId);
            return Ok(data);
        }


        [HttpGet]
        [ActionName("GetEmployeeCharges")]
        public IHttpActionResult GetEmployeeCharges(int EmpId)
        {
            var data = _empSer.GetEmployeeCharges(EmpId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetEmployeeSalary")]
        public IHttpActionResult GetEmployeeSalary(int EmpId)
        {

            var data = _empSalarySer.GetEmployeeSalary(EmpId);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("UpdateEmployeeDocument")]
        public IHttpActionResult UpdateEmployeeDocument(EmpDocRequestModel obj)
        {

            var data = _docSer.UpdateEmployeeDocument(obj.EmpId, obj.EmiratesId, obj.IssueDate, obj.ExpiryDate, obj.UserId);
            return Ok(data);
        }

        [ActionName("SubmitEmployee")]
        [HttpPost]
        public IHttpActionResult SubmitEmployee()
        {
            try
            {
                var response = new DTO();
                var httprequest = HttpContext.Current.Request;
                var EmployeeInfo = httprequest["EmployeeInfo"];
                var EmployeeDocument = httprequest["EmployeeDocument"];
                var establishmentId = httprequest["EstablishmentId"];

                if (establishmentId != null)
                    establishmentId = JsonConvert.DeserializeObject<string>(establishmentId.ToString());

                var employee = JsonConvert.SerializeObject(EmployeeInfo);
                Logger.TraceService(employee, "EmployeeDetail");

                HRMS_EmployeeMst_ST employeeInfo = JsonConvert.DeserializeObject<HRMS_EmployeeMst_ST>(EmployeeInfo.ToString());

                #region Add New Employee
                if (employeeInfo != null && employeeInfo.EmpId == 0)
                {

                    response = _empSer.SubmitEmployee(employeeInfo, establishmentId);
                    var currentEmployee = (HRMS_EmployeeMst_ST)response.data;


                    if (!response.isSuccessful)
                        return this.BadResponse(response.errors);

                    if (establishmentId != null)
                        response = _empSer.AddEmployeeToEstablishmentId(establishmentId, currentEmployee);


                    if (response.isSuccessful && EmployeeDocument != null)
                    {

                        response = _companySer.GetCompanyDetailByCompanyId(currentEmployee.CompanyId);
                        var employerdetail = (SMAM_CompanyMst_ST)response.data;

                        IList<HRMS_EmpDocs_ST> Employeedoc = JsonConvert.DeserializeObject<IList<HRMS_EmpDocs_ST>>(EmployeeDocument.ToString());
                        response = _docSer.SubmitDocuments(Employeedoc as IList<HRMS_EmpDocs_ST>, currentEmployee.EmpId, employerdetail.CustomerId, currentEmployee.CustomerId);

                        if (!response.isSuccessful)
                            return this.BadResponse("Employee Document Not Save");

                        if (response.isSuccessful && httprequest.Files.Count > 0)
                            response = _docSer.AddEmployeeDocumentsToServer(httprequest, currentEmployee.EmpId, employerdetail.CustomerId, currentEmployee.CustomerId);

                    }
                    return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = "Submit Successfully" });

                }
                #endregion

                #region Update Employee
                if (employeeInfo != null && employeeInfo.EmpId != 0)
                {
                    response = _empSer.UpdateEmployee(employeeInfo, establishmentId);
                    var currentEmployee = (HRMS_EmployeeMst_ST)response.data;

                    if (!response.isSuccessful)
                        return this.BadResponse(response.errors);

                    if (establishmentId != null)
                        response = _empSer.AddEmployeeToEstablishmentId(establishmentId, currentEmployee);

                    if (response.isSuccessful && EmployeeDocument != null)
                    {
                        response = _companySer.GetCompanyDetailByCompanyId(currentEmployee.CompanyId);
                        var employerdetail = (SMAM_CompanyMst_ST)response.data;

                        IList<HRMS_EmpDocs_ST> Employeedoc = JsonConvert.DeserializeObject<IList<HRMS_EmpDocs_ST>>(EmployeeDocument.ToString());
                        response = _docSer.SubmitDocuments(Employeedoc as IList<HRMS_EmpDocs_ST>, currentEmployee.EmpId, employerdetail.CustomerId, currentEmployee.CustomerId);

                        if (!response.isSuccessful)
                            return this.BadResponse("Employee Document Not Update");

                        if (response.isSuccessful && httprequest.Files.Count > 0)
                            response = _docSer.AddEmployeeDocumentsToServer(httprequest, currentEmployee.EmpId, employerdetail.CustomerId, currentEmployee.CustomerId);
                    }

                    return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = null });

                }
                #endregion
                return Ok(response);
            }
            catch (Exception Ex)
            {

                var error = JsonConvert.SerializeObject(Ex);
                Logger.TraceService(error, "EmployeeError");
                return this.BadResponse(Ex.Message);
            }
        }

        // Owais Siddiq



        [ActionName("AddEmployee")]
        [HttpPost]
        public IHttpActionResult AddEmployee()
        {
            try
            {
                var httprequest = HttpContext.Current.Request;
                HRMS_EmployeeMst_ST objemp = JsonConvert.DeserializeObject<HRMS_EmployeeMst_ST>(httprequest["EmployeeMst"].ToString(), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });


                var result = _empSer.AddEmployee(objemp);
                return Ok(result);

            }
            catch (Exception Ex)
            {
                return this.SuccessResponse(Ex.Message);
            }
            return Ok("");
        }

        //[ActionName("AddEmpDocument")]
        //[HttpPost]
        //public IHttpActionResult AddEmpDocument()
        //{
        //    try
        //    {
        //        var httprequest = HttpContext.Current.Request;

        //        IList<HRMS_EmpDocs_ST> objempdoc = JsonConvert.DeserializeObject<IList<HRMS_EmpDocs_ST>>(httprequest["EmployeeDoc"].ToString(), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        //        int? EmployeeId = Convert.ToInt32(httprequest["EmployeeId"]);

        //        _docSer.SubmitDocuments(objempdoc, Convert.ToInt32(EmployeeId));
        //        return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = null });

        //    }
        //    catch (Exception Ex)
        //    {
        //        return this.SuccessResponse(Ex.Message);
        //    }
        //    return Ok("");
        //}

        [ActionName("ViewDocument")]
        [HttpGet]
        public IHttpActionResult ViewDocument(string doc)
        {

            string basefilepath = ConfigurationManager.AppSettings["DocPath"].ToString();
            try
            {
                byte[] readText = File.ReadAllBytes(basefilepath + "/" + doc);
                String Document = Convert.ToBase64String(readText);
                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = Document });
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message, "Dcoument Error");
                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = false, errors = "Document Not Found in the Server", data = null });
            }



        }

        [ActionName("GetDocuments")]
        [HttpGet]
        public IHttpActionResult GetDocuments()
        {
            try
            {
                var data = _empSer.GetDocument();
                return Ok(data);
            }
            catch (Exception Ex)
            {
                return this.SuccessResponse(Ex.Message);

            }
            return Ok("");

        }


        [ActionName("DocumentsSave")]
        [HttpPost]
        public IHttpActionResult DocumentsSave()
        {

            try
            {
                string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/");  // @"E:\SFTP";
                var httprequest = HttpContext.Current.Request;
                object data = null;
                if (httprequest.Files.Count > 0)
                {
                    string FileName = "";
                    int count = 0;


                    string desc = httprequest["Desc"].ToString();
                    int? EmployeeId = Convert.ToInt32(httprequest["EmployeeId"]);
                    string CustomerId = httprequest["CustomerId"].ToString();
                    string CompCustomerId = httprequest["CompCustomerId"].ToString();


                    string[] descarr = desc.Split(',');
                    foreach (string file in httprequest.Files)
                    {

                        var Count = Convert.ToInt32(file.ToString().Replace("Files", ""));

                        var postedfile = httprequest.Files[file];

                        FileName = postedfile.FileName.ToString();

                        var filePath = @"\" + "00 - Portal";
                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }

                        filePath = filePath + @"\" + CompCustomerId;
                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }


                        filePath = filePath + @"\Employee Documents";
                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }

                        filePath = filePath + @"\" + CustomerId;
                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }

                        filePath = filePath + @"\" + CustomerId + "_" + descarr[Count].Replace(" ", "") + Path.GetExtension(postedfile.FileName);
                        postedfile.SaveAs(basefilepath + filePath);
                        count++;


                    }
                }
                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = null });
            }
            catch (Exception ex)
            {

                return this.InternalServerError();
            }


        }

        [ActionName("GetEmployeeDetails")]
        [HttpPost]
        public IHttpActionResult GetEmployeeDetails(int CompanyId, int ProductId, string EmpCode, string MobileNo)
        {
            var data = _empSer.GetEmployeeDetails(CompanyId, ProductId, EmpCode, MobileNo);
            return Ok(data);

        }


        [HttpGet]
        [ActionName("SPGetEmployeebyCompany")]
        public IHttpActionResult SPGetEmployeebyCompany(int CompanyId)
        {
            var data = _empSer.SPGetEmployeebyCompany(CompanyId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("SPGetEmployeeTrans")]
        public IHttpActionResult SPgetEmployeeTrans(string CustomerId, int AccountTypeId)
        {
            var data = _empSer.SPgetEmployeeTrans(CustomerId, AccountTypeId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("SPGetEmpScreeningTimeline")]
        public IHttpActionResult SPGetEmpScreeningTimeline(int? empId)
        {
            var data = _empSer.SPGetEmpScreeningTimeline(empId);
            return Ok(data);
        }


        [HttpGet]
        [ActionName("SPGetEmployeeById")]
        public IHttpActionResult SPGetEmployeeById(int EmpId)
        {
            var data = _empSer.SPGetEmployeeById(EmpId);
            return Ok(data);
        }

        [ActionName("GetEmployeeEmirateID")]
        [HttpGet]
        public IHttpActionResult GetEmployeeEmirateID(string CustomerId)
        {
            string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/");
            if (basefilepath != null)
            {
                var GetFile = new List<string>();
                ArrayList result = new ArrayList();

                var files = Directory.GetFiles(basefilepath);

                if (files.Length == 0)
                {
                    return Ok(new CORE_BASE.CORE.DTO { isSuccessful = false, errors = "File Not Found", data = null });
                }

                foreach (var f in files)
                {
                    if (f.Contains(CustomerId + "_EmiratesIdBackImage") || f.Contains(CustomerId + "_EmiratesIdFrontImage"))
                    {
                        GetFile.Add(f);
                    }
                }

                if (GetFile.Count == 0)
                {
                    return Ok(new CORE_BASE.CORE.DTO { isSuccessful = false, errors = "Document Not Found in the Server", data = null });
                }
                foreach (var f in GetFile)
                {
                    string base64String = null;
                    using (System.Drawing.Image image = System.Drawing.Image.FromFile(f))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();
                            base64String = Convert.ToBase64String(imageBytes);
                            var removepath = f.Replace(basefilepath, "");
                            var filename = removepath.Replace(CustomerId + "_", "");
                            var jsonResult = new { EmirateID = base64String, path = filename };
                            result.Add(jsonResult);
                        };

                    }
                }
                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = result });
            }

            return Ok();
        }

        [HttpGet]
        [ActionName("SP_GetEmpbyCompAndprod")]

        public IHttpActionResult SP_GetEmpbyComp_prod(int CompanyId, int ProductId, string ScreenType)
        {
            var data = _empSer.SP_GetEmpbyCompprod(CompanyId, ProductId, ScreenType);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("SP_GetCompbyProduct")]
        public IHttpActionResult GetCompProduct(int CompanyId)
        {
            var data = _empSer.Get_CompbyProduct(CompanyId);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("SP_GetEmployeebymultiFilter")]
        public IHttpActionResult SPGetEmployeebymultiFilter(UpdateEmployeeArchive obj)
        {
            var data = _empSer.SPGetEmployeebymultiFilter(obj);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("SP_UpdateEmployeeArchive")]
        public IHttpActionResult SPUpdateEmployeeArchive(List<UpdateEmployeeArchive> obj)
        {
            var data = _empSer.UpdateEmployeeArchive(obj);
            return Ok(data);
        }


        [HttpPost]
        [ActionName("AddEmployeeLinkingToEstablishment")]
        public IHttpActionResult AddEmployeeLinkingToEstablishment()
        {
            var files = HttpContext.Current.Request.Files;
            if (files == null || files.Count == 0)
                return Ok("file not found");

            var stream = Request.Content.ReadAsStreamAsync();
            var data = _empSer.AddEmployeeToEst(files, stream.Result);
            return Ok(data);
        }



        [HttpGet]
        [ActionName("GetAllEmployee")]
        public IHttpActionResult GetAllEmployee(int pageNumber, int numberofRecord)
        {
            var employeeList = _empSer.GetAllEmployee(pageNumber, numberofRecord);
            return Ok(employeeList);
        }


        //[ActionName("SubmitSalary")]
        //[HttpPost]
        //public IHttpActionResult SubmitSalary(IList<HRMS_EmpSalary_ST> obj)
        //{
        //    var data = _empSalarySer.SubmitSalary(obj);
        //    return Ok(data);

        //}

        //[HttpDelete]
        //[ActionName("DeleteMenu")]
        //public IHttpActionResult DeleteMenu(long Id)
        //{
        //    var data = _menuSer.DeleteMenu(Id);
        //    return Ok(data);
        //}
    }


}


//public class Attachments
//{
//    public object file { get; set; }
//    public int CompanyId { get; set; }
//}