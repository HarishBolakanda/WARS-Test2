using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WARS.IBusiness
{
    public interface IAutoParticipantSearchBL
    {
        DataSet SearchAutoParticipantData(string marketingOwer, string weaSalesLable, string artist, string projectTitle, out Int32 iErrorId);
        DataSet AddUpdateAutoParticipant(Int32 autoPartId, string marketingOwer, string weaSalesLable, Int32 artistId, Int32 projectId, string projectTitle, string marketingOnwerSearch, string weaSalesLabelSearch, string artistSearch, string projectSearch, string userCode, out Int32 newAutoPartId, out Int32 iErrorId);

    }
}
