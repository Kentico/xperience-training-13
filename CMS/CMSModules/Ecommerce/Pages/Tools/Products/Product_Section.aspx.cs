using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.ECOMMERCE, "Products")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Section : CMSProductsPage
{
    #region "Private & protected variables"

    protected string viewpage = "~/CMSPages/blank.htm";
    private DataClassInfo classInfo;
    private int parentNodeID;

    #endregion


    #region "Private properties"

    private TreeNode TreeNode
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Check hash
        QueryHelper.ValidateHash("hash");

        parentNodeID = QueryHelper.GetInteger("parentNodeId", 0);

        if (parentNodeID > 0)
        {
            CheckExploreTreePermission();
        }

        // Do not redirect when document does not exist
        DocumentManager.RedirectForNonExistingDocument = false;
        
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        const string CONTENT_CMSDESK_FOLDER = "~/CMSModules/Content/CMSDesk/";

        // Register script files
        ScriptHelper.RegisterScriptFile(this, CONTENT_CMSDESK_FOLDER + "EditTabs.js");

        bool checkCulture = false;
        bool splitViewSupported = false;
        string action = QueryHelper.GetString("action", "edit").ToLowerCSafe();

        switch (action)
        {
            // New dialog / new page form
            case "new":
                int classId = QueryHelper.GetInteger("classid", 0);
                if (classId <= 0)
                {
                    // Get by class name if specified
                    string className = QueryHelper.GetString("classname", string.Empty);
                    if (className != string.Empty)
                    {
                        classInfo = DataClassInfoProvider.GetDataClassInfo(className);
                        if (classInfo != null)
                        {
                            classId = classInfo.ClassID;
                        }
                    }
                }

                const string EC_PRODUCTS_FOLDER = "~/CMSModules/Ecommerce/Pages/Tools/Products/";

                if (classId > 0)
                {
                    viewpage = ResolveUrl(CONTENT_CMSDESK_FOLDER + "Edit/Edit.aspx");

                    // Check if document type is allowed under parent node
                    if (parentNodeID > 0)
                    {
                        // Get the node
                        TreeNode = Tree.SelectSingleNode(parentNodeID, TreeProvider.ALL_CULTURES);
                        if (TreeNode != null)
                        {
                            if (!DocumentHelper.IsDocumentTypeAllowed(TreeNode, classId))
                            {
                                viewpage = CONTENT_CMSDESK_FOLDER + "NotAllowed.aspx?action=child";
                            }
                        }
                    }

                    // Use product page when product type is selected
                    classInfo = classInfo ?? DataClassInfoProvider.GetDataClassInfo(classId);
                    if ((classInfo != null) && (classInfo.ClassIsProduct))
                    {
                        viewpage = ResolveUrl(EC_PRODUCTS_FOLDER + "Product_New.aspx");
                    }
                }
                else
                {
                    if (parentNodeID > 0)
                    {
                        viewpage = EC_PRODUCTS_FOLDER + "New_ProductOrSection.aspx";
                    }
                    else
                    {
                        viewpage = EC_PRODUCTS_FOLDER + "Product_New.aspx?parentNodeId=0";
                    }
                }
                break;

            case "delete":
                // Delete dialog
                viewpage = CONTENT_CMSDESK_FOLDER + "Delete.aspx";
                break;

            default:
                // Edit mode
                viewpage = CONTENT_CMSDESK_FOLDER + "Edit/edit.aspx?mode=editform";
                splitViewSupported = true;

                // Ensure class info
                if ((classInfo == null) && (Node != null))
                {
                    classInfo = DataClassInfoProvider.GetDataClassInfo(Node.NodeClassName);
                }

                checkCulture = true;
                break;
        }

        // If culture version should be checked, check
        if (checkCulture)
        {
            // Check (and ensure) the proper content culture
            if (!CheckPreferredCulture())
            {
                RefreshParentWindow();
            }

            // Check split mode 
            bool isSplitMode = PortalUIHelper.DisplaySplitMode;
            bool combineWithDefaultCulture = !isSplitMode && SiteInfoProvider.CombineWithDefaultCulture(SiteContext.CurrentSiteName);

            var nodeId = QueryHelper.GetInteger("nodeid", 0);
            TreeNode = Tree.SelectSingleNode(nodeId, CultureCode, combineWithDefaultCulture);
            if (TreeNode == null)
            {
                // Document does not exist -> redirect to new culture version creation dialog
                viewpage = ProductUIHelper.GetNewCultureVersionPageUrl();
            }
        }

        // Apply the additional transformations to the view page URL
        viewpage = URLHelper.AppendQuery(viewpage, RequestContext.CurrentQueryString);
        viewpage = URLHelper.RemoveParameterFromUrl(viewpage, "mode");
        viewpage = URLHelper.AddParameterToUrl(viewpage, "mode", "productssection");
        viewpage = ResolveUrl(viewpage);
        viewpage = URLHelper.AddParameterToUrl(viewpage, "hash", QueryHelper.GetHash(viewpage));

        // Split mode enabled
        if (splitViewSupported && PortalUIHelper.DisplaySplitMode && (TreeNode != null) && (action == "edit" || action == "preview"))
        {
            viewpage = DocumentUIHelper.GetSplitViewUrl(viewpage);
        }

        URLHelper.Redirect(UrlResolver.ResolveUrl(viewpage));
    }


    #endregion
}
