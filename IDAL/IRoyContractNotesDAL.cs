using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyContractNotesDAL
    {
        DataSet GetRoyContractNotes(string royaltorId, out string royaltor, out Int32 iErrorId);
        void SaveRoyContractNotes(string royaltorId, string notes, string loggedUser, out Int32 iErrorId);        
       
        
    }
}
