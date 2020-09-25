using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IStatementTextMaintenanceDAL
    {
        DataSet GetDropDownData(out Int32 iErrorId);
        DataSet GetStatementTextDetails(string companyCode, out Int32 iErrorId);
        DataSet SaveStatementTextDetails(string companyCode, Array updateList, string loggedUser, out Int32 iErrorId);
    }

}
