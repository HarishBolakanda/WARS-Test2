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

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter curTerritoriesList = new OracleParameter();
                OracleParameter curTerritoryTypeList = new OracleParameter();
                OracleParameter curCountryList = new OracleParameter();


                orlCmd = new OracleCommand("pkg_territory_search.p_get_intial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curTerritoriesList.OracleDbType = OracleDbType.RefCursor;
                curTerritoriesList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTerritoriesList);

                curTerritoryTypeList.OracleDbType = OracleDbType.RefCursor;
                curTerritoryTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTerritoryTypeList);

                curCountryList.OracleDbType = OracleDbType.RefCursor;
                curCountryList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCountryList);

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

        public void AddTerritoryToGroups(string territoryCode, Array territoryGroupcodes, string userCode, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inTerritoryCode = new OracleParameter();
                OracleParameter inGroupCodescode = new OracleParameter();
                OracleParameter inUsercode = new OracleParameter();

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

                inUsercode.OracleDbType = OracleDbType.Varchar2;
                inUsercode.Direction = ParameterDirection.Input;
                inUsercode.Value = userCode;
                orlCmd.Parameters.Add(inUsercode);

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

        public DataSet InsertUpdateTerritoryGroup(string flag, string territoryCode, string territoryName, string territoryLocation, string countryCode, string territoryType, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inFlag = new OracleParameter();
                OracleParameter inTerritoryCode = new OracleParameter();
                OracleParameter inTerritoryName = new OracleParameter();
                OracleParameter inTerritoryLocation = new OracleParameter();
                OracleParameter inCountryCode = new OracleParameter();
                OracleParameter inTerritoryType = new OracleParameter();
                OracleParameter inUsercode = new OracleParameter();
                OracleParameter curTerritoriesList = new OracleParameter();
                OracleParameter curTerritoriesData = new OracleParameter();
                

                orlCmd = new OracleCommand("pkg_territory_search.p_save_territory_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inFlag.OracleDbType = OracleDbType.Varchar2;
                inFlag.Direction = ParameterDirection.Input;
                inFlag.Value = flag;
                orlCmd.Parameters.Add(inFlag);

                inTerritoryCode.OracleDbType = OracleDbType.Varchar2;
                inTerritoryCode.Direction = ParameterDirection.Input;
                inTerritoryCode.Value = territoryCode;
                orlCmd.Parameters.Add(inTerritoryCode);

                inTerritoryName.OracleDbType = OracleDbType.Varchar2;
                inTerritoryName.Direction = ParameterDirection.Input;
                inTerritoryName.Value = territoryName;
                orlCmd.Parameters.Add(inTerritoryName);

                inTerritoryLocation.OracleDbType = OracleDbType.Varchar2;
                inTerritoryLocation.Direction = ParameterDirection.Input;
                inTerritoryLocation.Value = territoryLocation;
                orlCmd.Parameters.Add(inTerritoryLocation);

                inCountryCode.OracleDbType = OracleDbType.Varchar2;
                inCountryCode.Direction = ParameterDirection.Input;
                inCountryCode.Value = countryCode;
                orlCmd.Parameters.Add(inCountryCode);

                inTerritoryType.OracleDbType = OracleDbType.Varchar2;
                inTerritoryType.Direction = ParameterDirection.Input;
                inTerritoryType.Value = territoryType;
                orlCmd.Parameters.Add(inTerritoryType);

                inUsercode.OracleDbType = OracleDbType.Varchar2;
                inUsercode.Direction = ParameterDirection.Input;
                inUsercode.Value = userCode;
                orlCmd.Parameters.Add(inUsercode);

                curTerritoriesList.OracleDbType = OracleDbType.RefCursor;
                curTerritoriesList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTerritoriesList);

                curTerritoriesData.OracleDbType = OracleDbType.RefCursor;
                curTerritoriesData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTerritoriesData);

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
