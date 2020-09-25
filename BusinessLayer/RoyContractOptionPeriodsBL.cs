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
    public class RoyContractOptionPeriodsBL : IRoyContractOptionPeriodsBL
    {
        IRoyContractOptionPeriodsDAL royContractOptionPeriodsDAL;
        #region Constructor
        public RoyContractOptionPeriodsBL()
        {
            royContractOptionPeriodsDAL = new RoyContractOptionPeriodsDAL();
        }
        #endregion Constructor

        public DataSet GetOptionPeriodData(string royaltorId, string userRoleId, out string royaltor, out Int32 maxOptionPeriodCode, out Int32 iErrorId)
        {
            return royContractOptionPeriodsDAL.GetOptionPeriodData(royaltorId, userRoleId, out royaltor, out maxOptionPeriodCode, out iErrorId);
        }

        public DataSet SaveOptionPeriod(string royaltorId, Array optionPeriodList, Array deleteList, string loggedUser, string userRoleId, out string royaltor, out Int32 outmaxOptionPeriodCode, out Int32 iErrorId)
        {
            return royContractOptionPeriodsDAL.SaveOptionPeriod(royaltorId, optionPeriodList, deleteList, loggedUser, userRoleId, out royaltor, out  outmaxOptionPeriodCode, out iErrorId);
        }

        public void ValidateDelete(string royaltorId, string optionPeriodCode, out Int32 iErrorId)
        {
            royContractOptionPeriodsDAL.ValidateDelete(royaltorId, optionPeriodCode, out iErrorId);
        }

    }
}
