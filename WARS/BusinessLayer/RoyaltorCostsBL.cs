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
    public class RoyaltorCostsBL:IRoyaltorCostsBL
    {
        IRoyaltorCostsDAL RoyaltorCostsDAL;
        #region Constructor
        public RoyaltorCostsBL()
        {
            RoyaltorCostsDAL = new RoyaltorCostsDAL();
        }
        #endregion Constructor


        public DataSet GetDropDownData(out Int32 iErrorId)
        {
            return RoyaltorCostsDAL.GetDropDownData(out iErrorId);
        }

        public DataSet GetStmtPeriods(string royaltor, out Int32 iErrorId)
        {
            return RoyaltorCostsDAL.GetStmtPeriods(royaltor, out iErrorId);
        }


        public DataSet GetSearchData(string royaltor, string statementPeriod,  string fromDate, string toDate, out Int32 iErrorId)
        {
            return RoyaltorCostsDAL.GetSearchData(royaltor, statementPeriod, fromDate, toDate, out iErrorId);
        }

        public DataSet AddCost(string royaltor, string statementPeriod,  string fromDate, string toDate, string accountTypeId, string description, string date, string amount,
            string suppName, string projCode, string invNum, out Int32 iErrorId)
        {
            return RoyaltorCostsDAL.AddCost(royaltor, statementPeriod, fromDate, toDate, accountTypeId, description, date, amount, suppName, projCode, invNum, out iErrorId);
        }

        public DataSet UpdateCost(string journalEntryId, string royaltor, string statementPeriod,  string fromDate, string toDate, string description, string date, string amount,
            string suppName, string projCode, string invNum, out Int32 iErrorId)
        {
            return RoyaltorCostsDAL.UpdateCost(journalEntryId, royaltor, statementPeriod, fromDate, toDate, description, date, amount, suppName, projCode, invNum, out iErrorId);
        }

        public DataSet DeleteCost(string journalEntryId, string royaltor, string statementPeriod,  string fromDate, string toDate, out Int32 iErrorId)
        {
            return RoyaltorCostsDAL.DeleteCost(journalEntryId, royaltor, statementPeriod, fromDate, toDate, out iErrorId);
        }

        

    }
}
