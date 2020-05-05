using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("InternalStatus_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_InternalStatus_InternalStatus_List : CMSInternalStatusesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        var actions = CurrentMaster.HeaderActions;

        // New item action
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("InternalStatus_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("InternalStatus_Edit.aspx?siteId=" + SiteID)
        });

        gridElem.OnAction += gridElem_OnAction;

        // Show information about usage of global objects when used on site
        HandleGlobalObjectInformation(gridElem.ObjectType);

        // Filter records by site
        gridElem.WhereCondition = InitSiteWhereCondition("InternalStatusSiteID").ToString(true);

        gridElem.RememberStateByParam = GetGridRememberStateParam();
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int id = actionArgument.ToInteger(0);

        if (actionName == "edit")
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("InternalStatus_Edit.aspx?statusid=" + id + "&siteId=" + SiteID));
        }
    }
}
