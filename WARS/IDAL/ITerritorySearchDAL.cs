using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ITerritorySearchDAL
    {
        DataSet GetTerritoriesList(out Int32 iErrorId);
        DataSet GetTerritoryData(string territoryCode, out Int32 iErrorId);
        void AddTerritoryToGroups(string territoryCode, Array territoryGroupcodes, out Int32 iErrorId);
    }
}
