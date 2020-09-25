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
    public class AdHocStatementBL : IAdHocStatementBL
    {
        IAdHocStatementDAL AdHocStatementDAL;
        #region Constructor
        public AdHocStatementBL()
        {
            AdHocStatementDAL = new AdHocStatementDAL();
        }
        #endregion Constructor


        public DataSet GetDropDownData(out Int32 iErrorId)
        {
            return AdHocStatementDAL.GetDropDownData(out iErrorId);
        }

        public DataSet GetOwnerGroupData(string ownerCode, out string isAllowed, out Int32 iErrorId)
        {
            return AdHocStatementDAL.GetOwnerGroupData(ownerCode, out isAllowed, out iErrorId);
        }

        public void SaveStatement(string descriptionStartDate, string descriptionEndDate, string stmtStartDate, string stmtEndDate, string paymentDate, Array roysToAdd, string loggedUser, out string stmtTypeCode, out Int32 iErrorId)
        {
            AdHocStatementDAL.SaveStatement(descriptionStartDate, descriptionEndDate, stmtStartDate, stmtEndDate, paymentDate, roysToAdd, loggedUser, out stmtTypeCode, out iErrorId);
        }



    }
}
