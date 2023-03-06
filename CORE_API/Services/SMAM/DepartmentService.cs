using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.SMAM
{
    public class DepartmentService : CastService<SMAM_Departments_ST>
    {

        public DepartmentService()
        {

        }


        public DTO GetSDepartment()
        {
            var obj = new BLInfo<SMAM_Departments_ST>().GetQuerable<SMAM_Departments_ST>().Where(a => a.DeptId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.DeptId,
                           sa.IsActive,
                           sa.Description,
                           
                       }).ToList();
            return this.SuccessResponse(lst);
        }






        public DTO SubmitDepartment(IList<SMAM_Departments_ST> item, int? CreateBy, int StationId)
        {
            try
            {
                foreach (var obj in item)
                {
                    if (obj.DeptId != 0)
                        GetByPrimaryKey(obj.DeptId);
                    if (obj.DeptId == 0 || obj.DeptId == null)
                        PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                    {
                        New();
                        Current.CreateBy = CreateBy;
                        Current.CreatedOn = DateTime.Now;
                    }

                    Current.IsActive = obj.IsActive;
                    Current.Description = obj.Description;
                    Current.StationId = StationId;

                  
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


        //public DTO DeleteRoleData(long Id)
        //{
        //    try
        //    {
        //        GetByPrimaryKey(Id);
        //        if (Current != null)
        //            PrimaryKeyValue = Id;

        //        Delete();
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

