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

namespace CORE_API.Services.HRMS
{
    public class EmployeeChargesService : CastService<HRMS_EmpCharges_ST>
    {

        public EmployeeChargesService()
        {

        }
        public DTO SubmitCharges(IList<HRMS_EmpCharges_ST> item, int EmpId)
        {
            try
            {

                foreach (var obj in item)
                {
                    if (obj.EmpChrgId != null)
                        GetByPrimaryKey(obj.EmpChrgId);
                    if (obj.EmpChrgId == 0 || obj.EmpChrgId == null)
                        PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                    {
                        New();
                        Current.CreatedBy = obj.CreatedBy;
                        Current.CreatedOn = DateTime.Now;
                    }

                    Current.EmpId = EmpId;
                    Current.ChrgTypeId = obj.ChrgTypeId;
                    Current.Charges = obj.Charges;

                    DateTime? EffFrom = obj.EffFrom;
                    DateTime? EffTo = obj.EffTo;
                    Common.DateHandler(ref EffFrom);
                    Common.DateHandler(ref EffTo);
                    Current.EffFrom = EffFrom;
                    Current.EffTo = EffTo;

                    Save();

                }
                return this.SuccessResponse("Saved Succesfully");
            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return BadResponse(Errors);
                else
                    return BadResponse(Ex.Message);
            }


        }


    }
}
