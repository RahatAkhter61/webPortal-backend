using CORE_API.General;
using CORE_API.Models;
using CORE_API.Models.CardActivation;
using CORE_API.Services.HRMS;
using CORE_API.Services.SMAM;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace CORE_API.Services.SRCC
{
    public class CardActivationService
    {
        dbEngine dbEngine = new dbEngine();
        private readonly USPExportService _uspExportSer;
        private readonly OTPService _OTPSer;
        public CardActivationService()
        {
            _uspExportSer = new USPExportService();
            _OTPSer = new OTPService();

        }
        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        WebRequestHandler GetHandler()
        {
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
            //       | SecurityProtocolType.Tls11
            //       | SecurityProtocolType.Tls12
            //       | SecurityProtocolType.Ssl3;


            Logger.TraceService($"Certificate Adding", "ActivationLog");
            WebRequestHandler handler = new WebRequestHandler();
            X509Certificate certificate = X509Certificate.CreateFromCertFile(ConfigurationManager.AppSettings["CertPath"].ToString()); //kamel pay
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ClientCertificates.Add(certificate);
            handler.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            Logger.TraceService($"Finish Certificate Adding", "ActivationLog");
            return handler;

        }

        public Models.CardActivation.Token GetToken(bool addCertificate)
        {
            try
            {
                string ClientId = "2du79n6avwfz9hwzdk87xzu2";
                string ClientSecret = "7fyqCmKa3a";
                string iUrl = "https://apimash-uat.network.ae/KMPY/PCServices/Token";
                _WebClientBase _client = new _WebClientBase();
                if (addCertificate)
                {
                    _client.client = new HttpClient(GetHandler());
                    ClientId = "3eyh42dbuz3jge4dvykkkdhc";
                    ClientSecret = "DdbS4Xvr6k";
                    iUrl = "https://apimash-prd.network.ae/KMPY/PCServices/Token";


                }


                var pairs = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>( "grant_type", "client_credentials" ),
                            new KeyValuePair<string, string>( "client_id", ClientId ),
                            new KeyValuePair<string, string> ( "client_secret", ClientSecret)
                        };
                var content = new FormUrlEncodedContent(pairs);
                _client.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Logger.TraceService($"Token Request : {content}", "ActivationLog");
                Logger.TraceService($"Token URL Created : {iUrl}", "ActivationLog");
                var tokenResponse = _client.client.PostAsync(iUrl, content).Result;
                Logger.TraceService($"Token Response : {tokenResponse}", "ActivationLog");
                string token = tokenResponse.Content.ReadAsStringAsync().Result;
                var t = JsonConvert.DeserializeObject<Models.CardActivation.Token>(token);
                return t;
            }
            catch (Exception Ex)
            {
                Logger.TraceService($"Token Exception : {Ex.InnerException.InnerException.Message}", "ActivationLog");
                throw;
            }

        }

        HttpResponseMessage GetResponse(Models.CardActivation.Token t, string iURL, bool addCertificate, string content)
        {
            _WebClientBase _Webclient = new _WebClientBase();
            string BaseUrl = _Webclient._BaseURL;
            if (addCertificate)
            {
                BaseUrl = "https://apimash-prd.network.ae/KMPY/PCServices/";
            }
            _Webclient.client.BaseAddress = new Uri(BaseUrl);
            var myContent = content;//
            if (addCertificate)
            {
                _Webclient.client = new HttpClient(GetHandler());
                _Webclient.client.BaseAddress = new Uri(BaseUrl);
            }

            _Webclient.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _Webclient.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);

            var check = JsonConvert.DeserializeObject(myContent);
            var response = _Webclient.client.PostAsJsonAsync(iURL, JsonConvert.DeserializeObject(myContent)).Result;
            return response;
        }

        private string CardActivation(SRCC_CardActivation_TR _pendingRequestForActivation, string msgId)
        {

            try
            {
                PostObject postObject = new PostObject
                {
                    NISrvRequest = new NISrvRequest
                    {
                        request_card_activation = new request_card_activation
                        {
                            header = new CardActivationHeader
                            {

                                tracking_id = null,
                                bank_id = "KMPY",
                                msg_function = "REQ_CARD_ACTIVATION_SET_PIN",
                                msg_id = msgId,
                                msg_type = "TRANSACTION",
                                src_application = "CDM",
                                target_application = "PCMS",
                                timestamp = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss:SSS'Z'")
                            },

                            body = new CardActivationBody
                            {
                                card_no = _pendingRequestForActivation.CardNo,
                                card_type = _pendingRequestForActivation.CardType,
                                pin = _pendingRequestForActivation.Pin,
                                request_type = _pendingRequestForActivation.RequestType,
                                FIID = _pendingRequestForActivation.FIID
                            }
                        }
                    }
                };

                var myContent = JsonConvert.SerializeObject(postObject, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                return myContent;
            }
            catch (Exception e)
            {

                return null;
            }
        }
        public SRCC_CardActivation_TR SubmitCardActivation(string PinCode, string PlainCardNo, int USPExportId, int AccountId)
        {
            try
            {
                SRCC_CardActivation_TR objCurrent;
                //objCurrent = obj.ActivationId != 0 ? dbEngine._dbEngineContext.SRCC_CardActivation_TRs.Where(a => a.ActivationId == obj.ActivationId).FirstOrDefault() : new SRCC_CardActivation_TR();
                var uspExport = dbEngine._dbContext.HRMS_USPExport_HIs.Where(o => o.ExportId == USPExportId).FirstOrDefault();
                
                objCurrent = new SRCC_CardActivation_TR();

                //objCurrent.CardNo = PlainCardNo;
                objCurrent.CardId = uspExport != null ? uspExport.CardId : null;
                objCurrent.Pin = PinCode;
                //objCurrent.RequestedOn = DateTime.Now;
                objCurrent.ActionId = USPExportId;///Export Id 
                objCurrent.RequestType = "A";
                objCurrent.CardType = "PREPAID";
                objCurrent.FIID = "PAJK";
                objCurrent.AccountId = AccountId;
                //objCurrent.IsRequested = null;

                if (objCurrent.ActivationId == 0)
                    dbEngine._dbEngineContext.SRCC_CardActivation_TRs.InsertOnSubmit(objCurrent);
                if (uspExport != null)
                {
                    uspExport.CardDeliveredOn = DateTime.Now;
                    uspExport.CardDeliveredStatus = 1;
                }
                dbEngine._dbEngineContext.SubmitChanges();
                return objCurrent;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public bool SubmitLog(int APIId, string request, string response, DateTime requeston, DateTime responseon)
        {
            try
            {
                SRCC_APILogs_HI objCurrent;
                objCurrent = new SRCC_APILogs_HI();

                objCurrent.APIId = APIId;
                objCurrent.RequestOn = requeston;
                objCurrent.Request = request;
                objCurrent.Response = response;
                objCurrent.ResponseOn = responseon;

                if (objCurrent.APILogId == 0)
                    dbEngine._dbEngineContext.SRCC_APILogs_HIs.InsertOnSubmit(objCurrent);


                dbEngine._dbEngineContext.SubmitChanges();
                return true;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        static string GetKey()
        {
            string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/public.pem");
            return File.ReadAllText(basefilepath).Replace("-----BEGIN RSA PUBLIC KEY-----", "").Replace("-----END RSA PUBLIC KEY-----", "");
            //.Replace("\n", "");
        }

        //var encryptedpinValue = this.Encrypt(encoder.GetBytes(pin));
        //var encryptedpanValue = this.Encrypt(encoder.GetBytes(pan));
        public HSMResponse GenerateRSA(string pin, string pan)
        {

            try
            {
                Logger.TraceService($"Request for generate RSA Pin is {pin} and Pan : {pan}", "DeliveryLogInfo");

                //Logger.TraceService($"After Encryping RSA the Pin is {encryptedpinValue} and Pan : {encryptedpanValue}", "DeliveryLogInfo");
                string pinkey = EncryptPKCS1(pin);  //System.Convert.ToBase64String(encryptedpinValue);
                string pankey = EncryptPKCS1(pan); //System.Convert.ToBase64String(encryptedpanValue);
                Logger.TraceService($"After Converting Encryped RSA into Base64String the Pin is {pinkey} and Pan : {pankey}", "DeliveryLogInfo");
                if (pinkey != "" && pankey != "")
                {
                    string BaseURL = System.Configuration.ConfigurationManager.AppSettings["HSMURL"];
                    string UrlParameters = $"hsm/generatePin";

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(BaseURL);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HSMRequest objhsm = new HSMRequest()
                    {
                        pin = pinkey,
                        pan = pankey
                    };
                    var payload = JsonConvert.SerializeObject(objhsm);
                    DateTime RequestDate = DateTime.Now;
                    var objresponse = client.PostAsJsonAsync(UrlParameters, JsonConvert.DeserializeObject(payload)).Result;
                    var dataObjects = objresponse.Content.ReadAsStringAsync().Result;
                    DateTime ResponseDate = DateTime.Now;
                    SubmitLog(15, payload, dataObjects, RequestDate, ResponseDate);
                    HSMResponse obj = Newtonsoft.Json.JsonConvert.DeserializeObject<HSMResponse>(dataObjects);
                    Logger.TraceService($"HSM Response : {obj.error}  , {obj.data}  ", "DeliveryLogInfo");
                    return obj;

                }

                return null;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }


        }
        public string EncryptPKCS1(string plainText)
        {

            string pemFilePath = HttpContext.Current.Server.MapPath("~/SFTP/public.pem");
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PemReader pr = new PemReader(
                (StreamReader)File.OpenText(pemFilePath)
            );
            RsaKeyParameters keys = (RsaKeyParameters)pr.ReadObject();

            // Pure mathematical RSA implementation
            // RsaEngine eng = new RsaEngine();

            // PKCS1 v1.5 paddings
            Pkcs1Encoding eng = new Pkcs1Encoding(new RsaEngine());

            // PKCS1 OAEP paddings
            //OaepEncoding eng = new OaepEncoding(new RsaEngine());
            eng.Init(true, keys);

            int length = plainTextBytes.Length;
            int blockSize = eng.GetInputBlockSize();
            List<byte> cipherTextBytes = new List<byte>();
            for (int chunkPosition = 0;
                chunkPosition < length;
                chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, length - chunkPosition);
                cipherTextBytes.AddRange(eng.ProcessBlock(
                    plainTextBytes, chunkPosition, chunkSize
                ));
            }
            return Convert.ToBase64String(cipherTextBytes.ToArray());
        }

        public DTO ActiveCard(string PinCode, int ExportId, string CardNo, string PlainCardNo, string MobileNo, string IsAutoDelivery, int AccountId)
        {
            try
            {
               
                var objhsmresponse = GenerateRSA(PinCode, CardNo);
                if (objhsmresponse != null && objhsmresponse.error == false && objhsmresponse.data.data != "")
                {

                    Logger.TraceService($"Initiate card activation request : Pin {objhsmresponse.data.data} ", "DeliveryLogInfo");
                    SRCC_CardActivation_TR obj = SubmitCardActivation(objhsmresponse.data.data, PlainCardNo, ExportId, AccountId);
                    if (obj != null)
                    {
                        Logger.TraceService($"Request initiated for card activation " + obj.ActivationId, "HSMRequestError");
                        _uspExportSer.UpdateUSPStatus(ExportId);
                        return new DTO { isSuccessful = true, data = "Request has been sent for card activation, you will recieve a confirmation message shortly." };
                    }
                    else
                    {
                        Logger.TraceService($"Request failed for card activation " + ExportId, "HSMRequestError");
                        return new DTO { isSuccessful = false, errors = "Request has failed for card activation, please try again. Thank you" };
                    }


                }
                Logger.TraceService($"HSM Request Error " + objhsmresponse.data.message, "HSMRequestError");
                return new DTO { isSuccessful = false, errors = objhsmresponse.data.message };
                //}
                //else
                //    return new DTO { isSuccessful = true, data = "Card has been successfully delivered." };
            }
            catch (Exception Ex)
            {
                Logger.TraceService($"Card Activation Exception Error " + Ex.Message, "CardActivationExceptionError");
                return new DTO { isSuccessful = false, errors = Ex.Message.ToString() };
            }


        }



    }




}


public class HSMRequest
{

    public dynamic pin { get; set; }
    public dynamic pan { get; set; }
}


public class HSMResponse
{

    public bool error { get; set; }
    public HSMResponedata data { get; set; }
}

public class HSMResponedata
{
    public string message { get; set; }
    public string data { get; set; }
}