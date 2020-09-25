using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WARS.IDAL
{
    public interface ITaxRateMaintananceDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetTaxRateData(string companyCode, out Int32 iErrorId);
        DataSet SaveTaxRate(string companyCode, string startDate, string endDate, string taxType, double taxRate, string userCode, out Int32 iErrorId);
        DataSet UpdateTaxRate(string companyCodeSearch, string companyCode, string taxNo, string endDate, double taxRate, string userCode, out Int32 iErrorId);
    }
}
