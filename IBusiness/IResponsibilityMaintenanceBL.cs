using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IResponsibilityMaintenanceBL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet UpdateResponsibilityData(string responsibilityCode, string responsibilityDesc, string managerResponsibility, string userCode, out Int32 iErrorId);
        DataSet InsertResponsibilityData(string responsibilityCode, string responsibilityDesc, string managerResponsibility, string userCode, out Int32 iErrorId);
        DataSet DeleteResponsibilityData(string responsibilityCode, string userCode, out Int32 iErrorId);
    }
}
