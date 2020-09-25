/*
File Name   :   ArtistAudit.cs
Purpose     :   to display Artist Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     21-Sep-2018      Sreelekha                 Initial Creation
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Net;
using System.IO;
using System.Xml;
using WARS.BusinessLayer;
using System.Threading;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Web.SessionState;
using System.Text;
using System.Drawing;

namespace WARS
{
    public partial class ArtistAudit : System.Web.UI.Page
    {
        #region Global Declarations
        ArtistAuditBL artistAuditBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string sessionArtist = "", sessionDealType = "", sessionResponsibility = "";
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Artist Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Artist Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlArtistDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtArtist.Focus();
                    tdData.Style.Add("display", "none");

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void fuzzySearchArtist_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchArtist();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in artist search.", ex.Message);
            }
        }

        protected void btnHdnArtistSearch_Click(object sender, EventArgs e)
        {
            try
            {
                int artistId = Convert.ToInt32(txtArtist.Text.Split('-')[0].ToString().Trim());
                LoadArtistAuditData(artistId);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading artist details.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtArtist.Text = string.Empty;
                    return;
                }
               
                txtArtist.Text = lbFuzzySearch.SelectedValue.ToString();
                int artistId = Convert.ToInt32(txtArtist.Text.Split('-')[0].ToString().Trim());
                LoadArtistAuditData(artistId);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting Artist.", ex.Message);
            }
        }
        
        protected void gvAuditDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string changeType;
                Label lblArtistDesc;
                Label lblDealType;
                Label lblTeamResponsibility;
                Label lblManagerResponsibility;
                Label lblUpdatedby;
                Label lblUpdatedon;
                
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    changeType = (e.Row.FindControl("hdnChangeType") as HiddenField).Value;
                    lblArtistDesc = (e.Row.FindControl("lblArtistDesc") as Label);
                    lblDealType = (e.Row.FindControl("lblDealType") as Label);
                    lblTeamResponsibility = (e.Row.FindControl("lblTeamResponsibility") as Label);
                    lblManagerResponsibility = (e.Row.FindControl("lblManagerResponsibility") as Label);
                    lblUpdatedby = (e.Row.FindControl("lblUpdatedby") as Label);
                    lblUpdatedon = (e.Row.FindControl("lblUpdatedon") as Label);

                     //For deleted records 
                    //Change the color of details to red
                    if (changeType == "D")
                    {
                        lblArtistDesc.ForeColor = Color.Red;
                        lblDealType.ForeColor = Color.Red;
                        lblTeamResponsibility.ForeColor = Color.Red;
                        lblManagerResponsibility.ForeColor = Color.Red;
                        lblUpdatedby.ForeColor = Color.Red; ;
                        lblUpdatedon.ForeColor = Color.Red; ;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }

        protected void btnArtistMaint_Click(object sender, EventArgs e)
        {
            if (hdnIsValidSearch.Value == "Y")
            {
                ArtistSession();   
            }

            Response.Redirect("../DataMaintenance/ArtistResponsibilityMaintenance.aspx?fromAudit=Y");
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {
            
            if (Request.QueryString.Count > 0 && !string.IsNullOrEmpty(Request.QueryString[0]))
            {
                int artistId = Convert.ToInt32(Request.QueryString[0]);

                if (artistId != 0)
                {
                    LoadArtistAuditData(artistId);
                }
                else
                {
                    dtEmpty = new DataTable();
                    gvArtistDetails.DataSource = dtEmpty;
                    gvArtistDetails.DataBind();
                }

            }
        }

        private void LoadArtistAuditData(int artistId)
        {
            hdnIsValidSearch.Value = "Y";

            string artist;
            artistAuditBL = new ArtistAuditBL();
            DataSet artistData = artistAuditBL.GetArtistAuditData(artistId, out artist, out errorId);
            artistAuditBL = null;

           txtArtist.Text = artist;

            if (artistData.Tables.Count != 0 && errorId != 2)
            {
                BindGrid(artistData.Tables[0]);
            }
            else
            {
                ExceptionHandler("Error in fetching Artist data", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            tdData.Style.Remove("display");

            if (gridData.Rows.Count > 0)
            {
                gvArtistDetails.DataSource = gridData;
                gvArtistDetails.DataBind();
                CompareRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvArtistDetails.DataSource = dtEmpty;
                gvArtistDetails.DataBind();
            }
        }

        private void CompareRows()
        {
            for (int i = 0; i < gvArtistDetails.Rows.Count - 1; i++)
            {
                //exit if it is last row
                if (i == (gvArtistDetails.Rows.Count - 1))
                {
                    break;
                }

                GridViewRow currentRow = gvArtistDetails.Rows[i];

                if ((currentRow.FindControl("hdnDisplayOrder") as HiddenField).Value == "1")
                {
                    continue;
                }
                
                GridViewRow nextRow = gvArtistDetails.Rows[i + 1];

                //Comapre Artist Name
                if ((currentRow.FindControl("lblArtistDesc") as Label).Text != (nextRow.FindControl("lblArtistDesc") as Label).Text)
                {
                    (currentRow.FindControl("lblArtistDesc") as Label).ForeColor = Color.Red;
                }
                //Comapre DealType
                if ((currentRow.FindControl("lblDealType") as Label).Text != (nextRow.FindControl("lblDealType") as Label).Text)
                {
                    (currentRow.FindControl("lblDealType") as Label).ForeColor = Color.Red;
                }
                //Comapre Team Responsibility
                if ((currentRow.FindControl("lblTeamResponsibility") as Label).Text != (nextRow.FindControl("lblTeamResponsibility") as Label).Text)
                {
                    (currentRow.FindControl("lblTeamResponsibility") as Label).ForeColor = Color.Red;
                }
                //Comapre Manager Responsibility
                if ((currentRow.FindControl("lblManagerResponsibility") as Label).Text != (nextRow.FindControl("lblManagerResponsibility") as Label).Text)
                {
                    (currentRow.FindControl("lblManagerResponsibility") as Label).ForeColor = Color.Red;
                }

            }
        }

        private void FuzzySearchArtist()
        {
            if (txtArtist.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text Artist search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllArtisList(txtArtist.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();

        }
        
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        private void ArtistSession()
        {
            if (Session["ArtistMaintFilters"] != null)
            {
                DataTable dtArtistSearchedFilters = Session["ArtistMaintFilters"] as DataTable;

                foreach (DataRow dRow in dtArtistSearchedFilters.Rows)
                {
                    if (dRow["filter_name"].ToString() == "txtArtist")
                    {
                        sessionArtist = dRow["filter_value"].ToString();
                    }
                    else if (dRow["filter_name"].ToString() == "ddlDealType")
                    {
                        sessionDealType = dRow["filter_value"].ToString();
                    }
                    else if (dRow["filter_name"].ToString() == "ddlResponsibility")
                    {
                        sessionResponsibility = dRow["filter_value"].ToString();
                    }
                }
            }

            DataTable dtSearchedFilters = new DataTable();
            dtSearchedFilters.Columns.Add("filter_name", typeof(string));
            dtSearchedFilters.Columns.Add("filter_value", typeof(string));

            string artistName = txtArtist.Text.Split('-')[1].ToString().Trim();
            dtSearchedFilters.Rows.Add("txtArtist", artistName);

            if (sessionArtist == artistName)
            {
                dtSearchedFilters.Rows.Add("ddlDealType", sessionDealType);
                dtSearchedFilters.Rows.Add("ddlResponsibility", sessionResponsibility);
            }

            Session["ArtistMaintFilters"] = dtSearchedFilters;
        }

    
        #endregion Methods
     
       
    }
}