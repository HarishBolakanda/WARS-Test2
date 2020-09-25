using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
   public interface INotesOverviewDAL
    {
       DataSet GetNotesData (string royaltorId,string notesType,string statementPeriodId, out Int32 iErrorId);
       DataSet GetWorkflowDropdown(string royaltorId, out Int32 iErrorId);
    }
}
