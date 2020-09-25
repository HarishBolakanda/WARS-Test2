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
    public class CustomerMaintenanceBL : ICustomerMaintenanceBL
    {
        ICustomerMaintenanceDAL CustomerMaintenanceDAL;
        #region Constructor
        public CustomerMaintenanceBL()
        {
            CustomerMaintenanceDAL = new CustomerMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return CustomerMaintenanceDAL.GetInitialData(out iErrorId);
        }

        public DataSet UpdateCustomerData(Int32 customerCode, string fixedMobile,
            string displayOnStatementGlobal, string displayOnStatementAcount, string userCode, out Int32 iErrorId)
        {
            return CustomerMaintenanceDAL.UpdateCustomerData(customerCode, fixedMobile, displayOnStatementGlobal, displayOnStatementAcount, userCode, out iErrorId);
        }

    }
}
