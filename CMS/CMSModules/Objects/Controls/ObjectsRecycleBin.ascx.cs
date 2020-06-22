using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.EventLog;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_Objects_Controls_ObjectsRecycleBin : CMSUserControl
{
    #region "Private variables"

    private string mSiteName = String.Empty;
    private string mCurrentCulture = CultureHelper.DefaultUICultureCode;
    private SiteInfo mCurrentSite;
    private string mObjectType = String.Empty;
    private bool mIsSingleSite = true;
    private bool mRestrictUsers = true;

    private string mOrderBy = "VersionDeletedWhen DESC";
    private string mObjectDisplayName = String.Empty;
    private string mItemsPerPage = String.Empty;
    private string mGMTTooltip;
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


        public SiteInfo Site
        {
            get;
            private set;
        }


        public List<string> SelectedItems
        {
            get;
            set;
        }


        public BinSettingsContainer(CurrentUserInfo user, What what, SiteInfo site)
            : this()
        {
            User = user;
            CurrentWhat = what;
            Site = site;
        }
    }

    #endregion


    #region "Enumerations"

    protected enum Action
    {
        SelectAction = 0,
        Restore = 1,
        RestoreWithoutSiteBindings = 2,
        RestoreToCurrentSite = 3,
        Delete = 4
    }


    protected enum What
    {
        SelectedObjects = 0,
        AllObjects = 1
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
    /// Indicates if control is used only in one site mode(CMSDesk or Widget).
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

            mCurrentSite = null;
        }
    }


    /// <summary>
    /// Gets current selected site.
    /// </summary>
    public new SiteInfo CurrentSite
    {
        get
        {
            if ((mCurrentSite == null) && !String.IsNullOrEmpty(SiteName) && !SiteName.EqualsCSafe("##global##", true))
            {
                SiteInfo siteInfo = SiteInfo.Provider.Get(SiteName);
                if (siteInfo != null)
                {
                    mCurrentSite = siteInfo;
                }
            }
            return mCurrentSite;
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
    /// Filter by object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return mObjectType;
        }

        set
        {
            mObjectType = value;
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
    /// Object display name for grid filter.
    /// </summary>
    public string ObjectDisplayName
    {
        get
        {
            return mObjectDisplayName;
        }
        set
        {
            mObjectDisplayName = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return ugRecycleBin.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            ugRecycleBin.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates if the control data should perform the operations.
    /// </summary>
    public bool DelayedLoading
    {
        get
        {
            return ugRecycleBin.DelayedReload;
        }
        set
        {
            ugRecycleBin.DelayedReload = value;
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


    private void ugRecycleBin_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
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

        if (!RequestHelper.IsCallback())
        {
            ControlsHelper.RegisterPostbackControl(btnOk);

            // Create action script
            StringBuilder actionScript = new StringBuilder();
            actionScript.Append(
                @"
function PerformAction(selectionFunction, selectionField, dropId, validationLabel, whatId) {
  var selectionFieldElem = document.getElementById(selectionField);
  var label = document.getElementById(validationLabel);
  var items = selectionFieldElem.value;
  var whatDrp = document.getElementById(whatId);
  var allDocs = whatDrp.value == '", (int)What.AllObjects, @"';
  var action = document.getElementById(dropId).value;
  if (action == '", (int)Action.SelectAction, @"') {
     label.innerHTML = ", ScriptHelper.GetLocalizedString("massaction.selectsomeaction"), @";
     return false;
  }
  
  if(!eval(selectionFunction) || allDocs) {
     var confirmed = false;
     var confMessage = '';
     switch(action) {
        case '", (int)Action.RestoreToCurrentSite, @"':
        case '", (int)Action.RestoreWithoutSiteBindings, @"':
        case '", (int)Action.Restore, @"':
          confMessage = ", ScriptHelper.GetLocalizedString("objectversioning.recyclebin.confirmrestores"), @";
          break;
        
        case '", (int)Action.Delete, @"':
          confMessage = allDocs ?  ", ScriptHelper.GetLocalizedString("objectversioning.recyclebin.confirmemptyrecbin"), @" : ", ScriptHelper.GetLocalizedString("objectversioning.recyclebin.confirmdeleteselected") + @";
          break;
     }
     return confirm(confMessage);
  }
  else {
    label.innerHTML = ", ScriptHelper.GetLocalizedString("objectversioning.recyclebin.selectobjects"), @";
    return false;
  }
}
function ContextBinAction_", ugRecycleBin.ClientID, @"(action, versionId) {
  document.getElementById('", hdnValue.ClientID, @"').value = action + ';' + versionId;",
                ControlsHelper.GetPostBackEventReference(btnHidden), @";
}");

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
            if (drpAction.Items.Count == 0)
            {
                drpAction.Items.Add(new ListItem(GetString("general." + Action.Restore), Action.Restore.ToString("d")));
                drpAction.Items.Add(new ListItem(GetString("objectversioning.recyclebin." + Action.RestoreWithoutSiteBindings), Action.RestoreWithoutSiteBindings.ToString("d")));

                // Display restore to current site only if current site available
                SiteInfo si = SiteContext.CurrentSite;
                if (si != null)
                {
                    drpAction.Items.Add(new ListItem(String.Format(GetString("objectversioning.recyclebin." + Action.RestoreToCurrentSite), si.DisplayName), Action.RestoreToCurrentSite.ToString("d")));
                }
                drpAction.Items.Add(new ListItem(GetString("recyclebin.destroyhint"), Action.Delete.ToString("d")));

                drpWhat.Items.Add(new ListItem(GetString("contentlisting." + What.SelectedObjects), What.SelectedObjects.ToString("d")));
                drpWhat.Items.Add(new ListItem(GetString("contentlisting." + What.AllObjects), What.AllObjects.ToString("d")));
            }

            ugRecycleBin.OrderBy = OrderBy;

            var where = GetVersionObjectSiteIDWhereCondition();

            ugRecycleBin.WhereCondition = GetWhereCondition(where).ToString(true);
            ugRecycleBin.HideControlForZeroRows = false;
            ugRecycleBin.OnExternalDataBound += ugRecycleBin_OnExternalDataBound;
            ugRecycleBin.OnAction += ugRecycleBin_OnAction;

            // Hide or show site column
            ugRecycleBin.GridView.Columns[4].Visible = String.IsNullOrEmpty(SiteName);

            // Register the dialog script
            ScriptHelper.RegisterDialogScript(Page);

            // Initialize buttons

            string error = QueryHelper.GetString("displayerror", String.Empty);
            if (error != String.Empty)
            {
                ShowError(GetString("objectversioning.recyclebin.errorsomenotdestroyed"));
            }

            // Set visibility of panels
            pnlLog.Visible = false;
        }
        else
        {
            ugRecycleBin.StopProcessing = true;
        }

        filter.SiteID = (CurrentSite != null ? CurrentSite.SiteID : -1);
        filter.IsSingleSite = IsSingleSite;
        filter.DisplayUsersFromAllSites = !RestrictUsers;
        
        // Initialize events
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Hide multiple actions if grid is empty
        pnlFooter.Visible = ugRecycleBin.GridView.Rows.Count > 0;

        // Check if postback caused by filter button
        Control ctrlPostBack = ControlsHelper.GetPostBackControl(Page);
        if (ctrlPostBack is Button)
        {
            pnlGrid.Update();
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Restoring & destroying methods"

    /// <summary>
    /// Restores objects selected in UniGrid with binding to current site.
    /// </summary>
    private void RestoreToCurrentSite(BinSettingsContainer settings)
    {
        Restore(settings, Action.RestoreToCurrentSite);
    }


    /// <summary>
    /// Restores objects selected in UniGrid without site bindings.
    /// </summary>
    private void RestoreWithoutSiteBindings(BinSettingsContainer settings)
    {
        Restore(settings, Action.RestoreWithoutSiteBindings);
    }


    /// <summary>
    /// Restores objects selected in UniGrid with site bindings and children.
    /// </summary>
    private void RestoreWithChildren(BinSettingsContainer settings)
    {
        Restore(settings, Action.Restore);
    }


    /// <summary>
    /// Restores objects selected in UniGrid.
    /// </summary>
    private void Restore(BinSettingsContainer settings, Action action)
    {
        try
        {
            // Begin log
            AddLog(ResHelper.GetString("objectversioning.recyclebin.restoringobjects", mCurrentCulture));

            if (settings.User.IsAuthorizedPerResource("cms.globalpermissions", "RestoreObjects"))
            {
                DataSet recycleBin = GetRecycleBinSeletedItems(settings, "VersionID, VersionObjectDisplayName, VersionObjectType, VersionObjectID");

                if (!DataHelper.DataSourceIsEmpty(recycleBin))
                {
                    RestoreDataSet(recycleBin, action);
                }
            }
            else
            {
                CurrentError = ResHelper.GetString("objectversioning.recyclebin.restorationfailedpermissions");
                AddLog(CurrentError);
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
                CurrentError = ResHelper.GetString("objectversioning.recyclebin.restorationfailed", mCurrentCulture) + ": " + ResHelper.GetString("general.seeeventlog", mCurrentCulture);
                AddLog(CurrentError);

                // Log to event log
                LogException("OBJECTRESTORE", ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            CurrentError = ResHelper.GetString("objectversioning.recyclebin.restorationfailed", mCurrentCulture) + ": " + ResHelper.GetString("general.seeeventlog", mCurrentCulture);
            AddLog(CurrentError);

            // Log to event log
            LogException("OBJECTRESTORE", ex);
        }
    }


    /// <summary>
    /// Restores set of given version histories.
    /// </summary>
    /// <param name="recycleBin">DataSet with nodes to restore</param>
    /// <param name="action">Action to be performed</param>
    private void RestoreDataSet(DataSet recycleBin, Action action)
    {
        // Result flags
        bool resultOK = true;

        if (!DataHelper.DataSourceIsEmpty(recycleBin))
        {
            // Restore all objects
            foreach (DataRow dataRow in recycleBin.Tables[0].Rows)
            {
                int versionId = ValidationHelper.GetInteger(dataRow["VersionID"], 0);

                // Log current event
                string taskTitle = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(dataRow["VersionObjectDisplayName"], string.Empty)));

                // Restore object
                if (versionId > 0)
                {
                    GeneralizedInfo restoredObj = null;
                    try
                    {
                        switch (action)
                        {
                            case Action.Restore:
                                restoredObj = ObjectVersionManager.RestoreObject(versionId, true);
                                break;

                            case Action.RestoreToCurrentSite:
                                restoredObj = ObjectVersionManager.RestoreObject(versionId, SiteContext.CurrentSiteID);
                                break;

                            case Action.RestoreWithoutSiteBindings:
                                restoredObj = ObjectVersionManager.RestoreObject(versionId, 0);
                                break;
                        }
                    }
                    catch (CodeNameNotUniqueException ex)
                    {
                        CurrentError = String.Format(GetString("objectversioning.restorenotuniquecodename"), (ex.Object != null) ? "('" + ex.Object.ObjectCodeName + "')" : null);
                        AddLog(CurrentError);
                    }

                    if (restoredObj != null)
                    {
                        AddLog(ResHelper.GetString("general.object", mCurrentCulture) + " '" + taskTitle + "'");
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

        if (resultOK)
        {
            CurrentInfo = ResHelper.GetString("ObjectVersioning.Recyclebin.RestorationOK", mCurrentCulture);
            AddLog(CurrentInfo);
        }
        else
        {
            CurrentError = ResHelper.GetString("objectversioning.recyclebin.restorationfailed", mCurrentCulture);
            AddLog(CurrentError);
        }
    }


    private DataSet GetRecycleBinSeletedItems(BinSettingsContainer settings, string columns)
    {
        var where = new WhereCondition();

        switch (settings.CurrentWhat)
        {
            case What.AllObjects:
                if (IsSingleSite)
                {
                    where.WhereNull("VersionObjectSiteID");
                }
                if (settings.Site != null)
                {
                    where.Or().WhereEquals("VersionObjectSiteID", settings.Site.SiteID);
                }

                // Wrap filter condition with brackets
                where.Where(new WhereCondition(filter.WhereCondition) { WhereIsComplex = true });
                where = GetWhereCondition(where);
                break;

            case What.SelectedObjects:
                // Restore selected objects
                var toRestore = settings.SelectedItems;
                where.WhereIn("VersionID", toRestore);
                break;
        }

        return ObjectVersionHistoryInfoProvider.GetRecycleBin(where.ToString(true), OrderBy, -1, columns);
    }


    /// <summary>
    /// Empties recycle bin.
    /// </summary>
    private void EmptyBin(BinSettingsContainer settings)
    {
        // Begin log
        AddLog(ResHelper.GetString("Recyclebin.EmptyingBin", mCurrentCulture));

        try
        {
            DataSet recycleBin = GetRecycleBinSeletedItems(settings, "VersionID, VersionObjectType, VersionObjectID, VersionObjectDisplayName, VersionObjectSiteID");
            if (!DataHelper.DataSourceIsEmpty(recycleBin))
            {
                foreach (DataRow dr in recycleBin.Tables[0].Rows)
                {
                    string versionObjType = Convert.ToString(dr["VersionObjectType"]);
                    string objName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(dr["VersionObjectDisplayName"], string.Empty)));
                    SiteInfo currentSite = settings.Site;
                    
                    string siteName;
                    if (currentSite != null)
                    {
                        siteName = currentSite.SiteName;
                    }
                    else
                    {
                        int siteId = ValidationHelper.GetInteger(dr["VersionObjectSiteID"], 0);
                        siteName = SiteInfoProvider.GetSiteName(siteId);
                    }

                    // Check permissions
                    UserInfo currentUserInfo = settings.User;
                    if (!currentUserInfo.IsAuthorizedPerObject(PermissionsEnum.Destroy, versionObjType, siteName))
                    {
                        CurrentError = String.Format(ResHelper.GetString("objectversioning.Recyclebin.DestructionFailedPermissions", mCurrentCulture), objName);
                        AddLog(CurrentError);
                    }
                    else
                    {
                        AddLog(ResHelper.GetString("general.object", mCurrentCulture) + " '" + objName + "'");

                        // Destroy the version
                        int versionObjId = ValidationHelper.GetInteger(dr["VersionObjectID"], 0);
                        ObjectVersionManager.DestroyObjectHistory(versionObjType, versionObjId);
                        LogContext.LogEventToCurrent(EventType.INFORMATION, "Objects", "DESTROYOBJECT", String.Format(ResHelper.GetString("objectversioning.Recyclebin.objectdestroyed"), objName), RequestContext.RawURL, currentUserInfo.UserID, currentUserInfo.UserName, 0, null, RequestContext.UserHostAddress, (currentSite != null) ? currentSite.SiteID : 0, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);
                    }
                }
                if (!String.IsNullOrEmpty(CurrentError))
                {
                    CurrentError = ResHelper.GetString("objectversioning.recyclebin.errorsomenotdestroyed", mCurrentCulture);
                    AddLog(CurrentError);
                }
                else
                {
                    CurrentInfo = ResHelper.GetString("ObjectVersioning.Recyclebin.DestroyOK", mCurrentCulture);
                    AddLog(CurrentInfo);
                }
            }
        }
        catch (ThreadAbortException ex)
        {
            if (!CMSThread.Stopped(ex))
            {
                // Log error
                CurrentError = "Error occurred: " + ResHelper.GetString("general.seeeventlog", mCurrentCulture);
                AddLog(CurrentError);

                // Log to event log
                LogException("EMPTYINGBIN", ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            CurrentError = "Error occurred: " + ResHelper.GetString("general.seeeventlog", mCurrentCulture);
            AddLog(CurrentError);

            // Log to event log
            LogException("EMPTYINGBIN", ex);
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

        BinSettingsContainer binSettings = new BinSettingsContainer(CurrentUser, currentWhat, CurrentSite);

        switch (currentWhat)
        {
            case What.SelectedObjects:
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
            case Action.RestoreToCurrentSite:
            case Action.RestoreWithoutSiteBindings:
                {
                    ctlAsyncLog.TitleText = GetString("objectversioning.Recyclebin.Restoringobjects");

                    switch (action)
                    {
                        case Action.Restore:
                            RunAsync(p => RestoreWithChildren(binSettings));
                            break;

                        case Action.RestoreToCurrentSite:
                            RunAsync(p => RestoreToCurrentSite(binSettings));
                            break;

                        case Action.RestoreWithoutSiteBindings:
                            RunAsync(p => RestoreWithoutSiteBindings(binSettings));
                            break;
                    }
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


    protected void btnHidden_Click(object sender, EventArgs e)
    {
        // Process recycle bin action
        string[] args = hdnValue.Value.Split(';');
        if (args.Length == 2)
        {
            ugRecycleBin_OnAction(args[0], args[1]);
            ReloadData(false);
        }
    }

    #endregion


    #region "Grid events"

    protected object ugRecycleBin_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();
        switch (sourceName)
        {
            case "view":
                CMSGridActionButton imgView = (CMSGridActionButton)sender;
                if (imgView != null)
                {
                    GridViewRow gvr = (GridViewRow)parameter;
                    DataRowView data = (DataRowView)gvr.DataItem;
                    string viewVersionUrl = ResolveUrl("~/CMSModules/Objects/Dialogs/ViewObjectVersion.aspx?showall=1&nocompare=1&versionhistoryid=" + ValidationHelper.GetInteger(data["VersionID"], 0));
                    viewVersionUrl = URLHelper.AddParameterToUrl(viewVersionUrl, "hash", QueryHelper.GetHash(viewVersionUrl));
                    imgView.OnClientClick = "window.open(" + ScriptHelper.GetString(viewVersionUrl) + ");return false;";
                }
                break;


            case "deletedwhen":
            case "deletedwhentooltip":
                if (sourceName.EqualsCSafe("deletedwhen", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime deletedWhen = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                    return TimeZoneHelper.ConvertToUserTimeZone(deletedWhen, true, CurrentUser, CurrentSite);
                }
                else
                {
                    return mGMTTooltip ?? (mGMTTooltip = TimeZoneHelper.GetUTCLongStringOffset(CurrentUser, CurrentSite));
                }

            case "versionobjecttype":
                string objType = ValidationHelper.GetString(parameter, "");
                return HTMLHelper.HTMLEncode(GetString("ObjectType." + objType.Replace(".", "_")));
        }

        return (parameter != null) ? HTMLHelper.HTMLEncode(parameter.ToString()) : null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void ugRecycleBin_OnAction(string actionName, object actionArgument)
    {
        int versionHistoryId = ValidationHelper.GetInteger(actionArgument, 0);
        actionName = actionName.ToLowerCSafe();

        switch (actionName)
        {
            case "restorechilds":
            case "restorewithoutbindings":
            case "restorecurrentsite":
                try
                {
                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.globalpermissions", "RestoreObjects"))
                    {
                        switch (actionName)
                        {
                            case "restorechilds":
                                ObjectVersionManager.RestoreObject(versionHistoryId, true);
                                break;

                            case "restorewithoutbindings":
                                ObjectVersionManager.RestoreObject(versionHistoryId, 0);
                                break;

                            case "restorecurrentsite":
                                ObjectVersionManager.RestoreObject(versionHistoryId, SiteContext.CurrentSiteID);
                                break;
                        }

                        ShowConfirmation(GetString("ObjectVersioning.Recyclebin.RestorationOK"));
                    }
                    else
                    {
                        ShowError(ResHelper.GetString("objectversioning.recyclebin.restorationfailedpermissions"));
                    }
                }
                catch (CodeNameNotUniqueException ex)
                {
                    ShowError(String.Format(GetString("objectversioning.restorenotuniquecodename"), (ex.Object != null) ? "('" + ex.Object.ObjectCodeName + "')" : null));
                }
                catch (Exception ex)
                {
                    ShowError(GetString("objectversioning.recyclebin.restorationfailed") + GetString("general.seeeventlog"));

                    // Log to event log
                    LogException("OBJECTRESTORE", ex);
                }
                break;

            case "destroy":
                ObjectVersionHistoryInfo verInfo = ObjectVersionHistoryInfo.Provider.Get(versionHistoryId);
                if (verInfo != null)
                {
                    // Get object site name
                    string siteName = (CurrentSite != null) ? CurrentSite.SiteName : SiteInfoProvider.GetSiteName(verInfo.VersionObjectSiteID);

                    if (CurrentUser.IsAuthorizedPerObject(PermissionsEnum.Destroy, verInfo.VersionObjectType, siteName))
                    {
                        ObjectVersionManager.DestroyObjectHistory(verInfo.VersionObjectType, verInfo.VersionObjectID);
                        ShowConfirmation(GetString("ObjectVersioning.Recyclebin.DestroyOK"));
                    }
                    else
                    {
                        ShowError(String.Format(ResHelper.GetString("objectversioning.recyclebin.destructionfailedpermissions"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(verInfo.VersionObjectDisplayName))));
                    }
                }
                break;
        }

        ugRecycleBin.ResetSelection();
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Creates WHERE condition for VersionObjectSiteID column.
    /// </summary>
    private WhereCondition GetVersionObjectSiteIDWhereCondition()
    {
        var where = new WhereCondition();
        if (IsSingleSite || (SiteName == "##global##"))
        {
            where.Where("VersionObjectSiteID", QueryUnaryOperator.IsNull);
        }

        if (CurrentSite != null)
        {
            where.Or().Where("VersionObjectSiteID", QueryOperator.Equals, CurrentSite.SiteID);
        }

        return where;
    }


    /// <summary>
    /// Merges given where condition with additional settings.
    /// </summary>
    /// <param name="where">Original where condition</param>
    /// <returns>New where condition</returns>
    private WhereCondition GetWhereCondition(WhereCondition where)
    {
        // Create version condition
        var condition = new WhereCondition().WhereNotNull("VersionDeletedWhen");

        // Add recycle bin condition (ensure correct parentheses wrapping)
        condition.Where(where);

        // Filter by object name
        if (!string.IsNullOrEmpty(ObjectDisplayName))
        {
            condition.WhereContains("VersionObjectDisplayName", ObjectDisplayName);
        }

        // Filter by object type
        if (!String.IsNullOrEmpty(ObjectType))
        {
            condition.WhereContains("VersionObjectType", ObjectType);
        }

        return condition;
    }


    /// <summary>
    /// Reload control data.
    /// </summary>
    public void ReloadData(bool refreshInfo)
    {
        var where = GetVersionObjectSiteIDWhereCondition();

        // Reload grid data
        ugRecycleBin.WhereCondition = GetWhereCondition(where).ToString(true);
        ugRecycleBin.ReloadData();
        pnlGrid.Update();
    }


    /// <summary>
    /// Method to log exceptions to event log.
    /// </summary>
    /// <param name="eventCode">Code of event during which exception occurred</param>
    /// <param name="ex">Exception</param>
    private void LogException(string eventCode, Exception ex)
    {
        // Log exception to event log
        Service.Resolve<IEventLogService>().LogException("Object recycle bin", eventCode, ex);
    }

    #endregion
}
