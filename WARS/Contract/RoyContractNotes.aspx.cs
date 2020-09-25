/*
File Name   :   RoyContractNotes.cs
Purpose     :   to add/edit notes of royaltor contract

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     26-Feb-2018     Harish                 WUIN-367- initial creation
*/


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
    public partial class RoyContractNotes : System.Web.UI.Page
    {

        #region Global Declarations
        RoyContractNotesBL royContractNotesBL;
        Utilities util;
        Int32 errorId;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        #endregion Global Declarations

        #region EVENTS
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                royaltorId = Request.QueryString["RoyaltorId"];
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];

                //royaltorId = "14928";
                //royaltorId = "12340";
                //isNewRoyaltor = "Y";

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Notes";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Notes";
                }

                iFrameNotes.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        Button btnNotes = (Button)contractNavigationButtons.FindControl("btnNotes");
                        btnNotes.Enabled = false;
                        HiddenField hdnRoyaltorId = (HiddenField)contractNavigationButtons.FindControl("hdnRoyaltorId");
                        hdnRoyaltorId.Value = royaltorId;
                        HiddenField hdnRoyaltorIdHdr = (HiddenField)contractHdrNavigation.FindControl("hdnRoyaltorId");
                        hdnRoyaltorIdHdr.Value = royaltorId;

                        //WUIN-599 - resetting the flags
                        ((HiddenField)Master.FindControl("hdnIsContractScreen")).Value = "N";
                        ((HiddenField)Master.FindControl("hdnIsNotContractScreen")).Value = "N";
                        this.Master.FindControl("lnkBtnHome").Visible = false;


                        if (isNewRoyaltor == "Y")
                        {
                            btnSave.Text = "Save & Continue";
                            hdnIsNewRoyaltor.Value = "Y";

                            //enable contract buttons on this screen as this is the last screen of contract creation
                            //contractNavigationButtons.Disable();

                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.Notes.ToString());

                        }

                        txtRoyaltorId.Text = royaltorId;
                        LoadNotesData();

                        //WUIN-450 - If a new Royaltor is set up with Lock checked it should continue to allow entry of the details (the Royaltor set up needs to be completed!)
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId))
                        {
                            btnSave.ToolTip = "Royaltor Locked";
                        }

                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        // If a contract is already using by another user then making the screen readonly.
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId))
                        {
                            hdnOtherUserScreenLocked.Value = "Y";
                        }

                        //WUIN-1096 - Only Read access for ReadonlyUser
                        //WUIN-599 If a contract is already using by another user then making the screen readonly.
                        //WUIN-450 -Only Read access for locked contracts
                        if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId)) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId)))
                        {
                            EnableReadonly();
                        }
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                }

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
                royContractNotesBL = new RoyContractNotesBL();
                royContractNotesBL.SaveRoyContractNotes(royaltorId, notes, Convert.ToString(Session["UserCode"]), out errorId);
                royContractNotesBL = null;

                if (errorId == 2)
                {
                    ExceptionHandler("Error in saving notes", string.Empty);
                }
                else
                {
                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.Notes.ToString());

                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("Royaltor notes saved successfully", MessageType.Warning, PositionType.Auto);
                    }
                }

                hdnIsDataChanged.Value = "N";

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving notes", ex.Message);
            }
        }

        #endregion EVENTS

        #region METHODS

        private void LoadNotesData()
        {
            Session["RoyContractNotesRoyId"] = royaltorId;
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }


        private void EnableReadonly()
        {
            btnSave.Enabled = false;
        }
        #endregion METHODS



        #region Web Methods

        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static int UpdateScreenLockFlag()
        {
            int errorId;
            RoyaltorContractBL royContractBL = new RoyaltorContractBL();
            royContractBL.UpdateScreenLockFlag(HttpContext.Current.Session["ScreenLockedRoyaltorId"].ToString(), "N", HttpContext.Current.Session["UserCode"].ToString(), out errorId);
            royContractBL = null;
            return errorId;

        }

        #endregion Web Methods



    }
}