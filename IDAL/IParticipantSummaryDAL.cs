using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IParticipantSummaryDAL
    {
        DataSet GetParticipantSummary(string catNo, string activeParticips, string loggedInUser, out Int32 iErrorId);
        DataSet SaveParticipantDetails(string catNo, string catStatusCode, string isCatModified, Array participantsToAddUpdate,
                                             string activeParticips, string userCode, string userRoleId, out Int32 iErrorId);
        void CorrectMismatches(string catNo, string userCode, out Int32 iErrorId, out string errorMsg);
       void GetTrackTitlefromISRC(string catNo, string seqNo, out Int32 iErrorId, out string trackTitle, out string isrc, out string track_Listing_Id);
    }
}
