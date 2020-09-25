using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ICompanyMaintenanceDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetSearchedCompanyData(Int32 companyCode, out Int32 iErrorId);
        DataSet UpdateCompanyData(Int32 companyCode, string companyName, string companyDesc, string companyAddress1, string companyAddress2, string companyAddress3,
                string companyAddress4, string currencyCode, string domesticCurrencyGroup, string paymentThreshold, string thresholdRecouped, string thresholdUnrecouped,
                string isPrimary, string isDisplayVat, string accountCompany, string userCode, out Int32 iErrorId);
        DataSet InsertCompanyData(string companyName, string companyDesc, string companyAddress1, string companyAddress2, string companyAddress3,
                string companyAddress4, string currencyCode, string domesticCurrencyGroup, string paymentThreshold, string thresholdRecouped, string thresholdUnrecouped,
                string isPrimary, string isDisplayVat, string accountCompany, string userCode, out Int32 iErrorId);
    }
}
