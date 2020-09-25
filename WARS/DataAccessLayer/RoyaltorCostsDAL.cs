using System;
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
    public class RoyaltorCostsDAL : IRoyaltorCostsDAL
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

        public DataSet GetDropDownData(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();                                
                OracleParameter cur_royaltor = new OracleParameter();                
                OracleParameter cur_account_type = new OracleParameter();
                
                orlCmd = new OracleCommand("pkg_royaltor_costs_screen.p_get_dropdownlist_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                
                cur_royaltor.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor);
               
                cur_account_type.OracleDbType = OracleDbType.RefCursor;
                cur_account_type.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_account_type);
                
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

        public DataSet GetStmtPeriods(string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pStmtPeriods = new OracleParameter();

                orlCmd = new OracleCommand("pkg_royaltor_costs_screen.p_get_stmt_periods", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltorId);

                pStmtPeriods.OracleDbType = OracleDbType.RefCursor;
                pStmtPeriods.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtPeriods);

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

        public DataSet GetSearchData(string royaltor, string statementPeriod, string fromDate, string toDate, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pStmtPeriodId = new OracleParameter();
                OracleParameter pFromDate = new OracleParameter();
                OracleParameter pToDate = new OracleParameter();                                
                OracleParameter pCostData = new OracleParameter();
                orlCmd = new OracleCommand("pkg_royaltor_costs_screen.p_get_search_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;                
                pRoyaltorId.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltorId);

                pStmtPeriodId.OracleDbType = OracleDbType.Varchar2;
                pStmtPeriodId.Direction = ParameterDirection.Input;
                if (statementPeriod == "-")
                    pStmtPeriodId.Value = DBNull.Value;
                else
                    pStmtPeriodId.Value = statementPeriod;
                orlCmd.Parameters.Add(pStmtPeriodId);

                pFromDate.OracleDbType = OracleDbType.Varchar2;
                pFromDate.Direction = ParameterDirection.Input;
                if (fromDate == "")
                    pFromDate.Value = DBNull.Value;
                else
                    pFromDate.Value = fromDate;
                orlCmd.Parameters.Add(pFromDate);

                pToDate.OracleDbType = OracleDbType.Varchar2;
                pToDate.Direction = ParameterDirection.Input;
                if (toDate == "")
                    pToDate.Value = DBNull.Value;
                else
                    pToDate.Value = toDate;
                orlCmd.Parameters.Add(pToDate);
                
                pCostData.OracleDbType = OracleDbType.RefCursor;
                pCostData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCostData);  
                
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

        public DataSet AddCost(string royaltor, string statementPeriod, string fromDate, string toDate, string accountTypeId, string description, string date, string amount,
            string suppName, string projCode, string invNum, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pStmtPeriodId = new OracleParameter();
                OracleParameter pFromDate = new OracleParameter();
                OracleParameter pToDate = new OracleParameter();  
                OracleParameter pAccountTypeId = new OracleParameter();
                OracleParameter pDescription = new OracleParameter();
                OracleParameter pDate = new OracleParameter();
                OracleParameter pAmount = new OracleParameter();
                OracleParameter pSuppName = new OracleParameter();
                OracleParameter pProjCode = new OracleParameter();
                OracleParameter pInvNum = new OracleParameter();
                OracleParameter pCostData = new OracleParameter();
                orlCmd = new OracleCommand("pkg_royaltor_costs_screen.p_add_cost_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltorId);

                pStmtPeriodId.OracleDbType = OracleDbType.Varchar2;
                pStmtPeriodId.Direction = ParameterDirection.Input;
                if (statementPeriod == "-")
                    pStmtPeriodId.Value = DBNull.Value;
                else
                    pStmtPeriodId.Value = statementPeriod;
                orlCmd.Parameters.Add(pStmtPeriodId);

                pFromDate.OracleDbType = OracleDbType.Varchar2;
                pFromDate.Direction = ParameterDirection.Input;
                if (fromDate == "")
                    pFromDate.Value = DBNull.Value;
                else
                    pFromDate.Value = fromDate;
                orlCmd.Parameters.Add(pFromDate);

                pToDate.OracleDbType = OracleDbType.Varchar2;
                pToDate.Direction = ParameterDirection.Input;
                if (toDate == "")
                    pToDate.Value = DBNull.Value;
                else
                    pToDate.Value = toDate;
                orlCmd.Parameters.Add(pToDate);
                                
                pAccountTypeId.OracleDbType = OracleDbType.Varchar2;
                pAccountTypeId.Direction = ParameterDirection.Input;
                pAccountTypeId.Value = accountTypeId;
                orlCmd.Parameters.Add(pAccountTypeId);

                pDescription.OracleDbType = OracleDbType.Varchar2;
                pDescription.Direction = ParameterDirection.Input;
                pDescription.Value = description;
                orlCmd.Parameters.Add(pDescription);

                pDate.OracleDbType = OracleDbType.Varchar2;
                pDate.Direction = ParameterDirection.Input;
                pDate.Value = date;
                orlCmd.Parameters.Add(pDate);

                pAmount.OracleDbType = OracleDbType.Varchar2;
                pAmount.Direction = ParameterDirection.Input;
                pAmount.Value = amount;
                orlCmd.Parameters.Add(pAmount);

                pSuppName.OracleDbType = OracleDbType.Varchar2;
                pSuppName.Direction = ParameterDirection.Input;
                pSuppName.Value = suppName;
                orlCmd.Parameters.Add(pSuppName);

                pProjCode.OracleDbType = OracleDbType.Varchar2;
                pProjCode.Direction = ParameterDirection.Input;
                pProjCode.Value = projCode;
                orlCmd.Parameters.Add(pProjCode);

                pInvNum.OracleDbType = OracleDbType.Varchar2;
                pInvNum.Direction = ParameterDirection.Input;
                pInvNum.Value = invNum;
                orlCmd.Parameters.Add(pInvNum);

                pCostData.OracleDbType = OracleDbType.RefCursor;
                pCostData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCostData);

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

        public DataSet UpdateCost(string journalEntryId, string royaltor, string statementPeriod, string fromDate, string toDate, string description, string date, string amount,
            string suppName, string projCode, string invNum, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter pjournalEntryId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pStmtPeriodId = new OracleParameter();
                OracleParameter pFromDate = new OracleParameter();
                OracleParameter pToDate = new OracleParameter();  
                OracleParameter pDescription = new OracleParameter();
                OracleParameter pDate = new OracleParameter();
                OracleParameter pAmount = new OracleParameter();
                OracleParameter pSuppName = new OracleParameter();
                OracleParameter pProjCode = new OracleParameter();
                OracleParameter pInvNum = new OracleParameter();
                OracleParameter pCostData = new OracleParameter();
                orlCmd = new OracleCommand("pkg_royaltor_costs_screen.p_update_cost_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pjournalEntryId.OracleDbType = OracleDbType.Varchar2;
                pjournalEntryId.Direction = ParameterDirection.Input;
                pjournalEntryId.Value = journalEntryId;
                orlCmd.Parameters.Add(pjournalEntryId);

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltorId);

                pStmtPeriodId.OracleDbType = OracleDbType.Varchar2;
                pStmtPeriodId.Direction = ParameterDirection.Input;
                if (statementPeriod == "-")
                    pStmtPeriodId.Value = DBNull.Value;
                else
                    pStmtPeriodId.Value = statementPeriod;
                orlCmd.Parameters.Add(pStmtPeriodId);

                pFromDate.OracleDbType = OracleDbType.Varchar2;
                pFromDate.Direction = ParameterDirection.Input;
                if (fromDate == "")
                    pFromDate.Value = DBNull.Value;
                else
                    pFromDate.Value = fromDate;
                orlCmd.Parameters.Add(pFromDate);

                pToDate.OracleDbType = OracleDbType.Varchar2;
                pToDate.Direction = ParameterDirection.Input;
                if (toDate == "")
                    pToDate.Value = DBNull.Value;
                else
                    pToDate.Value = toDate;
                orlCmd.Parameters.Add(pToDate);

                pDescription.OracleDbType = OracleDbType.Varchar2;
                pDescription.Direction = ParameterDirection.Input;
                pDescription.Value = description;
                orlCmd.Parameters.Add(pDescription);

                pDate.OracleDbType = OracleDbType.Varchar2;
                pDate.Direction = ParameterDirection.Input;
                pDate.Value = date;
                orlCmd.Parameters.Add(pDate);

                pAmount.OracleDbType = OracleDbType.Varchar2;
                pAmount.Direction = ParameterDirection.Input;
                pAmount.Value = amount;
                orlCmd.Parameters.Add(pAmount);

                pSuppName.OracleDbType = OracleDbType.Varchar2;
                pSuppName.Direction = ParameterDirection.Input;
                pSuppName.Value = suppName;
                orlCmd.Parameters.Add(pSuppName);

                pProjCode.OracleDbType = OracleDbType.Varchar2;
                pProjCode.Direction = ParameterDirection.Input;
                pProjCode.Value = projCode;
                orlCmd.Parameters.Add(pProjCode);

                pInvNum.OracleDbType = OracleDbType.Varchar2;
                pInvNum.Direction = ParameterDirection.Input;
                pInvNum.Value = invNum;
                orlCmd.Parameters.Add(pInvNum);

                pCostData.OracleDbType = OracleDbType.RefCursor;
                pCostData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCostData);

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

        public DataSet DeleteCost(string journalEntryId, string royaltor, string statementPeriod, string fromDate, string toDate, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter pjournalEntryId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pStmtPeriodId = new OracleParameter();
                OracleParameter pFromDate = new OracleParameter();
                OracleParameter pToDate = new OracleParameter();  
                OracleParameter pCostData = new OracleParameter();
                orlCmd = new OracleCommand("pkg_royaltor_costs_screen.p_delete_cost_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pjournalEntryId.OracleDbType = OracleDbType.Varchar2;
                pjournalEntryId.Direction = ParameterDirection.Input;
                pjournalEntryId.Value = journalEntryId;
                orlCmd.Parameters.Add(pjournalEntryId);

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltorId);

                pStmtPeriodId.OracleDbType = OracleDbType.Varchar2;
                pStmtPeriodId.Direction = ParameterDirection.Input;
                if (statementPeriod == "-")
                    pStmtPeriodId.Value = DBNull.Value;
                else
                    pStmtPeriodId.Value = statementPeriod;
                orlCmd.Parameters.Add(pStmtPeriodId);

                pFromDate.OracleDbType = OracleDbType.Varchar2;
                pFromDate.Direction = ParameterDirection.Input;
                if (fromDate == "")
                    pFromDate.Value = DBNull.Value;
                else
                    pFromDate.Value = fromDate;
                orlCmd.Parameters.Add(pFromDate);

                pToDate.OracleDbType = OracleDbType.Varchar2;
                pToDate.Direction = ParameterDirection.Input;
                if (toDate == "")
                    pToDate.Value = DBNull.Value;
                else
                    pToDate.Value = toDate;
                orlCmd.Parameters.Add(pToDate);

                pCostData.OracleDbType = OracleDbType.RefCursor;
                pCostData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCostData);

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
