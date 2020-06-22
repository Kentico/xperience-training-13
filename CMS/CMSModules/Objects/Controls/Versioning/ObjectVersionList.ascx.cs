using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_Objects_Controls_Versioning_ObjectVersionList : CMSUserControl
{
    #region "Variables"

    private BaseInfo mObject = null;
    private bool mCheckPermissions = true;
    private bool? mAllowDestroy = null;
    private bool? mAllowModify = null;
    private bool mRegisterReloadHeaderScript = false;
    private bool mInvalidatePersistentEditedObject = false;

    #endregion


    #region "Events"

    /// <summary>
    /// Event triggered after rollback is made
    /// </summary>
    public EventHandler OnAfterRollback;

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
    /// Gets or sets version history object type.
    /// </summary>
    public string ObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets version history object type.
    /// </summary>
    public int ObjectID
    {
        get;
        set;
    }


    /// <summary>
    /// IInfo object representing object which version history is displayed.
    /// </summary>
    public BaseInfo Object
    {
        get
        {
            if (mObject == null)
            {
                if (!String.IsNullOrEmpty(ObjectType) && (ObjectID > 0))
                {
                    mObject = ProviderHelper.GetInfoById(ObjectType, ObjectID);
                    if (mObject != null)
                    {
                        UIContext.EditedObject = mObject;
                    }
                }
            }
            return mObject;
        }
        set
        {
            mObject = value;
            if (mObject != null)
            {
                ObjectID = mObject.Generalized.ObjectID;
                ObjectType = mObject.TypeInfo.ObjectType;
                UIContext.EditedObject = mObject;
            }
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return gridHistory.IsLiveSite;
        }
        set
        {
            gridHistory.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates if permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return mCheckPermissions;
        }
        set
        {
            mCheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates if user can destroy object version history.
    /// </summary>
    private bool AllowDestroy
    {
        get
        {
            if ((Object != null) && (mAllowDestroy == null))
            {
                mAllowDestroy = Object.CheckPermissions(PermissionsEnum.Destroy, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser);
            }

            return ValidationHelper.GetBoolean(mAllowDestroy, false);
        }
    }


    /// <summary>
    /// Indicates if user can modify object.
    /// </summary>
    private bool AllowModify
    {
        get
        {
            if ((Object != null) && (mAllowModify == null))
            {
                mAllowModify = Object.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser);
            }

            return ValidationHelper.GetBoolean(mAllowModify, false);
        }
    }


    /// <summary>
    /// Indicates if javascript to reload header should be rendered
    /// </summary>
    public bool RegisterReloadHeaderScript
    {
        get
        {
            return mRegisterReloadHeaderScript;
        }
        set
        {
            mRegisterReloadHeaderScript = value;
        }
    }


    /// <summary>
    /// Indicates if control should be displayed or not
    /// </summary>
    public override bool Visible
    {
        get
        {
            return base.Visible;
        }
        set
        {
            base.Visible = value;
            gridHistory.Visible = value;
        }
    }


    /// <summary>
    /// Reference to hidden refresh button
    /// </summary>
    public Button RefreshButton
    {
        get
        {
            return btnRefresh;
        }
    }


    /// <summary>
    /// Indicates if persistent edited object should be invalidated
    /// </summary>
    public bool InvalidatePersistentEditedObject
    {
        get
        {
            return (mInvalidatePersistentEditedObject && (Page is CMSPage));
        }
        set
        {
            mInvalidatePersistentEditedObject = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is used in the dialog mode.
    /// </summary>
    public bool DialogMode
    {
        get;
        set;
    }

    #endregion


    #region "Control methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);


        if (InvalidatePersistentEditedObject)
        {
            Control ctrl = ControlsHelper.GetPostBackControl(Page);
            if ((ctrl != null) && (ctrl.ClientID == gridHistory.ClientID))
            {
                CMSPage page = Page as CMSPage;
                if (page != null)
                {
                    page.PersistentEditedObject = null;
                }
            }
        }

        if (pnlObjectLocking.ObjectManager != null)
        {
            pnlObjectLocking.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;
        }
    }


    void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        // Reload list after object locking action
        gridHistory.ReloadData();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            gridHistory.StopProcessing = true;
        }
        else
        {
            SetupControl();

            // Initialize javascript
            InitScript();
        }
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    public void SetupControl()
    {
        ObjectEditMenu menu = ControlsHelper.GetChildControl(Page, typeof(ObjectEditMenu)) as ObjectEditMenu;
        if (menu != null)
        {
            menu.ShowSave = false;
        }

        var abstractMenu = ControlsHelper.GetChildControl(Page, typeof(IObjectEditMenu)) as IObjectEditMenu;
        if (abstractMenu != null)
        {
            abstractMenu.AbstractObjectManager.OnAfterAction += (sender, args) =>
            {
                if (DialogMode)
                {
                    ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "parentWOpenerRefresh", ScriptHelper.GetScript("if (parent && parent.wopener && parent.wopener.refresh) { parent.wopener.refresh(); }"));
                }
            };
        }

        gridHistory.ZeroRowsText = GetString("objectversioning.objecthasnohistory");
        if (Object != null)
        {
            // Set buttons confirmation
            btnDestroy.OnClientClick = "return confirm(" + ScriptHelper.GetString(ResHelper.GetString("VersionProperties.ConfirmDestroy")) + ");";
            btnMakeMajor.OnClientClick = "return confirm(" + ScriptHelper.GetString(ResHelper.GetString("VersionProperties.ConfirmMakeMajor")) + ");";

            gridHistory.OnExternalDataBound += gridHistory_OnExternalDataBound;
            gridHistory.OnAction += gridHistory_OnAction;
            gridHistory.WhereCondition = "VersionObjectType = '" + SqlHelper.GetSafeQueryString(Object.TypeInfo.ObjectType, false) + "' AND VersionObjectID = " + Object.Generalized.ObjectID;
            gridHistory.Columns = "VersionID, UserName, FullName, VersionModifiedWhen, VersionNumber";
        }
        else
        {
            gridHistory.StopProcessing = true;
            headTitle.Visible = false;
            btnDestroy.Visible = false;

            ShowError(GetString("objectversioning.uknownobject"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Object != null)
        {
            btnDestroy.Visible = btnMakeMajor.Visible = !gridHistory.IsEmpty;
            UIContext.EditedObject = Object;
        }
        else
        {
            btnDestroy.Visible = btnMakeMajor.Visible = false;
            UIContext.EditedObject = null;
        }
    }

    #endregion


    #region "Grid events"

    protected object gridHistory_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView data = null;
        sourceName = sourceName.ToLowerCSafe();
        CMSGridActionButton imageButton = null;
        switch (sourceName)
        {
            case "view":
                imageButton = ((CMSGridActionButton)sender);
                if (imageButton != null)
                {
                    GridViewRow gvr = (GridViewRow)parameter;
                    data = (DataRowView)gvr.DataItem;
                    string viewVersionUrl = ResolveUrl("~/CMSModules/Objects/Dialogs/ViewObjectVersion.aspx?versionhistoryid=" + ValidationHelper.GetInteger(data["VersionID"], 0) + "&clientid=" + ClientID);
                    viewVersionUrl = URLHelper.AddParameterToUrl(viewVersionUrl, "hash", QueryHelper.GetHash(viewVersionUrl));
                    imageButton.OnClientClick = "window.open(" + ScriptHelper.GetString(viewVersionUrl) + ");return false;";
                }
                break;

            case "allowdestroy":
                imageButton = ((CMSGridActionButton)sender);
                if (imageButton != null)
                {
                    if ((CheckPermissions && !AllowDestroy))
                    {
                        imageButton.Enabled = false;
                    }
                }
                break;

            case "rollback":
                imageButton = ((CMSGridActionButton)sender);
                if (imageButton != null)
                {
                    if ((CheckPermissions && !AllowModify))
                    {
                        imageButton.Enabled = false;
                    }
                }
                break;

            case "contextmenu":
                break;

            case "modifiedby":
                data = (DataRowView)parameter;
                string userName = ValidationHelper.GetString(data["UserName"], String.Empty);
                string fullName = ValidationHelper.GetString(data["FullName"], String.Empty);

                return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(userName, fullName));
        }

        return parameter;
    }


    protected void gridHistory_OnAction(string actionName, object actionArgument)
    {
        int versionHistoryId = ValidationHelper.GetInteger(actionArgument, 0);
        actionName = actionName.ToLowerCSafe();
        switch (actionName.ToLowerCSafe())
        {
            case "rollback":
            case "fullrollback":
                if (versionHistoryId > 0)
                {
                    // Check permissions
                    if (CheckPermissions && !AllowModify)
                    {
                        ShowError(GetString("History.ErrorNotAllowedToModify"));
                    }
                    else
                    {
                        try
                        {
                            var newVersionId = ObjectVersionManager.RollbackVersion(versionHistoryId, (actionName == "fullrollback"));
                            ObjectVersionHistoryInfo newVersion = ObjectVersionHistoryInfo.Provider.Get(newVersionId);

                            // Set object to null because after rollback it doesn't contain current data
                            Object = null;
                            gridHistory.ReloadData();

                            if (OnAfterRollback != null)
                            {
                                OnAfterRollback(this, null);
                            }

                            ShowConfirmation(GetString("objectversioning.rollbackOK"));

                            ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshContent", ScriptHelper.GetScript("RefreshRelatedContent_" + ClientID + "();"));

                            ScriptHelper.RefreshTabHeader(Page, newVersion.VersionObjectDisplayName);
                        }
                        catch (CodeNameNotUniqueException ex)
                        {
                            ShowError(String.Format(GetString("objectversioning.restorenotuniquecodename"), (ex.Object != null) ? "('" + ex.Object.ObjectCodeName + "')" : null));
                        }
                        catch (Exception ex)
                        {
                            ShowError(GetString("objectversioning.recyclebin.restorationfailed") + " " + GetString("general.seeeventlog"));

                            // Log to event log
                            Service.Resolve<IEventLogService>().LogException("Object version list", "OBJECTRESTORE", ex);
                        }
                    }
                }
                break;

            case "destroy":
                if (versionHistoryId > 0)
                {
                    // Check permissions
                    if (CheckPermissions && !AllowDestroy)
                    {
                        ShowError(GetString("History.ErrorNotAllowedToDestroy"));
                    }
                    else
                    {
                        ObjectVersionManager.DestroyObjectVersion(versionHistoryId);
                        ShowConfirmation(GetString("objectversioning.destroyOK"));
                    }
                }
                break;
        }
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// Button destroy history click.
    /// </summary>
    protected void btnDestroy_Click(object sender, EventArgs e)
    {
        var obj = Object;
        if (obj != null)
        {
            // Check permissions
            if (CheckPermissions && !AllowDestroy)
            {
                ShowError(GetString("History.ErrorNotAllowedToDestroy"));
                return;
            }

            var ti = obj.TypeInfo;

            ObjectVersionManager.DestroyObjectHistory(ti.ObjectType, obj.Generalized.ObjectID);

            string objType = GetString("Objecttype." + ti.ObjectType.Replace(".", "_"));
            string description = String.Format(GetString("objectversioning.historydestroyed"), SqlHelper.GetSafeQueryString(obj.Generalized.ObjectDisplayName, false));

            var logData = new EventLogData(EventTypeEnum.Information, objType, "DESTROYHISTORY")
            {
                EventDescription = description,
                EventUrl = RequestContext.RawURL
            };
            
            Service.Resolve<IEventLogService>().LogEvent(logData);

            ReloadData();
        }
        else
        {
            UIContext.EditedObject = null;
        }
    }


    /// <summary>
    /// Button make version major click.
    /// </summary>
    protected void btnMakeMajor_Click(object sender, EventArgs e)
    {
        if (Object != null)
        {
            // Check permissions
            if (CheckPermissions && !AllowModify)
            {
                ShowError(GetString("History.ErrorNotAllowedToModify"));
                return;
            }

            ObjectVersionHistoryInfo version = ObjectVersionManager.GetLatestVersion(Object.TypeInfo.ObjectType, Object.Generalized.ObjectID);
            if (version != null)
            {
                if (ObjectVersionManager.MakeVersionMajor(version))
                {
                    ShowConfirmation(GetString("objectversioning.makeversionmajorinfo"));
                }

                ReloadData();
            }
            else
            {
                ShowError(GetString("objectversioning.makeversionmajornoversion") + " " + GetString("objectversioning.objecthasnohistory"));
            }
        }
        else
        {
            UIContext.EditedObject = null;
        }
    }


    /// <summary>
    /// Button refresh click.
    /// </summary>
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        if (OnAfterRollback != null)
        {
            OnAfterRollback(this, null);
        }
    }


    protected void btnHidden_Click(object sender, EventArgs e)
    {
        // Process recycle bin action
        string[] args = hdnValue.Value.Split(';');
        if (args.Length == 2)
        {
            gridHistory_OnAction(args[0], args[1]);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data in unigrid.
    /// </summary>
    public void ReloadData()
    {
        gridHistory.ReloadData();
    }


    /// <summary>
    /// Register required javascript functions
    /// </summary>
    private void InitScript()
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("function RefreshRelatedContent_", ClientID, "(){if(window.RefreshContent != null) { window.RefreshContent(); }\n");
        if (RegisterReloadHeaderScript)
        {
            sbScript.Append("if(parent.frames[0] && parent.frames[0].ReloadAndSetTab) {parent.frames[0].ReloadAndSetTab(escape('selecttab|##versioningtab##'));}\n");
        }
        sbScript.Append("}\n");
        sbScript.Append("function RefreshVersions_", ClientID, "(){var button = document.getElementById('", btnRefresh.ClientID, "'); if(button){button.click();}}");
        sbScript.Append("function ContextVersionAction_", gridHistory.ClientID, @"(action, versionId) { document.getElementById('", hdnValue.ClientID, @"').value = action + ';' + versionId;", ControlsHelper.GetPostBackEventReference(btnHidden), ";}");

        // Register required js to reload related content
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ReloadContent_" + ClientID, ScriptHelper.GetScript(sbScript.ToString()));
    }

    #endregion
}
