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
    public class LoginBL:ILoginBL
    {
        ILoginDAL loginDAL;
        #region Constructor
        public LoginBL()
        {
            loginDAL = new LoginDAL();
        }
        #endregion Constructor


        public DataTable UserAuthentication(string loggedUser, out Int32 iErrorId, out string sErrorMsg)
        {
            return loginDAL.UserAuthentication(loggedUser, out iErrorId, out sErrorMsg);
        }
    }
}
