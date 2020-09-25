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
    public class TransactionMaintenanceBL : ITransactionMaintenanceBL
    {
        ITransactionMaintenanceDAL TransactionMaintenanceDAL;
        #region Constructor
        public TransactionMaintenanceBL()
        {
            TransactionMaintenanceDAL = new TransactionMaintenanceDAL();
        }
        #endregion Constructor
        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return TransactionMaintenanceDAL.GetInitialData(out iErrorId);
        }
        public DataSet GetSearchedTransactionsData(string seller, string salesType, string receivedDate, string reportedDate, string catno, string companyCode, out Int32 iErrorId)
        {
            return TransactionMaintenanceDAL.GetSearchedTransactionsData(seller, salesType, receivedDate, reportedDate, catno, companyCode, out iErrorId);
        }
        public void AddTransactionDetails(string addTransReceivedDate, string addTransSalesType, string addTransSales1, string addTransReceipts, string addTransDolExchRate,
            string addTransReportedDate, string addTransPrice1, string addTransSales2,
                                           string addTransReceipts2, string addTransCurrencyCode, string addTransCatNo, string addTransPrice2, string addTransSales3,
                                           string addTransReceipts3, string addTransWhtTax, string addTransSeller, string addTransPrice3, string addTransDestinationCountry, string addCompanyCode,
            string addOwnerShare, string addTotalShare, string userCode, out Int32 iErrorId)
        {
             TransactionMaintenanceDAL.AddTransactionDetails(addTransReceivedDate, addTransSalesType, addTransSales1, addTransReceipts, addTransDolExchRate,
            addTransReportedDate, addTransPrice1, addTransSales2, addTransReceipts2, addTransCurrencyCode, addTransCatNo, addTransPrice2, addTransSales3,
                                           addTransReceipts3, addTransWhtTax, addTransSeller, addTransPrice3, addTransDestinationCountry, addCompanyCode, addOwnerShare, addTotalShare, userCode, out iErrorId);
        }
        public DataSet UpdateTransactionDetails(string seller, string salesType, string receivedDate, string reportedDate, string catno, string companyCode, Array transToUpdate, Array transToDelete, string userCode, out Int32 iErrorId)
        {
            return TransactionMaintenanceDAL.UpdateTransactionDetails(seller, salesType, receivedDate, reportedDate, catno, companyCode, transToUpdate, transToDelete, userCode, out iErrorId);
        }
    }
}
