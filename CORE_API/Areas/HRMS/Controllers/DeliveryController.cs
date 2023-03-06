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
using CORE_API.Services.SRCC;

namespace CORE_API.Areas.HRMS.Controllers
{

    public class DeliveryController : BaseController
    {

        private readonly DeliveryService _delSer;
        private readonly USPExportService _uspExportSer;
        private readonly EmployeeService _empSer;
        private readonly OTPService _otpSer;
        private readonly CardActivationService _otpCardSer;
        public DeliveryController()
        {
            _delSer = new DeliveryService();
            _uspExportSer = new USPExportService();
            _empSer = new EmployeeService();
            _otpSer = new OTPService();
            _otpCardSer = new CardActivationService();
        }
        [HttpPost]
        [ActionName("SaveImage")]
        public IHttpActionResult SaveImage()
        {

            string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/");
            var httprequest = HttpContext.Current.Request;
            string CustomerId = httprequest["CustomerId"].ToString();
            var CombinedFile = httprequest.Files["CombinedImage"];
            var frontpostedfile = httprequest.Files["Front"];
            var backpostedfile = httprequest.Files["Back"];
            var fullpostedfile = httprequest.Files["Full"];
            var signaturepostedfile = httprequest.Files["Signature"];
            CombinedFile.SaveAs(basefilepath + CustomerId + "_" + "EmiratesId" + ".pdf");
            frontpostedfile.SaveAs(basefilepath + CustomerId + "_" + "EmiratesIdFrontImage" + ".jpg");
            backpostedfile.SaveAs(basefilepath + CustomerId + "_" + "EmiratesIdBackImage" + ".jpg");
            fullpostedfile.SaveAs(basefilepath + CustomerId + "_" + "EmiratesIdPhoto" + ".jpg");
            signaturepostedfile.SaveAs(basefilepath + CustomerId + "_" + "Signature" + ".jpg");
            Logger.TraceService("EmiratesId Images Saved", "DeliveryLog");
            return Ok("Submit");
        }

       
        [HttpGet]
        [ActionName("UpdateUSPExport")]
        public IHttpActionResult UpdateUSPExport(int ExportId)
        {
            var data = _uspExportSer.UpdateUSPExport(ExportId);
            return Ok(data);
        }
        [HttpGet]
        [ActionName("UpdateMobileNo")]
        public IHttpActionResult UpdateMobileNo(string MobileNo, int EmpId, int ExportId, string WalletId, string EmiratesId)
        {
           
            var verdata = _empSer.UpdateMobileNoValidation(EmpId, WalletId, MobileNo, EmiratesId);
            Logger.TraceService("verdata" + verdata, "verdata");

            if (verdata.errors == null)
            {

                var data = _empSer.UpdateMobileNo(MobileNo, EmpId);
                var objdata = _uspExportSer.UpdateMobileNo(MobileNo, ExportId);
                return Ok(objdata);
            }
            else
            {
                Logger.TraceService("UpdateMobileNo "+ verdata.errors
                    , "errors");
                return Ok(verdata);
            }
        }
        [HttpGet]
        [ActionName("UpdateMobileNoValidation")]
        public IHttpActionResult UpdateMobileNoValidation(int EmpId, string WalletId, string MobileNo, string EmiratesId)
        {
            try
            {

                var data = _empSer.UpdateMobileNoValidation(EmpId, WalletId, MobileNo, EmiratesId);
                
                if (data == null)
                {
                    return InternalServerError();
                }

                else
                {

                    return Ok(data);
                }


            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [ActionName("GetEmployeeDetailsForDelivery")]
        public IHttpActionResult GetEmployeeDetailsForDelivery(string WalletId, int ExecId)
        {
            var data = _delSer.GetEmployeeDetailsForDelivery(WalletId, ExecId);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("SendOTP")]
        public IHttpActionResult SendOTP(string MobileNo)

        {
            var data = _otpSer.SendOTP("23244", MobileNo);
            return Ok(data);
        }
        [HttpGet]
        [ActionName("SubmitOTP")]
        public IHttpActionResult SubmitOTP(int EmpId, int UserId, int Isresend, string MobileNo, string ProgramName)
        {
            var data = _otpSer.SubmitOTP(EmpId, UserId, Isresend, MobileNo, ProgramName);
            return Ok(data);
        }
        [HttpGet]
        [ActionName("VerifyOTP")]
        public IHttpActionResult VerifyOTP(int EmpId, string OTP)
        {
            var data = _otpSer.VerifyOTP(EmpId, OTP);
            return Ok(data);
        }
        //[HttpGet]
        //[ActionName("ActiveCard")]
        //public IHttpActionResult ActiveCard(string Pin, int ExportId)
        //{
        //    var data = _otpCardSer.ActiveCard(Pin, ExportId);
        //    return Ok(data);
        //}

        [HttpGet]
        [ActionName("CheckMobNumber")]
        public IHttpActionResult CheckMobNumber(string MobileNo)
        {
            return Ok(MobileNo);
        }

        [HttpPost]
        [ActionName("ActiveCard")]
        public IHttpActionResult ActiveCard()
        {
            var httprequest = HttpContext.Current.Request;
            string Pin = httprequest["Pin"].ToString();
            int ExportId = Convert.ToInt32(httprequest["ExportId"]);
            string CardNo = httprequest["CardNo"].ToString();
            string PlainCardNo = httprequest["PlainCardNo"].ToString();
            string MobileNo = httprequest["MobileNo"].ToString();
            string IsAgreeTerms = httprequest["IsAgreeTerms"].ToString();
            string IsAutoDelivery = httprequest["IsAutoDelivery"].ToString();
            int AccountId = Convert.ToInt32(httprequest["AccountId"]);
            _uspExportSer.UpdateTermsCondition(ExportId, IsAgreeTerms, IsAutoDelivery);
            //if () 
            var data = _otpCardSer.ActiveCard(Pin, ExportId, CardNo, PlainCardNo, MobileNo, IsAutoDelivery, AccountId);
            return Ok(data);
        }
        [HttpGet]
        [ActionName("RSA")]
        public IHttpActionResult RSA(string Pin, string Pan)

        {
            var data = _otpCardSer.GenerateRSA(Pin, Pan);
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetToken")]
        public IHttpActionResult GetToken()
        {
            var data = _otpCardSer.GetToken(true);
            return Ok(data);
        }


    }


}
