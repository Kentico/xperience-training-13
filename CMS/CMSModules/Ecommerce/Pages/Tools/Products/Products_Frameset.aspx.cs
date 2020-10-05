using System;
using System.Web;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.ECOMMERCE, "Products")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Products_Frameset : CMSProductsPage
{
    #region "Variables"

    private int? mResultNodeID;
    private int? mResultDocumentID;

    #endregion


    #region "Properties"

    private int SelectedNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(Request.Params["selectedNodeId"], 0);
        }
    }


    private int ExpandNodeID
    {
        get
        {
            return QueryHelper.GetInteger("expandnodeid", 0);
        }
    }


    private int SelectedDocumentID
    {
        get
        {
            return ValidationHelper.GetInteger(Request.Params["selectedDocId"], 0);
        }
    }


    private string SelectedCulture
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedCulture"], LocalizationContext.PreferredCultureCode);
        }
    }


    private TreeNode RootNode
    {
        get
        {
            // Root
            string baseDoc = "/";
            if (ProductsStartingPath != String.Empty)
            {
                // Change to user's root page
                baseDoc = ProductsStartingPath;
            }
            // Try to get culture-specific root node
            TreeNode rootNode = Tree.SelectSingleNode(SiteContext.CurrentSiteName, baseDoc, LocalizationContext.PreferredCultureCode, false, null, false);

            if (rootNode == null)
            {
                // Get root node
                rootNode = Tree.SelectSingleNode(SiteContext.CurrentSiteName, baseDoc, TreeProvider.ALL_CULTURES, false, null, false);
            }

            return rootNode;
        }
    }


    protected int ResultNodeID
    {
        get
        {
            if (mResultNodeID == null)
            {
                // Get ID from query string
                mResultNodeID = NodeID;
                if (mResultNodeID <= 0)
                {
                    // Get ID selected by user
                    mResultNodeID = SelectedNodeID;
                    if (mResultNodeID <= 0)
                    {
                        // If no node specified, add the root node id
                        if (NodeID <= 0)
                        {
                            TreeNode rootNode = RootNode;
                            if (rootNode != null)
                            {
                                mResultNodeID = rootNode.NodeID;
                            }
                        }
                    }
                }
            }
            return mResultNodeID.Value;
        }
    }


    protected int ResultDocumentID
    {
        get
        {
            if (mResultDocumentID == null)
            {
                // Get ID from query string
                mResultDocumentID = DocumentID;
                if (mResultDocumentID <= 0)
                {
                    // Get ID selected by user
                    mResultDocumentID = SelectedDocumentID;
                    if ((mResultDocumentID <= 0) && (NodeID <= 0))
                    {
                        TreeNode rootNode = RootNode;
                        // If the culture match with selected culture
                        if ((rootNode != null) && rootNode.DocumentCulture.EqualsCSafe(SelectedCulture))
                        {
                            // Get identifier from the root node
                            mResultDocumentID = rootNode.DocumentID;
                        }
                    }
                }
            }
            return mResultDocumentID.Value;
        }
    }

    #endregion

   
    #region "Page events"

    /// <summary>
    /// Constructor
    /// </summary>
    public CMSModules_Ecommerce_Pages_Tools_Products_Products_Frameset()
    {
        new ContentUrlRetriever(this, ProductUIHelper.GetProductPageUrl);
    }
    

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Do not include document manager to the controls collection
        EnsureDocumentManager = false;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        
        string contentUrl = "Product_List.aspx" + RequestContext.CurrentQueryString;

        // Display product list if display tree of product sections is not allowed
        if (ECommerceSettings.ProductsTree(SiteContext.CurrentSiteID) == ProductsTreeModeEnum.None)
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl(contentUrl));
        }

        contenttree.Values.AddRange(new[] { new UILayoutValue("NodeID", ResultNodeID), new UILayoutValue("ExpandNodeID", ExpandNodeID), new UILayoutValue("Culture", SelectedCulture) });

        if (NodeID <= 0 || !IsAllowedInProductsStartingPath(NodeID))
        {
            // Root
            string baseDoc = "/";
            if (!string.IsNullOrEmpty(ProductsStartingPath))
            {
                // Change to products root node
                baseDoc = ProductsStartingPath.TrimEnd('/');
            }

            // Get the root node
            TreeNode rootNode = Tree.SelectSingleNode(SiteContext.CurrentSiteName, baseDoc, TreeProvider.ALL_CULTURES, false, null, false);
            if (rootNode != null)
            {
                string nodeString = rootNode.NodeID.ToString();
                contentUrl = URLHelper.AddParameterToUrl(contentUrl, "nodeId", nodeString);

                // Set default live site URL
                string liveURL = DocumentUIHelper.GetAbsolutePageUrl(rootNode, rootNode.DocumentCulture);

                ScriptHelper.RegisterStartupScript(this, typeof(string), "SetDefaultLiveSiteURL", ScriptHelper.GetScript("SetLiveSiteURL('" + HttpUtility.JavaScriptStringEncode(liveURL) + "');"));
            }
        }

        contentview.Src = contentUrl;

        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Content/CMSDesk/Content.js");

        // Override content functions
        AddScript(
@"
function SetMode(mode, passive) {
    if (!CheckChanges()) {
        return false;
    }
 
    SetSelectedMode(mode);
    if (!passive) {
        DisplayDocument();
    }
    return true;
}

function DragOperation(nodeId, targetNodeId, operation) {
    window.PerformContentRedirect(null, 'drag', nodeId, '&action=' + operation + '&targetnodeid=' + targetNodeId + '&mode=productssection');
}
");
        // Set ddNotScroll global variable to ensure FloatingBehavior.startDrag() will count with scrolling offset  
        AddScript("window.ddNotScroll = true;");
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), script.GetHashCode().ToString(), ScriptHelper.GetScript(script));
    }

    #endregion


    #region "Private Methods"
    
    /// <summary>
    /// Checks whether node with given node ID is contained in a products tree,
    /// considering the ProductsStartingPath setting.
    /// </summary>
    /// <param name="nodeId">NodeID of a node to be checked on.</param>
    private bool IsAllowedInProductsStartingPath(int nodeId)
    {
        var nodePath = TreePathUtils.GetAliasPathByNodeId(nodeId);
        var nodeSite = TreePathUtils.GetNodeSite(nodeId);

        if (String.IsNullOrEmpty(nodePath))
        {
            return false;
        }

        if (!String.IsNullOrEmpty(ProductsStartingPath))
        {
            if ((TreePathUtils.GetNodeIdByAliasPath(nodeSite.SiteName, ProductsStartingPath) > 0) &&
                !nodePath.StartsWithCSafe(ProductsStartingPath, true))
            {
                // Products starting path is defined and node on that path exists,
                // but our node is not contained within that path
                return false;
            }
        }

        return true;
    }

    #endregion
}
