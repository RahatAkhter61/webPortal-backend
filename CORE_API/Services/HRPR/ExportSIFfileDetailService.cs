using ContextMapper;
using CORE_API.General;
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
    public class ExportSIFfileDetailService : CastService<HRPR_ExpSifDetail_TR>
    {

        private readonly AccountTransactionService _accountTran;

        public ExportSIFfileDetailService()
        {
            _accountTran = new AccountTransactionService();
        }

        public DTO SubmitSIFDETAIL(IList<ExportSIfMapper> obj, int? CompanyId, int expsifdata, string FileName, string IsCards = "")
        {
            try
            {
                foreach (var item in obj)
                {

                    PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                        New();


                    Current.RecordType = "EDR";
                    Current.ExpSifId = expsifdata;
                    Current.StartDate = item.StartDate;
                    Current.EndDate = item.EndDate;
                    Current.EmpId = IsCards == "false" ? null : (item.EmpId == 0 ? null : item.EmpId);
                    Current.Basic = item.FixAmount;
                    Current.Allowances = item.Allowances;
                    //Current.NetSalary = (item.Basic + item.Allowances);  //item.NetSalary;
                    Current.NetSalary = item.Total;
                    Current.LeaveDays = item.LeaveDays;
                    Current.RoutingCode = item.RoutingCode;
                    Current.IBAN = item.IBAN;
                    Current.PersonalNo = item.PersonalNo;
                  
                    //Current.PersonalNo 

                    Save();
                    if (IsCards == "" || IsCards == "true")
                        _accountTran.SubmitAccountTransanctionByExportDetail(Current, CompanyId, FileName);
                }

                return this.SuccessResponse("Saved Succesfully");


            }

            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return BadResponse(obj);
                else
                    return BadResponse(Ex.Message);
            }
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
