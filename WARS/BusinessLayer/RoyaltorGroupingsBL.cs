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
    public class RoyaltorGroupingsBL : IRoyaltorGroupingsBL
    {
        IRoyaltorGroupingsDAL RoyaltorGroupingsDAL;
        #region Constructor
        public RoyaltorGroupingsBL()
        {
            RoyaltorGroupingsDAL = new RoyaltorGroupingsDAL();
        }
        #endregion Constructor

        public DataSet GetRoyaltorGroupings(out Int32 iErrorId)
        {
            return RoyaltorGroupingsDAL.GetRoyaltorGroupings(out iErrorId);
        }

        public DataSet GetRoyaltorGroupingsInOutData(Int32 grpTypeCode, Int32 grpId, out Int32 iErrorId)
        {
            return RoyaltorGroupingsDAL.GetRoyaltorGroupingsInOutData(grpTypeCode,grpId,out iErrorId);
        }

        public DataSet AddRoyaltorToGroup(Int32 grpTypeCode, Int32 grpId, Int32 searchedRoyaltorId, Array royaltorId, out Int32 iErrorId)
        {
            return RoyaltorGroupingsDAL.AddRoyaltorToGroup(grpTypeCode, grpId, searchedRoyaltorId, royaltorId, out iErrorId);
        }

        public DataSet RemoveRoyaltorFromGroup(Int32 grpTypeCode, Int32 grpId, Int32 searchedRoyaltorId, Array royaltorId, out Int32 iErrorId)
        {
            return RoyaltorGroupingsDAL.RemoveRoyaltorFromGroup(grpTypeCode, grpId, searchedRoyaltorId, royaltorId, out iErrorId);
        }

        public DataSet GetUpdatedOutData(Int32 grpTypeCode, Int32 grpId, Int32 searchedRoyaltorId, out Int32 iErrorId)
        {
            return RoyaltorGroupingsDAL.GetUpdatedOutData(grpTypeCode, grpId, searchedRoyaltorId, out iErrorId);
        }

        public void GenerateGroupSummaries(string loggedUser, Int32 groupId, out Int32 iErrorId)
        {
            RoyaltorGroupingsDAL.GenerateGroupSummaries(loggedUser, groupId, out iErrorId);
        }

        public void GenerateAllGroupSummaries(string loggedUser, out Int32 iErrorId)
        {
            RoyaltorGroupingsDAL.GenerateAllGroupSummaries(loggedUser, out iErrorId);
        }

    }
}
