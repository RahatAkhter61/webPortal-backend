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
    public class MenuService : CastService<SMIM_Menu_ST>
    {

        public MenuService()
        {

        }


        protected override bool IsValidBeforeSave()
        {

            this.Errors.Clear();

            var obj = new BLInfo<SMIM_Menu_ST>().GetQuerable<SMIM_Menu_ST>().Where(a => a.MenuDescription == this.Current.MenuDescription && a.MenuId != this.Current.MenuId);

            if (obj.Count() > 0)
                this.Errors.Add("Menu is already exist.");
            //&& a.MenuId != this.Current.MenuId
            //if (new BLInfo<SMIM_Menu_ST>().GetQuerable<SMIM_Menu_ST>().Where(a => a.MenuSeq == this.Current.MenuSeq ).Count() > 0)
            //{
            //    this.Errors.Add("Duplicate Sequence is not allowed.");
            //}



            if (this.Errors.Count > 0)
                return true;
            else
                return false;


        }

        public DTO GetMenus()
        {
            var obj = new BLInfo<SMIM_Menu_ST>().GetQuerable<SMIM_Menu_ST>().Where(a => a.RightId != 0).ToList();

            var lst = (from sa in obj
                       select new
                       {
                           sa.MenuId,
                           MenuType = sa.MenuType == 'P' ? "Parent Menu" : "Child Menu",
                           MenuTypeCharValue = sa.MenuType,
                           sa.MenuSeq,
                           sa.MenuDescription,
                           IconPath = sa.IconPath != string.Empty ? sa.IconPath : "",
                           sa.RightId,
                           sa.ParentMenuId,
                           FormName = sa.SMIM_Rights_ST != null ? sa.SMIM_Rights_ST.FormName : "",
                           // ParentMenuName =
                           RightName = sa.SMIM_Rights_ST != null ? sa.SMIM_Rights_ST.RightName : "",
                           sa.urlPath

                       }).Select(a => new
                       {
                           a.MenuId,
                           a.MenuType,
                           a.MenuTypeCharValue,
                           a.FormName,
                           a.urlPath,
                           a.MenuSeq,
                           a.MenuDescription,
                           a.IconPath,
                           a.RightId,
                           a.ParentMenuId,
                           a.RightName,
                           ParentMenuName = a.ParentMenuId != null ? obj.Where(ab => ab.MenuId == a.ParentMenuId).FirstOrDefault().MenuDescription : null,
                           //   ParentMenuName = a.ParentMenuId != null ? GetParentMenuName(a.ParentMenuId) : null
                       }).ToList();//.OrderBy(a => a.FormName).ToList();
            return this.SuccessResponse(lst);
        }
        string GetParentMenuName(int? ParentMenuId)
        {
            string obj = new BLInfo<SMIM_Menu_ST>().GetQuerable<SMIM_Menu_ST>().Where(a => a.MenuId == ParentMenuId).FirstOrDefault().MenuDescription.ToString();
            return obj;

        }
        public DTO GetParentMenu()
        {
            var obj = new BLInfo<SMIM_Menu_ST>().GetQuerable<SMIM_Menu_ST>().Where(a => a.RightId != 0 && (a.ParentMenuId == 0 || a.ParentMenuId == null)).ToList();

            var lst = (from sa in obj
                       select new
                       {
                           sa.MenuId,
                           sa.MenuDescription

                       }).ToList();//.OrderBy(a => a.FormName).ToList();
            return this.SuccessResponse(lst);
        }


        public DTO GetMenusByRole(int RoleId)
        {
            bool? IsAllow = true;
            var objViewAssign = new BLInfo<SMIM_AssignRoles_ST>().GetQuerable<SMIM_AssignRoles_ST>().Where(a => a.RoleId == RoleId && a.Allow == true).ToList();  /// && a.SMIM_Rights_ST.RightName == "View"
            int?[] tabIds = null;
            if (objViewAssign != null)
            {
                tabIds = objViewAssign.Select(o => o.RightId).ToArray();
            }
            var obj = new BLInfo<SMIM_Menu_ST>().GetQuerable<SMIM_Menu_ST>().Where(a => tabIds.Contains(a.RightId) && a.ParentMenuId == null).ToList();

            #region
            //var lst = (from sa in obj
            //           select new
            //           {
            //               sa.MenuId,
            //             name=  sa.MenuDescription,
            //              state = sa.urlPath,
            //              icon = sa.IconPath,
            //              type = "link",

            //               sa.ParentMenuId,
            //               children =new BLInfo<SMIM_Menu_ST>().GetQuerable<SMIM_Menu_ST>().Where(a=>a.ParentMenuId == sa.MenuId && tabIds.Contains(sa.RightId)).Select(c=>new
            //               {
            //                   c.MenuId,
            //                   name = c.MenuDescription,
            //                   state = c.urlPath,
            //                   icon = c.IconPath,
            //                   type = "link",
            //               }),

            //           }).ToList();//.OrderBy(a => a.FormName).ToList();
            #endregion

            var lst = (from sa in obj
                       select new
                       {
                           sa.MenuId,
                           name = sa.MenuDescription,
                           state = sa.urlPath,
                           icon = sa.IconPath,
                           type = "link",

                           sa.ParentMenuId,

                           sa.MenuSeq
                       }).Select(a => new
                       {

                           a.MenuId,
                           a.name,
                           a.state,
                           a.icon,
                           a.type,
                           a.MenuSeq,
                           //!string.IsNullOrEmpty(ab.urlPath)
                           children = new BLInfo<SMIM_Menu_ST>().GetQuerable<SMIM_Menu_ST>().Where(ab => ab.ParentMenuId == a.MenuId && tabIds.Contains(ab.RightId) && (ab.urlPath != "" || ab.urlPath != null)).Select(c => new
                           {
                               c.MenuId,
                               name = c.MenuDescription,
                               state = c.urlPath,
                               icon = c.IconPath.ToString().Trim(),
                               type = "link",
                               c.MenuSeq
                           }).OrderBy(b => b.MenuSeq),
                       }).OrderBy(o => o.MenuSeq).ToList();

            return this.SuccessResponse(lst);
        }



        public DTO SubmitMenu(SMIM_Menu_ST obj)
        {
            try
            {
                if (obj.MenuId != 0)
                    GetByPrimaryKey(obj.MenuId);

                if (PrimaryKeyValue == null)
                    New();

                Current.MenuType = obj.MenuType;
                Current.MenuSeq = obj.MenuSeq;
                Current.ParentMenuId = obj.ParentMenuId;// obj.MenuType == 'P' ? 0 : 1;
                Current.IconPath = obj.IconPath;
                Current.urlPath = obj.MenuType == 'P' ? "" : obj.urlPath;
                Current.RightId = obj.RightId;
                Current.MenuDescription = obj.MenuDescription;
                Current.DefaultValue = obj.DefaultValue;
                Current.CreatedOn = DateTime.Now;
                Current.CreatedBy = obj.CreatedBy;
                //Current.
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

        public DTO DeleteMenu(long Id)
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

public static class Extensions
{
    public static T NewIfEmpty<T>(this T obj) where T : new()
    {
        T returnObj;
        if (obj == null)
            returnObj = new T();
        else
            returnObj = obj;

        return returnObj;
    }
    public static DateTime ToDateTime(this object Value)
    {
        DateTime obj = Convert.ToDateTime(Value);

        return obj;
    }


}