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
    public class ExchangeRateFactorsBL : IExchangeRateFactorsBL
    {
        IExchangeRateFactorsDAL ExchangeRateFactorsDAL;
        #region Constructor
        public ExchangeRateFactorsBL()
        {
            ExchangeRateFactorsDAL = new ExchangeRateFactorsDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return ExchangeRateFactorsDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetCompanyData(string companyCode, out Int32 iErrorId)
        {
            return ExchangeRateFactorsDAL.GetCompanyData(companyCode, out iErrorId);
        }

        public DataSet UpdateExchangeRateFactor(string companyCode, Int32 monthId, double exchangeRateFactor, string userCode, out Int32 iErrorId)
        {
            return ExchangeRateFactorsDAL.UpdateExchangeRateFactor(companyCode, monthId, exchangeRateFactor, userCode, out iErrorId);
        }

        public DataSet InsertExchangeRateFactor(string companyCode, Int32 month, Int32 year, double exchangeRateFactor, string userCode, out Int32 iErrorId)
        {
            return ExchangeRateFactorsDAL.InsertExchangeRateFactor(companyCode, month, year, exchangeRateFactor, userCode, out iErrorId);
        }

        //public DataSet UpdateTransactions(string companyCode, string currencyCode, string domesticCurrencyGroup, Array monthIds, Array exchangeRateFactor, string userCode, out Int32 iErrorId)
        //{
        //    return ExchangeRateFactorsDAL.UpdateTransactions(companyCode, currencyCode, domesticCurrencyGroup, monthIds, exchangeRateFactor, userCode, out iErrorId);
        //}

    }
}
