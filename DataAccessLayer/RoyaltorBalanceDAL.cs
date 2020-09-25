﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WARS.IDAL;
using Oracle.DataAccess.Types;

namespace WARS.DataAccessLayer
{
    public class RoyaltorBalanceDAL : IRoyaltorBalanceDAL
    {   
        #region Global Declarations
        ConnectionDAL connDAL;
        OracleConnection orlConn;
        OracleTransaction txn;
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;
        DataSet ds;
        OracleParameter ErrorId;
        string sErrorMsg;
        #endregion Global Declarations
       
        public DataSet GetRoyaltorDate(string royaltorId,out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inRoyaltorId = new OracleParameter();
                OracleParameter curRoyaltorData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_royaltor_balance.p_get_royaltor_date", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                inRoyaltorId.Direction = ParameterDirection.Input;
                inRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(inRoyaltorId);

                curRoyaltorData.OracleDbType = OracleDbType.RefCursor;
                curRoyaltorData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curRoyaltorData);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "out_n_status_code";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                // iErrorId = Convert.ToInt32(orlCmd.Parameters["out_n_status_code"].Value.ToString());                

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet GetSearchedData(string royaltorId, string dateType, string balanceDate, string voucherDate, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inRoyaltorId = new OracleParameter();
                OracleParameter inDateType = new OracleParameter();
                OracleParameter inBalanceDate = new OracleParameter();
                OracleParameter inVoucherDate = new OracleParameter();
                OracleParameter curRoyaltorBalance = new OracleParameter();
                OracleParameter curRoyaltorEarnings = new OracleParameter();
                OracleParameter curJournalData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_royaltor_balance.p_get_searched_royaltor", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                inRoyaltorId.Direction = ParameterDirection.Input;
                inRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(inRoyaltorId);

                inDateType.OracleDbType = OracleDbType.Varchar2;
                inDateType.Direction = ParameterDirection.Input;
                inDateType.Value = dateType;
                orlCmd.Parameters.Add(inDateType);


                inBalanceDate.OracleDbType = OracleDbType.Varchar2;
                inBalanceDate.Direction = ParameterDirection.Input;
                inBalanceDate.Value = balanceDate;
                orlCmd.Parameters.Add(inBalanceDate);

                inVoucherDate.OracleDbType = OracleDbType.Varchar2;
                inVoucherDate.Direction = ParameterDirection.Input;
                inVoucherDate.Value = voucherDate;
                orlCmd.Parameters.Add(inVoucherDate);

                curRoyaltorBalance.OracleDbType = OracleDbType.RefCursor;
                curRoyaltorBalance.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curRoyaltorBalance);

                curRoyaltorEarnings.OracleDbType = OracleDbType.RefCursor;
                curRoyaltorEarnings.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curRoyaltorEarnings);

                curJournalData.OracleDbType = OracleDbType.RefCursor;
                curJournalData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curJournalData);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "out_n_status_code";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

               // iErrorId = Convert.ToInt32(orlCmd.Parameters["out_n_status_code"].Value.ToString());

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        #region Private Methods
        private void OpenConnection(out Int32 iErrorId, out string sErrorMsg)
        {
            connDAL = new ConnectionDAL();
            orlConn = connDAL.Open(out iErrorId, out sErrorMsg);
        }

        public void CloseConnection()
        {
            if (connDAL != null)
            {
                connDAL.Close();
            }
        }
        #endregion Private Methods
    }
}