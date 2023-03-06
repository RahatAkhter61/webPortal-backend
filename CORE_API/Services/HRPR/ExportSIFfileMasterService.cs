using ContextMapper;
using CORE_API.General;
using CORE_API.Models;
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
    public class ExportSIFfileMasterService : CastService<HRPR_ExportSIFFiles_HI>
    {

        private readonly AccountTransactionService _accountTran;

        public ExportSIFfileMasterService()
        {

            _accountTran = new AccountTransactionService();
        }
        public HRPR_ExportSIFFiles_HI SubmitExportSIFFiles(HRPR_SifMaster_TR obj, CalculateMultipleChargeBindingModel chargeModel, string IsCards = "", string Establistmentdid = "", int UserId = 0, string Month = "", string Year = "")
        {
            try
            {
                //if (obj.SifId != 0)
                //{
                //    var OBJ = new BLInfo<HRPR_ExportSIFFiles_HI>().GetQuerable<HRPR_ExportSIFFiles_HI>().Where(a=>a.s)
                //    GetByPrimaryKey(obj.ExpSifID);

                //}
                if (obj.SifId == 0 || obj.SifId == null)

                    PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                {
                    New();
                    Current.CreatedOn = obj.CreatedOn;
                    Current.CreatedOn = DateTime.Now;
                }

                var SIF_Typ = string.Empty;
                if (IsCards == "true")
                    SIF_Typ = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(o => o.Type == "KP_Card").Select(o => o.Code).FirstOrDefault();
                else
                {
                    SIF_Typ = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(o => o.Type == "OB_Card").Select(o => o.Code).FirstOrDefault();
                }
                var objCompany = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == obj.CompanyID).FirstOrDefault();
                Current.Createdby = UserId;
                Current.CreatedOn = DateTime.Now;
                Current.Month = Month;
                Current.Year = Year;
                Current.SIF_Type = SIF_Typ;
                //  Current.EstId = objCompany != null ? objCompany.EstId : "";
                Current.EstId = Establistmentdid;
                Current.CompanyId = obj.CompanyID;
                Current.FileName = objCompany != null ? objCompany.EstId + DateTime.Now.ToString("yyMMddhhmmss") : "";    /// objCompany != null ? objCompany.EstId.Substring(objCompany.EstId.Length - 13) + DateTime.Now.ToString("yyMMddhhmmss") : "";
                Current.TotalAmount = obj.TotalSalary;// _reqSifFile.Sum(x => (Convert.ToDecimal(x.BasicSalary) + Convert.ToDecimal(x.Allowances)));
                Current.TotalEmployees = obj.TotalEmployees;
                Current.IsGenerated = false;


                Save();

                _accountTran.SubmitAccountTransanctionbyExportSIFFiles(Current, chargeModel, IsCards);


                return Current;


            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DTO GetSifDetailbyCompanyId(int companyId)
        {

            var returnSifDetail = new BLInfo<HRPR_ExportSIFFiles_HI>().GetQuerable<HRPR_ExportSIFFiles_HI>().Where(a => a.CompanyId == companyId).Select
                (o => new
                {
                    o.ExpSifID,
                    o.TotalAmount,
                    o.TotalEmployees,
                    SalaryMonth = o.Month + "-" + o.Year,
                    o.SIF_Type,
                    o.CreatedOn

                }).OrderByDescending(o => o.ExpSifID).ToList();

            return SuccessResponse(returnSifDetail);
        }
        //public DTO GetCompanyDeposit()
        //{
        //    try
        //    {
        //        var objdeposit = new BLInfo<HRPR_DepositSlip_TR>().GetQuerable<HRPR_DepositSlip_TR>().Where(a => a.SlipId != 0).ToList();
        //        var lst = (from sa in objdeposit
        //                   select new
        //                   {
        //                       sa.SlipId,
        //                       sa.SlipNo,
        //                       sa.CompanyId,
        //                       Company = sa.CompanyId != 0 ? new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == sa.CompanyId).FirstOrDefault().Name : "",
        //                       sa.Amount,
        //                       sa.Date,
        //                       sa.ImgPath,
        //                       DepositDate = sa.Date.Value.ToString("dd/MM/yyyy")

        //                   }).ToList();
        //        return SuccessResponse(lst);
        //    }
        //    catch (Exception Ex)
        //    {
        //        return BadResponse(Ex.Message);
        //    }
        //}
        //public DTO SubmitDeposit(HRPR_DepositSlip_TR obj, string filePath)
        //{
        //    try
        //    {


        //        if (obj.SlipId != null)
        //            GetByPrimaryKey(obj.SlipId);
        //        if (obj.SlipId == 0 || obj.SlipId == null)
        //            PrimaryKeyValue = null;
        //        if (PrimaryKeyValue == null)
        //            New();


        //        Current.CompanyId = obj.CompanyId;
        //        Current.SlipNo = obj.SlipNo;
        //        Current.Amount = obj.Amount;
        //        Current.ImgPath = filePath != "" ? filePath : obj.ImgPath;
        //        if (filePath != "")
        //        {
        //            Current.UploadedBy = obj.UploadedBy;
        //            Current.UploadedOn = DateTime.Now;
        //        }
        //        DateTime? Date = obj.Date;
        //        Common.DateHandler(ref Date);
        //        Current.Date = Date;

        //        Save();
        //        _accountTranSer.SubmitAccountTransanction(Current);


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

        //public DTO DeleteDeposit(long Id)
        //{
        //    try
        //    {
        //        GetByPrimaryKey(Id);
        //        if (Current != null)
        //            PrimaryKeyValue = Id;

        //        Delete();
        //        _accountTranSer.UpdateAccountTran(Id);

        //        return this.SuccessResponse("Deleted Successfully.");


        //    }
        //    catch (Exception Ex)
        //    {
        //        if (Errors.Count > 0)
        //            return this.BadResponse(Errors);
        //        else
        //            return this.BadResponse(Ex.Message);
        //    }
        //}
    }
}
