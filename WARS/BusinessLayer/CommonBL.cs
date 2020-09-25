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
    public class CommonBL:ICommonBL
    {
        ICommonDAL CommonDAL;
        #region Constructor
        public CommonBL()
        {
            CommonDAL = new CommonDAL();
        }
        #endregion Constructor

        /* WOS-389 - changes - modified by Harish
        public void GetNextRuntime(out string nextRunTime, out string estCompletionTime, out Int32 iErrorId, out string sErrorMsg)
        {
            CommonDAL.GetNextRuntime(out nextRunTime,out estCompletionTime, out iErrorId,out sErrorMsg);
        }
         * */

        public void GetSummaryStmtCUID(out string summaryStmtCUID, out Int32 iErrorId)
        {
            CommonDAL.GetSummaryStmtCUID(out summaryStmtCUID, out iErrorId);
        }

        public string GetDatabaseName()
        {
            return CommonDAL.GetDatabaseName();
        }

        public void RunRotaltyEngine(out string message, out Int32 iErrorId)
        {
            CommonDAL.RunRotaltyEngine(out message, out iErrorId);
        }

        public void RunStatementArchive(out string message, out Int32 iErrorId)
        {
            CommonDAL.RunStatementArchive(out message, out iErrorId);
        }

        public void LoadCostFile(out string message, out Int32 iErrorId)
        {
            CommonDAL.LoadCostFile(out message, out iErrorId);
        }

        public void LoadAdhocFile(out string message, out Int32 iErrorId)
        {
            CommonDAL.LoadAdhocFile(out message, out iErrorId);
        }

        public void GetErrorReportFolderCUID(out string errorReportFolderCUID, out Int32 iErrorId)
        {
            CommonDAL.GetErrorReportFolderCUID(out errorReportFolderCUID, out iErrorId);
        }
    }
}
