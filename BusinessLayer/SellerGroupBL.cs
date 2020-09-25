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
    public class SellerGroupBL : ISellerGroupBL
    {
        ISellerGroupDAL sellerGroupDAL;
        #region Constructor
        public SellerGroupBL()
        {
            sellerGroupDAL = new SellerGroupDAL();
        }
        #endregion Constructor

        public DataSet GetSellerGroupInOutData(string sellerGroupCode, out Int32 iErrorId)
        {
            return sellerGroupDAL.GetSellerGroupInOutData(sellerGroupCode, out iErrorId);
        }

        public DataSet AddTerritoryToGroup(string territoryGroupCode, Array territorycode, string userCode,  out Int32 iErrorId)
        {
            return sellerGroupDAL.AddTerritoryToGroup(territoryGroupCode, territorycode, userCode, out iErrorId);
        }

        public DataSet RemoveTerritoryToGroup(string territoryGroupCode, Array territorycode, string userCode, out Int32 iErrorId)
        {
            return sellerGroupDAL.RemoveTerritoryToGroup(territoryGroupCode, territorycode, userCode, out iErrorId);
        }

        public DataSet InsertTerritoryGroup(string territoryGroupCode, string territoryGroupName, string userCode, out Int32 iErrorId)
        {
            return sellerGroupDAL.InsertTerritoryGroup(territoryGroupCode, territoryGroupName, userCode, out iErrorId);
        }
    }
}
