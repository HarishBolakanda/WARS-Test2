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
    public class RoyContractTaxDetailsBL : IRoyContractTaxDetailsBL
    {
        IRoyContractTaxDetailsDAL royContractTaxDetailsDAL;
        #region Constructor
        public RoyContractTaxDetailsBL()
        {
            royContractTaxDetailsDAL = new RoyContractTaxDetailsDAL();
        }
        #endregion Constructor

        public DataSet GetDropdownData(string royaltorId, out Int32 iErrorId)
        {
            return royContractTaxDetailsDAL.GetDropdownData(royaltorId, out iErrorId);
        }

        public DataSet GetInitialData(string royaltorId, out string royaltor, out Int32 iErrorId)
        {
            return royContractTaxDetailsDAL.GetInitialData(royaltorId, out royaltor, out iErrorId);
        }

        public DataSet SaveRoyaltorTaxDetailsData(string royaltorId, Array royContTaxDetailsList, Array deleteList, string UserCode, out string royaltor, out Int32 iErrorId)
        {
            return royContractTaxDetailsDAL.SaveRoyaltorTaxDetailsData(royaltorId, royContTaxDetailsList, deleteList, UserCode, out royaltor, out iErrorId);
        }
    }
}
