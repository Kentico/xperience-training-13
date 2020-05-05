using System;
using System.Collections.Generic;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;

using System.Text;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DeviceProfiles;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.UniMenuConfig;
using CMS.URLRewritingEngine;


public partial class CMSModules_PortalEngine_Controls_OnSiteEdit_EditToolbar : CMSAbstractPortalUserControl, IPostBackEventHandler, ICallbackEventHandler
{
    #region "Constants"

    private const string MODULE_NAME = "cms.onsiteedit";
    private const string ELEMENT_NAME = "cmsonsiteedit";

    #endregion


    #region "Variables"

    private MessagesPlaceHolder mMessagePlaceholder;

    private readonly string preferredCultureCode = LocalizationContext.PreferredCultureCode;

    private bool isRTL;
    private bool is404;
    private string callbackResult = string.Empty;

    private const string modalDialogScript = "modalDialog({0}, '{1}', '90%', '90%');";

    private bool isRootDocument;

    private string checkChanges = " if (!CheckChanges()) { return false; } ";
    private int mNodeId = -1;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether toolbar is displayed for page not found mode
    /// </summary>
    protected bool IsPageNotFound
    {
        get;
        set;
    }


    /// <summary>
    /// Document's nodeId
    /// </summary>
    private int NodeId
    {
        get
        {
            if ((mNodeId == -1) && (DocumentContext.CurrentDocument != null))
            {
                mNodeId = DocumentContext.CurrentDocument.NodeID;
            }

            return mNodeId;
        }
    }


    /// <summary>
    /// Gets the current view mode
    /// </summary>
    protected ViewModeEnum ViewMode
    {
        get
        {
            return PortalContext.ViewMode;
        }
    }


    /// <summary>
    /// Gets the messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            if (mMessagePlaceholder == null)
            {
                mMessagePlaceholder = new MessagesPlaceHolder();
                mMessagePlaceholder.UseRelativePlaceHolder = false;
                mMessagePlaceholder.OffsetY = 5;
                mMessagePlaceholder.OffsetX = 5;
                mMessagePlaceholder.ContainerCssClass = "OnSiteEdit";
                plcEdit.Controls.Add(mMessagePlaceholder);
            }
            return mMessagePlaceholder;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        ucUIToolbar.ModuleName = MODULE_NAME;
        ucUIToolbar.ElementName = ELEMENT_NAME;

        // Set the viewmode according the the URL param
        if (ViewMode.IsLiveSite() && QueryHelper.Contains("viewmode"))
        {
            ViewModeEnum queryStringViewMode = ViewModeCode.GetPageEnumFromString(QueryHelper.GetString("viewmode", "livesite"));
            if (queryStringViewMode.IsEditLive())
            {
                SetViewMode(queryStringViewMode);
            }
        }

        if (ViewMode.IsEditLive())
        {
            // Check if there is required a redirect to the specific document
            if (QueryHelper.Contains("onsitenodeid"))
            {
                var nodeId = QueryHelper.GetInteger("onsitenodeid", 0);
                var treeProvider = new TreeProvider();
                var node = treeProvider.SelectSingleNode(nodeId);
                var url = UrlResolver.ResolveUrl(DocumentURLProvider.GetUrl(node));
                URLHelper.Redirect(url);
            }
        }

        base.OnInit(e);
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Do not process control by default
        StopProcessing = true;

        // Keep frequent objects
        CurrentUserInfo cui = MembershipContext.AuthenticatedUser;
        PageInfo pi = DocumentContext.CurrentPageInfo;

        if (pi == null)
        {
            IsPageNotFound = true;
            pi = new PageInfo();
            checkChanges = string.Empty;
        }

        ucUIToolbar.StopProcessing = true;

        // Get main UI element
        var element = UIElementInfoProvider.GetUIElementInfo(MODULE_NAME, ELEMENT_NAME);
        if (element == null)
        {
            return;
        }

