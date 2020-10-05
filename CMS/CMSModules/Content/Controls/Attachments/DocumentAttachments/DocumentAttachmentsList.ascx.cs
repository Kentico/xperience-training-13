using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;


using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_Attachments_DocumentAttachments_DocumentAttachmentsList : AttachmentsControl
{
    #region "Variables"

    private const string CONTENT_FOLDER = "~/CMSModules/Content/";

    private string mInnerDivClass = "NewAttachment";
    private int? mFilterLimit;
    private int mUpdateIconPanelWidth = 16;
    private AttachmentsWithVariantsTransformationDataProvider mVariantsProvider;

    #endregion


    #region "Properties"

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


    /// <summary>
    /// Minimal count of entries for display filter.
    /// </summary>
    public int FilterLimit
    {
        get
        {
            if (mFilterLimit == null)
            {
                mFilterLimit = ValidationHelper.GetInteger(Service.Resolve<IAppSettingsService>()["CMSDefaultListingFilterLimit"], 25);
            }
            return (int)mFilterLimit;
        }
        set
        {
            mFilterLimit = value;
        }
    }


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
    /// Info label.
    /// </summary>
    public Label InfoLabel
    {
        get
        {
            return MessagesPlaceHolder.InfoLabel;
        }
    }


    /// <summary>
    /// Indicates whether grouped attachments should be displayed.
    /// </summary>
    public override Guid GroupGUID
    {
        get
        {
            return base.GroupGUID;
        }
        set
        {
            base.GroupGUID = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                dsAttachments.AttachmentGroupGUID = value;
                newAttachmentElem.AttachmentGroupGUID = value;
            }
        }
    }


    /// <summary>
    /// Indicates if paging is allowed.
    /// </summary>
    public override bool AllowPaging
    {
        get
        {
            return base.AllowPaging;
        }
        set
        {
            base.AllowPaging = value;
            if (gridAttachments != null)
            {
                gridAttachments.Pager.DisplayPager = value;
                gridAttachments.GridView.AllowPaging = value;
            }
        }
    }


    /// <summary>
    /// Defines size of the page for paging.
    /// </summary>
    public override string PageSize
    {
        get
        {
            return base.PageSize;
        }
        set
        {
            base.PageSize = value;
            if (gridAttachments != null)
            {
                gridAttachments.PageSize = value;
            }
        }
    }


    /// <summary>
    /// Default page size.
    /// </summary>
    public override int DefaultPageSize
    {
        get
        {
            return base.DefaultPageSize;
        }
        set
        {
            base.DefaultPageSize = value;
            if (gridAttachments != null)
            {
                gridAttachments.Pager.DefaultPageSize = value;
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
            if ((Node != null) && (Node.DocumentID > 0))
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
    /// Content container
    /// </summary>
    public Panel Container
    {
        get
        {
            return pnlCont;
        }
    }


    /// <summary>
    /// Label for workflow information
    /// </summary>
    public Label WorkflowLabel
    {
        get
        {
            return lblWf;
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
        // Node is not null neither for insert mode
        if ((dsAttachments == null) || (Node == null))
        {
            StopProcessing = true;
        }
        Visible = !StopProcessing;

        if (StopProcessing)
        {
            if (dsAttachments != null)
            {
                dsAttachments.StopProcessing = true;
            }
            if (newAttachmentElem != null)
            {
                newAttachmentElem.StopProcessing = true;
            }
            // Do nothing
        }
        else
        {
            // Ensure info message
            if ((Request[Page.postEventSourceID] == hdnPostback.ClientID) || Request[Page.postEventSourceID] == hdnFullPostback.ClientID)
            {
                string action = Request[Page.postEventArgumentID];

                switch (action)
                {
                    case "insert":
                        ShowConfirmation(GetString("attach.inserted"));
                        break;

                    case "update":
                        ShowConfirmation(GetString("attach.updated"));
                        break;

                    case "delete":
                        ShowConfirmation(GetString("attach.deleted"));
                        break;
                }
            }

            RegisterScripts();

            SetupNewAttachmentButton();
            SetupGrid();

            DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;
        }
    }


    private void SetupGrid()
    {
        // Grid initialization
        gridAttachments.OnExternalDataBound += gridAttachments_OnExternalDataBound;
        gridAttachments.OnDataReload += gridAttachments_OnDataReload;
        gridAttachments.OnAction += gridAttachments_OnAction;
        gridAttachments.OnBeforeDataReload += gridAttachments_OnBeforeDataReload;
        gridAttachments.ZeroRowsText = GetString("attach.nodatafound");
        gridAttachments.IsLiveSite = IsLiveSite;
        gridAttachments.ShowActionsMenu = !IsLiveSite;
        gridAttachments.Columns = "AttachmentID, AttachmentGUID, AttachmentImageWidth, AttachmentImageHeight, AttachmentExtension, AttachmentName, AttachmentSize, AttachmentOrder, AttachmentTitle, AttachmentDescription";
        gridAttachments.OrderBy = "AttachmentOrder, AttachmentName, AttachmentID";

        // Get all possible column names from appropriate info
        var allColumns = (VersionHistoryID == 0) ? AttachmentInfo.TYPEINFO.ClassStructureInfo.ColumnNames : AttachmentHistoryInfo.TYPEINFO.ClassStructureInfo.ColumnNames;

        gridAttachments.AllColumns = SqlHelper.MergeColumns(allColumns).Replace("AttachmentHistoryID", "AttachmentID");
    }


    private void SetupNewAttachmentButton()
    {
        // Initialize button for adding attachments
        newAttachmentElem.SourceType = MediaSourceEnum.DocumentAttachments;
        newAttachmentElem.DocumentID = DocumentID;
        newAttachmentElem.NodeParentNodeID = NodeParentNodeID;
        newAttachmentElem.NodeClassName = NodeClassName;
        newAttachmentElem.ResizeToWidth = ResizeToWidth;
        newAttachmentElem.ResizeToHeight = ResizeToHeight;
        newAttachmentElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
        newAttachmentElem.AttachmentGroupGUID = GroupGUID;
        newAttachmentElem.FormGUID = FormGUID;
        newAttachmentElem.AllowedExtensions = AllowedExtensions;
        newAttachmentElem.ParentElemID = ClientID;
        newAttachmentElem.ForceLoad = true;
        newAttachmentElem.Text = GetString("attach.newattachment");
        newAttachmentElem.InnerElementClass = InnerDivClass;
        newAttachmentElem.InnerLoadingElementClass = InnerLoadingDivClass;
        newAttachmentElem.IsLiveSite = IsLiveSite;
        newAttachmentElem.CheckPermissions = CheckPermissions;
    }


    private void RegisterScripts()
    {
        // Refresh script
        string script = String.Format(@"
function RefreshUpdatePanel_{0}(hiddenFieldID, action) {{
    var hiddenField = document.getElementById(hiddenFieldID);
    if (hiddenField) {{
        __doPostBack(hiddenFieldID, action);
    }}
}}

function FullPageRefresh_{0}(action) {{
    if(RefreshTree != null)
    {{
        RefreshTree();
    }}

    var hiddenField = document.getElementById('{1}');
    if (hiddenField) {{
       __doPostBack('{1}', action);
    }}
}}

function InitRefresh_{0}(msg, fullRefresh, refreshTree, action)
{{
    if((msg != null) && (msg != '')){{ 
        alert(msg); action='error'; 
    }}
    
    if (fullRefresh) {{
        FullPageRefresh_{0}(action);
    }}
    else {{
        RefreshUpdatePanel_{0}('{2}', action);
    }}
}}
",
            ClientID,
            hdnFullPostback.ClientID,
            hdnPostback.ClientID
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
            ReloadData();
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
            lblWf.Visible = (lblWf.Text != string.Empty);

            // Ensure uploader button
            newAttachmentElem.Enabled = Enabled;

            // Check if full refresh is needed
            newAttachmentElem.FullRefresh = FullRefresh;

            // Hide actions
            gridAttachments.GridView.Columns[0].Visible = !HideActions;
            gridAttachments.GridView.Columns[1].Visible = !HideActions;
            newAttachmentElem.Visible = !HideActions;

            // Check if filter should be visible
            pnlFilter.Visible = (dsAttachments.TotalRecords > FilterLimit) || !String.IsNullOrEmpty(txtFilter.Text);
            if (pnlFilter.Visible)
            {
                // Set different message when no attachment will be found
                gridAttachments.ZeroRowsText = GetString("attach.nodata");
            }

            // Ensure correct layout
            Visible = !HideActions || pnlFilter.Visible;

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
            (((Node != null) ? String.Format("else{{ form = '&siteid=' + {0}; }}", Node.NodeSiteID) : string.Empty)),
            "'" + ResolveUrl((CONTENT_FOLDER + "CMSDesk/Edit/ImageEditor.aspx") + "?attachmentGUID=' + attachmentGUID + '&versionHistoryID=' + versionHistoryID + form + '&clientid=" + ClientID + "&refresh=1&hash=' + hash"),
            "'" + ApplicationUrlHelper.ResolveDialogUrl(String.Format("{0}?attachmentGUID=' + attachmentGUID + '&versionHistoryID=' + versionHistoryID + form + '&clientid={1}&refresh=1&hash=' + hash", CONTENT_FOLDER + "Attachments/Dialogs/MetaDataEditor.aspx", ClientID))
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
        return (dsAttachments != null && !DataHelper.DataSourceIsEmpty(dsAttachments.DataSource));
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        // Clear the variants provider so that it does not request previously loaded attachments / history
        mVariantsProvider = null;

        gridAttachments.ReloadData();
    }

    #endregion


    #region "Private methods"

    private string GetWhereConditionInternal()
    {
        return string.IsNullOrEmpty(txtFilter.Text) ? null : String.Format("AttachmentName LIKE '%{0}%'", SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(txtFilter.Text)));
    }

    #endregion


    #region "Grid events"

    protected DataSet gridAttachments_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        if (Node != null)
        {
            dsAttachments.Path = Node.NodeAliasPath;
            // Prefer culture from the node
            dsAttachments.CultureCode = !string.IsNullOrEmpty(Node.DocumentCulture) ? Node.DocumentCulture : LocalizationContext.PreferredCultureCode;
            dsAttachments.SiteName = SiteInfoProvider.GetSiteName(Node.OriginalNodeSiteID);
        }

        // Versioned attachments
        dsAttachments.DocumentVersionHistoryID = VersionHistoryID;
        dsAttachments.AttachmentGroupGUID = GroupGUID;

        // Set FormGUID if temporary attachments should be displayed
        if ((Form != null) && (Form.Mode != FormModeEnum.Update))
        {
            dsAttachments.AttachmentFormGUID = FormGUID;
        }
        else
        {
            dsAttachments.AttachmentFormGUID = Guid.Empty;
        }

        dsAttachments.WhereCondition = SqlHelper.AddWhereCondition(GetWhereConditionInternal(), completeWhere);
        dsAttachments.SelectedColumns = columns;
        dsAttachments.TopN = currentTopN;
        dsAttachments.OrderBy = currentOrder;
        dsAttachments.LoadPagesIndividually = true;
        dsAttachments.UniPagerControl = gridAttachments.Pager.UniPager;
        dsAttachments.LoadData(true);

        // Ensure right column name (for attachments under workflow)
        if (!DataHelper.DataSourceIsEmpty(dsAttachments.DataSource))
        {
            totalRecords = dsAttachments.PagerForceNumberOfResults;

            DataSet ds = (DataSet)dsAttachments.DataSource;
            if (ds != null)
            {
                DataTable dt = (ds).Tables[0];
                if (!dt.Columns.Contains("AttachmentFormGUID"))
                {
                    dt.Columns.Add("AttachmentFormGUID");
                }
            }
        }

        return (DataSet)dsAttachments.DataSource;
    }


    protected void gridAttachments_OnBeforeDataReload()
    {
        gridAttachments.IsLiveSite = IsLiveSite;
        gridAttachments.GridView.AllowPaging = AllowPaging;
        if (!AllowPaging)
        {
            gridAttachments.PageSize = "0";
        }
        gridAttachments.GridView.AllowSorting = false;
    }


    /// <summary>
    /// UniGrid action buttons event handler.
    /// </summary>
    protected void gridAttachments_OnAction(string actionName, object actionArgument)
    {
        if (!Enabled || HideActions)
        {
            return;
        }

        #region "Check permissions"

        if (CheckPermissions)
        {
            if (FormGUID != Guid.Empty)
            {
                if (!RaiseOnCheckPermissions("Create", this))
                {
                    if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(NodeParentNodeID, NodeClassName))
                    {
                        ShowError(GetString("attach.actiondenied"));
                        return;
                    }
                }
            }
            else
            {
                if (!RaiseOnCheckPermissions("Modify", this))
                {
                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
                    {
                        ShowError(GetString("attach.actiondenied"));
                        return;
                    }
                }
            }
        }

        #endregion


        Guid attachmentGuid = Guid.Empty;

        // Get action argument (Guid or int)
        if (ValidationHelper.IsGuid(actionArgument))
        {
            attachmentGuid = ValidationHelper.GetGuid(actionArgument, Guid.Empty);
        }

        ProcessGridAttachmentAction(actionName, attachmentGuid);
    }


    private void ProcessGridAttachmentAction(string actionName, Guid attachmentGuid)
    {
        // Process proper action
        switch (actionName.ToLowerInvariant())
        {
            case "moveup":
                if (attachmentGuid != Guid.Empty)
                {
                    // Move attachment up
                    if (FormGUID == Guid.Empty)
                    {
                        PerformAttachmentAction("moveup", () => DocumentHelper.MoveAttachmentUp(attachmentGuid, Node));
                    }
                    else
                    {
                        var ai = AttachmentInfoProvider.GetTemporaryAttachmentInfo(attachmentGuid);
                        if (ai != null)
                        {
                            ai.Generalized.MoveObjectUp();
                        }
                    }
                }
                break;

            case "movedown":
                if (attachmentGuid != Guid.Empty)
                {
                    // Move attachment down
                    if (FormGUID == Guid.Empty)
                    {
                        PerformAttachmentAction("movedown", () => DocumentHelper.MoveAttachmentDown(attachmentGuid, Node));
                    }
                    else
                    {
                        var ai = AttachmentInfoProvider.GetTemporaryAttachmentInfo(attachmentGuid);
                        if (ai != null)
                        {
                            ai.Generalized.MoveObjectDown();
                        }
                    }
                }
                break;

            case "delete":
                if (attachmentGuid != Guid.Empty)
                {
                    // Delete attachment
                    if (FormGUID == Guid.Empty)
                    {
                        PerformAttachmentAction("delete", () => DocumentHelper.DeleteAttachment(Node, attachmentGuid));
                    }
                    else
                    {
                        AttachmentInfoProvider.DeleteTemporaryAttachment(attachmentGuid, SiteContext.CurrentSiteName);
                    }

                    ShowConfirmation(GetString("attach.deleted"));
                }
                break;
        }
    }


    /// <summary>
    /// Performes attachment action and handles workflow if set.
    /// </summary>
    /// <param name="actionName">Action name to correct refresh</param>
    /// <param name="action">Action to perform</param>
    private void PerformAttachmentAction(string actionName, Action action)
    {
        // Store original values
        var originalStep = Node.WorkflowStep;
        var wasArchived = Node.IsArchived;
        var wasInPublishedStep = Node.IsInPublishStep;

        // Ensure automatic check-in/ check-out
        VersionManager vm = null;
        bool checkin = false;

        // Check out the document
        if (AutoCheck)
        {
            vm = VersionManager.GetInstance(TreeProvider);
            var step = vm.CheckOut(Node, Node.IsPublished);

            // Do not check-in document if not under a workflow anymore
            checkin = (step != null);
        }

        // Perform action
        if (action != null)
        {
            action();
        }

        // Check in the document
        if (AutoCheck)
        {
            if ((vm != null) && checkin && (Node.DocumentWorkflowStepID != 0))
            {
                vm.CheckIn(Node, null);
            }

            // Document state changed
            bool fullRefresh = (originalStep == null) || originalStep.StepIsArchived || originalStep.StepIsPublished ||
                               (wasInPublishedStep != Node.IsInPublishStep) || (wasArchived != Node.IsArchived) || (WorkflowManager.GetNodeWorkflow(Node) == null);

            // Ensure full page refresh
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), actionName + "Refresh", ScriptHelper.GetScript(GetInitRefreshScript(actionName, fullRefresh)));

            // Clear document manager properties
            DocumentManager.ClearProperties();
        }

        // Log synchronization task if not under workflow
        if (!UsesWorkflow)
        {
            DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, TreeProvider);
        }
    }


    /// <summary>
    /// Creates script calling for refresh.
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <param name="fullRefresh">Indicates if full page refresh is requested</param>
    private string GetInitRefreshScript(string actionName, bool fullRefresh)
    {
        return String.Format("InitRefresh_{0}('', {2}, false, '{1}');", ClientID, actionName, fullRefresh ? "true" : "false");
    }


    /// <summary>
    /// UniGrid external data bound.
    /// </summary>
    protected object gridAttachments_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView drv;

        sourceName = sourceName.ToLowerInvariant();

        switch (sourceName)
        {
            case "update":
                {
                    var pnlBlock = new Panel { ID = "pnlBlock" };

                    pnlBlock.Style.Add("margin", "0 auto");
                    pnlBlock.PreRender += (senderObject, args) => pnlBlock.Width = mUpdateIconPanelWidth;

                    drv = (DataRowView)parameter;

                    CreateUploaderControl(drv, pnlBlock);

                    return pnlBlock;
                }

            case "edit":

                drv = (DataRowView)((GridViewRow)parameter).DataItem;

                // Get file extension
                string extension = ValidationHelper.GetString(drv["AttachmentExtension"], string.Empty).ToLowerInvariant();
                Guid guid = ValidationHelper.GetGuid(drv["AttachmentGUID"], Guid.Empty);
                CMSGridActionButton img = sender as CMSGridActionButton;
                if (img != null)
                {
                    img.ToolTip = String.Format("{0}|{1}", extension, guid);
                    img.PreRender += img_PreRender;
                }
                break;

            case "delete":
                CMSGridActionButton imgDelete = sender as CMSGridActionButton;
                if (imgDelete != null)
                {
                    // Turn off validation
                    imgDelete.CausesValidation = false;
                    imgDelete.PreRender += imgDelete_PreRender;
                }
                break;

            case "moveup":
                CMSGridActionButton imgUp = sender as CMSGridActionButton;
                if (imgUp != null)
                {
                    // Turn off validation
                    imgUp.CausesValidation = false;
                    imgUp.PreRender += imgUp_PreRender;
                }
                break;

            case "movedown":
                CMSGridActionButton imgDown = sender as CMSGridActionButton;
                if (imgDown != null)
                {
                    // Turn off validation
                    imgDown.CausesValidation = false;
                    imgDown.PreRender += imgDown_PreRender;
                }
                break;

            case "attachment":
            case "attachmentsize":
                {
                    drv = (DataRowView)parameter;

                    // For grid export return text data, not a control
                    if (sender == null)
                    {
                        return (sourceName == "attachment") ? drv["AttachmentName"] : DataHelper.GetSizeString(ValidationHelper.GetLong(drv["AttachmentSize"], 0));
                    }

                    var id = ValidationHelper.GetInteger(drv["AttachmentID"], 0);
                    var fileName = (sourceName == "attachment") ? "Attachment.ascx" : "AttachmentSize.ascx";

                    return
                        new ObjectTransformation(AttachmentInfo.OBJECT_TYPE, id)
                        {
                            DataProvider = VariantsProvider,
                            TransformationName = "~/CMSModules/Content/Controls/Attachments/DocumentAttachments/Transformations/" + fileName,
                            NoDataTransformation = ResHelper.GetString("Attachment.NotAvailable")
                        };
                }
        }

        return parameter;
    }


    private void CreateUploaderControl(DataRowView drv, Panel pnlBlock)
    {
        // Add update control
        // Dynamically load uploader control
        var dfuElem = Page.LoadUserControl(CONTENT_FOLDER + "Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx") as DirectFileUploader;

        // Set uploader's properties
        if (dfuElem != null)
        {
            dfuElem.SourceType = MediaSourceEnum.DocumentAttachments;
            dfuElem.ID = "dfuElem" + DocumentID;
            dfuElem.IsLiveSite = IsLiveSite;
            dfuElem.ControlGroup = "update";
            dfuElem.AttachmentGUID = ValidationHelper.GetGuid(drv["AttachmentGUID"], Guid.Empty);
            dfuElem.DisplayInline = true;
            dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
            dfuElem.MaxNumberToUpload = 1;
            dfuElem.PreRender += dfuElem_PreRender;
            pnlBlock.Controls.Add(dfuElem);
        }
    }

    #endregion


    #region "Grid actions' events"

    protected void dfuElem_PreRender(object sender, EventArgs e)
    {
        var dfuElem = (DirectFileUploader)sender;

        dfuElem.ForceLoad = true;
        dfuElem.FormGUID = FormGUID;
        dfuElem.AttachmentGroupGUID = GroupGUID;
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
        dfuElem.CheckPermissions = CheckPermissions;
        dfuElem.IsLiveSite = IsLiveSite;
        dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
        dfuElem.MaxNumberToUpload = 1;
        dfuElem.FullRefresh = FullRefresh;

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
                img.Style.Add("cursor", "default");
            }
            else
            {
                string[] args = img.ToolTip.Split('|');
                string strForm = (FormGUID == Guid.Empty) ? string.Empty : FormGUID.ToString();
                string form = null;

                if (!String.IsNullOrEmpty(strForm))
                {
                    form = String.Format("&formguid={0}&parentid={1}", strForm, NodeParentNodeID);
                }
                else
                {
                    if (Node != null)
                    {
                        form += "&siteid=" + Node.NodeSiteID;
                    }
                }

                string isImage = ImageHelper.IsSupportedByImageEditor(args[0]) ? "true" : "false";
                // Prepare parameters
                string parameters = String.Format("?attachmentGUID={0}&versionHistoryID={1}{2}&clientid={3}&refresh=1", args[1], VersionHistoryID, form, ClientID);
                // Create security hash
                string validationHash = QueryHelper.GetHash(parameters);

                img.OnClientClick = String.Format("Edit_{0}('{1}', '{2}', '{3}', {4}, '{5}', {6});return false;", ClientID, args[1], strForm, VersionHistoryID, NodeParentNodeID, validationHash, isImage);
            }

            img.ToolTip = GetString("general.edit");
        }
        else
        {
            img.Visible = false;
        }
    }


    protected void imgDown_PreRender(object sender, EventArgs e)
    {
        CMSGridActionButton imgDown = (CMSGridActionButton)sender;
        if (!Enabled || !AllowChangeOrder)
        {
            // Disable move down icon in case that editing is not allowed
            imgDown.Enabled = false;
            imgDown.Style.Add("cursor", "default");
        }
    }


    protected void imgUp_PreRender(object sender, EventArgs e)
    {
        CMSGridActionButton imgUp = (CMSGridActionButton)sender;
        if (!Enabled || !AllowChangeOrder)
        {
            // Disable move up icon in case that editing is not allowed
            imgUp.Enabled = false;
            imgUp.Style.Add("cursor", "default");
        }
    }


    protected void imgDelete_PreRender(object sender, EventArgs e)
    {
        var imgDelete = (CMSGridActionButton)sender;
        if (!Enabled)
        {
            // Disable delete icon in case that editing is not allowed
            imgDelete.Enabled = false;
            imgDelete.Style.Add("cursor", "default");
        }
    }

    #endregion    
}