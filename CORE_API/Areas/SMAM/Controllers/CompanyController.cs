using CORE_API.Areas.SMAM.Models;
using CORE_API.General;
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
using System.Text;
using System.Configuration;
using CORE_API.Services.HRMS;
using CORE_API.Models;
using CORE_API.Services.HRPR;

namespace CORE_API.Areas.SMAM.Controllers
{
    public class CompanyController : BaseController
    {

        private readonly CompanyService _companySer;
        private readonly DocumentService _docSer;
        private readonly ChargesService _chargesSer;
        private readonly CompanyEstService _compEsb;
        private readonly CampService _CampSer;
        private readonly CompanyProductService _companyProductService;
        private readonly AccountTransactionService _accountTransactionService;
        private readonly ManualChargeService _manualChargeService;
        dbEngine dbEngine = new dbEngine();

        public CompanyController()
        {
            _companySer = new CompanyService();
            _docSer = new DocumentService();
            _chargesSer = new ChargesService();
            _compEsb = new CompanyEstService();
            _CampSer = new CampService();
            _companyProductService = new CompanyProductService();
            _accountTransactionService = new AccountTransactionService();
            _manualChargeService = new ManualChargeService();
        }

        [HttpGet]
        [ActionName("GetCampStatus")]
        public IHttpActionResult GetCampStatus()
        {

            var data = _companySer.GetCampStatus();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetChargesType")]
        public IHttpActionResult GetChargesType()
        {

            var data = _companySer.GetChargesType();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetCountry")]
        public IHttpActionResult GetCountry()
        {

            var data = _companySer.GetCountry();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetStates")]
        public IHttpActionResult GetStates()
        {

            var data = _companySer.GetStates();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetSalesAccount")]
        public IHttpActionResult GetSalesAccount()
        {
            var data = _companySer.GetSalesAccount();
            return Ok(data);
        }
        [HttpGet]
        [ActionName("GetCompanybyUser")]
        public IHttpActionResult GetCompanybyUser(int UserId)
        {
            var data = _companySer.GetCompanybyUser(UserId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetCompanyDataById")]
        public IHttpActionResult GetCompanyDataById(int CompanyId)
        {

            try
            {
                var data = _companySer.GetCompanyDataById(CompanyId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }

        }

        [HttpGet]
        [ActionName("GetCompanyDocuments")]
        public IHttpActionResult GetCompanyDocuments(int CmpDocId)
        {
            var data = _companySer.GetCompanyDocuments(CmpDocId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetCompanyCharges")]
        public IHttpActionResult GetCompanyCharges(int CmpChrgId)
        {
            var data = _companySer.GetCompanyCharges(CmpChrgId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetStationsByCompany")]
        public IHttpActionResult GetStationsByCompany(int CompanyId)
        {
            var data = _companySer.GetStationsByCompany(CompanyId);
            return Ok(data);
        }
        [HttpGet]
        [ActionName("GetDepartmentsByStation")]
        public IHttpActionResult GetDepartmentsByStation(int StationId)
        {
            var data = _companySer.GetDepartmentsByStation(StationId);
            return Ok(data);
        }

        [ActionName("SubmitCompany")]
        [HttpPost]
        public IHttpActionResult SubmitCompany()
        {
            try
            {
                var httprequest = HttpContext.Current.Request;
                SMAM_CompanyMst_ST objcompany = JsonConvert.DeserializeObject<SMAM_CompanyMst_ST>(httprequest["CompanyMst"].ToString());
                IList<SMAM_CompanyDocs_ST> objdoc = JsonConvert.DeserializeObject<IList<SMAM_CompanyDocs_ST>>(httprequest["CompanyDoc"].ToString());
                IList<SMAM_CompanyCharges_ST> objcharges = JsonConvert.DeserializeObject<IList<SMAM_CompanyCharges_ST>>(httprequest["CompanyCharges"].ToString());

                _companySer.SubmitCompany(objcompany);
                _docSer.SubmitDocuments(objdoc, objcompany.CompanyId);
                _chargesSer.SubmitCharges(objcharges, objcompany.CompanyId);

                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = null });
            }
            catch (Exception Ex)
            {
                return this.BadResponse(Ex.Message);
            }
            return Ok("");
        }


        [ActionName("AddCompany")]
        [HttpPost]
        public IHttpActionResult AddCompany()
        {

            try
            {
                var httprequest = HttpContext.Current.Request;
                var response = new DTO();

                var CompanyDocdetail = httprequest["CompanyDocdetail"];
                var CompanyCharges = httprequest["CompanyCharges"];
                var CompanyProduct = httprequest["CompanyProduct"];

                SMAM_CompanyMst_ST objcompany = JsonConvert.DeserializeObject<SMAM_CompanyMst_ST>(httprequest["CompanyMst"].ToString());
                response = _companySer.SubmitCompany(objcompany);
                var currentCompany = (SMAM_CompanyMst_ST)response.data;

                #region compnay_document
                if (response.isSuccessful && httprequest.Files.Count > 0)
                {
                    response = _docSer.AddCompanyDocuments(httprequest, currentCompany.CompanyId, currentCompany.CustomerId);
                }
                #endregion

                #region document_details
                if (response.isSuccessful && CompanyDocdetail != null)
                {
                    IList<SMAM_CompanyDocs_ST> objdoc = JsonConvert.DeserializeObject<IList<SMAM_CompanyDocs_ST>>(httprequest["CompanyDocdetail"].ToString());
                    response = _docSer.SubmitDocumentNew(objdoc as IList<SMAM_CompanyDocs_ST>, currentCompany.CompanyId, currentCompany.CustomerId);
                }
                #endregion

                #region company_charges
                if (response.isSuccessful && CompanyCharges != null)
                {
                    IList<SMAM_CompanyCharges_ST> objCharges = JsonConvert.DeserializeObject<IList<SMAM_CompanyCharges_ST>>(httprequest["CompanyCharges"].ToString());
                    response = _chargesSer.SubmitCharges(objCharges as IList<SMAM_CompanyCharges_ST>, currentCompany.CompanyId);
                }
                #endregion

                #region company_product
                if (response.isSuccessful && CompanyProduct != null)
                {
                    IList<SMAM_CompanyProducts_ST> objprod = JsonConvert.DeserializeObject<IList<SMAM_CompanyProducts_ST>>(httprequest["CompanyProduct"].ToString());
                    response = _companyProductService.AddCompanyProduct(objprod as List<SMAM_CompanyProducts_ST>, currentCompany.CompanyId);
                }
                #endregion
                if (response.isSuccessful)
                {
                    return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = "Successfully Save" });
                }
                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = response.isSuccessful, errors = response.errors, data = response.data });
            }
            catch (Exception Ex)
            {
                return this.BadResponse(Ex.Message);
            }


        }


        [ActionName("SubmitCompanyDocuments")]
        [HttpPost]
        public IHttpActionResult SubmitCompanyDocuments()
        {

            string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/");  // @"E:\SFTP";
            var httprequest = HttpContext.Current.Request;
            object data = null;
            if (httprequest.Files.Count > 0)
            {
                string FileName = "";
                int count = 0;

                int? CompanyId = Convert.ToInt32(httprequest["CompanyId"]);
                string desc = httprequest["Desc"].ToString();
                string companyname = httprequest["Name"].ToString();

                string[] descarr = desc.Split(',');
                foreach (string file in httprequest.Files)
                {
                    var postedfile = httprequest.Files[file];


                    FileName = postedfile.FileName.ToString();

                    //FileName = new String(Path.GetFileNameWithoutExtension(postedfile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    //FileName = FileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedfile.FileName);


                    var filePath = @"\" + companyname; ;/// basefilepath + @"\" + companyname; 
                    //  // HttpContext.Current.Server.MapPath("~/Company/" + FileName);

                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }

                    filePath = filePath + @"\Company Documents";
                    //  // HttpContext.Current.Server.MapPath("~/Company/" + FileName);

                    if (filePath != null)
                    {
                        if (!Directory.Exists(basefilepath + filePath))
                        {
                            Directory.CreateDirectory(basefilepath + filePath);
                        }
                    }

                    filePath = filePath + @"\" + FileName;
                    postedfile.SaveAs(basefilepath + filePath);
                    data = _docSer.UpdateDocuments(descarr[count], filePath, FileName, CompanyId);
                    count++;

                    //  _docSer.UpdateDocuments();
                }
            }
            return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = null });

        }

        [ActionName("GetCompanybyEstabl")]
        [HttpGet]
        public IHttpActionResult GetCompanybyEstabl(int CompanyId)
        {
            var data = _companySer.GetCompanybyEstabl(CompanyId);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("SubmitCompanyEstbl")]
        public IHttpActionResult SaveSubmitCompanyEstbl(SMAM_CompanyEst_HI obj)
        {
            var data = _compEsb.SubmitCompanyEstbl(obj);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("SPGetCompanyEsblist")]
        public IHttpActionResult SPGetCompanyEsblst(int UserId)
        {
            var data = _compEsb.SP_Get_CompanyEsblst(UserId);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("UpdateStatus")]
        public IHttpActionResult UpdateStatus(IList<SMAM_CompanyEst_HI> list)
        {
            var data = _compEsb.Update_Status(list);
            this.ExportFile(list);
            return Ok(data);
        }


        [HttpPost]
        [ActionName("Add_CampAllocation")]
        public IHttpActionResult CampAllocation(SMAM_Camp_ST obj)
        {
            var data = _CampSer.SP_CampAllocation(obj);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GET_CampAllocation")]
        public IHttpActionResult GET_CampAllocation(int? UserId)
        {
            var data = _CampSer.SP_GETCampAllocation(UserId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GET_SP_ForCamp")]
        public IHttpActionResult GET_ForCamp(int CompanyId)
        {
            var data = _CampSer.GET_ForCamp(CompanyId);
            return Ok(data);
        }
        [HttpGet]
        [ActionName("GET_SP_EmpbyCamp")]
        public IHttpActionResult GET_CampbyEmp(int CampId)
        {
            var data = _CampSer.GET_CampbyEmp(CampId);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("SP_Insert_CampEmp")]
        public IHttpActionResult SP_Insert_CampEmp(List<SMAM_CampEmp_ST> obj)
        {
            var data = _CampSer.Insert_CampEmp(obj);
            return Ok(data);
        }

        [HttpPost]
        [ActionName("SP_TranferAmountToCompany")]
        public IHttpActionResult TranferAmountToCompany(TransferAmountToCompany obj)
        {
            var data = _CampSer.SP_TranferAmountToCompany(obj);
            return Ok(data);
        }


        [ActionName("GetCompanyDocument")]
        [HttpGet]
        public IHttpActionResult GetCompanyDocument()
        {
            var data = _companySer.GetCompanyDocument();
            return Ok(data);
        }
        [HttpGet]
        [ActionName("GetParentCompanies")]
        public IHttpActionResult GetParentCompanies()
        {
            var data = _companySer.GetParentCompanies();
            return Ok(data);
        }

        // Generate Mol Establishment .txt File in SFTP Folder
        private void ExportFile(IList<SMAM_CompanyEst_HI> data)
        {

            StreamWriter sw = null;

            string basefilepath = ConfigurationManager.AppSettings["WPS_Mol"].ToString();
            DateTime CurrentDate = DateTime.Now;
            string fileName = string.Empty;
            string filePath = string.Empty;

            // Check Month Folder Format 'MMM'
            filePath = basefilepath + "\\" + CurrentDate.ToString("yyyy");
            if (filePath != null)
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

            }

            // Check Month Folder Format 'MMM'
            filePath = filePath + "\\" + CurrentDate.ToString("MMM");
            if (filePath != null)
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
            }

            // Check Day Folder Format 'dd'
            filePath = filePath + "\\" + CurrentDate.ToString("dd") + "\\";
            if (filePath != null)
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
            }

            var comp = dbEngine._dbContext.SMAM_CompanyEst_HIs.ToList();
            var rtn = comp != null ? comp.Where(o => o.Id == data[0]?.Id).Select(o => o.Employer_ID).FirstOrDefault() : "";

            if (rtn != null && rtn.Length > 13)
                fileName = "KP" + rtn.Trim().Substring(0, 13) + CurrentDate.ToString("yyMMdd") + ".txt";
            else
                fileName = "KP" + rtn.Trim() + CurrentDate.ToString("yyMMdd") + ".txt";

            using (sw = new StreamWriter(Path.Combine(filePath, fileName), true, Encoding.GetEncoding(1252)))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in data)
                {

                    var obj = comp.Where(o => o.Id == item.Id).FirstOrDefault();

                    sb.Clear();
                    sb.Append($"" +
                        $"{obj.Employer_ID}," + // column 1
                        $"{obj.Employer_Name}," + // column 2
                        $"{obj.Employer_Company_Active}," + // column 3
                        $"{obj.Employer_Bank_Account}," + //column 4
                        $"{obj.Employer_Mode}," + // column 5
                        $"{obj.Employer_Email}," + // column 6
                        $"{obj.Employer_DetailReivew_Flag}," + // column 7
                        $"{obj.Employer_Emirate_Code}"); // column 8

                    sw.WriteLine(sb.ToString());
                }

            }

        }

