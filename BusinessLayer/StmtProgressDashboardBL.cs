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
    public class StmtProgressDashboardBL : IStmtProgressDashboardBL
    {
        IStmtProgressDashboardDAL StmtProgressDashboardDAL;
        #region Constructor
        public StmtProgressDashboardBL()
        {
            StmtProgressDashboardDAL = new StmtProgressDashboardDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return StmtProgressDashboardDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetFilterData(string companyCode, string repSchedule, string teamResponsibility, string mngrResponsibility, string earningsCompare, string earnings, out Int32 iErrorId)
        {
            return StmtProgressDashboardDAL.GetFilterData(companyCode, repSchedule, teamResponsibility, mngrResponsibility, earningsCompare, earnings, out iErrorId);
        }


    }
}
