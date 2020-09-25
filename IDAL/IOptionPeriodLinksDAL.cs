using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IOptionPeriodLinksDAL
    {
        DataSet GetOptionPeriods(Int32 royaltorId, string loggedInUserRole, out Int32 iErrorId);
        DataSet GetIntialLinksData(Int32 royaltorId,Int32 optionPeriodCode, out Int32 iErrorId);
        DataSet GetArtistLinksData(string artistName, out Int32 iErrorid);
        DataSet OptionPeriodOperations(Array artistIdsToAdd, Array artistIdsToRemove, Array artistIdsToReplace,Int32 royaltorId, Int32 optionPeriodCode, string userCode, string displayArtistIds, out Int32 iErrorid);
    }
}
