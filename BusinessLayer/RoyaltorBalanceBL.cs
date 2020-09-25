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
    public class RoyaltorBalanceBL : IRoyaltorBalanceBL
    {
        IRoyaltorBalanceDAL RoyaltorBalanceDAL;
        #region Constructor
        public RoyaltorBalanceBL()
        {
            RoyaltorBalanceDAL = new RoyaltorBalanceDAL();
        }
        #endregion Constructor

       
        public DataSet GetRoyaltorDate(string royaltorId,out Int32 iErrorId)
        {
            return RoyaltorBalanceDAL.GetRoyaltorDate(royaltorId,out iErrorId);
        }

        public DataSet GetSearchedData(string royaltorId,string dateType,string balanceDate,string voucherDate, out Int32 iErrorId)
        {
            return RoyaltorBalanceDAL.GetSearchedData(royaltorId, dateType,balanceDate,voucherDate, out iErrorId);
        }
    }
}
