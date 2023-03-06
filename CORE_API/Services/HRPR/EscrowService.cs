using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Services.HRPR
{

    public class EscrowService : CastService<EscrowLedger>
    {

        public EscrowService()
        {
        }

        public DTO CreateEsrowTransaction(EscrowLedger model)
        {
            try
            {
                if (model != null)
                {
                    New();
                    Current.Amount = model.Amount;
                    Current.CompanyId = model.CompanyId;
                    Current.CreatedBy = model.CreatedBy;
                    Current.CreatedOn = DateTime.Now;
                    Current.TransactionId = model.TransactionId;
                    Current.StatusId = model.StatusId;
                    Save();
                    return SuccessResponse("Escrow Created Successfully");
                }
                return BadResponse("model cant be null");
            }
            catch (Exception ex)
            {

                return BadResponse(ex);
            }
        }
    }

}