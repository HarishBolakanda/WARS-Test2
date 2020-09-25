/*
File Name   :   WorkFlowCommentRichText.cs
Purpose     :   Rich textbox for workflow screen comments

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     27-Dec-2018     Harish(Infosys Limited)   Initial Creation
        
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using WARS.BusinessLayer;

namespace WARS.StatementProcessing
{
    public partial class WorkFlowCommentRichText : System.Web.UI.Page
    {
        #region Global Declarations
        WorkFlowBL workFlowBL;
        Utilities util;
        Int32 errorId;
        string royaltorId;
        string stmtPeriodId;
        #endregion Global Declarations

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {                
                if (Session["WorkflowCommentRoyId"] != null)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "SetTxtCommentHeight", "SetTxtCommentHeight();", true);
                    royaltorId = Convert.ToString(Session["WorkflowCommentRoyId"]);
                    stmtPeriodId = Convert.ToString(Session["WorkflowCommentStmtPeriodId"]);

                    workFlowBL = new WorkFlowBL();
                    DataSet commentData = workFlowBL.GetComment(royaltorId, stmtPeriodId, out errorId);
                    workFlowBL = null;

                    if (errorId != 2)
                    {
                        if (commentData.Tables[0].Rows.Count != 0)
                        {
                            string comment = Convert.ToString(commentData.Tables[0].Rows[0][0]);
                            //old data which was saved using Power Builder was in Rich Text format(RTF)
                            //check if comment is in rich text format and extract only the text if so
                            if (comment != string.Empty && (comment.StartsWith("{\rtf1") || comment.StartsWith("{\\rtf1")))
                            {
                                System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
                                rtBox.Rtf = Convert.ToString(commentData.Tables[0].Rows[0][0]);

                                //rtf new line does not work in HTML. replacing new line with HTML break
                                txtWorkFlowComment.Text = rtBox.Text.Replace("\n", "<br>");
                            }
                            else
                            {
                                txtWorkFlowComment.Text = comment;
                            }
                        }
                        else
                        {
                            txtWorkFlowComment.Text = string.Empty;
                        }

                        txtWorkFlowComment.Focus();

                    }
                    else
                    {
                        ExceptionHandler("Error in loading workflow statement comment", string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading workflow statement comment", ex.Message);
            }
        }


        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            Response.Redirect(@"~/Common/ExceptionPage.aspx", true);
        }

    }
}