using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyContractOptionPeriodsBL
    {
        DataSet GetOptionPeriodData(string royaltorId, string userRoleId, out string royaltor, out Int32 maxOptionPeriodCode, out Int32 iErrorId);
        DataSet SaveOptionPeriod(string royaltorId, Array optionPeriodList, Array deleteList, string loggedUser, string userRoleId, out string royaltor, out Int32 outmaxOptionPeriodCode, out Int32 iErrorId);
        void ValidateDelete(string royaltorId, string optionPeriodCode, out Int32 iErrorId);
    }
}
