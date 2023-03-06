using ContextMapper;
using CORE_API.General;
using CORE_BASE.CORE;
using CORE_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CORE_API.Services.FNGL
{
    public class TransactionManager
    {
        public FNGL_TransType_ST GetTransactionType(string Transactiontype)
        {
            return new BLInfo<FNGL_TransType_ST>().GetQuerable<FNGL_TransType_ST>().FirstOrDefault(a => a.TransTypeDesc.Equals(Transactiontype.ToString()));
        }

        public FNGL_TransType_ST GetTransactionTypebyCode(string TransCode)
        {
            return new BLInfo<FNGL_TransType_ST>().GetQuerable<FNGL_TransType_ST>().FirstOrDefault(a => a.TransCode.Equals(TransCode.ToString()));
        }

        public string CreateTransferNumber(string lastTransferNumber)
        {
            var lastAddedFullTransferNumber = lastTransferNumber ?? "000000";
            var lastAddedTransferNumber = Int64.Parse(lastAddedFullTransferNumber.Substring(0, 6));
            var newTransferNumber = Convert.ToString(++lastAddedTransferNumber);

            if (newTransferNumber.Length < 6)
                while (newTransferNumber.Length < 6)
                    newTransferNumber = "0" + newTransferNumber;

            return newTransferNumber;
        }

        public string GetLastTransferNumberAdded(string type)
        {
            int? transactionTypeDetail = GetTransactionType(type.ToString()).TransTypeId;

            var rtn = new BLInfo<FNGL_Transactions_HI>().GetQuerable<FNGL_Transactions_HI>().OrderByDescending(a => a.TransTypeId)
                .FirstOrDefault(a => a.TransTypeId == transactionTypeDetail && a.TransferId != null);

            return rtn != null ? rtn.TransReference : null;
        }

        

    }
}