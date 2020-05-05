using System;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_Controls_UI_ABVariant_NewPage : ContentActionsControl
{
    #region "Variables"

    private int mNodeId;
    private TreeNode mNode;
    private TreeProvider mTree;

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        rfvDocumentName.ErrorMessage = GetString("om.enterdocumentname");
        mNodeId = QueryHelper.GetInteger("parentnodeid", 0);
        // Get alias path
        mTree = new TreeProvider(MembershipContext.AuthenticatedUser);
        mNode = mTree.SelectSingleNode(mNodeId);

        if (!RequestHelper.IsPostBack())
        {
            if (mNode != null)
            {
                ucPath.Value = mNode.NodeAliasPath;
            }
        }
    }


    /// <summary>
    /// Creates document.
    /// </summary>
    public int Save()
    {
        // Validate input data
        string message = new Validator().NotEmpty(txtDocumentName.Text.Trim(), GetString("om.enterdocumentname")).Result;

        if (message != String.Empty)
        {
            ShowError(message);
            return 0;
        }

        if (mNode == null)
        {
            ShowError(GetString("general.invalidparameters"));
            return 0;
        }

        // Select parent node
        TreeNode parent = mTree.SelectSingleNode(SiteContext.CurrentSiteName, ucPath.Value.ToString(), TreeProvider.ALL_CULTURES, false, null, false);
        if (parent == null)
        {
            ShowError(GetString("om.pathdoesnotexists"));
            return 0;
        }

        // Check security
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(parent.NodeID, mNode.NodeClassName))
        {
            RedirectToAccessDenied(GetString("cmsdesk.notauthorizedtocreatedocument"));
            return 0;
        }

        var newDocument = ProcessAction(mNode, parent, "copynode", false, true, true);
        if (newDocument == null)
        {
            ShowError(string.Format(GetString("om.invalidchildtype"), parent.ClassName));
            return 0;
        }

        // Get all language translations
        var documents = DocumentHelper.GetDocuments()
                                      .All()
                                      .OnCurrentSite()
                                      .AllCultures()
                                      .WhereEquals("NodeID", newDocument.NodeID);

        // Limit length to 100 characters
        string documentName = TextHelper.LimitLength(txtDocumentName.Text.Trim(), 100, String.Empty);

        // Update all documents
        foreach (var document in documents)
        {
            UpdateDocument(document, documentName);

            // Set new node to any updated document to have updated info
            newDocument = document;
        }

        // Create new AB variant if AB test defined
        if (!CreateABVariant(newDocument))
        {
            return 0;
        }

        // Get the page mode
        if (PortalContext.ViewMode != ViewModeEnum.EditLive)
        {
            PortalContext.ViewMode = ViewModeEnum.EditForm;
        }
        txtDocumentName.Text = String.Empty;

        return newDocument.NodeID;
    }


    /// <summary>
    /// Creates new AB variant if AB test filled in.
    /// </summary>
    /// <param name="newDocument">Newly created document</param>
    /// <returns>True if new variant is successfully created or if AB test is not defined.</returns>
    private bool CreateABVariant(TreeNode newDocument)
    {
        // If ABTest selected - create new variant
        int abTestID = ValidationHelper.GetInteger(ucABTestSelector.Value, 0);
        if (abTestID <= 0)
        {
            return true;
        }

        // If no test found, handle it as no test was specified
        ABTestInfo abTest = ABTestInfo.Provider.Get(abTestID);
        if (abTest == null)
        {
            return true;
        }

        string defaultCodeName = TextHelper.LimitLength(ValidationHelper.GetCodeName(newDocument.NodeAlias), 45, String.Empty);
        string codeName = defaultCodeName;
        ABVariantInfo info = ABVariantInfoProvider.GetABVariantInfo(codeName, abTest.ABTestName, SiteContext.CurrentSiteName);

        // Find non existing variant code name 
        int index = 0;
        while (info != null)
        {
            index++;
            codeName = defaultCodeName + "-" + index;
            info = ABVariantInfoProvider.GetABVariantInfo(codeName, abTest.ABTestName, SiteContext.CurrentSiteName);
        }

        // Save AB Variant 
        ABVariantInfo variantInfo = new ABVariantInfo();
        variantInfo.ABVariantTestID = abTestID;
        variantInfo.ABVariantPath = newDocument.NodeAliasPath;
        variantInfo.ABVariantName = codeName;
        variantInfo.ABVariantDisplayName = newDocument.NodeAlias;
        variantInfo.ABVariantSiteID = SiteContext.CurrentSiteID;

        try
        {
            ABVariantInfo.Provider.Set(variantInfo);
        }
        catch (InfoObjectException ex)
        {
            newDocument.Delete(true, true);
            ShowError(ex.Message);
            return false;
        }
        return true;
    }


    /// <summary>
    /// Updates document based on new alias and checkbox properties.
    /// </summary>
    /// <param name="document">Document to update</param>
    /// <param name="name">New document name</param>
    private void UpdateDocument(TreeNode document, string name)
    {
        document.DocumentSearchExcluded = chkExcludeFromSearch.Checked;
        document.NodeAlias = name;
        document.DocumentName = name;
        document.NodeName = name;

        document.SetDocumentNameSource(name);
        document.Update();
    }

    #endregion
}