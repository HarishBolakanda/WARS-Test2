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
    public class ResponsibilityMaintenanceBL : IResponsibilityMaintenanceBL
    {
        IResponsibilityMaintenanceDAL ResponsibilityMaintenanceDAL;
        #region Constructor
        public ResponsibilityMaintenanceBL()
        {
            ResponsibilityMaintenanceDAL = new ResponsibilityMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return ResponsibilityMaintenanceDAL.GetInitialData(out iErrorId);
        }

        public DataSet UpdateResponsibilityData(string responsibilityCode, string responsibilityDesc, string managerResponsibility, string userCode, out Int32 iErrorId)
        {
            return ResponsibilityMaintenanceDAL.UpdateResponsibilityData(responsibilityCode, responsibilityDesc, managerResponsibility, userCode, out iErrorId);
        }

        public DataSet InsertResponsibilityData(string responsibilityCode, string responsibilityDesc, string managerResponsibility, string userCode, out Int32 iErrorId)
        {
            return ResponsibilityMaintenanceDAL.InsertResponsibilityData(responsibilityCode, responsibilityDesc, managerResponsibility, userCode, out iErrorId);
        }

        public DataSet DeleteResponsibilityData(string responsibilityCode, string userCode, out Int32 iErrorId)
        {
            return ResponsibilityMaintenanceDAL.DeleteResponsibilityData(responsibilityCode, userCode, out iErrorId);
        }
    }
}
