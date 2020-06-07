using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_RecycleBin_Controls_RecycleBin : CMSUserControl
{
    #region "Private variables"

    private bool mIsSingleSite = true;
    private string mSiteName = String.Empty;
    private string mCurrentCulture = CultureHelper.DefaultUICultureCode;
    private CurrentUserInfo mCurrentUser;
    private string mDocumentType = String.Empty;
    private bool mRestrictUsers = true;
    private SiteInfo mSelectedSite;

    private string mOrderBy = "VersionDeletedWhen DESC";
    private string mDocumentAge = String.Empty;
    private string mDocumentName = String.Empty;
    private string mItemsPerPage = String.Empty;
    private What currentWhat = default(What);

    private CMSAbstractRecycleBinFilterControl filter;

    #endregion


    #region "Structures"

    /// <summary>
    /// Structure that holds settings for async operations.
    /// </summary>
    private struct BinSettingsContainer
    {
        public CurrentUserInfo User
        {
            get;
            private set;
        }

        public What CurrentWhat
        {
            get;
            private set;
        }


        public List<string> SelectedItems
        {
            get;
            set;
        }


        public BinSettingsContainer(CurrentUserInfo user, What what)
            : this()
        {
            User = user;
            CurrentWhat = what;
        }
    }

    #endregion


    #region "Enumerations"

    protected enum Action
    {
        SelectAction = 0,
        Restore = 1,
        Delete = 2
    }

    protected enum What
    {
        SelectedDocuments = 0,
        AllDocuments = 1
    }

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
    /// Depends on site name set.
    /// </summary>
    public bool IsSingleSite
    {
        get
        {
            return mIsSingleSite;
        }
        set
        {
            mIsSingleSite = value;
        }
    }


    /// <summary>
    /// Gets number of document age conditions.
    /// </summary>
    protected int AgeModifiersCount
    {
        get
        {
            int count = 0;
            if (!String.IsNullOrEmpty(DocumentAge))
            {
                string[] ages = DocumentAge.Split(';');
                if (ages.Length == 2)
                {
                    if (ValidationHelper.GetInteger(ages[1], 0) > 0)
                    {
                        count++;
                    }

                    if (ValidationHelper.GetInteger(ages[0], 0) > 0)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }


    /// <summary>
    /// Site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return mSiteName;
        }
        set
        {
            mSiteName = value;
            SiteInfo siteInfo = SiteInfo.Provider.Get(mSiteName);
            if (siteInfo != null)
            {
                mSelectedSite = siteInfo;
            }
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    public string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }


    /// <summary>
    /// Current Info.
    /// </summary>
    public string CurrentInfo
    {
        get
        {
            return ctlAsyncLog.ProcessData.Information;
        }
        set
        {
            ctlAsyncLog.ProcessData.Information = value;
        }
    }


    /// <summary>
    /// Order by for grid.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return mOrderBy;
        }

        set
        {
            mOrderBy = value;
        }
    }


    /// <summary>
    /// Filter by document type.
    /// </summary>
    public string DocumentType
    {
        get
        {
            return mDocumentType;
        }

        set
        {
            mDocumentType = value;
        }
    }


    /// <summary>
    /// Items per page.
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return mItemsPerPage;
        }
        set
        {
            mItemsPerPage = value;
        }
    }


    /// <summary>
    /// Age of documents in days.
    /// </summary>
    public string DocumentAge
    {
        get
        {
            return mDocumentAge;
        }
        set
        {
            mDocumentAge = value;
        }
    }


    /// <summary>
    /// Document name for grid filter.
    /// </summary>
    public string DocumentName
    {
        get
        {
            return mDocumentName;
        }
        set
        {
            mDocumentName = value;
        }
    }


    /// <summary>
    /// Indicates if restrictions should be applied on users displayed in filter.
    /// </summary>
    public bool RestrictUsers
    {
        get
        {
            return mRestrictUsers;
        }
        set
        {
            mRestrictUsers = value;
        }
    }


    /// <summary>
    /// Indicates if date time filter will be displayed.
    /// </summary>
    public bool DisplayDateTimeFilter
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ugRecycleBin.OnFilterFieldCreated += ugRecycleBin_OnFilterFieldCreated;
        ugRecycleBin.HideFilterButton = true;
        ugRecycleBin.LoadGridDefinition();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            ugRecycleBin.StopProcessing = true;
            return;
        }

        // Register the main CMS script
        ScriptHelper.RegisterCMS(Page);

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Set current UI culture
        mCurrentCulture = CultureHelper.PreferredUICultureCode;

        // Get current user info
        mCurrentUser = MembershipContext.AuthenticatedUser;

        // Set current site ID to filter
        filter.SiteID = (mSelectedSite != null) ? mSelectedSite.SiteID : UniSelector.US_ALL_RECORDS;
        PrepareGrid();

        if (!RequestHelper.IsCallback())
        {
            ControlsHelper.RegisterPostbackControl(btnOk);
            // Create action script
            StringBuilder actionScript = new StringBuilder();
            actionScript.AppendLine("function PerformAction(selectionFunction, selectionField, dropId, validationLabel, whatId)");
            actionScript.AppendLine("{");
            actionScript.AppendLine("   var selectionFieldElem = document.getElementById(selectionField);");
            actionScript.AppendLine("   var label = document.getElementById(validationLabel);");
            actionScript.AppendLine("   var items = selectionFieldElem.value;");
            actionScript.AppendLine("   var whatDrp = document.getElementById(whatId);");
            actionScript.AppendLine("   var allDocs = whatDrp.value == '" + (int)What.AllDocuments + "';");
            actionScript.AppendLine("   var action = document.getElementById(dropId).value;");
            actionScript.AppendLine("   if (action == '" + (int)Action.SelectAction + "')");
            actionScript.AppendLine("   {");
            actionScript.AppendLine("       label.innerHTML = " + ScriptHelper.GetLocalizedString("massaction.selectsomeaction") + ";");
            actionScript.AppendLine("       label.style.display = 'block';");
            actionScript.AppendLine("       return false;");
            actionScript.AppendLine("   }");
            actionScript.AppendLine("   if(!eval(selectionFunction) || allDocs)");
            actionScript.AppendLine("   {");
            actionScript.AppendLine("       var confirmed = false;");
            actionScript.AppendLine("       var confMessage = '';");
            actionScript.AppendLine("       switch(action)");
            actionScript.AppendLine("       {");
            actionScript.AppendLine("           case '" + (int)Action.Restore + "':");
            actionScript.AppendLine("               confMessage = " + ScriptHelper.GetLocalizedString("recyclebin.confirmrestores") + ";");
            actionScript.AppendLine("               break;");
            actionScript.AppendLine("           case '" + (int)Action.Delete + "':");
            actionScript.AppendLine("               confMessage = allDocs ?  " + ScriptHelper.GetLocalizedString("recyclebin.confirmemptyrecbin") + " : " + ScriptHelper.GetLocalizedString("recyclebin.confirmdeleteselected") + ";");
            actionScript.AppendLine("               break;");
            actionScript.AppendLine("       }");
            actionScript.AppendLine("       return confirm(confMessage);");
            actionScript.AppendLine("   }");
            actionScript.AppendLine("   else");
            actionScript.AppendLine("   {");
            actionScript.AppendLine("       label.innerHTML = " + ScriptHelper.GetLocalizedString("documents.selectdocuments") + ";");
            actionScript.AppendLine("       label.style.display = 'block';");
            actionScript.AppendLine("       return false;");
            actionScript.AppendLine("   }");
            actionScript.AppendLine("}");
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "recycleBinScript", ScriptHelper.GetScript(actionScript.ToString()));

            // Set page size
            int itemsPerPage = ValidationHelper.GetInteger(ItemsPerPage, 0);
            if ((itemsPerPage > 0) && !RequestHelper.IsPostBack())
            {
                ugRecycleBin.Pager.DefaultPageSize = itemsPerPage;
            }

            // Add action to button
            btnOk.OnClientClick = "return PerformAction('" + ugRecycleBin.GetCheckSelectionScript() + "','" + ugRecycleBin.GetSelectionFieldClientID() + "','" + drpAction.ClientID + "','" + lblValidation.ClientID + "', '" + drpWhat.ClientID + "');";

            // Initialize dropdown lists
            if (!RequestHelper.IsPostBack())
            {
                drpAction.Items.Add(new ListItem(GetString("general." + Action.Restore), Convert.ToInt32(Action.Restore).ToString()));
                drpAction.Items.Add(new ListItem(GetString("recyclebin.destroyhint"), Convert.ToInt32(Action.Delete).ToString()));

                drpWhat.Items.Add(new ListItem(GetString("contentlisting." + What.SelectedDocuments), Convert.ToInt32(What.SelectedDocuments).ToString()));
                drpWhat.Items.Add(new ListItem(GetString("contentlisting." + What.AllDocuments), Convert.ToInt32(What.AllDocuments).ToString()));
            }

            // Register the dialog script
            ScriptHelper.RegisterDialogScript(Page);

            // Register script for viewing versions
            string viewVersionScript = "function ViewVersion(versionHistoryId) {modalDialog('" + ResolveUrl("~/CMSModules/RecycleBin/Pages/ViewVersion.aspx") + "?noCompare=1&versionHistoryId=' + versionHistoryId, 'contentversion', 900, 600);}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "viewVersionScript", ScriptHelper.GetScript(viewVersionScript));

            string error = QueryHelper.GetString("displayerror", String.Empty);
            if (error != String.Empty)
            {
                ShowError(GetString("recyclebin.errorsomenotdestroyed"));
            }

            // Set visibility of panels
            pnlLog.Visible = false;
        }
        else
        {
            ugRecycleBin.StopProcessing = true;
        }

        // Initialize events
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;
    }


    private void PrepareGrid()
    {
        string where = null;
        int siteId = (mSelectedSite != null) ? mSelectedSite.SiteID : 0;
        DateTime modifiedFrom = DateTimeHelper.ZERO_TIME;
        DateTime modifiedTo = DateTimeHelper.ZERO_TIME;
        SetDocumentAge(ref modifiedFrom, ref modifiedTo);
        bool modifiedFromSet = (modifiedFrom != DateTimeHelper.ZERO_TIME);
        bool modifiedToSet = (modifiedTo != DateTimeHelper.ZERO_TIME);

        // Prepare the parameters
        QueryDataParameters parameters = new QueryDataParameters();

        // Filter by selected site
        if (siteId > 0)
        {
            parameters.Add("@SiteID", siteId);
            where = "NodeSiteID = @SiteID";
        }

        if (modifiedFromSet)
        {
            parameters.Add("@FROM", modifiedFrom);
        }
        if (modifiedToSet)
        {
            parameters.Add("@TO", modifiedTo);
        }

        if (modifiedFromSet)
        {
            where = SqlHelper.AddWhereCondition(where, "ModifiedWhen >= @FROM ");
        }
        if (modifiedToSet)
        {
            where = SqlHelper.AddWhereCondition(where, "ModifiedWhen <= @TO ");
        }

        ugRecycleBin.QueryParameters = parameters;
        ugRecycleBin.WhereCondition = GetWhereCondition(where);
        ugRecycleBin.HideControlForZeroRows = false;
        ugRecycleBin.OnExternalDataBound += ugRecycleBin_OnExternalDataBound;
        ugRecycleBin.OnAction += ugRecycleBin_OnAction;
        ugRecycleBin.OrderBy = OrderBy;
    }


    protected void ugRecycleBin_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
    {
        filter = filterDefinition.ValueControl as CMSAbstractRecycleBinFilterControl;
        if (filter != null)
        {
            filter.DisplayDateTimeFilter = DisplayDateTimeFilter;

            // If filter is set
            if (filter.FilterIsSet)
            {
                ugRecycleBin.ZeroRowsText = GetString("unigrid.filteredzerorowstext");
            }
            else
            {
                ugRecycleBin.ZeroRowsText = IsSingleSite ? GetString("RecycleBin.NoDocuments") : GetString("RecycleBin.Empty");
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Hide multiple actions if grid is empty
        pnlFooter.Visible = ugRecycleBin.GridView.Rows.Count > 0;

        // Hide site name column
        ugRecycleBin.GridView.Columns[5].Visible = (mSelectedSite == null) && !IsSingleSite;

        base.OnPreRender(e);
    }

    #endregion


    #region "Restoring & destroying methods"

    /// <summary>
    /// Restores documents selected in UniGrid.
    /// </summary>
    private void Restore(object parameter)
    {
        try
        {
            // Begin log
            AddLog(ResHelper.GetString("Recyclebin.RestoringDocuments", mCurrentCulture));
            BinSettingsContainer settings = (BinSettingsContainer)parameter;
            DataSet recycleBin = null;
            switch (settings.CurrentWhat)
            {
                case What.AllDocuments:
                    DateTime modifiedFrom = DateTimeHelper.ZERO_TIME;
                    DateTime modifiedTo = DateTimeHelper.ZERO_TIME;
                    SetDocumentAge(ref modifiedFrom, ref modifiedTo);
                    recycleBin = VersionHistoryInfoProvider.GetRecycleBin((mSelectedSite != null) ? mSelectedSite.SiteID : 0, 0, GetWhereCondition(filter.WhereCondition), "VersionNodeAliasPath ASC", -1, "VersionHistoryID, VersionNodeAliasPath, VersionDocumentName", modifiedFrom, modifiedTo);
                    break;

                case What.SelectedDocuments:
                    // Restore selected documents
                    var toRestore = settings.SelectedItems;
                    if ((toRestore != null) && (toRestore.Count > 0))
                    {
                        string where = new WhereCondition().WhereIn("VersionHistoryID", toRestore).ToString(true);
                        recycleBin = VersionHistoryInfoProvider.GetRecycleBin(0, 0, where, "VersionNodeAliasPath ASC", -1, "VersionHistoryID, VersionNodeAliasPath, VersionDocumentName");
                    }
                    break;
            }

            if (!DataHelper.DataSourceIsEmpty(recycleBin))
            {
                RestoreDataSet(settings.User, recycleBin);
            }
        }
        catch (ThreadAbortException ex)
        {
            if (CMSThread.Stopped(ex))
            {
                // When canceled
                CurrentInfo = ResHelper.GetString("Recyclebin.RestorationCanceled", mCurrentCulture);
                AddLog(CurrentInfo);
            }
            else
            {
                // Log error
                LogException("RESTOREDOC", ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            LogException("RESTOREDOC", ex);
        }
    }


    /// <summary>
    /// Restores set of given version histories.
    /// </summary>
    /// <param name="currentUserInfo">Current user info</param>
    /// <param name="recycleBin">DataSet with nodes to restore</param>
    private void RestoreDataSet(CurrentUserInfo currentUserInfo, DataSet recycleBin)
    {
        // Result flags
        bool resultOK = true;
        bool permissionsOK = true;

        if (!DataHelper.DataSourceIsEmpty(recycleBin))
        {
            TreeProvider tree = new TreeProvider(currentUserInfo);
            tree.AllowAsyncActions = false;
            VersionManager versionManager = VersionManager.GetInstance(tree);
            // Restore all documents
            foreach (DataRow dataRow in recycleBin.Tables[0].Rows)
            {
                var path = ValidationHelper.GetString(dataRow["VersionNodeAliasPath"], string.Empty);
                var name = string.Format("{0} ({1})", ValidationHelper.GetString(dataRow["VersionDocumentName"], string.Empty), path);

                // Restore document
                int versionHistoryId = ValidationHelper.GetInteger(dataRow["VersionHistoryID"], 0);
                if (versionHistoryId > 0)
                {
                    var versionHistoryInfo = GetVersionHistoryInfo(versionHistoryId);

                    // Check permissions
                    if (!IsAuthorizedPerDocument(versionHistoryInfo, "Create", mCurrentUser))
                    {
                        CurrentError = String.Format(ResHelper.GetString("Recyclebin.RestorationFailedPermissions", mCurrentCulture), name);
                        AddLog(CurrentError);
                        permissionsOK = false;
                    }
                    else
                    {
                        var treeNode = versionManager.RestoreDocument(versionHistoryId);
                        if (treeNode != null)
                        {
                            AddLog(ResHelper.GetString("general.document", mCurrentCulture) + "'" + path + "'");
                        }
                        else
                        {
                            // Set result flag
                            if (resultOK)
                            {
                                resultOK = false;
                            }
                        }
                    }
                }
            }
        }

        if (resultOK && permissionsOK)
        {
            CurrentInfo = ResHelper.GetString("Recyclebin.RestorationOK", mCurrentCulture);
            AddLog(CurrentInfo);
        }
        else
        {
            CurrentError = ResHelper.GetString("Recyclebin.RestorationFailed", mCurrentCulture);
            if (!permissionsOK)
            {
                CurrentError += "<br />" + ResHelper.GetString("recyclebin.errorsomenotrestored", mCurrentCulture);
            }
            AddLog(CurrentError);
        }
    }


    /// <summary>
    /// Empties recycle bin.
    /// </summary>
    private void EmptyBin(object parameter)
    {
        // Begin log
        AddLog(ResHelper.GetString("Recyclebin.EmptyingBin", mCurrentCulture));

        BinSettingsContainer settings = (BinSettingsContainer)parameter;
        CurrentUserInfo currentUserInfo = settings.User;

        string where = null;
        DateTime modifiedFrom = DateTimeHelper.ZERO_TIME;
        DateTime modifiedTo = DateTimeHelper.ZERO_TIME;

        switch (settings.CurrentWhat)
        {
            case What.AllDocuments:
                SetDocumentAge(ref modifiedFrom, ref modifiedTo);
                where = GetWhereCondition(filter.WhereCondition);
                break;

            case What.SelectedDocuments:
                // Restore selected documents
                var toRestore = settings.SelectedItems;
                if ((toRestore != null) && (toRestore.Count > 0))
                {
                    where = new WhereCondition().WhereIn("VersionHistoryID", toRestore).ToString(true);
                }
                break;
        }

        DataSet recycleBin = VersionHistoryInfoProvider.GetRecycleBin((mSelectedSite != null) ? mSelectedSite.SiteID : 0, 0, @where, "VersionNodeAliasPath ASC", -1, null, modifiedFrom, modifiedTo);

        try
        {
            if (!DataHelper.DataSourceIsEmpty(recycleBin))
            {
                TreeProvider tree = new TreeProvider(currentUserInfo);
                tree.AllowAsyncActions = false;
                VersionManager versionManager = VersionManager.GetInstance(tree);

                foreach (DataRow dr in recycleBin.Tables[0].Rows)
                {
                    int versionHistoryId = Convert.ToInt32(dr["VersionHistoryID"]);
                    var versionHistoryInfo = GetVersionHistoryInfo(versionHistoryId);
                    var name = string.Format("{0} ({1})", ValidationHelper.GetString(dr["VersionDocumentName"], string.Empty), ValidationHelper.GetString(dr["VersionNodeAliasPath"], string.Empty));
                    // Check permissions
                    if (!IsAuthorizedPerDocument(versionHistoryInfo, "Destroy", mCurrentUser))
                    {
                        CurrentError = String.Format(ResHelper.GetString("Recyclebin.DestructionFailedPermissions", mCurrentCulture), name);
                        AddLog(CurrentError);
                    }
                    else
                    {
                        AddLog(ResHelper.GetString("general.document", mCurrentCulture) + "'" + HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["VersionNodeAliasPath"], string.Empty)) + "'");
                        // Destroy the version
                        versionManager.DestroyDocumentHistory(ValidationHelper.GetInteger(dr["DocumentID"], 0));
                        LogContext.LogEventToCurrent(EventType.INFORMATION, "Content", "DESTROYDOC", string.Format(ResHelper.GetString("Recyclebin.documentdestroyed"), name), RequestContext.RawURL, mCurrentUser.UserID, mCurrentUser.UserName, 0, null, RequestContext.UserHostAddress, SiteContext.CurrentSiteID, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);
                    }
                }
                if (!String.IsNullOrEmpty(CurrentError))
                {
                    CurrentError = ResHelper.GetString("recyclebin.errorsomenotdestroyed", mCurrentCulture);
                    AddLog(CurrentError);
                }
                else
                {
                    CurrentInfo = ResHelper.GetString("recyclebin.destroyok", mCurrentCulture);
                    AddLog(CurrentInfo);
                }
            }
        }
        catch (ThreadAbortException ex)
        {
            if (!CMSThread.Stopped(ex))
            {
                // Log error
                LogException("DESTROYDOC", ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            LogException("DESTROYDOC", ex);
        }
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        HandlePossibleErrors();
    }


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        HandlePossibleErrors();
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        HandlePossibleErrors();
    }
    

    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    private void HandlePossibleErrors()
    {
        TerminateCallbacks();
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        if (!String.IsNullOrEmpty(CurrentInfo))
        {
            ShowConfirmation(CurrentInfo);
        }
        ugRecycleBin.ResetSelection();
    }


    private void TerminateCallbacks()
    {
        string terminatingScript = ScriptHelper.GetScript("var __pendingCallbacks = new Array();");
        ScriptHelper.RegisterStartupScript(this, typeof(string), "terminatePendingCallbacks", terminatingScript);
    }


    /// <summary>
    /// Runs async thread.
    /// </summary>
    /// <param name="action">Method to run</param>
    protected void RunAsync(AsyncAction action)
    {
        pnlLog.Visible = true;
        //pnlContent.Visible = false;

        CurrentError = string.Empty;
        CurrentInfo = string.Empty;
        
        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Button handling"

    protected void btnOk_OnClick(object sender, EventArgs e)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        
        int actionValue = ValidationHelper.GetInteger(drpAction.SelectedValue, 0);
        Action action = (Action)actionValue;

        int whatValue = ValidationHelper.GetInteger(drpWhat.SelectedValue, 0);
        currentWhat = (What)whatValue;

        BinSettingsContainer binSettings = new BinSettingsContainer(mCurrentUser, currentWhat);

        switch (currentWhat)
        {
            case What.SelectedDocuments:
                if (ugRecycleBin.SelectedItems.Count <= 0)
                {
                    return;
                }

                binSettings.SelectedItems = ugRecycleBin.SelectedItems;
                break;
        }

        switch (action)
        {
            case Action.Restore:
                {
                    ctlAsyncLog.TitleText = GetString("Recyclebin.RestoringDocuments");
                    RunAsync(p => Restore(binSettings));
                }
                break;

            case Action.Delete:
                {
                    ctlAsyncLog.TitleText = GetString("recyclebin.emptyingbin");
                    RunAsync(p => EmptyBin(binSettings));
                }
                break;
        }
    }

    #endregion


    #region "Grid events"

    protected object ugRecycleBin_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName != null ? sourceName.ToLowerInvariant() : null;
        switch (sourceName)
        {
            case "view":
                CMSGridActionButton btnView = sender as CMSGridActionButton;
                if (btnView != null)
                {
                    GridViewRow row = (GridViewRow)parameter;
                    object siteId = DataHelper.GetDataRowViewValue((DataRowView)row.DataItem, "NodeSiteID");
                    SiteInfo siteInfo = SiteInfo.Provider.Get(ValidationHelper.GetInteger(siteId, 0));
                    if (siteInfo != null)
                    {
                        if (siteInfo.Status == SiteStatusEnum.Stopped)
                        {
                            btnView.Enabled = false;
                        }
                    }
                }
                break;

            case "nodesiteid":
                {
                    int siteId = ValidationHelper.GetInteger(parameter, 0);
                    SiteInfo siteInfo = SiteInfo.Provider.Get(siteId);
                    return (siteInfo != null) ? HTMLHelper.HTMLEncode(siteInfo.DisplayName) : string.Empty;
                }

            case "documentname":
                string documentName = ValidationHelper.GetString(parameter, null);
                return string.IsNullOrEmpty(documentName) ? GetString("general.na") : HTMLHelper.HTMLEncode(documentName);
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void ugRecycleBin_OnAction(string actionName, object actionArgument)
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        VersionManager versionManager = VersionManager.GetInstance(tree);
        int versionHistoryId = ValidationHelper.GetInteger(actionArgument, 0);
        var versionHistoryInfo = GetVersionHistoryInfo(versionHistoryId);

        switch (actionName.ToLowerInvariant())
        {
            case "restore":
                RestoreDocumentVersion(versionHistoryInfo, versionManager);
                break;

            case "destroy":
                DestroyDocumentHistory(versionHistoryInfo, versionManager);
                break;
        }

        ugRecycleBin.ResetSelection();
    }


    private void RestoreDocumentVersion(VersionHistoryInfo versionHistoryInfo, VersionManager versionManager)
    {
        try
        {
            if (IsAuthorizedPerDocument(versionHistoryInfo, "Create", mCurrentUser))
            {
                versionManager.RestoreDocument(versionHistoryInfo.VersionHistoryID);
                ShowConfirmation(GetString("Recyclebin.RestorationOK"));
            }
            else
            {
                var name = string.Format("{0} ({1})", versionHistoryInfo.VersionDocumentName, versionHistoryInfo.VersionNodeAliasPath);
                ShowError(String.Format(ResHelper.GetString("Recyclebin.RestorationFailedPermissions", mCurrentCulture), HTMLHelper.HTMLEncode(name)));
            }
        }
        catch (Exception ex)
        {
            LogAndShowError("Content", "RESTOREDOC", ex);
        }
    }


    private void DestroyDocumentHistory(VersionHistoryInfo versionHistoryInfo, VersionManager versionManager)
    {
        try
        {
            if (IsAuthorizedPerDocument(versionHistoryInfo, "Destroy", mCurrentUser))
            {
                versionManager.DestroyDocumentHistory(versionHistoryInfo.DocumentID);
                ShowConfirmation(GetString("recyclebin.destroyok"));
            }
            else
            {
                var name = string.Format("{0} ({1})", versionHistoryInfo.VersionDocumentName, versionHistoryInfo.VersionNodeAliasPath);
                ShowError(String.Format(ResHelper.GetString("recyclebin.destructionfailedpermissions", mCurrentCulture), HTMLHelper.HTMLEncode(name)));
            }
        }
        catch (Exception ex)
        {
            LogAndShowError("Content", "DESTROYDOC", ex);
        }
    }

    #endregion


    #region "Helper methods"

    private static VersionHistoryInfo GetVersionHistoryInfo(int versionHistoryId)
    {
        return VersionHistoryInfo.Provider.Get(versionHistoryId);
    }


    /// <summary>
    /// Merges given where condition with additional settings.
    /// </summary>
    /// <param name="where">Original where condition</param>
    /// <returns>New where condition</returns>
    private string GetWhereCondition(string where)
    {
        // Filter by document name
        if (!string.IsNullOrEmpty(DocumentName))
        {
            where = SqlHelper.AddWhereCondition(where, "CMS_VersionHistory.VersionDocumentName LIKE '%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(DocumentName)) + "%'");
        }

        // Filter by document type
        if (!String.IsNullOrEmpty(DocumentType))
        {
            where = SqlHelper.AddWhereCondition(where, "VersionClassID IN (SELECT ClassID FROM CMS_Class WHERE " + new WhereCondition().WhereIn("ClassName", DocumentType.Split(';')).ToString(true) + ")");
        }

        return where;
    }


    /// <summary>
    /// Sets age of documents in order to obtain correct data set.
    /// </summary>
    /// <param name="modifiedFrom">Document modified from</param>
    /// <param name="modifiedTo">Document modified to</param>
    private void SetDocumentAge(ref DateTime modifiedFrom, ref DateTime modifiedTo)
    {
        // Set age of documents
        if (!string.IsNullOrEmpty(DocumentAge))
        {
            string[] ages = DocumentAge.Split(';');
            if (ages.Length == 2)
            {
                // Compute 'from' and 'to' values
                int from = ValidationHelper.GetInteger(ages[1], 0);
                int to = ValidationHelper.GetInteger(ages[0], 0);

                if (from > 0)
                {
                    modifiedFrom = DateTime.Now.AddDays((-1) * from);
                }

                if (to > 0)
                {
                    modifiedTo = DateTime.Now.AddDays((-1) * to);
                }
            }
        }
    }


    /// <summary>
    /// Logs exception to current log and event log
    /// </summary>
    /// <param name="eventcode">Event code</param>
    /// <param name="ex">Exception</param>
    private void LogException(string eventcode, Exception ex)
    {
        CurrentError = String.Format(ResHelper.GetString("General.ErrorOccuredLog"), "Content", eventcode);
        AddLog(CurrentError);
        Service.Resolve<IEventLogService>().LogException("Content", eventcode, ex);
    }


    /// <summary>
    /// Reload control data.
    /// </summary>
    public void ReloadData()
    {
        // Reload grid data
        PrepareGrid();
        ugRecycleBin.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Check user permissions for document.
    /// </summary>
    /// <param name="versionHistoryInfo">Document version history info</param>
    /// <param name="permission">Permissions</param>
    /// <param name="user">User</param>
    /// <returns>TreeNode if authorized, null otherwise</returns>
    private bool IsAuthorizedPerDocument(VersionHistoryInfo versionHistoryInfo, string permission, CurrentUserInfo user)
    {
        // Check global permission
        var userHasGlobalPerm = user.IsAuthorizedPerResource("CMS.Content", permission);

        // Get the values form deleted node
        var className = new DocumentClassNameRetriever(versionHistoryInfo.Data, true).Retrieve();

        var additionalPermission = false;

        if (permission.Equals("create", StringComparison.InvariantCultureIgnoreCase))
        {
            additionalPermission = user.IsAuthorizedPerClassName(className, "CreateSpecific");
        }

        // Check permissions
        if (userHasGlobalPerm || user.IsAuthorizedPerClassName(className, permission) || additionalPermission)
        {
            return true;
        }

        return false;
    }

    #endregion
}
