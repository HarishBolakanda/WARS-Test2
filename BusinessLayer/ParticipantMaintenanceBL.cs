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
    public class ParticipantMaintenanceBL : IParticipantMaintenanceBL
    {
        IParticipantMaintenanceDAL ParticipantMaintenanceDAL;
        #region Constructor
        public ParticipantMaintenanceBL()
        {
            ParticipantMaintenanceDAL = new ParticipantMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(string catNo, string activeParticips, string userRoleId, out Int32 iErrorId)
        {
            return ParticipantMaintenanceDAL.GetInitialData(catNo, activeParticips, userRoleId, out iErrorId);
        }

        public DataSet GetAllOrActiveParticips(string catNo, string activeParticips, out Int32 iErrorId)
        {
            return ParticipantMaintenanceDAL.GetAllOrActiveParticips(catNo, activeParticips, out iErrorId);
        }

        public DataSet GetFuzzySearchLists(out Int32 iErrorId)
        {
            return ParticipantMaintenanceDAL.GetFuzzySearchLists(out iErrorId);
        }

        public DataSet SaveParticipantDetails(string catNo, string catStatusCode, string isCatModified, Array participantsToAddUpdate,
                                                string activeParticips, string userCode, string userRoleId, out Int32 iErrorId)
        {
            return ParticipantMaintenanceDAL.SaveParticipantDetails(catNo, catStatusCode, isCatModified, participantsToAddUpdate, activeParticips, userCode, userRoleId, out iErrorId);
        }
        public void GetTrackTitlefromISRC(string catNo, string seqNo, out Int32 iErrorId, out string trackTitle, out string isrc, out string track_Listing_Id)
        {
            ParticipantMaintenanceDAL.GetTrackTitlefromISRC(catNo, seqNo, out iErrorId, out trackTitle, out isrc, out track_Listing_Id);
        }
    }
}
