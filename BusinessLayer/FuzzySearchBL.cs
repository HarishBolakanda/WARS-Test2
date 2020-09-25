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
    public class FuzzySearchBL:IFuzzySearchBL
    {
        IFuzzySearchDAL FuzzySearchDAL;
        #region Constructor
        public FuzzySearchBL()
        {
            FuzzySearchDAL = new FuzzySearchDAL();
        }
        #endregion Constructor

        public DataSet GetFuzzySearchList(string searchList, out Int32 iErrorId)
        {
            return FuzzySearchDAL.GetFuzzySearchList(searchList, out iErrorId);
        }

        //JIRA-898 Changes -- Ravi --Start
        public DataSet GetFuzzySearchListTransRetr(string searchList, string userRoleId, out Int32 iErrorId)
        {
            return FuzzySearchDAL.GetFuzzySearchListTransRetr(searchList, userRoleId, out iErrorId);
        }
        //JIRA-898 Changes -- Ravi -- ENd
    }
}