        #region Fund Transfer Region

        [HttpPost]
        [ActionName("AddingTransferOfFunds")]
        public IHttpActionResult AddingTransferOfFunds(TransferOfFundsRequestModel obj)
        {
            var data = _CampSer.AddTransferOfFunds(obj);
            return Ok(data);
        }

        #endregion

        #region Company Charges By CompanyID
        [HttpGet]
        [ActionName("Get_ChargesbyCompanyId")]
        public IHttpActionResult GetChargesbyCompanyId(int CompanyId)
        {
            var companyChargesList = _chargesSer.GetChargesbyCompanyId(CompanyId);
            return Ok(companyChargesList);
        }
        #endregion

        #region Company Product By CompanyID
        [HttpGet]
        [ActionName("Get_ProductbyCompanyId")]
        public IHttpActionResult GetProductbyCompanyId(int CompanyId)
        {
            var data = _companyProductService.GetProductbyCompanyId(CompanyId);
            return Ok(data);
        }
        #endregion

        #region Company Document By CompanyID
        [HttpGet]
        [ActionName("Get_DocumentbyCompanyId")]
        public IHttpActionResult GetDocumentbyCompanyId(int CompanyId)
        {
            var data = _companySer.GetDocumentbyCompanyId(CompanyId);
            return Ok(data);
        }
        #endregion

