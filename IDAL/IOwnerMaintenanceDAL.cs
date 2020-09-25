using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IOwnerMaintenanceDAL
    {
        DataSet GetInitialData(out Int32 newOwnerCode, out Int32 iErrorId);
        DataSet UpdateOwnerData(Int32 ownerCode, string ownerName, string userCode, out Int32 newOwnerCode, out Int32 iErrorId);
        DataSet InsertOwnerData(Int32 ownerCode, string ownerName, string userCode, out Int32 newOwnerCode, out Int32 iErrorId);
        DataSet DeleteOwnerData(Int32 ownerCode, string userCode, out Int32 newOwnerCode, out Int32 iErrorId);
    }
}
