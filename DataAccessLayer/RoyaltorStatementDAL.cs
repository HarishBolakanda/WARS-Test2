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
    public class RoyaltorStatementDAL : IRoyaltorStatementDAL
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


        public DataSet GetRoyaltors(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();
                                
                OracleParameter cur_owners = new OracleParameter();

                orlCmd = new OracleCommand("pkg_royaltor_stmt_screen.p_get_dropdown_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                cur_owners.OracleDbType = OracleDbType.RefCursor;
                cur_owners.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_owners);

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

        public DataSet GetRoyStmtData(Int32 royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter cur_royaltor_stmt_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_royaltor_stmt_screen.p_get_royaltor_stmt_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                cur_royaltor_stmt_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_stmt_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_stmt_data);

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

        public DataSet UpdateRoyStmt(string royaltorID, Array stmtsToAdd, Array stmtsToRemove, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();
                OracleParameter royaltor_id = new OracleParameter();
                OracleParameter stmts_to_add = new OracleParameter();
                OracleParameter stmts_to_remove = new OracleParameter();
                OracleParameter logged_user = new OracleParameter();              
                OracleParameter cur_roy_stmt_data = new OracleParameter();
                orlCmd = new OracleCommand("pkg_royaltor_stmt_screen.p_update_royaltor_stmt_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                royaltor_id.OracleDbType = OracleDbType.Varchar2;
                royaltor_id.Direction = ParameterDirection.Input;
                royaltor_id.Value = royaltorID;
                orlCmd.Parameters.Add(royaltor_id);

                stmts_to_add.OracleDbType = OracleDbType.Int32;
                stmts_to_add.Direction = ParameterDirection.Input;
                stmts_to_add.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (stmtsToAdd.Length == 0)
                {
                    stmts_to_add.Size = 1;
                    stmts_to_add.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    stmts_to_add.Size = stmtsToAdd.Length;
                    stmts_to_add.Value = stmtsToAdd;
                }
                orlCmd.Parameters.Add(stmts_to_add);

                stmts_to_remove.OracleDbType = OracleDbType.Int32;
                stmts_to_remove.Direction = ParameterDirection.Input;
                stmts_to_remove.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (stmtsToRemove.Length == 0)
                {
                    stmts_to_remove.Size = 1;
                    stmts_to_remove.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    stmts_to_remove.Size = stmtsToRemove.Length;
                    stmts_to_remove.Value = stmtsToRemove;
                }
                orlCmd.Parameters.Add(stmts_to_remove);

                logged_user.OracleDbType = OracleDbType.Varchar2;
                logged_user.Direction = ParameterDirection.Input;
                logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(logged_user);

                cur_roy_stmt_data.OracleDbType = OracleDbType.RefCursor;
                cur_roy_stmt_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_roy_stmt_data);

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

        public DataSet GetOwnerStmtData(Int32 ownerCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_owner_id = new OracleParameter();
                //OracleParameter cur_owner_details = new OracleParameter();
                OracleParameter cur_owner_stmt_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_royaltor_stmt_screen.p_get_owner_stmt_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_owner_id.OracleDbType = OracleDbType.Int32;
                in_v_owner_id.Direction = ParameterDirection.Input;
                in_v_owner_id.Value = ownerCode;
                orlCmd.Parameters.Add(in_v_owner_id);

                //cur_owner_details.OracleDbType = OracleDbType.RefCursor;
                //cur_owner_details.Direction = System.Data.ParameterDirection.Output;
                //orlCmd.Parameters.Add(cur_owner_details);

                cur_owner_stmt_data.OracleDbType = OracleDbType.RefCursor;
                cur_owner_stmt_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_owner_stmt_data);

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

        public DataSet UpdateOwnerStmt(Int32 ownerCode, Array royaltorIds, Array stmtsToAdd, Array stmtsToRemove, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter owner_code = new OracleParameter();
                OracleParameter royaltor_ids = new OracleParameter();
                OracleParameter stmts_to_add = new OracleParameter();
                OracleParameter stmts_to_remove = new OracleParameter();
                OracleParameter logged_user = new OracleParameter();
                //OracleParameter cur_owner_details = new OracleParameter();
                OracleParameter cur_owner_stmt_data = new OracleParameter();
                orlCmd = new OracleCommand("pkg_royaltor_stmt_screen.p_update_owner_stmt_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                owner_code.OracleDbType = OracleDbType.Varchar2;
                owner_code.Direction = ParameterDirection.Input;
                owner_code.Value = ownerCode;
                orlCmd.Parameters.Add(owner_code);

                royaltor_ids.OracleDbType = OracleDbType.Int32;
                royaltor_ids.Direction = ParameterDirection.Input;
                royaltor_ids.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (royaltorIds.Length == 0)
                {
                    royaltor_ids.Size = 1;
                    royaltor_ids.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    royaltor_ids.Size = royaltorIds.Length;
                    royaltor_ids.Value = royaltorIds;
                }
                orlCmd.Parameters.Add(royaltor_ids);

                stmts_to_add.OracleDbType = OracleDbType.Int32;
                stmts_to_add.Direction = ParameterDirection.Input;
                stmts_to_add.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (stmtsToAdd.Length == 0)
                {
                    stmts_to_add.Size = 1;
                    stmts_to_add.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    stmts_to_add.Size = stmtsToAdd.Length;
                    stmts_to_add.Value = stmtsToAdd;
                }
                orlCmd.Parameters.Add(stmts_to_add);

                stmts_to_remove.OracleDbType = OracleDbType.Int32;
                stmts_to_remove.Direction = ParameterDirection.Input;
                stmts_to_remove.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (stmtsToRemove.Length == 0)
                {
                    stmts_to_remove.Size = 1;
                    stmts_to_remove.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    stmts_to_remove.Size = stmtsToRemove.Length;
                    stmts_to_remove.Value = stmtsToRemove;
                }
                orlCmd.Parameters.Add(stmts_to_remove);

                logged_user.OracleDbType = OracleDbType.Varchar2;
                logged_user.Direction = ParameterDirection.Input;
                logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(logged_user);

                //cur_owner_details.OracleDbType = OracleDbType.RefCursor;
                //cur_owner_details.Direction = System.Data.ParameterDirection.Output;
                //orlCmd.Parameters.Add(cur_owner_details);

                cur_owner_stmt_data.OracleDbType = OracleDbType.RefCursor;
                cur_owner_stmt_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_owner_stmt_data);

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