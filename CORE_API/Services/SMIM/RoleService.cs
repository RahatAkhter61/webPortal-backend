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

namespace CORE_API.Services.SMIM
{
    public class RoleService : CastService<SMIM_Roles_ST>
    {
        //private IBaseRepository<SMIM_Roles_ST> repo;
        //public RoleService(IBaseRepository<SMIM_Roles_ST> rep)
        //{
        //    repo = rep;
        //}

        public RoleService()
        {

        }


        public DTO GetRole()
        {
            var obj = new BLInfo<SMIM_Roles_ST>().GetQuerable<SMIM_Roles_ST>().Where(a => a.RoleId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.RoleName,
                           sa.RoleId,

                       }).ToList();
            return  this.SuccessResponse(lst);
        }

        public DTO SubmitRole(SMIM_Roles_ST obj)
        {
            try
            {
                if (obj.RoleId != 0)
                    GetByPrimaryKey(obj.RoleId);

                if (PrimaryKeyValue == null)
                    New();

                Current.RoleName = obj.RoleName;
                Save();

                return this.SuccessResponse("Saved Succesfully");
            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return BadResponse(obj);
                else
                    return BadResponse(Ex.Message);
            }
        }

        public DTO DeleteRoleData(long Id)
        {
            try
            {
                GetByPrimaryKey(Id);
                if (Current != null)
                    PrimaryKeyValue = Id;

                Delete();
                return this.SuccessResponse("Deleted Successfully.");


            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }
    }
}

