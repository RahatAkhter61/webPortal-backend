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
    public class StationService : CastService<SMAM_Stations_ST>
    {

        public StationService()
        {

        }


        public DTO GetStations()
        {
            var obj = new BLInfo<SMAM_Stations_ST>().GetQuerable<SMAM_Stations_ST>().Where(a => a.StationId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.StationId,
                           sa.StationName,
                           sa.StationType,
                           sa.PhoneNo,
                           sa.Address,
                           sa.CountryID,
                           sa.StateID,
                           sa.CompanyId
                       }).ToList();
            return this.SuccessResponse(lst);
        }






        public int SubmitStation(SMAM_Stations_ST obj)
        {
            try
            {
                if (obj.CompanyId != 0)
                    GetByPrimaryKey(obj.CompanyId);

                   if (PrimaryKeyValue == null)
                    {
                        New();
                        Current.CreatedBy = obj.CreatedBy;
                        Current.CreatedOn = DateTime.Now;
                   }

                   Current.CompanyId = obj.CompanyId;
                    Current.CountryID = obj.CountryID;
                    Current.StateID = obj.StateID;
                    Current.StationName = obj.StationName;
                    Current.StationType = obj.StationType;
                    Current.PhoneNo = obj.PhoneNo;
                    Current.ZipCode = obj.ZipCode;
                    Current.Address = obj.Address;
                    Save();

                    return Current.StationId;
                
                //return this.SuccessResponse("Saved Succesfully");
            }
            catch (Exception Ex)
            {
                //if (Errors.Count > 0)
                //    return BadResponse(obj);
                //else
                //    return BadResponse(Ex.Message);

                throw Ex;
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

