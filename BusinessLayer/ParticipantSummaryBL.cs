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
    public class ParticipantSummaryBL : IParticipantSummaryBL
    {
        IParticipantSummaryDAL participantSummaryDAL;
        #region Constructor
        public ParticipantSummaryBL()
        {
            participantSummaryDAL = new ParticipantSummaryDAL();
        }
        #endregion Constructor

        //DataTable dtCatalogue;
        //DataTable dtParticipants;
        DataSet dsParticipSummary;

        public DataSet GetParticipantSummary(string catNo, string activeParticips, string loggedInUser, out Int32 iErrorId)
        {
            dsParticipSummary = participantSummaryDAL.GetParticipantSummary(catNo, activeParticips, loggedInUser, out iErrorId);
            if (iErrorId != 2 && dsParticipSummary.Tables.Count != 0)
            {
                dsParticipSummary.Tables[0].TableName = "dtCatNoStatusList";
                dsParticipSummary.Tables[1].TableName = "dtISRCStatusList";
                dsParticipSummary.Tables[2].TableName = "dtCatDetails";
                dsParticipSummary.Tables[3].TableName = "dtParicipants";

                ParticipantOrderBy(dsParticipSummary.Tables["dtParicipants"]);
            }

            return dsParticipSummary;
        }

       

        /// <summary>
        /// To Order by the grid display data
        /// </summary>
        /// <param name="dtParticipants"></param>
        /// <returns></returns>
        private void ParticipantOrderBy(DataTable dtParticipants)
        {
            DataView dv = dtParticipants.DefaultView;
            dv.Sort = "royaltor_id,option_period_code,seller_group_code,share_tracks,share_total_tracks,share_time,share_total_time,track_title,esc_code,esc_track_count,status_code";
            DataTable dtSorted = dv.ToTable();

        }

        public void CorrectMismatches(string catNo, string userCode, out Int32 iErrorId, out string errorMsg)
        {
            participantSummaryDAL.CorrectMismatches(catNo, userCode, out iErrorId, out errorMsg);
        }
       

        public DataSet SaveParticipantDetails(string catNo, string catStatusCode, string isCatModified, Array participantsToAddUpdate,
                                              string activeParticips, string userCode, string userRoleId, out Int32 iErrorId)
        {
            return participantSummaryDAL.SaveParticipantDetails(catNo, catStatusCode, isCatModified, participantsToAddUpdate, activeParticips, userCode, userRoleId, out iErrorId);
        }

        public void GetTrackTitlefromISRC(string catNo, string seqNo, out Int32 iErrorId, out string trackTitle, out string isrc, out string track_Listing_Id)
        {
            participantSummaryDAL.GetTrackTitlefromISRC(catNo, seqNo, out iErrorId,out trackTitle, out isrc, out track_Listing_Id);
        }
    }
}
