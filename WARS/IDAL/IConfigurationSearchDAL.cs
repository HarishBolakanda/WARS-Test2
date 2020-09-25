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
        DataSet GetConfigurationsList(out Int32 iErrorId);
        DataSet GetConfigurationData(string ConfigurationCode, out Int32 iErrorId);
    }
}
