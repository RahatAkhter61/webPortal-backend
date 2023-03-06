using ContextMapper;
using CORE_API.Areas.HRPR.Models;
using CORE_API.General;
using CORE_API.Services.SRCC;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.HRMS
{
    public class DeliveryService : CastService<SMIM_AssignRoles_ST>
    {


        private readonly CardActivationService _cardActivation;
        public DeliveryService()
        {
            _cardActivation = new CardActivationService();
        }
        protected override bool IsValidBeforeSave()
        {

            this.Errors.Clear();


            if (this.Errors.Count > 0)
                return true;
            else
                return false;


        }


        public DTO GetEmployeeDetailsForDelivery(string WalletId, int ExecId)
        {
            var objUSPExport = new BLInfo<HRMS_USPExport_HI>().GetQuerable<HRMS_USPExport_HI>().Where(a => a.WalletId == WalletId && a.CardDeliveredOn == null).OrderByDescending(o => o.ExportId).FirstOrDefault();
            if (objUSPExport != null)
            {
                string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();
                string[] paramater = { "WalletId", "ExecId" };
                DataTable dt = ExecuteStoreProc("SP_GetEmployeeDetailsForDelivery", paramater, Connection, WalletId, ExecId);
                return this.SuccessResponse(dt);
            }
            else
                return this.BadResponse("KYC and card delivery already completed for the selected customer.");
        }

        public DTO updateActivateStatus(object obj)
        {

            try
            {

                if (obj == null) { return this.SuccessResponse("Update Successfully"); }

                // HTTPCLIENT CONFIGURATION
                string BaseURL = System.Configuration.ConfigurationManager.AppSettings["CRDSTUS"];
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string UrlParameters = $"card/updateActivateStatus";
                var jsonRequest = new
                {
                    Records = obj,
                    CardStatus = "DELIVERED",
                };
                // REQUEST BODY
                var Requestbody = jsonRequest;
                DateTime RequestDate = DateTime.Now;

                // RESPONSE 
                var objresponse = client.PostAsJsonAsync(UrlParameters, Requestbody).Result;
                var dataObjects = objresponse.Content.ReadAsStringAsync().Result;
                DateTime ResponseDate = DateTime.Now;

                // LOG SUBMIT FOR TRACKING
                _cardActivation.SubmitLog(19, JsonConvert.SerializeObject(Requestbody) , dataObjects, RequestDate, ResponseDate);
                HSMResponse HSMResp = Newtonsoft.Json.JsonConvert.DeserializeObject<HSMResponse>(dataObjects);
                Logger.TraceService($"HSM Response : {HSMResp.error}  , {HSMResp.data}  ", "updateActivateStatus");

                if (!HSMResp.error)
                {
                    return this.SuccessResponse(HSMResp.data.message);
                }
                else
                    return this.BadResponse(HSMResp.data.message);

            }

            catch (Exception ex)
            {
                Logger.TraceService($"HSM ERROR : {ex.Message}  ", "updateActivateStatus");
                _cardActivation.SubmitLog(19, null, ex.Message, DateTime.Now, DateTime.Now);
                return this.BadResponse(ex.Message);
            }
        }
    }
}

