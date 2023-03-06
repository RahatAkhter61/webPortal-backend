using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Services.WPS
{
    public class PRCService : CastService<PRCDetail>
    {

        dbEngine _dbEngine = new dbEngine();

        #region Get PRC Details by PAF Master Id
        public DTO GetPRCDetailbyId(int PAFId)
        {
            try
            {

                if (PAFId == 0)
                    return BadResponse("Please Insert Correct Information");

                var returnPRCDetaillst = _dbEngine._WPSContext.PRCDetails.Where(o => o.PafMasterId == PAFId).Select(ep => new
                {

                    ep.Id,
                    ep.PRCFilename,
                    ep.NoOfRecordsInPaf,
                    ep.TotalPay,
                    ep.TotalVariablePay,
                    ep.TotalFixedPay,
                    ep.GeneratedOn

                }).ToList();

                if (returnPRCDetaillst != null && returnPRCDetaillst.Count == 0)
                    return BadResponse("Record Not Found");

                return this.SuccessResponse(returnPRCDetaillst);
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message, "PRC_Details");
                return this.BadResponse(ex.Message);
            }
        }
        #endregion


    }
}