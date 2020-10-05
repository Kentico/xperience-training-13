using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing.Internal;
using CMS.DocumentEngine.Web.UI;
using CMS.FormEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

using Newtonsoft.Json.Linq;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;
using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_Controls_ViewVersion : VersionHistoryControl, IPostBackEventHandler
{
    #region "Variables"

    private FormInfo fi;

    private int versionHistoryId;
    private int versionCompare;

    private AttachmentsDataSource dsAttachments;
    private AttachmentsDataSource dsAttachmentsCompare;
    private IEnumerable<CulturePageUrlSlug> slugs;

    private Hashtable attachments;
    private Hashtable attachmentsCompare;

    private bool noCompare;
    private bool error;

    private TimeZoneInfo usedTimeZone;
    private TimeZoneInfo serverTimeZone;


    public CMSModules_Content_Controls_ViewVersion()
    {
        CompareNode = null;
    }


    private const string UNSORTED = "##UNSORTED##";
    private const string SLUGS = "##SLUGS##";

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
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Current edited node.
    /// </summary>
    public override TreeNode Node
    {
        get;
        set;
    }


    /// <summary>
    /// Compare node.
    /// </summary>
    public TreeNode CompareNode
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Shows the specified error message, optionally with a tooltip text.
    /// </summary>
    /// <param name="text">Error message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowError(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        base.ShowError(text, description, tooltipText, persistent);
        error = true;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ControlsHelper.EnsureScriptManager(Page);
        DocumentManager.RegisterSaveChangesScript = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register WOpener script
        ScriptHelper.RegisterWOpenerScript(Page);

        if (QueryHelper.GetBoolean("rollbackok", false))
        {
            ShowConfirmation(GetString("VersionProperties.RollbackOK"));
        }

        noCompare = QueryHelper.GetBoolean("noCompare", false);
        versionHistoryId = QueryHelper.GetInteger("versionhistoryid", 0);
        versionCompare = QueryHelper.GetInteger("compareHistoryId", 0);

        // Converting modified time to correct time zone
        usedTimeZone = TimeZoneHelper.GetTimeZoneInfo(CurrentUser, SiteContext.CurrentSite);
        serverTimeZone = TimeZoneHelper.ServerTimeZone;

        // No comparing available in Recycle bin
        pnlControl.Visible = !noCompare;

        if (versionHistoryId > 0)
        {
            try
            {
                // Get original version of document
                var version = VersionHistoryInfo.Provider.Get(versionHistoryId);
                slugs = version.GetPageUrlPathSlugs();
                Node = VersionManager.GetVersion(version, null);
                CompareNode = VersionManager.GetVersion(versionCompare);

                if (Node != null)
                {
                    // Check read permissions
                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) != AuthorizationResultEnum.Allowed)
                    {
                        RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName())));
                    }

                    if (!RequestHelper.IsPostBack())
                    {
                        LoadDropDown(Node.DocumentID, versionHistoryId);
                        ReloadData();
                    }

                    drpCompareTo.SelectedIndexChanged += drpCompareTo_SelectedIndexChanged;
                }
                else
                {
                    ShowInformation(GetString("editeddocument.notexists"));
                    pnlAdditionalControls.Visible = false;
                }
            }
            catch (Exception ex)
            {
                if (Node == null)
                {
                    RedirectToInformation(GetString("editeddocument.notexists"));
                }
                else
                {
                    ShowError(GetString("document.viewversionerror") + " " + ex.Message);
                }

                Service.Resolve<IEventLogService>().LogException("Content", "VIEWVERSION", ex);
            }
        }
    }


    /// <summary>
    /// Reloads control with new data.
    /// </summary>
    private void ReloadData()
    {
        tblDocument.Rows.Clear();

        DataClassInfo ci = DataClassInfoProvider.GetDataClassInfo(Node.NodeClassName);
        if (ci != null)
        {
            fi = FormHelper.GetFormInfo(ci.ClassName, false);

            TableHeaderCell labelCell = new TableHeaderCell();
            TableHeaderCell valueCell;

            // Add header column with version number
            if (CompareNode == null)
            {
                labelCell.Text = GetString("General.FieldName");
                labelCell.EnableViewState = false;
                valueCell = new TableHeaderCell();
                valueCell.EnableViewState = false;
                valueCell.Text = GetString("General.Value");

                // Add table header
                AddRow(labelCell, valueCell, "unigrid-head", true);
            }
            else
            {
                labelCell.Text = GetString("lock.versionnumber");
                valueCell = GetRollbackTableHeaderCell("source", Node.DocumentID, versionHistoryId);
                TableHeaderCell valueCompare = GetRollbackTableHeaderCell("compare", CompareNode.DocumentID, versionCompare);

                // Add table header
                AddRow(labelCell, valueCell, valueCompare, true, "unigrid-head", true);
            }

            if (ci.ClassIsProduct)
            {
                // Add coupled class fields
                ClassStructureInfo skuCsi = ClassStructureInfo.GetClassInfo("ecommerce.sku");
                if (skuCsi != null)
                {
                    foreach (string columnName in skuCsi.ColumnNames)
                    {
                        AddField(Node, CompareNode, columnName);
                    }
                }
            }

            if (ci.ClassIsCoupledClass)
            {
                // Add coupled class fields
                ClassStructureInfo coupledCsi = ClassStructureInfo.GetClassInfo(Node.NodeClassName);
                if (coupledCsi != null)
                {
                    foreach (string columnName in coupledCsi.ColumnNames)
                    {
                        // If comparing with other version and current coupled column is not versioned do not display it
                        if (!((CompareNode != null) && !(VersionManager.IsVersionedCoupledColumn(Node.NodeClassName, columnName))))
                        {
                            AddField(Node, CompareNode, columnName);
                        }
                    }
                }
            }

            // Add versioned document class fields
            ClassStructureInfo docCsi = ClassStructureInfo.GetClassInfo("cms.document");
            if (docCsi != null)
            {
                foreach (string columnName in docCsi.ColumnNames)
                {
                    // If comparing with other version and current document column is not versioned do not display it
                    if ((CompareNode == null) || VersionManager.IsVersionedDocumentColumn(columnName))
                    {
                        AddField(Node, CompareNode, columnName);
                    }
                }
            }

            // Add versioned document class fields
            ClassStructureInfo treeCsi = ClassStructureInfo.GetClassInfo("cms.tree");
            if (treeCsi != null)
            {
                foreach (string column in treeCsi.ColumnNames)
                {
                    // Do not display cms_tree columns when comparing with other version
                    // cms_tree columns are not versioned
                    if (CompareNode == null)
                    {
                        AddField(Node, CompareNode, column);
                    }
                }
            }

            // Add unsorted attachments to the table
            AddField(Node, CompareNode, UNSORTED);
            AddField(Node, null, SLUGS);
        }
    }


    /// <summary>
    /// Gets new table header cell which contains label and rollback image.
    /// </summary>
    /// <param name="suffixID">ID suffix</param>
    /// <param name="documentID">Document ID</param>
    /// <param name="versionID">Version history ID</param>
    private TableHeaderCell GetRollbackTableHeaderCell(string suffixID, int documentID, int versionID)
    {
        TableHeaderCell tblHeaderCell = new TableHeaderCell();
        tblHeaderCell.EnableViewState = false;

        string tooltip = GetString("history.versionrollbacktooltip");

        // Label
        Label lblValue = new Label();
        lblValue.ID = "lbl" + suffixID;
        lblValue.Text = HTMLHelper.HTMLEncode(GetVersionNumber(documentID, versionID)) + "&nbsp;";
        lblValue.EnableViewState = false;

        // Rollback image
        var imgRollback = new HyperLink();
        imgRollback.ID = "imgRollback" + suffixID;
        imgRollback.NavigateUrl = "#";
        imgRollback.Text = tooltip;
        imgRollback.EnableViewState = false;
        imgRollback.CssClass = "table-header-action";

        // Disable buttons according to permissions
        bool canApprove = WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve);

        if (!canApprove || !CanModify || (CheckedOutByAnotherUser && !CanCheckIn))
        {
            imgRollback.Enabled = false;
        }
        else
        {
            // Prepare onclick script
            string confirmScript = "if (confirm(" + ScriptHelper.GetString(GetString("Unigrid.VersionHistory.Actions.Rollback.Confirmation")) + ")) { ";
            confirmScript += Page.ClientScript.GetPostBackEventReference(this, versionID.ToString()) + "; } return false;";
            imgRollback.Attributes.Add("onclick", confirmScript);
        }

        tblHeaderCell.Controls.Add(lblValue);
        tblHeaderCell.Controls.Add(imgRollback);

        return tblHeaderCell;
    }


    /// <summary>
    /// Loads dropdown list with versions.
    /// </summary>
    /// <param name="documentId">ID of current document</param>
    /// <param name="historyId">ID of current version history</param>
    private void LoadDropDown(int documentId, int historyId)
    {
        DataSet ds = VersionManager.GetDocumentHistory(documentId)
            .WhereNotEquals("VersionHistoryID", historyId)
            .OrderByDescending("ModifiedWhen", "VersionNumber")
            .Columns("VersionHistoryID", "VersionNumber", "ModifiedWhen");

        // Converting modified time to correct time zone
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var version = GetVersionNumber(dr);
                drpCompareTo.Items.Add(new ListItem(version, ValidationHelper.GetString(dr["VersionHistoryID"], "0")));
            }
        }

        // If history to be compared is available
        if (drpCompareTo.Items.Count > 0)
        {
            drpCompareTo.Items.Insert(0, GetString("history.select"));
        }
        // Otherwise hide dropdown list
        else
        {
            pnlControl.Visible = false;
        }

        // Pre-select dropdown list
        if (drpCompareTo.Items.FindByValue(versionCompare.ToString()) != null)
        {
            drpCompareTo.SelectedValue = versionCompare.ToString();
        }
    }


    private string GetVersionNumber(DataRow dr)
    {
        string version = ValidationHelper.GetString(dr["VersionNumber"], null);
        version += " (" + GetUserDate(dr["ModifiedWhen"]) + ")";
        return version;
    }


    /// <summary>
    /// Returns string with converted DateTime parameters according to specific user timezone settings.
    /// </summary>
    private string GetUserDate(object dateTimeVal)
    {
        DateTime date = ValidationHelper.GetDateTime(dateTimeVal, DateTimeHelper.ZERO_TIME);
        return TimeZoneHelper.ConvertTimeZoneDateTime(date, serverTimeZone, usedTimeZone) + " " + TimeZoneHelper.GetUTCStringOffset(usedTimeZone);
    }


    /// <summary>
    /// Returns version number with date for given document.
    /// </summary>
    private string GetVersionNumber(int documentId, int version)
    {
        DataSet ds = VersionManager.GetDocumentHistory(documentId)
                                   .WhereEquals("VersionHistoryID", version)
                                   .TopN(1)
                                   .Columns("VersionNumber", "ModifiedWhen");

        if (DataHelper.DataSourceIsEmpty(ds))
        {
            return null;
        }

        return GetVersionNumber(ds.Tables[0].Rows[0]);
    }


    /// <summary>
    /// Adds the field to the form.
    /// </summary>
    /// <param name="node">Document node</param>
    /// <param name="compareNode">Document compare node</param>
    /// <param name="columnName">Column name</param>
    private void AddField(TreeNode node, TreeNode compareNode, string columnName)
    {
        FormFieldInfo ffi = null;
        if (fi != null)
        {
            ffi = fi.GetFormField(columnName);
        }

        TableCell leftValueCell = new TableCell();
        leftValueCell.EnableViewState = false;
        TableCell rightValueCell = new TableCell();
        rightValueCell.EnableViewState = false;
        TableCell labelCell = new TableCell();
        labelCell.EnableViewState = false;
        TextComparison compareLeft;
        TextComparison compareRight;
        bool loadValue = true;
        bool empty = true;
        bool allowLabel = true;
        bool compareMode = compareNode != null;

        // Get the caption
        if ((columnName == UNSORTED) || (ffi != null))
        {
            DocumentAttachment aiCompare = null;

            // Compare attachments
            if ((columnName == UNSORTED) || ((ffi != null) && (ffi.DataType == DocumentFieldDataType.DocAttachments)))
            {
                allowLabel = false;

                string title = String.Empty;
                if (columnName == UNSORTED)
                {
                    title = GetString("attach.unsorted") + ":";
                }
                else if (ffi != null)
                {
                    string caption = ffi.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, MacroContext.CurrentResolver);
                    title = (String.IsNullOrEmpty(caption) ? ffi.Name : caption) + ":";
                }

                // Prepare DataSource for original node
                loadValue = false;

                dsAttachments = new AttachmentsDataSource();
                dsAttachments.DocumentVersionHistoryID = versionHistoryId;
                if ((ffi != null) && (columnName != UNSORTED))
                {
                    dsAttachments.AttachmentGroupGUID = ffi.Guid;
                }
                dsAttachments.Path = node.NodeAliasPath;
                dsAttachments.CultureCode = LocalizationContext.PreferredCultureCode;
                SiteInfo si = SiteInfo.Provider.Get(node.NodeSiteID);
                dsAttachments.SiteName = si.SiteName;
                dsAttachments.OrderBy = "AttachmentOrder, AttachmentName, AttachmentHistoryID";
                dsAttachments.SelectedColumns = "AttachmentHistoryID, AttachmentGUID, AttachmentImageWidth, AttachmentImageHeight, AttachmentExtension, AttachmentName, AttachmentSize, AttachmentOrder, AttachmentTitle, AttachmentDescription";
                dsAttachments.IsLiveSite = false;
                dsAttachments.DataSource = null;
                dsAttachments.DataBind();

                // Get the attachments table
                attachments = GetAttachmentsTable((DataSet)dsAttachments.DataSource, versionHistoryId, node.DocumentID);

                // Prepare data source for compared node
                if (compareMode)
                {
                    dsAttachmentsCompare = new AttachmentsDataSource();
                    dsAttachmentsCompare.DocumentVersionHistoryID = versionCompare;
                    if ((ffi != null) && (columnName != UNSORTED))
                    {
                        dsAttachmentsCompare.AttachmentGroupGUID = ffi.Guid;
                    }
                    dsAttachmentsCompare.Path = compareNode.NodeAliasPath;
                    dsAttachmentsCompare.CultureCode = LocalizationContext.PreferredCultureCode;
                    dsAttachmentsCompare.SiteName = si.SiteName;
                    dsAttachmentsCompare.OrderBy = "AttachmentOrder, AttachmentName, AttachmentHistoryID";
                    dsAttachmentsCompare.SelectedColumns = "AttachmentHistoryID, AttachmentGUID, AttachmentImageWidth, AttachmentImageHeight, AttachmentExtension, AttachmentName, AttachmentSize, AttachmentOrder, AttachmentTitle, AttachmentDescription";
                    dsAttachmentsCompare.IsLiveSite = false;
                    dsAttachmentsCompare.DataSource = null;
                    dsAttachmentsCompare.DataBind();

                    // Get the table to compare
                    attachmentsCompare = GetAttachmentsTable((DataSet)dsAttachmentsCompare.DataSource, versionCompare, node.DocumentID);

                    // Switch the sides if older version is on the right
                    if (versionHistoryId > versionCompare)
                    {
                        Hashtable dummy = attachmentsCompare;
                        attachmentsCompare = attachments;
                        attachments = dummy;
                    }

                    // Add comparison
                    AddTableComparison(attachments, attachmentsCompare, "<strong>" + title + "</strong>", true, true);
                }
                else
                {
                    // Normal display
                    if (attachments.Count != 0)
                    {
                        bool first = true;

                        foreach (DictionaryEntry item in attachments)
                        {
                            string itemValue = ValidationHelper.GetString(item.Value, null);
                            if (!String.IsNullOrEmpty(itemValue))
                            {
                                leftValueCell = new TableCell();
                                labelCell = new TableCell();

                                if (first)
                                {
                                    labelCell.Text = "<strong>" + String.Format(title, item.Key) + "</strong>";
                                    first = false;
                                }
                                leftValueCell.Text = itemValue;

                                AddRow(labelCell, leftValueCell);
                            }
                        }
                    }
                }
            }
            // Compare single file attachment
            else if ((ffi != null) && (ffi.DataType == FieldDataType.File))
            {
                // Get the attachment
                var ai = DocumentHelper.GetAttachment(ValidationHelper.GetGuid(node.GetValue(columnName), Guid.Empty), versionHistoryId, false);

                if (compareMode)
                {
                    aiCompare = DocumentHelper.GetAttachment(ValidationHelper.GetGuid(compareNode.GetValue(columnName), Guid.Empty), versionCompare, false);
                }

                loadValue = false;

                // Prepare text comparison controls
                if ((ai != null) || (aiCompare != null))
                {
                    string textorig = null;
                    if (ai != null)
                    {
                        textorig = CreateAttachmentHtml(ai, versionHistoryId);
                    }
                    string textcompare = null;
                    if (aiCompare != null)
                    {
                        textcompare = CreateAttachmentHtml(aiCompare, versionCompare);
                    }

                    compareLeft = new TextComparison();
                    compareLeft.SynchronizedScrolling = false;
                    compareLeft.IgnoreHTMLTags = true;
                    compareLeft.ConsiderHTMLTagsEqual = true;
                    compareLeft.BalanceContent = false;

                    compareRight = new TextComparison();
                    compareRight.SynchronizedScrolling = false;
                    compareRight.IgnoreHTMLTags = true;

                    // Older version must be in the left column
                    if (versionHistoryId < versionCompare)
                    {
                        compareLeft.SourceText = textorig;
                        compareLeft.DestinationText = textcompare;
                    }
                    else
                    {
                        compareLeft.SourceText = textcompare;
                        compareLeft.DestinationText = textorig;
                    }

                    compareLeft.PairedControl = compareRight;
                    compareRight.RenderingMode = TextComparisonTypeEnum.DestinationText;

                    leftValueCell.Controls.Add(compareLeft);
                    rightValueCell.Controls.Add(compareRight);

                    // Add both cells
                    if (compareMode)
                    {
                        AddRow(labelCell, leftValueCell, rightValueCell);
                    }
                    // Add one cell only
                    else
                    {
                        leftValueCell.Controls.Clear();
                        Literal ltl = new Literal();
                        ltl.Text = textorig;
                        leftValueCell.Controls.Add(ltl);
                        AddRow(labelCell, leftValueCell);
                    }
                }
            }
        }
        if (columnName == SLUGS)
        {
            allowLabel = false;
            loadValue = false;

            if (slugs != null && slugs.Any())
            {
                bool first = true;

                foreach (var slug in slugs)
                {
                    leftValueCell = new TableCell();
                    labelCell = new TableCell();

                    if (first)
                    {
                        labelCell.Text = $"<strong>{GetString("content.ui.urlslugs")}:</strong>";
                        first = false;
                    }

                    leftValueCell.Text = $"{slug.Slug} ({CultureInfo.Provider.Get(slug.CultureCode)?.CultureName})";

                    AddRow(labelCell, leftValueCell);
                }
            }
        }

        if (allowLabel && String.IsNullOrEmpty(labelCell.Text))
        {
            labelCell.Text = "<strong>" + columnName + ":</strong>";
        }

        if (loadValue)
        {
            string textcompare = null;

            switch (columnName.ToLowerCSafe())
            {
                // Document content - display content of editable regions and editable web parts
                case "documentcontent":
                    EditableItems ei = new EditableItems();
                    ei.LoadContentXml(ValidationHelper.GetString(node.GetValue(columnName), String.Empty));

                    // Add text comparison control
                    if (compareMode)
                    {
                        EditableItems eiCompare = new EditableItems();
                        eiCompare.LoadContentXml(ValidationHelper.GetString(compareNode.GetValue(columnName), String.Empty));

                        // Create editable regions comparison
                        Hashtable hashtable;
                        Hashtable hashtableCompare;

                        // Older version must be in the left column
                        if (versionHistoryId < versionCompare)
                        {
                            hashtable = ei.EditableRegions;
                            hashtableCompare = eiCompare.EditableRegions;
                        }
                        else
                        {
                            hashtable = eiCompare.EditableRegions;
                            hashtableCompare = ei.EditableRegions;
                        }

                        // Add comparison
                        AddTableComparison(hashtable, hashtableCompare, "<strong>" + columnName + " ({0}):</strong>", false, false);

                        // Create editable webparts comparison
                        // Older version must be in the left column
                        if (versionHistoryId < versionCompare)
                        {
                            hashtable = ei.EditableWebParts;
                            hashtableCompare = eiCompare.EditableWebParts;
                        }
                        else
                        {
                            hashtable = eiCompare.EditableWebParts;
                            hashtableCompare = ei.EditableWebParts;
                        }

                        // Add comparison
                        AddTableComparison(hashtable, hashtableCompare, "<strong>" + columnName + " ({0}):</strong>", false, false);
                    }
                    // No compare node
                    else
                    {
                        // Editable regions
                        Hashtable hashtable = ei.EditableRegions;
                        if (hashtable.Count != 0)
                        {
                            foreach (DictionaryEntry region in hashtable)
                            {
                                string regionValue = ValidationHelper.GetString(region.Value, null);
                                if (!String.IsNullOrEmpty(regionValue))
                                {
                                    string regionKey = ValidationHelper.GetString(region.Key, null);

                                    leftValueCell = new TableCell();
                                    labelCell = new TableCell();

                                    labelCell.Text = "<strong>" + columnName + " (" + EditableItems.GetFirstKey(regionKey) + "):</strong>";
                                    leftValueCell.Text = HTMLHelper.HTMLEncode(HTMLHelper.StripTags(regionValue, false));

                                    AddRow(labelCell, leftValueCell);
                                }
                            }
                        }

                        // Editable web parts
                        hashtable = ei.EditableWebParts;
                        if (hashtable.Count != 0)
                        {
                            foreach (DictionaryEntry part in hashtable)
                            {
                                string partValue = ValidationHelper.GetString(part.Value, null);
                                if (!String.IsNullOrEmpty(partValue))
                                {
                                    string partKey = ValidationHelper.GetString(part.Key, null);
                                    leftValueCell = new TableCell();
                                    labelCell = new TableCell();

                                    labelCell.Text = "<strong>" + columnName + " (" + EditableItems.GetFirstKey(partKey) + "):</strong>";
                                    leftValueCell.Text = HTMLHelper.HTMLEncode(HTMLHelper.StripTags(partValue, false));

                                    AddRow(labelCell, leftValueCell);
                                }
                            }
                        }
                    }

                    break;

                case "documentpagebuilderwidgets":
                case "documentpagetemplateconfiguration":
                case "documentabtestconfiguration":
                    var sbWidgetsOriginal = new StringBuilder();
                    var sbWidgetsCompared = new StringBuilder();

                    string json = Convert.ToString(node.GetValue(columnName));

                    GenerateJsonFieldMarkup(ref sbWidgetsOriginal, json, true);

                    // Add text comparison control
                    if (compareMode)
                    {
                        string compareJson = Convert.ToString(compareNode.GetValue(columnName));
                        GenerateJsonFieldMarkup(ref sbWidgetsCompared, compareJson, false);
                    }

                    empty = sbWidgetsOriginal.Length == 0 && sbWidgetsCompared.Length == 0;

                    if (!empty)
                    {
                        leftValueCell.Text = GetValueForLeftCell(compareMode, sbWidgetsOriginal, sbWidgetsCompared);
                        rightValueCell.Text = GetValueForRightCell(compareMode, sbWidgetsOriginal, sbWidgetsCompared);
                    }
                    break;

                // Others, display the string value
                default:
                    // Shift date time values to user time zone
                    object origobject = node.GetValue(columnName);
                    string textorig;
                    if (origobject is DateTime)
                    {
                        textorig = TimeZoneHelper.ConvertToUserTimeZone(ValidationHelper.GetDateTime(origobject, DateTimeHelper.ZERO_TIME), true, CurrentUser, SiteContext.CurrentSite);
                    }
                    else
                    {
                        textorig = ValidationHelper.GetString(origobject, String.Empty);
                    }

                    // Add text comparison control
                    if (compareMode)
                    {
                        // Shift date time values to user time zone
                        object compareobject = compareNode.GetValue(columnName);
                        if (compareobject is DateTime)
                        {
                            textcompare = TimeZoneHelper.ConvertToUserTimeZone(ValidationHelper.GetDateTime(compareobject, DateTimeHelper.ZERO_TIME), true, CurrentUser, SiteContext.CurrentSite);
                        }
                        else
                        {
                            textcompare = ValidationHelper.GetString(compareobject, String.Empty);
                        }

                        compareLeft = new TextComparison();
                        compareLeft.SynchronizedScrolling = false;

                        compareRight = new TextComparison();
                        compareRight.SynchronizedScrolling = false;
                        compareRight.RenderingMode = TextComparisonTypeEnum.DestinationText;

                        // Older version must be in the left column
                        if (versionHistoryId < versionCompare)
                        {
                            compareLeft.SourceText = HTMLHelper.HTMLEncode(HTMLHelper.StripTags(textorig, false));
                            compareLeft.DestinationText = HTMLHelper.HTMLEncode(HTMLHelper.StripTags(textcompare, false));
                        }
                        else
                        {
                            compareLeft.SourceText = HTMLHelper.HTMLEncode(HTMLHelper.StripTags(textcompare, false));
                            compareLeft.DestinationText = HTMLHelper.HTMLEncode(HTMLHelper.StripTags(textorig, false));
                        }

                        compareLeft.PairedControl = compareRight;

                        if (Math.Max(compareLeft.SourceText.Length, compareLeft.DestinationText.Length) < 100)
                        {
                            compareLeft.BalanceContent = false;
                        }

                        leftValueCell.Controls.Add(compareLeft);
                        rightValueCell.Controls.Add(compareRight);
                    }
                    else
                    {
                        leftValueCell.Text = HTMLHelper.HTMLEncode(HTMLHelper.StripTags(textorig, false));
                    }

                    empty = (String.IsNullOrEmpty(textorig)) && (String.IsNullOrEmpty(textcompare));
                    break;
            }
        }

        if (!empty)
        {
            if (compareMode)
            {
                AddRow(labelCell, leftValueCell, rightValueCell);
            }
            else
            {
                AddRow(labelCell, leftValueCell);
            }
        }
    }


    private string GetValueForRightCell(bool compareMode, StringBuilder sbWidgetsOriginal, StringBuilder sbWidgetsCompared)
    {
        if (compareMode)
        {
            return versionHistoryId < versionCompare ? sbWidgetsCompared.ToString() : sbWidgetsOriginal.ToString();
        }

        return string.Empty;
    }


    private string GetValueForLeftCell(bool compareMode, StringBuilder sbWidgetsOriginal, StringBuilder sbWidgetsCompared)
    {
        if (!compareMode || versionHistoryId < versionCompare)
        {
            return sbWidgetsOriginal.ToString();
        }

        return sbWidgetsCompared.ToString();
    }


    private void GenerateJsonFieldMarkup(ref StringBuilder sbMarkup, string json, bool isLeft)
    {
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        var jConfiguration = JToken.Parse(json);

        sbMarkup.Append(@"<a href=""#"" onclick=""versionExpandWebParts(this)"" class=""", isLeft ? "vWpLeftLink" : "vWpRightLink", "\">" + GetString("General.Expand") + "</a>");
        sbMarkup.Append("<div style=\"display:none\" class=\"", isLeft ? "vWpLeft" : "vWpRight", "\">");

        JsonFieldToHtml(jConfiguration, ref sbMarkup);

        sbMarkup.Append("</div>");
    }


    private static void JsonFieldToHtml(JToken node, ref StringBuilder sb)
    {
        switch (node.Type)
        {
            case JTokenType.Object:
                foreach (var child in node.Children<JProperty>())
                {
                    JsonFieldToHtml(child, ref sb);
                }

                break;
            case JTokenType.Array:
                foreach (var child in node.Children())
                {
                    sb.Append(@"<div class=""json-array"">");
                    JsonFieldToHtml(child, ref sb);
                    sb.AppendLine("</div>");
                }

                break;
            case JTokenType.Property:
                {
                    var property = (JProperty)node;

                    sb.Append($"<div class='json-property'><i>{HTMLHelper.HTMLEncode(property.Name)}</i> : ");
                    JsonFieldToHtml(property.Value, ref sb);
                    sb.AppendLine("</div>");
                    break;
                }
            default:
                {
                    var propertyValue = (JValue)node;
                    var value = propertyValue.Value;
                    if (value != null)
                    {
                        sb.Append($"{HTMLHelper.HTMLEncode(value.ToString())}");
                    }

                    break;
                }
        }
    }


    /// <summary>
    /// Generates basic web parts markup
    /// </summary>
    /// <param name="sb">String builder</param>
    /// <param name="xml">Web part xml</param>
    private void GenerateWebPartsMarkup(ref StringBuilder sb, string xml, bool isLeft)
    {
        if (!String.IsNullOrEmpty(xml))
        {
            PageTemplateInstance inst = new PageTemplateInstance(xml);
            if (inst.WebPartZones.Count > 0)
            {
                sb.Append(@"<a href=""#"" onclick=""versionExpandWebParts(this)"" class=""", isLeft ? "vWpLeftLink" : "vWpRightLink", "\">" + GetString("General.Expand") + "</a>");
                sb.Append("<div style=\"display:none\" class=\"", isLeft ? "vWpLeft" : "vWpRight", "\">");
                foreach (WebPartZoneInstance zone in inst.WebPartZones)
                {
                    sb.Append("<b>Zone</b>: ", HTMLHelper.HTMLEncode(zone.ZoneID));
                    sb.Append(@"<div class=""VZoneEnvelope"">");
                    foreach (WebPartInstance webPart in zone.WebParts)
                    {
                        sb.Append(HTMLHelper.HTMLEncode(webPart.ControlID), " (", HTMLHelper.HTMLEncode(webPart.WebPartType), ")");
                        sb.Append(@"<div class=""VWebPartEnvelope"">");
                        foreach (DictionaryEntry item in webPart.Properties)
                        {
                            string itemValue = Convert.ToString(item.Value);
                            string itemKey = Convert.ToString(item.Key);
                            if (!String.IsNullOrEmpty(itemValue))
                            {
                                sb.Append(HTMLHelper.HTMLEncode(itemKey), " : ", HTMLHelper.HTMLEncode(itemValue), "<br />");
                            }
                        }
                        sb.Append("</div>");
                    }
                    sb.Append("</div>");
                }
                sb.Append("</div>");
            }
        }
    }


    /// <summary>
    /// Gets the hashtable of attachments from the DataSet.
    /// </summary>
    /// <param name="ds">Source DataSet</param>
    /// <param name="versionId">Version history ID</param>
    /// <param name="documentId">Document ID</param>
    protected Hashtable GetAttachmentsTable(DataSet ds, int versionId, int documentId)
    {
        Hashtable result = new Hashtable();
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                // Get attachment GUID
                Guid attachmentGuid = ValidationHelper.GetGuid(dr["AttachmentGUID"], Guid.Empty);
                result[attachmentGuid.ToString()] = CreateAttachmentHtml(new DataRowContainer(dr), versionId);
            }
        }

        return result;
    }


    /// <summary>
    /// Adds the table comparison for matching objects.
    /// </summary>
    /// <param name="hashtable">Left side table</param>
    /// <param name="hashtableCompare">Right side table</param>
    /// <param name="titleFormat">Title format string</param>
    /// <param name="includeAttachments">If true, the HTML code is kept (attachment comparison)</param>
    /// <param name="renderOnlyFirstTitle">If true, only first title is rendered</param>
    protected void AddTableComparison(Hashtable hashtable, Hashtable hashtableCompare, string titleFormat, bool includeAttachments, bool renderOnlyFirstTitle)
    {
        TableCell valueCell;
        TableCell valueCompare;
        TableCell labelCell;
        TextComparison comparefirst;
        TextComparison comparesecond;

        bool firstTitle = true;

        // Go through left column regions
        if (hashtable != null)
        {
            foreach (DictionaryEntry entry in hashtable)
            {
                object value = entry.Value;
                if (value != null)
                {
                    // Initialize comparators
                    comparefirst = new TextComparison();
                    comparefirst.SynchronizedScrolling = false;

                    comparesecond = new TextComparison();
                    comparesecond.SynchronizedScrolling = false;
                    comparesecond.RenderingMode = TextComparisonTypeEnum.DestinationText;

                    if (includeAttachments)
                    {
                        comparefirst.IgnoreHTMLTags = true;
                        comparefirst.ConsiderHTMLTagsEqual = true;

                        comparesecond.IgnoreHTMLTags = true;
                    }

                    comparefirst.PairedControl = comparesecond;

                    // Initialize cells
                    valueCell = new TableCell();
                    valueCompare = new TableCell();
                    labelCell = new TableCell();

                    string key = ValidationHelper.GetString(entry.Key, null);
                    string strValue = ValidationHelper.GetString(value, null);
                    if (firstTitle || !renderOnlyFirstTitle)
                    {
                        labelCell.Text = String.Format(titleFormat, EditableItems.GetFirstKey(key));
                        firstTitle = false;
                    }

                    comparefirst.SourceText = includeAttachments ? strValue : HTMLHelper.HTMLEncode(HTMLHelper.StripTags(strValue, false));

                    if ((hashtableCompare != null) && hashtableCompare.Contains(key))
                    {
                        // Compare to the existing other version
                        string compareKey = ValidationHelper.GetString(hashtableCompare[key], null);
                        comparefirst.DestinationText = includeAttachments ? compareKey : HTMLHelper.HTMLEncode(HTMLHelper.StripTags(compareKey, false));
                        hashtableCompare.Remove(key);
                    }
                    else
                    {
                        // Compare to an empty string
                        comparefirst.DestinationText = String.Empty;
                    }

                    // Do not balance content if too short
                    if (includeAttachments)
                    {
                        comparefirst.BalanceContent = false;
                    }
                    else if (Math.Max(comparefirst.SourceText.Length, comparefirst.DestinationText.Length) < 100)
                    {
                        comparefirst.BalanceContent = false;
                    }

                    // Create cell comparison
                    valueCell.Controls.Add(comparefirst);
                    valueCompare.Controls.Add(comparesecond);

                    AddRow(labelCell, valueCell, valueCompare);
                }
            }
        }

        // Go through right column regions which left
        if (hashtableCompare != null)
        {
            foreach (DictionaryEntry entry in hashtableCompare)
            {
                object value = entry.Value;
                if (value != null)
                {
                    // Initialize comparators
                    comparefirst = new TextComparison();
                    comparefirst.SynchronizedScrolling = false;

                    comparesecond = new TextComparison();
                    comparesecond.SynchronizedScrolling = false;
                    comparesecond.RenderingMode = TextComparisonTypeEnum.DestinationText;

                    comparefirst.PairedControl = comparesecond;

                    if (includeAttachments)
                    {
                        comparefirst.IgnoreHTMLTags = true;
                        comparefirst.ConsiderHTMLTagsEqual = true;

                        comparesecond.IgnoreHTMLTags = true;
                    }

                    // Initialize cells
                    valueCell = new TableCell();
                    valueCompare = new TableCell();
                    labelCell = new TableCell();

                    if (firstTitle || !renderOnlyFirstTitle)
                    {
                        labelCell.Text = String.Format(titleFormat, EditableItems.GetFirstKey(ValidationHelper.GetString(entry.Key, null)));
                        firstTitle = false;
                    }

                    comparefirst.SourceText = String.Empty;
                    string strValue = ValidationHelper.GetString(value, null);
                    comparefirst.DestinationText = includeAttachments ? strValue : HTMLHelper.HTMLEncode(HTMLHelper.StripTags(strValue, false));

                    if (includeAttachments)
                    {
                        comparefirst.BalanceContent = false;
                    }
                    else if (Math.Max(comparefirst.SourceText.Length, comparefirst.DestinationText.Length) < 100)
                    {
                        comparefirst.BalanceContent = false;
                    }

                    // Create cell comparison
                    valueCell.Controls.Add(comparefirst);
                    valueCompare.Controls.Add(comparesecond);

                    AddRow(labelCell, valueCell, valueCompare);
                }
            }
        }
    }


    /// <summary>
    /// Returns new TableRow with CSS class.
    /// </summary>
    private TableRow CreateRow(string cssClass, bool isHeader)
    {
        TableRow newRow = isHeader ? new TableHeaderRow() { TableSection = TableRowSection.TableHeader } : new TableRow();

        // Set CSS
        if (!String.IsNullOrEmpty(cssClass))
        {
            newRow.CssClass = cssClass;
        }

        return newRow;
    }


    /// <summary>
    /// Creates 2 column table.
    /// </summary>
    /// <param name="labelCell">Cell with label</param>
    /// <param name="valueCell">Cell with content</param>
    /// <param name="cssClass">CSS class</param>
    /// <param name="isHeader">Indicates if created row is header row</param>
    /// <returns>Returns TableRow object</returns>
    private void AddRow(TableCell labelCell, TableCell valueCell, string cssClass = null, bool isHeader = false)
    {
        TableRow newRow = CreateRow(cssClass, isHeader);

        newRow.Cells.Add(labelCell);
        valueCell.Width = new Unit(100, UnitType.Percentage);
        newRow.Cells.Add(valueCell);

        tblDocument.Rows.Add(newRow);
    }


    /// <summary>
    /// Creates 3 column table. Older version must be always in the left column.
    /// </summary>
    /// <param name="labelCell">Cell with label</param>
    /// <param name="valueCell">Cell with content</param>
    /// <param name="compareCell">Cell with compare content</param>
    /// <param name="switchSides">Indicates if cells should be switched to match corresponding version (False by default)</param>
    /// <param name="cssClass">CSS class</param>
    /// <param name="isHeader">Indicates if created row is header row (False by default)</param>
    /// <returns>Returns TableRow object</returns>
    private void AddRow(TableCell labelCell, TableCell valueCell, TableCell compareCell, bool switchSides = false, string cssClass = null, bool isHeader = false)
    {
        TableRow newRow = CreateRow(cssClass, isHeader);

        newRow.Cells.Add(labelCell);

        const int cellWidth = 40;
        // Switch sides to match version
        if (switchSides)
        {
            // Older version must be in the left column
            if (versionHistoryId < versionCompare)
            {
                valueCell.Width = new Unit(cellWidth, UnitType.Percentage);
                newRow.Cells.Add(valueCell);

                compareCell.Width = new Unit(cellWidth, UnitType.Percentage);
                newRow.Cells.Add(compareCell);
            }
            else
            {
                compareCell.Width = new Unit(cellWidth, UnitType.Percentage);
                newRow.Cells.Add(compareCell);

                valueCell.Width = new Unit(cellWidth, UnitType.Percentage);
                newRow.Cells.Add(valueCell);
            }
        }
        // Do not switch sides
        else
        {
            valueCell.Width = new Unit(cellWidth, UnitType.Percentage);
            newRow.Cells.Add(valueCell);

            compareCell.Width = new Unit(cellWidth, UnitType.Percentage);
            newRow.Cells.Add(compareCell);
        }

        tblDocument.Rows.Add(newRow);
    }


    /// <summary>
    /// Creates attachment string.
    /// </summary>
    private string CreateAttachmentHtml(IDataContainer dc, int versionId)
    {
        if (dc == null)
        {
            return null;
        }

        // Get attachment GUID
        Guid attachmentGuid = ValidationHelper.GetGuid(dc.GetValue("AttachmentGUID"), Guid.Empty);

        // Get attachment extension
        string attachmentExt = ValidationHelper.GetString(dc.GetValue("AttachmentExtension"), null);

        // Get link for attachment
        string attName = ValidationHelper.GetString(dc.GetValue("AttachmentName"), null);
        string attachmentUrl = ApplicationUrlHelper.ResolveUIUrl(AttachmentURLProvider.GetAttachmentUrl(attachmentGuid, attName, versionId));

        // Ensure correct URL
        attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "sitename", SiteContext.CurrentSiteName);

        // Optionally trim attachment name
        string attachmentName = TextHelper.LimitLength(attName, 90);
        bool isImage = ImageHelper.IsImage(attachmentExt);

        // Tooltip
        string tooltip = null;
        if (isImage)
        {
            int attachmentWidth = ValidationHelper.GetInteger(dc.GetValue("AttachmentImageWidth"), 0);
            if (attachmentWidth > 300)
            {
                attachmentWidth = 300;
            }
            tooltip = "onmouseout=\"UnTip()\" onmouseover=\"TipImage(" + attachmentWidth + ", '" + URLHelper.AddParameterToUrl(attachmentUrl, "width", "300") + "', " + ScriptHelper.GetString(HTMLHelper.HTMLEncode(attachmentName)) + ")\"";
        }

        string attachmentSize = DataHelper.GetSizeString(ValidationHelper.GetLong(dc.GetValue("AttachmentSize"), 0));
        string title = ValidationHelper.GetString(dc.GetValue("AttachmentTitle"), string.Empty);
        string description = ValidationHelper.GetString(dc.GetValue("AttachmentDescription"), string.Empty);

        // Icon
        var additional = new StringBuilder();
        additional.Append(tooltip, "onclick=\"javascript: window.open(", ScriptHelper.GetString(attachmentUrl), "); return false;\" style=\"cursor: pointer;\"");

        var sb = new StringBuilder();
        sb.Append(UIHelper.GetFileIcon(Page, attachmentExt, tooltip: HTMLHelper.HTMLEncode(attachmentName), additionalAttributes: additional.ToString()));
        sb.Append(" ", HTMLHelper.HTMLEncode(attachmentName), " (", HTMLHelper.HTMLEncode(attachmentSize), ")<br />");
        sb.Append("<table class=\"table-blank\"><tr><td><strong>", GetString("general.title"), ":</strong></td><td>", HTMLHelper.HTMLEncode(title), "</td></tr>");
        sb.Append("<tr><td><strong>", GetString("general.description"), ":</strong></td><td>", HTMLHelper.HTMLEncode(description), "</td></tr></table>");

        return sb.ToString();
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Dropdown list selection changed.
    /// </summary>
    private void drpCompareTo_SelectedIndexChanged(object sender, EventArgs e)
    {
        string url = RequestContext.CurrentURL;

        url = URLHelper.RemoveParameterFromUrl(url, "rollbackok");
        url = drpCompareTo.SelectedIndex == 0 ? URLHelper.RemoveParameterFromUrl(url, "compareHistoryId") : URLHelper.AddParameterToUrl(url, "compareHistoryId", drpCompareTo.SelectedValue);

        URLHelper.Redirect(url);
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Raises event postback event.
    /// </summary>
    /// <param name="eventArgument">Argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        int rollbackVersionId = ValidationHelper.GetInteger(eventArgument, 0);

        if (rollbackVersionId > 0)
        {
            if (Node == null)
            {
                return;
            }

            if (CheckedOutByUserID > 0)
            {
                // Document is checked out
                ShowError(GetString("VersionProperties.CannotRollbackCheckedOut"));
            }
            else
            {
                // Check permissions
                bool canApprove = WorkflowManager.CheckStepPermissions(Node, WorkflowActionEnum.Approve);

                if (!canApprove || !CanModify)
                {
                    ShowError(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName())));
                }
                else
                {
                    try
                    {
                        // Rollback version
                        int newVersionHistoryId = VersionManager.RollbackVersion(rollbackVersionId);

                        ShowConfirmation(GetString("VersionProperties.RollbackOK"));

                        string url = RequestContext.CurrentURL;

                        // Add URL parameters
                        url = URLHelper.AddParameterToUrl(url, "versionHistoryId", newVersionHistoryId.ToString());
                        url = URLHelper.AddParameterToUrl(url, "compareHistoryId", versionCompare.ToString());
                        url = URLHelper.AddParameterToUrl(url, "rollbackok", "1");

                        // Prepare URL
                        url = ScriptHelper.GetString(UrlResolver.ResolveUrl(url), true);

                        // Prepare script for refresh parent window and this dialog
                        StringBuilder builder = new StringBuilder();
                        builder.Append("if (wopener != null) {\n");
                        builder.Append("if (wopener.RefreshTree != null) {wopener.RefreshTree(", Node.NodeParentID, ", ", Node.NodeID, "); }");
                        builder.Append("wopener.document.location.replace(wopener.document.location);}\n");
                        builder.Append("window.document.location.replace(" + url + ");");

                        string script = ScriptHelper.GetScript(builder.ToString());
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshAndReload", script);
                    }
                    catch (Exception ex)
                    {
                        ShowError(GetString("versionproperties.rollbackerror"));
                        Service.Resolve<IEventLogService>().LogException("Content", "ROLLBACKVERSION", ex);
                    }
                }
            }

            // Display form if error occurs
            if (error)
            {
                ReloadData();
            }
        }
    }

    #endregion
}
