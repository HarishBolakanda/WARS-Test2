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
    public class RoyaltorStatementBL:IRoyaltorStatementBL
    {
        IRoyaltorStatementDAL RoyaltorStatementDAL;
        #region Constructor
        public RoyaltorStatementBL()
        {
            RoyaltorStatementDAL = new RoyaltorStatementDAL();
        }
        #endregion Constructor


        public DataSet GetRoyaltors(out Int32 iErrorId)
        {
            return RoyaltorStatementDAL.GetRoyaltors(out iErrorId);
        }

        public DataSet GetRoyStmtData(Int32 royaltorId, out Int32 iErrorId)
        {
            return RoyaltorStatementDAL.GetRoyStmtData(royaltorId, out iErrorId);
        }

        public DataSet UpdateRoyStmt(string royaltorID, Array stmtsToAdd, Array stmtsToRemove, string loggedUser, out Int32 iErrorId)
        {
            return RoyaltorStatementDAL.UpdateRoyStmt(royaltorID, stmtsToAdd, stmtsToRemove, loggedUser, out iErrorId);
        }
      
    }
}
