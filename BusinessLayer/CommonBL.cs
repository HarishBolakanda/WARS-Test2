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

        public void RunRotaltyEngine(string refreshType, out string message, out Int32 iErrorId)
        {
            CommonDAL.RunRotaltyEngine(refreshType, out message, out iErrorId);
        }

        public void RunStatementArchive(out Int32 iErrorId)
        {
            CommonDAL.RunStatementArchive(out iErrorId);
        }

        public void LoadAdhocCostFile(out string message, out Int32 iErrorId)
        {
            CommonDAL.LoadAdhocCostFile(out message, out iErrorId);
        }

        public void RunAccrualProcess(string flag, out string message, out Int32 iErrorId)
        {
            CommonDAL.RunAccrualProcess(flag,out message, out iErrorId);
        }

        public void GeneratePayments(out Int32 iErrorId)
        {
            CommonDAL.GeneratePayments(out iErrorId);
        }

        public void RunAutoConsolidate(out Int32 iErrorId)
        {
            CommonDAL.RunAutoConsolidate(out iErrorId);
        }

        public void GetSharedPaths(out string pdfStatementPath, out string fileUploadPath,out Int32 iErrorId)
        {
            CommonDAL.GetSharedPaths(out pdfStatementPath, out fileUploadPath,out iErrorId);
        }

        public void GetAdhocStatementOption(out string adhocStatementOption, out Int32 iErrorId)
        {
            CommonDAL.GetAdhocStatementOption(out adhocStatementOption,out iErrorId);
        }
    }
}
