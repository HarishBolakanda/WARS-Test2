using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IStatementRunBL
    {
        DataSet GetStmtRunData(string stmtEndPeriod, out Int32 iErrorId);
        DataSet UpdatestmtRunData(Array stmtsToRun, Array stmtsToRerun, Array stmtsToArchive, string stmtEndPeriod, string loggedUser, out Int32 iErrorId);        
    }
}
