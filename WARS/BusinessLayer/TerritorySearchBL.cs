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

        public DataSet GetTerritoriesList(out Int32 iErrorId)
        {
            return territorySearchDAL.GetTerritoriesList(out iErrorId);
        }

        public DataSet GetTerritoryData(string territoryCode, out Int32 iErrorId)
        {
            return territorySearchDAL.GetTerritoryData(territoryCode, out iErrorId);
        }

        public void AddTerritoryToGroups(string territoryCode, Array territoryGroupcodes, out Int32 iErrorId)
        {
            territorySearchDAL.AddTerritoryToGroups(territoryCode, territoryGroupcodes, out iErrorId);
        }
    }
}
