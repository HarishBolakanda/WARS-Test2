using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IConfigurationGroupAuditBL
    {
        DataSet GetConfigurationGroupAuditData(string ConfigurationGroupCode, string fromDate, string toDate, out Int32 iErrorId);
    }
}
