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
    public class RoyContractAuditBL : IRoyContractAuditBL
    {
        IRoyContractAuditDAL RoyContractAuditDAL;
        #region Constructor
        public RoyContractAuditBL()
        {
            RoyContractAuditDAL = new RoyContractAuditDAL();
        }
        #endregion Constructor

        public DataSet GetRoyContractAuditData(Int32 royaltorId, out Int32 iErrorId)
        {
            return RoyContractAuditDAL.GetRoyContractAuditData(royaltorId, out iErrorId);
        }

        public DataSet GetRoyReserveAuditData(Int32 royaltorId, out Int32 iErrorId)
        {
            return RoyContractAuditDAL.GetRoyReserveAuditData(royaltorId, out iErrorId);
        }
    }
}
