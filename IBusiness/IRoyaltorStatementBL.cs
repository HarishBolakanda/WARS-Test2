using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyaltorStatementBL
    {
        DataSet GetRoyaltors(out Int32 iErrorId);
        DataSet GetRoyStmtData(Int32 royaltorId, out Int32 iErrorId);
        DataSet UpdateRoyStmt(string royaltorID, Array stmtsToAdd, Array stmtsToRemove, string loggedUser, out Int32 iErrorId);
        DataSet GetOwnerStmtData(Int32 ownerCode, out Int32 iErrorId);
        DataSet UpdateOwnerStmt(Int32 ownerCode, Array royaltorIds, Array stmtsToAdd, Array stmtsToRemove, string loggedUser, out Int32 iErrorId);
    }
}
