﻿using System;
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
    public class TransactionRetrievalBL:ITransactionRetrievalBL
    {
        ITransactionRetrievalDAL TransactionRetrievalDAL;
        #region Constructor
        public TransactionRetrievalBL()
        {
            TransactionRetrievalDAL = new TransactionRetrievalDAL();
        }
        #endregion Constructor
        
        public DataSet GetSearchData(string royaltorId, string optionPeriodCode, string artistId, string catNumber, string projectCode, out Int32 iErrorId)
        {
            return TransactionRetrievalDAL.GetSearchData(royaltorId, optionPeriodCode, artistId, catNumber, projectCode, out iErrorId);
        }

        public DataSet AddToRetrieveTrans(Array catNums, string startDate, string endDate, string royaltorId, string optionPeriodCode, string artistId, string catNumber, string projectCode,
                                    string loggedUser, string isOverwrite, out Int32 iErrorId)
        {
            return TransactionRetrievalDAL.AddToRetrieveTrans(catNums, startDate, endDate, royaltorId, optionPeriodCode, artistId, catNumber, projectCode, loggedUser, isOverwrite, out iErrorId);
        }
        

    }
}
