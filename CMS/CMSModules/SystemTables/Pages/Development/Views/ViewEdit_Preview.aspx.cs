using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_SystemTables_Pages_Development_Views_ViewEdit_Preview : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHashString(QueryHelper.GetString("objname", null), QueryHelper.GetString("hash", null)))
        {
            ShowError(GetString("sysdev.views.corruptedparameters"));
            return;
        }

        // Ensure suffix
        UIHelper.SetBreadcrumbsSuffix(GetString("sysdev.view"));

        if (!RequestHelper.IsPostBack())
        {
            drpItems.Items.Add(new ListItem("25", "25"));
            drpItems.Items.Add(new ListItem("100", "100"));
            drpItems.Items.Add(new ListItem("1000", "1000"));
            drpItems.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        }

        string objName = QueryHelper.GetString("objname", null);

        // Check if edited view exists and redirect to error page if not
        TableManager tm = new TableManager(null);
        
        if (String.IsNullOrEmpty(objName) || !tm.ViewExists(objName))
        {
            EditedObject = null;
        }

        if (objName != null)
        {
            string top = "";

            int items = ValidationHelper.GetInteger(drpItems.SelectedValue, 25);
            if (items != -1)
            {
                top = "TOP " + items;
            }

#pragma warning disable BH2501 // Do not use ExecuteQuery in UI.
            DataSet ds = ConnectionHelper.ExecuteQuery("SELECT " + top + " * FROM " + objName, null, QueryTypeEnum.SQLQuery);
#pragma warning restore BH2501 // Do not use ExecuteQuery in UI.

            if (DataHelper.DataSourceIsEmpty(ds))
            {
                lblNoDataFound.Visible = true;
            }
            else
            {
                grdData.DataSource = ds;
                grdData.DataBind();
            }
        }
    }
}
