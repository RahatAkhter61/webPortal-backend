using AutoMapper;
using ContextMapper;
using CORE_API.General;
using CORE_API.Models.CompanyHistory;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.SMAM
{
    public class ChargesService : CastService<SMAM_CompanyCharges_ST>
    {

        private readonly CompanyChargesHistoryService _CompanyChargesHistoryService;

        public ChargesService()
        {
            _CompanyChargesHistoryService = new CompanyChargesHistoryService();
        }
        public DTO SubmitCharges(IList<SMAM_CompanyCharges_ST> item, int CompanyId)
        {
            try
            {

                foreach (var obj in item)
                {
                    if (obj.CmpChrgId != 0 && obj.CmpChrgId != null)
                        GetByPrimaryKey(obj.CmpChrgId);
                    if (obj.CmpChrgId == 0 || obj.CmpChrgId == null)
                        PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                    {
                        New();
                        Current.CreatedBy = obj.CreatedBy;
                        Current.CreatedOn = DateTime.Now;
                    }

                    Current.CompanyId = CompanyId;
                    Current.ChrgTypeId = obj.ChrgTypeId;
                    Current.Charges = obj.Charges;
                    Current.ProductId = obj.ProductId;
                    DateTime? EffFrom = obj.EffFrom;
                    DateTime? EffTo = obj.EffTo;
                    Common.DateHandler(ref EffFrom);
                    Common.DateHandler(ref EffTo);
                    Current.EffFrom = EffFrom;
                    Current.EffTo = EffTo;

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


        public DTO GetChargesbyCompanyId(int CompanyId)
        {
            try
            {

                var companyChargesList = (from t1 in new BLInfo<SMAM_CompanyCharges_ST>().GetQuerable<SMAM_CompanyCharges_ST>().ToList()
                                      join t2 in new BLInfo<SMAM_ChargesType_ST>().GetQuerable<SMAM_ChargesType_ST>().ToList() on t1.ChrgTypeId equals t2.ChrgTypeId
                                      join t3 in new BLInfo<SMAM_Products_ST>().GetQuerable<SMAM_Products_ST>().ToList() on t1.ProductId equals t3.ProductId
                                      where t1.CompanyId == CompanyId

                                      select new
                                      {
                                          t1.CmpChrgId,
                                          t1.CompanyId,
                                          t1.ChrgTypeId,
                                          Title = t2.Title,
                                          Description = t2.Description,
                                          t1.ProductId,
                                          Producttxt = t3.ShortName,
                                          t1.Charges,
                                          t1.CreatedBy,
                                          t1.CreatedOn,
                                          t1.EffFrom,
                                          t1.EffTo,


                                      });
             
                return this.SuccessResponse(companyChargesList);
            }
            catch (Exception ex)
            {

                return BadResponse(ex);
            }
        }

        public DTO updateCompanyCharges(List<SMAM_CompanyCharges_ST> model, int CompanyId)
        {
            try
            {

                if (model != null && model.Count > 0)
                {

                    var response = new DTO();
                    var CompanyChargesHistory = new List<CompanyChargesHistory>();

                    //var AddChargesHistory = new List<SMAM_CompanyCharges_ST>();
                    var arror = new ArrayList();

                    foreach (var item in model)
                    {
                        GetByPrimaryKey(item.CmpChrgId);

                        if (PrimaryKeyValue == null || Current == null)
                        {
                            New();
                            Current.CompanyId = CompanyId;
                            Current.ProductId = item.ProductId;
                            Current.ChrgTypeId = item.ChrgTypeId;
                            Current.Charges = item.Charges;
                            Current.EffFrom = item.EffFrom;
                            Current.EffTo = item.EffTo;
                            Current.CreatedBy = item.CreatedBy;
                            Current.CreatedOn = DateTime.Now;
                            Save();
                        }

                        if (PrimaryKeyValue != null)
                        {
                            var exist_rec = new BLInfo<SMAM_CompanyCharges_ST>().GetQuerable<SMAM_CompanyCharges_ST>().Where(o => o.CmpChrgId == Current.CmpChrgId).FirstOrDefault();

                            if (exist_rec != null)
                            {
                                var historyModel = new CompanyChargesHistory
                                {

                                    OldCharges = exist_rec.Charges,
                                    NewCharges = item.Charges,
                                    EffectiveFrom = exist_rec.EffFrom,
                                    EffectiveTo = exist_rec.EffTo,
                                    UpdatedBy = item.CreatedBy,
                                    UpdatedOn = DateTime.Now,
                                    CompanyChargeId = exist_rec.CmpChrgId
                                };
                                CompanyChargesHistory.Add(historyModel);
                            }
                        }
                    }

                    //MapperConfiguration chargesMapper = new MapperConfiguration(cfg => cfg.CreateMap<SMAM_CompanyCharges_ST, CompanyHistoryModel>());
                    ////Mapper.CreateMap<List<SMAM_CompanyCharges_ST>, List<CompanyHistoryModel>>();
                    ////var userDto = Mapper.Map<List<SMAM_CompanyCharges_ST>, List<CompanyHistoryModel>>(CompanyChargesHistory);

                    //var userDto = chargesMapper.CreateMapper().Map<List<CompanyHistoryModel>>(CompanyChargesHistory);

                    if (CompanyChargesHistory != null && CompanyChargesHistory.Count > 0)
                    {
                        response = _CompanyChargesHistoryService.CompanyChargesHistory(CompanyChargesHistory as List<CompanyChargesHistory>);

                        if(response.isSuccessful)
                        {
                            response = CompanyChargesUpdate(model);
                        }
                    }

                    return this.SuccessResponse("");
                }

                return this.SuccessResponse("");
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        private DTO CompanyChargesUpdate(List<SMAM_CompanyCharges_ST> model)
        {

            try
            {
                foreach (var item in model)
                {

                    GetByPrimaryKey(item.CmpChrgId);
                    if (PrimaryKeyValue != null)
                    {
                        Current.Charges = item.Charges;
                        Current.EffFrom = item.EffFrom;
                        Current.EffTo = item.EffTo;
                        Current.ModifiedBy = item.CreatedBy;
                        Current.ModifiedOn = DateTime.Now;
                        Save();
                    }
                }
                return this.SuccessResponse(Current);
            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }
        }

        public DTO ChargesTypebyCompanyandproductid(int companyId, int productId)
        {

            var productbyCompanyChargesType = (from t1 in new BLInfo<SMAM_CompanyCharges_ST>().GetQuerable<SMAM_CompanyCharges_ST>().ToList()

                                               join t2 in new BLInfo<SMAM_ChargesType_ST>().GetQuerable<SMAM_ChargesType_ST>().ToList() on t1.ChrgTypeId equals t2.ChrgTypeId into tempRecord
                                               from t3 in tempRecord.DefaultIfEmpty()
                                               where t1.ProductId == productId && t1.CompanyId == companyId

                                               select new
                                               {
                                                   t3.ChrgTypeId,
                                                   t3.Title,
                                                   t3.Description,
                                                   t1.Charges,
                                               }).ToList();

            return this.SuccessResponse(productbyCompanyChargesType);
        }
    }
}
