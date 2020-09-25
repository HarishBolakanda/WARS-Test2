using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;
using System.Data;

namespace WARS.BusinessLayer
{
    public class TerritorySearchBL : ITerritorySearchBL
    {
        ITerritorySearchDAL territorySearchDAL;
        #region Constructor
        public TerritorySearchBL()
        {
            territorySearchDAL = new TerritorySearchDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return territorySearchDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetTerritoryData(string territoryCode, out Int32 iErrorId)
        {
            return territorySearchDAL.GetTerritoryData(territoryCode, out iErrorId);
        }

        public void AddTerritoryToGroups(string territoryCode, Array territoryGroupcodes, string userCode, out Int32 iErrorId)
        {
            territorySearchDAL.AddTerritoryToGroups(territoryCode, territoryGroupcodes, userCode, out iErrorId);
        }

        public DataSet InsertUpdateTerritoryGroup(string flag, string territoryCode, string territoryName, string territoryLocation, string countryCode, string territoryType, string userCode, out Int32 iErrorId)
        {
            return territorySearchDAL.InsertUpdateTerritoryGroup(flag, territoryCode, territoryName, territoryLocation, countryCode, territoryType, userCode, out iErrorId);
        }
    }
}
