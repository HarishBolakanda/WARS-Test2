using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IConfigurationSearchBL
    {
        DataSet GetConfigurationsList(out Int32 iErrorId);
        DataSet GetConfigurationData(string ConfigurationCode, out Int32 iErrorId);
    }
}
