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
    public class UserRegistrationService : CastService<SMIM_UserMst_ST>
    {
        SMIM_UserMst_ST obj;

        public UserRegistrationService()
        {
            obj = new SMIM_UserMst_ST();
        }
        protected override bool IsValidBeforeSave()
        {

            this.Errors.Clear();

            var obj = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserName == this.Current.UserName && a.Email == this.Current.Email && a.UserId != this.Current.UserId);

            if (obj.Count() > 0)
                this.Errors.Add("User is already exist.");

            if (this.Errors.Count > 0)
                return true;
            else
                return false;


        }

        public DTO GetUsers()
        {
            var obj = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != 0).ToList();

            var lst = (from sa in obj
                       select new
                       {
                           sa.UserId,
                           sa.UserName,
                           sa.FirstName,
                           sa.LastName,
                           sa.Email,
                           sa.ContactNo,
                           sa.Password,
                           sa.RoleId,
                           sa.CompanyId,
                           RoleName = sa.SMIM_Roles_ST != null ? sa.SMIM_Roles_ST.RoleName : ""


                       }).OrderBy(a => a.UserId).ToList();
            return this.SuccessResponse(lst);
        }



        public DTO SubmitUsers(SMIM_UserMst_ST obj)
        {
            try
            {
                if (obj.UserId != 0)
                    GetByPrimaryKey(obj.UserId);

                if (PrimaryKeyValue == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.CreatedBy = obj.CreatedBy;
                }

                Current.FirstName = obj.FirstName;
                Current.LastName = obj.LastName;
                Current.UserName = obj.UserName;
                Current.CompanyId = obj.CompanyId;
                Current.RoleId = obj.RoleId;
                Current.Password = obj.Password;
                Current.Email = obj.Email;
                Current.ContactNo = obj.ContactNo;
                Current.LoginDate = null;
                Save();

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

        public DTO DeleteUser(long Id)
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

