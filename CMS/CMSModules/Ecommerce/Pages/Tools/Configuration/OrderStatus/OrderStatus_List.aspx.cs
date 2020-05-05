using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("OrderStatus_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_OrderStatus_OrderStatus_List : CMSOrderStatusesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        var actions = CurrentMaster.HeaderActions;

        // New item action
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("OrderStatus_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("OrderStatus_Edit.aspx?siteId=" + SiteID)
        });

        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.GridView.AllowSorting = false;

        // Show information about usage of global objects when used on site
        HandleGlobalObjectInformation(gridElem.ObjectType);

        // Filter records by site
        gridElem.WhereCondition = InitSiteWhereCondition("StatusSiteID").ToString(true);

        gridElem.RememberStateByParam = GetGridRememberStateParam();
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName == "statusName")
        {
            var statusRow = parameter as DataRowView;
            if (statusRow == null)
            {
                return string.Empty;
            }

            // Create tag with status name and color
            var statusInfo = new OrderStatusInfo(statusRow.Row);
            return new Tag
            {
                Text = statusInfo.StatusDisplayName,
                Color = statusInfo.StatusColor
            };
        }

        return parameter;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Check if at least one enabled status is present
        DataSet ds = OrderStatusInfoProvider.GetOrderStatuses(ConfiguredSiteID, true);
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            ShowWarning(GetString("com.orderstatus.noenabledfound"));
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int id = actionArgument.ToInteger(0);

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect(UrlResolver.ResolveUrl("OrderStatus_Edit.aspx?orderstatusid=" + id + "&siteId=" + SiteID));
                break;
        }
    }
}
