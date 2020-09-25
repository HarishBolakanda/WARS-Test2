using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyaltorActivityBL
    {
        /*WOS-389 - changes
        DataSet GetActivityData(out string nextRunTime, out Int32 iErrorId);
        DataSet UpdateRemoveFromRun(string levelFlag, string code, string loggedUser, string stmtPeriodId, out string nextRunTime, out Int32 iErrorId);
         * */
        DataSet GetActivityData(out Int32 iErrorId);
        DataSet UpdateRemoveFromRun(string levelFlag, string code, string loggedUser, string stmtPeriodId, out Int32 iErrorId);
        DataSet SetRetry(string levelFlag, string code, string loggedUser, string stmtPeriodId, out Int32 iErrorId);
    }
}
