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
    public class AccountTypeMaintenanceBL : IAccountTypeMaintenanceBL
    {
        IAccountTypeMaintenanceDAL AccountTypeMaintenanceDAL;
        #region Constructor
        public AccountTypeMaintenanceBL()
        {
            AccountTypeMaintenanceDAL = new AccountTypeMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return AccountTypeMaintenanceDAL.GetInitialData(out iErrorId);
        }

        public DataSet UpdateAccountTypeMapData(string accoutCodeType, string isInclude, string sourceType,
            string consolidLevel, string isIncludeNew, string userCode, out Int32 iErrorId)
        {
            return AccountTypeMaintenanceDAL.UpdateAccountTypeMapData(accoutCodeType, isInclude, sourceType, consolidLevel, isIncludeNew, userCode, out iErrorId);
        }

        public DataSet InsertAccountTypeMapData(string accoutCodeType, string accountTypeId, string sourceType,
            string consolidLevel, string isInclude, string userCode, out Int32 iErrorId)
        {
            return AccountTypeMaintenanceDAL.InsertAccountTypeMapData(accoutCodeType, accountTypeId, sourceType, consolidLevel, isInclude, userCode, out iErrorId);
        }

        public DataSet InsertAccountTypeData(string accountTypeDesc, string displayOrder, string userCode, out Int32 iErrorId)
        {
            return AccountTypeMaintenanceDAL.InsertAccountTypeData(accountTypeDesc, displayOrder, userCode, out iErrorId);
        }

    }
}
