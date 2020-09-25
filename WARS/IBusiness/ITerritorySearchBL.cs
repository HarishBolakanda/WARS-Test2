using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ITerritorySearchBL
    {
        DataSet GetTerritoriesList(out Int32 iErrorId);
        DataSet GetTerritoryData(string territoryCode, out Int32 iErrorId);
        void AddTerritoryToGroups(string territoryCode, Array territoryGroupcodes, out Int32 iErrorId);
    }
}
