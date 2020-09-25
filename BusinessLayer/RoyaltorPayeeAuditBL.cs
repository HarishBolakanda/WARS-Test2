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
    public class RoyaltorPayeeAuditBL : IRoyaltorPayeeAuditBL 
    {
        IRoyaltorPayeeAuditDAL RoyaltorPayeeAuditDAL;
        #region Constructor
        public RoyaltorPayeeAuditBL()
        {
            RoyaltorPayeeAuditDAL = new RoyaltorPayeeAuditDAL();
        }
        #endregion Constructor

        public DataSet GetRoyPayeeAuditData(Int32 royaltorId, out Int32 iErrorId)
        {
            return RoyaltorPayeeAuditDAL.GetRoyPayeeAuditData(royaltorId, out iErrorId);
        }

    }
}
