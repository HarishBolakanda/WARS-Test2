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
    public class StatementTextMaintenanceBL : IStatementTextMaintenanceBL
    {
        IStatementTextMaintenanceDAL StatementTextMaintenanceDAL;
        #region Constructor
        public StatementTextMaintenanceBL()
        {
            StatementTextMaintenanceDAL = new StatementTextMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetDropDownData(out Int32 iErrorId)
        {
            return StatementTextMaintenanceDAL.GetDropDownData(out iErrorId);
        }

        public DataSet GetStatementTextDetails(string companyCode, out Int32 iErrorId)
        {
            return StatementTextMaintenanceDAL.GetStatementTextDetails(companyCode, out iErrorId);
        }

        public DataSet SaveStatementTextDetails(string companyCode, Array updateList, string loggedUser, out Int32 iErrorId)
        {
            return StatementTextMaintenanceDAL.SaveStatementTextDetails(companyCode, updateList, loggedUser, out iErrorId);
        }

    }
}
