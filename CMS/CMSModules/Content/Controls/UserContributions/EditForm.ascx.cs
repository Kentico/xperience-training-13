using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_Controls_UserContributions_EditForm : CMSUserControl
{
    #region "Variables"

    /// <summary>
    /// On after approve event.
    /// </summary>
    public event EventHandler OnAfterApprove = null;

    /// <summary>
    /// On after reject event.
    /// </summary>
    public event EventHandler OnAfterReject = null;


    /// <summary>
    /// On after delete event.
    /// </summary>
    public event EventHandler OnAfterDelete = null;

    /// <summary>
    /// Data properties variable.
    /// </summary>
    private readonly CMSDataProperties mDataProperties = new CMSDataProperties();

    /// <summary>
    /// Indicates if the form has been loaded.
    /// </summary>
    private bool mFormLoaded = false;

    private DataClassInfo ci = null;

    #endregion


    #region "Document properties"

    /// <summary>
    /// Indicates if the control is used on a live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            docMan.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Document manager
    /// </summary>
    public override ICMSDocumentManager DocumentManager
    {
        get
        {
            return docMan;
        }
    }



    /// <summary>
    /// Culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CultureCode"], mDataProperties.CultureCode);
        }
        set
        {
            ViewState["CultureCode"] = value;
            menuElem.CultureCode = value;
            DocumentManager.CultureCode = value;
        }
    }


    /// <summary>
    /// Site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SiteName"], mDataProperties.SiteName);
        }
        set
        {
            ViewState["SiteName"] = value;
            DocumentManager.SiteName = value;
        }
    }


    /// <summary>
    /// Indicates if check-in/check-out functionality is automatic
    /// </summary>
    protected bool AutoCheck
    {
        get
        {
            return DocumentManager.AutoCheck;
        }
    }


    /// <summary>
    /// Gets Workflow manager instance.
    /// </summary>
    protected WorkflowManager WorkflowManager
    {
        get
        {
            return DocumentManager.WorkflowManager;
        }
    }


    /// <summary>
    /// Gets Version manager instance.
    /// </summary>
    protected VersionManager VersionManager
    {
        get
        {
            return DocumentManager.VersionManager;
        }
    }


    /// <summary>
    /// Tree provider instance.
    /// </summary>
    protected TreeProvider TreeProvider
    {
        get
        {
            return DocumentManager.Tree;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Returns true if new document mode.
    /// </summary>
    public bool NewDocument
    {
        get
        {
            return (Action.ToLowerCSafe() == "new");
        }
    }


    /// <summary>
    /// Returns true if new culture mode.
    /// </summary>
    public bool NewCulture
    {
        get
        {
            return (Action.ToLowerCSafe() == "newculture");
        }
    }


    /// <summary>
    /// Returns true in delete mode.
    /// </summary>
    public bool Delete
    {
        get
        {
            return (Action.ToLowerCSafe() == "delete");
        }
    }


    /// <summary>
    /// Returns true in edit mode.
    /// </summary>
    public bool Edit
    {
        get
        {
            return (Action.ToLowerCSafe() == "edit");
        }
    }


    /// <summary>
    /// Node ID.
    /// </summary>
    public int NodeID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["NodeID"], 0);
        }
        set
        {
            ViewState["NodeID"] = value;
            menuElem.NodeID = value;
            DocumentManager.NodeID = value;
        }
    }


    /// <summary>
    /// Document node.
    /// </summary>
    public TreeNode Node
    {
        get
        {
            return DocumentManager.Node;
        }
    }


    /// <summary>
    /// Form Action (mode).
    /// </summary>
    public string Action
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Action"], "");
        }
        set
        {
            ViewState["Action"] = value;
            if (NewDocument)
            {
                DocumentManager.Mode = FormModeEnum.Insert;
            }
            else if (NewCulture)
            {
                DocumentManager.Mode = FormModeEnum.InsertNewCultureVersion;
            }
            else
            {
                DocumentManager.Mode = FormModeEnum.Update;
            }
        }
    }


    /// <summary>
    /// Class ID.
    /// </summary>
    public int ClassID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ClassID"], 0);
        }
        set
        {
            ViewState["ClassID"] = value;
            DocumentManager.NewNodeClassID = value;
        }
    }


    /// <summary>
    /// Alternative form name.
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["AlternativeFormName"], null);
        }
        set
        {
            ViewState["AlternativeFormName"] = value;
        }
    }


    /// <summary>
    /// Form validation error message.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ValidationErrorMessage"], null);
        }
        set
        {
            ViewState["ValidationErrorMessage"] = value;
        }
    }
    

    /// <summary>
    /// Owner ID.
    /// </summary>
    public int OwnerID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["OwnerID"], 0);
        }
        set
        {
            ViewState["OwnerID"] = value;
        }
    }


    /// <summary>
    /// List of allowed child classes separated by semicolon.
    /// </summary>
    public string AllowedChildClasses
    {
        get
        {
            return ValidationHelper.GetString(ViewState["AllowedChildClasses"], "");
        }
        set
        {
            ViewState["AllowedChildClasses"] = value;
        }
    }


    /// <summary>
    /// Document ID to use for default data of new culture version.
    /// </summary>
    public int CopyDefaultDataFromDocumentID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["CopyDefaultDataFromDocumentID"], 0);
        }
        set
        {
            ViewState["CopyDefaultDataFromDocumentID"] = value;
            DocumentManager.SourceDocumentID = value;
        }
    }


    /// <summary>
    /// If true, form allows deleting the document.
    /// </summary>
    public bool AllowDelete
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["AllowDelete"], true);
        }
        set
        {
            ViewState["AllowDelete"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether document permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["CheckPermissions"], false);
        }
        set
        {
            ViewState["CheckPermissions"] = value;
            menuElem.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether document type permissions are required to create new document.
    /// </summary>
    public bool CheckDocPermissionsForInsert
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["CheckDocPermissionsForInsert"], true);
        }
        set
        {
            ViewState["CheckDocPermissionsForInsert"] = value;
        }
    }


    /// <summary>
    /// Determines whether to use progress script.
    /// </summary>
    public bool UseProgressScript
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["UseProgressScript"], true);
        }
        set
        {
            ViewState["UseProgressScript"] = value;
        }
    }


    /// <summary>
    /// Editing form.
    /// </summary>
    public CMSForm CMSForm
    {
        get
        {
            return formElem;
        }
    }


    /// <summary>
    /// Determines whether the save is allowed (form have to be loaded first).
    /// </summary>
    public bool AllowSave
    {
        get
        {
            return menuElem.AllowSave;
        }
    }


    /// <summary>
    /// Indicates whether activity logging is enabled.
    /// </summary>
    public bool LogActivity
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
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

            menuElem.StopProcessing = value;
            formElem.StopProcessing = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        // Reload data
        ReloadData(false);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        DocumentManager.LocalDocumentPanel = pnlDoc;
        DocumentManager.LocalMessagesPlaceHolder = formElem.MessagesPlaceHolder;

        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;
        DocumentManager.OnLoadData += DocumentManager_OnLoadData;

        formElem.StopProcessing = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register external data bound event handler for UniGrid
        gridClass.OnExternalDataBound += gridClass_OnExternalDataBound;
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    /// <param name="forceReload">Forces nested CMSForm to reload if true</param>
    public void ReloadData(bool forceReload)
    {
        if (StopProcessing)
        {
            return;
        }

        if (!mFormLoaded || forceReload)
        {
            // Check License
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.UserContributions);

            if (!StopProcessing)
            {
                // Set document manager mode
                if (NewDocument)
                {
                    DocumentManager.Mode = FormModeEnum.Insert;
                    DocumentManager.ParentNodeID = NodeID;
                    DocumentManager.NewNodeClassID = ClassID;
                    DocumentManager.CultureCode = CultureCode;
                    DocumentManager.SiteName = SiteName;
                }
                else if (NewCulture)
                {
                    DocumentManager.Mode = FormModeEnum.InsertNewCultureVersion;
                    DocumentManager.NodeID = NodeID;
                    DocumentManager.CultureCode = CultureCode;
                    DocumentManager.SiteName = SiteName;
                    DocumentManager.SourceDocumentID = CopyDefaultDataFromDocumentID;
                }
                else
                {
                    DocumentManager.Mode = FormModeEnum.Update;
                    DocumentManager.NodeID = NodeID;
                    DocumentManager.SiteName = SiteName;
                    DocumentManager.CultureCode = CultureCode;
                }

                ScriptHelper.RegisterDialogScript(Page);

                titleElem.TitleText = String.Empty;

                pnlSelectClass.Visible = false;
                pnlEdit.Visible = false;
                pnlInfo.Visible = false;
                pnlNewCulture.Visible = false;
                pnlDelete.Visible = false;

                // If node found, init the form

                if (NewDocument || (Node != null))
                {
                    // Delete action
                    if (Delete)
                    {
                        // Delete document
                        pnlDelete.Visible = true;

                        titleElem.TitleText = GetString("Content.DeleteTitle");
                        chkAllCultures.Text = GetString("ContentDelete.AllCultures");
                        chkDestroy.Text = GetString("ContentDelete.Destroy");

                        lblQuestion.Text = GetString("ContentDelete.Question");
                        btnYes.Text = GetString("general.yes");
                        // Prevent button double-click
                        btnYes.Attributes.Add("onclick", string.Format("document.getElementById('{0}').disabled=true;this.disabled=true;{1};", btnNo.ClientID, ControlsHelper.GetPostBackEventReference(btnYes, string.Empty, true, false)));
                        btnNo.Text = GetString("general.no");

                        DataSet culturesDS = CultureSiteInfoProvider.GetSiteCultures(SiteName);
                        if ((DataHelper.DataSourceIsEmpty(culturesDS)) || (culturesDS.Tables[0].Rows.Count <= 1))
                        {
                            chkAllCultures.Visible = false;
                            chkAllCultures.Checked = true;
                        }

                        if (Node.IsLink)
                        {
                            titleElem.TitleText = GetString("Content.DeleteTitleLink") + " \"" + HTMLHelper.HTMLEncode(Node.NodeName) + "\"";
                            lblQuestion.Text = GetString("ContentDelete.QuestionLink");
                            chkAllCultures.Checked = true;
                            plcCheck.Visible = false;
                        }
                        else
                        {
                            titleElem.TitleText = GetString("Content.DeleteTitle") + " \"" + HTMLHelper.HTMLEncode(Node.NodeName) + "\"";
                        }
                    }
                    // New document or edit action
                    else
                    {
                        if (NewDocument)
                        {
                            titleElem.TitleText = GetString("Content.NewTitle");
                        }

                        // Document type selection
                        if (NewDocument && (ClassID <= 0))
                        {
                            // Use parent node
                            TreeNode parentNode = DocumentManager.ParentNode;
                            if (parentNode != null)
                            {
                                // Select document type
                                pnlSelectClass.Visible = true;

                                // Apply document type scope
                                var whereCondition = DocumentTypeScopeInfoProvider.GetScopeClassWhereCondition(parentNode);

                                var parentClassId = ValidationHelper.GetInteger(parentNode.GetValue("NodeClassID"), 0);
                                var siteId = SiteInfoProvider.GetSiteID(SiteName);

                                // Get the allowed child classes
                                DataSet ds = AllowedChildClassInfoProvider.GetAllowedChildClasses(parentClassId, siteId)
                                    .Where(whereCondition)
                                    .OrderBy("ClassID")
                                    .Columns("ClassName", "ClassDisplayName", "ClassID");
                                
                                ArrayList deleteRows = new ArrayList();

                                if (!DataHelper.DataSourceIsEmpty(ds))
                                {
                                    // Get the unwanted classes
                                    string allowed = AllowedChildClasses.Trim().ToLowerCSafe();
                                    if (!string.IsNullOrEmpty(allowed))
                                    {
                                        allowed = String.Format(";{0};", allowed);
                                    }

                                    var userInfo = MembershipContext.AuthenticatedUser;
                                    string className = null;
                                    // Check if the user has 'Create' permission per Content
                                    bool isAuthorizedToCreateInContent = userInfo.IsAuthorizedPerResource("CMS.Content", "Create");
                                    bool hasNodeAllowCreate = (userInfo.IsAuthorizedPerTreeNode(parentNode, NodePermissionsEnum.Create) == AuthorizationResultEnum.Allowed);
                                    foreach (DataRow dr in ds.Tables[0].Rows)
                                    {
                                        className = DataHelper.GetStringValue(dr, "ClassName", String.Empty).ToLowerCSafe();
                                        // Document type is not allowed or user hasn't got permission, remove it from the data set
                                        if ((!string.IsNullOrEmpty(allowed) && (!allowed.Contains(";" + className + ";"))) ||
                                            (CheckPermissions && CheckDocPermissionsForInsert && !(isAuthorizedToCreateInContent || userInfo.IsAuthorizedPerClassName(className, "Create") || (userInfo.IsAuthorizedPerClassName(className, "CreateSpecific") && hasNodeAllowCreate))))
                                        {
                                            deleteRows.Add(dr);
                                        }
                                    }

                                    // Remove the rows
                                    foreach (DataRow dr in deleteRows)
                                    {
                                        ds.Tables[0].Rows.Remove(dr);
                                    }
                                }

                                // Check if some classes are available
                                if (!DataHelper.DataSourceIsEmpty(ds))
                                {
                                    // If number of classes is more than 1 display them in grid
                                    if (ds.Tables[0].Rows.Count > 1)
                                    {
                                        ds.Tables[0].DefaultView.Sort = "ClassDisplayName";
                                        lblError.Visible = false;
                                        lblInfo.Visible = true;
                                        lblInfo.Text = GetString("Content.NewInfo");

                                        DataSet sortedResult = new DataSet();
                                        sortedResult.Tables.Add(ds.Tables[0].DefaultView.ToTable());
                                        gridClass.DataSource = sortedResult;
                                        gridClass.ReloadData();
                                    }
                                    // else show form of the only class
                                    else
                                    {
                                        ClassID = DataHelper.GetIntValue(ds.Tables[0].Rows[0], "ClassID");
                                        ReloadData(true);
                                        return;
                                    }
                                }
                                else
                                {
                                    // Display error message
                                    lblError.Visible = true;
                                    lblError.Text = GetString("Content.NoAllowedChildDocuments");
                                    lblInfo.Visible = false;
                                    gridClass.Visible = false;
                                }
                            }
                            else
                            {
                                pnlInfo.Visible = true;
                                lblFormInfo.Text = GetString("EditForm.DocumentNotFound");
                            }
                        }
                        // Insert or update of a document
                        else
                        {
                            // Display the form
                            pnlEdit.Visible = true;

                            btnDelete.Attributes.Add("style", "display: none;");
                            btnRefresh.Attributes.Add("style", "display: none;");

                            // CMSForm initialization
                            formElem.NodeID = Node.NodeID;
                            formElem.SiteName = SiteName;
                            formElem.CultureCode = CultureCode;
                            formElem.ValidationErrorMessage = HTMLHelper.HTMLEncode(ValidationErrorMessage);
                            formElem.IsLiveSite = IsLiveSite;
                            
                            // Set the form mode
                            if (NewDocument)
                            {
                                ci = DataClassInfoProvider.GetDataClassInfo(ClassID);
                                if (ci == null)
                                {
                                    throw new Exception(String.Format("[CMSAdminControls/EditForm.aspx]: Class ID '{0}' not found.", ClassID));
                                }

                                string classDisplayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ci.ClassDisplayName));
                                titleElem.TitleText = GetString("Content.NewTitle") + ": " + classDisplayName;

                                // Set document owner
                                formElem.OwnerID = OwnerID;
                                formElem.FormMode = FormModeEnum.Insert;
                                string newClassName = ci.ClassName;
                                string newFormName = newClassName + ".default";
                                if (!String.IsNullOrEmpty(AlternativeFormName))
                                {
                                    // Set the alternative form full name
                                    formElem.AlternativeFormFullName = GetAltFormFullName(ci.ClassName);
                                }
                                if (newFormName.ToLowerCSafe() != formElem.FormName.ToLowerCSafe())
                                {
                                    formElem.FormName = newFormName;
                                }
                            }
                            else if (NewCulture)
                            {
                                formElem.FormMode = FormModeEnum.InsertNewCultureVersion;
                                // Default data document ID
                                formElem.CopyDefaultDataFromDocumentId = CopyDefaultDataFromDocumentID;

                                ci = DataClassInfoProvider.GetDataClassInfo(Node.NodeClassName);
                                formElem.FormName = Node.NodeClassName + ".default";
                                if (!String.IsNullOrEmpty(AlternativeFormName))
                                {
                                    // Set the alternative form full name
                                    formElem.AlternativeFormFullName = GetAltFormFullName(ci.ClassName);
                                }
                            }
                            else
                            {
                                formElem.FormMode = FormModeEnum.Update;
                                ci = DataClassInfoProvider.GetDataClassInfo(Node.NodeClassName);
                                formElem.FormName = String.Empty;
                                if (!String.IsNullOrEmpty(AlternativeFormName))
                                {
                                    // Set the alternative form full name
                                    formElem.AlternativeFormFullName = GetAltFormFullName(ci.ClassName);
                                }
                            }

                            // Allow the CMSForm
                            formElem.StopProcessing = false;

                            ReloadForm();
                            formElem.LoadForm(true);
                        }
                    }
                }
                // New culture version
                else
                {
                    // Switch to new culture version mode
                    DocumentManager.Mode = FormModeEnum.InsertNewCultureVersion;
                    DocumentManager.NodeID = NodeID;
                    DocumentManager.CultureCode = CultureCode;
                    DocumentManager.SiteName = SiteName;

                    if (Node != null)
                    {
                        // Offer a new culture creation
                        pnlNewCulture.Visible = true;

                        titleElem.TitleText = GetString("Content.NewCultureVersionTitle") + " (" + HTMLHelper.HTMLEncode(LocalizationContext.PreferredCultureCode) + ")";
                        lblNewCultureInfo.Text = GetString("ContentNewCultureVersion.Info");
                        radCopy.Text = GetString("ContentNewCultureVersion.Copy");
                        radEmpty.Text = GetString("ContentNewCultureVersion.Empty");

                        radCopy.Attributes.Add("onclick", "ShowSelection();");
                        radEmpty.Attributes.Add("onclick", "ShowSelection()");

                        AddScript(
                            "function ShowSelection() { \n" +
                            "   if (document.getElementById('" + radCopy.ClientID + "').checked) { document.getElementById('divCultures').style.display = 'block'; } \n" +
                            "   else { document.getElementById('divCultures').style.display = 'none'; } \n" +
                            "} \n"
                            );

                        btnOk.Text = GetString("ContentNewCultureVersion.Create");

                        // Load culture versions
                        SiteInfo si = SiteInfo.Provider.Get(Node.NodeSiteID);
                        if (si != null)
                        {
                            lstCultures.Items.Clear();

                            DataSet nodes = TreeProvider.SelectNodes(si.SiteName, Node.NodeAliasPath, TreeProvider.ALL_CULTURES, false, null, null, null, 1, false);
                            foreach (DataRow nodeCulture in nodes.Tables[0].Rows)
                            {
                                ListItem li = new ListItem();
                                li.Text = CultureInfo.Provider.Get(nodeCulture["DocumentCulture"].ToString()).CultureName;
                                li.Value = nodeCulture["DocumentID"].ToString();
                                lstCultures.Items.Add(li);
                            }
                            if (lstCultures.Items.Count > 0)
                            {
                                lstCultures.SelectedIndex = 0;
                            }
                        }
                    }
                    else
                    {
                        pnlInfo.Visible = true;
                        lblFormInfo.Text = GetString("EditForm.DocumentNotFound");
                    }
                }
            }
            // Set flag that the form is loaded
            mFormLoaded = true;
        }
    }


    /// <summary>
    /// Unigrid external databound.
    /// </summary>
    protected object gridClass_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Display link to class type
            case "classdisplayname":
                {
                    DataRowView row = (DataRowView)parameter;

                    var btn = new LinkButton();
                    btn.CssClass = "UserContributionNewClass";
                    btn.CommandArgument = ValidationHelper.GetString(row["ClassID"], "0");
                    btn.Command += btnClass_Command;

                    var img = new Literal();
                    var className = Convert.ToString(row["ClassName"]);
                    var doc = DataClassInfoProvider.GetDataClassInfo(className);
                    var iconClass = doc.GetValue("ClassIconClass", String.Empty);
                    img.Text = UIHelper.GetDocumentTypeIcon(Page, className, iconClass);

                    var lbl = new Label();
                    string classDisplayName = Convert.ToString(row["ClassDisplayName"]);
                    lbl.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(classDisplayName));

                    btn.Controls.Add(img);
                    btn.Controls.Add(lbl);
                    return btn;
                }
        }

        return null;
    }


    private void ReloadForm()
    {
        // Enable the CMSForm
        formElem.Enabled = true;

        if ((Node != null) && !NewDocument && !NewCulture)
        {
            // Check the permissions
            if (CheckPermissions)
            {
                // Check read permissions
                if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
                {
                    RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName())));
                }
                // Check modify permissions
                else if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
                {
                    formElem.Enabled = false;
                    DocumentManager.DocumentInfo = String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName()));
                }
            }
        }

        // Reload edit menu
        menuElem.ShowDelete = AllowDelete && Edit;
        menuElem.CheckPermissions = CheckPermissions;
    }


    /// <summary>
    /// Adds the alert message to the output request window.
    /// </summary>
    /// <param name="message">Message to display</param>
    private void AddAlert(string message)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), message.GetHashCode().ToString(), ScriptHelper.GetAlertScript(message));
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    private void AddScript(string script)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), script.GetHashCode().ToString(), ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Save new or existing document.
    /// </summary>
    public bool SaveDocument()
    {
        return DocumentManager.SaveDocument();
    }


    void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        switch (e.ActionName)
        {
            case ComponentEvents.SAVE:
                // Set the edit mode
                if (Node != null)
                {
                    NodeID = Node.NodeID;
                    Action = "edit";
                    ReloadData(true);
                }

                AddScript("changed=false;");
                break;

            case DocumentComponentEvents.APPROVE:
                RaiseOnAfterApprove();
                break;

            case DocumentComponentEvents.REJECT:
                RaiseOnAfterReject();
                break;

            case DocumentComponentEvents.UNDO_CHECKOUT:
                formElem.LoadForm(true);

                // Reload the values in the form
                formElem.LoadControlValues();
                break;

            default:
                break;
        }

        ReloadForm();
    }


    void DocumentManager_OnLoadData(object sender, DocumentManagerEventArgs e)
    {
        formElem.LoadControlValues();
    }


    /// <summary>
    /// Refresh button click event handler.
    /// </summary>
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check permission to modify document
            if (!CheckPermissions || (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed))
            {
                // Ensure version for later detection whether node is published
                VersionManager.EnsureVersion(Node, Node.IsPublished);

                // Move to edit step
                WorkflowManager.MoveToFirstStep(Node);

                // Reload form
                ReloadForm();
                if (DocumentManager.SaveChanges)
                {
                    ScriptHelper.RegisterStartupScript(this, typeof(string), "moveToEditStepChange", ScriptHelper.GetScript("Changed();"));
                }
            }
        }
    }


    /// <summary>
    /// New class selection click event handler.
    /// </summary>
    protected void btnClass_Command(object sender, CommandEventArgs e)
    {
        int newClassId = ValidationHelper.GetInteger(e.CommandArgument, 0);
        if (newClassId > 0)
        {
            ClassID = newClassId;
            ReloadData(true);
        }
    }


    /// <summary>
    /// OK button click event handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        Action = "newculture";
        CopyDefaultDataFromDocumentID = radCopy.Checked ? ValidationHelper.GetInteger(lstCultures.SelectedValue, 0) : 0;
        DocumentManager.ClearNode();

        ReloadData(true);
    }


    /// <summary>
    /// Yes button click event handler.
    /// </summary>
    protected void btnYes_Click(object sender, EventArgs e)
    {
        // Prepare the where condition
        string where = "NodeID = " + NodeID;

        // Get the documents
        DataSet ds = null;
        if (chkAllCultures.Checked)
        {
            ds = TreeProvider.SelectNodes(SiteName, "/%", TreeProvider.ALL_CULTURES, true, null, where, null, -1, false);
        }
        else
        {
            ds = TreeProvider.SelectNodes(SiteName, "/%", CultureCode, false, null, where, null, -1, false);
        }

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Get node alias
            var nodeAlias = DataHelper.GetStringValue(ds.Tables[0].Rows[0], "NodeAlias", string.Empty);
            // Get parent alias path

            var parentAliasPath = TreePathUtils.GetParentPath(DataHelper.GetStringValue(ds.Tables[0].Rows[0], "NodeAliasPath", string.Empty));

            // Delete the documents
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var aliasPath = ValidationHelper.GetString(dr["NodeAliasPath"], string.Empty);
                var culture = ValidationHelper.GetString(dr["DocumentCulture"], string.Empty);
                var className = ValidationHelper.GetString(dr["ClassName"], string.Empty);

                // Get the node
                var treeNode = TreeProvider.SelectSingleNode(SiteName, aliasPath, culture, false, className, false);

                if (treeNode != null)
                {
                    // Check delete permissions
                    var hasUserDeletePermission = !CheckPermissions || IsUserAuthorizedToDeleteDocument(treeNode, chkDestroy.Checked);

                    if (hasUserDeletePermission)
                    {
                        // Delete the document
                        try
                        {
                            DocumentHelper.DeleteDocument(treeNode, TreeProvider, chkAllCultures.Checked, chkDestroy.Checked);
                        }
                        catch (Exception ex)
                        {
                            var logData = new EventLogData(EventTypeEnum.Error, "Content", "DELETEDOC")
                            {
                                EventDescription = EventLogProvider.GetExceptionLogMessage(ex),
                                EventUrl = RequestContext.RawURL,
                                UserID = MembershipContext.AuthenticatedUser.UserID,
                                UserName = MembershipContext.AuthenticatedUser.UserName,
                                NodeID = treeNode.NodeID,
                                DocumentName = treeNode.GetDocumentName(),
                                IPAddress = RequestContext.UserHostAddress,
                                SiteID = SiteContext.CurrentSiteID
                            };
                            
                            Service.Resolve<IEventLogService>().LogEvent(logData);
                            AddAlert(GetString("ContentRequest.DeleteFailed") + ": " + ex.Message);
                            return;
                        }
                    }
                    // Access denied - not authorized to delete the document
                    else
                    {
                        AddAlert(String.Format(GetString("cmsdesk.notauthorizedtodeletedocument"), HTMLHelper.HTMLEncode(treeNode.GetDocumentName())));
                        return;
                    }
                }
                else
                {
                    AddAlert(GetString("ContentRequest.ErrorMissingSource"));
                    return;
                }
            }

            RaiseOnAfterDelete();
        }
        else
        {
            AddAlert(GetString("DeleteDocument.CultureNotExists"));
        }
    }


    /// <summary>
    /// No button click event handler.
    /// </summary>
    protected void btnNo_Click(object sender, EventArgs e)
    {
        Action = "edit";
        ReloadData(true);
    }


    /// <summary>
    /// Delete button click event handler.
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        Action = "delete";
        ReloadData(true);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (Visible)
        {
            if (pnlEdit.Visible)
            {
                // Register other scripts which are necessary in edit mode
                if (UseProgressScript)
                {
                    ScriptHelper.RegisterLoader(Page);
                }

                // Register script
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Delete_" + menuElem.ClientID + "(NodeID) { " + Page.ClientScript.GetPostBackEventReference(btnDelete, null) + "; } \n");
                sb.AppendLine("function " + formElem.ClientID + "_RefreshForm(){" + Page.ClientScript.GetPostBackEventReference(btnRefresh, "") + " }");

                // Register the scripts
                AddScript(sb.ToString());

                ScriptHelper.RegisterBootstrapScripts(Page);

                if (formElem.FieldControls != null)
                {
                    // Disable maximize plugin on HTML editors                
                    var htmlControls = formElem.FormInformation.GetFieldsWithControl(FormFieldControlName.HTMLAREA);

                    foreach (FormFieldInfo field in htmlControls)
                    {
                        Control control = formElem.FieldControls[field.Name];
                        CMSHtmlEditor htmlEditor = ControlsHelper.GetChildControl(control, typeof(CMSHtmlEditor)) as CMSHtmlEditor;
                        if (htmlEditor != null)
                        {
                            htmlEditor.Node = DocumentContext.CurrentDocument;
                            htmlEditor.RemovePlugins.Add("maximize");
                        }
                    }
                }

                if (!NewDocument && !NewCulture)
                {
                    formElem.Enabled = AllowSave;
                }
            }
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Checks whether the user is authorized to delete document.
    /// </summary>
    /// <param name="treeNode">Document node</param>
    /// <param name="deleteDocHistory">Delete document history?</param>
    protected bool IsUserAuthorizedToDeleteDocument(TreeNode treeNode, bool deleteDocHistory)
    {
        bool isAuthorized = true;

        var currentUser = MembershipContext.AuthenticatedUser;

        // Check delete permission
        if (currentUser.IsAuthorizedPerDocument(treeNode, new[] { NodePermissionsEnum.Delete, NodePermissionsEnum.Read }) == AuthorizationResultEnum.Allowed)
        {
            if (deleteDocHistory)
            {
                // Check destroy permission
                if (currentUser.IsAuthorizedPerDocument(treeNode, NodePermissionsEnum.Destroy) != AuthorizationResultEnum.Allowed)
                {
                    isAuthorized = false;
                }
            }
        }
        else
        {
            isAuthorized = false;
        }

        return isAuthorized;
    }


    /// <summary>
    /// Returns alternative form name in full version - 'ClassName.AltFormCodeName'.
    /// </summary>
    /// <param name="className">Class name</param>
    private string GetAltFormFullName(string className)
    {
        if (!string.IsNullOrEmpty(AlternativeFormName) && !string.IsNullOrEmpty(className) && !AlternativeFormName.StartsWithCSafe(className))
        {
            if (AlternativeFormName.Contains("."))
            {
                // Remove class name if it is different from class name in parameter
                AlternativeFormName = AlternativeFormName.Remove(0, AlternativeFormName.LastIndexOfCSafe(".") + 1);
            }
            return className + "." + AlternativeFormName;
        }

        return AlternativeFormName;
    }


    /// <summary>
    /// Raises the OnAfterApprove event.
    /// </summary>
    private void RaiseOnAfterApprove()
    {
        if (OnAfterApprove != null)
        {
            OnAfterApprove(this, null);
        }
    }


    /// <summary>
    /// Raises the OnAfterReject event.
    /// </summary>
    private void RaiseOnAfterReject()
    {
        if (OnAfterReject != null)
        {
            OnAfterReject(this, null);
        }
    }


    /// <summary>
    /// Raises the OnAfterDelete event.
    /// </summary>
    private void RaiseOnAfterDelete()
    {
        if (OnAfterDelete != null)
        {
            OnAfterDelete(this, null);
        }
    }

    #endregion
}