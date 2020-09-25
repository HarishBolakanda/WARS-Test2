using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ISalesTypeGroupingDAL
    {        
        DataSet GetSalesTypeGroupInOutData(string salesTypeGroupCode, out Int32 iErrorId);
        DataSet AddSalesTypeToGroup(string salesTypeGroupCode, Array salesTypecodes, string userCode, out Int32 iErrorId);
        DataSet RemoveSalesTypeFromGroup(string salesTypeGroupCode, Array salesTypecodes,string userCode, out Int32 iErrorId);
        DataSet InsertSalesTypeGroup(string salesTypeGroupCode, string salesTypeGroupName, string userCode, out Int32 iErrorId);
    }
}
