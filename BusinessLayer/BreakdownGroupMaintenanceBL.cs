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
    public class BreakdownGroupMaintenanceBL : IBreakdownGroupMaintenanceBL
    {
        IBreakdownGroupMaintenanceDAL BreakdownGroupMaintenanceDAL;
        #region Constructor
        public BreakdownGroupMaintenanceBL()
        {
            BreakdownGroupMaintenanceDAL = new BreakdownGroupMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return BreakdownGroupMaintenanceDAL.GetInitialData(out iErrorId);
        }

        public DataSet UpdateBreakdownGroupData(string breakdownGrpCode, string breakdownGrpDesc, string territoryGrpCode,
            string configGrpCode, string salesTypeGrpCode, string GFSPLAccount, string GFSBLAccount, string userCode, out Int32 iErrorId)
        {
            return BreakdownGroupMaintenanceDAL.UpdateBreakdownGroupData(breakdownGrpCode, breakdownGrpDesc, territoryGrpCode, configGrpCode, salesTypeGrpCode, GFSPLAccount, GFSBLAccount, userCode, out iErrorId);
        }

        public DataSet InsertBreakdownGroupData(string breakdownGrpCode, string breakdownGrpDesc, string territoryGrpCode,
            string configGrpCode, string salesTypeGrpCode, string GFSPLAccount, string GFSBLAccount, string userCode, out Int32 iErrorId)
        {
            return BreakdownGroupMaintenanceDAL.InsertBreakdownGroupData(breakdownGrpCode, breakdownGrpDesc, territoryGrpCode, configGrpCode, salesTypeGrpCode, GFSPLAccount, GFSBLAccount, userCode, out iErrorId);
        }

        public DataSet DeleteBreakdownGroupData(string breakdownGrpCode, out Int32 iErrorId)
        {
            return BreakdownGroupMaintenanceDAL.DeleteBreakdownGroupData(breakdownGrpCode, out iErrorId);
        }
    }
}
