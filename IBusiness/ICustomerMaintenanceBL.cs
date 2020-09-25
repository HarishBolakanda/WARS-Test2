using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ICustomerMaintenanceBL
    {
        DataSet GetInitialData(out Int32 iErrorId);

        DataSet UpdateCustomerData(Int32 customerCode, string fixedMobile,
            string displayOnStatementGlobal, string displayOnStatementAcount, string userCode, out Int32 iErrorId);
    }
}
