using ContextMapper;
using CORE_API.Areas.PCMS.Controllers;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CORE_API.Services.PCMS
{
    public class ManualTransactionService : CastService<MTP_Master>
    {
        dbEngine dbEngine = new dbEngine();
        public ManualTransactionService()
        { }

        public DTO GetModetype()
        {
            try
            {
                var result = new BLInfo<MasterCharge>().GetQuerable<MasterCharge>().Where(o => o.IsActive == true).ToList();
                return this.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO SubmitManualTransaction(MTP_Master obj)
        {

            try
            {

                var status = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(o => o.Description == "In-Process")
                    .Select(o => o.Statusid).FirstOrDefault();

                New();
                Current.CompanyId = obj.CompanyId;
                Current.ProductId = obj.ProductId;
                Current.Mode = obj.Mode;
                Current.Category = obj.Category;
                Current.TotalRecords = obj.TotalRecords;
                Current.TotalAmount = obj.TotalAmount;
                Current.Createdon = DateTime.Now;
                Current.Createdby = obj.Createdby;
                Current.Status = status;
                Current.MTP_Details = obj.MTP_Details;
                Save();

                return this.SuccessResponse("Saved Successfully");
            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }
        }

        public DTO MTPUpdateStatus(int MTPMasterId, int status)
        {
            try
            {

                var obj = new BLInfo<MTP_Master>().GetQuerable<MTP_Master>().Where(o => o.MTPMasterId == MTPMasterId).FirstOrDefault();
                if (obj != null)
                    GetByPrimaryKey(obj.MTPMasterId);

                if (PrimaryKeyValue != null)
                {
                    Current.Status = status;
                    Save();
                    return SuccessResponse("Save Successful");
                }

                return BadResponse("Record Not Found");
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO SP_GetMTPTrans(int UserId)
        {
            string[] paramater = { "UserId" };
            DataTable dt = ExecuteSp("SP_GetMTPTrans", paramater, UserId);
            return this.SuccessResponse(dt);
        }

        public DTO SP_GetMTPTransdtlbyId(int MTPMasterId)
        {
            try
            {
                string[] paramater = { "MTPMasterId" };
                DataTable dt = ExecuteSp("SP_GetMTPTransdtlbyId", paramater, MTPMasterId);
                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }


        }



    }
}