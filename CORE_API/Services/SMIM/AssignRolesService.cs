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

namespace CORE_API.Services.SMIM
{
    public class AssignRolesService : CastService<SMIM_AssignRoles_ST>
    {

        public AssignRolesService()
        {

        }
        protected override bool IsValidBeforeSave()
        {

            this.Errors.Clear();

            
            if (this.Errors.Count > 0)
                return true;
            else
                return false;


        }

        public DTO GetRightsbyRoles(long RoleId)
        {


            string[] paramater = { "roleId" };
            DataTable dt = ExecuteSp("sp_GetAssignROles",paramater, RoleId);

            #region left join
            //var obj = new BLInfo<SMIM_Rights_ST>().GetQuerable<SMIM_Rights_ST>().Where(a => a.RightId != 0).ToList();
            //var objAssignroles = new BLInfo<SMIM_AssignRoles_ST>().GetQuerable<SMIM_AssignRoles_ST>().Where(a => a.RoleId != 0).ToList();

            //var lst = (from sa in  objAssignroles
            //           join ar in obj
            //           on sa.RightId equals ar.RightId into rd from rt in rd.DefaultIfEmpty().ToList();
            //           //where rt.SMIM_AssignRoles_STs. == RoleId
            //           //select new
            //           //{
            //           //  //  RoleName = rt.SMIM_Rights_ST != null ? rt.SMIM_Rights_ST.RightName: "",
            //           //    //FormName = rt.SMIM_Rights_ST != null ? rt.SMIM_Rights_ST.FormName : "",
            //           //    //rt.AccessId,
            //           //  //AccessId=  rt.AccessId != null ? rt.AccessId:0,
            //           //  //Allow = rt.Allow != null ? rt.Allow : null,
            //           //  //RoleId = rt.RoleId != null ? rt.RoleId: null,
                        
            //           //  //sa.FormName,sa.RightName


            //           //}).ToList();
            #endregion
            return this.SuccessResponse(dt);
        }

       

        public DTO SubmitAssignRights(List<SMIM_AssignRoles_ST> lst)
        {
            try
            {

                foreach (var obj in lst)
                {
                   // int roleId = int.Parse(lst[0].RoleId.ToString());
                    PrimaryKeyValue = null;
                    if (obj.AccessId != 0)
                        GetByPrimaryKey(obj.AccessId);

                    if (PrimaryKeyValue == null)
                    {
                        New();
                        int AccessId = new BLInfo<SMIM_AssignRoles_ST>().GetQuerable<SMIM_AssignRoles_ST>().Max(a => a.AccessId);
                        if(AccessId != 0 && AccessId !=null)
                        Current.AccessId = ++AccessId;
                    }

                    Current.RightId = obj.RightId;
                    Current.RoleId = obj.RoleId;
                    Current.Allow = obj.Allow;
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
        
        public DTO DeleteRights(long Id)
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

