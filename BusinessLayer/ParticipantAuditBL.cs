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
    public class ParticipantAuditBL : IParticipantAuditBL
    {
        IParticipantAuditDAL ParticipantAuditDAL;
        #region Constructor
        public ParticipantAuditBL()
        {
            ParticipantAuditDAL = new ParticipantAuditDAL();
        }
        #endregion Constructor
        
        public DataSet GetParticipantData(string catNo,string fromDate,string toDate, out Int32 iErrorId)
        {
            return ParticipantAuditDAL.GetParticipantData(catNo,fromDate,toDate,out iErrorId);
        }
    }
}
