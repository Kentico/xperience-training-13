using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("PublicStatus_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_PublicStatus_PublicStatus_List : CMSPublicStatusesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        var actions = CurrentMaster.HeaderActions;

        // New item action
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("PublicStatus_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("PublicStatus_Edit.aspx?siteId=" + SiteID)
        });

        gridElem.OnAction += gridElem_OnAction;

        // Show information about usage of global objects when used on site
        HandleGlobalObjectInformation(gridElem.ObjectType);

        gridElem.WhereCondition = InitSiteWhereCondition("PublicStatusSiteID").ToString(true);
        gridElem.RememberStateByParam = GetGridRememberStateParam();
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int statusId = ValidationHelper.GetInteger(actionArgument, 0);

        if (actionName == "edit")
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("PublicStatus_Edit.aspx?publicStatusId=" + statusId + "&siteId=" + SiteID));
        }
    }
}
