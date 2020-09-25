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
    public class AutoParticipantSearchDAL : IAutoParticipantSearchDAL
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

        public DataSet SearchAutoParticipantData(string marketingOwer, string weaSalesLable, string artist, string projectTitle, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter inMarketingOwer = new OracleParameter();
                OracleParameter inWEASalesLable = new OracleParameter();
                OracleParameter inArtist = new OracleParameter();
                OracleParameter inProjectTitle = new OracleParameter();
                OracleParameter pAutoParticipantData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_auto_participant_search.p_search_auto_participants", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;


                inMarketingOwer.OracleDbType = OracleDbType.Varchar2;
                inMarketingOwer.Direction = ParameterDirection.Input;
                inMarketingOwer.Value = marketingOwer;
                orlCmd.Parameters.Add(inMarketingOwer);


                inWEASalesLable.OracleDbType = OracleDbType.Varchar2;
                inWEASalesLable.Direction = ParameterDirection.Input;
                inWEASalesLable.Value = weaSalesLable;
                orlCmd.Parameters.Add(inWEASalesLable);


                inArtist.OracleDbType = OracleDbType.Varchar2;
                inArtist.Direction = ParameterDirection.Input;
                inArtist.Value = artist;
                orlCmd.Parameters.Add(inArtist);

                inProjectTitle.OracleDbType = OracleDbType.Varchar2;
                inProjectTitle.Direction = ParameterDirection.Input;
                inProjectTitle.Value = projectTitle;
                orlCmd.Parameters.Add(inProjectTitle);

                pAutoParticipantData.OracleDbType = OracleDbType.RefCursor;
                pAutoParticipantData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pAutoParticipantData);

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


        public DataSet AddUpdateAutoParticipant(Int32 autoPartId, string marketingOwer, string weaSalesLable, Int32 artistId, Int32 projectId, string projectTitle, string marketingOnwerSearch, string weaSalesLabelSearch, string artistSearch, string projectSearch, string userCode, out Int32 newAutoPartId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inAutoPartId = new OracleParameter();
                OracleParameter inMarketingOwer = new OracleParameter();
                OracleParameter inWEASalesLable = new OracleParameter();
                OracleParameter inArtistId = new OracleParameter();
                OracleParameter inProjectId = new OracleParameter();
                OracleParameter inProjectTitle = new OracleParameter();

                OracleParameter inMarketingOwerSearch = new OracleParameter();
                OracleParameter inWEASalesLableSearch = new OracleParameter();
                OracleParameter inArtistSearch = new OracleParameter();
                OracleParameter inProjectSearch = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter pAutoParticipantData = new OracleParameter();
                OracleParameter outNewAutoPartId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_auto_participant_search.p_add_update_auto_participant", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inAutoPartId.OracleDbType = OracleDbType.Int32;
                inAutoPartId.Direction = ParameterDirection.Input;
                inAutoPartId.Value = autoPartId;
                orlCmd.Parameters.Add(inAutoPartId);

                inMarketingOwer.OracleDbType = OracleDbType.Varchar2;
                inMarketingOwer.Direction = ParameterDirection.Input;
                inMarketingOwer.Value = marketingOwer;
                orlCmd.Parameters.Add(inMarketingOwer);


                inWEASalesLable.OracleDbType = OracleDbType.Varchar2;
                inWEASalesLable.Direction = ParameterDirection.Input;
                inWEASalesLable.Value = weaSalesLable;
                orlCmd.Parameters.Add(inWEASalesLable);


                inArtistId.OracleDbType = OracleDbType.Int32;
                inArtistId.Direction = ParameterDirection.Input;
                inArtistId.Value = artistId;
                orlCmd.Parameters.Add(inArtistId);

                inProjectId.OracleDbType = OracleDbType.Int32;
                inProjectId.Direction = ParameterDirection.Input;
                inProjectId.Value = projectId;
                orlCmd.Parameters.Add(inProjectId);

                inProjectTitle.OracleDbType = OracleDbType.Varchar2;
                inProjectTitle.Direction = ParameterDirection.Input;
                inProjectTitle.Value = projectTitle;
                orlCmd.Parameters.Add(inProjectTitle);

                inMarketingOwerSearch.OracleDbType = OracleDbType.Varchar2;
                inMarketingOwerSearch.Direction = ParameterDirection.Input;
                inMarketingOwerSearch.Value = marketingOnwerSearch;
                orlCmd.Parameters.Add(inMarketingOwerSearch);

                inWEASalesLableSearch.OracleDbType = OracleDbType.Varchar2;
                inWEASalesLableSearch.Direction = ParameterDirection.Input;
                inWEASalesLableSearch.Value = weaSalesLabelSearch;
                orlCmd.Parameters.Add(inWEASalesLableSearch);

                inArtistSearch.OracleDbType = OracleDbType.Varchar2;
                inArtistSearch.Direction = ParameterDirection.Input;
                inArtistSearch.Value = artistSearch;
                orlCmd.Parameters.Add(inArtistSearch);

                inProjectSearch.OracleDbType = OracleDbType.Varchar2;
                inProjectSearch.Direction = ParameterDirection.Input;
                inProjectSearch.Value = projectSearch;
                orlCmd.Parameters.Add(inProjectSearch);


                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);


                pAutoParticipantData.OracleDbType = OracleDbType.RefCursor;
                pAutoParticipantData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pAutoParticipantData);

                outNewAutoPartId.OracleDbType = OracleDbType.Int32;
                outNewAutoPartId.Direction = ParameterDirection.Output;
                outNewAutoPartId.ParameterName = "NewAutoPartId";
                orlCmd.Parameters.Add(outNewAutoPartId);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                if (iErrorId == 0)
                {
                    newAutoPartId = Convert.ToInt32(orlCmd.Parameters["NewAutoPartId"].Value.ToString());
                }
                else { newAutoPartId = 0; }

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
