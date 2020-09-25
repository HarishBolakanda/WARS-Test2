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
    public class RoyContractEscHistoryBL : IRoyContractEscHistoryBL
    {
        IRoyContractEscHistoryDAL royContractEscHistoryDAL;
        #region Constructor
        public RoyContractEscHistoryBL()
        {
            royContractEscHistoryDAL = new RoyContractEscHistoryDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(string royaltorId, out Int32 iErrorId)
        {            
            return SortDisplayOrder(royContractEscHistoryDAL.GetInitialData(royaltorId, out iErrorId));
        }

        public DataSet GetEscHistory(string royaltorId, string escCode, out Int32 iErrorId)
        {
            return SortDisplayOrder(royContractEscHistoryDAL.GetEscHistory(royaltorId, escCode, out iErrorId));
        }

        public DataSet GetEscHistorySummary(string royaltorId, string escCode, out Int32 iErrorId)
        {
            return SortDisplayOrder(royContractEscHistoryDAL.GetEscHistorySummary(royaltorId, escCode, out iErrorId));
        }

        public DataSet SaveEscHistory(string royaltorId, string escCode, string sellerGrpCode, string priceGrpCode, string configGrpCode, string sales, string adjSales, out Int32 iErrorId)
        {
            return SortDisplayOrder(royContractEscHistoryDAL.SaveEscHistory(royaltorId, escCode, sellerGrpCode, priceGrpCode, configGrpCode, sales, adjSales, out iErrorId));
        }

        public DataSet DeleteEscHistory(string royaltorId, string escCode, string sellerGrpCode, string priceGrpCode, string configGrpCode, out Int32 iErrorId)
        {
            return SortDisplayOrder((royContractEscHistoryDAL.DeleteEscHistory(royaltorId, escCode, sellerGrpCode, priceGrpCode, configGrpCode, out iErrorId)));
        }

        private DataSet SortDisplayOrder(DataSet dsEscHistory)
        {
            if (dsEscHistory.Tables["EscHistory"] != null)
            {
                DataView dv = dsEscHistory.Tables["EscHistory"].DefaultView;
                dv.Sort = "territory,configuration,pricegroup";
                
            }

            return dsEscHistory;
        }
      
       
      
    }
}
