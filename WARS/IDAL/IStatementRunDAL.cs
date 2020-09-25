using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IStatementRunDAL
    {
        
        DataSet GetStmtRunData(string stmtEndPeriod, out Int32 iErrorId);
        DataSet UpdatestmtRunData(Array stmtsToRun, Array stmtsToRerun, Array stmtsToArchive, string stmtEndPeriod, string loggedUser, out Int32 iErrorId);        
        
    }
}
