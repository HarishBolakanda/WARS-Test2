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
    public class RoyaltorSubsidiaryRatesAuditBL : IRoyaltorSubsidiaryRatesAuditBL
    {
        IRoyaltorSubsidiaryRatesAuditDAL RoyaltorSubsidiaryRatesAuditDAL;
        #region Constructor
        public RoyaltorSubsidiaryRatesAuditBL()
        {
            RoyaltorSubsidiaryRatesAuditDAL = new RoyaltorSubsidiaryRatesAuditDAL();
        }
        #endregion Constructor

        public DataSet GetSubsidiaryRatesAuditData(string royaltorId, string fromDate, string toDate, string userRoleId, out Int32 iErrorId)
        { 
            return RoyaltorSubsidiaryRatesAuditDAL.GetSubsidiaryRatesAuditData(royaltorId, fromDate, toDate, userRoleId, out iErrorId);
        }

    }
}
