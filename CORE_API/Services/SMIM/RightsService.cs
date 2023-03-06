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
    public class RightsService : CastService<SMIM_Rights_ST>
    {

        public RightsService()
        {

        }
        protected override bool IsValidBeforeSave()
        {

            this.Errors.Clear();

            var obj = new BLInfo<SMIM_Rights_ST>().GetQuerable<SMIM_Rights_ST>().Where(a => a.FormName == this.Current.FormName && a.RightName == this.Current.RightName && a.RightId != this.Current.RightId);

            if (obj.Count() > 0)
                this.Errors.Add(obj.FirstOrDefault().RightName + "rights is already exist against " + obj.FirstOrDefault().FormName);

            if (this.Errors.Count > 0)
                return true;
            else
                return false;


        }

        public DTO GetRights()
        {
            var obj = new BLInfo<SMIM_Rights_ST>().GetQuerable<SMIM_Rights_ST>().Where(a => a.RightId != 0).ToList();

            var lst = (from sa in obj
                       select new
                       {
                           sa.RightId,
                           sa.RightName,
                           sa.FormName
                       }).OrderBy(a=>a.FormName).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO GetRightsByForm(string FormName)
        {
            var obj = new BLInfo<SMIM_Rights_ST>().GetQuerable<SMIM_Rights_ST>().Where(a => a.RightId != 0 && a.FormName == FormName).ToList();

            var lst = (from sa in obj
                       select new
                       {
                           sa.RightId,
                           sa.RightName,
                           sa.FormName
                       }).OrderBy(a => a.FormName).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO GetForms()
        {
            var obj = new BLInfo<SMIM_Rights_ST>().GetQuerable<SMIM_Rights_ST>().GroupBy(u => u.FormName).Select(f =>  f.Key).ToList();

            //var lst = (from sa in obj
            //           select new
            //           {
                          
            //               sa.FormName
            //           }).GroupBy(a => a.FormName).ToList();
            return this.SuccessResponse(obj);
        }

       

        public DTO SubmitRights(SMIM_Rights_ST obj)
        {
            try
            {
                if (obj.RightId != 0)
                    GetByPrimaryKey(obj.RightId);

                if (PrimaryKeyValue == null)
                    New();

                Current.FormName = obj.FormName;
                Current.RightName = obj.RightName;
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

