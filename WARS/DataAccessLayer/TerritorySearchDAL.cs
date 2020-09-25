using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WARS.IDAL;
using Oracle.DataAccess.Types;
using WARS.DataAccessLayer;

namespace WARS.DataAccessLayer
{
    public class TerritorySearchDAL: ITerritorySearchDAL
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

        public DataSet GetTerritoriesList(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter curTerritoriesList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_territory_search.p_get_territories", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curTerritoriesList.OracleDbType = OracleDbType.RefCursor;
                curTerritoriesList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTerritoriesList);

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
            }
            finally
            {
                CloseConnection();
            }
            return ds;
        }

        public DataSet GetTerritoryData(string territoryCode, out Int32 iErrorId)
        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inTerritoryCode = new OracleParameter();
                OracleParameter curTerritoryData = new OracleParameter();
                OracleParameter curTerritoryGroupData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_territory_search.p_get_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inTerritoryCode.OracleDbType = OracleDbType.Varchar2;
                inTerritoryCode.Direction = ParameterDirection.Input;
                inTerritoryCode.Value = territoryCode;
                orlCmd.Parameters.Add(inTerritoryCode);

                curTerritoryData.OracleDbType = OracleDbType.RefCursor;
                curTerritoryData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTerritoryData);

                curTerritoryGroupData.OracleDbType = OracleDbType.RefCursor;
                curTerritoryGroupData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTerritoryGroupData);

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
            }
            finally
            {
                CloseConnection();
            }
            return ds;
        }

        public void AddTerritoryToGroups(string territoryCode, Array territoryGroupcodes, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inTerritoryCode = new OracleParameter();
                OracleParameter inGroupCodescode = new OracleParameter();

                orlCmd = new OracleCommand("pkg_territory_search.p_add_territory_to_groups", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inTerritoryCode.OracleDbType = OracleDbType.Varchar2;
                inTerritoryCode.Direction = ParameterDirection.Input;
                inTerritoryCode.Value = territoryCode;
                orlCmd.Parameters.Add(inTerritoryCode);

                inGroupCodescode.OracleDbType = OracleDbType.Varchar2;
                inGroupCodescode.Direction = ParameterDirection.Input;
                inGroupCodescode.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (territoryGroupcodes.Length == 0)
                {
                    inGroupCodescode.Size = 0;
                    inGroupCodescode.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inGroupCodescode.Size = territoryGroupcodes.Length;
                    inGroupCodescode.Value = territoryGroupcodes;
                }
                orlCmd.Parameters.Add(inGroupCodescode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
            }
            catch (Exception ex)
            {
                iErrorId = 2;
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
