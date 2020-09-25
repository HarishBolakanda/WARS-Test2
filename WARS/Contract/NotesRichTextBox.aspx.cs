using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Data;
using WARS.BusinessLayer;


namespace WARS.Contract
{
    public partial class NotesRichTextBox : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractNotesBL royContractNotesBL;        
        Int32 errorId;
        string royaltorId = string.Empty;        
        #endregion Global Declarations

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                royaltorId = Convert.ToString(Session["RoyContractNotesRoyId"]);
                hdnNotesRoyaltor.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this, typeof(Page), "SetTxtNotesHeight", "SetTxtNotesHeight();", true);
                
                string royaltor;
                royContractNotesBL = new RoyContractNotesBL();
                DataSet notesData = royContractNotesBL.GetRoyContractNotes(royaltorId, out royaltor, out errorId);
                royContractNotesBL = null;

                if (errorId != 2)
                {
                    //populate royaltor text field in parent page
                    hdnNotesRoyaltor.Value = royaltor;
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "PopulateRoyaltor", "iFramePopulateRoy();", true);

                    if (notesData.Tables[0].Rows.Count != 0)
                    {
                        string notes = Convert.ToString(notesData.Tables[0].Rows[0][0]);
                        //old data which was saved using Power Builder was in Rich Text format(RTF)
                        //check if notes is in rich text format and extract only the text if so
                        if (notes != string.Empty && (notes.StartsWith("{\rtf1") || notes.StartsWith("{\\rtf1")))
                        {
                            System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
                            rtBox.Rtf = Convert.ToString(notesData.Tables[0].Rows[0][0]);

                            //rtf new line does not work in HTML. replacing new line with HTML break
                            // Decode HTML otherwise HTMLEditorExtender displays tags on postback
                            //txtNotes.Text = rtBox.Text.Replace("\n", "<br>");
                            txtNotes.Text = HttpUtility.HtmlDecode( rtBox.Text.Replace("\n", "<br>"));
                            
                        }
                        else
                        {
                            // Decode HTML otherwise HTMLEditorExtender displays tags on postback
                            //txtNotes.Text = notes;
                            txtNotes.Text = HttpUtility.HtmlDecode(notes);
                        }
                    }
                    else
                    {
                        txtNotes.Text = string.Empty;
                    }


                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading contract notes control", ex.Message);
            }

        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {   
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;            
            Response.Redirect(@"~/Common/ExceptionPage.aspx", true);
        }

    }
}