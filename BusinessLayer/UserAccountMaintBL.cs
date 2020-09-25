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
    public class UserAccountMaintBL:IUserAccountMaintBL
    {
        IUserAccountMaintDAL UserAccountMaintDAL;
        #region Constructor
        public UserAccountMaintBL()
        {
            UserAccountMaintDAL = new UserAccountMaintDAL();
        }
        #endregion Constructor

        public DataSet GetData(string userFilter, out Int32 iErrorId)
        {
            return UserAccountMaintDAL.GetData(userFilter, out iErrorId);
        }

        public DataSet AddUserAccount(string userName, string userCode, string userAccId, string respCode, string roleId, string paymentRoleId, string isActive, string userFilter, string loggedUser,
                               out Int32 iErrorId)
        {
            return UserAccountMaintDAL.AddUserAccount(userName, userCode, userAccId, respCode, roleId, paymentRoleId, isActive, userFilter, loggedUser, out iErrorId);
        }

        public DataSet UpdateUserAccount(string userAccId, string userCode, string respCode, string userName, string userCodeNew, string userAccIdNew, string respCodeNew,
                                    string roleId, string paymentRoleId, string isActive, string userFilter, string loggedUser, out Int32 iErrorId)
        {
            return UserAccountMaintDAL.UpdateUserAccount(userAccId, userCode, respCode, userName, userCodeNew, userAccIdNew, respCodeNew,
                                                        roleId, paymentRoleId, isActive, userFilter, loggedUser, out iErrorId);
        }

        public void UpdateRespChanges(Int32 respToReplace, Int32 newResp,string loggedUser, out Int32 iErrorId)
        {
            UserAccountMaintDAL.UpdateRespChanges(respToReplace, newResp, loggedUser, out iErrorId);
        }
                
        
    }
}
