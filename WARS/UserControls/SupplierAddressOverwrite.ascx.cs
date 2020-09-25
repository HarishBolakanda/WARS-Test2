/*
File Name   :   SupplierAddressOverwrite.ascx.cs
Purpose     :   Used for overwriting payee address with supplier address - displayed as a pop up from roy contract payee supplier link screen

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     01-Sep-2017     Pratik(Infosys Limited)   Initial Creation
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
using System.Web.UI.HtmlControls;

namespace WARS.UserControls
{
    public partial class SupplierAddressOverwrite : System.Web.UI.UserControl
    {
        #region Global Declarations
        SupplierAddressOverwriteBL supplierAddressOverwriteBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {

            UserAuthorization();

        }

        protected void btnAddressOverwrite_Click(object sender, EventArgs e)
        {
            try
            {
                mpeSaveCancel.Show();
            }
            catch (Exception ex)
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in address overwrite - " + ex.Message);
                util = null;
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            try
            {
                supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
                DataSet updatedData = supplierAddressOverwriteBL.OverwritePayeeAddress(hdnSelectedPayee.Value, hdnRoyaltorId.Value, txtSuppName.Text, txtSupplier.Text.Trim(), txtSupllierSiteName.Text, txtSuppAdd1.Text, txtSuppAdd2.Text, txtSuppAdd3.Text, txtSuppCity.Text, txtSuppPostcode.Text, Convert.ToString(Session["UserCode"]), out errorId);
                supplierAddressOverwriteBL = null;

                if (updatedData.Tables.Count != 0 && errorId != 2)
                {
                    BindPayeeData(updatedData.Tables[0]);
                    BindSupplierData(updatedData.Tables[1]);
                    hdnIsAddressOverwritten.Value = "Y";
                }
                else
                {
                    util = new Utilities();
                    util.GenericExceptionHandler("Error in address overwrite.");
                    util = null;
                }

            }
            catch (Exception ex)
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in address overwrite - " + ex.Message);
                util = null;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            mpeSaveCancel.Hide();
        }

        #endregion Events

        #region Methods

        private void UserAuthorization()
        {

            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnAddressOverwrite.Enabled = false;
            }

        }

        public void LoadInitialData()
        {
            supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
            DataSet initialData = supplierAddressOverwriteBL.GetInitialData(out errorId);
            supplierAddressOverwriteBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                Session["SuppAddOverwriteSupplierList"] = initialData.Tables[0];
            }
            else
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in loading supplier list.");
                util = null;
            }
        }

        public void LoadData(string intPartyId, string royaltorId, string supplierNumber, string supplierSiteName)
        {
            try
            {
                supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
                DataSet initialData = supplierAddressOverwriteBL.GetPayeeSuppData(intPartyId, royaltorId, supplierNumber, supplierSiteName, out errorId);
                supplierAddressOverwriteBL = null;

                if (initialData.Tables.Count != 0 && errorId != 2)
                {
                    hdnRoyaltorId.Value = royaltorId;
                    BindPayeeData(initialData.Tables[0]);
                    BindSupplierData(initialData.Tables[1]);
                }
                else
                {
                    util = new Utilities();
                    util.GenericExceptionHandler("Error in loading data.");
                    util = null;
                }

            }
            catch (Exception ex)
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in getting initial data - " + ex.Message);
                util = null;
            }

        }

        private void BindPayeeData(DataTable dtPayeeDetails)
        {
            if (dtPayeeDetails.Rows.Count > 0)
            {
                hdnSelectedPayee.Value = dtPayeeDetails.Rows[0]["int_party_id"].ToString();
                txtPayee.Text = dtPayeeDetails.Rows[0]["int_party_name"].ToString();
                txtMismatchFlag.Text = dtPayeeDetails.Rows[0]["mismatch_address"].ToString();
                txtPayeeName.Text = dtPayeeDetails.Rows[0]["int_party_name"].ToString();
                txtPayeeAdd1.Text = dtPayeeDetails.Rows[0]["int_party_add1"].ToString();
                txtPayeeAdd2.Text = dtPayeeDetails.Rows[0]["int_party_add2"].ToString();
                txtPayeeAdd3.Text = dtPayeeDetails.Rows[0]["int_party_add3"].ToString();
                txtPayeeAdd4.Text = dtPayeeDetails.Rows[0]["int_party_add4"].ToString();
                txtPayeePostcode.Text = dtPayeeDetails.Rows[0]["int_party_postcode"].ToString();
            }


        }

        private void BindSupplierData(DataTable dtSupplierDetails)
        {
            if (dtSupplierDetails.Rows.Count > 0)
            {
                txtSupplier.Text = dtSupplierDetails.Rows[0]["supplier"].ToString();
                txtSupllierSiteName.Text = dtSupplierDetails.Rows[0]["supplier_site_name"].ToString();
                txtSuppName.Text = dtSupplierDetails.Rows[0]["supplier_name"].ToString();
                txtSuppAdd1.Text = dtSupplierDetails.Rows[0]["supplier_add1"].ToString();
                txtSuppAdd2.Text = dtSupplierDetails.Rows[0]["supplier_add2"].ToString();
                txtSuppAdd3.Text = dtSupplierDetails.Rows[0]["supplier_add3"].ToString();
                txtSuppCity.Text = dtSupplierDetails.Rows[0]["supplier_add4"].ToString();
                txtSuppPostcode.Text = dtSupplierDetails.Rows[0]["supplier_postcode"].ToString();
            }

            ComparePayeeSupplierDetails();
        }

        private void ComparePayeeSupplierDetails()
        {
            if (txtPayeeName.Text.Trim() != txtSuppName.Text.Trim())
            {
                txtPayeeName.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeName.CssClass = "textboxStyle";
            }

            if (txtPayeeAdd1.Text.Trim() != txtSuppAdd1.Text.Trim())
            {
                txtPayeeAdd1.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeAdd1.CssClass = "textboxStyle";
            }

            if (txtPayeeAdd2.Text.Trim() != txtSuppAdd2.Text.Trim())
            {
                txtPayeeAdd2.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeAdd2.CssClass = "textboxStyle";
            }

            if (txtPayeeAdd3.Text.Trim() != txtSuppAdd3.Text.Trim())
            {
                txtPayeeAdd3.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeAdd3.CssClass = "textboxStyle";
            }

            if (txtPayeeAdd4.Text.Trim() != txtSuppCity.Text.Trim())
            {
                txtPayeeAdd4.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeAdd4.CssClass = "textboxStyle";
            }

            if (txtPayeePostcode.Text.Trim() != txtSuppPostcode.Text.Trim())
            {
                txtPayeePostcode.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeePostcode.CssClass = "textboxStyle";
            }


        }

        #endregion Methods

    }
}