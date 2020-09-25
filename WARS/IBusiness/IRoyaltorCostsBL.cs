using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyaltorCostsBL
    {
        DataSet GetDropDownData(out Int32 iErrorId);
        DataSet GetStmtPeriods(string royaltor, out Int32 iErrorId);
        DataSet GetSearchData(string royaltor, string statementPeriod, string fromDate, string toDate, out Int32 iErrorId);
        DataSet AddCost(string royaltor, string statementPeriod, string fromDate, string toDate, string accountTypeId, string description, string date, string amount,
            string suppName, string projCode, string invNum, out Int32 iErrorId);
        DataSet UpdateCost(string journalEntryId, string royaltor, string statementPeriod, string fromDate, string toDate, string description, string date, string amount,
            string suppName, string projCode, string invNum, out Int32 iErrorId);
        DataSet DeleteCost(string journalEntryId, string royaltor, string statementPeriod, string fromDate, string toDate, out Int32 iErrorId);
    }
}
