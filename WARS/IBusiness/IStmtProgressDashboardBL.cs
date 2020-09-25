using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IStmtProgressDashboardBL
    {        
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetFilterData(string stmtRptDays, string rptDaysPlus,string respCode, out Int32 iErrorId);        
    }
}
