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
    public class ConfigurationGroupingBL : IConfigurationGroupingBL
    {
        IConfigurationGroupingDAL configurationGroupingDAL;

        #region Constructor
        public ConfigurationGroupingBL()
        {
            configurationGroupingDAL = new ConfigurationGroupingDAL();
        }
        #endregion Constructor

        public DataSet GetConfigurationGroupInOutData(string configGroupCode, out Int32 iErrorId)
        {
            return configurationGroupingDAL.GetConfigurationGroupInOutData(configGroupCode, out iErrorId);
        }

        public DataSet AddConfigurationToGroup(string configGroupCode, Array configcodes, string userCode, out Int32 iErrorId)
        {
            return configurationGroupingDAL.AddConfigurationToGroup(configGroupCode, configcodes, userCode, out iErrorId);
        }

        public DataSet RemoveConfigurationFromGroup(string configGroupCode, Array configcodes, string userCode,out Int32 iErrorId)
        {
            return configurationGroupingDAL.RemoveConfigurationFromGroup(configGroupCode, configcodes,userCode, out iErrorId);
        }

        public DataSet InsertConfigurationGroup(string configGroupCode, string configGroupName, string userCode, out Int32 iErrorId)
        {
            return configurationGroupingDAL.InsertConfigurationGroup(configGroupCode, configGroupName, userCode, out iErrorId);
        }
    }
}
