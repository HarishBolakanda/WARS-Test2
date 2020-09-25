using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;


namespace WARS.BusinessLayer
{
    public class RoyContractNotesBL:IRoyContractNotesBL
    {
        IRoyContractNotesDAL royContractNotesDAL;
        #region Constructor
        public RoyContractNotesBL()
        {
            royContractNotesDAL = new RoyContractNotesDAL();
        }
        #endregion Constructor

        public DataSet GetRoyContractNotes(string royaltorId, out string royaltor, out Int32 iErrorId)
        {
            return royContractNotesDAL.GetRoyContractNotes(royaltorId, out royaltor, out iErrorId);
        }

        public void SaveRoyContractNotes(string royaltorId, string notes, string loggedUser, out Int32 iErrorId)
        {
            royContractNotesDAL.SaveRoyContractNotes(royaltorId, notes, loggedUser, out iErrorId);
        }

       

      
    }
}
