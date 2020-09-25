using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WARS.IDAL;
using Oracle.DataAccess.Types;
using System;

namespace WARS.DataAccessLayer
{
    public class AutoParticipantMaintenanceDAL : IAutoParticipantMaintenanceDAL
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

        public DataSet GetDropDownData(string autoPariticp_Id, string loggedInUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pautoPariticp_Id = new OracleParameter();
                OracleParameter pLoggedInUser = new OracleParameter();
                OracleParameter cur_Auto_Participant_Data = new OracleParameter();
                OracleParameter cur_Auto_Participant_Grid_details = new OracleParameter();
                OracleParameter cur_Option_Period_details = new OracleParameter();
                OracleParameter cur_Esc_Code_details = new OracleParameter();
                OracleParameter cur_Territory_list_details = new OracleParameter();
                OracleParameter cur_Tune_Code_details = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_auto_participant.p_load_dropdown_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pautoPariticp_Id.OracleDbType = OracleDbType.Varchar2;
                pautoPariticp_Id.Direction = ParameterDirection.Input;
                pautoPariticp_Id.Value = autoPariticp_Id;
                orlCmd.Parameters.Add(pautoPariticp_Id);

                pLoggedInUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedInUser.Direction = ParameterDirection.Input;
                pLoggedInUser.Value = loggedInUser;
                orlCmd.Parameters.Add(pLoggedInUser);

                cur_Option_Period_details.OracleDbType = OracleDbType.RefCursor;
                cur_Option_Period_details.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_Option_Period_details);

                cur_Esc_Code_details.OracleDbType = OracleDbType.RefCursor;
                cur_Esc_Code_details.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_Esc_Code_details);

                cur_Tune_Code_details.OracleDbType = OracleDbType.RefCursor;
                cur_Tune_Code_details.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_Tune_Code_details);

                cur_Territory_list_details.OracleDbType = OracleDbType.RefCursor;
                cur_Territory_list_details.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_Territory_list_details);

                cur_Auto_Participant_Data.OracleDbType = OracleDbType.RefCursor;
                cur_Auto_Participant_Data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_Auto_Participant_Data);

                cur_Auto_Participant_Grid_details.OracleDbType = OracleDbType.RefCursor;
                cur_Auto_Participant_Grid_details.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_Auto_Participant_Grid_details);

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

        public DataSet SaveAutoParticipantDetails(string autoPariticp_ID, Array autoParticipantsToAddUpdate, string userRole, string loggedInUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();

                OracleParameter p_auto_participID = new OracleParameter();
                OracleParameter pAddUpdateList = new OracleParameter();
                OracleParameter pUserRole = new OracleParameter();
                OracleParameter pLoggedInUserRole = new OracleParameter();
                OracleParameter cur_auto_partip_Maint = new OracleParameter();
                OracleParameter cur_auto_partip_Maint_details = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_auto_participant.p_add_update_auto_participant", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                p_auto_participID.OracleDbType = OracleDbType.Int32;
                p_auto_participID.Direction = ParameterDirection.Input;
                p_auto_participID.Value = autoPariticp_ID;
                orlCmd.Parameters.Add(p_auto_participID);

                pAddUpdateList.OracleDbType = OracleDbType.Varchar2;
                pAddUpdateList.Direction = ParameterDirection.Input;
                pAddUpdateList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (autoParticipantsToAddUpdate.Length == 0)
                {
                    pAddUpdateList.Size = 1;
                    pAddUpdateList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pAddUpdateList.Size = autoParticipantsToAddUpdate.Length;
                    pAddUpdateList.Value = autoParticipantsToAddUpdate;
                }
                orlCmd.Parameters.Add(pAddUpdateList);

                pUserRole.OracleDbType = OracleDbType.Varchar2;
                pUserRole.Direction = ParameterDirection.Input;
                pUserRole.Value = userRole;
                orlCmd.Parameters.Add(pUserRole);

                pLoggedInUserRole.OracleDbType = OracleDbType.Varchar2;
                pLoggedInUserRole.Direction = ParameterDirection.Input;
                pLoggedInUserRole.Value = loggedInUser;
                orlCmd.Parameters.Add(pLoggedInUserRole);

                cur_auto_partip_Maint.OracleDbType = OracleDbType.RefCursor;
                cur_auto_partip_Maint.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_auto_partip_Maint);

                cur_auto_partip_Maint_details.OracleDbType = OracleDbType.RefCursor;
                cur_auto_partip_Maint_details.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_auto_partip_Maint_details);

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
