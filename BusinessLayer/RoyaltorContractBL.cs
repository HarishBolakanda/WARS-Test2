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
    public class RoyaltorContractBL : IRoyaltorContractBL
    {
        IRoyaltorContractDAL royaltorContractDAL;
        #region Constructor
        public RoyaltorContractBL()
        {
            royaltorContractDAL = new RoyaltorContractDAL();
        }
        #endregion Constructor

        public DataSet GetDropdownData(out Int32 iErrorId)
        {
            return royaltorContractDAL.GetDropdownData(out iErrorId);
        }

        public DataSet GetSearchData(string royaltor, out Int32 iErrorId)
        {
            return royaltorContractDAL.GetSearchData(royaltor, out iErrorId);
        }

        public DataSet AddUpateContract(string royaltor, string isRespChanged, string isOwnerChanged, Array contractParams, Array liquidationParams, string loggedUser, out Int32 iErrorId)
        {
            return royaltorContractDAL.AddUpateContract(royaltor, isRespChanged, isOwnerChanged, contractParams, liquidationParams, loggedUser, out iErrorId);
        }

        public void CopyContract(string royaltorIdCopy, string royaltorIdNew, string royaltorName, string loggedUser, string optionCodes, string royRates, string subRates, string packRates, string escCodes, out Int32 iErrorId)
        {
            royaltorContractDAL.CopyContract(royaltorIdCopy, royaltorIdNew, royaltorName, loggedUser, optionCodes, royRates, subRates, packRates, escCodes, out iErrorId);
        }

        public DataSet LockUnlockContact(string royaltor, string locked, string loggedUser, out Int32 iErrorId)
        {
            return royaltorContractDAL.LockUnlockContact(royaltor, locked, loggedUser, out iErrorId);
        }

        public DataSet AddOwnerDetails(string ownercode, string ownerName, string userCode, out Int32 iErrorId)
        {
            return royaltorContractDAL.AddOwnerDetails(ownercode, ownerName, userCode, out iErrorId);
        }

        public string GetNewOwnerCode(out Int32 iErrorId)
        {
            return royaltorContractDAL.GetNewOwnerCode(out iErrorId);
        }

        public DataSet GetOptionsForCopyContract(string royaltorId, out Int32 iErrorId)
        {
            return royaltorContractDAL.GetOptionsForCopyContract(royaltorId, out iErrorId);

        }

        public void UpdateScreenLockFlag(string royaltor, string screenLockFlag, string loggedUser, out Int32 iErrorId)
        {
            royaltorContractDAL.UpdateScreenLockFlag(royaltor, screenLockFlag, loggedUser, out iErrorId);
        }


    }
}
