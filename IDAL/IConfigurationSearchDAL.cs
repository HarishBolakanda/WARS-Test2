using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IConfigurationSearchDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetConfigurationData(string ConfigurationCode, out Int32 iErrorId);
        DataSet SaveConfigGroup(string flag, string configTypeCode, string configTypeName, string configType, string userCode, out Int32 iErrorId);
    }
}
