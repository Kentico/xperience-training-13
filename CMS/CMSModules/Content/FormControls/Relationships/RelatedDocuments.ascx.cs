using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Relationships;
using CMS.SiteProvider;
using CMS.UIControls;

using Action = CMS.UIControls.UniGridConfig.Action;
using MessageTypeEnum = CMS.Base.Web.UI.MessageTypeEnum;
using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_FormControls_Relationships_RelatedDocuments : ReadOnlyFormEngineUserControl
{
    #region "Constants"

    private const string ADHOC_RELATIONSHIP_NAME = "##AUTO##";

    #endregion


    #region "Variables"

    private TreeProvider mTreeProvider;
    private bool mShowAddRelation = true;
    private DialogConfiguration mConfig;
    private string mAdHocRelationshipName;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Control is used as form field of Page data type (as ad hoc relationship).
    /// </summary>
    private bool IsAdHocRelationship
    {
        get
        {
            return (FieldInfo != null) && (FieldInfo.DataType == DocumentFieldDataType.DocRelationships);
        }
    }


    /// <summary>
    /// Ad-hoc relationship name based on document type class name and field GUID.
    /// </summary>
    private string AdHocRelationshipName
    {
        get
        {
            if (mAdHocRelationshipName != null)
            {
                return mAdHocRelationshipName;
            }

            if (!IsAdHocRelationship)
            {
                throw new NotSupportedException("Control can be used only for a Page data type of a form field.");
            }

            if (TreeNode != null)
            {
                mAdHocRelationshipName = RelationshipNameInfoProvider.GetAdHocRelationshipNameCodeName(TreeNode.NodeClassName, FieldInfo);
            }

            return mAdHocRelationshipName;
        }
    }


    /// <summary>
    /// Indicates if relationship name is created as ad-hoc within first relation created.
    /// </summary>
    private bool UseAdHocRelationshipName
    {
        get
        {
            return String.Equals(RelationshipName, ADHOC_RELATIONSHIP_NAME, StringComparison.InvariantCultureIgnoreCase);
        }
    }


    /// <summary>
    /// Gets the configuration for Copy and Move dialog.
    /// </summary>
    private DialogConfiguration Config
    {
        get
        {
            if (mConfig != null)
            {
                return mConfig;
            }

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
                ContentSites = IsAdHocRelationship ? AvailableSitesEnum.OnlyCurrentSite : AvailableSitesEnum.All,
                AdditionalQueryParameters = "contentchanged=false"
            };
            return mConfig;
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return GetValue("RelationshipName", String.Empty);
        }
        set
        {
            SetValue("RelationshipName", value);
        }
    }


    /// <summary>
    /// Indicates if allow switch sides.
    /// </summary>
    public bool AllowSwitchSides
    {
        get
        {
            return GetValue("AllowSwitchSides", true);
        }
        set
        {
            SetValue("AllowSwitchSides", value);
        }
    }


    /// <summary>
    /// Default side (False - left, True - right).
    /// </summary>
    public bool DefaultSide
    {
        get
        {
            return GetValue("DefaultSide", true);
        }
        set
        {
            SetValue("DefaultSide", value);
        }
    }


    /// <summary>
    /// Default page size.
    /// </summary>
    public int DefaultPageSize
    {
        get
        {
            return GetValue("DefaultPageSize", 5);
        }
        set
        {
            SetValue("DefaultPageSize", value);
        }
    }


    /// <summary>
    /// Number of related pages which can be added to page. 
    /// </summary>
    public int RelatedPagesLimit
    {
        get
        {
            return GetValue("RelatedPagesLimit", 0);
        }
        set
        {
            SetValue("RelatedPagesLimit", value);
        }
    }


    /// <summary>
    /// Page size values separated with comma.
    /// </summary>
    public string PageSize
    {
        get
        {
            return GetValue("PageSize", "5,10,25,50,100,##ALL##");
        }
        set
        {
            SetValue("PageSize", value);
        }
    }


    /// <summary>
    /// Indicates id show link 'Add relation'.
    /// </summary>
    public bool ShowAddRelation
    {
        get
        {
            return mShowAddRelation;
        }
        set
        {
            mShowAddRelation = value;
        }
    }


    /// <summary>
    /// Path where content tree in document selection dialog will start.
    /// </summary>
    public string StartingPath
    {
        get
        {
            return GetValue("StartingPath", "/");
        }
        set
        {
            SetValue("StartingPath", value);
        }
    }


    /// <summary>
    /// Gets or sets the document;.
    /// </summary>
    public TreeNode TreeNode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets tree provider.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
    }


    /// <summary>
    /// Enables or disables control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            UniGridRelationship.GridView.Enabled = value;
            btnNewRelationship.Enabled = value;
        }
    }


    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Paging
        UniGridRelationship.PageSize = PageSize;
        UniGridRelationship.Pager.DefaultPageSize = DefaultPageSize;
    }


    /// <summary>
    /// PageLoad event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Object type cannot be defined in xml definition -> it would ignore code behind configuration
        UniGridRelationship.ObjectType = (IsAdHocRelationship) ? RelationshipInfo.OBJECT_TYPE_ADHOC : RelationshipInfo.OBJECT_TYPE;

        if (StopProcessing)
        {
            UniGridRelationship.StopProcessing = StopProcessing;
            return;
        }

        // Set tree node from Form object
        if ((TreeNode == null) && (Form != null) && (Form.EditedObject != null))
        {
            var node = Form.EditedObject as TreeNode;
            if ((node != null) && (Form.Mode == FormModeEnum.Update))
            {
                TreeNode = node;
            }
            else
            {
                ShowInformation(GetString("relationship.editdocumenterror"));
            }
        }

        if (TreeNode != null)
        {
            InitUniGrid();

            int nodeId = TreeNode.NodeID;

            // Add relationship name condition
            var condition = new WhereCondition().WhereIn("RelationshipNameID", new IDQuery<RelationshipNameInfo>().Where(GetRelationshipNameCondition()));

            // Switch sides is disabled
            if (!AllowSwitchSides)
            {
                condition.WhereEquals(DefaultSide ? "RightNodeID" : "LeftNodeID", nodeId);
            }
            else
            {
                condition.Where(new WhereCondition().WhereEquals("RightNodeID", nodeId).Or().WhereEquals("LeftNodeID", nodeId));
            }

            InitFilterVisibility();

            UniGridRelationship.WhereCondition = condition.ToString(true);

            if (ShowAddRelation)
            {
                btnNewRelationship.OnClientClick = GetAddRelatedDocumentScript() + " return false;";
            }
            else
            {
                pnlNewLink.Visible = false;
            }
        }
        else
        {
            UniGridRelationship.StopProcessing = true;
            UniGridRelationship.Visible = false;

            btnNewRelationship.Enabled = false;
        }

        if (RequestHelper.IsPostBack())
        {
            string target = Request[Page.postEventSourceID];
            if ((target != pnlUpdate.ClientID) && (target != pnlUpdate.UniqueID))
            {
                return;
            }

            string action = Request[Page.postEventArgumentID];
            if (string.IsNullOrEmpty(action))
            {
                return;
            }

            switch (action.ToLowerInvariant())
            {
                // Insert from 'Select document' dialog
                case "insertfromselectdocument":
                    SaveRelationship();
                    break;
            }
        }
        else
        {
            bool inserted = QueryHelper.GetBoolean("inserted", false);
            if (inserted)
            {
                ShowConfirmation(GetString("relationship.wasadded"));
            }
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (TreeNode == null || RelatedPagesLimit <= 0)
        {
            return;
        }

        var relationshipCount = GetRelationshipCount();
        var pageCanBeAdded = relationshipCount < RelatedPagesLimit;

        btnNewRelationship.Enabled = pageCanBeAdded;
        btnNewRelationship.ToolTipResourceString = pageCanBeAdded ? null : "relationship.pagelimit.reached";

        if (relationshipCount > 0)
        {
            ShowMessage(MessageTypeEnum.Information, String.Format(GetString("relationship.pagelimit.info"), RelatedPagesLimit), null, null, true);
        }
    }


    /// <summary>
    /// Initializes the filter visibility
    /// </summary>
    private void InitFilterVisibility()
    {
        if (!AllowSwitchSides)
        {
            // Hide filter for the default side (always current document)
            var hideField = DefaultSide ? "RightNode" : "LeftNode";
            UniGridRelationship.FilterForm.FieldsToHide.Add(hideField);
        }

        if (!String.IsNullOrEmpty(RelationshipName))
        {
            // Hide filter for relationship name (always the same)
            UniGridRelationship.FilterForm.FieldsToHide.Add("RelationshipName");
        }
    }


    /// <summary>
    /// Gets where condition for selecting relationship names
    /// </summary>
    private WhereCondition GetRelationshipNameCondition()
    {
        var relationshipNameCondition = new WhereCondition();

        if (!UseAdHocRelationshipName)
        {
            relationshipNameCondition.Where(new WhereCondition().WhereFalse("RelationshipNameIsAdHoc").Or().WhereNull("RelationshipNameIsAdHoc"));

            if (!String.IsNullOrEmpty(RelationshipName))
            {
                relationshipNameCondition.WhereEquals("RelationshipName", RelationshipName);
            }
        }
        else
        {
            relationshipNameCondition.WhereEquals("RelationshipName", AdHocRelationshipName);
        }

        return relationshipNameCondition;
    }


    private void InitUniGrid()
    {
        var zeroRowsText = GetString("relationship.nodatafound");

        if (RelatedPagesLimit > 0)
        {
            zeroRowsText = zeroRowsText + " " + String.Format(GetString("relationship.pagelimit.info"), RelatedPagesLimit);
        }

        // Set unigrid
        UniGridRelationship.OnExternalDataBound += UniGridRelationship_OnExternalDataBound;
        UniGridRelationship.OnBeforeDataReload += UniGridRelationship_OnBeforeDataReload;
        UniGridRelationship.OnAction += UniGridRelationship_OnAction;
        UniGridRelationship.OnLoadColumns += UniGridRelationship_OnLoadColumns;
        UniGridRelationship.ZeroRowsText = zeroRowsText;
        UniGridRelationship.ShowActionsMenu = !IsLiveSite;
        UniGridRelationship.ShowExportMenu = true;
        UniGridRelationship.OrderBy = "RelationshipOrder";
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Performs actions before reload grid.
    /// </summary>
    private void UniGridRelationship_OnBeforeDataReload()
    {
        DataControlField rightTypeColumn = UniGridRelationship.NamedColumns["RightClassID"];

        // Hide columns
        if (!string.IsNullOrEmpty(RelationshipName) && !AllowSwitchSides)
        {
            string headerText = GetString("relationship.relateddocument");
            DataControlField leftColumn = UniGridRelationship.NamedColumns["LeftNodeName"];
            DataControlField relationshipNameColumn = UniGridRelationship.NamedColumns["RelationshipDisplayName"];
            DataControlField rightColumn = UniGridRelationship.NamedColumns["RightNodeName"];

            if (DefaultSide)
            {
                leftColumn.HeaderText = headerText;
                leftColumn.HeaderStyle.Width = new Unit("100%");
                rightColumn.Visible = false;
                rightTypeColumn.Visible = false;
            }
            else
            {
                DataControlField leftColumnType = UniGridRelationship.NamedColumns["LeftClassID"];

                rightColumn.HeaderText = headerText;
                rightTypeColumn.HeaderStyle.Width = new Unit("100%");
                leftColumn.Visible = false;
                leftColumnType.Visible = false;
            }

            // Hide relationship name column
            relationshipNameColumn.Visible = false;
        }
        else
        {
            rightTypeColumn.HeaderStyle.Width = new Unit("100%");
        }
    }


    /// <summary>
    /// Fires on the grid action.
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <param name="actionArgument">Action argument</param>
    private void UniGridRelationship_OnAction(string actionName, object actionArgument)
    {
        // Check modify permissions
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(TreeNode, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
        {
            return;
        }

        switch (actionName.ToLowerInvariant())
        {
            case "delete":
                var relationshipId = actionArgument.ToInteger(0);

                // If parameters are valid
                if (relationshipId > 0)
                {
                    // Remove relationship
                    RelationshipInfoProvider.RemoveRelationship(relationshipId);

                    // Log synchronization
                    DocumentSynchronizationHelper.LogDocumentChange(TreeNode.NodeSiteName, TreeNode.NodeAliasPath, TaskTypeEnum.UpdateDocument, TreeProvider);

                    ShowConfirmation(GetString("relationship.wasdeleted"));
                }

                break;
        }
    }

    private void UniGridRelationship_OnLoadColumns()
    {
        string[] actionsToRemove;

        if (!IsAdHocRelationship)
        {
            // Hide ordering actions if relationship is not ad hoc and does not support ordering
            actionsToRemove = new[] { "#move", "#moveup", "#movedown" };
        }
        else
        {
            if (IsLiveSite)
            {
                actionsToRemove = new[] { "#move" };
            }
            else
            {
                actionsToRemove = new[] { "#moveup", "#movedown" };
            }
        }

        UniGridRelationship.GridActions.Actions.RemoveAll(a =>
                           {
                               var action = a as Action;
                               if (action == null)
                               {
                                   return false;
                               }
                               return actionsToRemove.Contains(action.Name);
                           });
    }


    /// <summary>
    /// Binds the grid columns.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    private object UniGridRelationship_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "lefnodename":
            case "rightnodename":
                var tr = new ObjectTransformation(PredefinedObjectType.NODE, ValidationHelper.GetInteger(parameter, 0));
                tr.EncodeOutput = false;
                tr.Transformation = "{%NodeName|(default)" + GetString("general.root") + "|(encode)%}";
                return tr;

            case "delete":
                var btn = ((CMSGridActionButton)sender);
                btn.PreRender += imgDelete_PreRender;
                break;
        }

        return parameter;
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected void imgDelete_PreRender(object sender, EventArgs e)
    {
        CMSGridActionButton imgDelete = (CMSGridActionButton)sender;
        if (!Enabled)
        {
            // Disable delete icon in case that editing is not allowed
            imgDelete.Enabled = false;
            imgDelete.Style.Add("cursor", "default");
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Saves relationship.
    /// </summary>
    public void SaveRelationship()
    {
        if (TreeNode == null)
        {
            return;
        }

        // Check modify permissions
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(TreeNode, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
        {
            return;
        }

        bool currentNodeIsOnLeftSide = !DefaultSide;

        // Selected node Id
        int selectedNodeId = ValidationHelper.GetInteger(hdnSelectedNodeId.Value, 0);

        // Get relationshipNameId
        var relationshipNameId = GetRelationshipNameId();

        if ((selectedNodeId <= 0) || (relationshipNameId <= 0))
        {
            return;
        }

        try
        {
            // Left side
            if (currentNodeIsOnLeftSide)
            {
                RelationshipInfoProvider.AddRelationship(TreeNode.NodeID, selectedNodeId, relationshipNameId);
            }
            // Right side
            else
            {
                RelationshipInfoProvider.AddRelationship(selectedNodeId, TreeNode.NodeID, relationshipNameId);
            }

            // Log synchronization
            DocumentSynchronizationHelper.LogDocumentChange(TreeNode.NodeSiteName, TreeNode.NodeAliasPath, TaskTypeEnum.UpdateDocument, TreeProvider);

            ShowConfirmation(GetString("relationship.wasadded"));
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }


    /// <summary>
    /// Returns Javascript used for invoking 'add related document' dialog.
    /// </summary>
    public string GetAddRelatedDocumentScript()
    {
        string postbackArgument;
        if (!AllowSwitchSides && !string.IsNullOrEmpty(RelationshipName))
        {
            postbackArgument = "insertfromselectdocument";

            // Register javascript 'postback' function
            string script = "function RefreshRelatedPanel(elementId) { if (elementId != null) { __doPostBack(elementId, '" + postbackArgument + "'); } } \n";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshRelatedPanel", ScriptHelper.GetScript(script));

            // Dialog 'Select document'
            Config.EditorClientID = pnlUpdate.ClientID + ";" + hdnSelectedNodeId.ClientID;

            // Set dialog starting path
            if (!string.IsNullOrEmpty(StartingPath))
            {
                Config.ContentStartingPath = StartingPath;
            }

            string url = CMSDialogHelper.GetDialogUrl(Config, IsLiveSite, false, null, false);

            return string.Format("modalDialog('{0}', 'contentselectnode', '90%', '85%');", url);
        }
        else
        {
            postbackArgument = "insert";

            // Register javascript 'postback' function
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshUpdatePanel_" + ClientID, ScriptHelper.GetScript(
                "function RefreshUpdatePanel_" + ClientID + "(){ " + Page.ClientScript.GetPostBackEventReference(pnlUpdate, postbackArgument) + "; } \n"));

            // Dialog 'Add related document'
            string query = "?nodeid=" + TreeNode.NodeID;
            query = URLHelper.AddUrlParameter(query, "defaultside", DefaultSide.ToString());
            query = URLHelper.AddUrlParameter(query, "allowswitchsides", AllowSwitchSides.ToString());
            query = URLHelper.AddUrlParameter(query, "relationshipname", RelationshipName);
            query = URLHelper.AddUrlParameter(query, "externalControlID", ClientID);
            query = URLHelper.AddUrlParameter(query, "startingpath", StartingPath ?? "");

            query = query.Replace("%", "%25").Replace("/", "%2F");

            query = URLHelper.AddUrlParameter(query, "hash", QueryHelper.GetHash(query));

            string url;
            if (IsLiveSite)
            {
                url = ResolveUrl("~/CMSFormControls/LiveSelectors/RelatedDocuments.aspx" + query);
            }
            else
            {
                url = ResolveUrl("~/CMSFormControls/Selectors/RelatedDocuments.aspx" + query);
            }

            return string.Format("modalDialog('{0}', 'AddRelatedDocument', '900', '315');", url);
        }
    }


    private int GetRelationshipNameId()
    {
        var relationshipName = UseAdHocRelationshipName ? RelationshipNameInfoProvider.GetAdHocRelationshipNameCodeName(TreeNode.ClassName, FieldInfo) : RelationshipName;
        var relationshipNameInfo = RelationshipNameInfoProvider.GetRelationshipNameInfo(relationshipName);

        if (relationshipNameInfo == null)
        {
            throw new NullReferenceException("Missing relationship name to use for relation.");
        }

        return relationshipNameInfo.RelationshipNameId;
    }


    private int GetRelationshipCount()
    {
        return RelationshipInfoProvider.GetRelationships()
                                       .WhereEquals("RelationshipNameID", GetRelationshipNameId())
                                       .WhereEquals("LeftNodeID", TreeNode.NodeID).GetCount();
    }

    #endregion
}