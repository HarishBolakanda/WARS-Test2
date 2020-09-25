using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IAdHocStatementDAL
    {
        DataSet GetDropDownData(out Int32 iErrorId);
        DataSet GetOwnerGroupData(string ownerCode, out string isAllowed, out Int32 iErrorId);
        void SaveStatement(string descriptionStartDate, string descriptionEndDate, string stmtStartDate, string stmtTypeCode, string paymentDate, Array roysToAdd, string loggedUser, out string stmtEndDate, out Int32 iErrorId);
    }
}
