using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ILoginDAL
    {
        DataTable UserAuthentication(string loggedUser, out Int32 iErrorId, out string sErrorMsg);
    }
}
