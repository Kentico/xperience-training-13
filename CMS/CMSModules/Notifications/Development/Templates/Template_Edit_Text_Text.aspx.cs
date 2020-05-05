using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.Notifications.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Notifications_Development_Templates_Template_Edit_Text_Text : CMSNotificationsPage
{
    #region "Variables"

    private int templateId;
    private int gatewayId;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Show info message if changes were saved
        if (QueryHelper.GetBoolean("saved", false))
        {
            // Show message
            ShowChangesSaved();
        }

        templateId = QueryHelper.GetInteger("templateid", 0);
        gatewayId = QueryHelper.GetInteger("gatewayid", 0);
        templateTextElem.TemplateID = templateId;
        templateTextElem.GatewayID = gatewayId;

        NotificationGatewayInfo gateway = NotificationGatewayInfoProvider.GetNotificationGatewayInfo(gatewayId);
        if (gateway != null)
        {
            CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
            CurrentMaster.HeaderActions.ActionsList.Add(new SaveAction());

            // Initializes page breadcrumbs
            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = GetString("notification.templatetexteditlist"),
                RedirectUrl = "~/CMSModules/Notifications/Development/Templates/Template_Edit_Text.aspx?templateId=" + templateId
            });

            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = gateway.GatewayDisplayName
            });

            // Macros help
            lnkMoreMacros.Text = GetString("notification.template.text.helplnk");
            lblHelpHeader.Text = GetString("notification.template.text.helpheader");
            DisplayHelperTable();
        }
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                templateTextElem.SaveData();
                URLHelper.Redirect(UrlResolver.ResolveUrl("Template_Edit_Text_Text.aspx?gatewayId=" + gatewayId + "&templateid=" + templateId + "&saved=1"));
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Fills table holding additional macros.
    /// </summary>
    /// <param name="tableColumns">Data for table columns</param>
    private void FillHelperTable(string[,] tableColumns)
    {
        for (int i = 0; i <= tableColumns.GetUpperBound(0); i++)
        {
            TableRow tRow = new TableRow();
            TableCell leftCell = new TableCell();
            leftCell.Wrap = false;

            TableCell rightCell = new TableCell();

            Label lblExample = new Label();
            Label lblExplanation = new Label();

            // Init labels
            lblExample.Text = tableColumns[i, 0];
            lblExplanation.Text = tableColumns[i, 1];

            // Add labels to the cells
            leftCell.Controls.Add(lblExample);
            rightCell.Controls.Add(lblExplanation);

            leftCell.Width = new Unit(250);

            // Add cells to the row
            tRow.Cells.Add(leftCell);
            tRow.Cells.Add(rightCell);

            // Add row to the table
            tblHelp.Rows.Add(tRow);
        }
    }


    /// <summary>
    /// Displays helper table with makro examples.
    /// </summary>
    private void DisplayHelperTable()
    {
        // 0 - left column (example), 1 - right column (explanation)
        string[,] tableColumns = new string[5,2];

        int i = 0;

        //transformation expression examples
        tableColumns[i, 0] = "{%notificationsubscription.SubscriptionID%}";
        tableColumns[i++, 1] = GetString("notification.template.macrohelp.Subscription");

        tableColumns[i, 0] = "{%notificationgateway.GatewayID%}";
        tableColumns[i++, 1] = GetString("notification.template.macrohelp.Gateway");

        tableColumns[i, 0] = "{%notificationuser.UserID%}";
        tableColumns[i++, 1] = GetString("notification.template.macrohelp.User");

        tableColumns[i, 0] = "{%notificationcustomdata.XXX%}";
        tableColumns[i++, 1] = GetString("notification.template.macrohelp.CustomData");

        tableColumns[i, 0] = "{%documentlink%}";
        tableColumns[i, 1] = GetString("notification.template.macrohelp.DocumentLink");

        FillHelperTable(tableColumns);
    }

    #endregion
}