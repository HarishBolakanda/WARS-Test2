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
    public class RoyaltorPackagingRatesAuditBL : IRoyaltorPackagingRatesAuditBL
    {
        
        IRoyaltorPackagingRatesAuditDAL RoyaltorPackagingRatesAuditDAL;
        #region Constructor
        public RoyaltorPackagingRatesAuditBL()
        {
            RoyaltorPackagingRatesAuditDAL = new RoyaltorPackagingRatesAuditDAL();
        }
        #endregion Constructor
                
        public DataSet GetRoyPkgRatesAuditData(string royaltorId, string optionPeriodCode, string fromDate, string toDate, string userRoleId, out Int32 iErrorId)
        {
            return RoyaltorPackagingRatesAuditDAL.GetRoyPkgRatesAuditData(royaltorId, optionPeriodCode, fromDate, toDate, userRoleId, out iErrorId);
        }
    }
}
