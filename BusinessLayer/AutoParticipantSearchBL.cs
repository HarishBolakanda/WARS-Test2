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
    public class AutoParticipantSearchBL : IAutoParticipantSearchBL
    {

        IAutoParticipantSearchDAL autoParticipantSearchDAL;
        #region Constructor
        public AutoParticipantSearchBL()
        {
            autoParticipantSearchDAL = new AutoParticipantSearchDAL();
        }
        #endregion Constructor

        public DataSet SearchAutoParticipantData(string marketingOwer, string weaSalesLable, string artist, string projectTitle, out Int32 iErrorId)
        {
            return autoParticipantSearchDAL.SearchAutoParticipantData(marketingOwer, weaSalesLable, artist, projectTitle, out iErrorId);
        }

        public DataSet AddUpdateAutoParticipant(Int32 autoPartId, string marketingOwer, string weaSalesLable, Int32 artistId, Int32 projectId, string projectTitle, string marketingOnwerSearch, string weaSalesLabelSearch, string artistSearch, string projectSearch, string userCode, out Int32 newAutoPartId, out Int32 iErrorId)
        {
            return autoParticipantSearchDAL.AddUpdateAutoParticipant(autoPartId, marketingOwer, weaSalesLable, artistId, projectId, projectTitle, marketingOnwerSearch, weaSalesLabelSearch, artistSearch, projectSearch, userCode, out newAutoPartId, out iErrorId);
        }
    }
}
