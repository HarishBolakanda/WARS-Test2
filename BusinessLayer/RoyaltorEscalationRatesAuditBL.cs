using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;

namespace WARS.BusinessLayer
{
    public class RoyaltorEscalationRatesAuditBL : IRoyaltorEscalationRatesAuditBL 
    {
        IRoyaltorEscalationRatesAuditDAL RoyaltorEscalationRatesAuditDAL;
        #region Constructor
        public RoyaltorEscalationRatesAuditBL()
        {
            RoyaltorEscalationRatesAuditDAL = new RoyaltorEscalationRatesAuditDAL();
        }
        #endregion Constructor
        
        public DataSet GetEscalationRatesAuditData(string royaltorId, string fromDate, string toDate, out Int32 iErrorId)
        {
            return RoyaltorEscalationRatesAuditDAL.GetEscalationRatesAuditData(royaltorId, fromDate, toDate, out iErrorId);
        }
    }
}
