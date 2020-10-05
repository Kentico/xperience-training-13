using System;
using System.Data;

using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_Attachments_DocumentAttachments_DirectUploader : AttachmentsControl, IUploaderControl
{
    #region "Variables"

    private const string CONTENT_FOLDER = "~/CMSModules/Content/";

    private string mInnerDivClass = "NewAttachment";

    private Guid attachmentGuid = Guid.Empty;
    private DocumentAttachment innerAttachment;

    private bool createTempAttachment;
    private AttachmentsWithVariantsTransformationDataProvider mVariantsProvider;

    #endregion


    #region "Properties"

    /// <summary>
    /// CSS class of the new attachment link.
    /// </summary>
    public string InnerDivClass
    {
        get
        {
            return mInnerDivClass;
        }
        set
        {
            mInnerDivClass = value;
        }
    }


    /// <summary>
    /// Last performed action.
    /// </summary>
    public string LastAction
    {
        get
        {
            return ValidationHelper.GetString(ViewState["LastAction"], null);
        }
        set
        {
            ViewState["LastAction"] = value;
        }
    }


    /// <summary>
    /// Info label.
    /// </summary>
    public Label InfoLabel
    {
        get
        {
            return lblInfo;
        }
    }


    /// <summary>
    /// Inner attachment GUID (GUID of temporary attachment created for new culture version).
    /// </summary>
    public override Guid InnerAttachmentGUID
    {
        get
        {
            return ValidationHelper.GetGuid(ViewState["InnerAttachmentGUID"], base.InnerAttachmentGUID);
        }
        set
        {
            ViewState["InnerAttachmentGUID"] = value;
            base.InnerAttachmentGUID = value;
        }
    }


    /// <summary>
    /// Name of document attachment column.
    /// </summary>
    public override string GUIDColumnName
    {
        get
        {
            return base.GUIDColumnName;
        }
        set
        {
            base.GUIDColumnName = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                newAttachmentElem.AttachmentGUIDColumnName = value;
            }
        }
    }


    /// <summary>
    /// Width of the attachment.
    /// </summary>
    public override int ResizeToWidth
    {
        get
        {
            return base.ResizeToWidth;
        }
        set
        {
            base.ResizeToWidth = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.ResizeToWidth = value;
            }
        }
    }


    /// <summary>
    /// Height of the attachment.
    /// </summary>
    public override int ResizeToHeight
    {
        get
        {
            return base.ResizeToHeight;
        }
        set
        {
            base.ResizeToHeight = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.ResizeToHeight = value;
            }
        }
    }


    /// <summary>
    /// Maximal side size of the attachment.
    /// </summary>
    public override int ResizeToMaxSideSize
    {
        get
        {
            return base.ResizeToMaxSideSize;
        }
        set
        {
            base.ResizeToMaxSideSize = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.ResizeToMaxSideSize = value;
            }
        }
    }


    /// <summary>
    /// List of allowed extensions.
    /// </summary>
    public override string AllowedExtensions
    {
        get
        {
            return base.AllowedExtensions;
        }
        set
        {
            base.AllowedExtensions = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.AllowedExtensions = value;
            }
        }
    }


    /// <summary>
    /// Specifies the document for which the attachments should be displayed.
    /// </summary>
    public override int DocumentID
    {
        get
        {
            return base.DocumentID;
        }
        set
        {
            base.DocumentID = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.DocumentID = value;
            }
        }
    }


    /// <summary>
    /// Specifies the version of the document (optional).
    /// </summary>
    public int VersionHistoryID
    {
        get
        {
            // When a new culture version is created, after an uploader action temporary attachment is handled
            if ((Form != null) && (Form.Mode == FormModeEnum.InsertNewCultureVersion) && !string.IsNullOrEmpty(LastAction))
            {
                return 0;
            }
            else if (Node != null)
            {
                return Node.DocumentCheckedOutVersionHistoryID;
            }

            return 0;
        }
    }


    /// <summary>
    /// Defines the form GUID; indicates that the temporary attachment will be handled.
    /// </summary>
    public override Guid FormGUID
    {
        get
        {
            return base.FormGUID;
        }
        set
        {
            base.FormGUID = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                dsAttachments.AttachmentFormGUID = value;
                newAttachmentElem.FormGUID = value;
            }
        }
    }


    /// <summary>
    /// Value of the control.
    /// </summary>
    public override object Value
    {
        get
        {
            return hdnAttachGuid.Value;
        }
        set
        {
            hdnAttachGuid.Value = value?.ToString();
        }
    }


    /// <summary>
    /// Name of the attachment.
    /// </summary>
    public string AttachmentName
    {
        get
        {
            return hdnAttachName.Value;
        }
    }


    /// <summary>
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                dsAttachments.StopProcessing = value;
                newAttachmentElem.StopProcessing = value;
            }
        }
    }


    /// <summary>
    /// Provider for variants data for attachment
    /// </summary>
    private ObjectTransformationDataProvider VariantsProvider
    {
        get
        {
            return mVariantsProvider ?? (mVariantsProvider = new AttachmentsWithVariantsTransformationDataProvider(Node, DocumentManager, FormGUID));
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Ensure info message
        if ((Request[Page.postEventSourceID] == hdnPostback.ClientID) || Request[Page.postEventSourceID] == hdnFullPostback.ClientID)
        {
            string action = Request[Page.postEventArgumentID];

            if (action != null)
            {
                string[] values = action.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                action = values[0];
                if ((values.Length > 1) && ValidationHelper.IsGuid(values[1]))
                {
                    Value = values[1];
                }
            }

            LastAction = action;
        }

        RegisterScripts();
        SetupGrid();

        if (DocumentManager != null)
        {
            DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;
        }

        // Ensure to raise the events
        if (RequestHelper.IsPostBack())
        {
            switch (LastAction)
            {
                case "delete":
                    RaiseDeleteFile(this, e);
                    Value = null;
                    break;

                case "update":
                    RaiseUploadFile(this, e);
                    break;
            }

            LoadAttachmentGUID();
        }

        // Load data
        ReloadData(false);
    }


    private void LoadAttachmentGUID()
    {
        InnerAttachmentGUID = ValidationHelper.GetGuid(Value, Guid.Empty);
    }


    private void SetupGrid()
    {
        // Grid initialization
        gridAttachments.OnExternalDataBound += GridDocsOnExternalDataBound;
        gridAttachments.OnAction += GridAttachmentsOnAction;
        gridAttachments.IsLiveSite = IsLiveSite;
        gridAttachments.Pager.PageSizeOptions = "10";
        gridAttachments.Pager.DefaultPageSize = 10;

        pnlAttachments.CssClass = "AttachmentsList SingleAttachment";
        pnlGrid.Attributes.Add("style", "padding-top: 2px;");
    }


    private void SetupNewAttachmentButton()
    {
        // Initialize button for adding attachments
        newAttachmentElem.SourceType = MediaSourceEnum.Attachment;
        newAttachmentElem.DocumentID = DocumentID;
        newAttachmentElem.NodeParentNodeID = NodeParentNodeID;
        newAttachmentElem.NodeClassName = NodeClassName;
        newAttachmentElem.ResizeToWidth = ResizeToWidth;
        newAttachmentElem.ResizeToHeight = ResizeToHeight;
        newAttachmentElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
        newAttachmentElem.FormGUID = FormGUID;
        newAttachmentElem.AttachmentGUIDColumnName = GUIDColumnName;
        newAttachmentElem.AllowedExtensions = AllowedExtensions;
        newAttachmentElem.ParentElemID = ClientID;
        newAttachmentElem.ForceLoad = true;
        newAttachmentElem.Text = GetString("attach.uploadfile");
        newAttachmentElem.InnerElementClass = InnerDivClass;
        newAttachmentElem.InnerLoadingElementClass = InnerLoadingDivClass;
        newAttachmentElem.IsLiveSite = IsLiveSite;
        newAttachmentElem.IncludeNewItemInfo = true;
        newAttachmentElem.CheckPermissions = CheckPermissions;
        newAttachmentElem.NodeSiteName = SiteName;
    }


    private void RegisterScripts()
    {
        string refreshScript;

        if ((Node != null) && (Node.DocumentID > 0))
        {
            refreshScript = $@"
if (window.RefreshTree && (fullRefresh || refreshTree)) {{
    RefreshTree({NodeParentNodeID}, {NodeID}); 
}}
RefreshUpdatePanel_{ClientID}(fullRefresh ? '{hdnFullPostback.ClientID}' : '{hdnPostback.ClientID}', action + '|' + guid);";
        }
        else
        {
            refreshScript = $"RefreshUpdatePanel_{ClientID}('{hdnPostback.ClientID}', action + '|' + guid);";
        }

        // Refresh script
        var script = String.Format(
@"
function RefreshUpdatePanel_{0}(hiddenFieldID, action) {{
    var hiddenField = document.getElementById(hiddenFieldID);
    if (hiddenField) {{
        __doPostBack(hiddenFieldID, action);
    }}
}}

function FullPageRefresh_{0}(guid) {{
    if (RefreshTree != null) {{
        RefreshTree();
    }}
    
    var hiddenField = document.getElementById('{1}');
    if (hiddenField) {{
        __doPostBack('{1}', 'refresh|' + guid);
    }}
}}

function InitRefresh_{0}(msg, fullRefresh, refreshTree, guid, action) {{
    if ((msg != null) && (msg != """")) {{
        alert(msg);
        action = 'error';
    }}

    {2}
}}

function DeleteConfirmation() {{
    return confirm({3});
}}
",
            ClientID,
            hdnFullPostback.ClientID,
            refreshScript,
            ScriptHelper.GetLocalizedString("attach.deleteconfirmation")
        );

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AttachmentScripts_" + ClientID, ScriptHelper.GetScript(script));

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterTooltip(Page);
    }


    void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        // Refresh control state
        if ((e.ActionName == DocumentComponentEvents.UNDO_CHECKOUT) || // After undo checkout action (there will be different attachments from previous version)
            e.WorkflowFinished || // When the workflow is finished (there may be published attachments instead of versioned)
            (e.ActionName == ComponentEvents.SAVE && (e.Mode != FormModeEnum.Update)) // When a new document is created, temporary may be converted to normal attachments
            )
        {
            // Update attachment GUID to current
            if (!String.IsNullOrEmpty(GUIDColumnName))
            {
                Value = e.Node.GetValue(GUIDColumnName);
                LoadAttachmentGUID();
            }

            ReloadData(true);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            lblError.Visible = (lblError.Text != "");
            lblInfo.Visible = (lblInfo.Text != "");

            // Ensure uploader button
            newAttachmentElem.Enabled = Enabled;

            // Hide actions
            gridAttachments.GridView.Columns[0].Visible = !HideActions;
            gridAttachments.GridView.Columns[1].Visible = !HideActions;
            if (Enabled)
            {
                newAttachmentElem.Visible = !HideActions;
            }
            else
            {
                newAttachmentElem.Visible = !HideActions && (attachmentGuid == Guid.Empty);
            }

            // Check if full refresh should be performed after upload
            newAttachmentElem.FullRefresh = FullRefresh;

            // Ensure correct layout
            bool gridHasData = !DataHelper.DataSourceIsEmpty(gridAttachments.DataSource);
            Visible = gridHasData || !HideActions;
            pnlGrid.Visible = gridHasData;

            // Initialize button for adding attachments
            plcUploader.Visible = (attachmentGuid == Guid.Empty) || !gridHasData;

            RegisterEditScript();
        }
    }


    private void RegisterEditScript()
    {
        // Dialog for editing attachment
        var sb = new StringBuilder();

        sb.AppendLine(String.Format(@"
function Edit_{0}(attachmentGUID, formGUID, versionHistoryID, parentId, hash, image) {{ 
  var form = '';
  if (formGUID != '') {{ 
      form = '&formguid=' + formGUID + '&parentid=' + parentId; 
  }}
  {1}
  if (image) {{
      modalDialog({2}, 'editorDialog', 905, 670); 
  }}
  else {{
      modalDialog({3}, 'editorDialog', 700, 400); 
  }}
  return false; 
}}",
            ClientID,
            (((Node != null) && (Node.DocumentID > 0)) ? "else{form = '&siteid=' + " + Node.NodeSiteID + ";}" : ""),
            "'" + ResolveUrl(CONTENT_FOLDER + "CMSDesk/Edit/ImageEditor.aspx" + "?attachmentGUID=' + attachmentGUID + '&refresh=1&versionHistoryID=' + versionHistoryID + form + '&clientid=" + ClientID + "&hash=' + hash"),
            "'" + ApplicationUrlHelper.ResolveDialogUrl(CONTENT_FOLDER + "Attachments/Dialogs/MetaDataEditor.aspx" + "?attachmentGUID=' + attachmentGUID + '&refresh=1&versionHistoryID=' + versionHistoryID + form + '&clientid=" + ClientID + "&hash=' + hash")
        ));

        // Register script for editing attachment
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AttachmentEditScripts_" + ClientID, ScriptHelper.GetScript(sb.ToString()));
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Indicates if the control contains some data.
    /// </summary>
    public override bool HasData()
    {
        return !DataHelper.DataSourceIsEmpty(dsAttachments.DataSource);
    }


    /// <summary>
    /// Clears data.
    /// </summary>
    public void Clear()
    {
        Value = Guid.Empty;
        ReloadData(true);
    }


    public override void ReloadData(bool forceReload)
    {
        // Clear the variants provider so that it does not request previously loaded attachments / history
        mVariantsProvider = null;

        Visible = !StopProcessing;
        if (StopProcessing)
        {
            dsAttachments.StopProcessing = true;
            newAttachmentElem.StopProcessing = true;
            // Do nothing
        }
        else
        {
            if ((Node != null) && (Node.DocumentID > 0))
            {
                dsAttachments.Path = Node.NodeAliasPath;
                dsAttachments.CultureCode = Node.DocumentCulture;
                dsAttachments.SiteName = SiteInfoProvider.GetSiteName(Node.OriginalNodeSiteID);
            }

            // Get attachment GUID
            attachmentGuid = ValidationHelper.GetGuid(Value, Guid.Empty);
            if (attachmentGuid == Guid.Empty)
            {
                dsAttachments.StopProcessing = true;
            }

            dsAttachments.DocumentVersionHistoryID = VersionHistoryID;
            dsAttachments.AttachmentFormGUID = FormGUID;
            dsAttachments.AttachmentGUID = attachmentGuid;

            // Force reload datasource
            if (forceReload)
            {
                dsAttachments.DataSource = null;
                dsAttachments.DataBind();
            }

            // Ensure right column name (for attachments under workflow)
            if (!DataHelper.DataSourceIsEmpty(dsAttachments.DataSource))
            {
                DataSet ds = (DataSet)dsAttachments.DataSource;
                if (ds != null)
                {
                    DataTable dt = (ds).Tables[0];
                    if (!dt.Columns.Contains("AttachmentFormGUID"))
                    {
                        dt.Columns.Add("AttachmentFormGUID");
                    }

                    // Get inner attachment
                    innerAttachment = new DocumentAttachment(dt.Rows[0]);
                    Value = innerAttachment.AttachmentGUID;
                    hdnAttachName.Value = innerAttachment.AttachmentName;

                    // Check if temporary attachment should be created
                    createTempAttachment = ((DocumentID == 0) && (DocumentID != innerAttachment.AttachmentDocumentID));
                }
            }

            SetupNewAttachmentButton();

            // Bind UniGrid to DataSource
            gridAttachments.DataSource = dsAttachments.DataSource;
            gridAttachments.LoadGridDefinition();
            gridAttachments.ReloadData();
        }
    }


    /// <summary>
    /// Overloaded ReloadData.
    /// </summary>
    public override void ReloadData()
    {
        ReloadData(false);
    }

    #endregion


    #region "Private & protected methods"

    /// <summary>
    /// UniGrid action buttons event handler.
    /// </summary>
    private void GridAttachmentsOnAction(string actionName, object actionArgument)
    {
        if (!Enabled || HideActions)
        {
            return;
        }

        if (!CheckActionPermissions())
        {
            return;
        }

        if (string.Equals(actionName, "delete", StringComparison.InvariantCultureIgnoreCase))
        {
            var attGuid = GetAttachmentGuid(actionArgument);
            DeleteAttachmentAction(attGuid);
        }

        // Force reload data
        ReloadData(true);
    }


    private void DeleteAttachmentAction(Guid attGuid)
    {
        if (!createTempAttachment)
        {
            if (attGuid != Guid.Empty)
            {
                // Delete attachment
                if (FormGUID == Guid.Empty)
                {
                    // Ensure automatic check-in/ check-out
                    VersionManager vm = null;

                    // Check out the document
                    if (AutoCheck)
                    {
                        vm = VersionManager.GetInstance(TreeProvider);
                        vm.CheckOut(Node, Node.IsPublished);
                    }

                    // If the GUID column is set, use it to process additional actions for field attachments
                    if (!String.IsNullOrEmpty(GUIDColumnName))
                    {
                        DocumentHelper.DeleteAttachment(Node, GUIDColumnName);
                    }
                    else
                    {
                        DocumentHelper.DeleteAttachment(Node, attGuid);
                    }
                    DocumentHelper.UpdateDocument(Node, TreeProvider);

                    // Ensure full page refresh
                    if (AutoCheck)
                    {
                        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "deleteRefresh", ScriptHelper.GetScript("InitRefresh_" + ClientID + "('', true, true, '" + attGuid + "', 'delete');"));
                    }
                    else
                    {
                        string script = "if (window.RefreshTree) { RefreshTree(" + Node.NodeParentID + ", " + Node.NodeID + "); }";
                        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "refreshTree", ScriptHelper.GetScript(script));
                    }

                    // Check in the document
                    if (AutoCheck)
                    {
                        vm?.CheckIn(Node, null);
                    }

                    // Log synchronization task if not under workflow
                    if (!UsesWorkflow)
                    {
                        DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, TreeProvider);
                    }
                }
                else
                {
                    AttachmentInfoProvider.DeleteTemporaryAttachment(attGuid, SiteContext.CurrentSiteName);
                }
            }
        }

        LastAction = "delete";
        Value = null;
    }


    private static Guid GetAttachmentGuid(object actionArgument)
    {
        Guid attGuid = Guid.Empty;

        // Get action argument (Guid or int)
        if (ValidationHelper.IsGuid(actionArgument))
        {
            attGuid = ValidationHelper.GetGuid(actionArgument, Guid.Empty);
        }

        return attGuid;
    }


    private bool CheckActionPermissions()
    {
        if (CheckPermissions)
        {
            if (FormGUID != Guid.Empty)
            {
                if (!RaiseOnCheckPermissions("Create", this))
                {
                    if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(NodeParentNodeID, NodeClassName))
                    {
                        lblError.Text = GetString("attach.actiondenied");
                        return false;
                    }
                }
            }
            else
            {
                if (!RaiseOnCheckPermissions("Modify", this))
                {
                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
                    {
                        lblError.Text = GetString("attach.actiondenied");
                        return false;
                    }
                }
            }
        }

        return true;
    }


    /// <summary>
    /// UniGrid external data bound.
    /// </summary>
    private object GridDocsOnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (String.IsNullOrEmpty(sourceName))
        {
            return parameter;
        }

        switch (sourceName.ToLowerInvariant())
        {
            case "update":
                {
                    var drv = parameter as DataRowView;

                    var plcUpd = new PlaceHolder { ID = "plcUdateAction" };
                    var pnlBlock = new Panel { ID = "pnlBlock" };

                    plcUpd.Controls.Add(pnlBlock);

                    CreateUploaderControl(drv, pnlBlock);

                    return plcUpd;
                }

            case "edit":
                {
                    var row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

                    // Get file extension
                    string extension = ValidationHelper.GetString(row["AttachmentExtension"], string.Empty)
                                                       .ToLowerInvariant();

                    // Get attachment GUID
                    attachmentGuid = ValidationHelper.GetGuid(row["AttachmentGUID"], Guid.Empty);

                    var img = sender as CMSGridActionButton;
                    if (img != null)
                    {
                        if (createTempAttachment)
                        {
                            img.Visible = false;
                        }
                        else
                        {
                            img.ScreenReaderDescription = extension;
                            img.ToolTip = attachmentGuid.ToString();
                            img.PreRender += img_PreRender;
                        }
                    }
                    return parameter;
                }


            case "delete":
                CMSGridActionButton imgDelete = sender as CMSGridActionButton;
                if (imgDelete != null)
                {
                    // Turn off validation
                    imgDelete.CausesValidation = false;
                    imgDelete.PreRender += imgDelete_PreRender;
                }
                return parameter;

            case "attachment":
            case "attachmentsize":
                {
                    var id = ValidationHelper.GetInteger(parameter, 0);
                    var fileName = (sourceName == "attachment") ? "Attachment.ascx" : "AttachmentSize.ascx";

                    return new ObjectTransformation(AttachmentInfo.OBJECT_TYPE, id)
                    {
                        DataProvider = VariantsProvider,
                        TransformationName = "~/CMSModules/Content/Controls/Attachments/DocumentAttachments/Transformations/" + fileName,
                        NoDataTransformation = ResHelper.GetString("Attachment.NotAvailable")
                    };
                }

            default:
                return parameter;
        }
    }


    private void CreateUploaderControl(DataRowView drv, Panel pnlBlock)
    {
        // Add update control
        // Dynamically load uploader control
        DirectFileUploader dfuElem = Page.LoadUserControl(CONTENT_FOLDER + "Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx") as DirectFileUploader;

        // Set uploader's properties
        if (dfuElem != null)
        {
            dfuElem.ID = "dfuElem" + DocumentID;
            dfuElem.SourceType = MediaSourceEnum.Attachment;
            dfuElem.DisplayInline = true;

            if (!createTempAttachment)
            {
                dfuElem.AttachmentGUID = ValidationHelper.GetGuid(drv["AttachmentGUID"], Guid.Empty);
            }

            dfuElem.ForceLoad = true;
            dfuElem.FormGUID = FormGUID;
            dfuElem.AttachmentGUIDColumnName = GUIDColumnName;
            dfuElem.DocumentID = DocumentID;
            dfuElem.NodeParentNodeID = NodeParentNodeID;
            dfuElem.NodeClassName = NodeClassName;
            dfuElem.ResizeToWidth = ResizeToWidth;
            dfuElem.ResizeToHeight = ResizeToHeight;
            dfuElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
            dfuElem.AllowedExtensions = AllowedExtensions;
            dfuElem.ShowIconMode = true;
            dfuElem.InsertMode = false;
            dfuElem.ParentElemID = ClientID;
            dfuElem.IncludeNewItemInfo = true;
            dfuElem.CheckPermissions = CheckPermissions;
            dfuElem.NodeSiteName = SiteName;
            dfuElem.IsLiveSite = IsLiveSite;
            // Setting of the direct single mode
            dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
            dfuElem.MaxNumberToUpload = 1;

            dfuElem.PreRender += dfuElem_PreRender;
            pnlBlock.Controls.Add(dfuElem);
        }
    }


    protected void dfuElem_PreRender(object sender, EventArgs e)
    {
        var dfuElem = (DirectFileUploader)sender;

        dfuElem.ForceLoad = true;
        dfuElem.FormGUID = FormGUID;
        dfuElem.AttachmentGUIDColumnName = GUIDColumnName;
        dfuElem.DocumentID = DocumentID;
        dfuElem.NodeParentNodeID = NodeParentNodeID;
        dfuElem.NodeClassName = NodeClassName;
        dfuElem.ResizeToWidth = ResizeToWidth;
        dfuElem.ResizeToHeight = ResizeToHeight;
        dfuElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
        dfuElem.AllowedExtensions = AllowedExtensions;
        dfuElem.ShowIconMode = true;
        dfuElem.InsertMode = false;
        dfuElem.ParentElemID = ClientID;
        dfuElem.IncludeNewItemInfo = true;
        dfuElem.CheckPermissions = CheckPermissions;
        dfuElem.NodeSiteName = SiteName;
        dfuElem.IsLiveSite = IsLiveSite;
        dfuElem.FullRefresh = FullRefresh;

        // Setting of the direct single mode
        dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
        dfuElem.Width = 16;
        dfuElem.Height = 16;
        dfuElem.MaxNumberToUpload = 1;

        dfuElem.Enabled = Enabled;
    }


    protected void img_PreRender(object sender, EventArgs e)
    {
        var img = (CMSGridActionButton)sender;

        if (AuthenticationHelper.IsAuthenticated())
        {
            if (!Enabled)
            {
                // Disable edit icon
                img.Enabled = false;
            }
            else
            {
                string strForm = (FormGUID == Guid.Empty) ? "" : FormGUID.ToString();

                // Create security hash
                string form = null;
                if (!String.IsNullOrEmpty(strForm))
                {
                    form = $"&formguid={strForm}&parentid={NodeParentNodeID}";
                }
                else if ((Node != null) && (Node.DocumentID > 0))
                {
                    form += "&siteid=" + Node.NodeSiteID;
                }
                string isImage = ImageHelper.IsSupportedByImageEditor(img.ScreenReaderDescription) ? "true" : "false";
                string parameters = $"?attachmentGUID={img.ToolTip}&refresh=1&versionHistoryID={VersionHistoryID}{form}&clientid={ClientID}";
                string validationHash = QueryHelper.GetHash(parameters);


                img.OnClientClick = $"Edit_{ClientID}('{img.ToolTip}', '{strForm}', '{VersionHistoryID}', {NodeParentNodeID}, '{validationHash}', {isImage});return false;";
            }

            img.ToolTip = GetString("general.edit");
        }
        else
        {
            img.Visible = false;
        }
    }


    private void imgDelete_PreRender(object sender, EventArgs e)
    {
        var imgDelete = (CMSGridActionButton)sender;
        if (!Enabled || !AllowDelete)
        {
            // Disable delete icon in case that editing is not allowed
            imgDelete.Enabled = false;
            imgDelete.Style.Add("cursor", "default");
        }
    }

    #endregion
}