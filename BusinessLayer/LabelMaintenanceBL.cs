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
    public class LabelMaintenanceBL : ILabelMaintenanceBL
    {
        ILabelMaintenanceDAL LabelMaintenanceDAL;
        #region Constructor
        public LabelMaintenanceBL()
        {
            LabelMaintenanceDAL = new LabelMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return LabelMaintenanceDAL.GetInitialData(out iErrorId);
        }

        public DataSet UpdateLabelData(string labelCode, string labelDesc, string userCode, out Int32 iErrorId)
        {
            return LabelMaintenanceDAL.UpdateLabelData(labelCode, labelDesc, userCode, out iErrorId);
        }

        public DataSet InsertLabelData(string labelCode,string labelDesc, string userCode, out Int32 iErrorId)
        {
            return LabelMaintenanceDAL.InsertLabelData(labelCode, labelDesc, userCode, out iErrorId);
        }

        public DataSet DeleteLabelData(string labelCode, string userCode, out Int32 iErrorId)
        {
            return LabelMaintenanceDAL.DeleteLabelData(labelCode, userCode, out iErrorId);
        }
    }
}
