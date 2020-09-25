using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyContractPayeeSuppDAL
    {
        DataSet GetDropdownData(string royaltorId, out Int32 iErrorId);
        DataSet GetInitialData(string royaltorId, out string royaltor, out string supplierSiteNameRegValue, out Int32 iErrorId);
        DataSet GetPayeeSupplier(string royaltorId, string intPartyId, out Int32 iErrorId);
        DataSet SavePayeeSupplier(string royaltorId, string intPartyId, string payeeCurrency, string supplierNumberOld, string supplierNumber, string supplierSiteNameOld, string supplierSiteName, string accountCompany, string mismatchFlag, string saveType, string userCode, out Int32 iErrorId);
        
    }
}
