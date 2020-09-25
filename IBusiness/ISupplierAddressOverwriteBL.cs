using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ISupplierAddressOverwriteBL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetPayeeList(string intParty, out Int32 iErrorId);
        DataSet GetPayeeSuppData(string intPartyId, string royaltorId, string supplierNumber, string supplierSiteName, out Int32 iErrorId);
        DataSet OverwritePayeeAddress(string intPartyId, string royaltorId, string supplierName, string supplierNumber, string supplierSiteName, string suppAddress1, string suppAddress2, string suppAddress3, string suppAddress4, string suppPostCode, string userCode, out Int32 iErrorId);
        DataSet GetPayeeRoyaltorList(string intPartyId, string supplierNumber, string supplierSiteName, out Int32 iErrorId);
    }
}
