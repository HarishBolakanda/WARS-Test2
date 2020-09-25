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
    public class PaymentExchangeRatesBL : IPaymentExchangeRatesBL
    {
        IPaymentExchangeRatesDAL PaymentExchangeRatesDAL;
        #region Constructor
        public PaymentExchangeRatesBL()
        {
            PaymentExchangeRatesDAL = new PaymentExchangeRatesDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out string primaryCompanyCode, out Int32 iErrorId)
        {
            return PaymentExchangeRatesDAL.GetInitialData(out primaryCompanyCode,out iErrorId);
        }

        public DataSet GetPaymentExchangeRateData(string companyCode, out Int32 iErrorId)
        {
            return PaymentExchangeRatesDAL.GetPaymentExchangeRateData(companyCode,out iErrorId);
        }

        public DataSet UpdatePaymentExchangeRates(string companyCode, string currencyCode, Int32 monthId, double paymentExchangeRate, string userCode, out Int32 iErrorId)
        {
            return PaymentExchangeRatesDAL.UpdatePaymentExchangeRates(companyCode,currencyCode, monthId, paymentExchangeRate, userCode, out iErrorId);
        }

        public DataSet InsertPaymentExchangeRates(string companyCode, string currencyCode, Int32 month, Int32 year, double paymentExchangeRate, string userCode, out Int32 iErrorId)
        {
            return PaymentExchangeRatesDAL.InsertPaymentExchangeRates(companyCode,currencyCode, month, year, paymentExchangeRate, userCode, out iErrorId);
        }              
    }
}
