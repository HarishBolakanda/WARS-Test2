using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IBreakdownGroupMaintenanceDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);

        DataSet UpdateBreakdownGroupData(string breakdownGrpCode, string breakdownGrpDesc, string territoryGrpCode,
            string configGrpCode, string salesTypeGrpCode, string GFSPLAccount, string GFSBLAccount, string userCode, out Int32 iErrorId);

        DataSet InsertBreakdownGroupData(string breakdownGrpCode, string breakdownGrpDesc, string territoryGrpCode,
            string configGrpCode, string salesTypeGrpCode, string GFSPLAccount, string GFSBLAccount, string userCode, out Int32 iErrorId);

        DataSet DeleteBreakdownGroupData(string breakdownGrpCode, out Int32 iErrorId);
    }
}
