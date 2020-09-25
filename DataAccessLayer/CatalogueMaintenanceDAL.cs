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
    public class CatalogueMaintenanceDAL : ICatalogueMaintenanceDAL
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

        public DataSet GetInitialData(string catNo, out Int32 trackListingCount, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCatNo = new OracleParameter();
                OracleParameter curCatalogueData = new OracleParameter();                
                OracleParameter curMurOwnerList = new OracleParameter();
                OracleParameter curConfigGroupList = new OracleParameter();
                OracleParameter curTimeTrackList = new OracleParameter();
                OracleParameter curCatStatusList = new OracleParameter();                
                OracleParameter outTrackListingCount = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_catalogue.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCatNo.OracleDbType = OracleDbType.Varchar2;
                inCatNo.Direction = ParameterDirection.Input;
                inCatNo.Value = (catNo != string.Empty ? catNo.ToUpper() : string.Empty); ;
                orlCmd.Parameters.Add(inCatNo);

                curCatalogueData.OracleDbType = OracleDbType.RefCursor;
                curCatalogueData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCatalogueData);

                curMurOwnerList.OracleDbType = OracleDbType.RefCursor;
                curMurOwnerList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curMurOwnerList);

                curConfigGroupList.OracleDbType = OracleDbType.RefCursor;
                curConfigGroupList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curConfigGroupList);

                curTimeTrackList.OracleDbType = OracleDbType.RefCursor;
                curTimeTrackList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTimeTrackList);

                curCatStatusList.OracleDbType = OracleDbType.RefCursor;
                curCatStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCatStatusList);

                outTrackListingCount.OracleDbType = OracleDbType.Int32;
                outTrackListingCount.Direction = ParameterDirection.Output;
                outTrackListingCount.ParameterName = "TrackListingCount";
                orlCmd.Parameters.Add(outTrackListingCount);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                trackListingCount = Convert.ToInt32(orlCmd.Parameters["TrackListingCount"].Value.ToString());
            }
            catch (Exception ex)
            {
                iErrorId = 2;
                trackListingCount = 0;
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet GetSearchFilters(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter curArtistList = new OracleParameter();
                OracleParameter curProjectList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_catalogue.p_get_search_filters", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                
                curArtistList.OracleDbType = OracleDbType.RefCursor;
                curArtistList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curArtistList);

                curProjectList.OracleDbType = OracleDbType.RefCursor;
                curProjectList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curProjectList);

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

        public DataSet GetCatnoParticipants(string catNo, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();                
                OracleParameter curParticipants = new OracleParameter();
                OracleParameter curISRCParticipants = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_catalogue.p_get_catno_participants", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = (catNo != string.Empty ? catNo.ToUpper() : string.Empty);
                orlCmd.Parameters.Add(pCatNo);

                curParticipants.OracleDbType = OracleDbType.RefCursor;
                curParticipants.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curParticipants);

                curISRCParticipants.OracleDbType = OracleDbType.RefCursor;
                curISRCParticipants.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curISRCParticipants);

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

        public DataSet SaveCatalogueDetails(string catNo, Array catalogueDetails, string userCode, out Int32 trackListingCount, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCatNo = new OracleParameter();
                OracleParameter inCatalogueDetails = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curCatalogueData = new OracleParameter();
                OracleParameter outTrackListingCount = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_catalogue.p_save_catalogue_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCatNo.OracleDbType = OracleDbType.Varchar2;
                inCatNo.Direction = ParameterDirection.Input;
                inCatNo.Value = (catNo != string.Empty ? catNo.ToUpper() : string.Empty);
                orlCmd.Parameters.Add(inCatNo);

                inCatalogueDetails.OracleDbType = OracleDbType.Varchar2;
                inCatalogueDetails.Direction = ParameterDirection.Input;
                inCatalogueDetails.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (catalogueDetails.Length == 0)
                {
                    inCatalogueDetails.Size = 0;
                    inCatalogueDetails.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inCatalogueDetails.Size = catalogueDetails.Length;
                    inCatalogueDetails.Value = catalogueDetails;
                }
                orlCmd.Parameters.Add(inCatalogueDetails);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curCatalogueData.OracleDbType = OracleDbType.RefCursor;
                curCatalogueData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCatalogueData);

                outTrackListingCount.OracleDbType = OracleDbType.Int32;
                outTrackListingCount.Direction = ParameterDirection.Output;
                outTrackListingCount.ParameterName = "TrackListingCount";
                orlCmd.Parameters.Add(outTrackListingCount);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                trackListingCount = Convert.ToInt32(orlCmd.Parameters["TrackListingCount"].Value.ToString());

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                trackListingCount = 0;
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
