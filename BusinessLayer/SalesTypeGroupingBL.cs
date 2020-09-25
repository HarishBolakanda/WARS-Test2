using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;
using System.Data;

namespace WARS.BusinessLayer
{
    public class SalesTypeGroupingBL : ISalesTypeGroupingBL
    {
        ISalesTypeGroupingDAL salesTypeGroupingDAL;

        #region Constructor
        public SalesTypeGroupingBL()
        {
            salesTypeGroupingDAL = new SalesTypeGroupingDAL();
        }
        #endregion Constructor

        public DataSet GetSalesTypeGroupInOutData(string salesTypeGroupCode, out Int32 iErrorId)
        {
            return salesTypeGroupingDAL.GetSalesTypeGroupInOutData(salesTypeGroupCode, out iErrorId);
        }

        public DataSet AddSalesTypeToGroup(string salesTypeGroupCode, Array salesTypecodes, string userCode, out Int32 iErrorId)
        {
            return salesTypeGroupingDAL.AddSalesTypeToGroup(salesTypeGroupCode, salesTypecodes, userCode, out iErrorId);
        }

        public DataSet RemoveSalesTypeFromGroup(string salesTypeGroupCode, Array salesTypecodes,string userCode, out Int32 iErrorId)
        {
            return salesTypeGroupingDAL.RemoveSalesTypeFromGroup(salesTypeGroupCode, salesTypecodes,userCode, out iErrorId);
        }

        public DataSet InsertSalesTypeGroup(string salesTypeGroupCode, string salesTypeGroupName, string userCode, out Int32 iErrorId)
        {
            return salesTypeGroupingDAL.InsertSalesTypeGroup(salesTypeGroupCode, salesTypeGroupName, userCode, out iErrorId);
        }
    }
}
