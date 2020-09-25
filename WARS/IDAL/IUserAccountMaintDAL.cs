using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IUserAccountMaintDAL
    {
        
        DataSet GetData(string userFilter, out Int32 iErrorId);
        DataSet AddUserAccount(string userName, string userCode, string userAccId, string respCode, string roleId, string isActive, string userFilter, string loggedUser,
                                out Int32 iErrorId);
        DataSet UpdateUserAccount(string userAccId,string userCode,string respCode, string userName, string userCodeNew, string userAccIdNew, string respCodeNew, 
                                    string roleId, string isActive, string userFilter, string loggedUser, out Int32 iErrorId);
        void UpdateRespChanges(string respToReplace, string newResp, out Int32 iErrorId);
        
    }
}
