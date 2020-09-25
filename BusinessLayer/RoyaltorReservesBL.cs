using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;


namespace WARS.BusinessLayer
{
    public class RoyaltorReservesBL : IRoyaltorReservesBL
    {
        IRoyaltorReservesDAL RoyaltorReservesDAL;
        #region Constructor
        public RoyaltorReservesBL()
        {
            RoyaltorReservesDAL = new RoyaltorReservesDAL();
        }
        #endregion Constructor

        public DataSet GetRoyaltorData(Int32 royaltorId, out Int32 iErrorId)
        {
            return RoyaltorReservesDAL.GetRoyaltorData(royaltorId, out iErrorId);
        }
        
        public DataSet UpdateRoyaltorBalData(Int32 royaltorId, Int32 balancePeriodId, double updatedBalance, string loggedUser, out Int32 iErrorId)
        {
            return RoyaltorReservesDAL.UpdateRoyaltorBalData(royaltorId, balancePeriodId, updatedBalance, loggedUser, out iErrorId);
        }

        public DataSet DeleteRoyaltorBalData(Int32 royaltorId, Int32 balancePeriodId, string loggedUser, out Int32 iErrorId)
        {
            return RoyaltorReservesDAL.DeleteRoyaltorBalData(royaltorId, balancePeriodId, loggedUser, out iErrorId);
        }

        public DataSet DeleteRoyaltorRsvData(Int32 royaltorId, Int32 reservePeriodId, string loggedUser, out Int32 iErrorId)
        {
            return RoyaltorReservesDAL.DeleteRoyaltorRsvData(royaltorId, reservePeriodId, loggedUser, out iErrorId);
        }

        public DataSet InsertRoyaltorRsvData(Int32 royaltorId, Int32 reservePeriodId, Int32 lqdInterval, double rsvAmount, string userCode, out Int32 iErrorId)
        {
            return RoyaltorReservesDAL.InsertRoyaltorRsvData(royaltorId, reservePeriodId, lqdInterval, rsvAmount, userCode, out iErrorId);
        }

        public DataSet UpdateRoyaltorRsvData(Int32 royaltorId, Int32 oldReservePeriodID, Int32 newReservePeriodID, Int32 lqdInterval, double rsvAmount, string loggedUser, out Int32 iErrorId)
        {
            return RoyaltorReservesDAL.UpdateRoyaltorRsvData(royaltorId, oldReservePeriodID,newReservePeriodID, lqdInterval, rsvAmount,loggedUser, out iErrorId);
        }

        public DataSet DeleteRoyaltorLqdData(Int32 royaltorId, Int32 reservePeriodId, Int32 lqdPeriodId, string loggedUser, out Int32 iErrorId)
        {
            return RoyaltorReservesDAL.DeleteRoyaltorLqdData(royaltorId, reservePeriodId, lqdPeriodId, loggedUser, out iErrorId);
        }

        public DataSet InsertRoyaltorLqdData(Int32 royaltorId, Int32 reservePeriodId, Int32 lqdPeriodId,Int32 lqdInterval, double lqdAmount, string userCode, out Int32 iErrorId)        
        {
            return RoyaltorReservesDAL.InsertRoyaltorLqdData(royaltorId, reservePeriodId, lqdPeriodId, lqdInterval, lqdAmount, userCode, out iErrorId);            
        }

        public DataSet UpdateRoyaltorLqdData(Int32 royaltorId, Int32 reservePeriodID, Int32 oldLqdPeriod, Int32 newLqdPeriod, Int32 lqdInterval, double lqdAmount, string loggedUser, out Int32 iErrorId)        
        {
            return RoyaltorReservesDAL.UpdateRoyaltorLqdData(royaltorId, reservePeriodID, oldLqdPeriod, newLqdPeriod, lqdInterval, lqdAmount, loggedUser, out iErrorId);            
        }
    }
}
