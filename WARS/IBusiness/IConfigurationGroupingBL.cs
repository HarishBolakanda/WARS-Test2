using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IConfigurationGroupingBL
    {
        DataSet GetConfigurationGroupList(out Int32 iErrorId);
        DataSet GetConfigurationGroupInOutData(string configGroupCode, out Int32 iErrorId);
        DataSet AddConfigurationToGroup(string configGroupCode, Array configcodes, out Int32 iErrorId);
        DataSet RemoveConfigurationFromGroup(string configGroupCode, Array configcodes, out Int32 iErrorId);
        DataSet InsertConfigurationGroup(string configGroupCode, string configGroupName, out Int32 iErrorId);
    }
}
