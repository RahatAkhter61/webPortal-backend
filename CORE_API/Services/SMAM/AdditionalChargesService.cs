using CORE_API.General;
using CORE_API.Models.Transaction;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Services.SMAM
{
    public class AdditionalChargesService : CastService<AdditionalCharge>
    {

        dbEngine db = new dbEngine();
        public AdditionalChargesService()
        {

            
        }
        

        public DTO AddCompanyCharges(ChargeTransaction model)
        {
            try
            {
                if (model != null)
                {
                    New();
                    Current.ChargeAmount = model.ChargeAmount;
                    Current.CompanyChargeId = model.ChargeType;
                    Current.IsDeducted = model.IsDeducted;
                    Current.CreatedBy = model.CreatedBy;
                    Current.CreatedOn = DateTime.Now;
                    Current.TransactionId = model.TransactionId;
                    Current.TotalAmount = model.TotalAmount;
                    Current.VatAmount = model.VatAmount;
                    Current.VatPercentage = model.VatPercent;
                    Save();
                    return SuccessResponse(Current.ChargeId);
                }
                return BadResponse("Model Could not be null");
            }
            catch (Exception ex)
            {

                return BadResponse(ex);
            }
        }

    }
}