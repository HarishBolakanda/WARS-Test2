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
    public class RoyaltorSearchBL:IRoyaltorSearchBL
    {
        IRoyaltorSearchDAL royaltorSearchDAL;
        #region Constructor
        public RoyaltorSearchBL()
        {
            royaltorSearchDAL = new RoyaltorSearchDAL();
        }
        #endregion Constructor

        public DataSet GetDropdownData(out Int32 iErrorId)
        {
            return royaltorSearchDAL.GetDropdownData(out iErrorId);
        }

        public DataSet GetSearchData(string royaltor, string plgRoyaltor, string owner, string isCompanySelected, string responsibility, string status, string isRoyaltorHeld, string contractType, Array royaltorBulkSearchList, out Int32 iErrorId)
        {
            return royaltorSearchDAL.GetSearchData(royaltor, plgRoyaltor, owner, isCompanySelected, responsibility, status, isRoyaltorHeld, contractType, royaltorBulkSearchList, out iErrorId);
        }

        public DataSet UpdateRoyaltor(string royaltor, string plgRoyaltor, string owner, string isCompanySelected, string responsibility, string status, string isRoyaltorHeld, string contractType, Array royaltors,
                               string isLockUnlock, string lockUnlockAll, string updateStatusCode, string loggedUser, out Int32 iErrorId)
        {
            return royaltorSearchDAL.UpdateRoyaltor(royaltor, plgRoyaltor, owner, isCompanySelected, responsibility, status, isRoyaltorHeld, contractType, royaltors, isLockUnlock, lockUnlockAll, updateStatusCode, loggedUser, out iErrorId);
        }
    }
}
