using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE_API.Models.CompanyHistory
{
    public class CompanyHistoryModel
    {

        private int CmpChrgId;

        public int? CompanyId;

        public int? ChrgTypeId;

        public decimal? Charges;

        public decimal? NewCharges;

        public DateTime EffFrom;

        public DateTime EffTo;

        public int? CreatedBy;

        public DateTime? CreatedOn;

        public int? ModifiedBy;

        public DateTime? ModifiedOn;

        public int? ProductId;


    }
}
