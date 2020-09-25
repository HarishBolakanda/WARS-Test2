using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IAdHocStatementBL
    {
        DataSet GetDropDownData(out Int32 iErrorId);
        DataSet GetOwnerGroupData(string ownerCode, out string isAllowed, out Int32 iErrorId);
        void SaveStatement(string descriptionStartDate, string descriptionEndDate, string stmtStartDate, string stmtTypeCode, string paymentDate, Array roysToAdd, string loggedUser, out string stmtEndDate, out Int32 iErrorId);
    }
}
