using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;

public partial class CMSModules_Content_FormControls_Documents_SelectPath : FormEngineUserControl, ICallbackEventHandler, IPostBackEventHandler
{
    #region "Variables & constants"

    private DialogConfiguration mConfig;
    private TreeProvider mTreeProvider;
    private string callbackResult = string.Empty;
    private int nodeIdFromPath;
    private const string separator = "##SEP##";

    private string selectedSiteName;
    private bool siteNameIsAll;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets the value that indicates whether control should use postback after selection
    /// </summary>
    public bool UpdateControlAfterSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UpdateControlAfterSelection"), false);
        }
        set
        {
            SetValue("UpdateControlAfterSelection", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether text input is enabled
    /// </summary>
    public bool DisableTextInput
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisableTextInput"), false);
        }
        set
        {
            SetValue("DisableTextInput", value);
        }
    }


    /// <summary>
    /// Returns TreeProvider object.
    /// </summary>
    private TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets the configuration for Copy and Move dialog.
    /// </summary>
    public DialogConfiguration Config
    {
        get
        {
            if (mConfig == null)
            {
                if (UseFieldInfoSettings || ((FieldInfo != null) && FieldInfo.Settings.Contains("Dialogs_Content_Hide")))
                {
                    // Load configuration from field settings
                    mConfig = GetDialogConfiguration();
                    mConfig.OutputFormat = OutputFormatEnum.URL;
                    mConfig.EditorClientID = txtPath.ClientID;

                    if (String.IsNullOrEmpty(mConfig.ContentSelectedSite))
                    {
                        mConfig.ContentSelectedSite = (String.IsNullOrEmpty(selectedSiteName)) ? SiteContext.CurrentSiteName : selectedSiteName;
                    }

                    SinglePathMode = false;
                }
                else
                {
                    // Set dialog configuration
                    mConfig = new DialogConfiguration();
                    mConfig.HideLibraries = true;
                    mConfig.ContentSelectedSite = SiteContext.CurrentSiteName;
                    mConfig.HideAnchor = true;
                    mConfig.HideAttachments = true;
                    mConfig.HideContent = false;
                    mConfig.HideEmail = true;
                    mConfig.HideLibraries = true;
                    mConfig.HideWeb = true;
                    mConfig.OutputFormat = OutputFormatEnum.Custom;
                    mConfig.CustomFormatCode = "selectpath";
                    mConfig.SelectableContent = SelectableContentEnum.AllContent;
                    mConfig.EditorClientID = SinglePathMode ? txtNodeId.ClientID : txtPath.ClientID;
                    mConfig.ContentUseRelativeUrl = UseRelativeUrl;

                    if (SubItemsNotByDefault)
                    {
                        mConfig.AdditionalQueryParameters = "SubItemsNotByDefault=1";
                    }
                }

                if (EnableSiteSelection.HasValue)
                {
                    mConfig.ContentSites = EnableSiteSelection.Value ? AvailableSitesEnum.All : AvailableSitesEnum.OnlyCurrentSite;
                }
                else if (ControlsHelper.CheckControlContext(this, ControlContext.WIDGET_PROPERTIES) && (!siteNameIsAll))
                {
                    // If used in a widget, site selection is provided by a site selector form control (using HasDependingField/DependsOnAnotherField principle)
                    // therefore the site selector drop-down list in the SelectPath dialog contains only a single site - preselected by the site selector form control
                    mConfig.ContentSites = (String.IsNullOrEmpty(selectedSiteName)) ? AvailableSitesEnum.OnlyCurrentSite : AvailableSitesEnum.OnlySingleSite;
                }
                else if (SiteID > 0)
                {
                    // Preselect site name if site identifier is selected
                    mConfig.ContentSites = AvailableSitesEnum.OnlySingleSite;
                    SiteInfo si = SiteInfo.Provider.Get(SiteID);
                    if (si != null)
                    {
                        mConfig.ContentSelectedSite = si.SiteName;
                    }
                }
                else if (AllowSetPermissions)
                {
                    // Use only current site for path selection since returned value alias path only.
                    mConfig.ContentSites = AvailableSitesEnum.OnlyCurrentSite;
                }
                else
                {
                    // Use all sites as default
                    mConfig.ContentSites = AvailableSitesEnum.All;
                }
            }

            return mConfig;
        }
    }


    /// <summary>
    /// Indicates whether returned URL should be relative.
    /// </summary>
    public bool UseRelativeUrl
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseRelativeUrl"), false);
        }
        set
        {
            SetValue("UseRelativeUrl", value);
        }
    }


    /// <summary>
    /// Gets the path text box.
    /// </summary>
    public CMSTextBox PathTextBox
    {
        get
        {
            EnsureChildControls();
            return txtPath;
        }
    }


    /// <summary>
    /// Gets the select path button.
    /// </summary>
    public CMSButton SelectButton
    {
        get
        {
            EnsureChildControls();
            return btnSelectPath;
        }
    }


    /// <summary>
    /// Indicates whether check box "Only sub items" is checked by default or not.
    /// </summary>
    public bool SubItemsNotByDefault
    {
        get;
        set;
    }


    public bool SinglePathMode
    {
        get
        {
            return GetValue("SinglePathMode", true);
        }
        set
        {
            SetValue("SinglePathMode", value);
        }
    }


    /// <summary>
    /// Determines whether to enable site selection or not.
    /// </summary>
    public bool? EnableSiteSelection
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            PathTextBox.Enabled = value;
            btnSelectPath.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return PathTextBox.Text;
        }
        set
        {
            PathTextBox.Text = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Gets selected node id.
    /// </summary>
    public int NodeId
    {
        get
        {
            return ValidationHelper.GetInteger(txtNodeId.Text, 0);
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox with path.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return PathTextBox.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the site from which the path is selected.
    /// </summary>
    public int SiteID
    {
        get
        {
            return GetValue("SiteID", 0);
        }
        set
        {
            SetValue("SiteID", value);
        }
    }


    /// <summary>
    /// Returns name of site.
    /// </summary>
    private string SiteName
    {
        get
        {
            if (String.IsNullOrEmpty(Config.ContentSelectedSite))
            {
                SiteInfo si = SiteInfo.Provider.Get(SiteID);
                if (si != null)
                {
                    return si.SiteName;
                }
                else
                {
                    return SiteContext.CurrentSiteName;
                }
            }

            return Config.ContentSelectedSite;
        }
    }


    /// <summary>
    /// Determines whether to allow setting permissions for selected path.
    /// </summary>
    public bool AllowSetPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowSetPermissions"), false);
        }
        set
        {
            SetValue("AllowSetPermissions", value);
        }
    }

    /// <summary>
    /// Indicates if dialog settings should be loaded from field info or not. 
    /// </summary>
    private bool UseFieldInfoSettings
    {
        get
        {
            return GetValue("UseFieldInfoSettings", false);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnPreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Hide hidden textbox for node ID
        txtNodeId.Style.Add(HtmlTextWriterStyle.Display, "none");

        if (RequestHelper.IsPostBack() && DependsOnAnotherField)
        {
            if (siteNameIsAll)
            {
                // Refresh the dialog script if the site name was changed to "ALL" (this enables the site selection in the dialog window)
                btnSelectPath.OnClientClick = GetDialogScript();
            }

            pnlUpdate.Update();
        }
        else
        {
            txtPath.Enabled = !DisableTextInput && txtPath.Enabled;
            if (UpdateControlAfterSelection)
            {
                txtNodeId.Attributes.Add("onchange", ControlsHelper.GetPostBackEventReference(this, "refresh"));
            }
            
            base.OnPreRender(e);
        }
    }


    /// <summary>
    /// Creates script for opening selection dialog.
    /// </summary>
    /// <returns></returns>
    private string GetDialogScript()
    {
        return "modalDialog('" + GetSelectionDialogUrl() + "','PathSelection', '90%', '85%'); return false;";
    }


    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        SetFormSiteName();

        // Register JavaScripts
        ScriptHelper.RegisterDialogScript(Page);
        btnSelectPath.OnClientClick = GetDialogScript();
        btnSelectPath.Text = GetString("general.select");
        btnSelectPath.ButtonStyle = ButtonStyle.Default;

        // Set max length
        txtPath.MaxLength = (FieldInfo != null) ? FieldInfo.Size : 200;

        btnSetPermissions.Text = GetString("selectsinglepath.setpermissions");
        btnSetPermissions.ButtonStyle = ButtonStyle.Default;

        txtNodeId.TextChanged += txtNodeId_TextChanged;

        RegisterScripts();

        // Set control visibility
        btnSetPermissions.Visible = AllowSetPermissions;
    }


    /// <summary>
    /// Registers necessary scripts.
    /// </summary>
    private void RegisterScripts()
    {
        if (AllowSetPermissions)
        {
            // Script for opening dialog, shows alert if document doesn't exist
            var urlScript = $@"
function PerformAction(content, context) {{
    var arr = content.split('{separator}');
    if(arr[0] == '0')
    {{
        alert('{GetString("content.documentnotexists")}');
    }}
    else
    {{
        modalDialog(arr[1], 'SetPermissions', '605', '800');
    }}
}}";

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "GetPermissionsUrl", urlScript, true);

            btnSetPermissions.OnClientClick = Page.ClientScript.GetCallbackEventReference(this, "document.getElementById('" + PathTextBox.ClientID + "').value", "PerformAction", "'SetPermissionContext'") + "; return false;";

            // Disable text box if there is no current document
            if (DocumentContext.CurrentDocument == null)
            {
                var textChanged = $@"
function TextChanged_{ClientID}() {{
    var textElem = document.getElementById('{PathTextBox.ClientID}');
    if ((textElem != null) && (textElem.value == null || textElem.value == ''))
    {{
        BTN_Disable('{btnSetPermissions.ClientID}');
    }}
    else
    {{
        BTN_Enable('{btnSetPermissions.ClientID}');
    }}
    setTimeout('TextChanged_{ClientID}()', 500);
}}
setTimeout('TextChanged_{ClientID}()', 500);";

                ScriptHelper.RegisterStartupScript(this, typeof(string), "TextChanged" + ClientID, textChanged, true);
            }
        }

        // Register script for changing control state
        var changeStatScript = $@"
function ChangeState_{ClientID}(state) {{
    {ControlsHelper.GetPostBackEventReference(this, "changestate|").Replace("'changestate|'", "'changestate|' + state")};
}}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ChangeState_" + ClientID, changeStatScript, true);
    }


    /// <summary>
    /// TextChanged event handler.
    /// </summary>
    private void txtNodeId_TextChanged(object sender, EventArgs e)
    {
        int nodeId = ValidationHelper.GetInteger(txtNodeId.Text, 0);
        if (nodeId <= 0)
        {
            return;
        }

        TreeNode node = TreeProvider.SelectSingleNode(nodeId);
        if (node == null)
        {
            return;
        }

        SiteID = node.NodeSiteID;
        PathTextBox.Text = node.NodeAliasPath;

        // Raise change event
        RaiseOnChanged();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns the value of the given property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        if (propertyName == "DialogConfiguration")
        {
            return Config;
        }

        return base.GetValue(propertyName);
    }


    /// <summary>
    /// Returns Correct URL of the path selection dialog.
    /// </summary>
    private string GetSelectionDialogUrl()
    {
        string url = CMSDialogHelper.GetDialogUrl(Config, false, null, false);

        url = URLHelper.RemoveParameterFromUrl(url, "hash");

        // Set single path mode
        if (SinglePathMode)
        {
            url = URLHelper.AddParameterToUrl(url, "selectionmode", "single");
        }

        // Recreate correct hash string to secure input
        string query = URLHelper.UrlEncodeQueryString(url);
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(query));

        return url;
    }


    /// <summary>
    /// Returns Correct URL of the 'Set permissions' dialog.
    /// </summary>
    private string GetPermissionsDialogUrl(string nodeAliasPath)
    {
        string url = ResolveUrl("~/CMSModules/Content/FormControls/Documents/ChangePermissions/ChangePermissions.aspx");
        // Use current document path if not set
        if (string.IsNullOrEmpty(nodeAliasPath) && (DocumentContext.CurrentDocument != null))
        {
            nodeAliasPath = DocumentContext.CurrentDocument.NodeAliasPath;
        }
        nodeIdFromPath = TreePathUtils.GetNodeIdByAliasPath(SiteName, MacroResolver.ResolveCurrentPath(nodeAliasPath));
        url = URLHelper.AddParameterToUrl(url, "nodeid", nodeIdFromPath.ToString());
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));
        return url;
    }

    
    /// <summary>
    /// Sets the site name if the SiteName field is available in the form.
    /// The outcome of this method is used for the configuration of the "Config" property
    /// </summary>
    private void SetFormSiteName()
    {
        if (DependsOnAnotherField && (Form != null) && Form.IsFieldAvailable("SiteName"))
        {
            string siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), "");

            if (string.IsNullOrEmpty(siteName) || siteName.Equals("##all##", StringComparison.OrdinalIgnoreCase))
            {
                selectedSiteName = string.Empty;
                siteNameIsAll = true;
                return;
            }

            if (!String.IsNullOrEmpty(siteName))
            {
                selectedSiteName = siteName;
                return;
            }
        }

        selectedSiteName = null;
    }

    #endregion


    #region "Callback handling"

    /// <summary>
    /// Raises the callback event.
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        callbackResult = GetPermissionsDialogUrl(eventArgument);
    }


    /// <summary>
    /// Returns the result of a callback.
    /// </summary>
    public string GetCallbackResult()
    {
        return nodeIdFromPath + separator + callbackResult;
    }

    #endregion


    #region IPostBackEventHandler Members

    /// <summary>
    /// Raises post back event.
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument.Equals("refresh", StringComparison.OrdinalIgnoreCase))
        {
            RaiseOnChanged();
        }
        else if (eventArgument.StartsWith("changestate", StringComparison.OrdinalIgnoreCase))
        {
            string[] pars = eventArgument.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (pars.Length == 2)
            {
                bool state = ValidationHelper.GetBoolean(pars[1], true);
                Enabled = state;
            }
        }
    }

    #endregion
}