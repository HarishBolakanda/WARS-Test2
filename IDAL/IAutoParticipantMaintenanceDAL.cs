using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IAutoParticipantMaintenanceDAL
    {
        DataSet GetDropDownData(string autoPariticp_ID, string loggedInUser, out Int32 iErrorId);
        DataSet SaveAutoParticipantDetails(string autoPariticp_ID, Array autoParticipantsToAddUpdate, string userRole, string loggedInUser, out Int32 iErrorId);
    }
}
