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
    public class TransactionRetrievalStatusBL : ITransactionRetrievalStatusBL
    {
        ITransactionRetrievalStatusDAL TransactionRetrievalStatusDAL;
        #region Constructor
        public TransactionRetrievalStatusBL()
        {
            TransactionRetrievalStatusDAL = new TransactionRetrievalStatusDAL();
        }
        #endregion Constructor

        public DataSet GetInitialTransactionData(out Int32 iErrorId)
        {
            return TransactionRetrievalStatusDAL.GetInitialTransactionData(out iErrorId);
        }
        
        public DataSet GetSearchData(string royaltorId, string optionPeriodCode, string artistId, string catNumber, string projectCode, out Int32 iErrorId)
        {
            return TransactionRetrievalStatusDAL.GetSearchData(royaltorId, optionPeriodCode, artistId, catNumber, projectCode, out iErrorId);
        }
    }
}
