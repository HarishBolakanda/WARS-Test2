using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IParticipantMaintenanceDAL
    {
        DataSet GetInitialData(string catNo, string activeParticips, string loggedInUserRoleId, out Int32 iErrorId);
        DataSet GetAllOrActiveParticips(string catNo, string activeParticips, out Int32 iErrorId);
        DataSet GetFuzzySearchLists(out Int32 iErrorId);
        DataSet SaveParticipantDetails(string catNo, string catStatusCode, string isCatModified, Array participantsToAddUpdate, string activeParticips, string userCode, string userRoleId, out Int32 iErrorId);
        void GetTrackTitlefromISRC(string catNo, string seqNo, out Int32 iErrorId, out string trackTitle, out string isrc, out string track_Listing_Id);
    }
}
