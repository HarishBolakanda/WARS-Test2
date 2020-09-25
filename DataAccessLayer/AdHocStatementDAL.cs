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
    public class AdHocStatementDAL : IAdHocStatementDAL
    {

        #region Global Declarations        
        ConnectionDAL connDAL;
        OracleConnection orlConn;        
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
                
                OracleParameter pOwners = new OracleParameter();
                orlCmd = new OracleCommand("pkg_adhoc_stmt_screen.p_get_dropdownlist_values", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                
                pOwners.OracleDbType = OracleDbType.RefCursor;
                pOwners.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOwners);

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

        public DataSet GetOwnerGroupData(string ownerCode, out string isAllowed, out Int32 iErrorId)
        {
            ds = new DataSet();
            isAllowed = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter pOwnerCode = new OracleParameter();
                OracleParameter pIsAllowed = new OracleParameter();
                OracleParameter pRoyaltors = new OracleParameter();                
                orlCmd = new OracleCommand("pkg_adhoc_stmt_screen.p_get_owner_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pOwnerCode.Direction = ParameterDirection.Input;
                pOwnerCode.Value = ownerCode;
                orlCmd.Parameters.Add(pOwnerCode);

                pIsAllowed.OracleDbType = OracleDbType.Varchar2;
                pIsAllowed.Size = 200;
                pIsAllowed.Direction = ParameterDirection.Output;
                pIsAllowed.ParameterName = "IsAllowed";
                orlCmd.Parameters.Add(pIsAllowed);

                pRoyaltors.OracleDbType = OracleDbType.RefCursor;
                pRoyaltors.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoyaltors);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                isAllowed = orlCmd.Parameters["IsAllowed"].Value.ToString();
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

        public void SaveStatement(string descriptionStartDate, string descriptionEndDate, string stmtStartDate, string stmtEndDate, string paymentDate, Array roysToAdd, string loggedUser, out string stmtTypeCode, out Int32 iErrorId)
        {
            try
            {
                stmtTypeCode = string.Empty;
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter pDescriptionStartDate = new OracleParameter();
                OracleParameter pDescriptionEndDate = new OracleParameter();
                OracleParameter pStmtStartDate = new OracleParameter();
                OracleParameter pStmtEndDate = new OracleParameter();
                OracleParameter pStmtTypeCode = new OracleParameter();
                OracleParameter pPaymentDate = new OracleParameter();
                OracleParameter pRoysToAdd = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();                
                orlCmd = new OracleCommand("pkg_adhoc_stmt_screen.p_create_adhoc_stmt", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pDescriptionStartDate.OracleDbType = OracleDbType.Varchar2;
                pDescriptionStartDate.Direction = ParameterDirection.Input;
                pDescriptionStartDate.Value = descriptionStartDate;
                orlCmd.Parameters.Add(pDescriptionStartDate);

                pDescriptionEndDate.OracleDbType = OracleDbType.Varchar2;
                pDescriptionEndDate.Direction = ParameterDirection.Input;
                pDescriptionEndDate.Value = descriptionEndDate;
                orlCmd.Parameters.Add(pDescriptionEndDate);

                pStmtStartDate.OracleDbType = OracleDbType.Varchar2;
                pStmtStartDate.Direction = ParameterDirection.Input;
                pStmtStartDate.Value = stmtStartDate;
                orlCmd.Parameters.Add(pStmtStartDate);

                pStmtEndDate.OracleDbType = OracleDbType.Varchar2;
                pStmtEndDate.Direction = ParameterDirection.Input;
                pStmtEndDate.Value = stmtEndDate;
                orlCmd.Parameters.Add(pStmtEndDate);

                pPaymentDate.OracleDbType = OracleDbType.Varchar2;
                pPaymentDate.Direction = ParameterDirection.Input;
                pPaymentDate.Value = paymentDate;
                orlCmd.Parameters.Add(pPaymentDate);

                pRoysToAdd.OracleDbType = OracleDbType.Varchar2;
                pRoysToAdd.Direction = ParameterDirection.Input;
                pRoysToAdd.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (roysToAdd.Length == 0)
                {
                    pRoysToAdd.Size = 1;
                    pRoysToAdd.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pRoysToAdd.Size = roysToAdd.Length;
                    pRoysToAdd.Value = roysToAdd;
                }
                orlCmd.Parameters.Add(pRoysToAdd);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pStmtTypeCode.OracleDbType = OracleDbType.Varchar2;
                pStmtTypeCode.Size = 200;
                pStmtTypeCode.Direction = ParameterDirection.Output;
                pStmtTypeCode.ParameterName = "StmtTypeCode";
                orlCmd.Parameters.Add(pStmtTypeCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();
                stmtTypeCode = orlCmd.Parameters["StmtTypeCode"].Value.ToString();
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
