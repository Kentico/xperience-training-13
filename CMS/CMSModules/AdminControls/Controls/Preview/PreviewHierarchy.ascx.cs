using System;

using CMS.Base;
using CMS.Helpers;

using System.Text;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Preview_PreviewHierarchy : CMSPreviewControl
{
    #region "Variables"

    UILayoutPane panePreview;
    UILayoutPane paneToolbar;
    int previewValue;
    bool registerScrollScript;
    private bool mEnablePreview = true;
    private UILayoutPane mPaneContent = null;
    private UILayoutPane mPaneContentMain = null;
    private UILayoutPane mPaneFooter = null;
    private UILayout mPaneLayout = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the key under which the preview state is stored in the cookies
    /// </summary>
    public String CookiesPreviewStateName
    {
        get;
        set;
    }


    /// <summary>
    /// Path for content (edit) control. Must be set, before page load.
    /// </summary>
    public String ContentControlPath
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether show panel separator (used in vertical mode)
    /// </summary>
    public bool ShowPanelSeparator
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether the preview control should display the "Save" and "Save and close" buttons.
    /// </summary>
    public bool DisplayFooter
    {
        get;
        set;
    }


    /// <summary>
    /// If true, empty EditedObject is allowed for preview
    /// </summary>
    public bool AllowEmptyObject
    {
        get;
        set;
    }


    /// <summary>
    /// If true, scroll position is stored between save requests
    /// </summary>
    public bool StorePreviewScrollPosition
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether use preview (hide preview button and don't split)
    /// </summary>
    public bool EnablePreview
    {
        get
        {
            return mEnablePreview;
        }
        set
        {
            mEnablePreview = false;
        }
    }


    /// <summary>
    /// Pane content
    /// </summary>
    public UILayoutPane PaneContent
    {
        get
        {
            return mPaneContent ?? (mPaneContent = PaneLayout.Panes.Find(x => x.ID.EqualsCSafe("panecontent", true)));
        }
    }


    /// <summary>
    /// Return UILayoutPane control.
    /// </summary>
    public UILayout PaneLayout
    {
        get
        {
            return mPaneLayout ?? (mPaneLayout = mainPane.FindControl("layoutElem") as UILayout);
        }
    }


    /// <summary>
    /// Pane content
    /// </summary>
    protected UILayoutPane PaneContentMain
    {
        get
        {
            return mPaneContentMain ?? (mPaneContentMain = PaneLayout.Panes.Find(x => x.ID.EqualsCSafe("panecontentmain", true)));
        }
    }


    /// <summary>
    /// Pane content
    /// </summary>
    protected UILayoutPane PaneFooter
    {
        get
        {
            return mPaneFooter ?? (mPaneFooter = PaneLayout.Panes.Find(x => x.ID.EqualsCSafe("panefooter", true)));
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        PaneContent.ControlPath = ContentControlPath;
        base.OnInit(e);
    }


    /// <summary>
    /// Adds content envelope ClientID. Must be added after page Init event to calculate right ClientID but before OnLoad event.
    /// </summary>
    public void RegisterEnvelopeClientID()
    {
        PaneContent.Values.Add(new UILayoutValue("ParentClientID", PaneContent.ClientID));
    }


    protected override void OnLoad(EventArgs e)
    {
        // This loads the user control itself. User control then sets the UIContext.EditedObject value.
        var previewControl = PaneContent.UserControl as CMSPreviewControl;

        ICMSMasterPage master = Page.Master as ICMSMasterPage;
        if ((master != null) && (master.FooterContainer != null))
        {
            master.FooterContainer.Visible = false;
        }

        ScriptHelper.RegisterScriptFile(Page, "Controls/CodePreview.js");

        paneToolbar = new UILayoutPane();

        PaneFooter.Visible = DisplayFooter;

        previewValue = GetPreviewStateFromCookies(CookiesPreviewStateName);
        if ((UIContext.EditedObject == null) && !AllowEmptyObject)
        {
            previewValue = 0;
        }

        // Change preview state handling
        string args = Request[Page.postEventArgumentID];
        string target = Request[Page.postEventSourceID];

        if (target == btnHidden.UniqueID)
        {
            switch (args)
            {
                case "vertical":
                    previewValue = 1;
                    break;

                case "horizontal":
                    previewValue = 2;
                    break;

                case "split":
                    previewValue = (previewValue == 0) ? 1 : 0;
                    if (previewValue != 0)
                    {
                        RegisterFullScreen();
                        PreviewInitialized = true;
                        paneToolbar.Values.Add(new UILayoutValue("PreviewInitialized", true));
                    }
                    break;
            }

            SetPreviewStateToCookies(CookiesPreviewStateName, previewValue);
            paneToolbar.Values.Add(new UILayoutValue("SetControls", true));
        }

        if (!EnablePreview)
        {
            previewValue = 0;
        }

        UILayout subLayout = PaneContentMain.FindControl("layoutElem") as UILayout;
        panePreview = subLayout.FindControl("panePreview") as UILayoutPane;

        PaneContentMain.Visible = true;
        subLayout.StopProcessing = false;

        PaneFooter.SpacingOpen = 0;

        // Check if inner control denied displaying preview
        if ((previewControl != null) && !previewControl.ShowPreview)
        {
            previewValue = 0;
        }

        switch (previewValue)
        {
            // No split
            case 0:
                {
                    subLayout.StopProcessing = true;
                    PaneContentMain.Visible = false;
                    PaneContent.Direction = PaneDirectionEnum.Center;
                }
                break;

            // Vertical
            case 1:
                {
                    PaneContent.SpacingOpen = 8;
                    PaneContent.SpacingClosed = 8;
                    PaneContent.ResizerClass = "TransformationVerticalResizer";
                    paneToolbar.PaneClass = "VerticalToolbar";
                    paneToolbar.Values.Add(new UILayoutValue("ShowPanelSeparator", ShowPanelSeparator));
                    paneToolbar.SpacingOpen = 0;

                    PaneLayout.Controls.Add(paneToolbar);
                    PaneLayout.Panes.Add(paneToolbar);
                    PaneContent.Direction = PaneDirectionEnum.West;
                }
                break;

            // Horizontal
            case 2:
                {
                    paneToolbar.PaneClass = "HorizontalToolbar";

                    subLayout.Controls.Add(paneToolbar);
                    subLayout.Panes.Add(paneToolbar);
                }
                break;
        }

        // Pane toolbar
        paneToolbar.ID = "paneToolbar";
        paneToolbar.Direction = PaneDirectionEnum.North;
        paneToolbar.ControlPath = "~/CMSModules/AdminControls/Controls/Preview/PreviewNavigationButtons.ascx";
        paneToolbar.Resizable = false;
        paneToolbar.Slidable = false;
        paneToolbar.RenderAs = HtmlTextWriterTag.Div;
        paneToolbar.SpacingOpen = 0;

        paneTitle.Visible = DisplayTitlePane;

        paneToolbar.Values.Add(new UILayoutValue("PreviewURLSuffix", PreviewURLSuffix));
        paneToolbar.Values.Add(new UILayoutValue("CookiesPreviewStateName", CookiesPreviewStateName));
        paneToolbar.Values.Add(new UILayoutValue("PreviewObjectName", PreviewObjectName));
        paneToolbar.Values.Add(new UILayoutValue("DefaultAliasPath", DefaultAliasPath));
        paneToolbar.Values.Add(new UILayoutValue("DialogMode", DialogMode));
        paneToolbar.Values.Add(new UILayoutValue("IgnoreSessionValues", IgnoreSessionValues));
        paneToolbar.Values.Add(new UILayoutValue("DefaultPreviewPath", DefaultPreviewPath));
        
        if (PaneContent.UserControl != null)
        {
            PaneContent.UserControl.SetValue("DialogMode", DialogMode);
        }


        if (previewValue != 0)
        {
            ScriptHelper.HideVerticalTabs(Page);

            if (!RequestHelper.IsPostBack())
            {
                RegisterFullScreen();
            }
        }

        base.OnLoad(e);
    }


    /// <summary>
    /// Registers javascript for layout communication
    /// </summary>
    private void RegisterHierarchyScripts()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@"
function refreshPreview() {
    var preview = document.getElementById('", panePreview.ClientID, @"');
    
    var state = ", previewValue, @";
    if (preview != null) {
        var loc = preview.src;
        preview.src = preventCacheLocation(preview.src, preview.src);
    }
                                                
    if (state != 0) {                                                                                                     
        showProgressBar(preview);
    }
} 

function showProgressBar(preview) {

    if (window.Loader) {    
        window.Loader.show();
    }
   
    var jpreview = $cmsj(preview); 
    $cmsj(jpreview).load(function() 
    {
        if (window.Loader) {  
            window.Loader.hide();
        }

        // Prevent multiple load events
        $cmsj(jpreview).unbind('load');
    });
}

function refreshPreviewParam(src) {  
    var preview = document.getElementById('", panePreview.ClientID, @"');
    var state = ", previewValue, @";
    if (preview != null) {
        preview.src = preventCacheLocation(src, preview.src);
    }
    if (state != 0) { 
        showProgressBar(preview, 0, 0);
    }
}

function performToolbarAction(action) {
    ", Page.ClientScript.GetPostBackEventReference(btnHidden, "#").Replace("'#'", "action"), @" 
}

function preventCacheLocation(src,contentSrc) { 
        var newLocation = src;
        if (src == contentSrc) {                                                            
            if (newLocation.indexOf('refreshtoken=0') != -1) {
                newLocation = newLocation.replace('refreshtoken=0','refreshtoken=1');
            } else {
                newLocation = newLocation.replace('refreshtoken=1','refreshtoken=0');
            }
        }
        return newLocation;
}
");

        ScriptHelper.RegisterLoader(Page);
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "RefreshWindow", sb.ToString(), true);

    }


    protected override void OnPreRender(EventArgs e)
    {
        String url = GetPreviewURL();

        // Register scroll script only for same domains to prevent access denied js errors
        registerScrollScript = (StorePreviewScrollPosition && (URLHelper.GetDomain(url) == RequestContext.CurrentDomain));
        RegisterHierarchyScripts();

        panePreview.Src = url;

        base.OnPreRender(e);
    }


    /// <summary>
    /// Add parameter to content parameter collection
    /// </summary>
    /// <param name="val">Content parameter</param>
    public void AddContentParameter(UILayoutValue val)
    {
        PaneContent.Values.Add(val);
    }


    /// <summary>
    /// Register script for window fullscreen in modal dialog
    /// </summary>
    private void RegisterFullScreen()
    {
        if (DialogMode)
        {
            const string script = @"
$cmsj(document).ready(function() {

    if (GetTop) 
    {
        var topFrame = GetTop();
        if(typeof topFrame.setToFullScreen !== 'undefined')
        {
            topFrame.setToFullScreen();
            $cmsj(window).trigger('resize');
        }
        else
        {
            $cmsj(topFrame).load(function(){ topFrame.setToFullScreen(); $cmsj(window).trigger('resize'); });
        }
    }
})";

            ScriptHelper.RegisterStartupScript(Page, typeof(String), "FullScreenScript", ScriptHelper.GetScript(script));
        }
    }

    #endregion
}
