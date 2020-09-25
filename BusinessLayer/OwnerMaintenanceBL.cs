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
    public class OwnerMaintenanceBL : IOwnerMaintenanceBL
    {
        IOwnerMaintenanceDAL OwnerMaintenanceDAL;
        #region Constructor
        public OwnerMaintenanceBL()
        {
            OwnerMaintenanceDAL = new OwnerMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 newOwnerCode, out Int32 iErrorId)
        {
            return OwnerMaintenanceDAL.GetInitialData(out newOwnerCode, out iErrorId);
        }

        public DataSet UpdateOwnerData(Int32 ownerCode, string ownerName, string userCode, out Int32 newOwnerCode, out Int32 iErrorId)
        {
            return OwnerMaintenanceDAL.UpdateOwnerData(ownerCode, ownerName, userCode, out newOwnerCode, out iErrorId);
        }

        public DataSet InsertOwnerData(Int32 ownerCode, string ownerName, string userCode, out Int32 newOwnerCode, out Int32 iErrorId)
        {
            return OwnerMaintenanceDAL.InsertOwnerData(ownerCode, ownerName, userCode, out newOwnerCode, out iErrorId);
        }

        public DataSet DeleteOwnerData(Int32 ownerCode, string userCode, out Int32 newOwnerCode, out Int32 iErrorId)
        {
            return OwnerMaintenanceDAL.DeleteOwnerData(ownerCode, userCode, out newOwnerCode, out iErrorId);
        }
    }
}
