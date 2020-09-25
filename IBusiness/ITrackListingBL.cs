using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ITrackListingBL
    {
        /* UserRole Added by RaviMulugu as a part of JIRA-898 on 15th November 2018 --Start */
        DataSet GetTrackListing(string catNo, string filterStatus, string filterUnit, string filterSide, string loggedInUserRole, out Int32 iErrorId);
        //DataSet GetTrackListing(string UserRole, string catNo, string filterStatus, string filterUnit, string filterSide, out Int32 iErrorId);
        /* UserRole Added by RaviMulugu as a part of JIRA-898 on 15th November 2018 --End */
        DataTable GetFilteredData(string catNo, string filterStatus, string filterUnit, string filterSide, string loggedInUserRole, out Int32 iErrorId);
        DataSet SaveComment(string catNo, string filterStatus, string filterUnit, string filterSide, string isrcDealId, string comment, string saveDelete, string loggedInUserRole, string userCode, out Int32 iErrorId);
        DataSet SaveAllTrackDetails(string catNo, string filterStatus, string filterUnit, string filterSide, string catStatusCode, string trackTimeFlag, string catStatusChanged, string catFlagChanged, string userCode,
                                   Array trackList, Array trackParticipantList, string loggedInUserRole, out Int32 iErrorId);
        DataSet CopyParticipant(string catNo, string filterStatus, string filterUnit, string filterSide, string userCode, string copyPart, string selectedTrackIds, string loggedInUserRole, out Int32 iErrorId);
        void ConsolidateParticipants(string catNo, string userCode, out Int32 iErrorId, out string errorMsg);
    }
}
