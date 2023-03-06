using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.HRMS
{
    public class EmployeeDocumentService : CastService<HRMS_EmpDocs_ST>
    {

        public EmployeeDocumentService()
        {

        }

        public DTO UpdateDocuments(string desc, string docpath, string docname, int? EmpId)
        {
            try
            {



                if (desc != null || desc != "")
                {
                    var obj = new BLInfo<HRMS_EmpDocs_ST>().GetQuerable<HRMS_EmpDocs_ST>().Where(a => a.SMAM_Documents_ST.Description == desc && a.EmpId == EmpId).FirstOrDefault().EmpDocID;
                    GetByPrimaryKey(obj);

                }


                if (PrimaryKeyValue != null)
                {

                    Current.DocPath = docpath;
                    Save();


                }
                return this.SuccessResponse("Saved Successfully.");
            }
            catch (Exception Ex)
            {

                throw Ex;
            }

        }
        public DTO UpdateEmployeeDocument(int EmpId, string EmiratesId, DateTime? IssueDate, DateTime? ExpiryDate, int UserId)
        {
            try
            {
                if (EmpId != 0)
                {
                    var obj = new BLInfo<HRMS_EmpDocs_ST>().GetQuerable<HRMS_EmpDocs_ST>().Where(a => a.EmpId == EmpId && a.DocId == 6 && a.IsActive == 'Y').FirstOrDefault();
                    if (obj != null)
                    {
                        GetByPrimaryKey(obj.EmpDocID);

                        Current.IsActive = 'N';
                        Current.ModifiedBy = UserId;
                        Current.ModifiedOn = DateTime.Now;

                        Save();
                    }
                    AddEmployeeDocument(Current, EmpId, EmiratesId, IssueDate, ExpiryDate, UserId);
                }
                //if (EmpId == 0 || EmpId == null)
                //    PrimaryKeyValue = null;
                //if (PrimaryKeyValue == null)
                //{
                //    New();
                //}

                return SuccessResponse("Saved Successfully");
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }
        public DTO AddEmployeeDocument(HRMS_EmpDocs_ST obj, int EmpId, string EmiratesId, DateTime? IssueDate, DateTime? ExpiryDate, int UserId)
        {
            try
            {

                PrimaryKeyValue = null;
                if (PrimaryKeyValue == null)
                {
                    New();
                }
                Current.DocPath = obj?.DocPath;
                Current.DocId = 6;
                Current.EmpId = obj != null ? obj.EmpId : EmpId;
                Current.IsActive = 'Y';
                Current.DocNo = EmiratesId;
                Current.CreatedBy = UserId;
                Current.CreatedOn = DateTime.Now;
                DateTime? Issue = IssueDate;
                DateTime? Expiry = ExpiryDate;
                Common.DateHandler(ref Issue);
                Common.DateHandler(ref Expiry);
                Current.IssueDate = Issue;
                Current.ExpiryDate = Expiry;
                Save();


                return this.SuccessResponse("Saved Succesfully");


            }

            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return BadResponse(obj);
                else
                    return BadResponse(Ex.Message);
            }
        }
        public DTO SubmitDocuments(IList<HRMS_EmpDocs_ST> model, int EmpId, string employerCustomerId, string CustomerId)
        {
            try
            {


                foreach (var item in model)
                {

                    if (item.EmpDocID != 0)
                    {
                        GetByPrimaryKey(item.EmpDocID);
                    }

                    if (PrimaryKeyValue != null)
                    {
                        Current.ModifiedOn = DateTime.Now;
                        Current.ModifiedBy = item.CreatedBy;
                        Current.IsActive = 'N';
                    }

                    New();

                    if (item.EmpDocID == 0)
                        Current.DocPath = "00 - Portal/" + employerCustomerId + "/Employee Documents/" + CustomerId + "_" + (item.DocPath != null ? item.DocPath.Replace("/", @"\") : "");
                    else
                        Current.DocPath = item.DocPath;

                    Current.DocId = item.DocId;
                    Current.EmpId = EmpId;
                    Current.DocNo = item.DocNo;

                    DateTime? IssueDate = item.IssueDate;
                    DateTime? ExpiryDate = item.ExpiryDate;
                    Common.DateHandler(ref IssueDate);
                    Common.DateHandler(ref ExpiryDate);

                    Current.IssueDate = IssueDate;
                    Current.ExpiryDate = ExpiryDate;
                    Current.IsActive = item.IsActive;
                    Current.CreatedBy = item.CreatedBy;
                    Current.CreatedOn = DateTime.Now;

                    Save();


                }
                return this.SuccessResponse(Current);
            }
            catch (Exception Ex)
            {
                return BadResponse(Ex.Message);
            }
        }
        public DTO AddEmployeeDocumentsToServer(HttpRequest httprequest, int EmpId, string employerCustomerId, string CustomerId)
        {

            try
            {
                string basefilepath = ConfigurationManager.AppSettings["DocPath"].ToString();
                string FolderName = ConfigurationManager.AppSettings["FolderName"].ToString();

                if (httprequest.Files.Count > 0)
                {
                    string FileName = string.Empty;

                    foreach (string file in httprequest.Files)
                    {
                        var postedfile = httprequest.Files[file];
                        FileName = postedfile.FileName.ToString();

                        var filePath = FolderName;

                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }

                        filePath = filePath + @"\" + employerCustomerId;

                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }

                        filePath = filePath + @"\Employee Documents";

                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }


                        filePath = filePath + @"\" + CustomerId + "_" + FileName;
                        postedfile.SaveAs(basefilepath + filePath);
                    }

                    return new DTO { isSuccessful = true, errors = null, data = null };
                }

                return new DTO { isSuccessful = false, errors = null, data = null };
            }
            catch (Exception ex)
            {
                return new DTO { isSuccessful = false, errors = ex.Message, data = null };
            }


        }

    }
}


public class EmpDocRequestModel
{
    public int EmpId { get; set; }
    public string EmiratesId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int UserId { get; set; }
}