        #region Update Company Employer 
        [HttpPost]
        [ActionName("Update_Company")]
        public IHttpActionResult Update_Company()
        {
            try
            {
                var httprequest = HttpContext.Current.Request;
                var response = new DTO();
                var Company_info = httprequest["CompanyMst"].ToString();
                //int CompanyId = int.Parse(httprequest["CompanyId"]);
                //var CustomerId = httprequest["CustomerId"].ToString();
                var CompanyDocdetail = httprequest["CompanyDocdetail"];
                var CompanyCharges = httprequest["CompanyCharges"];
                var CompanyProduct = httprequest["CompanyProduct"];


                SMAM_CompanyMst_ST objcompany = JsonConvert.DeserializeObject<SMAM_CompanyMst_ST>(Company_info);
                response = _companySer.UpdateCompany(objcompany);
                var currentCompany = (SMAM_CompanyMst_ST)response.data;

                if (response.isSuccessful && CompanyDocdetail != null && CompanyDocdetail.Length > 0)
                {
                    List<SMAM_CompanyDocs_ST> objdocument = JsonConvert.DeserializeObject<List<SMAM_CompanyDocs_ST>>(CompanyDocdetail);
                    response = _docSer.updateCompanyDocumentDetails(httprequest, objdocument, currentCompany.CompanyId, currentCompany.CustomerId);
                }

                if (response.isSuccessful && CompanyCharges != null && CompanyCharges.Length > 0)
                {
                    List<SMAM_CompanyCharges_ST> objcharges = JsonConvert.DeserializeObject<List<SMAM_CompanyCharges_ST>>(CompanyCharges);
                    response = _chargesSer.updateCompanyCharges(objcharges, currentCompany.CompanyId);
                }

                if (response.isSuccessful && CompanyProduct != null && CompanyProduct.Length > 0)
                {
                    IList<SMAM_CompanyProducts_ST> objprod = JsonConvert.DeserializeObject<IList<SMAM_CompanyProducts_ST>>(CompanyProduct);
                    response = _companyProductService.UpdateCompanyProduct(objprod as List<SMAM_CompanyProducts_ST>, currentCompany.CompanyId);
                }

                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = "Update Successfully Save" });

            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        #endregion

