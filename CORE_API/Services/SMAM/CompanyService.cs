using ContextMapper;
using CORE_API.General;
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
    public class CompanyService : CastService<SMAM_CompanyMst_ST>
    {

        dbEngine db = new dbEngine();
        public CompanyService()
        {

        }

        public DTO GetCampStatus()
        {
            var obj = new BLInfo<SMAM_CmpStatus_ST>().GetQuerable<SMAM_CmpStatus_ST>().Where(a => a.CmpStatusId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.CmpStatusId,
                           sa.Description,
                           sa.Title

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO GetCountry()
        {
            var obj = new BLInfo<SMAM_Country_ST>().GetQuerable<SMAM_Country_ST>().Where(a => a.CountryId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.CountryId,
                           sa.Name,

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO GetStates()
        {
            var obj = new BLInfo<SMAM_States_ST>().GetQuerable<SMAM_States_ST>().Where(a => a.StateId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.StateId,
                           sa.StateName,
                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO GetSalesAccount()
        {
            var obj = new BLInfo<SMAM_SalesAccounts_ST>().GetQuerable<SMAM_SalesAccounts_ST>().Where(a => a.SalesAccountId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.SalesAccountId,
                           sa.Name,
                           sa.Email
                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO GetChargesType()
        {
            var AllCharges = new BLInfo<SMAM_ChargesType_ST>().GetQuerable<SMAM_ChargesType_ST>().Select
                (o => new
                {
                    o.ChrgTypeId,
                    o.Type,
                    o.Title,
                    o.Code,
                    o.Description,
                }).ToList();

            var sifcharges = new BLInfo<SMAM_ChargesType_ST>().GetQuerable<SMAM_ChargesType_ST>().Where(a =>
            a.ChrgTypeId != 0 && (a.Title.Equals(Enums.Chargetypes.PerFile) || a.Title == Enums.Chargetypes.PerRecord)).Select(o => new
            {
                o.ChrgTypeId,
                o.Type,
                o.Title,
                o.Code,
                o.Description,
            }).ToList();


            var result = new
            {
                sifcharges = sifcharges,
                allcharges = AllCharges
            };
            return this.SuccessResponse(result);

        }
        public DTO GetCompanyCharges(int CmpChrgId)
        {
            //var obj = new BLInfo<SMAM_CompanyCharges_ST>().GetQuerable<SMAM_CompanyCharges_ST>().Where(a => a.CmpChrgId == CmpChrgId).ToList();
            //var lst = (from sa in obj
            //           select new
            //           {
            //               sa.CmpChrgId,
            //               sa.Charges,
            //               sa.ChrgTypeId,
            //               ChargeType = sa.SMAM_ChargesType_ST != null ? sa.SMAM_ChargesType_ST.Type : null,
            //               sa.CompanyId,
            //               Company = sa.SMAM_CompanyMst_ST != null ? sa.SMAM_CompanyMst_ST.Name : "",
            //               sa.EffFrom,
            //               sa.EffTo,
            //               EffFromDate = sa.EffFrom.Value.ToString("dd/MM/yyyy"),
            //               EffToDate = sa.EffTo.Value.ToString("dd/MM/yyyy"),

            //           }).ToList();

            string[] paramater = { "companyId" };
            DataTable dt = ExecuteSp("sp_GetCompanyCharges", paramater, CmpChrgId);
            return this.SuccessResponse(dt);

        }
        public DTO GetCompanyDocs(int CmpDocId)
        {
            var obj = new BLInfo<SMAM_CompanyDocs_ST>().GetQuerable<SMAM_CompanyDocs_ST>().Where(a => a.CmpDocId == CmpDocId).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.CmpDocId,
                           sa.DocId,
                           sa.DocNo,
                           sa.DocPath,
                           sa.IssueDate,
                           sa.ExpiryDate,
                           ExpiryDateString = sa.ExpiryDate != null ? sa.ExpiryDate.Value.ToString("dd/MM/yyyy") : null,
                           IssueDateString = sa.IssueDate != null ? sa.IssueDate.Value.ToString("dd/MM/yyyy") : null,
                           sa.IsActive

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO GetCompanybyUser(int UserId)
        {

            int? companyId = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId == UserId).FirstOrDefault().NewIfEmpty().CompanyId;

            var obj = companyId != null ? new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == companyId).ToList() :
               new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId != null).ToList();

            var lst = (from sa in obj
                       select new
                       {
                           ParentCompanystxt = sa.ParentCompanyId != null ? new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(o => o.CompanyId == sa.ParentCompanyId).Select(o => o.Name).FirstOrDefault() : "",
                           sa.CompanyId,
                           sa.Address,
                           sa.Title,
                           sa.Name,
                           sa.EstId,
                           CountryName = sa.CountryId != null ? new BLInfo<SMAM_Country_ST>().GetQuerable<SMAM_Country_ST>().Where(o => o.CountryId == sa.CountryId).Select(o => o.Name).FirstOrDefault() : "",
                           StateName = sa.StateId != null ? new BLInfo<SMAM_States_ST>().GetQuerable<SMAM_States_ST>().Where(o => o.StateId == sa.StateId).Select(o => o.StateName).FirstOrDefault() : "",
                           CreatedByName = sa.CreatedBy != null ? new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(o => o.UserId == sa.CreatedBy).Select(o => o.UserName).FirstOrDefault() : "",
                           PayRollId = sa.PayRollId == 2 ? "WPS" : "Non-WPS",
                           isFreeZone = sa.isFreeZone == true ? "Yes" : "No",
                           isMigrated = sa.isMigrated == true ? "Yes" : "No",
                           isSMSRequired = sa.isSMSRequired == true ? "Yes" : "No",
                           sa.Balance

                       }).OrderByDescending(o => o.CompanyId).ToList();

            return this.SuccessResponse(lst);
        }
        public DTO GetCompanyDataById(int CompanyId)
        {
            var complinaceDetail = (from t1 in db._dbContext.SMAM_CompanyBatch_HIs
                                    join t2 in db._dbContext.SMAM_CompanyBatchDtl_HIs on t1.BatchId equals t2.BatchId
                                    join t3 in db._dbContext.SMAM_CmpStatus_STs on t2.StatusId equals t3.CmpStatusId
                                    where t2.CompanyId == CompanyId &&
                                    (t3.Title.Equals(Enums.CompanyStatus.New) || t3.Title.Equals(Enums.CompanyStatus.CKF) || t3.Title.Equals(Enums.CompanyStatus.CKP))

                                    select new
                                    {
                                        t1.BatchId,
                                        ComplianceDate = t1.CreatedOn,
                                        ComplianceStatus = t3.Description

                                    }).OrderByDescending(o => o.BatchId).FirstOrDefault();


            var companyDetail = (from t1 in db._dbContext.SMAM_CompanyMst_STs
                                 join t2 in db._dbContext.SMIM_UserMst_STs on t1.CreatedBy equals t2.UserId into userMst
                                 from user in userMst.DefaultIfEmpty()

                                 join t3 in db._dbContext.SMAM_SalesAccounts_STs on t1.SalesAccountId equals t3.SalesAccountId into saleAcc
                                 from sal in saleAcc.DefaultIfEmpty()

                                 join t4 in db._dbContext.SMAM_Country_STs on t1.CountryId equals t4.CountryId into country
                                 from cou in country.DefaultIfEmpty()

                                 join t5 in db._dbContext.SMAM_States_STs on t1.StateId equals t5.StateId into state
                                 from stat in state.DefaultIfEmpty()

                                 join t6 in db._dbContext.SMAM_CmpStatus_STs on t1.CmpStatusId equals t6.CmpStatusId into campstatus
                                 from camp in campstatus.DefaultIfEmpty()

                                 where t1.CompanyId == CompanyId

                                 select new
                                 {
                                     t1.CustomerId,
                                     t1.ParentCompanyId,
                                     t1.CompanyId,
                                     t1.Address,
                                     t1.SalesAccountId,
                                     t1.MailAddress,
                                     t1.MailBuildingNo,
                                     t1.Email,
                                     t1.ZipCode,
                                     t1.BusinessName,
                                     t1.PhoneNo,
                                     t1.MailCountryId,
                                     t1.MailStateId,
                                     t1.MailZipCode,
                                     t1.BuildingNo,
                                     t1.isSMSRequired,
                                     t1.isMigrated,
                                     t1.PayRollId,
                                     t1.isFreeZone,
                                     t1.Title,
                                     t1.Name,
                                     t1.EstId,
                                     t1.FirstName,
                                     t1.MiddleName,
                                     t1.LastName,
                                     t1.LoginId,
                                     t1.StateId,
                                     t1.CountryId,
                                     t1.CmpStatusId,
                                     t1.CreatedBy,
                                     CreatedDateString = t1.CreatedOn,
                                     ProductName = t1.PayRollId == 2 ? "WPS" : "Non WPS",
                                     SalesAccountName = sal.Name,
                                     CountryName = cou.Name,
                                     stat.StateName,
                                     CreatedByName = user.UserName,
                                     CampStatusName = camp.Description

                                 }).FirstOrDefault();

            var result = new
            {
                complinaceDetail = complinaceDetail,
                companyDetail = companyDetail
            };


            return this.SuccessResponse(result);
        }
        public DTO GetCompanyDocuments(int CompanyId)
        {

            string[] paramater = { "companyId" };
            DataTable dt = ExecuteSp("sp_GetCompanyDocuments", paramater, CompanyId);
            return this.SuccessResponse(dt);
        }
        public DTO SubmitCompany(SMAM_CompanyMst_ST obj)
        {
            try
            {
                if (obj.Name != null && obj.Name.Trim().Length > 40)
                    return BadResponse("Employer Name Max 40 Character");

                if (obj.BusinessName != null && obj.BusinessName.Trim().Length > 40)
                    return BadResponse("Business Name Max 40 Character");

                string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsetting"].ToString();
                if (obj != null && obj.CompanyId != null && obj.CompanyId > 0)
                    GetByPrimaryKey(obj.CompanyId);

                if (PrimaryKeyValue == null)
                {
                    New();
                    Current.CreatedBy = obj.CreatedBy;
                    Current.CreatedOn = DateTime.Now;
                }
                else
                {
                    Current.ModifiedBy = obj.CreatedBy;
                    Current.ModifiedOn = DateTime.Now;
                }
                Current.SalesAccountId = obj.SalesAccountId;
                Current.ParentCompanyId = obj.ParentCompanyId;
                Current.Title = obj.Title;
                Current.Name = obj.Name;
                Current.EstId = obj.EstId;
                Current.FirstName = obj.FirstName;
                Current.MiddleName = obj.MiddleName;
                Current.LastName = obj.LastName;
                Current.Address = obj.Address;
                Current.StateId = obj.StateId;
                Current.CountryId = obj.CountryId;
                Current.LoginId = obj.LoginId;
                Current.CmpStatusId = obj.CmpStatusId;
                Current.Email = obj.Email;
                Current.PhoneNo = obj.PhoneNo;
                Current.Balance = obj.Balance;
                Current.BusinessName = obj.BusinessName;
                Current.ZipCode = obj.ZipCode;
                Current.MailZipCode = obj.MailZipCode;
                Current.BuildingNo = obj.BuildingNo;
                Current.MailStateId = obj.MailStateId;
                Current.MailCountryId = obj.MailCountryId;
                Current.MailAddress = obj.MailAddress;
                Current.MailBuildingNo = obj.MailBuildingNo;
                Current.isFreeZone = obj.isFreeZone;
                Current.isMigrated = obj.isMigrated;
                Current.IsMigratedCompany = obj.IsMigratedCompany;
                Current.isSMSRequired = obj.isSMSRequired;
                Current.PayRollId = obj.PayRollId;

                Save();

                if (Current.CustomerId == null || string.IsNullOrEmpty(Current.CustomerId))
                {
                    AddCustomerId(Current.CompanyId);
                }
                #region generateCifForCompany
                #region CreateCif gainst Employee
                var objData = new BLInfo<CifMaster>().GetQuerable<CifMaster>().OrderByDescending(a => a.Id).FirstOrDefault();
                string lastUserCifNumber = objData == null ? "10000000" : objData.CifNumber.ToString();
                var StatusId = new BLInfo<CifStatus>().GetQuerable<CifStatus>().FirstOrDefault(x => x.Title == Enums.CifStatuses.ACTIVE.ToString());
                string queryForCifMaster = "INSERT INTO dbo.CifMaster (CifNumber, StatusId,UserCategoryCode,CreatedOn) " +
                   " output INSERTED.ID VALUES (@CifNumber, @StatusId,@UserCategoryCode,@CreatedOn)";
                string queryForlinkEmployee = "INSERT INTO dbo.CifCompanyLinkings (CifId, CompnayId) " +
                   "VALUES (@CifId, @CompanyId)";
                int CifId = 0;

                using (SqlConnection con = new SqlConnection(Connection))
                {


                    using (SqlCommand cmd = new SqlCommand(queryForCifMaster, con))
                    {
                        cmd.Parameters.Add("@CifNumber", SqlDbType.NVarChar, 12).Value = Common.GenerateCifNumber(lastUserCifNumber);
                        cmd.Parameters.Add("@StatusId", SqlDbType.Int).Value = StatusId.CifStatusId;
                        cmd.Parameters.Add("@UserCategoryCode", SqlDbType.NVarChar).Value = Enums.UserCategoryCode.Company;
                        cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                        if (con.State == ConnectionState.Closed)
                            con.Open();
                        CifId = (int)cmd.ExecuteScalar();
                        con.Close();
                    }
                    using (SqlCommand cmdlinkmployee = new SqlCommand(queryForlinkEmployee, con))
                    {
                        cmdlinkmployee.Parameters.Add("@CifId", SqlDbType.Int).Value = CifId;
                        cmdlinkmployee.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Current.CompanyId;
                        if (con.State == ConnectionState.Closed)
                            con.Open();
                        cmdlinkmployee.ExecuteNonQuery();
                        con.Close();
                    }
                }
                #endregion
                #endregion
                return this.SuccessResponse(Current);
            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return BadResponse(obj);
                else
                    return BadResponse(Ex.Message);
            }
        }
        public DTO GetStationsByCompany(int CompantId)
        {
            var obj = new BLInfo<SMAM_Stations_ST>().GetQuerable<SMAM_Stations_ST>().Where(a => a.CompanyId == CompantId).ToList();
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
        public DTO GetParentCompanies()
        {
            var obj = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.ParentCompanyId == null).ToList();

            var lst = (from sa in obj
                       select new
                       {
                           sa.CompanyId,
                           sa.Name,
                           sa.Balance

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO GetDepartmentsByStation(int StationId)
        {
            var obj = new BLInfo<SMAM_Departments_ST>().GetQuerable<SMAM_Departments_ST>().Where(a => a.StationId == StationId).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.DeptId,
                           sa.IsActive,
                           sa.Description,

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO UpdateCompantBalance(int? Id, decimal? Balance)
        {
            try
            {
                //   var obj =  new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(a => a.CompanyId == Id).FirstOrDefault()
                GetByPrimaryKey(Id);
                if (Current != null)
                    PrimaryKeyValue = Id;

                Current.Balance = Balance;
                Save();

                return this.SuccessResponse("Updated Successfully.");


            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return this.BadResponse(Errors);
                else
                    return this.BadResponse(Ex.Message);
            }
        }
        public DTO GetCompanybyEstabl(int CompanyId)
        {

            var obj = new BLInfo<SMAM_CompanyEst_HI>().GetQuerable<SMAM_CompanyEst_HI>().Where(a => a.CompanyId == CompanyId).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           Id = sa.Id,
                           Employer_Name = sa.Employer_Name,
                           IsActive = sa.IsActive,
                           Employer_ID = sa.Employer_ID.ToString().Trim()

                       }).ToList();
            return this.SuccessResponse(lst);

        }
        private bool AddCustomerId(int companyId)
        {
            try
            {
                GetByPrimaryKey(companyId);
                Current.CustomerId = "KPE" + DateTime.Now.Year.ToString() + "00" + Current.CompanyId.ToString();
                Save();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public DTO GetCompanyDocument()
        {
            var obj = new BLInfo<SMAM_Documents_ST>().GetQuerable<SMAM_Documents_ST>().Where(a => a.DocType == 'C').ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.DocId,
                           sa.DocType,
                           sa.Description,
                           sa.Title

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO UpdateCompanyBalance(decimal? amount, int? companyId, int? uploadedBy)
        {
            try
            {
                if (amount > 0 && companyId > 0)
                {
                    GetByPrimaryKey(companyId);
                    if (PrimaryKeyValue == null || Current == null)
                        return BadResponse("Company Does'nt Exist");

                    Current.ModifiedBy = uploadedBy;
                    Current.ModifiedOn = DateTime.Now;
                    Current.Balance = Convert.ToDecimal(Current.Balance != null ? Current.Balance += amount : amount);
                    Save();

                    return SuccessResponse(Current);
                }
                return BadResponse("Something Went Wrong");
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
        public DTO UpdateCompany(SMAM_CompanyMst_ST model)
        {
            try
            {
                GetByPrimaryKey(model.CompanyId);
                if (PrimaryKeyValue == null || Current == null)
                    return BadResponse("Company Does'nt Exist");

                Current.ParentCompanyId = model.ParentCompanyId;
                Current.Title = model.Title;
                Current.EstId = model.EstId;
                Current.Name = model.Name;
                Current.FirstName = model.FirstName;
                Current.MiddleName = model.MiddleName;
                Current.LastName = model.LastName;
                Current.BusinessName = model.BusinessName;
                Current.Email = model.Email;
                Current.PhoneNo = model.PhoneNo;

                Current.CountryId = model.CountryId;
                Current.StateId = model.StateId;
                Current.StateId = model.StateId;
                Current.ZipCode = model.ZipCode;
                Current.Address = model.Address;


                Current.MailCountryId = model.MailCountryId;
                Current.MailStateId = model.MailStateId;
                Current.MailZipCode = model.MailZipCode;
                Current.MailBuildingNo = model.MailBuildingNo;
                Current.MailAddress = model.MailAddress;
                Current.ModifiedBy = model.CreatedBy;
                Current.ModifiedOn = DateTime.Now;

                Current.isFreeZone = model.isFreeZone != null ? model.isFreeZone : false;
                Current.isMigrated = model.isMigrated != null ? model.isMigrated : false;
                Current.IsMigratedCompany = model.IsMigratedCompany != null ? model.IsMigratedCompany : false;
                Current.isSMSRequired = model.isSMSRequired != null ? model.isSMSRequired : false;
                Current.PayRollId = model.PayRollId;

                Save();
                return SuccessResponse(Current);

            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }
        }
        public DTO GetDocumentbyCompanyId(int CompanyId)
        {
            var obj = new BLInfo<SMAM_CompanyDocs_ST>().GetQuerable<SMAM_CompanyDocs_ST>().Where(a => a.CompanyId == CompanyId && a.IsActive == 'Y').ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.CmpDocId,
                           sa.DocId,
                           Description = new BLInfo<SMAM_Documents_ST>().GetQuerable<SMAM_Documents_ST>().Where(o => o.DocId == sa.DocId).FirstOrDefault().Description,
                           sa.DocNo,
                           sa.FileName,
                           sa.IsActive,
                           sa.IssueDate,
                           sa.ExpiryDate,
                           sa.DocPath,
                           sa.LicenseName

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO GetCompany(int? pageNo = 1, int? limit = 1, string customerId = null, string establishmentId = null, bool? isAllCompanies = false)
        {

            if (isAllCompanies == true)
            {
                var allCompanyRecords = db._dbContext.SMAM_CompanyMst_STs.Select(o => new
                {
                    o.CompanyId,
                    o.Name,
                    o.ParentCompanyId,
                    o.PayRollId,
                    o.SalesAccountId,
                    o.Balance,
                    o.CustomerId,
                    o.EstId
                }).ToList();

                return SuccessResponse(allCompanyRecords);
            }
            else
            {
                string[] paramater = { "pageNumber", "NumberofRecord", "customerId", "establishmentId" };
                DataTable employerList = ExecuteSp("SP_GetAllEmployer", paramater, pageNo, limit, customerId, establishmentId);
                return this.SuccessResponse(employerList);
            }

        }
        public DTO GetCompanyDetailByCompanyId(int? companyId)
        {
            var companyDetail = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(o => o.CompanyId == companyId).FirstOrDefault();
            return this.SuccessResponse(companyDetail);
        }
    }
}

