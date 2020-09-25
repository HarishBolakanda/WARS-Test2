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
    public class TerritoryGroupAuditBL : ITerritoryGroupAuditBL
    {
        ITerritoryGroupAuditDAL TerritoryGroupAuditDAL;
        #region Constructor
        public TerritoryGroupAuditBL()
        {
            TerritoryGroupAuditDAL = new TerritoryGroupAuditDAL();
        }
        #endregion Constructor
               
        public DataSet GetTerritoryGroupAuditData(string territoryGroupCode, string fromDate, string toDate, out Int32 iErrorId)
        {
            return TerritoryGroupAuditDAL.GetTerritoryGroupAuditData(territoryGroupCode, fromDate, toDate, out iErrorId);
        }
    }
}
