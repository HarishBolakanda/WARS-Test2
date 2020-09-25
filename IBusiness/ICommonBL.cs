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
        void RunRotaltyEngine(string refreshType, out string message, out Int32 iErrorId);
        void RunStatementArchive(out Int32 iErrorId);
        void LoadAdhocCostFile(out string message, out Int32 iErrorId);        
        void RunAccrualProcess(string flag,out string message, out Int32 iErrorId);
        void GeneratePayments(out Int32 iErrorId);
        void RunAutoConsolidate(out Int32 iErrorId);
        void GetSharedPaths(out string pdfStatementPath, out string fileUploadPath, out Int32 iErrorId);
        void GetAdhocStatementOption(out string adhocStatementOption, out Int32 iErrorId);
    }
}
