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
    public class StatementTextMaintenanceDAL : IStatementTextMaintenanceDAL
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
                OracleParameter pStmtTextDetails = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_statement_text.p_get_company_dropdownlist", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pStmtTextDetails.OracleDbType = OracleDbType.RefCursor;
                pStmtTextDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtTextDetails);

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

        public DataSet GetStatementTextDetails(string companyCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter pStmtTextDetails = new OracleParameter();
                OracleParameter pCompanyCode = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_statement_text.p_get_statement_text_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCompanyCode.OracleDbType = OracleDbType.Varchar2;
                pCompanyCode.Direction = ParameterDirection.Input;
                pCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(pCompanyCode);


                pStmtTextDetails.OracleDbType = OracleDbType.RefCursor;
                pStmtTextDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtTextDetails);

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
        public DataSet SaveStatementTextDetails(string companyCode, Array updateList, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();

                OracleParameter pCompanyCode = new OracleParameter();
                OracleParameter pUpdateList = new OracleParameter();
                OracleParameter pDeleteList = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pStmtTextDetails = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_statement_text.p_save_statement_text_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCompanyCode.OracleDbType = OracleDbType.Varchar2;
                pCompanyCode.Direction = ParameterDirection.Input;
                pCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(pCompanyCode);

                pUpdateList.OracleDbType = OracleDbType.Varchar2;
                pUpdateList.Direction = ParameterDirection.Input;
                pUpdateList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (updateList.Length == 0)
                {
                    pUpdateList.Size = 1;
                    pUpdateList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pUpdateList.Size = updateList.Length;
                    pUpdateList.Value = updateList;
                }
                orlCmd.Parameters.Add(pUpdateList);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pStmtTextDetails.OracleDbType = OracleDbType.RefCursor;
                pStmtTextDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtTextDetails);

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
