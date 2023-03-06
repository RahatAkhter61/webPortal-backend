using ContextMapper;
using CORE_API.General;
using CORE_API.Models.ComapnyWhiteListing;
using CORE_API.Models.Transaction;
using CORE_API.Services.SMAM;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.HRPR
{
    public class CompanyDepositService : CastService<HRPR_DepositSlip_TR>
    {
        private readonly AccountTransactionService _accountTranSer;
        private readonly CompanyService _companyService;

        dbEngine db = new dbEngine();
        public CompanyDepositService()
        {
            _accountTranSer = new AccountTransactionService();
            _companyService = new CompanyService();
        }

        public DTO GetCompanyDeposit(int CompanyId)
        {

            var companyDepositeList = (from t1 in db._dbContext.HRPR_DepositSlip_TRs
                                       join t2 in db._dbContext.SMAM_CompanyMst_STs on t1.CompanyId equals t2.CompanyId
                                       join t3 in db._dbContext.Status on t1.StatusId equals t3.StatusId
                                       where t1.CompanyId == CompanyId

                                       select new
                                       {
                                           t1.SlipId,
                                           t1.SlipNo,
                                           t1.CompanyId,
                                           t2.Name,
                                           t1.Amount,
                                           t1.Date,
                                           t1.ImgPath,
                                           Status = t3.Name,
                                           DepositDate = t1.Date.Value.ToString("dd/MM/yyyy")
                                       });
            return SuccessResponse(companyDepositeList);

        }

        public DTO GetAllDepositSlip(DateTime Fromdate, DateTime Todate)
        {
            try
            {
                var depositSlip = (from t1 in db._dbContext.HRPR_DepositSlip_TRs
                                   join t2 in db._dbContext.SMAM_CompanyMst_STs on t1.CompanyId equals t2.CompanyId
                                   join t3 in db._dbContext.Status on t1.StatusId equals t3.StatusId
                                   where t1.SlipId != 0 && t1.UploadedOn >= Fromdate && t1.UploadedOn <= Convert.ToDateTime(Todate).AddDays(1)
                                   select new

                                   {
                                       t1.SlipId,
                                       t1.SlipNo,
                                       t1.CompanyId,
                                       Company = t2.Name,
                                       t1.Amount,
                                       t1.Date,
                                       t1.ImgPath,
                                       t1.Description,
                                       DepositDate = t1.Date,
                                       Status = t3.Name,
                                       SalaryAmount = t1.SalaryAmount != null ? t1.SalaryAmount : 0,
                                       ChargeAmount = t1.ChargeAmount != null ? t1.ChargeAmount : 0,
                                   }).ToList();




                return SuccessResponse(depositSlip);
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }

        public DTO SubmitDeposit(HRPR_DepositSlip_TR obj, string filePath)
        {
            try
            {


                if (obj.SlipId != null)
                    GetByPrimaryKey(obj.SlipId);

                if (obj.SlipId == 0 || obj.SlipId == null)
                    PrimaryKeyValue = null;

                if (PrimaryKeyValue == null)
                    New();


                Current.CompanyId = obj.CompanyId;
                Current.SlipNo = obj.SlipNo;
                Current.Amount = obj.Amount;

                Current.ImgPath = filePath != "" ? filePath : obj.ImgPath;
                if (filePath != "")
                {
                    Current.UploadedBy = obj.UploadedBy;
                    Current.UploadedOn = DateTime.Now;
                }
                DateTime? Date = obj.Date;
                Common.DateHandler(ref Date);
                Current.Date = Date;

                Save();
                _accountTranSer.SubmitAccountTransanction(Current);


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

        public DTO DeleteDeposit(long Id)
        {
            try
            {
                GetByPrimaryKey(Id);
                if (Current != null)
                    PrimaryKeyValue = Id;

                Delete();
                _accountTranSer.UpdateAccountTran(Id);

                return this.SuccessResponse("Deleted Successfully.");


            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }

        public DTO AddCompanyDeposit(HRPR_DepositSlip_TR model, HttpRequest request)
        {
            try
            {

                if (!ValidateDeposite(model) && request.Files.Count != 0 && request.Files.Count > 1)
                    return BadResponse(new DTO { isSuccessful = false, data = null, errors = null });

                var companyDetail = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().FirstOrDefault(x => x.CompanyId == model.CompanyId);
                if (companyDetail == null)
                    return BadResponse(new DTO { isSuccessful = false, data = null, errors = "Company Doesn't Exist" });

                #region SaveFileRegion
                //string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/00 - Portal/");
                string basefilepath = ConfigurationManager.AppSettings["DocPath"].ToString();


                string TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss");
                string FileName = "";
                var postedfile = request.Files[0];
                FileName = "Deposit Slips_" + companyDetail.CustomerId + "_" + TimeSpan + Path.GetExtension(postedfile.FileName);
                //postedfile.FileName.ToString();


                var filePath = "00 - Portal";

                if (filePath != null)
                {
                    if (!Directory.Exists(basefilepath + filePath))
                    {
                        Directory.CreateDirectory(basefilepath + filePath);
                    }
                }


                filePath = filePath + @"\" + companyDetail.CustomerId;

                if (filePath != null)
                {
                    if (!Directory.Exists(basefilepath + filePath))
                    {
                        Directory.CreateDirectory(basefilepath + filePath);
                    }
                }

                filePath = filePath + @"\Deposit Slips";
                if (filePath != null)
                {
                    if (!Directory.Exists(basefilepath + filePath))
                    {
                        Directory.CreateDirectory(basefilepath + filePath);
                    }
                }

                filePath = filePath + @"\" + FileName;
                postedfile.SaveAs(basefilepath + filePath);
                #endregion

                #region Save Comapny Deposit

                New();
                Current.CompanyId = model.CompanyId;
                Current.Amount = model.Amount;
                Current.ChargeAmount = model.ChargeAmount > 0 ? model.ChargeAmount : 0M;
                Current.SalaryAmount = model.SalaryAmount != null || model.SalaryAmount > 0 ? model.SalaryAmount : 0M;
                Current.SlipNo = model.SlipNo;
                Current.ImgPath = filePath;
                Current.StatusId = model.StatusId;
                Current.UploadedBy = model.UploadedBy;
                Current.UploadedOn = DateTime.Now;
                Current.Description = model.Description;
                Current.Date = model.Date;
                Save();

                #endregion

                #region Make Transaction
                var depositSlipTransaction = new DepositSlipTrsansactionModel
                {
                    CompanyId = model.CompanyId,
                    TransactionDescription = Current.Description,
                    SlipId = Current.SlipId,
                    CreditAmount = model.Amount,
                    TransactionReference = Current.SlipNo
                };
                var transactionResponse = _accountTranSer.AddDepositSlipTransaction(depositSlipTransaction, Enums.TransactionTypes.UploadDepositSlip);
                #endregion

                #region Update Company Balance
                var updateCompanyBalance = _companyService.UpdateCompanyBalance(model.Amount, model.CompanyId, model.UploadedBy);

                #endregion 

                return SuccessResponse(new DTO { isSuccessful = true, data = null, errors = null });

            }
            catch (Exception ex)
            {
                return SuccessResponse(new DTO
                {
                    isSuccessful = false,
                    data = null,
                    errors = ex.Message

                });
            }
        }

        private bool ValidateDeposite(HRPR_DepositSlip_TR model)
        {
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.ImgPath) && model.CompanyId > 0 && !string.IsNullOrEmpty(model.SlipNo) &&
                    model.Amount > 0 && model.SalaryAmount > 0 && (model.Amount == (model.ChargeAmount + model.SalaryAmount))
                    )
                    return true;
                return false;

            }
            return false;
        }

        public DTO GetCommonStatus()
        {

            var commonStatus = Common.GetStatusType(Enums.StatusTypes.CommonStatus);
            var statusReocrd = Common.GetStatusByTypeId(commonStatus.StatustypeId).Select(o => new
            {
                o.Name,
                o.TypeId,
                o.StatusId
            }).ToList();

            if (statusReocrd.Count == 0)
                return BadResponse("Status Not Available...!");

            return SuccessResponse(statusReocrd);

        }

        public async Task<DTO> UpdateDepositSlipStatus(IList<CompanyDeposite> model)
        {

            var status = string.Empty;

            var getCmmonStatus = (from t1 in db._dbContext.StatusTypes
                                  join t2 in db._dbContext.Status on t1.StatustypeId equals t2.TypeId
                                  where t1.Name.Equals(Enums.StatusTypes.CommonStatus) && t2.Name.Equals(Enums.StatusTypes.Approved)

                                  select new
                                  {
                                      t2.StatusId,
                                      t2.Name

                                  }).FirstOrDefault();

            if (model.Count == 0)
                return BadResponse("No Record Found");

            List<ApprovalRequestModel> list = new List<ApprovalRequestModel>();

            foreach (var item in model)
            {

                var depositeSlip = db._dbContext.HRPR_DepositSlip_TRs.Where(x => x.SlipId == item.SlipId).FirstOrDefault();
                depositeSlip.StatusId = item.Status;

                #region Update Company Balance
                if (item.Status == getCmmonStatus.StatusId)
                {
                    _companyService.UpdateCompanyBalance(item.DepositeAmount, item.CompanyId, item.UploadedBy);
                    status = Enums.ThirdPartyStatus.ApprovedByKamelPay;
                }
                else
                    status = Enums.ThirdPartyStatus.RejectByKamelPay;
                #endregion
               // list.Add(new ApprovalRequestModel { approvalId = item.SlipId.ToString(), status = status, type = Enums.ThirdPartyStatus.DepositFileUpload });

            }

            //var requestBody = new { data = list };
            //#region Post Request To Middel Ware 
            //string nodeUrl = ConfigurationManager.AppSettings["Nodeurl"].ToString();
            //int timeoutInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["timeoutInSeconds"].ToString());
            //var uri = new Uri(nodeUrl);
            //var response = await Common.PostAsync<BaseModel>(uri, "api/approvals/updateStatus", requestBody, timeoutInSeconds);
            //#endregion

            db._dbContext.SubmitChanges();
            return SuccessResponse("Successfully Update");

        }
    }
}
