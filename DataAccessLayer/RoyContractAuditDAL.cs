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
    public class RoyContractAuditDAL : IRoyContractAuditDAL
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

        public DataSet GetRoyContractAuditData(Int32 royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pRoyAuditData = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_roy_contract_audit.p_get_contract_audit_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Int32;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pRoyAuditData.OracleDbType = OracleDbType.RefCursor;
                pRoyAuditData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoyAuditData);               

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                                
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

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

        public DataSet GetRoyReserveAuditData(Int32 royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inRoyaltorId = new OracleParameter();
                OracleParameter curRsvAuditData = new OracleParameter();
                OracleParameter curRoyAuditData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_roy_contract_audit.p_get_reserve_audit_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inRoyaltorId.OracleDbType = OracleDbType.Int32;
                inRoyaltorId.Direction = ParameterDirection.Input;
                inRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(inRoyaltorId);

                curRsvAuditData.OracleDbType = OracleDbType.RefCursor;
                curRsvAuditData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curRsvAuditData);

                curRoyAuditData.OracleDbType = OracleDbType.RefCursor;
                curRoyAuditData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curRoyAuditData);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

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
        #endregion
    }
}