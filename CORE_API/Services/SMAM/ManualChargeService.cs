using CORE_API.General;
using CORE_API.Models.Transaction;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace CORE_API.Services.SMAM
{
    public class ManualChargeService : CastService<ManualChargesDetail>
    {
        dbEngine db = new dbEngine();
        public ManualChargeService()
        {


        }

        public DTO AddCompanyManaualCharges(ManualChargesDetail model)
        {
            try

            {
                Logger.TraceService(model.ProductId + model.AdditionalChargeId + model.Quantity + model.Description + " asdsad", "chargeEntry");
                New();
                Current.ProductId = model.ProductId;
                Current.Quantity = model.Quantity;
                Current.Description = model.Description;
                Current.AdditionalChargeId = model.AdditionalChargeId;
                Current.CreatedOn = DateTime.Now;
                Save();
                return SuccessResponse("manaual charges detail successfully added");
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message, "chargeEntry");
                return BadResponse(ex.Message);
            }



        }

        public DTO GetManualChargesDetail(int? UserId)
        {
            try
            {
                string[] paramater = { "UserId" };
                DataTable returnManualChargesDetail = ExecuteSp("SP_GetManualChargesDetail", paramater, UserId);
                return this.SuccessResponse(returnManualChargesDetail);
            }
            catch (Exception ex)
            {

                return BadResponse(ex.Message);
            }
        }
    }
}