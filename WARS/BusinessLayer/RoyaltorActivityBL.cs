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
    public class RoyaltorActivityBL:IRoyaltorActivityBL
    {
        IRoyaltorActivityDAL royaltorActivityDAL;
        #region Constructor
        public RoyaltorActivityBL()
        {
            royaltorActivityDAL = new RoyaltorActivityDAL();
        }
        #endregion Constructor
             
        public DataSet GetActivityData(out Int32 iErrorId)
        {
            return royaltorActivityDAL.GetActivityData(out iErrorId);
        }

        public DataSet UpdateRemoveFromRun(string levelFlag, string code, string loggedUser, string stmtPeriodId, out Int32 iErrorId)
        {
            return royaltorActivityDAL.UpdateRemoveFromRun(levelFlag, code, loggedUser, stmtPeriodId, out iErrorId);
        }

        public DataSet SetRetry(string levelFlag, string code, string loggedUser, string stmtPeriodId, out Int32 iErrorId)
        {
            return royaltorActivityDAL.SetRetry(levelFlag, code, loggedUser, stmtPeriodId, out iErrorId);
        }
    }
}
