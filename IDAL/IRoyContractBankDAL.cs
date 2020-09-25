using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyContractBankDAL
    {
        DataSet GetInitialData(Int32 royaltorId, out string royaltor, out string primaryPayee, out Int32 iErrorId);
        DataSet GetPayeeBankDetails(Int32 interestedPartyId, out Int32 iErrorId);
        DataSet MergePayeeBankDomesticDetails(Int32 interestedPartyId, string vatNumber, string supplierNumber, string paymentMethodCode,
           string bankName, string bankAddress, string accountName, string sortCode, string accountNumber, string vendorSiteCode, string userCode, out Int32 iErrorId);
        DataSet MergePayeeBankForeignDetails(Int32 interestedPartyId, string vatNumber, string supplierNumber, string paymentMethodCode,
           string iban, string swiftCode, string abaRouting, string accountName, string accountNumber, string bankAddress, string currencyCode, string vendorSiteCode, string userCode, out Int32 iErrorId);
    }
}
