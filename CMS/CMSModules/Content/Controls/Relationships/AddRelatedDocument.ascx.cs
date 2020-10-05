using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;

using System.Text;

using CMS.DataEngine;
using CMS.Membership;
using CMS.Relationships;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Relationships_AddRelatedDocument : CMSUserControl
{
    #region "Protected variables"

    protected int currentNodeId = 0;
    protected TreeNode node = null;
    protected DialogConfiguration mConfig = null;
    protected bool mEnabled = true;
    protected bool mShowButtons = true;
    protected bool mAllowSwitchSides = true;
    protected RelationshipNameInfo relationshipNameInfo = null;
    protected string mRedirectAfterSaveUrl = "~/CMSModules/Content/CMSDesk/Properties/Relateddocs_List.aspx";

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the configuration for dialog.
    /// </summary>
    private DialogConfiguration Config
    {
        get
        {
            if (mConfig == null)
            {
                mConfig = new DialogConfiguration
                {
                    HideLibraries = true,
                    ContentSelectedSite = SiteContext.CurrentSiteName,
                    HideAnchor = true,
                    HideAttachments = true,
                    HideContent = false,
                    HideEmail = true,
                    HideWeb = true,
                    OutputFormat = OutputFormatEnum.Custom,
                    CustomFormatCode = "relationship",
                    SelectableContent = SelectableContentEnum.AllContent
                };
            }
            return mConfig;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates if allow switch sides.
    /// </summary>
    public bool AllowSwitchSides
    {
        get
        {
            return mAllowSwitchSides;
        }
        set
        {
            mAllowSwitchSides = value;
        }
    }


    /// <summary>
    /// Default side (False - left, True - right).
    /// </summary>
    public bool DefaultSide
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets relationship name.
    /// </summary>
    public string RelationshipName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the document.
    /// </summary>
    public TreeNode TreeNode
    {
        get;
        set;
    }


    /// <summary>
    /// Enables or disables controls.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;

            btnLeftNode.Enabled = mEnabled;
            btnRightNode.Enabled = mEnabled;
            btnOk.Enabled = mEnabled;
            btnSwitchSides.Enabled = mEnabled;
            relNameSelector.Enabled = mEnabled;
            txtLeftNode.Enabled = mEnabled;
            txtRightNode.Enabled = mEnabled;
        }
    }


    /// <summary>
    /// Indicates if show buttons (OK, Close).
    /// </summary>
    public bool ShowButtons
    {
        get
        {
            return mShowButtons;
        }
        set
        {
            mShowButtons = value;
        }
    }


    /// <summary>
    /// Url to redirect after successful save.
    /// </summary>
    public string RedirectAfterSaveUrl
    {
        get
        {
            return mRedirectAfterSaveUrl;
        }
        set
        {
            mRedirectAfterSaveUrl = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Disable document manager events
        DocumentManager.RegisterEvents = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);

        DefaultSide = QueryHelper.GetBoolean("defaultside", DefaultSide);
        AllowSwitchSides = QueryHelper.GetBoolean("allowswitchsides", AllowSwitchSides);
        RelationshipName = QueryHelper.GetString("relationshipname", RelationshipName);
        Config.ContentStartingPath = QueryHelper.GetString("startingpath", Config.ContentStartingPath);

        relNameSelector.IsLiveSite = false;
        btnSwitchSides.Visible = AllowSwitchSides;
        btnOk.Visible = ShowButtons;

        // Initialize dialog scripts
        Config.EditorClientID = txtLeftNode.ClientID + ";" + hdnSelectedNodeId.ClientID;
        string url = CMSDialogHelper.GetDialogUrl(Config, false, null, false);
        btnLeftNode.OnClientClick = "modalDialog('" + url + "', 'contentselectnode', '90%', '85%'); return false;";

        Config.EditorClientID = txtRightNode.ClientID + ";" + hdnSelectedNodeId.ClientID;
        url = CMSDialogHelper.GetDialogUrl(Config, false, null, false);
        btnRightNode.OnClientClick = "modalDialog('" + url + "', 'contentselectnode', '90%', '85%'); return false;";

        if (TreeNode != null)
        {
            currentNodeId = TreeNode.NodeID;

            var documentName = HTMLHelper.HTMLEncode(TreeNode.GetDocumentName());
            // Check modify permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(TreeNode, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                Enabled = false;
                ShowInformation(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), documentName));
            }

            lblRightNode.Text = lblLeftNode.Text = documentName;
        }
        else
        {
            Enabled = false;
        }

        // All relationship names for current site
        if (string.IsNullOrEmpty(RelationshipName))
        {
            relNameSelector.Visible = true;
            lblRelName.Visible = false;
        }
        else
        {
            relationshipNameInfo = RelationshipNameInfo.Provider.Get(RelationshipName);
            if (relationshipNameInfo != null)
            {
                lblRelName.Text = relationshipNameInfo.RelationshipDisplayName;
            }

            relNameSelector.Visible = false;
            lblRelName.Visible = true;
        }

        // Register switching js
        if (btnSwitchSides.Enabled && btnSwitchSides.Visible)
        {
            RegisterScript();
        }

        if (!RequestHelper.IsPostBack())
        {
            hdnCurrentOnLeft.Value = !DefaultSide ? "true" : "false";
        }

        bool isLeftSide = ValidationHelper.GetBoolean(hdnCurrentOnLeft.Value, false);

        // Left side
        if (isLeftSide)
        {
            pnlLeftSelectedNode.AddCssClass("hidden");
            lblRightNode.AddCssClass("hidden");

            pnlRightSelectedNode.RemoveCssClass("hidden");
            lblLeftNode.RemoveCssClass("hidden");
        }
        // Right side
        else
        {
            lblLeftNode.AddCssClass("hidden");
            pnlRightSelectedNode.AddCssClass("hidden");

            pnlLeftSelectedNode.RemoveCssClass("hidden");
            lblRightNode.RemoveCssClass("hidden");
        }

        // Clear breadcrumbs suffix (we don't want it when creating new object)
        UIHelper.SetBreadcrumbsSuffix("");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        StringBuilder sb = new StringBuilder();
        if (txtLeftNode.Visible && txtRightNode.Enabled)
        {
            sb.Append("$cmsj('#", txtLeftNode.ClientID, "').change(ClearDialogNodeId);");
        }

        if (txtRightNode.Visible && txtRightNode.Enabled)
        {
            sb.Append("$cmsj('#", txtRightNode.ClientID, "').change(ClearDialogNodeId);");
        }

        if (sb.Length > 0)
        {
            sb.AppendFormat(@"
function ClearDialogNodeId() {{
    $cmsj('#{0}').val('');
}}
", hdnSelectedNodeId.ClientID);
            ScriptHelper.RegisterStartupScript(this, typeof(string), "ClearDialogNodeId", sb.ToString(), true);
        }
    }


    /// <summary>
    /// Handles OK button event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Argument</param>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (SaveRelationship())
        {
            if (!string.IsNullOrEmpty(RedirectAfterSaveUrl))
            {
                string url = URLHelper.AddParameterToUrl(RedirectAfterSaveUrl, "nodeid", currentNodeId.ToString());
                url = URLHelper.AddParameterToUrl(url, "inserted", "1");

                // Redirect
                URLHelper.Redirect(UrlResolver.ResolveUrl(url));
            }
        }
    }


    /// <summary>
    /// Saves relationship.
    /// </summary>
    /// <returns>True, if relatioship was successfully saved.</returns>
    public bool SaveRelationship()
    {
        bool saved = false;

        // Check modify permissions
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(TreeNode, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
        {
            return saved;
        }

        bool currentNodeIsOnLeftSide = ValidationHelper.GetBoolean(Request.Params[hdnCurrentOnLeft.UniqueID], false);
        int selectedNodeId = ValidationHelper.GetInteger(hdnSelectedNodeId.Value, 0);
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

        // Try to get by path if not selected
        if (selectedNodeId <= 0)
        {
            string aliaspath = currentNodeIsOnLeftSide ? txtRightNode.Text.Trim() : txtLeftNode.Text.Trim();

            if (aliaspath != string.Empty)
            {
                node = tree.SelectSingleNode(SiteContext.CurrentSiteName, aliaspath, TreeProvider.ALL_CULTURES);
                if (node != null)
                {
                    selectedNodeId = node.NodeID;
                }
                else
                {
                    ShowError(GetString("relationship.selectcorrectrelateddoc"));
                }
            }
            else
            {
                ShowError(GetString("relationship.selectrelateddoc"));
            }
        }

        int selectedValue = 0;
        // Only one relationship name in textbox
        if ((relationshipNameInfo != null) && (lblRelName.Visible))
        {
            selectedValue = relationshipNameInfo.RelationshipNameId;
        }
        // Value from relationship name selector
        else if (relNameSelector.Visible)
        {
            selectedValue = ValidationHelper.GetInteger(relNameSelector.Value, 0);
        }

        if ((currentNodeId > 0) && (selectedNodeId > 0) && (selectedValue > 0))
        {
            int relationshipNameId = selectedValue;

            try
            {
                // Left side
                if (currentNodeIsOnLeftSide)
                {
                    RelationshipInfo.Provider.Add(currentNodeId, selectedNodeId, relationshipNameId);
                }
                // Right side
                else
                {
                    RelationshipInfo.Provider.Add(selectedNodeId, currentNodeId, relationshipNameId);
                }

                // Log synchronization for single document
                TreeNode currentNode = node ?? tree.SelectSingleNode(currentNodeId);
                DocumentSynchronizationHelper.LogDocumentChange(currentNode, TaskTypeEnum.UpdateDocument, tree);

                saved = true;

                ShowChangesSaved();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        return saved;
    }


    /// <summary>
    /// Registers javascript functions.
    /// </summary>
    private void RegisterScript()
    {
        string leftCurentNodeClientID = lblLeftNode.ClientID;
        string rightCurrentNodeClientID = lblRightNode.ClientID;

        string leftSelectedNodeClientID = pnlLeftSelectedNode.ClientID;
        string rightSelectedNodeClientID = pnlRightSelectedNode.ClientID;

        string txtLeftNodeClientID = txtLeftNode.ClientID;
        string txtRightNodeClientID = txtRightNode.ClientID;
        
        string currentOnLeftClientID = hdnCurrentOnLeft.ClientID;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("function SwitchSides() {");
        sb.AppendLine("  var leftSide = $cmsj('#" + currentOnLeftClientID + "').val();");
        sb.AppendLine("  if (leftSide === 'true') {");
        sb.AppendLine("    $cmsj('#" + leftCurentNodeClientID + "').addClass('hidden');");
        sb.AppendLine("    $cmsj('#" + leftSelectedNodeClientID + "').removeClass('hidden');");
        sb.AppendLine("    $cmsj('#" + rightCurrentNodeClientID + "').removeClass('hidden');");
        sb.AppendLine("    $cmsj('#" + rightSelectedNodeClientID + "').addClass('hidden');");
        sb.AppendLine("    $cmsj('#" + currentOnLeftClientID + "').val('false');");
        sb.AppendLine("    $cmsj('#" + txtLeftNodeClientID + "').val($cmsj('#" + txtRightNodeClientID + "').val());");
        sb.AppendLine("  }");
        sb.AppendLine("  else if (leftSide === 'false') {");
        sb.AppendLine("    $cmsj('#" + leftCurentNodeClientID + "').removeClass('hidden');");
        sb.AppendLine("    $cmsj('#" + leftSelectedNodeClientID + "').addClass('hidden');");
        sb.AppendLine("    $cmsj('#" + rightCurrentNodeClientID + "').addClass('hidden');");
        sb.AppendLine("    $cmsj('#" + rightSelectedNodeClientID + "').removeClass('hidden');");
        sb.AppendLine("    $cmsj('#" + currentOnLeftClientID + "').val('true');");
        sb.AppendLine("    $cmsj('#" + txtRightNodeClientID + "').val($cmsj('#" + txtLeftNodeClientID + "').val());");
        sb.AppendLine("  }");
        sb.AppendLine("  return false;");
        sb.AppendLine("}");

        // Register script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SwitchSides", ScriptHelper.GetScript(sb.ToString()));
    }

    #endregion
}
