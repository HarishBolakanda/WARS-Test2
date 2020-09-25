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
    public class RoyaltorPayeeSuppAuditBL : IRoyaltorPayeeSuppAuditBL
    {
        IRoyaltorPayeeSuppAuditDAL RoyaltorPayeeSuppAuditDAL;
        #region Constructor
        public RoyaltorPayeeSuppAuditBL()
        {
            RoyaltorPayeeSuppAuditDAL = new RoyaltorPayeeSuppAuditDAL();
        }
        #endregion Constructor
        
        public DataSet GetRoyPayeeSuppAuditData(string royaltorId, string intPartyId, out Int32 iErrorId)
        {
            return RoyaltorPayeeSuppAuditDAL.GetRoyPayeeSuppAuditData(royaltorId, intPartyId, out iErrorId);
        }
    }
}
