using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace CORE_API.Services.SMAM
{
    public class DocumentService : CastService<SMAM_CompanyDocs_ST>
    {

        public DocumentService()
        {

        }

        public DTO UpdateDocuments(string desc, string docpath, string docname, int? CompanyId)
        {
            try
            {

                if (desc != null || desc != "")
                {
                    var obj = new BLInfo<SMAM_CompanyDocs_ST>().GetQuerable<SMAM_CompanyDocs_ST>().Where(a => a.SMAM_Documents_ST.Description == desc && a.CompanyId == CompanyId).FirstOrDefault().CmpDocId;
                    GetByPrimaryKey(obj);
                }
                if (PrimaryKeyValue != null)
                {
                    Current.DocPath = docpath;
                    Current.FileName = docname;
                    Save();


                }
                return this.SuccessResponse("Saved Successfully.");
            }
            catch (Exception Ex)
            {

                throw Ex;
            }

        }

        public DTO SubmitDocuments(IList<SMAM_CompanyDocs_ST> obj, int CompanyId)
        {
            try
            {


                foreach (var item in obj)
                {


                    if (item.CmpDocId != null && item.CmpDocId != 0)
                    {
                        GetByPrimaryKey(item.CmpDocId);

                    }
                    if (item.CmpDocId == null || item.CmpDocId == 0)
                        PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                    {
                        New();
                        Current.CreatedBy = item.CreatedBy;
                        Current.CreatedOn = DateTime.Now;
                    }


                    Current.DocId = item.DocId;
                    Current.CompanyId = CompanyId;
                    Current.DocNo = item.DocNo;

                    DateTime? IssueDate = item.IssueDate;
                    DateTime? ExpiryDate = item.ExpiryDate;
                    Common.DateHandler(ref IssueDate);
                    Common.DateHandler(ref ExpiryDate);
                    Current.IssueDate = IssueDate;
                    Current.ExpiryDate = ExpiryDate;
                    //  Current.IsActive = item.IsActive;
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

        public DTO AddCompanyDocuments(HttpRequest httprequest, int company_id, string customer_Id)
        {

            try
            {
                //string basefilepath = HttpContext.Current.Server.MapPath("~/SFTP/00 - Portal/");  // @"E:\SFTP";

                string basefilepath = ConfigurationManager.AppSettings["DocPath"].ToString();

                if (httprequest.Files.Count > 0)
                {
                    string FileName = "";

                    foreach (string file in httprequest.Files)
                    {
                        var postedfile = httprequest.Files[file];
                        FileName = postedfile.FileName.ToString();

                        var filePath = "00 - Portal";

                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }

                        filePath = filePath + @"\" + customer_Id;

                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }

                        filePath = filePath + @"\Company Documents";
                        if (filePath != null)
                        {
                            if (!Directory.Exists(basefilepath + filePath))
                            {
                                Directory.CreateDirectory(basefilepath + filePath);
                            }
                        }

                        filePath = filePath + @"\" + customer_Id + "_" + FileName;
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

        public DTO SubmitDocumentNew(IList<SMAM_CompanyDocs_ST> obj, int CompanyId, string CustomerId)
        {
            try
            {
                foreach (var item in obj)
                {
                    if (item.CmpDocId != null && item.CmpDocId != 0)
                    {
                        GetByPrimaryKey(item.CmpDocId);

                    }
                    if (item.CmpDocId == null || item.CmpDocId == 0)
                        PrimaryKeyValue = null;
                    if (PrimaryKeyValue == null)
                    {
                        New();
                        Current.CreatedBy = item.CreatedBy;
                        Current.CreatedOn = DateTime.Now;
                    }

                    Current.DocId = item.DocId;
                    Current.CompanyId = CompanyId;
                    Current.DocNo = item.DocNo;
                    Current.DocPath = "00 - Portal/" + CustomerId + "/Company Documents/" + CustomerId + "_" + item.DocPath;
                    Current.FileName = Path.ChangeExtension(item.DocPath, null);
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

                return new DTO { isSuccessful = true, errors = null, data = null };

            }
            catch (Exception Ex)
            {
                if (Errors.Count > 0)
                    return BadResponse(Errors);
                else
                    return BadResponse(Ex.Message);
            }
        }

        public DTO DocumentView()
        {
            try
            {

                return this.SuccessResponse("");
            }
            catch (Exception ex)
            {

                return this.BadResponse(ex.Message);
            }
        }

        public DTO updateCompanyDocumentDetails(HttpRequest httprequest, List<SMAM_CompanyDocs_ST> model, int companyId, string CustomerId)
        {
            try
            {
                if (model != null && model.Count > 0)
                {
                    foreach (var item in model)
                    {

                        GetByPrimaryKey(item.CmpDocId);

                        if (PrimaryKeyValue != null && item.CmpDocId != 0)
                        {
                            Current.CompanyId = companyId;
                            Current.DocId = item.DocId;
                            Current.DocNo = item.DocNo;
                            DateTime? IssueDate = item.IssueDate;
                            DateTime? ExpiryDate = item.ExpiryDate;
                            Common.DateHandler(ref IssueDate);
                            Common.DateHandler(ref ExpiryDate);
                            Current.IssueDate = IssueDate;
                            Current.ExpiryDate = ExpiryDate;
                            Current.IsActive = item.IsActive;
                            Current.ModifiedBy = item.CreatedBy;
                            Current.ModifiedOn = DateTime.Now;
                            Current.LicenseName = item.LicenseName;
                        }

                        if (PrimaryKeyValue == null && item.CmpDocId == 0)
                        {
                            var companyDocs = new BLInfo<SMAM_CompanyDocs_ST>()
                                            .GetQuerable<SMAM_CompanyDocs_ST>()
                                            .Where(a => a.DocId == item.DocId &&
                                                    a.CompanyId == companyId &&
                                                    a.ExpiryDate <= DateTime.Now).ToList();
                            foreach (var docs in companyDocs)
                            {
                                GetByPrimaryKey(docs.CmpDocId);
                                if (PrimaryKeyValue != null && docs.CmpDocId != 0)
                                {
                                    Current.IsActive = 'N';
                                    Current.ModifiedBy = item.CreatedBy;
                                    Current.ModifiedOn = DateTime.Now;
                                }
                                Save();
                            }

                            New();
                            Current.CompanyId = companyId;
                            Current.DocNo = item.DocNo;
                            Current.DocPath = "00 - Portal/" + CustomerId + "/Company Documents/" + CustomerId + "_" + item.DocPath;
                            Current.DocId = item.DocId;
                            Current.FileName = CustomerId + "_" + Path.ChangeExtension(item.DocPath, null);
                            DateTime? IssueDate = item.IssueDate;
                            DateTime? ExpiryDate = item.ExpiryDate;
                            Common.DateHandler(ref IssueDate);
                            Common.DateHandler(ref ExpiryDate);
                            Current.IssueDate = IssueDate;
                            Current.ExpiryDate = ExpiryDate;
                            Current.IsActive = item.IsActive;
                            Current.CreatedBy = item.CreatedBy;
                            Current.CreatedOn = DateTime.Now;
                            Current.LicenseName = item.LicenseName;
                        }
                        Save();

                    }

                    if (httprequest.Files.Count > 0)
                    {
                        AddCompanyDocuments(httprequest, companyId, CustomerId);
                    }

                }

                return SuccessResponse("");
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }
    }
}
