using ContextMapper;
using CORE_API.General;
using CORE_API.Models.WPS;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CORE_API.Services.WPS
{
    public class PAFService : CastService<PAFMaster>
    {

        dbEngine _dbEngine = new dbEngine();

        #region Get ALL PAF Master Record
        public DTO GetAllPAF(DateTime? PAFUploadedOn)
        {
            try
            {

                string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();
                string[] paramater = { "PAFUploadedOn" };
                DataTable Returnlist = ExecuteStoreProc("SPGetAllPAF", paramater, Connection, PAFUploadedOn);

                return this.SuccessResponse(Returnlist);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }
        #endregion  

        #region Get ALL PAF Details by PAF MAster Id
        public DTO GetPAFDetailbyId(int PAFId)
        {
            try
            {

                if (PAFId == 0)
                    return BadResponse("Please Insert Correct Infromation");

                var returnPAFDetaillst = (from t1 in _dbEngine._WPSContext.PAFMasters
                                          join t2 in _dbEngine._WPSContext.PAFDetails on t1.Id equals t2.PAFMasterId
                                          join t3 in _dbEngine._WPSContext.TopUpRequests on t2.Id equals t3.PAFDetailId
                                          where t1.Id == PAFId

                                          select new
                                          {

                                              PAFMasterId = t1.Id,
                                              PAFDetailId = t2.Id,
                                              topupRequestId = t3.Id,
                                              t1.EstablishmentNo,
                                              t2.FixedAmount,
                                              t2.VariableAmount,
                                              t2.LeaveDays,
                                              t2.NetAmount,
                                              t2.EmpPersonalNumber,
                                              t2.NoOfDaysInMonth,
                                              t3.Status,

                                          }).ToList();

                if (returnPAFDetaillst == null && returnPAFDetaillst.Count == 0)
                    return this.BadResponse("Record Not Found");

                return this.SuccessResponse(returnPAFDetaillst);
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message, "PAF_Details");
                return this.BadResponse(ex.Message);
            }
        }
        #endregion

        #region Update Status TopUpRequest
        public DTO UpdateStatusTopUpRequest(List<WPSupdatetopupstatus> model)
        {

            try
            {
                if (model == null && model.Count == 0)
                    return this.BadResponse("List Empty");

                foreach (var item in model)
                {

                    #region
                    if (item.PAFDetailId == 0)
                    {
                        var topupnpendinglist = _dbEngine._WPSContext.TopUpRequests.Where(x => x.PAFMasterId == item.PAFMasterId && x.Status.Equals(Enums.StatusTypes.Pending)).ToList();
                        foreach (var obj in topupnpendinglist)
                        {
                            obj.Status = item.Status;
                        }

                    }
                    #endregion

                    #region Update Detail Individual
                    if (item.PAFDetailId != 0)
                    {
                        var topupnpendinglist = _dbEngine._WPSContext.TopUpRequests.Where(x => x.PAFDetailId == item.PAFDetailId && x.Status.Equals(Enums.StatusTypes.Pending)).FirstOrDefault();
                        topupnpendinglist.Status = item.Status;
                    }
                    #endregion

                }

                _dbEngine._WPSContext.SubmitChanges();

                return this.SuccessResponse("Update Successfully");
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }
        #endregion



    }
}