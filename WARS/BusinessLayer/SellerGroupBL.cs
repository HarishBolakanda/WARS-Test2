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

        public DataSet GetSellerGroupList(out Int32 iErrorId)
        {
            return sellerGroupDAL.GetSellerGroupList(out iErrorId);
        }

        public DataSet GetSellerGroupInOutData(string sellerGroupCode, out Int32 locationCount, out Int32 iErrorId)
        {
            return sellerGroupDAL.GetSellerGroupInOutData(sellerGroupCode,out locationCount, out iErrorId);
        }

        public DataSet AddTerritoryToGroup(string territoryGroupCode, Array territorycode, out Int32 locationCount, out Int32 iErrorId)
        {
            return sellerGroupDAL.AddTerritoryToGroup(territoryGroupCode, territorycode,out locationCount, out iErrorId);
        }

        public DataSet RemoveTerritoryToGroup(string territoryGroupCode, Array territorycode, out Int32 locationCount, out Int32 iErrorId)
        {
            return sellerGroupDAL.RemoveTerritoryToGroup(territoryGroupCode, territorycode,out locationCount, out iErrorId);
        }

        public DataSet InsertTerritoryGroup(string territoryGroupCode, string territoryGroupName, out Int32 iErrorId)
        {
            return sellerGroupDAL.InsertTerritoryGroup(territoryGroupCode, territoryGroupName, out iErrorId);
        }
    }
}
