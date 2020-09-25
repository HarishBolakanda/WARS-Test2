using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IMissingParticipantsDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetSearchedData(string catNo, string title, string artist, string configCode, string teamResponsibility, string teamResponsibilityTrack,
             string mngrResponsibility, string managerResponsibilityTrack, string trnxStartDate, string trnxStartDateCompare, string trnxEndDate, string trnxEndDateCompare, string catnoStatusCode, string trackStatusCode, string trnxValue, string isrc, out Int32 iErrorId);
        string IsTrackListingExist(string catNo, out string catnoLegacy, out Int32 iErrorId);
    }
}
