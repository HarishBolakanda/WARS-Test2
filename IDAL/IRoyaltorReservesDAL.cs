using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyaltorReservesDAL
    {        
        DataSet GetRoyaltorData(Int32 royaltorId, out Int32 iErrorId);        
        DataSet UpdateRoyaltorBalData(Int32 royaltorId, Int32 balancePeriodId, double updatedBalance, string loggedUser, out Int32 iErrorId);
        DataSet DeleteRoyaltorBalData(Int32 royaltorId, Int32 balancePeriodId, string loggedUser, out Int32 iErrorId);
        DataSet DeleteRoyaltorRsvData(Int32 royaltorId, Int32 reservePeriodId, string loggedUser, out Int32 iErrorId);
        DataSet InsertRoyaltorRsvData(Int32 royaltorId, Int32 reservePeriodID, Int32 lqdInterval, double rsvAmount, string userCode, out Int32 iErrorId);
        DataSet UpdateRoyaltorRsvData(Int32 royaltorId, Int32 oldReservePeriodID, Int32 newReservePeriodID, Int32 lqdInterval, double rsvAmount, string loggedUser, out Int32 iErrorId);
        DataSet DeleteRoyaltorLqdData(Int32 royaltorId, Int32 reservePeriodId,Int32 lqdPeriodId, string loggedUser, out Int32 iErrorId);
        DataSet InsertRoyaltorLqdData(Int32 royaltorId, Int32 reservePeriodId, Int32 lqdPeriodId, Int32 lqdInterval, double lqdAmount, string loggedUser, out Int32 iErrorId);        
        DataSet UpdateRoyaltorLqdData(Int32 royaltorId, Int32 reservePeriodID, Int32 oldLqdPeriod,Int32 newLqdPeriod,Int32 lqdInterval, double lqdAmount, string loggedUser, out Int32 iErrorId);        
    }
}
