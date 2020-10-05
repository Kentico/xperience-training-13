using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

[UIElement(ModuleName.ECOMMERCE, "Products.Categories")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Categories : CMSProductsPage
{
    #region "Variables"

    protected bool hasModifyPermission = true;
    protected CurrentUserInfo currentUser;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get current user
        currentUser = MembershipContext.AuthenticatedUser;

        // Enable split mode
        EnableSplitMode = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // UI settings
        categoriesElem.DisplaySavedMessage = false;
        categoriesElem.OnAfterSave += categoriesElem_OnAfterSave;
        categoriesElem.UniSelector.OnSelectionChanged += categoriesElem_OnSelectionChanged;

        if (Node != null)
        {
            // Check read permissions
            if (!Node.CheckPermissions(PermissionsEnum.Read, Node.NodeSiteName, currentUser))
            {
                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName())));
            }

            // Check modify permissions
            else if (!Node.CheckPermissions(PermissionsEnum.Modify, Node.NodeSiteName, currentUser))
            {
                hasModifyPermission = false;

                // Disable selector
                categoriesElem.Enabled = false;

                DocumentManager.DocumentInfo = String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName()));
            }

            // Display all global categories in administration UI
            categoriesElem.UserID = currentUser.UserID;
            categoriesElem.Node = Node;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display 'The changes were saved' message
        if (QueryHelper.GetBoolean("saved", false))
        {
            ShowChangesSaved();
        }
    }

    #endregion


    #region "Handlers"

    private void categoriesElem_OnAfterSave()
    {
        if (hasModifyPermission)
        {
            // Log the synchronization
            DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, Tree);
        }

        ShowChangesSaved();

        // Refresh frame in split mode
        if (PortalUIHelper.DisplaySplitMode && (CultureHelper.GetOriginalPreferredCulture() != Node.DocumentCulture))
        {
            AddScript("SplitModeRefreshFrame();");
        }
    }


    private void categoriesElem_OnSelectionChanged(object sender, EventArgs e)
    {
        if (hasModifyPermission)
        {
            categoriesElem.Save();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ltlScript.Text += ScriptHelper.GetScript(script);
    }

    #endregion
}
