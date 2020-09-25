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
    public class AutoParticipantMaintenanceBL : IAutoParticipantMaintenanceBL
    {
        IAutoParticipantMaintenanceDAL autoParticipantMaintenanceDAL;
        #region Constructor
        public AutoParticipantMaintenanceBL()
        {
            autoParticipantMaintenanceDAL = new AutoParticipantMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetDropDownData(string autoPariticp_ID, string loggedInUser, out Int32 iErrorId)
        {
            return autoParticipantMaintenanceDAL.GetDropDownData(autoPariticp_ID, loggedInUser,out iErrorId);
        }

        public DataSet SaveAutoParticipantDetails(string autoPariticp_ID, Array autoParticipantsToAddUpdate, string userRole, string loggedInUser, out Int32 iErrorId)
        {
            return autoParticipantMaintenanceDAL.SaveAutoParticipantDetails(autoPariticp_ID, autoParticipantsToAddUpdate, userRole, loggedInUser,out iErrorId);
        }

    }
}
