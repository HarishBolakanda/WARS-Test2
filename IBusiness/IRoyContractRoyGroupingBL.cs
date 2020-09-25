using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyContractRoyGroupingBL
    {
        DataSet GetInitialData(Int32 royaltorId, out Int32 iErrorId);        
        DataSet UpdateRoyaltorGrouping(Int32 royaltorId, string summaryMasterRoyaltor, string txtMasterRoyaltor, string accrualRoyaltor, string dspAnalyticsRoyaltor, string gfsLabel, string gfsCompany, string printGroup, string userCode, out Int32 iErrorId);
    }
}
