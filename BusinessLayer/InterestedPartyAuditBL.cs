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
    public class InterestedPartyAuditBL : IInterestedPartyAuditBL
    {
        
        IInterestedPartyAuditDAL InterestedPartyAuditDAL;
        #region Constructor
        public InterestedPartyAuditBL()
        {
            InterestedPartyAuditDAL = new InterestedPartyAuditDAL();
        }
        #endregion Constructor
               

        public DataSet GetInterestedPartyAuditData(Int32 intPartyId, out string intParty, out Int32 iErrorId)
        {
            return InterestedPartyAuditDAL.GetInterestedPartyAuditData(intPartyId, out intParty ,out iErrorId);
        }
    }
}
