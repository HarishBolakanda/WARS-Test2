using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ICommonBL
    {
        /* WOS-389 - changes - removed by Harish
        void GetNextRuntime(out string nextRunTime, out string estCompletionTime, out Int32 iErrorId, out string sErrorMsg);*/
        void GetSummaryStmtCUID(out string summaryStmtCUID, out Int32 iErrorId);
        string GetDatabaseName();
        void RunRotaltyEngine(out string message, out Int32 iErrorId);
        void RunStatementArchive(out string message, out Int32 iErrorId);
        void LoadCostFile(out string message, out Int32 iErrorId);
        void LoadAdhocFile(out string message, out Int32 iErrorId);
        void GetErrorReportFolderCUID(out string errorReportFolderCUID, out Int32 iErrorId);
    }
}
