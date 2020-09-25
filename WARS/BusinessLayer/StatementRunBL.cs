using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;


namespace WARS.BusinessLayer
{
    public class StatementRunBL:IStatementRunBL
    {
        IStatementRunDAL StatementRunDAL;
        #region Constructor
        public StatementRunBL()
        {
            StatementRunDAL = new StatementRunDAL();
        }
        #endregion Constructor
       

        public DataSet GetStmtRunData(string stmtEndPeriod, out Int32 iErrorId)
        {
            return StatementRunDAL.GetStmtRunData(stmtEndPeriod, out iErrorId);
        }

        public DataSet UpdatestmtRunData(Array stmtsToRun, Array stmtsToRerun, Array stmtsToArchive, string stmtEndPeriod, string loggedUser, out Int32 iErrorId)
        {
            return StatementRunDAL.UpdatestmtRunData(stmtsToRun,stmtsToRerun, stmtsToArchive, stmtEndPeriod, loggedUser, out iErrorId);
        }
      
    }
}
