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


        public DataSet GetDropDownData(out Int32 iErrorId)
        {
            return WorkFlowDAL.GetDropDownData(out iErrorId);
        }

        public DataSet GetFiltersForResp(string teamRespCode, string mngrRespCode, out Int32 iErrorId)
        {
            return WorkFlowDAL.GetFiltersForResp(teamRespCode, mngrRespCode, out iErrorId);
        }

        public DataSet GetWorkflowSearchData(string compCode, string rep_schedule, string owner, string royaltor, string teamResponsibility, string mngrResponsibility, string priority, string producer, string status,
                                             string earnings, string earningsCompare, string closingBalance, string closingBalanceCompare, out Int32 iErrorId)
        {
            return WorkFlowDAL.GetWorkflowSearchData(compCode, rep_schedule, owner, royaltor, teamResponsibility, mngrResponsibility, priority, producer, status, earnings, earningsCompare,
                                                     closingBalance, closingBalanceCompare, out iErrorId);
        }

        public DataSet UpdateWorkflowStatus(string pageMode, string status, Array stmtRowsToUpdate, string compCode, string repSchedule, string owner,
                                     string royaltor, string teamResponsibility, string mngrResponsibility, string priority, string producer, string statusSearch,
                                     string earnings, string earningsCompare, string closingBalance, string closingBalanceCompare, string loggedUser, out Int32 iErrorId)
        {
            return WorkFlowDAL.UpdateWorkflowStatus(pageMode, status, stmtRowsToUpdate, compCode, repSchedule, owner, royaltor, teamResponsibility, mngrResponsibility, priority,
                                                    producer, statusSearch, earnings, earningsCompare, closingBalance, closingBalanceCompare, loggedUser, out iErrorId);
        }

        public DataSet GetComment(string royaltorId, string stmtPeriodId, out Int32 iErrorId)
        {
            return WorkFlowDAL.GetComment(royaltorId, stmtPeriodId, out iErrorId);
        }

        public DataSet SaveComment(string royaltorID, string stmtPeriodId, string comment, string saveDelete, string compCodeSearch, string repScheduleSearch, string ownerSearch, string royaltorSearch,
                                   string teamResponsibility, string mngrResponsibility, string prioritySearch, string producerSearch, string statusSearch,
                                   string earnings, string earningsCompare, string closingBalance, string closingBalanceCompare, string userCode, out Int32 iErrorId)
        {
            return WorkFlowDAL.SaveComment(royaltorID, stmtPeriodId, comment, saveDelete, compCodeSearch, repScheduleSearch, ownerSearch, royaltorSearch, teamResponsibility, mngrResponsibility, prioritySearch,
                                            producerSearch, statusSearch, earnings, earningsCompare, closingBalance, closingBalanceCompare, userCode, out iErrorId);
        }

        public void UpdateRecreateStat(Array rowsToUpdate, string loggedUser, out Int32 iErrorId)
        {
            WorkFlowDAL.UpdateRecreateStat(rowsToUpdate, loggedUser, out iErrorId);
        }

        public void UpdateSummaryStmtDetails(string royaltorId, string stmtPeriodId, out string createFrontSheet, out string masterGroupedRoyaltor, out string summaryMasterRotaltor,
                                             out string stmtPeriodSortCode, out string createInvoice, out Int32 iErrorId)
        {
            WorkFlowDAL.UpdateSummaryStmtDetails(royaltorId, stmtPeriodId, out createFrontSheet, out masterGroupedRoyaltor, out summaryMasterRotaltor, out stmtPeriodSortCode, out createInvoice, out iErrorId);
        }

        public DataSet GetIntPartyIds(string royaltorID, out Int32 iErrorId)
        {
            return WorkFlowDAL.GetIntPartyIds(royaltorID, out iErrorId);
        }

        public void GetStmtDirectory(out string stmtDirectory, out Int32 iErrorId)
        {
            WorkFlowDAL.GetStmtDirectory(out stmtDirectory, out iErrorId);
        }

        public void RecalSummaryBulk(Array royaltorStmts, string loggedUser, out Int32 iErrorId)
        {
            WorkFlowDAL.RecalSummaryBulk(royaltorStmts, loggedUser, out iErrorId);
        }

    }
}
