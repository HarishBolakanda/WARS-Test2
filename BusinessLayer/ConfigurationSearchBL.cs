using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;
using System.Data;

namespace WARS.BusinessLayer
{
    public class ConfigurationSearchBL : IConfigurationSearchBL
    {
        IConfigurationSearchDAL configurationSearchDAL;
        #region Constructor
        public ConfigurationSearchBL()
        {
            configurationSearchDAL = new ConfigurationSearchDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {

            return configurationSearchDAL.GetInitialData(out iErrorId);
        }
               
        public DataSet GetConfigurationData(string ConfigurationCode, out Int32 iErrorId)
        {
            return configurationSearchDAL.GetConfigurationData(ConfigurationCode, out iErrorId);
        }

        public DataSet SaveConfigGroup(string flag, string configTypeCode, string configTypeName, string configType, string userCode, out Int32 iErrorId)
        {
            return configurationSearchDAL.SaveConfigGroup(flag, configTypeCode, configTypeName, configType, userCode, out iErrorId);
        }
    }
}
