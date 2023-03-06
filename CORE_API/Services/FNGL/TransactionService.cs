using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CORE_API.Services.FNGL
{
    public class TransactionService : CastService<FNGL_Transactions_HI>
    {

        dbEngine db = new dbEngine();
        public DTO GetAllDebitCreditTransaction(int? pageNo = null, int limit = 0, int? companyId = null)
        {

            var totalRecords = db._dbContext.FNGL_Transactions_HIs.Where(o => o.CompanyId == companyId).ToList().Count();

            var debitCreditTransaction = (from t1 in db._dbContext.FNGL_Transactions_HIs
                                          join t2 in db._dbContext.FNGL_TransType_STs on t1.TransTypeId equals t2.TransTypeId into transtype
                                          from trans in transtype.DefaultIfEmpty()
                                          join t3 in db._dbContext.SMAM_CompanyMst_STs on t1.CompanyId equals t3.CompanyId into compMst
                                          from comp in compMst.DefaultIfEmpty()
                                          join t4 in db._dbContext.HRMS_EmployeeMst_STs on t1.EmpId equals t4.EmpId into employeeMst
                                          from emp in employeeMst.DefaultIfEmpty()

                                          where t1.CompanyId == companyId

                                          select new
                                          {
                                              t1.TransId,
                                              t1.TransReference,
                                              t1.TransDate,
                                              t1.TransDesc,
                                              DebitAmount = t1.DebitAmount != null ? t1.DebitAmount : 0M,
                                              CreditAmount = t1.CreditAmount != null ? t1.CreditAmount : 0M,
                                              trans.TransTypeDesc,
                                              CompanyName = comp.Name,
                                              EmployeeName = emp.DisplayName,
                                              totalRecords

                                          }).Skip(pageNo.HasValue ? pageNo.Value : 0)
                                                      .Take(limit > 0 ? limit : int.MaxValue).OrderByDescending(o => o.TransId).ToList();



            if (debitCreditTransaction.Count == 0)
                return BadResponse("No Transaction Available");

            return this.SuccessResponse(debitCreditTransaction);
        }
    }
}