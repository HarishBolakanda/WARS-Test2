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
    public class CatalogueSearchBL : ICatalogueSearchBL
    {
        ICatalogueSearchDAL CatalogueSearchDAL;
        #region Constructor
        public CatalogueSearchBL()
        {
            CatalogueSearchDAL = new CatalogueSearchDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return CatalogueSearchDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetSearchedCatData(string catNo, string title, string artist, string configCode, string isrc, string teamResp, string teamRespTrack, string managerResp, string managerRespTrack, string catnoStatus, string trackStatus,
                                           Array catnoBulkSearchList, out Int32 iErrorId)
        {
            return CatalogueSearchDAL.GetSearchedCatData(catNo, title, artist, configCode, isrc, teamResp, teamRespTrack, managerResp, managerRespTrack, catnoStatus, trackStatus, catnoBulkSearchList, out iErrorId);
        }

        public DataSet GetCatnoParticipants(Array catnoList, out Int32 iErrorId)
        {
            return CatalogueSearchDAL.GetCatnoParticipants(catnoList, out iErrorId);
        }

        public DataSet UpdateCatalogueDetails(string catNo, string title, string artist, string configCode, string isrc, string teamResp, string teamRespTrack, string managerResp, string managerRespTrack, string statusFilter, string statusCode, string trackStatus,
                                       Array catnoBulkSearchList, Array catalogueDetails, string userCode, out Int32 iErrorId)
        {
            return CatalogueSearchDAL.UpdateCatalogueDetails(catNo, title, artist, configCode, isrc, teamResp, teamRespTrack, managerResp, managerRespTrack, statusFilter, statusCode, trackStatus, catnoBulkSearchList, catalogueDetails, userCode, out iErrorId);
        }

    }
}
