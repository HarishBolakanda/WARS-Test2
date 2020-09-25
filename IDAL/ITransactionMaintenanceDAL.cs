using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ITransactionMaintenanceDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetSearchedTransactionsData(string seller, string salesType, string receivedDate, string reportedDate, string catno, string companyCode, out Int32 iErrorId);
        void AddTransactionDetails(string addTransReceivedDate, string addTransSalesType, string addTransSales1, string addTransReceipts, string addTransDolExchRate,
            string addTransReportedDate, string addTransPrice1, string addTransSales2,
                                           string addTransReceipts2, string addTransCurrencyCode, string addTransCatNo, string addTransPrice2, string addTransSales3,
                                           string addTransReceipts3, string addTransWhtTax, string addTransSeller, string addTransPrice3, string addTransDestinationCountry,
            string addOwnerShare, string addTotalShare, string addCompanyCode, string userCode, out Int32 iErrorId);
        DataSet UpdateTransactionDetails(string seller, string salesType, string receivedDate, string reportedDate, string catno, string companyCode, Array transToUpdate, Array transToDelete, string userCode, out Int32 iErrorId);
    }
}
