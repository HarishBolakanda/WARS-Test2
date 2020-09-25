using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface INotesOverviewBL
    {
        DataSet GetNotesData (string royaltorId,string notesType,string statementPeriodId, out Int32 iErrorId);
        DataSet GetWorkflowDropdown(string royaltorId, out Int32 iErrorId);
    }
}
