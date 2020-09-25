/*
File Name   :   BOReports.cs
Purpose     :   to view and generate BO reports

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     16-Sep-2016     Harish(Infosys Limited)   Initial Creation
2.0     11-May-2018     Harish                    WUIN-632 change: as it can list only 50 documents info max at a time, need a logic to increment the offset and limt values by 50 each time till 
 *                                                all the documents are fetched and get the one present in the folder  
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
using CrystalDecisions.Enterprise;

namespace WARS
{
    public partial class BOReports : System.Web.UI.Page
    {

        #region Global Declarations
        Utilities util;
        string baseURL = ConfigurationManager.AppSettings["BOServerBaseURL"];
        string reportDocUrl = ConfigurationManager.AppSettings["BOReportsURL"];
        int webReqTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["WebReqTimeout"].ToString());
        string BORptFolder = string.Empty;
        string rwsLogonToken;
        #endregion Global Declarations

        #region Sorting
        string ascending = "Asc";
        string descending = "Desc";
        string sort_Up = "~/Images/Sort_up.png";
        string sort_Down = "~/Images/Sort_down.png";
        #endregion Sorting

        #region EVENTS
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BORptFolder = Request.QueryString["folder"];

                    if (BORptFolder == "Adhoc")
                    {
                        lblScreenName.Text = "BO AD-HOC REPORTS";
                    }
                    else if (BORptFolder == "Archive")
                    {
                        lblScreenName.Text = "BO ARCHIVE REPORTS";
                    }
                    else if (BORptFolder == "Audit")
                    {
                        lblScreenName.Text = "BO AUDIT REPORTS";
                    }
                    else if (BORptFolder == "Checking")
                    {
                        lblScreenName.Text = "BO STATEMENT CHECKING REPORTS";
                    }

                    lblTab.Focus();//tabbing sequence starts here

                    if (Session["DatabaseName"] != null)
                    {
                        this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "BO Reports";
                    }
                    else
                    {
                        util = new Utilities();
                        string dbName = util.GetDatabaseName();
                        util = null;
                        Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                        this.Title = dbName.Split('.')[0].ToString() + " - " + "BO Reports";
                    }

                    GetReportList();
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

        protected void btnOpenRpt_Click(object sender, EventArgs e)
        {

            try
            {
                GridViewRow gvr = ((LinkButton)sender).NamingContainer as GridViewRow;
                string cuid = (gvr.FindControl("hdnCUID") as HiddenField).Value;
                string logonToken = GetLogonToken();
                string url = reportDocUrl + "/BOE/OpenDocument/opendoc/openDocument.jsp?sIDType=CUID&iDocID=" + cuid + "&token=" + logonToken + "&sRefresh=Y";

                ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_REPORT", "window.open( '" + url + "', '_blank');", true);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in opening Business Objects report", ex.Message);
            }


        }


        protected void gvBOReports_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
            if (e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.HasControls())
                    {
                        LinkButton lnkHeader = (LinkButton)tc.Controls[0];
                        lnkHeader.Style.Add("color", "black");
                        lnkHeader.Style.Add("text-decoration", "none");

                        if (lnkHeader != null && hdnSortExpression.Value == lnkHeader.CommandArgument)
                        {
                            // initialize a new image
                            Image imgSort = new Image();
                            imgSort.ImageUrl = (hdnSortDirection.Value == ascending) ? sort_Up : sort_Down;
                            // adding a space and the image to the header link
                            tc.Controls.Add(new LiteralControl(" "));
                            tc.Controls.Add(imgSort);
                        }

                    }
                }
            }
            //JIRA-746 Changes by Ravi on 05/03/2019 -- End
        }

        protected void gvBOReports_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortingDirection = string.Empty;
            Utilities util = new Utilities();
            string sortDirec = util.SortingDirection(hdnSortDirection.Value);
            if (sortDirec == ascending)
            {
                sortingDirection = descending;
            }
            else
            {
                sortingDirection = ascending;
            }
            DataTable dataTable = (DataTable)Session["BOReportsData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvBOReports.DataSource = dataView;
                gvBOReports.DataBind();
            }
        }
        #endregion EVENTS

        #region METHODS

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        private void LoginBO()
        {
            string userName = Utilities.GetBOAccountUserId();
            string password = Utilities.GetBOAccountUserPassoword();
            string auth = "secEnterprise";
            string LogonURI = baseURL + "logon/long";

            try
            {
                //Making GET Request to  /logon/long to receive XML template.        
                HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(LogonURI);
                myWebRequest.ContentType = "application/xml";
                myWebRequest.Method = "GET";

                //Returns the response to the request made
                WebResponse myWebResponse = myWebRequest.GetResponse();
                //Creating an instance of StreamReader to read the data stream from the resource
                StreamReader sr = new StreamReader(myWebResponse.GetResponseStream());
                //Reads all the characters from the current position to the end of the stream and store it as string
                string output = sr.ReadToEnd();
                //Initialize a new instance of the XmlDocument class
                XmlDocument doc = new XmlDocument();
                //Loads the document from the specified URI
                doc.LoadXml(output);

                //Returns an XmlNodeList containing a list of all descendant elements 
                //that match the specified name i.e. attr
                XmlNodeList nodelist = doc.GetElementsByTagName("attr");
                //  Add the logon parameters to the attribute nodes of the document
                foreach (XmlNode node in nodelist)
                {
                    if (node.Attributes["name"].Value == "userName")
                        node.InnerText = userName;

                    if (node.Attributes["name"].Value == "password")
                        node.InnerText = password;

                    if (node.Attributes["name"].Value == "auth")
                        node.InnerText = auth;
                }

                //Making POST request to /logon/long to receive a logon token            
                WebRequest myWebRequest1 = WebRequest.Create(LogonURI);
                myWebRequest1.ContentType = "application/xml";
                myWebRequest1.Method = "POST";

                byte[] reqBodyBytes = System.Text.Encoding.Default.GetBytes(doc.OuterXml);
                Stream reqStream = myWebRequest1.GetRequestStream();
                reqStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
                reqStream.Close();

                using (var myWebResponse1 = myWebRequest1.GetResponse())
                {
                    //Finding the value of the X-SAP-LogonToken
                    rwsLogonToken = myWebResponse1.Headers["X-SAP-LogonToken"].ToString();
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in logging Business Objects", ex.Message);
            }

        }

        private void LogOffBO()
        {
            string LogOffURI = baseURL + "logoff";

            try
            {
                //Making POST request to /logoff to log off the BI Platform
                WebRequest myWebRequest2 = WebRequest.Create(LogOffURI);
                myWebRequest2.ContentType = "application/xml";
                myWebRequest2.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
                myWebRequest2.Method = "POST";

                //Checking for the response
                WebResponse myWebResponse2 = myWebRequest2.GetResponse();
                StreamReader sr1 = new StreamReader(myWebResponse2.GetResponseStream());
                string output1 = sr1.ReadToEnd();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in logging off from Business Objects", ex.Message);
            }
        }

        private void GetReportList()
        {

            try
            {
                LoginBO();
                string reportFolderCUID = GetReportFolderCUID();
                string folderId = GetIDfromCUID(reportFolderCUID);

                //WUIN-632 change: as it can list only 50 documents info max at a time, need a logic to increment the offset and limt values by 50 each time till all the documents are fetched
                //and get the one present in the folder                 
                int offset = 0;
                DataSet dsBOReports;
                DataTable dtBOReports;
                DataTable dtBOReportsFolderLevel;
                DataTable dtBOReportsInFolder = new DataTable();

                while (true)
                {
                    string InfoStoreURI = baseURL + "raylight/v1/documents?offset=" + offset + "&limit=50";
                    HttpWebRequest myWebRequestParam = (HttpWebRequest)WebRequest.Create(InfoStoreURI);
                    myWebRequestParam.ContentType = "application/xml";
                    myWebRequestParam.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
                    myWebRequestParam.Timeout = webReqTimeout;
                    myWebRequestParam.Method = "GET";
                    WebResponse myWebResponseParam = myWebRequestParam.GetResponse();
                    StreamReader srParam = new StreamReader(myWebResponseParam.GetResponseStream());
                    string outputParam = srParam.ReadToEnd();
                    XmlDocument docParam = new XmlDocument();
                    docParam.LoadXml(outputParam);

                    dsBOReports = new DataSet();
                    dsBOReports.ReadXml(new XmlTextReader(new StringReader(outputParam)));
                    dtBOReports = dsBOReports.Tables[0];

                    DataRow[] drBOReportsFolderLevel = dtBOReports.Select("folderId='" + folderId + "'");
                    if (drBOReportsFolderLevel.Count() > 0)
                    {
                        dtBOReportsFolderLevel = dtBOReports.Select("folderId='" + folderId + "'").CopyToDataTable();
                    }
                    else
                    {
                        dtBOReportsFolderLevel = new DataTable();
                    }

                    //add the reports in folder to the final list to be displayed on screen
                    if (offset == 0)
                    {
                        //clone only for the first time
                        dtBOReportsInFolder = dtBOReportsFolderLevel.Clone();
                    }

                    foreach (DataRow dr in dtBOReportsFolderLevel.Rows)
                    {
                        dtBOReportsInFolder.ImportRow(dr);
                    }

                    if (dtBOReports.Rows.Count < 50)
                    {
                        break;
                    }
                    else
                    {
                        //Exception handling if this is the end of document list and there are no other documents
                        //check if there are other documents or this is the last one in the list 
                        //this is needed as if this is the end of document list and if tried to query from next offset value, exception is thrown
                        //need to handle this exception

                        try
                        {
                            string InfoStoreURIExp = baseURL + "raylight/v1/documents?offset=" + (offset + 50) + "&limit=50";
                            HttpWebRequest myWebRequestParamExp = (HttpWebRequest)WebRequest.Create(InfoStoreURIExp);
                            myWebRequestParamExp.ContentType = "application/xml";
                            myWebRequestParamExp.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
                            myWebRequestParamExp.Timeout = webReqTimeout;
                            myWebRequestParamExp.Method = "GET";
                            WebResponse myWebResponseParamExp = myWebRequestParamExp.GetResponse();
                        }
                        catch (Exception ex)
                        {
                            break;
                        }

                    }

                    //increment offset value to next possible value i.e is +50
                    offset = offset + 50;

                }

                if (dtBOReportsInFolder.Rows.Count > 0)
                {
                    Session["BOReportsData"] = dtBOReportsInFolder;
                    gvBOReports.DataSource = dtBOReportsInFolder;
                    gvBOReports.DataBind();
                }
                else
                {
                    DataTable dtEmpty = new DataTable();
                    gvBOReports.DataSource = dtEmpty;
                    gvBOReports.EmptyDataText = "No reports available in BO";
                    gvBOReports.DataBind();
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading report list", ex.Message);
            }
            finally
            {
                LogOffBO();
            }
        }

        private string GetIDfromCUID(string cuid)
        {
            string reportID = string.Empty;
            string InfoStoreURI = baseURL + "infostore/cuid_" + cuid;
            HttpWebRequest myWebRequestParam = (HttpWebRequest)WebRequest.Create(InfoStoreURI);
            myWebRequestParam.ContentType = "application/xml";
            myWebRequestParam.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
            myWebRequestParam.Timeout = webReqTimeout;
            myWebRequestParam.Method = "GET";
            WebResponse myWebResponseParam = myWebRequestParam.GetResponse();
            StreamReader srParam = new StreamReader(myWebResponseParam.GetResponseStream());
            string outputParam = srParam.ReadToEnd();
            XmlDocument docParam = new XmlDocument();
            docParam.LoadXml(outputParam);

            XmlNodeList nodelist = docParam.GetElementsByTagName("attr");
            foreach (XmlNode node in nodelist)
            {
                if (node.Attributes["name"].Value == "id")
                    reportID = node.InnerText;
            }

            return reportID;
        }

        private string GetLogonToken()
        {

            SessionMgr cdSessionManager = new SessionMgr();
            EnterpriseSession cdSession;
            string cdToken = string.Empty;

            // CMS Credential Variables  
            String cmsUser = Utilities.GetBOAccountUserId();
            String cmsPassword = Utilities.GetBOAccountUserPassoword();
            String cmsServer = ConfigurationManager.AppSettings["BOServerLogonTokenURL"].ToString();
            // CMS Configuration Variables  
            String cmsAuthType = "secEnterprise";

            // Create Logon Token for use with Open Document method  
            try
            {
                // logon to CMS & return the active session  
                cdSession = cdSessionManager.Logon(cmsUser, cmsPassword, cmsServer, cmsAuthType);
                // create the security token for this logon  
                // The createLogonToken method allows you to specify the machine that can use the token (which  
                // can be empty to allow any user to use the token), the number of minutes the token is valid for, and  
                // the number of logons that the token can be used for as parameters.  
                cdToken = cdSession.LogonTokenMgr.CreateLogonTokenEx("", 5, 1);
                cdSession.Logoff();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching logon token", ex.Message);
            }

            return cdToken;


        }

        private string GetReportFolderCUID()
        {
            string reportFolderCUID = string.Empty;
            int errorId;

            BOReportsBL BOrptBL = new BOReportsBL();
            BOrptBL.GetBOReportFolderCuid(BORptFolder, out reportFolderCUID, out errorId);
            BOrptBL = null;

            return reportFolderCUID;

        }


        #endregion METHODS




    }
}