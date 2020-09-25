using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IInterestedPartyMaintenanceBL
    {
        DataSet GetInitialData(out Int32 iErrorId);

        DataSet UpdateInterestedPartyData(Int32 intPartyId, string intPartyName, string intPartyAdd1,
            string intPartyAdd2, string intPartyAdd3, string intPartyAdd4, string intPartyPostcode, string email, string vatNumber, string taxType, string isGenerateInvoice, string isSendStatement, string userCode, out Int32 iErrorId);

        DataSet InsertInterestedPartyData(string intPartyType, string intPartyName, string intPartyAdd1,
            string intPartyAdd2, string intPartyAdd3, string intPartyAdd4, string intPartyPostcode, string email, string vatNumber, string taxType, string isGenerateInvoice, string isSendStatement, string userCode, out Int32 iErrorId);

        DataSet DeleteInterestedPartyData(Int32 intPartyId, out Int32 iErrorId);

        DataSet GetLinkedRoyaltorDetails(Int32 intPartyId, out Int32 iErrorId);

        //DataSet GetPayeeBankDetails(Int32 intPartyId, out Int32 iErrorId);

        DataSet GetSupplierDetails(Int32 intPartyId, out Int32 iErrorId);
    }
}
