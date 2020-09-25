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
    public class RoyaltorGroupingAuditBL : IRoyaltorGroupingAuditBL
    {
        IRoyaltorGroupingAuditDAL RoyaltorGroupingAuditDAL;
        #region Constructor
        public RoyaltorGroupingAuditBL()
        {
            RoyaltorGroupingAuditDAL = new RoyaltorGroupingAuditDAL();
        }
        #endregion Constructor
        
        public DataSet GetRoyaltorData(string royaltorId, out Int32 iErrorId)
        {
            return RoyaltorGroupingAuditDAL.GetRoyaltorData(royaltorId, out iErrorId);
        }
    }
}
