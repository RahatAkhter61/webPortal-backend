using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.General
{
    public class CastService<T> : Base<T> where T : class
    {
        
        public CastService()
        {
        }

    }
    //public class BaseService<T> where T : class,new()
    //{
    //    protected IBaseRepository<SMIM_Roles_ST> objMaster = null;
    //    public BaseService(IBaseRepository<SMIM_Roles_ST> rep)
    //    {
    //        objMaster = rep;
    //    }

    //    public BaseService()
    //    {

    //    }
       

    //    public DTO SubmitRole(SMIM_Roles_ST obj)
    //    {
    //        try
    //        {
    //            if (obj.RoleId != 0)
    //                objMaster.GetByPrimaryKey(obj.RoleId);

    //            if (objMaster.PrimaryKeyValue == null)
    //                objMaster.New();

    //            objMaster.Current.RoleName = obj.RoleName;
    //            objMaster.Save();

    //            return this.SuccessResponse("Saved Succesfully");
    //        }
    //        catch (Exception Ex)
    //        {
    //            if (objMaster.Errors.Count > 0)
    //                return objMaster.BadResponse(obj);
    //            else
    //                return objMaster.BadResponse(Ex.Message);
    //        }


    //    }
    //    public DTO SuccessResponse(object data)
    //    {
    //        return new DTO { isSuccessful = true, data = data };
    //    }

    //    public DTO BadResponse(object data)
    //    {
    //        return new DTO { isSuccessful = false, errors = data };
    //    }
    //}



}