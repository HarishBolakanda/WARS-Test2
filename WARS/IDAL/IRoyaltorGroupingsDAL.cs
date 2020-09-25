using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyaltorGroupingsDAL
    {
        DataSet GetRoyaltorGroupings(out Int32 iErrorId);
        DataSet GetRoyaltorGroupingsInOutData(Int32 grpTypeCode, Int32 grpId,out Int32 iErrorId);
        DataSet AddRoyaltorToGroup(Int32 grpTypeCode, Int32 grpId, Int32 searchedRoyaltorId, Array royaltorId, out Int32 iErrorId);
        DataSet RemoveRoyaltorFromGroup(Int32 grpTypeCode, Int32 grpId, Int32 searchedRoyaltorId, Array royaltorId, out Int32 iErrorId);
        DataSet GetUpdatedOutData(Int32 grpTypeCode, Int32 grpId,Int32 searchedRoyaltorId, out Int32 iErrorId);
        void GenerateGroupSummaries(string loggedUser,Int32 groupId, out Int32 iErrorId);
        void GenerateAllGroupSummaries(string loggedUser,out Int32 iErrorId);
    }
}
