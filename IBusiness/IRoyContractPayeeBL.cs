using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyContractPayeeBL
    {
        DataSet GetPayeeData(string royaltorId, out string royaltor, out Int32 iErrorId);
        DataSet GetIntPartySearchList(string intPartyType, string intPartyName, string intPartyIds, out Int32 iErrorId);
        DataSet SavePayee(string royaltorId, Array payeeList, Array deleteList, string loggedUser, out string royaltor, out Int32 iErrorId);
        void AddInterestedParty(string partyType, string partyName, string address1, string address2, string address3, string address4, string postCode, string email, string VATNum, string taxType, string generateInvoice,
            string loggedUser, out string intPartyId,out string ipNumber, out Int32 iErrorId);

    }
}
