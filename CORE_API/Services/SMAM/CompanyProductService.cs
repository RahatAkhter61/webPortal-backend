using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CORE_API.General.Enums;

namespace CORE_API.Services.SMAM
{
    public class CompanyProductService : CastService<SMAM_CompanyProducts_ST>
    {

        public CompanyProductService()
        {

        }

        public DTO AddCompanyProduct(List<SMAM_CompanyProducts_ST> model, int Companyid)
        {

            if (model != null && model.Count > 0)
            {

                foreach (var item in model)
                {
                    New();

                    #region Wps Product Region
                    var productDetail = new BLInfo<SMAM_Products_ST>().GetQuerable<SMAM_Products_ST>().Where(o => o.ProductId == item.ProductId).FirstOrDefault().NewIfEmpty();
                    var productCode = new BLInfo<SMAM_CompanyProducts_ST>().GetQuerable<SMAM_CompanyProducts_ST>().OrderByDescending(a => a.ProductCode).FirstOrDefault(a => a.ProductId == productDetail.ProductId).ProductCode.NewIfEmpty();

                    Current.CompanyId = Companyid;
                    Current.ProductId = productDetail.ProductId;
                    Current.ProductName = productDetail.ProductCode + "_" + Convert.ToInt32(productCode != null ? productCode + 1 : Convert.ToInt32(productDetail.ProductCode + 1)) + "_" + productDetail.ShortName;
                    Current.ProductCode = Convert.ToInt32(productCode != null ? productCode + 1 : Convert.ToInt32(productDetail.ProductCode + 1));
                    Current.IsActive = item.IsActive;
                    Current.EffFrom = item.EffFrom;
                    Current.EffTo = item.EffTo;
                    Current.CreatedBy = item.CreatedBy;
                    Current.CreatedOn = DateTime.Now;
                    Current.PCIFStatus = item.PCIFStatus;
                    #endregion

                    Save();
                }

                return this.SuccessResponse("Company Products Add Successfully");
            }
            else
            {
                return BadResponse("Company products List must not be empty");
            }
        }

        public DTO GetProductbyCompanyId(int CompanyId)
        {
            try
            {
                var obj = new BLInfo<SMAM_CompanyProducts_ST>().GetQuerable<SMAM_CompanyProducts_ST>().Where(a => a.CompanyId == CompanyId).ToList();
                var lst = (from sa in obj
                           select new
                           {
                               sa.CompanyId,
                               sa.ProductId,
                               ProductName = new BLInfo<SMAM_Products_ST>().GetQuerable<SMAM_Products_ST>().Where(a => a.ProductId == sa.ProductId).FirstOrDefault().ShortName,
                               sa.IsActive,
                               sa.CreatedBy,
                               sa.CreatedOn,
                               sa.EffFrom,
                               sa.EffTo,

                           }).ToList();

                return this.SuccessResponse(lst);
            }
            catch (Exception ex)
            {

                return BadResponse(ex);
            }
        }

        public DTO UpdateCompanyProduct(List<SMAM_CompanyProducts_ST> model, int CompanyId)
        {
            try
            {
                if (model != null && model.Count > 0)
                {
                    var wpsProduct = new BLInfo<SMAM_Products_ST>().GetQuerable<SMAM_Products_ST>().FirstOrDefault(a => a.ProductCode.Equals(CompanyProductsCode.Wps));
                    var nonWpsProduct = new BLInfo<SMAM_Products_ST>().GetQuerable<SMAM_Products_ST>().FirstOrDefault(a => a.ProductCode.Equals(CompanyProductsCode.nonWps));

                    foreach (var item in model)
                    {
                        GetByPrimaryKey(item.ProductId);

                        #region Add New Product
                        if (PrimaryKeyValue == null)
                        {
                            New();
                            #region Wps Product Region
                            if (item.ProductId == wpsProduct.ProductId)
                            {
                                var maxCompanyProduct = new BLInfo<SMAM_CompanyProducts_ST>().GetQuerable<SMAM_CompanyProducts_ST>().OrderByDescending(a => a.ProductCode).FirstOrDefault(a => a.ProductId == wpsProduct.ProductId);
                                Current.CompanyId = CompanyId;
                                Current.ProductId = wpsProduct.ProductId;
                                Current.ProductName = wpsProduct.ProductCode + "_" + Convert.ToInt32(maxCompanyProduct.ProductCode + 1) + "_" + wpsProduct.ShortName;
                                Current.ProductCode = Convert.ToInt32(maxCompanyProduct.ProductCode + 1);
                                Current.IsActive = item.IsActive;
                                Current.EffFrom = item.EffFrom;
                                Current.EffTo = item.EffTo;
                                Current.CreatedBy = item.CreatedBy;
                                Current.CreatedOn = DateTime.Now;
                            }
                            #endregion

                            #region NonWps product Region
                            if (item.ProductId == nonWpsProduct.ProductId)
                            {
                                var maxCompanyProductNonWps = new BLInfo<SMAM_CompanyProducts_ST>().GetQuerable<SMAM_CompanyProducts_ST>().OrderByDescending(a => a.ProductCode).FirstOrDefault(a => a.ProductId == nonWpsProduct.ProductId);

                                Current.CompanyId = CompanyId;
                                Current.ProductId = nonWpsProduct.ProductId;
                                Current.ProductName = nonWpsProduct.ProductCode + "_" + Convert.ToInt32(maxCompanyProductNonWps.ProductCode + 1) + "_" + nonWpsProduct.ShortName;
                                Current.ProductCode = Convert.ToInt32(maxCompanyProductNonWps.ProductCode + 1);
                                Current.IsActive = item.IsActive;
                                Current.EffFrom = item.EffFrom;
                                Current.EffTo = item.EffTo;
                                Current.CreatedBy = item.CreatedBy;
                                Current.CreatedOn = DateTime.Now;
                            }

                            #endregion
                            Save();
                        }
                        #endregion Add New Product

                    }

                }
                return this.SuccessResponse("");
            }
            catch (Exception ex)
            {
                return this.BadResponse(ex.Message);
            }
        }
    }
}