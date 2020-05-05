using System;

using CMS;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

[assembly: RegisterCustomClass("ContentEditTabsControlExtender", typeof(ContentEditTabsControlExtender))]

/// <summary>
/// Content edit tabs control extender
/// </summary>
public class ContentEditTabsControlExtender : UITabsExtender
{
    private bool showProductTab;


    /// <summary>
    /// Gets current document.
    /// </summary>
    public TreeNode Node
    {
        get;
        set;
    }


    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();

        CMSContentPage.CheckSecurity();

        Control.Page.Load += OnLoad;
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        Control.OnTabCreated += OnTabCreated;
    }


    /// <summary>
    /// Redirects to new document language version page.
    /// </summary>
    protected virtual void RedirectToNewCultureVersionPage()
    {
        URLHelper.Redirect(DocumentUIHelper.GetNewCultureVersionPageUrl());
    }


    /// <summary>
    /// Fires when the page loads
    /// </summary>
    private void OnLoad(object sender, EventArgs eventArgs)
    {
        var page = (CMSPage)Control.Page;

        var manager = page.DocumentManager;

        manager.RedirectForNonExistingDocument = false;
        manager.Tree.CombineWithDefaultCulture = false;

        var node = manager.Node;

        if (node != null)
        {
            Node = node;

            ScriptHelper.RegisterScriptFile(Control.Page, "~/CMSModules/Content/CMSDesk/EditTabs.js");

            // Document from different site
            if (node.NodeSiteID != SiteContext.CurrentSiteID)
            {
                URLHelper.Redirect(DocumentUIHelper.GetPageNotAvailable(string.Empty, false, node.DocumentName));
            }

            showProductTab = node.HasSKU;

            DocumentUIHelper.EnsureDocumentBreadcrumbs(page.PageBreadcrumbs, node, null, null);
        }
        else
        {
            // Document does not exist -> redirect to new culture version creation dialog
            RedirectToNewCultureVersionPage();
        }
    }


    /// <summary>
    /// Fires when a tab is created
    /// </summary>
    private void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        string mode = null;

        var element = e.UIElement;

        // Hides UI element if it is not relevant for edited node
        if (DocumentUIHelper.IsElementHiddenForNode(element, Node))
        {
            e.Tab = null;
            return;
        }

        switch (element.ElementName.ToLowerInvariant())
        {
            case "page":
                {
                    mode = "edit";
                }
                break;

            case "editform":
                // Keep edit form
                {
                    mode = "editform";
                }
                break;

            case "product":
                {
                    if (!showProductTab)
                    {
                        e.Tab = null;
                        return;
                    }

                    mode = "product";
                }
                break;

            case "properties":
                {
                    mode = "properties";
                }
                break;

            case "analytics":
                {
                    mode = "analytics";
                }
                break;
        }

        if (Node != null)
        {
            var tab = e.Tab;

            var settings = new UIPageURLSettings
            {
                Mode = mode,
                Node = Node,
                NodeID = Node.NodeID,
                Culture = Node.DocumentCulture,
                PreferredURL = tab.RedirectUrl
            };

            tab.RedirectUrl = DocumentUIHelper.GetDocumentPageUrl(settings);
        }
    }
}
