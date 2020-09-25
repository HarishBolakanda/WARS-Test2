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
    public class AutoParticipantMaintAuditBL : IAutoParticipantMaintAuditBL
    {
        IAutoParticipantMaintAuditDAL autoParticipantMaintAuditDAL;
        #region Constructor
        public AutoParticipantMaintAuditBL()
        {
            autoParticipantMaintAuditDAL = new AutoParticipantMaintAuditDAL();
        }
        #endregion Constructor
        public DataSet GetAutoPartMaintAuditData(string autoPartId, out Int32 iErrorId)
        {
            return autoParticipantMaintAuditDAL.GetAutoPartMaintAuditData(autoPartId, out iErrorId);
        }
    }
}
