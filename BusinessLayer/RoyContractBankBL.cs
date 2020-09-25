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
    public class RoyContractBankBL : IRoyContractBankBL
    {
        IRoyContractBankDAL royContractBankDAL;
        #region Constructor
        public RoyContractBankBL()
        {
            royContractBankDAL = new RoyContractBankDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(Int32 royaltorId, out string royaltor, out string primaryPayee, out Int32 iErrorId)
        {
            return royContractBankDAL.GetInitialData(royaltorId, out royaltor, out primaryPayee, out iErrorId);
        }

        public DataSet GetPayeeBankDetails(Int32 interestedPartyId, out Int32 iErrorId)
        {
            return royContractBankDAL.GetPayeeBankDetails(interestedPartyId, out iErrorId);
        }

        public DataSet MergePayeeBankDomesticDetails(Int32 interestedPartyId, string vatNumber, string supplierNumber, string paymentMethodCode,
           string bankName, string bankAddress, string accountName, string sortCode, string accountNumber, string vendorSiteCode, string userCode, out Int32 iErrorId)
        {
            return royContractBankDAL.MergePayeeBankDomesticDetails(interestedPartyId, vatNumber, supplierNumber, paymentMethodCode, bankName, bankAddress, accountName, sortCode, accountNumber, vendorSiteCode, userCode, out iErrorId);
        }

        public DataSet MergePayeeBankForeignDetails(Int32 interestedPartyId, string vatNumber, string supplierNumber, string paymentMethodCode,
           string iban, string swiftCode, string abaRouting, string accountName, string accountNumber, string bankAddress, string currencyCode, string vendorSiteCode, string userCode, out Int32 iErrorId)
        {
            return royContractBankDAL.MergePayeeBankForeignDetails(interestedPartyId, vatNumber, supplierNumber, paymentMethodCode, iban, swiftCode, abaRouting, accountName, accountNumber, bankAddress, currencyCode, vendorSiteCode, userCode, out iErrorId);
        }
    }
}
