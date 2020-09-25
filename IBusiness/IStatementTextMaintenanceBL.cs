using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IStatementTextMaintenanceBL
    {
        DataSet GetDropDownData(out Int32 iErrorId);
        DataSet GetStatementTextDetails(string companyCode, out Int32 iErrorId);
        DataSet SaveStatementTextDetails(string companyCode, Array updateList, string loggedUser, out Int32 iErrorId);

    }
}
