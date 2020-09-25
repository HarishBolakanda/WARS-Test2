using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ICatalogueSearchDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetSearchedCatData(string catNo, string title, string artist, string configCode, string isrc, string teamResp, string teamRespTrack, string managerResp, string managerRespTrack, string catnoStatus, string trackStatus,
                                           Array catnoBulkSearchList, out Int32 iErrorId);
        DataSet GetCatnoParticipants(Array catnoList, out Int32 iErrorId);
        DataSet UpdateCatalogueDetails(string catNo, string title, string artist, string configCode, string isrc, string teamResp, string teamRespTrack, string managerResp, string managerRespTrack, string statusFilter, string statusCode, string trackStatus,
                                 Array catnoBulkSearchList, Array catalogueDetails, string userCode, out Int32 iErrorId);

    }
}
