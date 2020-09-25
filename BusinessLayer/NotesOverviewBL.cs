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
   public class NotesOverviewBL : INotesOverviewBL
    {
        INotesOverviewDAL notesOverviewDAL;
        #region Constructor
        public NotesOverviewBL()
        {
            notesOverviewDAL = new NotesOverviewDAL();
        }
        #endregion Constructor

        public DataSet GetNotesData(string royaltorId,string notesType,string statementPeriodId, out Int32 iErrorId)
        {
            return notesOverviewDAL.GetNotesData(royaltorId,notesType,statementPeriodId, out iErrorId);
        }
       
        public DataSet GetWorkflowDropdown(string royaltorId, out  Int32 iErrorId)
        {
            return notesOverviewDAL.GetWorkflowDropdown(royaltorId,out iErrorId);
        }

    }
}
