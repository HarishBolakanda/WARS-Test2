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
    public class MissingParticipantsBL : IMissingParticipantsBL
    {
        IMissingParticipantsDAL MissingParticipantsDAL;
        #region Constructor
        public MissingParticipantsBL()
        {
            MissingParticipantsDAL = new MissingParticipantsDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return MissingParticipantsDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetSearchedData(string catNo, string title, string artist, string configCode, string teamResponsibility, string teamResponsibilityTrack,
            string mngrResponsibility, string managerResponsibilityTrack, string trnxStartDate, string trnxStartDateCompare, string trnxEndDate, string trnxEndDateCompare, string catnoStatusCode, string trackStatusCode, string trnxValue, string isrc, out Int32 iErrorId)
        {
            return MissingParticipantsDAL.GetSearchedData(catNo, title, artist, configCode, teamResponsibility, teamResponsibilityTrack, mngrResponsibility, managerResponsibilityTrack, trnxStartDate, trnxStartDateCompare, trnxEndDate, trnxEndDateCompare, catnoStatusCode, trackStatusCode, trnxValue, isrc, out iErrorId);
        }

        public string IsTrackListingExist(string catNo, out string catnoLegacy, out Int32 iErrorId)
        {
            return MissingParticipantsDAL.IsTrackListingExist(catNo, out catnoLegacy, out iErrorId);
        }
    }
}
