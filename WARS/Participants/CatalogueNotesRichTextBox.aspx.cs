using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WARS.BusinessLayer;

namespace WARS.Participants
{
    public partial class CatalogueNotesRichTextBox : System.Web.UI.Page
    {
        #region Global Declarations
        CatalogueNotesBL catalogueNotesBL;
        Int32 errorId;
        string catNo = string.Empty;
        #endregion Global Declarations

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                catNo = Convert.ToString(Session["CatalogueNo"]);
                hdnCatNo.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this, typeof(Page), "SetTxtNotesHeight", "SetTxtNotesHeight();", true);

                catalogueNotesBL = new CatalogueNotesBL();
                DataSet notesData = catalogueNotesBL.GetCatalogueNotes(catNo, out errorId);
                catalogueNotesBL = null;

                if (errorId != 2)
                {
                    //populate royaltor text field in parent page
                    hdnCatNo.Value = catNo;
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "PopulateCatNo", "iFramePopulateCatNo();", true);

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
                            txtNotes.Text = rtBox.Text.Replace("\n", "<br>");
                        }
                        else
                        {
                            txtNotes.Text = notes;
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
                ExceptionHandler("Error in loading Catalogue notes", ex.Message);
            }

        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            Response.Redirect(@"~/Common/ExceptionPage.aspx", true);
        }
    }
}