using System;

using CMS.DeviceProfiles;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_DeviceProfiles_Controls_LayoutBinding : CMSUserControl
{

    #region "Public properties"

    /// <summary>
    /// Gets or sets the source layout.
    /// </summary>
    public LayoutInfo SourceLayout { get; set; }


    /// <summary>
    /// Gets or sets the device profile.
    /// </summary>
    public DeviceProfileInfo DeviceProfile { get; set; }


    /// <summary>
    /// Gets the target layout for the specified device profile and source layout.
    /// </summary>
    public LayoutInfo TargetLayout { get; private set; }

    #endregion


    #region "Page methods"

    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        EnsureChildControls();
        Initialize();
    }


    /// <summary>
    /// Initializes this layout binding control.
    /// </summary>
    private void Initialize()
    {
        // Initialize source layout controls
        ltrSourceLayoutIcon.Text = PortalHelper.GetIconHtml(SourceLayout.LayoutThumbnailGUID, ValidationHelper.GetString(SourceLayout.LayoutIconClass, PortalHelper.DefaultPageLayoutIconClass));
        SourceLayoutDisplayNameLabel.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(SourceLayout.LayoutDisplayName));

        // Initialize target layout controls
        TargetLayout = DeviceProfileLayoutInfoProvider.GetTargetLayoutInfo(DeviceProfile, SourceLayout);
        if (TargetLayout != null)
        {
            ltrTargetLayoutIcon.Text = PortalHelper.GetIconHtml(TargetLayout.LayoutThumbnailGUID, ValidationHelper.GetString(TargetLayout.LayoutIconClass, PortalHelper.DefaultPageLayoutIconClass));
            TargetLayoutDisplayNameLabel.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(TargetLayout.LayoutDisplayName));
        }
        else
        {
            ltrTargetLayoutIcon.Text = PortalHelper.GetIconHtml(Guid.Empty, "icon-question-circle empty-device-layout-mapping");
            TargetLayoutDisplayNameLabel.Text = HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.sethint"));
        }

        // Initialize script to open target layout selection dialog
        string baseUrl = URLHelper.ResolveUrl("~/CMSModules/DeviceProfiles/Pages/SelectLayout.aspx");
        string url = String.Format("{0}?deviceProfileId={1:D}&sourceLayoutId={2:D}&targetLayoutId={3:D}", baseUrl, DeviceProfile.ProfileID, SourceLayout.LayoutId, TargetLayout != null ? TargetLayout.LayoutId : 0);
        string script = String.Format("modalDialog('{0}', 'SelectLayout', '1000', '785', null)", URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url)));
        TargetLayoutItemControl.Attributes.Add("onclick", script);

        btnDelete.ToolTip = GetString("device_profile.layoutmapping.unset");
        btnDelete.OnClientClick = "$cmsj.Event(event).stopPropagation(); Client_UnsetTargetLayout({sourceLayoutId:" + SourceLayout.LayoutId.ToString("D") + "}); return false;";
    }

    #endregion

}