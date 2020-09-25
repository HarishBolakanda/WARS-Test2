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
    public class TransactionRetrievalStatusDAL : ITransactionRetrievalStatusDAL
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

        public DataSet GetInitialTransactionData(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter curTransactions = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_trans_retrieval_status.p_get_initial_trans_retrieval", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curTransactions.OracleDbType = OracleDbType.RefCursor;
                curTransactions.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTransactions);

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

        public DataSet GetSearchData(string royaltorId, string optionPeriodCode, string artistId, string catNumber, string projectCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pOptionPreiodCode = new OracleParameter();
                OracleParameter pArtistId = new OracleParameter();
                OracleParameter pCatNumber = new OracleParameter();
                OracleParameter pProjectCode = new OracleParameter();
                OracleParameter pCurCatalogues = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_trans_retrieval_status.p_get_catalogue_search", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                if (royaltorId == string.Empty)
                    pRoyaltorId.Value = DBNull.Value;
                else
                    pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pOptionPreiodCode.OracleDbType = OracleDbType.Varchar2;
                pOptionPreiodCode.Direction = ParameterDirection.Input;
                if (royaltorId == string.Empty)
                    pOptionPreiodCode.Value = DBNull.Value;
                else
                    pOptionPreiodCode.Value = optionPeriodCode;
                orlCmd.Parameters.Add(pOptionPreiodCode);

                pArtistId.OracleDbType = OracleDbType.Varchar2;
                pArtistId.Direction = ParameterDirection.Input;
                if (artistId == string.Empty)
                    pArtistId.Value = DBNull.Value;
                else
                    pArtistId.Value = artistId;
                orlCmd.Parameters.Add(pArtistId);

                pCatNumber.OracleDbType = OracleDbType.Varchar2;
                pCatNumber.Direction = ParameterDirection.Input;
                if (catNumber == string.Empty)
                    pCatNumber.Value = DBNull.Value;
                else
                    pCatNumber.Value = catNumber;
                orlCmd.Parameters.Add(pCatNumber);

                pProjectCode.OracleDbType = OracleDbType.Varchar2;
                pProjectCode.Direction = ParameterDirection.Input;
                if (projectCode == string.Empty)
                    pProjectCode.Value = DBNull.Value;
                else
                    pProjectCode.Value = projectCode;
                orlCmd.Parameters.Add(pProjectCode);

                pCurCatalogues.OracleDbType = OracleDbType.RefCursor;
                pCurCatalogues.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCurCatalogues);

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
