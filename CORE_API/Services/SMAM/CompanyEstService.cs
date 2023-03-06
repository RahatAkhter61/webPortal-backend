using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CORE_API.Services.SMAM
{
    public class CompanyEstService : CastService<SMAM_CompanyEst_HI>
    {

        public DTO SubmitCompanyEstbl(SMAM_CompanyEst_HI obj)
        {
            try
            {
                if (obj == null)
                    return this.BadResponse("Error");

                string[] paramater = { "userId", "CompanyId", "Employer_ID", "Employer_Name" };
                DataTable dt = ExecuteSp("SP_InsertCompanyEsblist", paramater, obj.CreatedBy, obj.CompanyId, obj.Employer_ID, obj.Employer_Name);
                return this.SuccessResponse("Saved Succesfully");

            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO SP_Get_CompanyEsblst(int userId)
        {
            try
            {
                string[] paramater = { "userId" };
                DataTable dt = ExecuteSp("SP_GetCompanyEsblist", paramater, userId);
                return this.SuccessResponse(dt);
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO Update_Status(IList<SMAM_CompanyEst_HI> list)
        {
            try
            {

                if (list == null && list.Count == 0)
                    this.BadResponse("Please Select At least one record");

                foreach (var obj in list)
                {
                    if (obj.Id != 0)
                        GetByPrimaryKey(obj.Id);

                    if (PrimaryKeyValue != null)
                    {
                        Current.IsGenerated = true;
                        Current.ModifiedOn = DateTime.Now;
                        Save();
                    }
                }
                return this.SuccessResponse("Generate Successful..!");

            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO DeleteCompanyEstbl(long id)
        {
            try
            {

                GetByPrimaryKey(id);
                if (Current != null)
                    PrimaryKeyValue = id;
                Delete();

                return this.SuccessResponse("Deleted Successfully.");
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }

        public DTO EditCompanyEstbl(SMAM_CompanyEst_HI model)
        {
            try
            {

                if (model.Id != 0)
                    GetByPrimaryKey(model.Id);

                if (PrimaryKeyValue != null)
                {
                    Current.Employer_ID = model.Employer_ID;
                    Current.Employer_Name = model.Employer_Name;
                    Current.ModifiedOn = DateTime.Now;
                    Current.ModifiedBy = model.CreatedBy;

                    Save();
                }
                return this.SuccessResponse("Update Successfully.");
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }


    }
}