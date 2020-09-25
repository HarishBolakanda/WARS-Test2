using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IOptionPeriodLinksBL
    {
        DataSet GetRoyaltorsArtistsList(out Int32 iErrorId);
        DataSet GetOptionPeriods(Int32 royaltorId, out Int32 iErrorId);
        DataSet GetIntialLinksData(Int32 royaltorId, Int32 opetionPeriodCode, out Int32 iErrorId);
        DataSet GetArtistLinksData(string artistName, out Int32 iErrorid);
        DataSet OptionPeriodOperations(Array artistIdsToAdd, Array artistIdsToRemove, Array artistIdsToReplace, Int32 royaltorId, Int32 optionPeriodCode, string userCode, string displayArtistIds, out Int32 iErrorid);
    }
}
