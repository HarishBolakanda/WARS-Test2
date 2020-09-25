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
    public class ArtistAuditBL : IArtistAuditBL
    {
        IArtistAuditDAL ArtistAuditDAL;
        #region Constructor
        public ArtistAuditBL()
        {
            ArtistAuditDAL = new ArtistAuditDAL();
        }
        #endregion Constructor
                
        public DataSet GetArtistAuditData(Int32 artistId, out string artist, out Int32 iErrorId)
        {
            return ArtistAuditDAL.GetArtistAuditData(artistId,out artist, out iErrorId);
        }
    }
}
