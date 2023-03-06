
using ContextMapper;
using CORE_API.General;
using CORE_API.Models.ComapnyWhiteListing;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static CORE_API.General.Enums;

namespace CORE_API.Services.SMAM
{
    public class CompanyWhitelistingService : CastService<SMAM_CompanyWhitelisting>
    {

        dbEngine db = new dbEngine();

        public CompanyWhitelistingService()
        {

        }

        public DTO AddEditCompanyWhitelisting(IList<CompanyWhileListing> model)
        {
            try
            {
                if (model.Count == 0)
                    return BadResponse("Record not Selected");

                List<SMAM_CompanyWhitelisting> companyWhileListing = new List<SMAM_CompanyWhitelisting>();
                List<CompanyLoanBracket> companyLoanBracket = new List<CompanyLoanBracket>();

                #region Company White List
                for (int i = 0; i < model.Count; i++)
                {

                    if (model[i].Id != 0)
                        GetByPrimaryKey(model[i].Id);


                    if (PrimaryKeyValue == null)
                    {

                        var companyWhileListingEntry = new SMAM_CompanyWhitelisting();
                        companyWhileListingEntry.StatusId = model[i].StatusId;
                        companyWhileListingEntry.CompanyId = model[i].CompanyId;
                        companyWhileListingEntry.tmlStatusId = model[i].tmlStatus;
                        companyWhileListingEntry.CreatedBy = model[i].CreatedBy;
                        companyWhileListingEntry.CreatedOn = DateTime.Now;
                        companyWhileListing.Add(companyWhileListingEntry);

                        foreach (var item in model[i].CompanyLoanBracketModel)
                        {
                            var companyLoanBracketEntry = new CompanyLoanBracket();

                            companyLoanBracketEntry.CompanyId = model[i].CompanyId;
                            companyLoanBracketEntry.CreatedBy = model[i].CreatedBy;
                            companyLoanBracketEntry.CreatedOn = DateTime.Now;
                            companyLoanBracketEntry.FromAmount = item.FromAmount;
                            companyLoanBracketEntry.ToAmount = item.ToAmount;
                            companyLoanBracketEntry.Fees = item.Fees;
                            companyLoanBracket.Add(companyLoanBracketEntry);
                        }

                    }
                    if (PrimaryKeyValue != null)
                    {
                        var companyWhiteListing = db._dbContext.SMAM_CompanyWhitelistings.Where(o => o.Id == model[i].Id).FirstOrDefault();
                        companyWhiteListing.StatusId = model[i].StatusId;
                        companyWhiteListing.tmlStatusId = model[i].tmlStatus;
                        companyWhiteListing.ModifiedBy = model[i].CreatedBy;
                        companyWhiteListing.ModifiedOn = DateTime.Now;
                    }
                }
                #endregion

                db._dbContext.SMAM_CompanyWhitelistings.InsertAllOnSubmit(companyWhileListing);
                db._dbContext.CompanyLoanBrackets.InsertAllOnSubmit(companyLoanBracket);
                db._dbContext.SubmitChanges();

                return this.SuccessResponse("Saved Succesfully");
            }
            catch (Exception Ex)
            {
                Logger.TraceService(Ex.Message + Ex, "Company White Listing");
                return BadResponse(Ex.Message);
            }


        }

        public DTO AddEditCompanySalaryBracket(IList<CompanyLoanBracketModel> companyLoanBracket, int companyId)
        {

            var requestSalaryBracket = companyLoanBracket.Where(o => o.BracketId == 0).ToList();

            if (requestSalaryBracket != null && requestSalaryBracket.Count > 4)
                return BadResponse("Maximum Add Four Company Salary Bracket Records");

            List<CompanyLoanBracket> companyLoanBracketModal = new List<CompanyLoanBracket>();
            var checkLoanBrackets = db._dbContext.CompanyLoanBrackets.Where(o => o.CompanyId == companyId).ToList();


            for (int i = 0; i < companyLoanBracket.Count; i++)
            {

                var item = companyLoanBracket[i];
                var companyLoanBracketEntry = new CompanyLoanBracket();

                if (item.BracketId == 0)
                {
                    companyLoanBracketEntry.FromAmount = item.FromAmount;
                    companyLoanBracketEntry.ToAmount = item.ToAmount;
                    companyLoanBracketEntry.Fees = item.Fees;
                    companyLoanBracketEntry.CompanyId = companyId;
                    companyLoanBracketEntry.CreatedBy = item.CreatedBy;
                    companyLoanBracketEntry.CreatedOn = DateTime.Now;
                    companyLoanBracketModal.Add(companyLoanBracketEntry);
                }

                if (item.BracketId > 0)
                {
                    checkLoanBrackets[i].FromAmount = item.FromAmount;
                    checkLoanBrackets[i].ToAmount = item.ToAmount;
                    checkLoanBrackets[i].Fees = item.Fees;
                }
            }

            db._dbContext.CompanyLoanBrackets.InsertAllOnSubmit(companyLoanBracketModal);
            db._dbContext.SubmitChanges();

            return SuccessResponse("Update Successfully");
        }

