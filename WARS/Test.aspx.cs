using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Net;
using System.IO;

namespace WARS
{
    public partial class Test : System.Web.UI.Page
    {
        DataTable table = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
           //Testing GIT
        }

        



        protected void btnGB_Click(object sender, EventArgs e)
        {
            //string filePath = "C:\\Harish\\SubLic GB WARS Network - Copy.mdb";     
            string filePath = "C:\\Harish\\WARSUAT_StatementLog_20170215.txt";     
            
       
            string fileExtention = Path.GetExtension(filePath);
            WebClient client = new WebClient();
            Byte[] buffer = client.DownloadData(filePath);
            //Response.ContentType = ".mdb";

            Response.AddHeader("content-length", buffer.Length.ToString());
            Response.BinaryWrite(buffer);
        }

        protected void fileUploadComment_UploadComplete(object sender, AjaxControlToolkit.AjaxFileUploadEventArgs e)
        {

        }
    }
}