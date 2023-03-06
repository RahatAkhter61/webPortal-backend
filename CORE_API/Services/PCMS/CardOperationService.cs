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
    public class CardOperationService : CastService<CardOperationMaster>
    {
        dbEngine dbEngine = new dbEngine();
        public CardOperationService()
        { }

        public DTO SPGetCardOperaion(int? UserId)
        {

            try
            {
                string[] paramater = { "UserId" };
                DataTable dt = ExecuteSp("SP_GetCardOperaion", paramater, UserId);
                return this.SuccessResponse(dt);

            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }

        }

        public DTO SubmitCardOperation(CardOperationMaster obj)
        {

            try
            {
                var status = new BLInfo<PCMS_Status_CL>().GetQuerable<PCMS_Status_CL>().Where(o => o.Description == "In-Process")
                  .Select(o => o.Statusid).FirstOrDefault();

                New();
                Current.CompanyId = obj.CompanyId;
                Current.ProductId = obj.ProductId;
                Current.TotalRecords = obj.CardOperationDetails.Count;
                Current.Status = status;
                Current.Operation_Type = obj.Operation_Type;
                Current.Createdon = DateTime.Now;
                Current.Createdby = obj.Createdby;
                Current.CardOperationDetails = obj.CardOperationDetails;
                Save();

                return this.SuccessResponse("Saved Successfully");
            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }
        }

        public DTO UpdateCardOperation(int cOMasterId, int statusId,int EmpId)
        {
            try
            {
                var obj = new BLInfo<CardOperationMaster>().GetQuerable<CardOperationMaster>().Where(o => o.COMasterId == cOMasterId).FirstOrDefault();
                if (obj != null)
                    GetByPrimaryKey(obj.COMasterId);

                if (PrimaryKeyValue != null)
                {
                    Current.Status = statusId;
                    Current.Modifyby = EmpId;
                    Current.Modifyon = DateTime.Now;
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

        public DTO GetCardOPdtlbyId(int COMasterId)
        {
            string[] paramater = { "COMasterId" };
            DataTable dt = ExecuteSp("SP_GetCardOPdtlbyId", paramater, COMasterId);
            return this.SuccessResponse(dt);
        }

    }
}