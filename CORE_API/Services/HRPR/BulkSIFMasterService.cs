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

namespace CORE_API.Services.HRPR
{
    public class BulkSIFMasterService : CastService<HRPR_BulkSIFFiles_HI>
    {

        public BulkSIFMasterService()
        {

        }
        //protected override bool IsValidBeforeSave()
        //{
        //    this.Errors.Clear();


        //    var obj = 

        //    if (this.Errors.Count > 0)
        //        return true;
        //    else
        //        return false;
        //}

        public int SubmitBulkSIFFiles(string FileName, int TotalEmployee, int CompanyId, string Month, string Year, string UserId)
        {
            try
            {
                New();
                Current.FileName = FileName;
                Current.CompanyId = CompanyId;
                Current.TotalEmployee = TotalEmployee;
                Current.Month = Month;
                Current.Year = Year;
                Current.CreatedOn = DateTime.Now;
                Current.Createdby = Convert.ToInt32(UserId);

                Save();

                return Current.TFileID;
                //  return this.SuccessResponse("Saved Succesfully");


            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }


    }


}
