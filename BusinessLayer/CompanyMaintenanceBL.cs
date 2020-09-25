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
    public class CompanyMaintenanceBL : ICompanyMaintenanceBL
    {
        ICompanyMaintenanceDAL CompanyMaintenanceDAL;
        #region Constructor
        public CompanyMaintenanceBL()
        {
            CompanyMaintenanceDAL = new CompanyMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return CompanyMaintenanceDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetSearchedCompanyData(Int32 companyCode, out Int32 iErrorId)
        {
            return CompanyMaintenanceDAL.GetSearchedCompanyData(companyCode, out iErrorId);
        }

        public DataSet UpdateCompanyData(Int32 companyCode, string companyName, string companyDesc, string companyAddress1, string companyAddress2, string companyAddress3,
                string companyAddress4, string currencyCode, string domesticCurrencyGroup, string paymentThreshold, string thresholdRecouped, string thresholdUnrecouped,
                string isPrimary, string isDisplayVat, string accountCompany, string userCode, out Int32 iErrorId)
        {
            return CompanyMaintenanceDAL.UpdateCompanyData(companyCode, companyName, companyDesc, companyAddress1, companyAddress2, companyAddress3,
                 companyAddress4, currencyCode, domesticCurrencyGroup, paymentThreshold, thresholdRecouped, thresholdUnrecouped,
                 isPrimary,isDisplayVat, accountCompany, userCode, out iErrorId);
        }

        public DataSet InsertCompanyData(string companyName, string companyDesc, string companyAddress1, string companyAddress2, string companyAddress3,
                string companyAddress4, string currencyCode, string domesticCurrencyGroup, string paymentThreshold, string thresholdRecouped, string thresholdUnrecouped,
                string isPrimary, string isDisplayVat, string accountCompany, string userCode, out Int32 iErrorId)
        {
            return CompanyMaintenanceDAL.InsertCompanyData(companyName, companyDesc, companyAddress1, companyAddress2, companyAddress3,
                 companyAddress4, currencyCode, domesticCurrencyGroup, paymentThreshold, thresholdRecouped, thresholdUnrecouped,
                 isPrimary, isDisplayVat, accountCompany, userCode, out iErrorId);
        }
    }
}
