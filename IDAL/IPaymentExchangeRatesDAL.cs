using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IPaymentExchangeRatesDAL
    {
        DataSet GetInitialData(out string primaryCompanyCode, out Int32 iErrorId);
        DataSet GetPaymentExchangeRateData(string companyCode, out Int32 iErrorId);
        DataSet UpdatePaymentExchangeRates(string companyCode, string currencyCode, Int32 monthId, double paymentExchangeRate, string userCode, out Int32 iErrorId);
        DataSet InsertPaymentExchangeRates(string companyCode, string currencyCode, Int32 month, Int32 year, double paymentExchangeRate, string userCode, out Int32 iErrorId);
    }
}