        // Check whether user is authorized to edit page
        if ((pi != null)
            && AuthenticationHelper.IsAuthenticated()
            && cui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor, SiteContext.CurrentSiteName)
            && ((IsPageNotFound && pi.NodeID == 0) || cui.IsAuthorizedPerTreeNode(pi.NodeID, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed)
            && CMSPage.CheckUIElementAccessHierarchical(element, redirectToAccessDenied: false))
        {
            // Enable processing
            StopProcessing = false;

            // Check whether the preferred culture is RTL
            isRTL = CultureHelper.IsUICultureRTL();

            // Add link to CSS file
            CssRegistration.RegisterCssLink(Page, "Design", "OnSiteEdit.css");
            CssRegistration.RegisterBootstrap(Page);

            // Filter UI element buttons
            ucUIToolbar.OnButtonFiltered += ucUIToolbar_OnButtonFiltered;
            ucUIToolbar.OnButtonCreated += ucUIToolbar_OnButtonCreated;
            ucUIToolbar.OnGroupsCreated += ucUIToolbar_OnGroupsCreated;
            ucUIToolbar.IsRTL = isRTL;

            // Register edit script file
            RegisterEditScripts(pi);

            if (ViewMode.IsEditLive())
            {
                popupHandler.Visible = true;
                IsLiveSite = false;
                MessagesPlaceHolder.IsLiveSite = false;
                MessagesPlaceHolder.Opacity = 100;

                // Keep content of editable web parts when saving the document changes
                if (!IsPageNotFound)
                {
                    PortalManager.PreserveContent = true;
                }

                // Display warning in the Safe mode
                if (PortalHelper.SafeMode)
                {
                    string safeModeText = GetString("onsiteedit.safemode") + "<br/><a href=\"" + RequestContext.RawURL.Replace("safemode=1", "safemode=0") + "\">" + GetString("general.close") + "</a> " + GetString("contentedit.safemode2");
                    string safeModeDescription = GetString("onsiteedit.safemode") + "<br/>" + GetString("general.seeeventlog");

                    // Display the warning message
                    ShowWarning(safeModeText, safeModeDescription, "");
                }

                ucUIToolbar.StopProcessing = false;
                
                pnlUpdateProgress.Visible = true;
            }
            // Mode menu on live site
            else if (ViewMode.IsLiveSite())
            {
                // Hide the edit panel, show only slider button
                pnlToolbarSpace.Visible = false;
                pnlToolbar.Visible = false;
                pnlSlider.Visible = true;

                icon.CssClass = "cms-icon-80 icon-edit";
                icon.ToolTip = GetString("onsitedit.editmode");

                lblSliderText.Text = GetString("onsiteedit.editmode");
                pnlButton.Attributes.Add("onclick", "OnSiteEdit_ChangeEditMode();");

                // Hide the OnSite edit button when displayed in administration
                pnlSlider.Style.Add("display", "none");
            }
        }
        // Hide control actions for unauthorized users
        else
        {
            plcEdit.Visible = false;
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (ViewMode.IsEditLive())
        {
            // Display workflow info message
            if (DocumentManager.Workflow != null)
            {
                string message = DocumentManager.GetDocumentInfo(true);
                if (!string.IsNullOrEmpty(message))
                {
                    ShowInformation(message);
                }
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Groups created event handler.
    /// </summary>
    protected void ucUIToolbar_OnGroupsCreated(object sender, List<Group> groups)
    {
        // Try handle culture menu for page not found caused by untranslated document
        if (IsPageNotFound)
        {
            PageInfo pi = new PageInfo();
            if ((pi != null) && (pi.NodeID != 0))
            {
                ReplaceCultureButton(groups);
            }
        }
        // Do not process groups for page not found
        else
        {
            // Show "Save" button only when the current page contains an editor widget zone.
            Group widgetsCategory = groups.Find(g => g.CssClass.Contains("OnSiteWidgetsCategory"));
            if ((widgetsCategory != null)
                && (PortalManager.PageTemplate != null) && (PortalManager.PageTemplate.TemplateInstance != null) && (PortalManager.PageTemplate.TemplateInstance.WebPartZones != null))
            {
                bool showSaveButton = false;
                foreach (var zoneInstance in PortalManager.PageTemplate.TemplateInstance.WebPartZones)
                {
                    if (zoneInstance.WidgetZoneType == WidgetZoneTypeEnum.Editor)
                    {
                        showSaveButton = true;
                        break;
                    }
                }

                if (!showSaveButton)
                {
                    // No editor widget zones found => remove Save button
                    groups.Remove(widgetsCategory);
                }
            }

            ReplaceCultureButton(groups);

            // Replace Device profile button
            Group deviceProfile = groups.Find(g => g.CssClass.Contains("OnSiteDeviceProfile"));
            if (deviceProfile != null)
            {
                // Hide device profile selector when there is only one device defined or device profile module is disabled or license is not sufficient
                if (!DeviceProfileInfoProvider.IsDeviceProfilesEnabled(SiteContext.CurrentSiteName) || !LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.DeviceProfiles))
                {
                    // Remove the device profile button
                    groups.Remove(deviceProfile);
                }
                else
                {
                    deviceProfile.ControlPath = "~/CMSModules/DeviceProfiles/Controls/ProfilesMenuControl.ascx";
                }
            }

            // Replace the Highlight and Hidden buttons
            Group otherGroup = groups.Find(g => g.CssClass.Contains("OnSiteOthers"));
            if (otherGroup != null)
            {
                otherGroup.ControlPath = "~/CMSAdminControls/UI/UniMenu/OnSiteEdit/OtherMenu.ascx";
            }

            Group adminsGroup = groups.Find(g => g.CssClass.Contains("OnSiteAdmins"));
            if (adminsGroup != null)
            {
                adminsGroup.CssClass += " BigCMSDeskButton";
            }
        }
    }

    /// <summary>
    /// Hides culture button for less than two site cultures
    /// </summary>
    private void ReplaceCultureButton(List<Group> groups)
    {
        // Replace Culture button
        Group culture = groups.Find(g => g.CssClass.Contains("OnSiteCultures"));
        if (culture != null)
        {
            // Hide culture selector when there is only one culture for the current site
            InfoDataSet<CultureInfo> sites = CultureSiteInfoProvider.GetSiteCultures(SiteContext.CurrentSiteName);
            if (sites.Tables[0].Rows.Count < 2)
            {
                // Remove the culture button
                groups.Remove(culture);
            }
            else
            {
                culture.ControlPath = "~/CMSAdminControls/UI/UniMenu/OnSiteEdit/CultureMenu.ascx";
            }
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// OnButtonCreated event handler.
    /// </summary>
    protected void ucUIToolbar_OnButtonCreated(object sender, UniMenuArgs e)
    {
        if ((e.UIElement == null) || (e.ButtonControl == null))
        {
            return;
        }

        switch (e.UIElement.ElementName.ToLowerCSafe())
        {
            case "onsitedelete":
                if (isRootDocument)
                {
                    DisableButton(e, GetString("onsitedit.deleteroot"));
                }
                break;

            case "onsitesave":
                if (e.ButtonControl != null)
                {
                    string saveEnabledTooltip = GetString("onsiteedit.saveenabledtooltip");
                    string saveDisabledTooltip = GetString("onsiteedit.savedisabledtooltip");

                    e.ButtonControl.CssClass = "BigButtonDisabled OESaveButton";
                    e.ButtonControl.Attributes.Add("data-enabledTooltip", saveEnabledTooltip);
                    e.ButtonControl.Attributes.Add("data-disabledTooltip", saveDisabledTooltip);
                    e.ButtonControl.ToolTip = saveDisabledTooltip;
                }
                break;

            case "onsiteclose":
            case "onsitesignout":
                {
                    string script = string.Empty;

                    // Show javascript confirmation when the document is not found or published
                    if (IsPageNotFound ||
                        ((DocumentContext.CurrentPageInfo != null) && !DocumentContext.CurrentPageInfo.IsPublished && URLRewriter.PageNotFoundForNonPublished(SiteContext.CurrentSiteName)))
                    {
                        script = "if (!confirm(" + ScriptHelper.GetLocalizedString("onsiteedit.signout404confirmation") + ")) { return false; } ";
                        is404 = true;
                    }

                    // Sign out postback script
                    string eventCode = (e.UIElement.ElementName.ToLowerCSafe() == "onsitesignout") ? "signout" : "changeviewmode";
                    script += ControlsHelper.GetPostBackEventReference(this, eventCode);
                    script = checkChanges + script;

                    e.ButtonControl.Attributes.Add("onclick", script);
                }
                break;

            case "onsitecmsdesk":
                {
                    string buttonScript = String.Format("self.location.href = '{0}?nodeid={1}&returnviewmode=editlive{2}';", ResolveUrl(AdministrationUrlHelper.GetAdministrationUrl()), NodeId, ApplicationUrlHelper.GetApplicationHash("cms.content", "content"));
                    buttonScript = checkChanges + buttonScript;
                    e.ButtonControl.Attributes.Add("onclick", buttonScript);
                }
                break;
        }
    }


    /// <summary>
    /// OnButtonFiltered event handler.
    /// </summary>
    protected bool ucUIToolbar_OnButtonFiltered(object sender, UniMenuArgs e)
    {
        if (e.UIElement == null)
        {
            return false;
        }

        // Process only basic elements in page not found mode
        if (IsPageNotFound)
        {
            switch (e.UIElement.ElementName.ToLowerCSafe())
            {
                case "onsitecmsdesk":
                case "onsitelist":
                case "onsiteclose":
                case "onsitesignout":
                    break;

                default:
                    return false;
            }
        }

        return true;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Registers script files for on-site editing
    /// </summary>
    /// <param name="pi">Current page info</param>
    private void RegisterEditScripts(PageInfo pi)
    {
        ScriptHelper.RegisterJQueryCookie(Page);
        StringBuilder script = new StringBuilder();

        // Edit toolbar scripts
        if (ViewMode.IsEditLive())
        {
            // Dialog scripts
            ScriptHelper.RegisterDialogScript(Page);

            // General url settings
            UIPageURLSettings settings = new UIPageURLSettings();
            settings.AllowSplitview = false;
            settings.NodeID = pi.NodeID;
            settings.Culture = pi.DocumentCulture;
            settings.AdditionalQuery = "dialog=1";

            // Edit document
            settings.Mode = "editform";
            settings.Action = null;
            string editUrl = DocumentUIHelper.GetDocumentPageUrl(settings);
            settings.Mode = string.Empty;

            // Toolbar - Edit button script
            editUrl = URLHelper.RemoveParameterFromUrl(editUrl, "mode");
            string scriptEdit = GetModalDialogScript(editUrl, "editpage");

            // Delete document
            settings.Action = "delete";
            string deleteUrl = DocumentUIHelper.GetDocumentPageUrl(settings);
            // Toolbar - Delete button script
            string scriptDelete = GetModalDialogScript(deleteUrl, "deletepage");
            // User contributions - Delete item script
            string scriptDeleteItem = GetModalDialogScript(AddItemUrlParameters(deleteUrl), "deletepage");

            // New document
            settings.Action = "new";
            settings.AdditionalQuery += "&reloadnewpage=1";
            string newUrl = DocumentUIHelper.GetDocumentPageUrl(settings);
            newUrl = AddNewItemUrlParameters(newUrl);
            // Include culture code
            newUrl = URLHelper.AddParameterToUrl(newUrl, "parentculture", preferredCultureCode);
            // Toolbar - New button script
            string scriptNew = GetModalDialogScript(newUrl, "newpage");

            const string CONTENT_CMSDESK_FOLDER = "~/CMSModules/Content/CMSDesk/";

            // Toolbar - Properties button script
            string scriptProperties = GetModalDialogScript(ResolveUrl(CONTENT_CMSDESK_FOLDER + "Properties/Properties_Frameset.aspx?mode=editlive&documentid=" + pi.DocumentID), "propertiespage");

            // Display items from current level by default
            int nodeId = pi.NodeParentID;
            // If current level is root display first level
            if (nodeId == 0)
            {
                isRootDocument = true;
                nodeId = pi.NodeID;
            }

            // In page not found mode display first level
            if (nodeId == 0)
            {
                TreeProvider tp = new TreeProvider();
                TreeNode tn = tp.SelectSingleNode(SiteContext.CurrentSiteName, "/", TreeProvider.ALL_CULTURES);
                if (tn != null)
                {
                    nodeId = tn.NodeID;
                }
            }

            // Listing
            string listItemUrl = ResolveUrl(CONTENT_CMSDESK_FOLDER + "View/listing.aspx?dialog=1&wopenernodeid=" + pi.NodeID + "&nodeid=##id##");
            string scriptListItem = GetModalDialogScript(listItemUrl.Replace("##id##", nodeId.ToString()), "listingpage");

            // New culture
            string newCultureUrl = ResolveUrl(CONTENT_CMSDESK_FOLDER + "New/NewCultureVersion.aspx?nodeid=##id##&culture=##cult##&dialog=1");

            script.Append(@"
                var OEIsRTL = ", (isRTL ? "true" : "false"), @";
                var OECurrentNodeId = ", (DocumentContext.CurrentPageInfo != null) ? DocumentContext.CurrentPageInfo.NodeID : 0, @";
                var OEIsMobile = ", (DeviceContext.CurrentDevice.IsMobile() ? "true" : "false"), @";
                var OEHdnPostbackValue = null;

                function NewDocument(parentId, classId, targetWindow)
                {
                    OEClearZIndex(OEActiveWebPart);

                    if (targetWindow == null) {
                        ", scriptNew, @";   
                    }
                    else {
                        targetWindow.location.href = '", newUrl, @"';
                    }
                }

                function EditDocument(nodeId, targetWindow)
                {
                    OEClearZIndex(OEActiveWebPart); 

                    // Edit item button in repeaters and datalists
                    var arg = 'editurl;' + nodeId;
                    ", Page.ClientScript.GetCallbackEventReference(this, "arg", "OECallbackHandler", null), @";
                   
                }

                function SaveDocumentWidgets() {
                    if (!CMSContentManager.contentModified()) {
                        return false;
                    }
                    if (window.CMSContentManager) { CMSContentManager.allowSubmit = true; }
                    ", ControlsHelper.GetPostBackEventReference(this, "savedocument"), @";
                }

                function OECallbackHandler(arg, context) {
                    ", GetModalDialogScript("arg", "editpage", false), @"
                }

                function DeleteDocument(nodeId)
                {
                    OEClearZIndex(OEActiveWebPart); 
                    ", scriptDeleteItem, @" 
                }

                function OnSiteToolbarAction(name)
                {
                    ", checkChanges, @"

                    switch(name)
                    {
                        case 'edit':
                                ", scriptEdit, @"
                            break;

                        case 'properties':
                                ", scriptProperties, @"
                            break;

                        case 'new':
                                var parentId = OECurrentNodeId;
                                var classId = 0;
                                ", scriptNew, @"
                            break;

                        case 'delete':
                                ", scriptDelete, @"
                            break;

                        case 'list':
                                ", scriptListItem, @"
                            break;

                        default:
                            alert('Required action is not implemented.');
                    }
                }

                function SelectNode(nodeId, parentNodeId)
                {
                    var liu = '", listItemUrl, @"';
                    return liu.replace(/##id##/g, nodeId);
                }

                function NewDocumentCulture(nodeId, culture)
                {
                    ", checkChanges, @"

                    var liu = '", newCultureUrl, @"';
                    liu = liu.replace(/##id##/g, nodeId);
                    liu = liu.replace(/##cult##/g, culture);
                    ", GetModalDialogScript("liu", "newculture", false), @"
                }

                function OEEnsureHdnPostbackValue() {
                    if (OEHdnPostbackValue == null) {
                        OEHdnPostbackValue = document.getElementById('", hdnPostbackValue.ClientID, @"');
                    }
                }

                // Changes the device profile
                function ChangeDevice(device) {
                    ", checkChanges, @"
                    OEEnsureHdnPostbackValue();
                    OEHdnPostbackValue.value = device;
                    ", ControlsHelper.GetPostBackEventReference(this, "changedeviceprofile"), @"
                } ");

            ScriptHelper.RegisterJQueryDialog(Page);

            // Register OnSiteEdit script file
            ScriptHelper.RegisterScriptFile(Page, "DesignMode/OnSiteEdit.js");
            ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/jquery/jquery-url.js");
        }
        // Slider buttons scripts
        else if (ViewMode.IsLiveSite())
        {
            plcRfrWOpnrScript.Visible = false;

            script.Append(@"

                function OESlideSideToolbar() {
                    // Hide the slider button in administration
                    if (parent != this) {
                        return;
                    }

                    var toolbarEl = document.getElementById('", pnlSlider.ClientID, @"');
                    toolbarEl.style.display = ""block"";
                    // Show slider button
                    toolbarEl.style.top = ""0px"";
                }

                // Display slider button
                $cmsj(document).ready(function() { OESlideSideToolbar(); });
                ");
        }

        script.Append(" function OnSiteEdit_ChangeEditMode(){", ControlsHelper.GetPostBackEventReference(this, "changeviewmode"), "} ");

        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "OnSiteEditActions", ScriptHelper.GetScript(script.ToString()));
    }


    /// <summary>
    /// Disables the UniMenu button.
    /// </summary>
    /// <param name="e">The UniMenu item argument</param>
    /// <param name="disabledMessage">The message displayed in the tooltip when the user does not have sufficient permissions</param>
    private void DisableButton(UniMenuArgs e, string disabledMessage)
    {
        if (e.ImageControl != null)
        {
            // Replace the image with the disabled image (i.e: "new.png" -> "newDisabled.png")
            string imageUrl = e.ImageControl.ImageUrl;
            int extensionPosition = imageUrl.LastIndexOf('.');
            if (extensionPosition != -1)
            {
                imageUrl = imageUrl.Substring(0, extensionPosition) + "Disabled" + imageUrl.Substring(extensionPosition);
                e.ImageControl.ImageUrl = imageUrl;
            }
        }

        if (e.ButtonControl != null)
        {
            // Disable the menu button
            e.ButtonControl.Attributes.Remove("onclick");
            e.ButtonControl.CssClass = "BigButtonDisabled";
            e.ButtonControl.ToolTip = disabledMessage;
        }
    }


    /// <summary>
    /// Signs out the current user.
    /// </summary>
    private void SignOut()
    {
        string signOutUrl = RequestContext.CurrentURL;

        // Redirect to the root document when page not found
        if (is404)
        {
            signOutUrl = ResolveUrl("~/");
        }

        // LiveID sign out URL is set if this LiveID session (otherwise the CurrentURL is used)
        AuthenticationHelper.SignOut(ref signOutUrl);

        var sanitizedSignOutUrl = ScriptHelper.GetString(signOutUrl, encapsulate: false);
        var signOutScript = ScriptHelper.GetScript("location.replace('" + sanitizedSignOutUrl + "');");
        ScriptHelper.RegisterStartupScript(this, typeof(string), "livesiteScript", signOutScript);
    }


    /// <summary>
    /// Sets the view mode.
    /// </summary>
    /// <param name="viewMode">The view mode</param>
    private void SetViewMode(ViewModeEnum viewMode)
    {
        if (PortalHelper.IsOnSiteEditingEnabled(SiteContext.CurrentSiteName) && AuthenticationHelper.IsAuthenticated())
        {
            // Remove the "viewmode" param from url and redirect
            var returnUrl = GetURLWithoutViewMode();

            PortalContext.ViewMode = viewMode;

            if (is404)
            {
                // Redirect to the root document when page not found
                returnUrl = ResolveUrl("~/");
            }

            // Redirect to the URL
            URLHelper.Redirect(UrlResolver.ResolveUrl(returnUrl));
        }
    }


    /// <summary>
    /// Removes query string parameter viewmode from currentURL.
    /// </summary>
    private static string GetURLWithoutViewMode()
    {
        return URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, "viewmode");
    }


    /// <summary>
    /// Adds the general dialog URL parameters (used in the user contribution actions)
    /// </summary>
    /// <param name="url">The URL to modify</param>
    private string AddItemUrlParameters(string url)
    {
        // Edit item
        url = URLHelper.RemoveQuery(url);
        url = URLHelper.AddParameterToUrl(url, "dialog", "1");
        url = URLHelper.AddParameterToUrl(url, "nodeid", "' + nodeId + '");

        return url;
    }


    /// <summary>
    /// Adds the new item dialog URL parameters (used in the user contribution actions)
    /// </summary>
    /// <param name="actionUrl">The URL to modify</param>
    private string AddNewItemUrlParameters(string actionUrl)
    {
        actionUrl = URLHelper.RemoveQuery(actionUrl);
        actionUrl = URLHelper.AddParameterToUrl(actionUrl, "action", "new");
        actionUrl = URLHelper.AddParameterToUrl(actionUrl, "parentnodeid", "' + parentId + '");
        actionUrl = URLHelper.AddParameterToUrl(actionUrl, "classid", "' + classId + '");
        actionUrl = URLHelper.AddParameterToUrl(actionUrl, "dialog", "1");

        return actionUrl;
    }


    /// <summary>
    /// Gets the modal dialog script.
    /// </summary>
    /// <param name="url">The URL of the modal dialog</param>
    /// <param name="dialogName">Name of the dialog</param>
    /// <param name="useQuotesForUrl">If set to <c>TRUE</c>, wrap the given url into the single quotes.
    /// When FALSE, do not wrap the url -> in this case it can be used for javascript variables holding url itself</param>
    private string GetModalDialogScript(string url, string dialogName, bool useQuotesForUrl = true)
    {
        if (useQuotesForUrl)
        {
            url = "'" + url + "'";
        }

        return String.Format(modalDialogScript, url, dialogName);
    }


    /// <summary>
    /// Returns localized string.
    /// </summary>
    /// <param name="stringName">String to localize</param>
    /// <param name="culture">Culture to be used for localization</param>
    public override string GetString(string stringName, string culture = null)
    {
        // Force UI culture for toolbar
        if (culture == null)
        {
            culture = MembershipContext.AuthenticatedUser.PreferredUICultureCode;
        }

        return base.GetString(stringName, culture);
    }

    #endregion


    #region IPostBackEventHandler Members

    /// <summary>
    /// Processes an event raised when a form is posted to the server.
    /// </summary>
    public void RaisePostBackEvent(string eventArgument)
    {
        eventArgument = eventArgument.ToLowerCSafe();

        switch (eventArgument)
        {
            case "changeviewmode":
                {
                    // Switch view mode
                    SetViewMode(PortalContext.ViewMode.IsEditLive() ? ViewModeEnum.LiveSite : ViewModeEnum.EditLive);
                }
                break;

            case "changedeviceprofile":
                {
                    // Set the device name
                    string deviceName = hdnPostbackValue.Value;
                    DeviceContext.CurrentDeviceProfileName = deviceName;

                    // Refresh the document
                    URLHelper.Redirect(RequestContext.RawURL);
                }
                break;

            case "signout":
                SignOut();
                break;

            case "savedocument":
                if (PortalContext.ViewMode.IsEditLive())
                {
                    // Save editor widgets
                    DocumentManager.SaveDocument();
                }
                break;
        }
    }

    #endregion


    #region "Callback handling"

    /// <summary>
    /// Raises the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        string[] argumentParts = eventArgument.Split(';');

        if (argumentParts.Length > 0)
        {
            string action = argumentParts[0];

            switch (action)
            {
                // Get edit page url (also consider custom document type urls)
                case "editurl":
                    {
                        int nodeId = ValidationHelper.GetInteger(argumentParts[1], 0);

                        // Prepare url retriever settings
                        UIPageURLSettings settings = new UIPageURLSettings();
                        settings.AllowSplitview = false;
                        settings.NodeID = nodeId;
                        settings.AdditionalQuery = "dialog=1";
                        settings.Mode = "editform";
                        settings.Action = null;
                        settings.Culture = preferredCultureCode;

                        // Get edit page url
                        callbackResult = DocumentUIHelper.GetDocumentPageUrl(settings);
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Prepares the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        return callbackResult;
    }

    #endregion
}
