using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static CORE_API.Areas.HRMS.Models.UnprocesessedParameters;

namespace CORE_API.Services.HRMS
{
    public class EmployeeService : CastService<HRMS_EmployeeMst_ST>
    {

        dbEngine db = new dbEngine();
        public EmployeeService()
        {

        }

        public DTO UpdateMobileNo(string MobileNo, int EmpId)
        {
            try
            {

                if (EmpId != 0)
                    GetByPrimaryKey(EmpId);
                if (EmpId == 0 || EmpId == null)
                    PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                {
                    New();
                }
                bool flag = true;
                var isMobileAlreadyExistInEmpMst = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(a => a.EmpId != EmpId && a.MobileNo==MobileNo).FirstOrDefault();
                if (isMobileAlreadyExistInEmpMst != null) 
                {
                    Current.MobileNo = "971500000000";
                    flag = false;
                    Logger.TraceService("Mobile no updated with default Number Exist In HRMS_EmployeeMst_ST " + MobileNo, "DeliveryLogInfo");
                }
                var getEmployeeEmiratesId= new BLInfo<HRMS_EmpDocs_ST>().GetQuerable<HRMS_EmpDocs_ST>().Where(a => a.EmpId == EmpId && a.DocId==6).FirstOrDefault();
                if (getEmployeeEmiratesId != null) 
                {
                    var checkUserLogin = db._dbEngineContext.SRCC_UserLogin_ST.FirstOrDefault(x=> x.EmiratesId!=getEmployeeEmiratesId.DocNo && x.MobileNo==MobileNo);
                    if (checkUserLogin != null) 
                    {
                        Current.MobileNo = "971500000000";
                        flag = false;
                        Logger.TraceService("Mobile no updated with default Number Exist In SRCC_UserLogin_ST " + MobileNo, "DeliveryLogInfo");
                    }
                }
                if (flag)
                    Current.MobileNo = MobileNo;

                Save();
                Logger.TraceService("Mobile no updated " + MobileNo, "DeliveryLogInfo");
                return SuccessResponse("Updated Mobile no Successfully");
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }
        public DTO UpdateMobileNoValidation(int EmpId, string WalletId, string MobileNo, string EmiratesId)
        {

            string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsetting"].ToString();
            string[] paramater = { "WalletId", "MobileNo", "EmiratesId" };
            DataTable dt = ExecuteStoreProc("SP_HRMS_ValidateMobile", paramater, Connection, WalletId, MobileNo, EmiratesId);
          
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {

                    int id = Convert.ToInt32(row["ReturnId"]);
                    string Msg = row["ReturnMsg"].ToString().Trim();
                 
                    if (id == 0 && Msg.Trim().ToLower().Equals("mobileno available"))
                    {


                        var objData = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(a => a.EmpId == EmpId).FirstOrDefault();

                        string[] paramater_logs = { "p_EmpId", "p_NewMobNo", "p_OldMobNo" };
                         using (SqlConnection con = new SqlConnection(Connection))
                        {
                            using (SqlCommand cmd
                                = new SqlCommand("dbo.SP_HRMS_UpdateMobileNo_Log", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@p_EmpId", EmpId);
                                cmd.Parameters.AddWithValue("@p_NewMobNo", MobileNo);
                                cmd.Parameters.AddWithValue("@p_OldMobNo", objData.MobileNo);
                                if (con.State == ConnectionState.Closed)
                                    con.Open();
                                int a = cmd.ExecuteNonQuery();
                            }
                        }
                        
                    }
                    else
                    {
                        return new CORE_BASE.CORE.DTO { isSuccessful = false, errors = Msg, data = null };
                    }
                }
            }
            return this.SuccessResponse(dt);
        }
        public DTO GetEmployeeType()
        {
            var obj = new BLInfo<HRMS_EmpType_ST>().GetQuerable<HRMS_EmpType_ST>().Where(a => a.EmpTypeId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.EmpTypeId,
                           sa.Description,
                           sa.Title

                       }).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO GetEmployeeStatus()
        {
            var obj = new BLInfo<HRMS_EmpStatus_ST>().GetQuerable<HRMS_EmpStatus_ST>().Where(a => a.EmpStatusId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.EmpStatusId,
                           sa.Description,
                           sa.Title

                       }).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO UpdateEmployee(HRMS_EmployeeMst_ST employeemodel, string establishmentId)
        {
            try
            {

                if (employeemodel.EmpId != 0)


                if (employeemodel.PersonalNo.Length != 14)
                    return BadResponse($"Personal number must be 14 digits against {establishmentId} EstablishmentNumber for company id {employeemodel.CompanyId}");


                if (!string.IsNullOrEmpty(establishmentId))
                    {
                        var _companyEst = new BLInfo<SMAM_CompanyEst_HI>().GetQuerable<SMAM_CompanyEst_HI>().FirstOrDefault(x => x.Employer_ID == establishmentId && x.CompanyId == employeemodel.CompanyId);
                        if (_companyEst == null)
                            return BadResponse($"Establishment not found against {establishmentId} EstablishmentNumber for company id {employeemodel.CompanyId}");

                        if (employeemodel.PersonalNo != "00000000000000")
                        {
                            var empEstablishments = new BLInfo<EmployeeEstablishmentLinking>().GetQuerable<EmployeeEstablishmentLinking>().Where(x => x.EstablishmentNumber == establishmentId).ToList();
                            if (empEstablishments.Count > 0)
                            {
                                var empIds = empEstablishments.Select(x => x.EmpId).ToList();
                                var employees = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(x => empIds.Contains(x.EmpId)).ToList();
                                var isUpdateEmployee = employees.FirstOrDefault(x => x.EmpId != employeemodel.EmpId && x.PersonalNo == employeemodel.PersonalNo);
                                if (isUpdateEmployee != null)
                                    return BadResponse($"MOL already exists against {establishmentId} EstablishmentNumber for company id {employeemodel.CompanyId}");
                            }
                        }
                    }

                GetByPrimaryKey(employeemodel.EmpId);

                Current.PersonalNo = employeemodel.PersonalNo;
                Current.EmpCode = employeemodel.EmpCode;
                Current.FirstName = employeemodel.FirstName;
                Current.MiddleName = employeemodel.MiddleName;
                Current.LastName = employeemodel.LastName;
                Current.DisplayName = employeemodel.DisplayName;
                Current.PositionId = employeemodel.PositionId;
                Current.NationalityId = employeemodel.NationalityId;
                Current.Gender = employeemodel.Gender;
                Current.MaritalStatus = employeemodel.MaritalStatus;
                Current.DOB = employeemodel.DOB;
                Current.DOJ = employeemodel.DOJ;
                Current.EmpStatusId = employeemodel.EmpStatusId;
                Current.EmpTypeId = employeemodel.EmpTypeId;
                Current.IsActive = employeemodel.IsActive;
                Current.ModifiedBy = employeemodel.ModifiedBy;
                Current.ModifiedOn = DateTime.Now;


                //Contact Details | Home
                Current.CountryId = employeemodel.CountryId;
                Current.StateId = employeemodel.StateId;
                //  Current.MobileNo = employeemodel.MobileNo; // mobile No will Not be change from webPortal edit Employee Direcly from now
                Current.Email = employeemodel.Email;
                Current.ZipCode = employeemodel.ZipCode;
                Current.Address = employeemodel.Address;

                //Contact Details | Work
                Current.WorkCountryId = employeemodel.WorkCountryId;
                Current.WorkStateId = employeemodel.WorkStateId;
                Current.WorkPhoneNo = employeemodel.WorkPhoneNo;
                Current.WorkZipCode = employeemodel.WorkZipCode;
                Current.WorkAddress = employeemodel.WorkAddress;

                Save();

                return this.SuccessResponse(Current);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO GetEmployeeProduct()
        {
            var obj = new BLInfo<SMAM_Products_ST>().GetQuerable<SMAM_Products_ST>().Where(a => a.ProductId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.ProductId,
                           sa.ProductCode,
                           sa.ShortName,
                           sa.Description

                       }).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO GetPositions()
        {
            var obj = new BLInfo<HRMS_Positions_ST>().GetQuerable<HRMS_Positions_ST>().Where(a => a.PositionId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.PositionId,
                           sa.Description,
                           sa.Title

                       }).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO GetNationality()
        {
            var obj = new BLInfo<HRMS_Nationality_ST>().GetQuerable<HRMS_Nationality_ST>().Where(a => a.NationalityId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.NationalityId,
                           sa.Description,

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO GetBank()
        {
            var obj = new BLInfo<SMAM_Banks_ST>().GetQuerable<SMAM_Banks_ST>().Where(a => a.BankId != 0).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.BankId,
                           sa.BankName,

                       }).ToList();
            return this.SuccessResponse(lst);
        }
        public DTO GetEmployeebyCompany(int? CompanyId)
        {
            var obj = CompanyId != null ? new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(a => a.EmpId != null && a.CompanyId == CompanyId).ToList()
                : new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(a => a.EmpId != null).ToList();
            var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != null).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.EmpId,
                           sa.EmpStatusId,
                           sa.EmpTypeId,
                           sa.CompanyId,
                           sa.BankId,
                           sa.NationalityId,
                           sa.PositionId,
                           sa.IBAN,
                           sa.PersonalNo,
                           sa.EmpCode,
                           sa.DisplayName,
                           sa.FirstName,
                           sa.MiddleName,
                           sa.LastName,
                           Employeename = sa.FirstName + " " + sa.MiddleName + " " + sa.LastName,
                           GenderText = (sa.Gender == 'M' || sa.Gender == 'm') ? "Male" : (sa.Gender == 'F' || sa.Gender == 'f') ? "Female" : "Unknown",
                           sa.Gender,
                           sa.DOB,
                           sa.DOJ,
                           CreatedDateString = sa.CreatedOn != null ? sa.CreatedOn.Value.ToString("dd/MM/yyyy") : null,
                           DOBString = sa.DOB != null ? sa.DOB.Value.ToString("dd/MM/yyyy") : null,
                           DOJString = sa.DOJ != null ? sa.DOJ.Value.ToString("dd/MM/yyyy") : null,
                           sa.MaritalStatus,
                           sa.IsActive,
                           sa.CreatedBy,
                           CreatedByName = objUser.Where(a => a.UserId == sa.CreatedBy).FirstOrDefault().NewIfEmpty().UserName,
                           sa.Address,
                           sa.Email,
                           sa.MobileNo,
                           sa.AltMobileNo,
                           sa.CountryId,
                           sa.StateId,
                           sa.ZipCode,
                           sa.WorkAddress,
                           sa.WorkCountryId,
                           sa.WorkPhoneNo,
                           sa.WorkStateId,
                           sa.WorkZipCode,
                           sa.ImgPath,
                           sa.ProductId



                       }).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO GetEmployee()
        {
            var obj = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(a => a.EmpId != null).ToList();
            var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != null).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.EmpId,
                           sa.EmpStatusId,
                           sa.EmpTypeId,
                           sa.CompanyId,
                           sa.BankId,
                           sa.NationalityId,
                           sa.PositionId,
                           sa.IBAN,
                           sa.PersonalNo,
                           sa.EmpCode,
                           sa.DisplayName,
                           sa.FirstName,
                           sa.MiddleName,
                           sa.LastName,
                           Employeename = sa.FirstName + " " + sa.MiddleName + " " + sa.LastName,
                           GenderText = (sa.Gender == 'M' || sa.Gender == 'm') ? "Male" : (sa.Gender == 'F' || sa.Gender == 'f') ? "Female" : "Unknown",
                           sa.Gender,
                           sa.DOB,
                           sa.DOJ,
                           CreatedDateString = sa.CreatedOn != null ? sa.CreatedOn.Value.ToString("dd/MM/yyyy") : null,
                           DOBString = sa.DOB != null ? sa.DOB.Value.ToString("dd/MM/yyyy") : null,
                           DOJString = sa.DOJ != null ? sa.DOJ.Value.ToString("dd/MM/yyyy") : null,
                           sa.MaritalStatus,
                           sa.IsActive,
                           sa.CreatedBy,
                           CreatedByName = objUser.Where(a => a.UserId == sa.CreatedBy).FirstOrDefault().NewIfEmpty().UserName,
                           sa.Address,
                           sa.Email,
                           sa.MobileNo,
                           sa.AltMobileNo,
                           sa.CountryId,
                           sa.StateId,
                           sa.ZipCode,
                           sa.WorkAddress,
                           sa.WorkCountryId,
                           sa.WorkPhoneNo,
                           sa.WorkStateId,
                           sa.WorkZipCode,
                           sa.ImgPath,
                           sa.ProductId



                       }).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO GetEmployeeById(int EmpId)
        {
            var obj = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(a => a.EmpId == EmpId).ToList();
            var objUser = new BLInfo<SMIM_UserMst_ST>().GetQuerable<SMIM_UserMst_ST>().Where(a => a.UserId != null).ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.EmpId,
                           sa.CustomerId,
                           EmpCampId = 0,
                           sa.EmpStatusId,
                           sa.EmpTypeId,
                           sa.CompanyId,
                           sa.BankId,
                           sa.NationalityId,
                           sa.PositionId,
                           sa.IBAN,
                           sa.PersonalNo,
                           sa.EmpCode,
                           sa.DisplayName,
                           sa.FirstName,
                           sa.MiddleName,
                           sa.LastName,
                           Employeename = sa.FirstName + " " + sa.MiddleName + " " + sa.LastName,
                           GenderText = (sa.Gender == 'M' || sa.Gender == 'm') ? "Male" : (sa.Gender == 'F' || sa.Gender == 'f') ? "Female" : "Unknown",
                           sa.Gender,
                           sa.DOB,
                           sa.DOJ,
                           CreatedDateString = sa.CreatedOn != null ? sa.CreatedOn.Value.ToString("dd/MM/yyyy") : null,
                           DOBString = sa.DOB != null ? sa.DOB.Value.ToString("dd/MM/yyyy") : null,
                           DOJString = sa.DOJ != null ? sa.DOJ.Value.ToString("dd/MM/yyyy") : null,
                           sa.MaritalStatus,
                           sa.IsActive,
                           sa.CreatedBy,
                           CreatedByName = objUser.Where(a => a.UserId == sa.CreatedBy).FirstOrDefault().NewIfEmpty().UserName,
                           sa.Address,
                           sa.Email,
                           sa.MobileNo,
                           sa.AltMobileNo,
                           sa.CountryId,
                           sa.StateId,
                           sa.ZipCode,
                           sa.WorkAddress,
                           sa.WorkCountryId,
                           sa.WorkPhoneNo,
                           sa.WorkStateId,
                           sa.WorkZipCode,
                           sa.ImgPath,
                           sa.ProductId
                       }).FirstOrDefault();
            return this.SuccessResponse(lst);
        }

        public DTO GetEmployeeDocuments(int EmpId)
        {

            string[] paramater = { "empId" };
            DataTable dt = ExecuteSp("sp_GetEmployeeDocuments", paramater, EmpId);
            return this.SuccessResponse(dt);
        }

        public DTO SPGetEmployeebyCompany(int CompanyId)
        {

            string[] paramater = { "CompanyId" };
            DataTable dt = ExecuteSp("sp_GetEmployeebyCompanyId", paramater, CompanyId);
            return this.SuccessResponse(dt);
        }

        public DTO SPGetEmployeeById(int EmpId)
        {

            string[] paramater = { "EmpId" };
            DataTable dt = ExecuteSp("SP_SPGetEmployeeById", paramater, EmpId);
            return this.SuccessResponse(dt);
        }

        public DTO SPgetEmployeeTrans(string CustomerId, int AccountTypeId)
        {
            try
            {
                string[] paramater = { "CustomerId", "AccountTypeId" };
                string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();
                DataTable dt = ExecuteStoreProc("SP_GetEmployeeTrans", paramater, Connection, CustomerId, AccountTypeId);
                return new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = dt };
            }
            catch (Exception ex)
            {
                return new CORE_BASE.CORE.DTO { isSuccessful = false, errors = ex.Message, data = null };
            }

        }

        public DTO SPGetEmpScreeningTimeline(int? EmpId)
        {
            try
            {
                string[] paramater = { "EmpId" };
                DataTable dt = ExecuteSp("SP_GetScreeningTimeline", paramater, EmpId);
                return new CORE_BASE.CORE.DTO { isSuccessful = true, errors = null, data = dt };
            }
            catch (Exception ex)
            {
                return new CORE_BASE.CORE.DTO { isSuccessful = false, errors = ex.Message, data = null };
            }

        }

        public DTO SP_GetEmpbyCompprod(int CompanyId, int ProductId, string ScreenType)
        {
            string[] paramater = { "CompanyId", "ProductId", "ScreenType" };
            DataTable dt = ExecuteSp("SP_GetEmpbyCompAndprod", paramater, CompanyId, ProductId, ScreenType);
            return this.SuccessResponse(dt);
        }

        public DTO Get_CompbyProduct(int CompanyId)
        {
            try
            {
                string[] paramater = { "CompanyId" };
                DataTable dt = ExecuteSp("SP_GetCompProduct", paramater, CompanyId);
                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }

        }

        public DTO GetEmployeeCharges(int CmpChrgId)
        {

            string[] paramater = { "empId" };
            DataTable dt = ExecuteSp("sp_GetEmployeeCharges", paramater, CmpChrgId);
            return this.SuccessResponse(dt);
        }

        public DTO SubmitEmployee(HRMS_EmployeeMst_ST obj, string establishmentId)
        {
            try
            {
                int? newSeqNo;
                string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsetting"].ToString();

                if (obj.PersonalNo.Length!=14)
                    return BadResponse($"Personal number must be 14 digits against {establishmentId} EstablishmentNumber for company id {obj.CompanyId}");

                if (!string.IsNullOrEmpty(establishmentId))
                {
                    
                    var _companyEst = new BLInfo<SMAM_CompanyEst_HI>().GetQuerable<SMAM_CompanyEst_HI>().FirstOrDefault(x => x.Employer_ID == establishmentId && x.CompanyId == obj.CompanyId);
                    if (_companyEst == null)
                    {
                        return BadResponse($"Establishment not found against {establishmentId} EstablishmentNumber for company id {obj.CompanyId}");
                    }

                    if (obj.PersonalNo != "00000000000000")
                    {
                        var empEstablishments = new BLInfo<EmployeeEstablishmentLinking>().GetQuerable<EmployeeEstablishmentLinking>().Where(x => x.EstablishmentNumber == establishmentId).ToList();
                        if (empEstablishments.Count > 0)
                        {
                            var empIds = empEstablishments.Select(x => x.EmpId).ToList();
                            var employees = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(x => empIds.Contains(x.EmpId)).ToList();
                            if (employees.Any(x => x.PersonalNo == obj.PersonalNo))
                                return BadResponse($"MOL already exists against {establishmentId} EstablishmentNumber for company id {obj.CompanyId}");
                        }
                    }
                }

                int? productCode = new BLInfo<SMAM_CompanyProducts_ST>()
                                        .GetQuerable<SMAM_CompanyProducts_ST>()
                                        .Where(o => o.ProductId == obj.ProductId && 
                                                o.CompanyId == obj.CompanyId)
                                        .Select(o => o.ProductCode)
                                        .FirstOrDefault();

                if (productCode == null)
                    return BadResponse("Product Code Not Found");

                int? existSeqNo = new BLInfo<HRMS_EMPGUID_NEW>()
                                        .GetQuerable<HRMS_EMPGUID_NEW>()
                                        .Where(o => o.ProductId == productCode)
                                        .Select(o => o.SeqNo)
                                        .Max()
                                        .NewIfEmpty();

                newSeqNo = existSeqNo != null ? existSeqNo + 1 : 1;

                //if (obj.EmpId != 0)
                //    GetByPrimaryKey(obj.EmpId);

                New();
                Current.CompanyId = obj.CompanyId;
                Current.PersonalNo = obj.PersonalNo;
                Current.EmpCode = obj.EmpCode;
                Current.Title = obj.Title;
                Current.FirstName = obj.FirstName;
                Current.MiddleName = obj.MiddleName;
                Current.LastName = obj.LastName;
                Current.DisplayName = obj.DisplayName;
                Current.MotherName = obj.MotherName;
                Current.ImgPath = obj.ImgPath;
                Current.Gender = obj.Gender;
                Current.DOB = obj.DOB;
                Current.DOJ = obj.DOJ;
                Current.MaritalStatus = obj.MaritalStatus;
                Current.EmpStatusId = obj.EmpStatusId;
                Current.EmpTypeId = obj.EmpTypeId;
                Current.PositionId = obj.PositionId;
                Current.NationalityId = obj.NationalityId;
                Current.IsActive = obj.IsActive;
                Current.CreatedBy = obj.CreatedBy;
                Current.CreatedOn = obj.CreatedOn;
                Current.BankId = obj.BankId;
                Current.IBAN = obj.IBAN;
                Current.MobileNo = obj.MobileNo;
                Current.AltMobileNo = obj.AltMobileNo;
                Current.Address = obj.Address;
                Current.StateId = obj.StateId;
                Current.CountryId = obj.CountryId;
                Current.ZipCode = obj.ZipCode;
                Current.WorkAddress = obj.WorkAddress;
                Current.WorkStateId = obj.WorkStateId;
                Current.WorkCountryId = obj.WorkCountryId;
                Current.WorkZipCode = obj.WorkZipCode;
                Current.ProductId = obj.ProductId;
                Current.CustomerId = "KP" + productCode.ToString() + newSeqNo;
                Current.Email = obj.Email;
                Current.ApprovedBy = obj.ApprovedBy;
                Current.ApprovedOn = obj.ApprovedOn;
                Current.IsArchive = obj.IsArchive;
                Save();

                if (Current.EmpId != 0)
                {

                    #region CreateCif gainst Employee
                    var objData = new BLInfo<CifMaster>().GetQuerable<CifMaster>().OrderByDescending(a => a.Id).FirstOrDefault();
                    string lastUserCifNumber = objData == null ? "10000000" : objData.CifNumber.ToString();
                    var StatusId = new BLInfo<CifStatus>().GetQuerable<CifStatus>().FirstOrDefault(x => x.Title == Enums.CifStatuses.ACTIVE.ToString());

                    string queryForHRMS_EMPGUID_NEW = "INSERT INTO dbo.HRMS_EMPGUID_NEW (CusId, SeqNo,EmpId,ProductId) " +
                       " output INSERTED.CusId VALUES (@CusId, @SeqNo, @EmpId,@ProductId)";

                    string queryForCifMaster = "INSERT INTO dbo.CifMaster (CifNumber, StatusId,UserCategoryCode,CreatedOn) " +
                       " output INSERTED.ID VALUES (@CifNumber, @StatusId,@UserCategoryCode,@CreatedOn)";

                    string queryForlinkEmployee = "INSERT INTO dbo.CifEmployeeLinkings (CifId, EmplId) " +
                       "VALUES (@CifId, @EmpId)";

                    int CifId = 0;
                    string CusId = "";

                    using (SqlConnection con = new SqlConnection(Connection))
                    {

                        using (SqlCommand cmd = new SqlCommand(queryForHRMS_EMPGUID_NEW, con))
                        {
                            cmd.Parameters.Add("@CusId", SqlDbType.VarChar).Value = Current.CustomerId;
                            cmd.Parameters.Add("@SeqNo", SqlDbType.Int).Value = newSeqNo;
                            cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = Current.EmpId;
                            cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productCode;
                            if (con.State == ConnectionState.Closed)
                                con.Open();
                            CusId = (string)cmd.ExecuteScalar();
                            con.Close();
                        }

                        using (SqlCommand cmd = new SqlCommand(queryForCifMaster, con))
                        {
                            cmd.Parameters.Add("@CifNumber", SqlDbType.NVarChar, 12).Value = Common.GenerateCifNumber(lastUserCifNumber);
                            cmd.Parameters.Add("@StatusId", SqlDbType.Int).Value = StatusId.CifStatusId;
                            cmd.Parameters.Add("@UserCategoryCode", SqlDbType.NVarChar).Value = Enums.UserCategoryCode.Employee;
                            cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                            if (con.State == ConnectionState.Closed)
                                con.Open();
                            CifId = (int)cmd.ExecuteScalar();
                            con.Close();
                        }
                        using (SqlCommand cmdlinkmployee = new SqlCommand(queryForlinkEmployee, con))
                        {
                            cmdlinkmployee.Parameters.Add("@CifId", SqlDbType.Int).Value = CifId;
                            cmdlinkmployee.Parameters.Add("@EmpId", SqlDbType.Int).Value = Current.EmpId;
                            if (con.State == ConnectionState.Closed)
                                con.Open();
                            cmdlinkmployee.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    #endregion
                }
                return this.SuccessResponse(Current);
            }
            catch (Exception Ex)
            {
                //if (Errors.Count > 0)
                //    return BadResponse(obj);
                //else
                return BadResponse(Ex.Message);
            }
        }

        public DTO GetDocument()
        {
            var obj = new BLInfo<SMAM_Documents_ST>().GetQuerable<SMAM_Documents_ST>().Where(o => o.DocType == 'E').ToList();
            var lst = (from sa in obj
                       select new
                       {
                           sa.DocId,
                           sa.Description,
                           sa.DocType,

                       }).ToList();
            return this.SuccessResponse(lst);
        }

        public DTO AddEmployee(HRMS_EmployeeMst_ST obj)
        {
            try
            {

                var Product = new BLInfo<SMAM_Products_ST>().GetQuerable<SMAM_Products_ST>().Where(o => o.ProductId == obj.ProductId).FirstOrDefault();
                var EmpCount = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().ToList();

                var comp = new BLInfo<SMAM_CompanyMst_ST>().GetQuerable<SMAM_CompanyMst_ST>().Where(o => o.CompanyId == obj.CompanyId).FirstOrDefault();
                var seqno = EmpCount.Count + 1;
                var CustomerId = "KP" + Product.ProductCode + seqno;

                New();
                Current.CompanyId = obj.CompanyId;
                Current.PersonalNo = obj.PersonalNo;
                Current.EmpCode = obj.EmpCode;
                Current.EmpTypeId = obj.EmpTypeId;
                Current.EmpStatusId = obj.EmpStatusId;
                Current.FirstName = obj.FirstName;
                Current.MiddleName = obj.MiddleName;
                Current.LastName = obj.LastName;
                Current.DisplayName = obj.DisplayName;
                Current.Gender = obj.Gender;
                Current.MaritalStatus = obj.MaritalStatus;
                Current.PositionId = obj.PositionId;
                Current.NationalityId = obj.NationalityId;
                Current.IsActive = obj.IsActive;
                Current.DOB = obj.DOB;
                Current.DOJ = obj.DOJ;
                Current.Address = obj.Address;
                Current.Email = obj.Email;
                Current.Title = obj.Title;
                Current.MobileNo = obj.MobileNo;
                Current.StateId = obj.StateId;
                Current.CountryId = obj.CountryId;
                Current.ZipCode = obj.ZipCode;
                Current.ImgPath = obj.ImgPath;
                Current.ProductId = obj.ProductId;
                Current.AltMobileNo = obj.AltMobileNo;

                Current.WorkAddress = obj.WorkAddress;
                Current.WorkPhoneNo = obj.WorkPhoneNo;
                Current.WorkStateId = obj.WorkStateId;
                Current.WorkCountryId = obj.WorkCountryId;
                Current.WorkZipCode = obj.WorkZipCode;

                Current.CustomerId = CustomerId;
                Current.CreatedBy = obj.CreatedBy;
                Current.CreatedOn = DateTime.Now;
                Save();




                return new CORE_BASE.CORE.DTO
                {
                    isSuccessful = true,
                    errors = null,
                    data = new
                    {
                        EmpId = Current.EmpId,
                        CustomerId = Current.CustomerId,
                        CompCustomerId = comp.CustomerId,
                    }
                };

            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }

        public DTO GetEmployeeDetails(int CompanyId, int ProductId, string EmpCode, string MobileNo)
        {

            try
            {
                if (CompanyId != null && ProductId != null)
                {
                    string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();
                    string[] paramater = { "CompanyId", "ProductId", "EmpCode", "MobileNo" };
                    DataTable dt = ExecuteStoreProc("SP_GetEmployeeDetails", paramater, Connection, CompanyId, ProductId, EmpCode, MobileNo);

                    return this.SuccessResponse(dt);
                }

                return this.SuccessResponse("");
            }
            catch (Exception ex)
            {
                Logger.TraceService(ex.Message, "SP_GetEmployeeDetails");
                return this.BadResponse(ex.Message);
            }

        }

        public DTO SPGetEmployeebymultiFilter(UpdateEmployeeArchive obj)
        {

            try
            {
                string[] paramater = { "UserId", "CompanyId", "ProductId", "EmpCode", "WalletId", "CustomerId", "IsActive", "IsArchive", "CardActivated" };
                DataTable dt = ExecuteSp("SP_GetEmployeebymultiFilter", paramater, obj.UserId, obj.CompanyId, obj.ProductId, obj.EmpCode, obj.WalletId, obj.CustomerId, obj.IsActive, obj.IsArchive,obj.CardActivated);

                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO UpdateEmployeeArchive(List<UpdateEmployeeArchive> obj)
        {

            try
            {
                foreach (var item in obj)
                {
                    string[] paramater = { "UserId", "EmpId", "IsArchive" };
                    DataTable dt = ExecuteSp("SP_UpdateEmployeeArchive", paramater, item.UserId, item.EmpId, item.IsArchive);
                }
                return this.SuccessResponse("Update Successfully..!");

            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }


        }

        private bool AddCustomerId(int EmpId, int? ProductId, int? CompanyId, int? NewSeqNo, int? ProductCode)
        {
            try
            {
                GetByPrimaryKey(EmpId);
                Current.CustomerId = "KP" + ProductCode.ToString() + NewSeqNo;
                Save();

                return true;
            }
            catch (Exception ex)
            {


                return false;
            }
        }

        public DTO GetAllEmployee(int pageNumber, int numberofRecord)
        {
            try
            {

                string[] paramater = { "pageNumber", "NumberofRecord" };
                DataTable employeeList = ExecuteSp("SP_GetAllEmployee", paramater, pageNumber, numberofRecord);

                return this.SuccessResponse(employeeList);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO AddEmployeeToEst(HttpFileCollection files, Stream stream)
        {
            try
            {
                HashSet<string> responses = new HashSet<string>();
                if (stream == null)
                    return this.BadResponse("stream not found for excel sheet");

                var excel = new ExcelPackage(stream);
                DataTable dt = Common.ExcelSheetsToDataTable(excel);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["connsetting"].ToString();

                    if (PrimaryKeyValue == null)
                        New();

                    foreach (DataRow item in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(item["EmpCode"].ToString()))
                        {
                            var emp = new BLInfo<HRMS_EmployeeMst_ST>().GetQuerable<HRMS_EmployeeMst_ST>().Where(o => o.EmpCode == item["EmpCode"].ToString()).FirstOrDefault();

                            if (emp != null)
                            {
                                var empLink = new BLInfo<EmployeeEstablishmentLinking>().GetQuerable<EmployeeEstablishmentLinking>().Where(o => o.EstablishmentNumber == item["EstablishmentNumber"].ToString() && emp.EmpId == o.EmpId).FirstOrDefault();
                                if (empLink == null)
                                {
                                    string queryForlinkEmployee = "INSERT INTO dbo.EmployeeEstablishmentLinking (EmpId, EstablishmentNumber,CreatedOn) " +
                                  " VALUES (@EmpId, @EstablishmentNumber,@CreatedOn)";

                                    using (SqlConnection con = new SqlConnection(Connection))
                                    {
                                        using (SqlCommand cmdlinkmployee = new SqlCommand(queryForlinkEmployee, con))
                                        {
                                            cmdlinkmployee.Parameters.Add("@EmpId", SqlDbType.Int).Value = emp.EmpId;
                                            cmdlinkmployee.Parameters.Add("@EstablishmentNumber", SqlDbType.NVarChar).Value = item["EstablishmentNumber"].ToString();
                                            cmdlinkmployee.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                                            if (con.State == ConnectionState.Closed)
                                                con.Open();
                                            cmdlinkmployee.ExecuteNonQuery();
                                            con.Close();
                                        }
                                    }
                                    responses.Add($"Employee code {item["EmpCode"].ToString()} successfully linked for  {emp.CustomerId} customer");
                                }
                                else
                                {
                                    responses.Add($"Employee {item["EmpCode"].ToString()} code already exists against {item["EstablishmentNumber"].ToString()} Establishment Number");
                                }
                            }
                            else
                            {
                                responses.Add($"Employee code not found for {item["EmpCode"].ToString()} code");
                            }
                        }
                    }
                }

                return this.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO AddEmployeeToEstablishmentId(string establishmentId, HRMS_EmployeeMst_ST currentEmployee)
        {


            var empEstabLinkingExits = db._dbContext.EmployeeEstablishmentLinkings.Where(o => o.EmpId == currentEmployee.EmpId).FirstOrDefault();

            if (empEstabLinkingExits != null)
                empEstabLinkingExits.EstablishmentNumber = establishmentId;

            else
            {
                EmployeeEstablishmentLinking current = new EmployeeEstablishmentLinking();
                current.EmpId = currentEmployee.EmpId;
                current.EstablishmentNumber = establishmentId;
                current.CreatedOn = DateTime.Now;
                db._dbContext.EmployeeEstablishmentLinkings.InsertOnSubmit(current);
            }
            db._dbContext.SubmitChanges();
            return this.SuccessResponse("Employee Establishment Save");
        }
    }
}
