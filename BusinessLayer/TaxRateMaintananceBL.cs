using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;

namespace WARS.BusinessLayer
{
    public class TaxRateMaintananceBL : ITaxRateMaintananceBL
    {
        ITaxRateMaintananceDAL TaxRateMaintananceDAL;
        #region Constructor
        public TaxRateMaintananceBL()
        {
            TaxRateMaintananceDAL = new TaxRateMaintananceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return TaxRateMaintananceDAL.GetInitialData( out iErrorId);
        }

        public DataSet GetTaxRateData(string companyCode, out Int32 iErrorId)
        {
            return TaxRateMaintananceDAL.GetTaxRateData(companyCode,out iErrorId);
        }

        public DataSet SaveTaxRate(string companyCode, string startDate, string endDate, string taxType, double taxRate, string userCode, out Int32 iErrorId)
        {
            return TaxRateMaintananceDAL.SaveTaxRate(companyCode, startDate, endDate, taxType, taxRate, userCode, out iErrorId);
        }

        public DataSet UpdateTaxRate(string companyCodeSearch, string companyCode, string taxNo, string endDate, double taxRate, string userCode, out Int32 iErrorId)
        {
            return TaxRateMaintananceDAL.UpdateTaxRate(companyCodeSearch,companyCode, taxNo, endDate, taxRate, userCode, out iErrorId);
        }
    }
}
