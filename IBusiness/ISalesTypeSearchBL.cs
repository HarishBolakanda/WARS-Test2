using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ISalesTypeSearchBL
    {
        DataSet GetInitialData(out Int32 iErrorId);    
        DataSet GetSalesTypeData(string salesTypeCode, out Int32 iErrorId);
        DataSet SaveSalesTypeGroup(string flag, string salesTypeCode, string salesTypeName, string salesTypeType, string escalationProrata, string userCode, out Int32 iErrorId);
    }
}
