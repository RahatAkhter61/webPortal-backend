using ContextMapper;
using CORE_API.General;
using CORE_API.Models.CompanyHistory;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.SMAM
{
    public class CompanyChargesHistoryService : CastService<CompanyChargesHistory>
    {

        public CompanyChargesHistoryService()
        {

        }

        public DTO CompanyChargesHistory(List<CompanyChargesHistory> objhistory)
        {
            try
            {

                if (objhistory != null && objhistory.Count > 0)
                {
                    foreach (var item in objhistory)
                    {
                        New();
                        Current.OldCharges = item.OldCharges;
                        Current.NewCharges = item.NewCharges;
                        Current.EffectiveFrom = item.EffectiveFrom;
                        Current.EffectiveTo = item.EffectiveTo;
                        Current.UpdatedBy = item.UpdatedBy;
                        Current.UpdatedOn = DateTime.Now;
                        Current.CompanyChargeId = item.CompanyChargeId;
                        Save();
                    }
                }
                return this.SuccessResponse(Current);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }


    }
}
