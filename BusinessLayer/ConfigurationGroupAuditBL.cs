using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;

namespace WARS.BusinessLayer
{
    public class ConfigurationGroupAuditBL : IConfigurationGroupAuditBL
    {
        IConfigurationGroupAuditDAL ConfigurationGroupAuditDAL;
        #region Constructor
        public ConfigurationGroupAuditBL()
        {
            ConfigurationGroupAuditDAL = new ConfigurationGroupAuditDAL();
        }
        #endregion Constructor


        public DataSet GetConfigurationGroupAuditData(string ConfigurationGroupCode, string fromDate, string toDate, out Int32 iErrorId)
        {
            return ConfigurationGroupAuditDAL.GetConfigurationGroupAuditData(ConfigurationGroupCode, fromDate, toDate, out iErrorId);
        }
    }
}
