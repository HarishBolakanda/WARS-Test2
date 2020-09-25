using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WARS.BusinessLayer;

namespace WARS.Participants
{
    public partial class CatalogueNotes : System.Web.UI.Page
    {
        #region Global Declarations
        CatalogueNotesBL catalogueNotesBL;
        Utilities util;
        Int32 errorId;
        string catNo = string.Empty;
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                catNo = Request.QueryString["CatNo"];

                if (catNo == null)
                {
                    msgView.SetMessage("Not a valid CatNo!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Catalogue - Notes";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Catalogue - Notes";
                }

                iFrameNotes.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        txtCatNo.Text = catNo;
                        LoadCatalogueNotesData();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }
                }

                UserAuthorization();
            }
            catch (ThreadAbortException ex1)
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string notes = txtHidData.Value.ToString();
                catalogueNotesBL = new CatalogueNotesBL();
                catalogueNotesBL.SaveCatalogueNotes(catNo, notes, Convert.ToString(Session["UserCode"]), out errorId);
                catalogueNotesBL = null;

                if (errorId == 2)
                {
                    ExceptionHandler("Error in saving notes", string.Empty);
                }
                else
                {
                    msgView.SetMessage("Catalogue notes saved successfully", MessageType.Warning, PositionType.Auto);
                }

                hdnIsDataChanged.Value = "N";

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving notes", ex.Message);
            }
        }

        protected void btnCatalogueSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Participants/CatalogueSearch.aspx?isNewRequest=N", false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        #endregion Events

        #region METHODS

        private void LoadCatalogueNotesData()
        {
            Session["CatalogueNo"] = catNo;
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSave.Enabled = false;

            }

        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        #endregion METHODS

    }
}