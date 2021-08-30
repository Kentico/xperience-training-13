using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.UIControls;

[UIElement(ModuleName.CONTACTMANAGEMENT, "Contacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_List : CMSContactManagementPage
{
    private const string HOWTO_VIDEO_URL = "https://www.youtube.com/watch?v=h8bnBnAZB14&list=PL9RdJplq_ukaNEfpLp0YVJENKU2NAokbM&index=3";
    private const string HOWTO_VIDEO_LENGTH = "5:14";

    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        InitSmartTip();

        // Set header actions (add button)
        string url = ResolveUrl("New.aspx");

        hdrActions.AddAction(new HeaderAction
        {
            Text = GetString("om.contact.importcsv"),
            RedirectUrl = ResolveUrl("ImportCSV.aspx"),
            ButtonStyle = ButtonStyle.Primary
        });

        hdrActions.AddAction(new HeaderAction
            {
                Text = GetString("om.contact.new"),
                RedirectUrl = url,
                ButtonStyle = ButtonStyle.Default
        });

        var deletingInactiveContactsMethod = SettingsKeyInfoProvider.GetValue("CMSDeleteInactiveContactsMethod");
        if (string.IsNullOrEmpty(deletingInactiveContactsMethod) && LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.FullContactManagement))
        {
            ShowWarning(string.Format(GetString("om.contactlist.inactivecontacts.warning"), DocumentationHelper.GetDocumentationTopicUrl("contacts_automatic_deletion")));               
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable actions for unauthorized users
        hdrActions.Enabled = AuthorizationHelper.AuthorizedModifyContact(false);
    }

    #endregion

    private void InitSmartTip()
    {
        tipHowCMWorks.ExpandedHeader = GetString("contactmanagement.listing.howto.title");
        tipHowCMWorks.Content = $@"
<div class=""smarttip-video"">
    <a href=""{HOWTO_VIDEO_URL}"" target=""_blank"">
        <img src=""{UIHelper.GetImageUrl("CMSModules/CMS_ContactManagement/cm_howto_video_thumbnail.png")}"" class=""smarttip-video-thumbnail"">
        <span class=""smarttip-video-title"">{GetString("contactmanagement.listing.howto.content")}</span>
        <span class=""smarttip-video-length"">{HOWTO_VIDEO_LENGTH}</span>
    </a>
</div>";
    }
}