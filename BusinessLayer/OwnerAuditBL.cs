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
    public class OwnerAuditBL : IOwnerAuditBL
    {
        IOwnerAuditDAL OwnerAuditDAL;
        #region Constructor
        public OwnerAuditBL()
        {
            OwnerAuditDAL = new OwnerAuditDAL();
        }
        #endregion Constructor
        
        public DataSet GetOwnerAuditData(string ownerCode, out Int32 iErrorId)
        {
            return OwnerAuditDAL.GetOwnerAuditData(ownerCode, out iErrorId);
        }
    }
}
