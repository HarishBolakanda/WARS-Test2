using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;
using System.Data;

namespace WARS.BusinessLayer
{
    public class OptionPeriodLinksBL : IOptionPeriodLinksBL
    {
        IOptionPeriodLinksDAL OptionPeriodLinksDAL;
        #region Constructor
        public OptionPeriodLinksBL()
        {
            OptionPeriodLinksDAL = new OptionPeriodLinksDAL();
        }
        #endregion Constructor

        public DataSet GetRoyaltorsArtistsList(out Int32 iErrorId)
        {
            return OptionPeriodLinksDAL.GetRoyaltorsArtistsList(out iErrorId);
        }

        public DataSet GetOptionPeriods(Int32 royaltorId, out Int32 iErrorId)
        {
            return OptionPeriodLinksDAL.GetOptionPeriods(royaltorId, out iErrorId);
        }

        public DataSet GetIntialLinksData(Int32 royaltorId, Int32 opetionPeriodCode, out Int32 iErrorId)
        {
            return OptionPeriodLinksDAL.GetIntialLinksData(royaltorId, opetionPeriodCode, out iErrorId);
        }

        public DataSet GetArtistLinksData(string artistName, out Int32 iErrorid)
        {
            return OptionPeriodLinksDAL.GetArtistLinksData(artistName, out iErrorid);
        }

        public DataSet OptionPeriodOperations(Array artistIdsToAdd, Array artistIdsToRemove, Array artistIdsToReplace, Int32 royaltorId, Int32 optionPeriodCode, string userCode, string displayArtistIds, out Int32 iErrorid)
        {
            return OptionPeriodLinksDAL.OptionPeriodOperations(artistIdsToAdd, artistIdsToRemove, artistIdsToReplace, royaltorId, optionPeriodCode, userCode, displayArtistIds, out iErrorid);
        }
    }
}