        #region  Document View

        [ActionName("DocumentView")]
        [HttpGet]
        public IHttpActionResult DocumentView(string ImgPath)
        {

            string basefilepath = ConfigurationManager.AppSettings["DocPath"].ToString();
            try
            {
                byte[] readText = File.ReadAllBytes(basefilepath + "/" + ImgPath);
                String Document = Convert.ToBase64String(readText);
                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = Document });
            }
            catch (Exception ex)
            {
                return Ok(new CORE_BASE.CORE.DTO { isSuccessful = false, errors = "Document Not Found in the Server", data = null });
            }



        }
        #endregion


        #region Detete Company Estblistment ID
        [HttpDelete]
        [ActionName("DeleteCompanyEstbl")]
        public IHttpActionResult DeleteCompanyEstbl(long Id)
        {
            var data = _compEsb.DeleteCompanyEstbl(Id);
            return Ok(data);
        }
        #endregion

        #region Edit Company Estblistment ID
        [HttpPost]
        [ActionName("EditCompanyEstbl")]
        public IHttpActionResult EditCompanyEstbl(SMAM_CompanyEst_HI model)
        {
            var data = _compEsb.EditCompanyEstbl(model);
            return Ok(data);
        }
        #endregion

        #region Company Manual Charges Module
        [HttpPost]
        [ActionName("CalculatManualCharges")]
        public IHttpActionResult CalculatManualCharges(ManualChargesModel model)
        {
            try
            {
                if (model == null)
                    return BadResponse("Model should not be null");
                var data = _accountTransactionService.CalculateManualCharges(model);
                return Ok(data);
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost]
        [ActionName("PostManualCharges")]
        public IHttpActionResult PostManualCharges(ManualChargesTransactionModel model)
        {
            try
            {
                if (model == null)
                    return BadResponse("Model should not be null");
                var data = _accountTransactionService.PostManualCharges(model);
                return Ok(data);
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        [ActionName("GetCompany")]
        public IHttpActionResult GetCompany(int? pageNo = 1, int? limit = 1, string customerId = null, string establishmentId = null,bool? isAllCompanies = false)
        {
            try
            {


                var data = _companySer.GetCompany(pageNo, limit, customerId, establishmentId, isAllCompanies);
                return Ok(data);
            }
            catch (Exception Ex)
            {
                return this.BadResponse(Ex.Message);
            }

        }


        #endregion

        #region Get by SP ManualChargesDetail
        [HttpGet]
        [ActionName("GetAllManualCharges")]
        public IHttpActionResult GetAllManualCharges(int? UserId)
        {
            try
            {
                var returnManualChargesDetail = _manualChargeService.GetManualChargesDetail(UserId);
                return Ok(returnManualChargesDetail);
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        #region ProductbyCompanyChargesType
        [HttpGet]
        [ActionName("GetChargesTypebyCompanyandproductid")]
        public IHttpActionResult ChargesTypebyCompanyandproductid(int companyId, int productId)
        {
            try
            {
                var companyChargesType = _chargesSer.ChargesTypebyCompanyandproductid(companyId, productId);
                return Ok(companyChargesType);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }

        }
        #endregion

    }

}

//[ActionName("SubmitDocuments")]
//[HttpPost]
//public IHttpActionResult SubmitDocuments()
//{

//    var httprequest = HttpContext.Current.Request;

//    var postedfile = httprequest.Files["file"];
//    var obj = httprequest["Company"];
//    List<SMAM_CompanyDocs_ST> objdoc = JsonConvert.DeserializeObject<List<SMAM_CompanyDocs_ST>>(httprequest["Documents"].ToString());
//    var result = JsonConvert.DeserializeObject<SMAM_CompanyMst_ST>(obj);
//    //var lstoc = JsonConvert.DeserializeObject<SMAM_CompanyDocs_ST>(objdoc);
//    var data = _docSer.SubmitDocuments();
//    return Ok(data);

//}

//[ActionName("SubmitCharges")]
//[HttpPost]
//public IHttpActionResult SubmitCharges(SMAM_CompanyCharges_ST obj)
//{
//    var data = _chargesSer.SubmitCharges(obj);
//    return Ok(data);

//}

public class Attachments
{
    public object file { get; set; }
    public int CompanyId { get; set; }
}


