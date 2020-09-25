using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ITransactionRetrievalBL
    {        
        DataSet GetSearchData(string royaltorId, string optionPeriodCode, string artistId, string catNumber, string projectCode, out Int32 iErrorId);
        DataSet AddToRetrieveTrans(Array catNums, string startDate, string endDate, string royaltorId, string optionPeriodCode, string artistId, string catNumber, string projectCode,
                                    string loggedUser, string isOverwrite, out Int32 iErrorId);
        
    }
}
