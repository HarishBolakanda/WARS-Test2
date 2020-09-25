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
    public class ArtistResponsibilityMaintenanceBL : IArtistResponsibilityMaintenanceBL
    {
        ArtistResponsibilityMaintenanceDAL ArtistResponsibilityMaintenanceDAL;
        #region Constructor
        public ArtistResponsibilityMaintenanceBL()
        {
            ArtistResponsibilityMaintenanceDAL = new ArtistResponsibilityMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return ArtistResponsibilityMaintenanceDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetSearchedArtistData(string artistName, string dealType, string teamResponsibility, out Int32 iErrorId)
        {
            return ArtistResponsibilityMaintenanceDAL.GetSearchedArtistData(artistName, dealType, teamResponsibility, out iErrorId);
        }

        public DataSet UpdateArtistData(string artistId, string teamResponsibility, string mngrResponsibility,
            string userCode, string artistName, out Int32 iErrorId)
        {
            return ArtistResponsibilityMaintenanceDAL.UpdateArtistData(artistId, teamResponsibility, mngrResponsibility, userCode, artistName, out iErrorId);
        }

        public DataSet InsertArtistData(string artistName, string dealType, string teamResponsibility, string mngrResponsibility,
           string userCode, string artistNameSearch, string dealTypeSearch, string responsibilitySearch, out Int32 iErrorId)
        {
            return ArtistResponsibilityMaintenanceDAL.InsertArtistData(artistName, dealType, teamResponsibility, mngrResponsibility, userCode, artistNameSearch, dealTypeSearch, responsibilitySearch, out iErrorId);
        }

    }
}
