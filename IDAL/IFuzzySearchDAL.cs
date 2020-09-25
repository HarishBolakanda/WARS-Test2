using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IFuzzySearchDAL
    {
        DataSet GetFuzzySearchList(string searchList, out Int32 iErrorId);
        DataSet GetFuzzySearchListTransRetr(string searchList, string userRoleId, out Int32 iErrorId); //JIRA-898 Changes -- Ravi
    }
}
