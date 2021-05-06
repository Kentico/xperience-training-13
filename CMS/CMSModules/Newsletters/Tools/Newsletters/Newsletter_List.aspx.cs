using System;

using CMS.Base;
using CMS.Base.Web.UI;
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
    private const string HOWTO_VIDEO_URL = "https://youtu.be/ej3TSxDwcEw";
    private const string HOWTO_VIDEO_LENGTH = "5:09";


    protected void Page_Load(object sender, EventArgs e)
    {
        InitSmartTip();

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


    private void InitSmartTip()
    {
        tipHowEMWorks.ExpandedHeader = GetString("newsletter.listing.howto.title");
        tipHowEMWorks.Content = $@"
<div class=""smarttip-video"">
    <a href=""{HOWTO_VIDEO_URL}"" target=""_blank"">
        <img src=""{UIHelper.GetImageUrl("CMSModules/CMS_Newsletter/em_howto_video_thumbnail.png")}"" class=""smarttip-video-thumbnail"">
        <span class=""smarttip-video-title"">{GetString("newsletter.listing.howto.content")}</span>
        <span class=""smarttip-video-length"">{HOWTO_VIDEO_LENGTH}</span>
    </a>
</div>";
    }
}