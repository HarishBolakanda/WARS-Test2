using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ISellerGroupBL
    {
        DataSet GetSellerGroupList(out Int32 iErrorId);
        DataSet GetSellerGroupInOutData(string sellerGroupCode, out Int32 locationCount, out Int32 iErrorId);
        DataSet AddTerritoryToGroup(string territoryGroupCode, Array territorycode, out Int32 locationCount, out Int32 iErrorId);
        DataSet RemoveTerritoryToGroup(string territoryGroupCode, Array territorycode, out Int32 locationCount, out Int32 iErrorId);
        DataSet InsertTerritoryGroup(string territoryGroupCode, string territoryGroupName, out Int32 iErrorId);
    }
}
