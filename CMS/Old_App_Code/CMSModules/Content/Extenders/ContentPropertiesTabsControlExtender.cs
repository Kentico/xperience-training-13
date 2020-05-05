using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.UIControls;

[assembly: RegisterCustomClass("ContentPropertiesTabsControlExtender", typeof(ContentPropertiesTabsControlExtender))]

/// <summary>
/// Content edit tabs control extender
/// </summary>
public class ContentPropertiesTabsControlExtender : UITabsExtender
{
    /// <summary>
    /// Document manager
    /// </summary>
    public ICMSDocumentManager DocumentManager
    {
        get;
        set;
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        var page = (CMSPage)Control.Page;

        // Setup the document manager
        DocumentManager = page.DocumentManager;

        ScriptHelper.RegisterScriptFile(Control.Page, "~/CMSModules/Content/CMSDesk/Properties/PropertiesTabs.js");

        Control.OnTabCreated += OnTabCreated;
    }


    protected void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        var tab = e.Tab;
        var element = e.UIElement;

        var manager = DocumentManager;
        var node = manager.Node;

        bool splitViewSupported = PortalContext.ViewMode != ViewModeEnum.EditLive;

        string elementName = element.ElementName.ToLowerCSafe();

        if (DocumentUIHelper.IsElementHiddenForNode(element, node))
        {
            e.Tab = null;
            return;
        }

        switch (elementName)
        {
            case "properties.languages":
                splitViewSupported = false;
                break;

            case "properties.security":
            case "properties.relateddocs":
            case "properties.linkeddocs":
                splitViewSupported = false;
                break;
        }

        // Ensure split view mode
        if (splitViewSupported && PortalUIHelper.DisplaySplitMode)
        {
            tab.RedirectUrl = DocumentUIHelper.GetSplitViewUrl(tab.RedirectUrl);
        }
    }
}
