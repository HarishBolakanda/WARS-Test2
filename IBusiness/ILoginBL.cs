using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ILoginBL
    {
        DataTable UserAuthentication(string loggedUser, out Int32 iErrorId, out string sErrorMsg);
    }
}
