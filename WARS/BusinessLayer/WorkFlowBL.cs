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
    public class WorkFlowBL:IWorkFlowBL
    {
        IWorkFlowDAL WorkFlowDAL;
        #region Constructor
        public WorkFlowBL()
        {
            WorkFlowDAL = new WorkFlowDAL();
        }
        #endregion Constructor


        public DataSet GetDropDownData(string loggedUser, out string defaultThresholds, out Int32 iErrorId)
        {
            return WorkFlowDAL.GetDropDownData(loggedUser, out defaultThresholds, out iErrorId);
        }

        public DataSet GetFiltersForResp(string respCode, out Int32 iErrorId)
        {
            return WorkFlowDAL.GetFiltersForResp(respCode, out iErrorId);
        }

        /* removed by harish (12-01-2017) as it is no longer being used
        public DataSet GetWorkflowData(string loggedUser, out string respCode, out Int32 iErrorId)        
        {
            return WorkFlowDAL.GetWorkflowData(loggedUser, out respCode, out iErrorId);            
        }*/


        public DataSet GetWorkflowSearchData(string compCode, string rep_schedule, string owner, string royaltor, string responsibility, string priority, string status,
            string recoupedThreshold, string unrecoupedThreshold, out Int32 iErrorId)
        {
            return WorkFlowDAL.GetWorkflowSearchData(compCode, rep_schedule, owner, royaltor, responsibility, priority, status, recoupedThreshold, unrecoupedThreshold,
                  out iErrorId);
        }

        public DataSet UpdateWorkflowStatus(string pageMode, string royaltorID, string stmtPeriodId, string ownerCode, string status, string compCode, string rep_schedule, string owner, string royaltor, string responsibility,
                                     string priority, string status_search, string recoupedThreshold, string unrecoupedThreshold, string loggedUser,
                                     out Int32 iErrorId)
        {
            return WorkFlowDAL.UpdateWorkflowStatus(pageMode, royaltorID, stmtPeriodId, ownerCode, status, compCode, rep_schedule, owner, royaltor, responsibility, priority,
                                                    status_search, recoupedThreshold, unrecoupedThreshold, loggedUser, out iErrorId);
        }

        public void UpdateRecreateStat(Array rowsToUpdate, string loggedUser, out Int32 iErrorId)
        {
            WorkFlowDAL.UpdateRecreateStat(rowsToUpdate, loggedUser, out iErrorId);
        }

        public void UpdateSummaryStmtDetails(string royaltorId, string stmtPeriodId, out string createFrontSheet, out string masterGroupedRoyaltor,
                                                   out string stmtPeriodSortCode, out string createInvoice, out Int32 iErrorId)
        {
            WorkFlowDAL.UpdateSummaryStmtDetails(royaltorId, stmtPeriodId, out createFrontSheet, out masterGroupedRoyaltor, out stmtPeriodSortCode, out createInvoice, out iErrorId);
        }

        public DataSet GetCourtesySharIds(string royaltorID, out Int32 iErrorId)
        {
            return WorkFlowDAL.GetCourtesySharIds(royaltorID, out iErrorId);
        }

        public void GetStmtDirectory(out string stmtDirectory, out Int32 iErrorId)
        {
            WorkFlowDAL.GetStmtDirectory(out stmtDirectory, out iErrorId);
        }

    }
}
