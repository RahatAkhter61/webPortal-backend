using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORE_API.Services.SALE
{
    public class SaleTrackerService : CastService<SALES_DealComment>
    {
        public SaleTrackerService()
        { }

        public DTO GetAllDealStage()
        {
            var objlist = new BLInfo<SALES_DealStageMaster>().GetQuerable<SALES_DealStageMaster>().Where(a => a.IsActive == true).ToList();
            if (objlist.Count > 0)
            {
                return this.SuccessResponse(objlist);
            }
            return this.BadResponse("Not Found");
        }

        public DTO GetbyuserId(int UserId)
        {
            try
            {
                var objcomany = new BLInfo<SALES_DealCompany>().GetQuerable<SALES_DealCompany>().Where(a => a.Createdby == UserId).ToList();
                return SuccessResponse(objcomany);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO SubmitSalestracker(SALES_DealComment obj)
        {
            try
            {
                New();
                Current = obj;
                Save();
                return this.SuccessResponse("Saved Succesfully");
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO UpdateSaletracker(SALES_DealComment DTO)
        {
            try
            {

                var obj = new BLInfo<SALES_DealComment>().GetQuerable<SALES_DealComment>().Where(a => a.CompanyId == DTO.CompanyId).FirstOrDefault();

                if (obj != null)
                    GetByPrimaryKey(obj.CommentId);

                if (PrimaryKeyValue != null)
                {
                    if (Current.SALES_DealCompany != null)
                    {
                        Current.SALES_DealCompany.ContactNo = DTO.SALES_DealCompany.ContactNo;
                        Current.SALES_DealCompany.ContactPerson = DTO.SALES_DealCompany.ContactPerson;
                        Current.SALES_DealCompany.Email = DTO.SALES_DealCompany.Email;
                        Current.SALES_DealCompany.PotentialAccount = DTO.SALES_DealCompany.PotentialAccount;
                        Current.SALES_DealCompany.Modifyon = DTO.SALES_DealCompany.Modifyon;
                    }
                }

                New();
                Current.CompanyId = DTO.CompanyId;
                Current.Comments = DTO.Comments;
                Current.MeetingDate = DTO.MeetingDate;
                Current.Createdby = DTO.Createdby;
                Current.DealStage = DTO.DealStage;
                Current.Createdon = DTO.Createdon;
                Current.Modifyon = DTO.Modifyon;
                Save();
                return this.SuccessResponse("Update Succesfully");
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO Get_AllSalesAccounts()
        {

            try
            {
                var list = new BLInfo<SMAM_SalesAccounts_ST>().GetQuerable<SMAM_SalesAccounts_ST>().ToList();
                return this.SuccessResponse(list);
            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }
            
        }
    }
}