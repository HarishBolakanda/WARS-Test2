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
    public class RoyaltorRatesAuditBL : IRoyaltorRatesAuditBL
    {
        IRoyaltorRatesAuditDAL RoyaltorRatesAuditDAL;
        #region Constructor
        public RoyaltorRatesAuditBL()
        {
            RoyaltorRatesAuditDAL = new RoyaltorRatesAuditDAL();
        }
        #endregion Constructor

        public DataSet GetRoyRatesAuditData(string royaltorId, string optionPeriodCode, string fromDate, string toDate, string userRoleId, out Int32 iErrorId)
        {
            return RoyaltorRatesAuditDAL.GetRoyRatesAuditData(royaltorId, optionPeriodCode, fromDate, toDate, userRoleId, out iErrorId);
        }
    }
}
