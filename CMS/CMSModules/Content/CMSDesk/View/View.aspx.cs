using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_View_View : CMSContentPage
{
    private const string COOKIE_POLICY_DETECTION_PATH = "/KenticoCookiePolicyCheck";
    private const string COOKIE_POLICY_DETECTION_COOKIE_NAME = "KenticoCookiePolicyTest";

    #region "Variables & Properties"

    private string viewPage;

    /// <summary>
    /// Overridden messages placeholder for correct positioning
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
        set
        {
            plcMess = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnPreInit
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        PortalContext.ViewMode = ViewModeEnum.Preview;
        DocumentManager.RedirectForNonExistingDocument = false;

        base.OnPreInit(e);
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");

        // Setup Edit menu
        bool preview = PortalContext.ViewMode.IsPreview();

        editMenu.ShowProperties = false;
        editMenu.ShowSpellCheck = true;
        editMenu.ShowSave = !preview;
        editMenu.ShowCheckOut = !preview;
        editMenu.ShowCheckIn = !preview;
        editMenu.ShowUndoCheckOut = !preview;
        editMenu.ShowApplyWorkflow = !preview;
        editMenu.NodeID = NodeID;
        editMenu.CultureCode = CultureCode;
        editMenu.UseSmallIcons = true;
        editMenu.IsLiveSite = false;

        viewPage = DocumentUIHelper.GetViewPageUrl();
        ucView.ViewPage = string.Empty;
        ucView.RotateDevice = ValidationHelper.GetBoolean(CookieHelper.GetValue(CookieName.CurrentDeviceProfileRotate), false);

        const string deviceRotateScript = @"
$cmsj(document).ready(function () {
    if (window.CMSDeviceProfile) {
        CMSDeviceProfile.OnRotationFunction = (function() {
            CMSView.InitializeFrame(CMSView.PreviewWidth, CMSView.PreviewHeight, !CMSView.Rotated);
            CMSView.DeviceWindowResize();
        });
    }
});";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "deviceRotateScript", deviceRotateScript, true);

        // Bind external buttons (i.e. Persona selector)
        var extensionTarget = editMenu as IExtensibleEditMenu;
        extensionTarget.InitializeExtenders("Content");

        // Preview link is not valid after going through workflow because DocumentWorkflowCycleGUID has changed
        if (Node != null)
        {
            DocumentManager.OnAfterAction += (obj, args) => viewPage = Node.GetPreviewLink(MembershipContext.AuthenticatedUser.UserName, embededInAdministration: true);
        }
    }


    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        ucView.ViewPage = "about:blank";

        // Modify frame 'src' attribute and add administration domain into it
        ScriptHelper.RegisterModule(this, "CMS.Builder/FrameSrcAttributeModifier", new
        {
            frameId = ucView.FrameID,
            frameSrc = viewPage,
            mixedContentMessage = GetString("builder.ui.mixedcontenterrormessage"),
            applicationPath = SystemContext.ApplicationPath
        });

        RegisterCookiePolicyDetection();
    }


    /// <summary>
    /// Registers client scripts that display a message in case when the browser prevents iframe pages from setting cookies. The message informs user 
    /// that the previewed page may not work properly as e.g. form submission requires CSRF token saved in cookie.
    /// </summary>
    private void RegisterCookiePolicyDetection()
    {
        var page = Node;
        if (page == null && SiteInfoProvider.CombineWithDefaultCulture(CurrentSiteName))
        {
            // Preview shows page in default culture if combine is turned on
            page = DocumentHelper.GetDocument(NodeID, CultureHelper.GetDefaultCultureCode(CurrentSiteName), null);
        }

        if (!(page?.HasUrl() ?? false))
        {
            return;
        }

        string cookieValue = Guid.NewGuid().ToString();
        string sitePresentationUrl = GetPresentationUrl(page);
        string sourceOrigin = GetOriginFromUrl(RequestContext.URL.ToString());
        string targetOrigin = GetOriginFromUrl(sitePresentationUrl);
        string iframeUrl = sitePresentationUrl + COOKIE_POLICY_DETECTION_PATH;
        iframeUrl += QueryHelper.BuildQueryWithHash("origin", sourceOrigin, "cookieName", COOKIE_POLICY_DETECTION_COOKIE_NAME, "cookieValue", cookieValue);

        ScriptHelper.RegisterRequireJs(this);

        var moduleParameters = new
        {
            iframeUrl,
            targetOrigin,
            cookieName = COOKIE_POLICY_DETECTION_COOKIE_NAME,
            cookieValue,
            message = GetString("content.ui.preview.thirdpartycookiesblocked")
        };

        ScriptHelper.RegisterModule(this, "CMS/CookiePolicyDetection", moduleParameters);
    }


    private string GetPresentationUrl(TreeNode page)
    {
        return DocumentURLProvider.GetPresentationUrl(page.NodeSiteID, page.DocumentCulture);
    }


    private string GetOriginFromUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.GetLeftPart(UriPartial.Authority) : String.Empty;
    }

    #endregion
}
