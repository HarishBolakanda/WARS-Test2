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
    public class RoyContractRoyGroupingBL : IRoyContractRoyGroupingBL
    {
        IRoyContractRoyGroupingDAL royContractRoyGroupingDAL;
        #region Constructor
        public RoyContractRoyGroupingBL()
        {
            royContractRoyGroupingDAL = new RoyContractRoyGroupingDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(Int32 royaltorId, out Int32 iErrorId)
        {
            return royContractRoyGroupingDAL.GetInitialData(royaltorId, out iErrorId);
        }

        public DataSet UpdateRoyaltorGrouping(Int32 royaltorId, string summaryMasterRoyaltor, string txtMasterRoyaltor, string accrualRoyaltor, string dspAnalyticsRoyaltor, string gfsLabel, string gfsCompany, string printGroup, string userCode, out Int32 iErrorId)
        {
            return royContractRoyGroupingDAL.UpdateRoyaltorGrouping(royaltorId, summaryMasterRoyaltor, txtMasterRoyaltor, accrualRoyaltor, dspAnalyticsRoyaltor, gfsLabel, gfsCompany, printGroup, userCode, out iErrorId);
        }
    }
}
