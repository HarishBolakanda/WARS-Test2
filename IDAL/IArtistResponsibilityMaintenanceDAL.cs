using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IArtistResponsibilityMaintenanceDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);

        DataSet GetSearchedArtistData(string artistName,string dealType,string teamResponsibility, out Int32 iErrorId);

        DataSet UpdateArtistData(string artistId, string teamResponsibility, string mngrResponsibility,
            string userCode, string artistName, out Int32 iErrorId);

        DataSet InsertArtistData(string artistName, string dealType, string teamResponsibility, string mngrResponsibility,
           string userCode, string artistNameSearch, string dealTypeSearch, string responsibilitySearch, out Int32 iErrorId);
    }
}
