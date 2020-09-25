using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyaltorContractBL
    {
        DataSet GetDropdownData(out Int32 iErrorId);
        DataSet GetSearchData(string royaltor, out Int32 iErrorId);
        DataSet AddUpateContract(string royaltor, string isRespChanged, string isOwnerChanged, Array contractParams, Array liquidationParams, string loggedUser, out Int32 iErrorId);
        void CopyContract(string royaltorIdCopy, string royaltorIdNew, string royaltorName, string loggedUser, string optionCodes, string royRates, string subRates, string packRates, string escCodes, out Int32 iErrorId);
        DataSet LockUnlockContact(string royaltor, string locked, string loggedUser, out Int32 iErrorId);
        DataSet AddOwnerDetails(string ownerCode, string ownerName, string userCode, out Int32 iErrorId);
        string GetNewOwnerCode(out Int32 iErrorId);
        DataSet GetOptionsForCopyContract(string royaltorId, out Int32 iErrorId);
        void UpdateScreenLockFlag(string royaltor, string flscreenLockFlagag, string loggedUser, out Int32 iErrorId);
    
    }
}
