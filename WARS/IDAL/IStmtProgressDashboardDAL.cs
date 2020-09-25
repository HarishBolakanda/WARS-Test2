using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IStmtProgressDashboardDAL
    {        
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetFilterData(string stmtRptDays, string rptDaysPlus, string respCode, out Int32 iErrorId);        
    }
}
