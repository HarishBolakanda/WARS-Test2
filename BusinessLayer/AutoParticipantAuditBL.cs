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
    public class AutoParticipantAuditBL : IAutoParticipantAuditBL
    { 
        IAutoParticipantAuditDAL AutoParticipantAuditDAL;
        #region Constructor
        public AutoParticipantAuditBL()
        {
            AutoParticipantAuditDAL = new AutoParticipantAuditDAL();
        }
        #endregion Constructor

        public DataSet GetAutoParticipantAuditData(string autoPartId, out Int32 iErrorId)
        {
            return AutoParticipantAuditDAL.GetAutoParticipantAuditData(autoPartId, out iErrorId);
        }
    }
}
