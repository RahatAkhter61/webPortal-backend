using ContextMapper;
using CORE_API.General;
using CORE_API.Models;
using CORE_API.Models.Transaction;
using CORE_API.Services.FNGL;
using CORE_API.Services.SMAM;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.HRPR
{
    public class AccountTransactionService : CastService<FNGL_Transactions_HI>
    {

        private readonly CompanyService _compServ;
        private readonly TransactionManager _transactionManager;
        private readonly AdditionalChargesService _additionalCharges;
        private readonly EscrowService _escrowService;
        private readonly ManualChargeService _manualChargeService;

        public AccountTransactionService()
        {
            _compServ = new CompanyService();
            _transactionManager = new TransactionManager();
            _additionalCharges = new AdditionalChargesService();
            _escrowService = new EscrowService();
            _manualChargeService = new ManualChargeService();
        }

        public DTO GetCompanyDeposit()
        {
            try
            {
                var objdeposit = new BLInfo<HRPR_DepositSlip_TR>().GetQuerable<HRPR_DepositSlip_TR>().Where(a => a.SlipId != 0).ToList();
                var lst = (from sa in objdeposit
                           select new
                           {
                               sa.SlipId,
                               sa.SlipNo,
                               sa.CompanyId,
                               Company = sa.CompanyId != 0 ? new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == sa.CompanyId).FirstOrDefault().Name : "",
                               sa.Amount,
                               sa.Date,
                               sa.ImgPath,
                               DepositDate = sa.Date.Value.ToString("dd/MM/yyyy")

                           }).ToList();
                return SuccessResponse(lst);
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }


        public DTO SubmitAccountTransanction(HRPR_DepositSlip_TR obj)
        {
            try
            {


                if (obj.SlipId != null)
                {
                    var objaccount = new BLInfo<FNGL_AccountTransactions_HI>().GetQuerable<FNGL_AccountTransactions_HI>().Where(a => a.SlipId == obj.SlipId).FirstOrDefault();
                    if (objaccount != null)
                    {
                        long TransId = objaccount.TransId;
                        GetByPrimaryKey(TransId);
                    }
                }
                if (obj.SlipId == 0 || obj.SlipId == null)
                    PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                    New();


                Current.CompanyId = obj.CompanyId;
                Current.EmpId = 1;
                Current.TransTypeId = 1;
                Current.DebitAmount = obj.Amount;
                Current.CreditAmount = 0;

                Current.SlipId = obj.SlipId;
                Current.TransDesc = "Upload Deposit Slip";
                Current.TransReference = "Upload Deposit Slip";

                DateTime? Date = obj.Date;
                Common.DateHandler(ref Date);
                Current.TransDate = Date;

                Save();


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

        public DTO SubmitAccountTransanctionbyExportSIFFiles(HRPR_ExportSIFFiles_HI obj, CalculateMultipleChargeBindingModel chargeModel, string IsCards = "")
        {
            try
            {



                if (obj.ExpSifID == 0 || obj.ExpSifID == null)
                    PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                    New();


                Current.CompanyId = obj.CompanyId;
                decimal? _CompanyBalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == obj.CompanyId).FirstOrDefault().NewIfEmpty().Balance;
                decimal? _TotalBalance = (_CompanyBalance.HasValue ? (_CompanyBalance.Value - obj.TotalAmount) : 0);
                Current.DebitAmount = obj.TotalAmount;
                Current.CreditAmount = 0;
                Current.SifId = obj.ExpSifID;
                Current.TransDate = DateTime.Now;
                Current.TransDesc = "Payroll Process";
                Current.TransReference = obj.FileName;
                Current.TransTypeId = 2;


                Save();

                if (IsCards == "" || IsCards == "false")
                    _compServ.UpdateCompantBalance(obj.CompanyId, _TotalBalance);


                foreach (var chargeIds in chargeModel.ChargeIds)
                {
                    var chargeObject = new ChargeTransaction
                    {
                        TotalAmount = Current.DebitAmount,
                        ChargeType = int.Parse($"{chargeIds}"),
                        TransactionType = Enums.TransactionTypes.CompanyCharge,
                        CompanyId = Current.CompanyId,
                        Sifid = Current.SifId,
                        TransactionId = Current.TransId,
                        IsDeducted = chargeModel.IsDeducted.Equals("True") ? true : false,
                        CreatedBy = obj.Createdby,
                        ProductId = chargeModel.ProductId

                    };
                    var performCharge = PerformSifCharges(chargeObject);
                }

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

        public DTO CalculateMultipleCharges(CalculateMultipleChargeBindingModel model)
        {
            try
            {


                string VAT = new BLInfo<SystemSetting>()
                            .GetQuerable<SystemSetting>()
                            .FirstOrDefault(x => x.Key.Equals(Enums.SystemSettings.VAT)).Value;

                decimal VATPercent = string.IsNullOrEmpty(VAT) ? 0 : decimal.Parse(VAT);

                List<ChargesResponseModel> response = new List<ChargesResponseModel>();
                foreach (var chrgId in model.ChargeIds)
                {

                    ChargesResponseModel result;
                    decimal? companyBalance = new BLInfo<SMAM_CompanyMst_ST>()
                                        .GetQuerable<SMAM_CompanyMst_ST>()
                                        .FirstOrDefault(x => x.CompanyId == model.CompanyId).Balance;

                    var chargetypeDetail = new BLInfo<SMAM_ChargesType_ST>()
                                       .GetQuerable<SMAM_ChargesType_ST>()
                                       .FirstOrDefault(x => x.ChrgTypeId == chrgId);
                    var companyCharges = new BLInfo<SMAM_CompanyCharges_ST>()
                                    .GetQuerable<SMAM_CompanyCharges_ST>()
                                    .FirstOrDefault(x => x.CompanyId == model.CompanyId &&
                                                            x.ChrgTypeId == chrgId &&
                                                            x.ProductId == model.ProductId);
                    decimal? chargeAmount = 0, VATAmount = 0, totalAmount = 0;

                    if (companyCharges != null)
                    {
                        if (model.ProductId == 2)
                        {

                            if (chargetypeDetail.Code.Equals(Enums.ChargesTypeCode.PerRecord))
                            {
                                chargeAmount = companyCharges != null ? companyCharges.Charges * model.TotalRecords : 0;
                                VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0)) * model.TotalRecords;
                                totalAmount = chargeAmount + VATAmount;
                            }
                            else
                            {
                                chargeAmount = companyCharges != null ? companyCharges.Charges : 0;
                                VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0));
                                totalAmount = chargeAmount + VATAmount;
                            }
                        }

                        else if (model.ProductId == 1)
                        {
                            if (chargetypeDetail.Code.Equals(Enums.ChargesTypeCode.PerRecord))
                            {
                                chargeAmount = companyCharges != null ? companyCharges.Charges * model.TotalRecords : 0;
                                VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0)) * model.TotalRecords;
                                totalAmount = chargeAmount + VATAmount;
                            }
                            else
                            {
                                chargeAmount = companyCharges != null ? companyCharges.Charges : 0;
                                VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0));
                                totalAmount = chargeAmount + VATAmount;
                            }

                        }

                        result = new ChargesResponseModel
                        {
                            VatAmount = VATAmount,
                            ChargeAmount = chargeAmount,
                            VatPercent = VATPercent,
                            totalChargeAmount = totalAmount,
                            Currentbalance = companyBalance,
                            CompanyChargeId = companyCharges != null ? companyCharges.CmpChrgId : (int?)null,
                            Title = chargetypeDetail.Title,
                            Description = chargetypeDetail.Description
                        };
                    }
                    else
                    {
                        result = new ChargesResponseModel
                        {
                            VatAmount = VATAmount,
                            ChargeAmount = chargeAmount,
                            VatPercent = VATPercent,
                            totalChargeAmount = totalAmount,
                            Currentbalance = companyBalance,
                            CompanyChargeId = companyCharges != null ? companyCharges.CmpChrgId : (int?)null,
                            Title = chargetypeDetail.Title,
                            Description = chargetypeDetail.Description
                        };
                    }
                    response.Add(result);
                }
                return SuccessResponse(response);
            }
            catch (Exception ex)
            {

                return BadResponse(ex.Message);
            }
            return null;
        }

        public DTO CalculateManualCharges(ManualChargesModel model)
        {
            try
            {

                var companyCharges = new BLInfo<SMAM_CompanyCharges_ST>().GetQuerable<SMAM_CompanyCharges_ST>().FirstOrDefault(x => x.CompanyId == model.CompanyId
                && x.ChrgTypeId == model.CompanyChargeId && x.ProductId == model.ProductId);
                string VAT = new BLInfo<SystemSetting>().GetQuerable<SystemSetting>().FirstOrDefault(x => x.Key.Equals(Enums.SystemSettings.VAT)).Value;
                decimal? companyBalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().FirstOrDefault(x => x.CompanyId == model.CompanyId).Balance;
                decimal VATPercent = string.IsNullOrEmpty(VAT) ? 0 : decimal.Parse(VAT);
                decimal? chargeAmount = 0, VATAmount = 0, totalAmount = 0;
                ChargesResponseModel response;
                //if (companyCharges == null)
                //    return BadResponse("Company Don't have Charges");
                if (companyCharges != null)
                {
                    //if charge type id PerFile
                    //if (companyCharges.SMAM_ChargesType_ST.Title.Equals(Enums.Chargetypes.PerFile))
                    //{
                    chargeAmount = companyCharges != null ? companyCharges.Charges : 0;
                    VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0));
                    totalAmount = chargeAmount + VATAmount;
                    //}
                    //else if (companyCharges.SMAM_ChargesType_ST.Title.Equals(Enums.Chargetypes.PerRecord) && companyCharges.SMAM_Products_ST.ProductCode.Equals(Enums.CompanyProductsCode.Wps))
                    //{
                    //    int? sifRecord = new BLInfo<HRPR_ExportSIFFiles_HI>().GetQuerable<HRPR_ExportSIFFiles_HI>().FirstOrDefault(x => x.ExpSifID == model.SifId).TotalEmployees;
                    //    chargeAmount = companyCharges != null ? companyCharges.Charges * (sifRecord != null ? sifRecord : 0) : 0;
                    //    VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0)) * (sifRecord != null ? sifRecord : 0);
                    //    totalAmount = chargeAmount + VATAmount;
                    //}
                    //else if (companyCharges.SMAM_ChargesType_ST.Title.Equals(Enums.Chargetypes.PerRecord) && companyCharges.SMAM_Products_ST.ProductCode.Equals(Enums.CompanyProductsCode.nonWps))
                    //{
                    //    int? sifRecord = new BLInfo<PCMS_Cashload_Master>().GetQuerable<PCMS_Cashload_Master>().FirstOrDefault(x => x.CashloadMasterId == model.SifId).TotalRecords;
                    //    chargeAmount = companyCharges != null ? companyCharges.Charges * (sifRecord != null ? sifRecord : 0) : 0;
                    //    VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0)) * (sifRecord != null ? sifRecord : 0);
                    //    totalAmount = chargeAmount + VATAmount;
                    //}

                    //else
                    //{
                    //    chargeAmount = companyCharges != null ? companyCharges.Charges : 0;
                    //    VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0));
                    //    totalAmount = chargeAmount + VATAmount;
                    //}

                    response = new ChargesResponseModel
                    {
                        VatAmount = VATAmount * (model.Quantity > 1 ? model.Quantity : 1),
                        ChargeAmount = chargeAmount * (model.Quantity > 1 ? model.Quantity : 1),
                        VatPercent = VATPercent,
                        totalChargeAmount = totalAmount * (model.Quantity > 1 ? model.Quantity : 1),
                        Currentbalance = companyBalance,
                        CompanyChargeId = companyCharges != null ? companyCharges.CmpChrgId : (int?)null
                    };
                }
                else
                {
                    response = new ChargesResponseModel
                    {
                        VatAmount = VATAmount,
                        ChargeAmount = chargeAmount,
                        VatPercent = VATPercent,
                        totalChargeAmount = totalAmount,
                        Currentbalance = companyBalance,
                        CompanyChargeId = companyCharges != null ? companyCharges.CmpChrgId : (int?)null
                    };
                }
                return SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }

        }

        public DTO PostManualCharges(ManualChargesTransactionModel model)
        {
            try
            {
                if (model.IsDeducted)
                {
                    decimal? companyCurrentbalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().FirstOrDefault(x => x.CompanyId == model.CompanyId).Balance;
                    if (companyCurrentbalance < model.ChargeAmount)
                        return BadResponse("Company Don't have Sufficient Balance");
                }


                #region GenerateChargesTransaction
                if (model.ChargeAmount > 0)
                {
                    var transactionType = _transactionManager.GetTransactionType(Enums.TransactionTypes.CompanyCharge).TransTypeId;
                    New();
                    Current.TransTypeId = transactionType;
                    Current.CompanyId = model.CompanyId;
                    Current.DebitAmount = model.totalChargeAmount;
                    Current.CreditAmount = 0M;
                    Current.TransDate = model.TransactionDate;
                    Current.TransDesc = "Manual Charges Transaction";
                    Current.TransReference = "";
                    Current.SifId = model.SifId == 0 ? null : model.SifId;
                    Save();
                }
                #endregion
                #region DeductCompanyBalanceWithCharges
                if (model.IsDeducted == true && model.ChargeAmount > 0)
                {
                    var companyCurrentBalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().FirstOrDefault(x => x.CompanyId == model.CompanyId).Balance;
                    _compServ.UpdateCompantBalance(model.CompanyId, companyCurrentBalance - model.totalChargeAmount);
                }
                #endregion

                #region MakeEntryOnChargesTables
                if (model.ChargeAmount > 0)
                {
                    var chargeObject = new ChargeTransaction
                    {
                        ChargeType = model.CompanyChargeId.Value,
                        TotalAmount = model.ChargeAmount + model.VatAmount,
                        IsDeducted = model.IsDeducted,
                        CreatedBy = model.CreatedBy,
                        TransactionId = Current.TransId,
                        VatAmount = model.VatAmount,
                        VatPercent = model.VatPercent,
                        ChargeAmount = model.ChargeAmount
                    };
                    var chargeEntry = _additionalCharges.AddCompanyCharges(chargeObject);
                    
                    if (chargeEntry.isSuccessful)
                    {
                        var manualChargeDetail = new ManualChargesDetail
                        {
                            ProductId = model.ProductId,
                            Quantity = model.Quantity,
                            Description = model.Description,
                            AdditionalChargeId = (int)chargeEntry.data,
                            CreatedOn = DateTime.Now
                        };
                        
                        var manualDetailEntry = _manualChargeService.AddCompanyManaualCharges(manualChargeDetail);
                    }

                }
                #endregion


                return SuccessResponse("Charges Add Successfully");
            }
            catch (Exception ex)
            {

                return BadResponse(ex.Message);
            }
        }



        public DTO CalculateCharges(CalculateChargeBindingModel model)
        {
            try
            {

                var productDetail = new BLInfo<SMAM_Products_ST>().GetQuerable<SMAM_Products_ST>().FirstOrDefault(x => x.ProductId == model.ProductId);

                var chargetypeDetail = new BLInfo<SMAM_ChargesType_ST>().GetQuerable<SMAM_ChargesType_ST>().FirstOrDefault(x => x.ChrgTypeId == model.ChargeId);

                var companyCharges = new BLInfo<SMAM_CompanyCharges_ST>().GetQuerable<SMAM_CompanyCharges_ST>().FirstOrDefault(x => x.CompanyId == model.CompanyId
                && x.ChrgTypeId == model.ChargeId && x.ProductId == productDetail.ProductId);

                string VAT = new BLInfo<SystemSetting>().GetQuerable<SystemSetting>().FirstOrDefault(x => x.Key.Equals(Enums.SystemSettings.VAT)).Value;
                decimal? companyBalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().FirstOrDefault(x => x.CompanyId == model.CompanyId).Balance;
                decimal VATPercent = string.IsNullOrEmpty(VAT) ? 0 : decimal.Parse(VAT);
                decimal? chargeAmount = 0, VATAmount = 0, totalAmount = 0;
                ChargesResponseModel response;
                //if (companyCharges == null)
                //    return BadResponse("Company Don't have Charges");
                if (companyCharges != null)
                {
                    //if charge type id PerFile

                    if (chargetypeDetail.Code.Equals(Enums.ChargesTypeCode.PerRecord))
                    {
                        chargeAmount = companyCharges != null ? companyCharges.Charges * model.TotalRecords : 0;
                        VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0)) * model.TotalRecords;
                        totalAmount = chargeAmount + VATAmount;
                    }
                    else
                    {
                        chargeAmount = companyCharges != null ? companyCharges.Charges : 0;
                        VATAmount = (VATPercent / 100 * (companyCharges != null ? companyCharges.Charges : 0));
                        totalAmount = chargeAmount + VATAmount;
                    }

                    response = new ChargesResponseModel
                    {
                        VatAmount = VATAmount,
                        ChargeAmount = chargeAmount,
                        VatPercent = VATPercent,
                        totalChargeAmount = totalAmount,
                        Currentbalance = companyBalance,
                        CompanyChargeId = companyCharges != null ? companyCharges.CmpChrgId : (int?)null,
                        Description = chargetypeDetail.Description
                    };
                }
                else
                {
                    response = new ChargesResponseModel
                    {
                        VatAmount = VATAmount,
                        ChargeAmount = chargeAmount,
                        VatPercent = VATPercent,
                        totalChargeAmount = totalAmount,
                        Currentbalance = companyBalance,
                        CompanyChargeId = companyCharges != null ? companyCharges.CmpChrgId : (int?)null,
                        Description = chargetypeDetail.Description
                    };
                }
                return SuccessResponse(response);
            }
            catch (Exception ex)
            {

                return BadResponse(ex.Message);
            }
            return null;
        }

        public DTO PerformSifCharges(ChargeTransaction model)
        {
            try
            {


                #region Calculate Charges
                int? totalRecordsInSif = 0;
                if (model.Sifid > 0)
                    totalRecordsInSif = new BLInfo<HRPR_ExportSIFFiles_HI>().GetQuerable<HRPR_ExportSIFFiles_HI>().FirstOrDefault(x => x.ExpSifID == model.Sifid).TotalEmployees;
                else if (model.CashLoadId > 0)
                    totalRecordsInSif = new BLInfo<PCMS_Cashload_Master>().GetQuerable<PCMS_Cashload_Master>().FirstOrDefault(x => x.CashloadMasterId == model.CashLoadId).TotalRecords;
                var chargemodel = new CalculateChargeBindingModel
                {
                    ChargeId = model.ChargeType,
                    TotalRecords = totalRecordsInSif,
                    CompanyId = model.CompanyId,
                    IsDeducted = model.IsDeducted == true ? "true" : "false",
                    ProductId = model.ProductId,
                };
                var charges = (ChargesResponseModel)CalculateCharges(chargemodel).data;
                #endregion

                #region GenerateChargesTransaction
                if (charges.ChargeAmount > 0)
                {
                    var transactionType = _transactionManager.GetTransactionType(Enums.TransactionTypes.CompanyCharge).TransTypeId;
                    New();
                    Current.TransTypeId = transactionType;
                    Current.CompanyId = model.CompanyId;
                    Current.DebitAmount = charges.totalChargeAmount;
                    Current.CreditAmount = 0M;
                    Current.TransDate = DateTime.Now;
                    Current.TransDesc = "";
                    Current.TransReference = "";
                    if (model.Sifid > 0)
                        Current.SifId = model.Sifid;
                    else if (model.CashLoadId > 0)
                        Current.CashLoadId = model.CashLoadId;
                    Save();
                }
                #endregion
                #region DeductCompanyBalanceWithCharges
                if (model.IsDeducted == true && charges.ChargeAmount > 0)
                {
                    var companyCurrentBalance = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().FirstOrDefault(x => x.CompanyId == model.CompanyId).Balance;
                    _compServ.UpdateCompantBalance(model.CompanyId, companyCurrentBalance - charges.totalChargeAmount);
                }
                #endregion

                #region MakeEntryOnChargesTables
                if (charges.ChargeAmount > 0)
                {
                    var chargeObject = new ChargeTransaction
                    {
                        ChargeType = charges.CompanyChargeId.Value,
                        TotalAmount = charges.totalChargeAmount,
                        IsDeducted = model.IsDeducted,
                        CreatedBy = model.CreatedBy,
                        TransactionId = Current.TransId,
                        VatAmount = charges.VatAmount,
                        VatPercent = charges.VatPercent,
                        ChargeAmount = charges.totalChargeAmount
                    };
                    var chargeEntry = _additionalCharges.AddCompanyCharges(chargeObject);
                }
                #endregion

                #region Make Ecrow
                // currently we have a problem with cashload we did'nt recieve sif reference against cashload all the time so for this reason this code will be commented
                //int? statusType = new BLInfo<StatusType>().GetQuerable<StatusType>().FirstOrDefault(x => x.Name.Equals(Enums.StatusTypes.Ecrow)).StatustypeId;
                //if (statusType == null)
                //    return BadResponse("|We don't have a status type Name Escrow");
                //int? freezEscrowStatus = new BLInfo<Status>().GetQuerable<Status>().FirstOrDefault(x => x.TypeId==statusType && x.Name.Equals(Enums.EscrowStatus.Freez)).StatusId;
                //var escrowmodel = new EscrowLedger 
                //{ 
                //CompanyId=model.CompanyId,
                //Amount=model.TotalAmount,
                //CreatedBy=model.CreatedBy,
                //StatusId=freezEscrowStatus,
                //TransactionId=model.TransactionId
                //};
                //var addEscrowTransaction = _escrowService.CreateEsrowTransaction(escrowmodel);
                #endregion

                return SuccessResponse("Charges Add Successfully");
            }
            catch (Exception ex)
            {

                return BadResponse(ex);
            }

        }

        public DTO SubmitAccountTransanctionByExportDetail(HRPR_ExpSifDetail_TR obj, int? CompanyId, string FileName)
        {
            try
            {
                PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                    New();


                Current.CompanyId = CompanyId;
                Current.EmpId = obj.EmpId;
                //Current.TransTypeId = 1;
                Current.DebitAmount = 0; // obj.Basic + obj.Allowances;
                Current.CreditAmount = obj.Basic + (obj.Allowances != null ? obj.Allowances : 0);
                Current.SifId = obj.ExpSifId;
                Current.TransDate = DateTime.Now;
                Current.TransDesc = "Payroll Process";
                Current.TransReference = FileName;   //Need to work On It...
                Current.TransTypeId = 2;


                Save();


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

        public DTO UpdateAccountTran(long Id)
        {
            try
            {
                var obj = new BLInfo<FNGL_AccountTransactions_HI>().GetQuerable<FNGL_AccountTransactions_HI>().Where(a => a.SlipId == Id).FirstOrDefault();

                if (obj != null)
                {
                    long TransId = obj.TransId;
                    GetByPrimaryKey(TransId);

                }
                if (Current != null)
                    PrimaryKeyValue = Id;


                Current.CreditAmount = obj != null ? obj.DebitAmount : 0;

                Save();
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
        //public DTO SubmitSalary(IList<HRMS_EmpSalary_ST> item, int EmpId)
        //{
        //    try
        //    {

        //        foreach (var obj in item)
        //        {
        //            if (obj.EmpSalId != null)
        //                GetByPrimaryKey(obj.EmpSalId);
        //            if (obj.EmpSalId == 0 || obj.EmpSalId == null)
        //                PrimaryKeyValue = null;
        //            if (PrimaryKeyValue == null)
        //            {
        //                New();
        //                Current.CreatedBy = obj.CreatedBy;
        //                Current.CreatedOn = DateTime.Now;
        //            }

        //            Current.EmpId = EmpId;
        //            Current.BasicSalary = obj.BasicSalary;
        //            Current.Travel = obj.Travel;
        //            Current.Housing = obj.Housing;
        //            Current.Allowances = obj.Allowances;

        //            DateTime? EffFrom = obj.EffFrom;
        //            DateTime? EffTo = obj.EffTo;
        //            Common.DateHandler(ref EffFrom);
        //            Common.DateHandler(ref EffTo);
        //            Current.EffFrom = EffFrom;
        //            Current.EffTo = EffTo;

        //            Save();

        //        }
        //        return this.SuccessResponse("Saved Succesfully");
        //    }
        //    catch (Exception Ex)
        //    {
        //        if (Errors.Count > 0)
        //            return BadResponse(Errors);
        //        else
        //            return BadResponse(Ex.Message);
        //    }


        //}

        public DTO AddDepositSlipTransaction(DepositSlipTrsansactionModel model, string transactionType)
        {
            try
            {
                var transactionTypeDetail = _transactionManager.GetTransactionType(transactionType);

                if (transactionTypeDetail == null)
                    return BadResponse("Transaction type Does'nt Exixst ");

                New();
                Current.TransTypeId = transactionTypeDetail.TransTypeId;
                Current.CompanyId = model.CompanyId;
                Current.DebitAmount = 0M;
                Current.CreditAmount = model.CreditAmount;
                Current.TransDate = DateTime.Now;
                Current.TransDesc = model.TransactionDescription;
                Current.TransReference = model.TransactionReference;
                Current.SlipId = model.SlipId;
                Save();
                return SuccessResponse(Current);
            }
            catch (Exception ex)
            {

                return BadResponse(ex);
            }
            return BadResponse("");
        }
    }
}

