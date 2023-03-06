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
    public class LoginService : CastService<SMIM_UserMst_ST>
    {

        public LoginService()
        {

        }

        public DTO GetUserVerification(string UserName, string Password)
        {

            //await this.Runasync(() => { });
            try
            {
                List<string> Error = new List<string>();


                if (UserName != null && Password != null)
                {


                    var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(sa => sa.UserName == UserName && sa.Password == Password).FirstOrDefault();
                    if (objUser != null)
                    {
                        //  string Loginname = "";
                        //  var objUserRole = new BLInfo<User>().GetQuerable<User>().Where(sa => sa.LoginName == UserName && sa.Role == "Admin").FirstOrDefault();
                        Dictionary<dynamic, dynamic> dic = new Dictionary<dynamic, dynamic>();
                        dic.Add("UserName", objUser.UserName);
                        dic.Add("UserId", objUser.UserId);
                        dic.Add("RoleId", objUser.RoleId);
                        dic.Add("CompanyId", objUser.CompanyId);
                        return this.SuccessResponse(dic);

                    }
                    else
                        return this.BadResponse("Invalid Username or Password.");
                }
                else
                {
                    if (UserName == null)
                        //return this.BadResponse("Required : UserName");
                        Error.Add("Required : Username");
                    if (Password == null)
                        //  return this.BadResponse("Required : Password");
                        Error.Add("Required : Password");
                }

                if (Error.Count > 0)
                    return this.BadResponse(Error);

                return this.SuccessResponse("");
            }
            catch (Exception Ex)
            {
                return this.BadResponse(Ex);
            }
        }
    
    }
}

