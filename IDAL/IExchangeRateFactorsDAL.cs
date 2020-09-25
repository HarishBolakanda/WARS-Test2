using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IExchangeRateFactorsDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetCompanyData(string companyCode, out Int32 iErrorId);
        DataSet UpdateExchangeRateFactor(string companyCode, Int32 monthId, double exchangeRateFactor, string userCode, out Int32 iErrorId);
        DataSet InsertExchangeRateFactor(string companyCode, Int32 month, Int32 year, double exchangeRateFactor, string userCode, out Int32 iErrorId);
        //DataSet UpdateTransactions(string companyCode, string currencyCode, string domesticCurrencyGroup, Array monthIds, Array exchangeRateFactor, string userCode, out Int32 iErrorId);
    }
}
