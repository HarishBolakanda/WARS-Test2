using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyContractNotesBL
    {
        DataSet GetRoyContractNotes(string royaltorId, out string royaltor, out Int32 iErrorId);
        void SaveRoyContractNotes(string royaltorId, string notes, string loggedUser, out Int32 iErrorId); 

    }
}
