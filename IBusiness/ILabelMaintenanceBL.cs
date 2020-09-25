using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ILabelMaintenanceBL
    {
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet UpdateLabelData(string labelCode, string labelDesc, string userCode, out Int32 iErrorId);
        DataSet InsertLabelData(string labelCode, string labelDesc, string userCode, out Int32 iErrorId);
        DataSet DeleteLabelData(string labelCode, string userCode, out Int32 iErrorId);
    }
}
