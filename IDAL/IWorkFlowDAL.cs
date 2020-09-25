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
        DataSet GetDropDownData(out Int32 iErrorId);

        DataSet GetFiltersForResp(string teamRespCode, string mngrRespCode, out Int32 iErrorId);

        DataSet GetWorkflowSearchData(string compCode, string rep_schedule, string owner, string royaltor, string teamResponsibility, string mngrResponsibility, string priority, string producer, string status,
                                      string earnings, string earningsCompare, string closingBalance, string closingBalanceCompare, out Int32 iErrorId);

        DataSet UpdateWorkflowStatus(string pageMode, string status, Array stmtRowsToUpdate, string compCode, string repSchedule, string owner,
                                     string royaltor, string teamResponsibility, string mngrResponsibility, string priority, string producer, string statusSearch,
                                     string earnings, string earningsCompare, string closingBalance, string closingBalanceCompare, string loggedUser, out Int32 iErrorId);

        DataSet GetComment(string royaltorId, string stmtPeriodId, out Int32 iErrorId);

        DataSet SaveComment(string royaltorID, string stmtPeriodId, string comment, string saveDelete, string compCodeSearch, string repScheduleSearch, string ownerSearch, string royaltorSearch,
                            string teamResponsibility, string mngrResponsibility, string prioritySearch, string producerSearch, string statusSearch,
                            string earnings, string earningsCompare, string closingBalance, string closingBalanceCompare, string userCode, out Int32 iErrorId);

        void UpdateRecreateStat(Array rowsToUpdate, string loggedUser, out Int32 iErrorId);

        void UpdateSummaryStmtDetails(string royaltorId, string stmtPeriodId, out string createFrontSheet, out string masterGroupedRoyaltor, out string summaryMasterRotaltor,
                                        out string stmtPeriodSortCode, out string createInvoice, out Int32 iErrorId);

        DataSet GetIntPartyIds(string royaltorID, out Int32 iErrorId);

        void GetStmtDirectory(out string stmtDirectory, out Int32 iErrorId);

        void RecalSummaryBulk(Array royaltorStmts, string loggedUser, out Int32 iErrorId);

    }
}


