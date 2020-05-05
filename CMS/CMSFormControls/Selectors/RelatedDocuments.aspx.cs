using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_RelatedDocuments : CMSModalPage
{
    #region "Variables"

    private TreeProvider mTreeProvider;
    private TreeNode node;

    #endregion


    #region "Properties"

    /// <summary>
    /// Tree provider.
    /// </summary>
    protected TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize modal page
        RegisterEscScript();
        ScriptHelper.RegisterWOpenerScript(Page);

        if (QueryHelper.ValidateHash("hash"))
        {
            string title = GetString("Relationship.AddRelatedDocs");
            Page.Title = title;
            PageTitle.TitleText = title;
            Save += btnSave_Click;

            AddNoCacheTag();

            addRelatedDocument.ShowButtons = false;

            if (EditedDocument != null)
            {
                // Get the node
                node = EditedDocument;

                if (node != null)
                {
                    // Check read permissions
                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
                    {
                        RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), HTMLHelper.HTMLEncode(node.GetDocumentName())));
                    }
                    else
                    {
                        lblInfo.Visible = false;
                    }

                    // Set tree node
                    addRelatedDocument.TreeNode = node;
                }
            }
        }
        else
        {
            addRelatedDocument.Visible = false;
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ltlScript.Text = ScriptHelper.GetScript("window.location = '" + url + "';");
        }
    }

   
    /// <summary>
    /// Save meta data of attachment.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Argument</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (addRelatedDocument.SaveRelationship())
        {
            string script = GetRefreshUpdatePanelScript();
            ltlScript.Text = ScriptHelper.GetScript(script);
        }
    }


    private string GetRefreshUpdatePanelScript()
    {
        var externalControlId = QueryHelper.GetControlClientId("externalControlID", string.Empty);
        return String.Format("if (wopener.RefreshUpdatePanel_{0}) {{ wopener.RefreshUpdatePanel_{0}(); CloseDialog(); }}", externalControlId);
    }

    #endregion
}
