using System;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[assembly: RegisterCustomClass("PreviewTabsControlExtender", typeof(PreviewTabsControlExtender))]

/// <summary>
/// Preview tabs control extender
/// </summary>
public class PreviewTabsControlExtender : UITabsExtender
{
    /// <summary>
    /// Initializes the extender
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();

        Control.Page.Load += OnLoad;
    }


    public override void OnInitTabs()
    {
        Control.OnTabCreated += OnTabCreated;
    }


    private void OnLoad(object sender, EventArgs eventArgs)
    {
        var page = (CMSPage)Control.Page;

        // Setup the document manager
        var manager = page.DocumentManager;

        manager.RedirectForNonExistingDocument = false;

        ScriptHelper.RegisterScriptFile(Control.Page, "~/CMSModules/Content/CMSDesk/View/ViewTabs.js");
        
        var node = manager.Node;
        if (node != null)
        {
            DocumentUIHelper.EnsureDocumentBreadcrumbs(page.PageBreadcrumbs, node, null, null);
        }
    }


    private void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        var element = e.UIElement;

        string elem = element.ElementName.ToLowerCSafe();

        switch (elem)
        {
            case "validation":
                // Show validate only when not disabled
                if (QueryHelper.GetBoolean("hidevalidate", false))
                {
                    e.Tab = null;
                }
                break;
        }
    }
}
