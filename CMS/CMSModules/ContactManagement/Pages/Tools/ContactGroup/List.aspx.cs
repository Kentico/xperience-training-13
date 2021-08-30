using System;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.UIControls;


[Title("om.contactgroup.list")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "ContactGroups")]
public partial class CMSModules_ContactManagement_Pages_Tools_ContactGroup_List : CMSContactManagementPage
{
    private const string HOWTO_VIDEO_URL = "https://www.youtube.com/watch?v=h8bnBnAZB14&list=PL9RdJplq_ukaNEfpLp0YVJENKU2NAokbM&index=3";
    private const string HOWTO_VIDEO_LENGTH = "5:14";

    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        InitSmartTip();

        hdrActions.AddAction(new HeaderAction
        {
            Text = GetString("om.contactgroup.new"),
            RedirectUrl = ResolveUrl(UIContextHelper.GetElementUrl(ModuleName.CONTACTMANAGEMENT, "NewContactGroup") + "&displaytitle=false")
        });

        // Register script for UniMenu button selection
        AddMenuButtonSelectScript(this, "ContactGroups", null, "menu");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable actions for unauthorized users
        if (!AuthorizationHelper.AuthorizedModifyContact(false))
        {
            hdrActions.Enabled = false;
        }
    }

    #endregion

    private void InitSmartTip()
    {
        tipHowCGWorks.ExpandedHeader = GetString("contactgroup.listing.howto.title");
        tipHowCGWorks.Content = $@"
<div class=""smarttip-video"">
    <a href=""{HOWTO_VIDEO_URL}"" target=""_blank"">
        <img src=""{UIHelper.GetImageUrl("CMSModules/CMS_ContactManagement/cm_howto_video_thumbnail.png")}"" class=""smarttip-video-thumbnail"">
        <span class=""smarttip-video-title"">{GetString("contactgroup.listing.howto.content")}</span>
        <span class=""smarttip-video-length"">{HOWTO_VIDEO_LENGTH}</span>
    </a>
</div>";
    }
}