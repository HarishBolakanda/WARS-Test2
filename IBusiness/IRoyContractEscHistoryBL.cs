using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyContractEscHistoryBL
    {
        DataSet GetInitialData(string royaltorId, out Int32 iErrorId);
        DataSet GetEscHistory(string royaltorId, string escCode, out Int32 iErrorId);
        DataSet GetEscHistorySummary(string royaltorId, string escCode, out Int32 iErrorId);
        DataSet SaveEscHistory(string royaltorId, string escCode, string sellerGrpCode, string priceGrpCode, string configGrpCode, string sales, string adjSales, out Int32 iErrorId);
        DataSet DeleteEscHistory(string royaltorId, string escCode, string sellerGrpCode, string priceGrpCode, string configGrpCode, out Int32 iErrorId);
    }
}
