using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyaltorSearchDAL
    {    
        DataSet GetDropdownData(out Int32 iErrorId);
        DataSet GetSearchData(string royaltor, string plgRoyaltor, string owner, string isCompanySelected, string responsibility, string status, string isRoyaltorHeld, string contractType, Array royaltorBulkSearchList, out Int32 iErrorId);
        DataSet UpdateRoyaltor(string royaltor, string plgRoyaltor, string owner, string isCompanySelected, string responsibility, string status, string isRoyaltorHeld, string contractType, Array royaltors,
                               string isLockUnlock, string lockUnlockAll, string updateStatusCode, string loggedUser, out Int32 iErrorId);
    }
}
