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
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetTerritoryData(string territoryCode, out Int32 iErrorId);
        void AddTerritoryToGroups(string territoryCode, Array territoryGroupcodes, string userCode, out Int32 iErrorId);
        DataSet InsertUpdateTerritoryGroup(string flag, string territoryCode, string territoryName, string territoryLocation, string countryCode, string territoryType, string userCode, out Int32 iErrorId);
    }
}
