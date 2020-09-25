using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IAccountTypeMaintenanceDAL 
    {
        DataSet GetInitialData(out Int32 iErrorId);

        DataSet UpdateAccountTypeMapData(string accoutCodeType, string isInclude, string sourceType,
            string consolidLevel, string isIncludeNew, string userCode, out Int32 iErrorId);

        DataSet InsertAccountTypeMapData(string accoutCodeType, string accountTypeId, string sourceType,
            string consolidLevel, string isInclude, string userCode, out Int32 iErrorId);

        DataSet InsertAccountTypeData(string accountTypeDesc, string displayOrder, string userCode, out Int32 iErrorId);
    }
}
