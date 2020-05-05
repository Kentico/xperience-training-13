using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("newsletters.newsletters")]
[Action(0, "Newsletter_List.NewItemCaption", "Newsletter_New.aspx")]
[UIElement(ModuleName.NEWSLETTER, "Newsletters")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_List : CMSNewsletterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup UniGrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.WhereCondition = "NewsletterSiteID=" + SiteContext.CurrentSiteID;
        UniGrid.ZeroRowsText = GetString("general.nodatafound");
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        UniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
    }


    private object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "type":
                var emailCommunication = (EmailCommunicationTypeEnum)parameter;
                var localizedEmailCommunication = emailCommunication.ToLocalizedString("emailcommunicationtype");
                return HTMLHelper.HTMLEncode(localizedEmailCommunication);
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                string url = UIContextHelper.GetElementUrl("cms.newsletter", "EditNewsletterProperties", false, actionArgument.ToInteger(0));
                URLHelper.Redirect(url);
                break;

            case "delete":
                var newsletter = NewsletterInfo.Provider.Get(ValidationHelper.GetInteger(actionArgument, 0));
                if (newsletter == null)
                {
                    RedirectToAccessDenied(GetString("general.invalidparameters"));
                }
                
                if (!newsletter.CheckPermissions(PermissionsEnum.Delete, CurrentSiteName, CurrentUser))
                {
                    RedirectToAccessDenied(newsletter.TypeInfo.ModuleName, "Configure");
                }

                // delete Newsletter object from database
                newsletter.Delete();
                
                break;
        }
    }
}