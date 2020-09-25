using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IWorkFlowDAL
    {
        DataSet GetDropDownData(string loggedUser, out string defaultThresholds, out Int32 iErrorId);
        DataSet GetFiltersForResp(string respCode, out Int32 iErrorId);
        /* removed by harish (12-01-2017) as it is no longer being used
        DataSet GetWorkflowData(string loggedUser, out string respCode, out Int32 iErrorId);        
         * */
        DataSet GetWorkflowSearchData(string compCode, string rep_schedule,string owner,string royaltor, string responsibility, string priority,string status,
            string recoupedThreshold, string unrecoupedThreshold, out Int32 iErrorId);
        DataSet UpdateWorkflowStatus(string pageMode, string royaltorID, string stmtPeriodId, string ownerCode, string status, string compCode, string rep_schedule, string owner, string royaltor, string responsibility,
                                     string priority, string status_search, string recoupedThreshold, string unrecoupedThreshold, string loggedUser,
                                     out Int32 iErrorId);
        void UpdateRecreateStat(Array rowsToUpdate, string loggedUser, out Int32 iErrorId);
        void UpdateSummaryStmtDetails(string royaltorId, string stmtPeriodId, out string createFrontSheet,  out string masterGroupedRoyaltor,
                                        out string stmtPeriodSortCode, out string createInvoice, out Int32 iErrorId);
        DataSet GetCourtesySharIds(string royaltorID, out Int32 iErrorId);
        void GetStmtDirectory(out string stmtDirectory, out Int32 iErrorId);
    }
}
