using ContextMapper;
using CORE_API.General;
using CORE_API.Models;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.SMAM
{
    public class OTPService : CastService<SMAM_OTP_HI>
    {

        public OTPService()
        {

        }

        public DTO OTP(SMAM_OTP_HI objCurrent, int EmpId, int UserId, string MobileNo, string ProgramName)
        {
            try
            {
                if (objCurrent != null)
                {
                    if (objCurrent.OTPId != 0 && objCurrent.OTPId != null)
                        GetByPrimaryKey(objCurrent.OTPId);
                }
                else
                {
                    PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                        New();
                }
                bool isLocal = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLocal"]);
                Current.EmpId = EmpId;
                Random rnd = new Random();
                string rndotp = !isLocal ? rnd.Next(10000, 99999).ToString() : "66666";
                Current.OTP =rndotp;
                Current.OTPSendTo = MobileNo;
                Current.OTPPurpose = "For Card Activation";
                Current.OTPSentOn = DateTime.Now;
                Current.OTPExpiredOn = DateTime.Now.AddMinutes(1);
                Current.ResendTries = 0;
                Current.GeneratedBy = UserId;

                Save();
                //SendOTP(rndotp,MobileNo);
              //  string Message = $"Dear Customer, Your OTP is {rndotp}. Use this Passcode to complete your card activation. Thank you";
              
                SendMessage(MobileNo, rndotp,ProgramName);
                return this.SuccessResponse("OTP send successfully..!");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DTO VerifyOTP(int EmpId, string OTP)
        {
            try
            {
                var objCurrent = new BLInfo<SMAM_OTP_HI>().GetQuerable<SMAM_OTP_HI>().Where(a => a.EmpId == EmpId && a.OPTVerifiedOn == null).OrderByDescending(a => a.EmpId).FirstOrDefault();
                if (objCurrent != null)
                {
                    if (objCurrent.OTPId != 0 && objCurrent.OTPId != null)
                        GetByPrimaryKey(objCurrent.OTPId);
                }

                if (objCurrent != null)
                {
                    DateTime? SentOn = objCurrent.OTPSentOn;
                    DateTime? Expireddate = objCurrent.OTPExpiredOn;
                    DateTime CurrentDt = DateTime.Now;

                    if (CurrentDt >= SentOn && CurrentDt <= Expireddate)
                    {
                        var SavedOTP = objCurrent.OTP;
                        if (SavedOTP == OTP)
                        {
                            Current.OPTVerifiedOn = DateTime.Now;
                            Save();
                            return this.SuccessResponse("Verified successfully..!");
                        }
                        else
                            return this.BadResponse("Invalid OTP");
                    }

                }
                return this.SuccessResponse("");
            }
            catch (Exception)
            {

                throw;
            }
        }
        private bool UpdateResendTries(SMAM_OTP_HI obj)
        {
            try
            {
                if (obj != null)
                {
                    if (obj.OTPId != 0 && obj.OTPId != null)
                        GetByPrimaryKey(obj.OTPId);
                }

                Current.ResendTries = ++obj.ResendTries;
                Save();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DTO SubmitOTP(int EmpId, int UserId, int Isresend, string MobileNo, string ProgramName)
        {
            try
            {
                var objCurrent = new BLInfo<SMAM_OTP_HI>().GetQuerable<SMAM_OTP_HI>().Where(a => a.EmpId == EmpId && a.OPTVerifiedOn == null).OrderByDescending(a => a.EmpId).FirstOrDefault();


                if (Isresend == 1)
                {
                    if (objCurrent != null)
                    {
                        DateTime? SentOn = objCurrent.OTPSentOn;
                        DateTime? Expireddate = objCurrent.OTPExpiredOn;
                        DateTime CurrentDt = DateTime.Now;

                        if (CurrentDt >= SentOn && CurrentDt <= Expireddate)
                        {
                            UpdateResendTries(objCurrent);
                            return this.SuccessResponse("OTP resend successfully..!");
                        }
                        else
                            OTP(objCurrent, EmpId, UserId, MobileNo,ProgramName);
                    }
                }
                else
                {
                    OTP(objCurrent, EmpId, UserId, MobileNo, ProgramName);
                }
                //   SendOTP(rndotp, Current.OTPSendTo);
                return this.SuccessResponse("Saved Succesfully");
            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return BadResponse(Errors);
                else
                    return BadResponse(Ex.Message);
            }


        }
        public void SendMessage(string MobileNo, string otp, string ProgramName)
        {
            try
            {
                bool isLocal = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLocal"]);
                if (!isLocal)
                {
                    string Message = "";
                    string smsobj = new BLInfo<SMAL_Alerts_ST>().GetQuerable<SMAL_Alerts_ST>().Where(sh => sh.AlertId == 1).FirstOrDefault().SMSBody;
                    if (!String.IsNullOrEmpty(smsobj))
                        Message = smsobj.Replace("{OTP}", otp).Replace("{ProgramName}", ProgramName);
                    SMSApiService.IsmsapiClient smsClient = new SMSApiService.IsmsapiClient();
                    string smsAPIKey = "2cd74f8d-60ff-4341-a46c-a3f2297cf891202146"; // smsClient.GetApiKey("", "");
                    string smsResponsesString = smsClient.SendTextSMS(smsAPIKey, Message, 0, "KAMELPAY", MobileNo, 0);
                    Logger.TraceService("SMS Respone" + smsResponsesString, "SMSResponse");
                }
                else
                    Logger.TraceService($"isLocal Environment : {isLocal} ", "SMSResponse");

            }
            catch (Exception Ex)
            {
                Logger.TraceService($"Error: SMS API Response : {Ex.Message} ", "SMSResponseException");
            }

        }
        public DTO SendOTP(string OTP, string MobileNo)
        {
            try
            {

                bool isLocal = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLocal"]);
                if (!isLocal)
                {
                    string Message = $"Dear Customer, Your OTP is {OTP}. Use this Passcode to complete your card activation. Thank you";
                    string BaseURL = System.Configuration.ConfigurationManager.AppSettings["SMSAPI"];
                    string UrlParameters = $"SendSMS?username=Edssample&apiId=yomOzOmR&json=True&destination={MobileNo}&source=AD-OGLE&text={Message}edsoptout.ae";

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(BaseURL);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    SMSResponse obj = new SMSResponse();
                    var objresponse = client.GetAsync(UrlParameters).Result;
                    var dataObjects = objresponse.Content.ReadAsStringAsync().Result;
                    SMSResponse smsobj = Newtonsoft.Json.JsonConvert.DeserializeObject<SMSResponse>(dataObjects);
                    return this.SuccessResponse(smsobj.Description);
                }
                else
                {
                    Logger.TraceService($"isLocal Environment : {isLocal} ", "SMSResponse");
                    return this.BadResponse("Local Environment" + isLocal.ToString());
                }
            }
            catch (Exception Ex)
            {
                Logger.TraceService($"Exception : {Ex.Message} ", "SMSResponse");
                return this.BadResponse(Ex.Message);
            }

        }
        /// <summary>
        /// Need to Generalize Later meanwhile it's for testing purpose : Shahzad
        /// </summary>
        /// <param name="OTP"></param>
        /// <param name="MobileNo"></param>
        /// <returns></returns>
        //public DTO SendMessage(string CRN, string MobileNo)
        //{
        //    try
        //    {
        //        string Messagetosend = $"Card is successfully activated for the user CRN :  {CRN}.Thank you";
        //        string BaseURL = System.Configuration.ConfigurationManager.AppSettings["SMSAPI"];
        //        string UrlParameters = $"SendSMS?username=Edssample&apiId=yomOzOmR&json=True&destination={MobileNo}&source=AD-OGLE&text={Messagetosend}edsoptout.ae";

        //        HttpClient client = new HttpClient();
        //        client.BaseAddress = new Uri(BaseURL);
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        SMSResponse obj = new SMSResponse();
        //        var objresponse = client.GetAsync(UrlParameters).Result;
        //        var dataObjects = objresponse.Content.ReadAsStringAsync().Result;
        //        SMSResponse smsobj = Newtonsoft.Json.JsonConvert.DeserializeObject<SMSResponse>(dataObjects);
        //        return this.SuccessResponse(smsobj.Description);

        //    }
        //    catch (Exception Ex)
        //    {
        //        Logger.TraceService($"Activation Response : {Ex.Message} ", "ActivationLog");

        //        return this.SuccessResponse("");
        //    }

        //}
    }


}
