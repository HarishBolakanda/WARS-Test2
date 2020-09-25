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
    public class PaymentDetailsBL : IPaymentDetailsBL
    {
        IPaymentDetailsDAL PaymentDetailsDAL;
        #region Constructor
        public PaymentDetailsBL()
        {
            PaymentDetailsDAL = new PaymentDetailsDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return PaymentDetailsDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetSupplierList(string searchText, out Int32 iErrorId)
        {
            return PaymentDetailsDAL.GetSupplierList(searchText, out iErrorId);
        }

        public DataSet GetSearchedData(string paymentNo, string paymentDate, string statusCode, string supplierNumber, string supplierSiteName, string paymentAmount, string filename, string royaltorId, out Int32 iErrorId)
        {
            return PaymentDetailsDAL.GetSearchedData(paymentNo, paymentDate, statusCode, supplierNumber, supplierSiteName, paymentAmount, filename, royaltorId, out iErrorId);
        }

        public void CreateAPInterface()
        {
            PaymentDetailsDAL.CreateAPInterface();
        }
    }
}
