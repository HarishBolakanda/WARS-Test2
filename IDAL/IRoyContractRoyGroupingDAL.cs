using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyContractRoyGroupingDAL
    {
        DataSet GetInitialData(Int32 royaltorId, out Int32 iErrorId);        
        DataSet UpdateRoyaltorGrouping(Int32 royaltorId, string summaryMasterRoyaltor, string txtMasterRoyaltor, string accrualRoyaltor, string dspAnalyticsRoyaltor, string gfsLabel, string gfsCompany, string printGroup, string userCode, out Int32 iErrorId);
    }
}