        public DTO GetAllCompanyNonWhitelisting()
        {

            var getAllCompanyNonWhitelisting = (from t1 in db._dbContext.SMAM_CompanyMst_STs
                                                join t2 in db._dbContext.SMAM_CompanyWhitelistings on t1.CompanyId equals t2.CompanyId into ps_jointable
                                                join t3 in db._dbContext.SMAM_CmpStatus_STs on t1.CmpStatusId equals t3.CmpStatusId into temp

                                                from p1 in ps_jointable.DefaultIfEmpty()
                                                from p2 in temp.DefaultIfEmpty()

                                                where p1.Id == null

                                                select new
                                                {
                                                    t1.CompanyId,
                                                    CompanyName = t1.Name,
                                                    t1.EstId,
                                                    t1.CustomerId,
                                                    p2.Description
                                                }).ToList();

            if (getAllCompanyNonWhitelisting.Count == 0)
                return BadResponse("Company Not Available");

            return SuccessResponse(getAllCompanyNonWhitelisting);
        }

        public DTO GetCompanyWhitelisting(int userId, int? pageNo, int limit)
        {

            var roleName = (from t1 in db._dbContext.SMIM_UserMst_STs
                            join t2 in db._dbContext.SMIM_Roles_STs on t1.RoleId equals t2.RoleId
                            where t1.UserId == userId &&
                            t2.RoleName.Equals(Enums.AdvanceSalaryStatus.ComplianceView)
                            select new
                            {
                                t2.RoleName,
                            }).FirstOrDefault();

            string[] paramater = { "pageNumber", "NumberofRecord" };
            DataTable getCompanyWhitelisting = ExecuteSp("SP_GetCompanyWhitelisting", paramater, pageNo, limit);

            int totatRecord = getCompanyWhitelisting.Rows.Count;

            if (totatRecord == 0)
                return BadResponse("No Record Available");

            var response = new
            {
                roleName = roleName,
                companyWhitelisting = getCompanyWhitelisting,

            };

            return SuccessResponse(response);
        }

        public DTO CompanyWhitelistStatus()
        {


            var companyLiersStatus = (from t1 in db._dbContext.StatusTypes
                                      join t2 in db._dbContext.Status on t1.StatustypeId equals t2.TypeId into ps_jointable
                                      from p in ps_jointable.DefaultIfEmpty()
                                      where t1.Name.Equals(Enums.CompanyWhiteListing.CompanyLiers)
                                      select new
                                      {
                                          p.Name,
                                          p.StatusId
                                      }).ToList();

            var companyWhiteListStatus = (from t1 in db._dbContext.StatusTypes
                                          join t2 in db._dbContext.Status on t1.StatustypeId equals t2.TypeId into ps_jointable
                                          from p in ps_jointable.DefaultIfEmpty()
                                          where t1.Name.Equals(Enums.CompanyWhiteListing.CompanyWhitelisting)
                                          select new
                                          {
                                              p.Name,
                                              p.StatusId
                                          }).ToList();


            if (companyWhiteListStatus.Count == 0)
                return BadResponse("company White Status Not Available");

            var response = new
            {
                companyLiersStatus = companyLiersStatus,
                companyWhiteListStatus = companyWhiteListStatus,

            };

            return SuccessResponse(response);

        }

        public DTO CompanySalaryBracket(int companyId)
        {


            var companySalaryBracket = (from t1 in db._dbContext.CompanyLoanBrackets
                                        join t2 in db._dbContext.SMAM_CompanyMst_STs on t1.CompanyId equals t2.CompanyId
                                        where t1.CompanyId == companyId
                                        select new
                                        {
                                            t1.BracketId,
                                            t1.FromAmount,
                                            t1.ToAmount,
                                            t1.Fees,
                                            t1.CompanyId,
                                            t2.Name,
                                        }).ToList();

            return SuccessResponse(companySalaryBracket);
        }
    }


}