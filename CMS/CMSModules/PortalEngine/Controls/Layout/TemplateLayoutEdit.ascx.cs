using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_Layout_TemplateLayoutEdit : CMSPreviewControl
{
    #region "Variables"

    // Type of the edited object
    private enum EditedObjectTypeEnum
    {
        Layout = 0,
        Template = 1,
    }

    protected bool startWithFullScreen = false;
    private int previewState;
    private int mTemplateId;
    private EditedObjectTypeEnum? mEditedObjectType;
    private PageTemplateInfo mPageTemplateInfo;
    private bool enablePreview = true;

    private bool? mShowSharedLayoutWarnings;
    private bool requiresDialog;
    private bool dialog;
    private int sharedLayoutId;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets the edited object.
    /// </summary>
    private new BaseInfo EditedObject
    {
        get
        {
            return base.EditedObject as BaseInfo;
        }
    }


    /// <summary>
    /// Gets the type of the edited object.
    /// </summary>
    private EditedObjectTypeEnum EditedObjectType
    {
        get
        {
            if (mEditedObjectType == null)
            {
                mEditedObjectType = EditedObjectTypeEnum.Layout;

                if (EditedObject != null)
                {
                    switch (EditedObject.TypeInfo.ObjectType)
                    {
                        case PageTemplateInfo.OBJECT_TYPE:
                            mEditedObjectType = EditedObjectTypeEnum.Template;
                            break;
                    }
                }
            }

            return mEditedObjectType.Value;
        }
    }


    /// <summary>
    /// Gets a value that indicates if warnings about the shared layouts should be visible.
    /// </summary>
    private bool ShowSharedLayoutWarnings
    {
        get
        {
            if (!mShowSharedLayoutWarnings.HasValue)
            {
                mShowSharedLayoutWarnings = QueryHelper.GetBoolean("sharedlayoutwarnings", true);
            }
            return mShowSharedLayoutWarnings.Value;
        }
    }


    /// <summary>
    /// Gets the template ID.
    /// </summary>
    private int TemplateID
    {
        get
        {
            if (mTemplateId == 0)
            {
                mTemplateId = QueryHelper.GetInteger("templateid", 0);
            }

            return mTemplateId;
        }
    }


    /// <summary>
    /// Gets the current page template info object.
    /// </summary>
    private PageTemplateInfo PageTemplateInfo
    {
        get
        {
            return mPageTemplateInfo ?? (mPageTemplateInfo = PageTemplateInfoProvider.GetPageTemplateInfo(TemplateID));
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Ensures correct displaying of info messages.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
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
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
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
            EditForm.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Actual form according to correct object type.
    /// </summary>
    public UIForm EditForm
    {
        get
        {
            switch (EditedObjectType)
            {
                case EditedObjectTypeEnum.Template:
                    return EditFormTemplate;

                default:
                    return EditFormLayout;
            }
        }
    }


    /// <summary>
    /// If true, for PageTemplates it is possible to switch between shared and custom layout.
    /// </summary>
    public bool AllowTypeSwitching
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowTypeSwitching"), false);
        }
        set
        {
            SetValue("AllowTypeSwitching", value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Hide all UIForms
        EditFormTemplate.Visible = false;
        EditFormTemplate.StopProcessing = true;
        EditFormLayout.Visible = false;
        EditFormLayout.StopProcessing = true;

        sharedLayoutId = QueryHelper.GetInteger("newshared", 0);

        // Show UIForm for the current edited object type
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                EditFormTemplate.Visible = true;
                EditFormTemplate.StopProcessing = false;
                break;

            case EditedObjectTypeEnum.Layout:
                EditFormLayout.Visible = true;
                EditFormLayout.StopProcessing = false;
                break;
        }

        if (AllowTypeSwitching && (EditedObjectType == EditedObjectTypeEnum.Layout))
        {
            // Force ObjectManager to work with PageTemplate even though the EditedObject is layout
            editMenuElem.ObjectManager.ObjectType = PageTemplateInfo.OBJECT_TYPE;
            editMenuElem.ObjectManager.ObjectID = TemplateID;
        }

        pnlType.Visible = AllowTypeSwitching;
        requiresDialog = ((CMSDeskPage)Page).RequiresDialog;
        dialog = (QueryHelper.GetBoolean("dialog", false) || QueryHelper.GetBoolean("isindialog", false));
    }


    protected void EditForm_Create(object sender, EventArgs e)
    {
        EditForm.OnAfterDataLoad += EditForm_OnAfterDataLoad;
    }


    /// <summary>
    /// UIForm's after data load event handler.
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Setup Preview mode state
        previewState = GetPreviewState();

        bool fullScreen = ((previewState != 0) && enablePreview);
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                codeElem.Editor.AutoSize = true;
                codeElem.Editor.ParentElementID = ParentClientID;
                codeElem.FullscreenMode = fullScreen;
                break;

            case EditedObjectTypeEnum.Layout:
                codeLayoutElem.Editor.AutoSize = true;
                codeLayoutElem.Editor.ParentElementID = ParentClientID;
                codeLayoutElem.FullscreenMode = fullScreen;
                break;
        }
    }


    /// <summary>
    /// Display info messages
    /// </summary>
    /// <param name="forceDisplay">If true, message is displayed even on postback</param>
    private void DisplayMessage(bool forceDisplay)
    {
        bool showMessage = editMenuElem.ObjectManager.IsObjectChecked() && (!RequestHelper.IsPostBack() || forceDisplay);

        // Display shared template warning
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                {
                    if (showMessage)
                    {
                        PageTemplateInfo pti = EditFormTemplate.EditedObject as PageTemplateInfo;
                        ShowSharedTemplateWarningMessage(pti);
                    }
                }
                break;

            case EditedObjectTypeEnum.Layout:
                {
                    LayoutInfo layoutInfo = EditFormLayout.EditedObject as LayoutInfo;
                    if (showMessage)
                    {
                        if (DialogMode && ShowSharedLayoutWarnings && (layoutInfo != null))
                        {
                            ShowInformation(string.Format(GetString("layout.sharedwarning"), layoutInfo.LayoutDisplayName));
                        }
                    }
                }
                break;
        }
    }


    /// <summary>
    /// Returns preview state based on edited object type.
    /// </summary>
    private int GetPreviewState()
    {
        int state = 0;

        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                state = GetPreviewStateFromCookies(PAGETEMPLATELAYOUT);
                break;

            case EditedObjectTypeEnum.Layout:
                state = GetPreviewStateFromCookies(PAGELAYOUT);
                break;
        }

        // Hide 'Preview' button as it's no longer valid for MVC scenarios (see PLATFORM-15329)
        enablePreview = false;

        return state;
    }


    protected override void OnLoad(EventArgs e)
    {
        DisplayMessage(false);

        base.OnLoad(e);

        // Setup Preview mode state
        previewState = GetPreviewState();

        if (AllowTypeSwitching && (EditedObject != null))
        {
            // Standard layout
            radCustom.Checked = (EditedObjectType == EditedObjectTypeEnum.Template);
            radShared.Checked = !radCustom.Checked;
        }

        RegisterScripts();

        editMenuElem.MenuPanel.CssClass = "PreviewMenu";
        editMenuElem.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
    }


    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Don't save shared layout object, update only necessary objects
        if ((EditedObjectType == EditedObjectTypeEnum.Layout) && radShared.Checked)
        {
            LayoutInfo li = EditedObject as LayoutInfo;
            int newLayoutId = ValidationHelper.GetInteger(drpLayout.Value, 0);

            // We need to save also page template if shared template is used
            if ((PageTemplateInfo != null) && (PageTemplateInfo.LayoutID != li.LayoutId))
            {
                PageTemplateInfo.LayoutID = newLayoutId;
                PageTemplateInfo.Update();
            }
            ShowChangesSaved();

            // Prevent from saving object
            EditForm.StopProcessing = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        HandleFullScreen();

        base.OnPreRender(e);

        if (AllowTypeSwitching && (EditedObject != null))
        {
            if (sharedLayoutId > 0)
            {
                drpLayout.Value = sharedLayoutId;
            }
            else if (EditedObjectType == EditedObjectTypeEnum.Layout)
            {
                drpLayout.Value = EditedObject.Generalized.ObjectID;
            }

            radCustom.Text = GetString("TemplateLayout.Custom");

            var sanitizedCurrentUrl = ScriptHelper.GetString(RequestContext.CurrentURL, encapsulate: false);
            var customLayoutUrl = URLHelper.AddParameterToUrl(sanitizedCurrentUrl, "newshared", "0");
            customLayoutUrl = URLHelper.AddParameterToUrl(customLayoutUrl, "oldshared", drpLayout.Value.ToString());

            radCustom.Attributes.Add("onclick", "window.location = '" + customLayoutUrl + "'");

            radShared.Text = GetString("TemplateLayout.Shared");
            if (drpLayout.UniSelector.HasData)
            {
                var sharedLayoutUrl = URLHelper.AddParameterToUrl(sanitizedCurrentUrl, "newshared", drpLayout.Value.ToString());
                radShared.Attributes.Add("onclick", "window.location = '" + sharedLayoutUrl + "'");
            }

            // Get the current layout type
            bool radioButtonsEnabled = !SynchronizationHelper.UseCheckinCheckout;

            // Page template layout
            radioButtonsEnabled |= PageTemplateInfo.Generalized.IsCheckedOutByUser(MembershipContext.AuthenticatedUser);

            // Disable also radio buttons when the object is not checked out
            radShared.Enabled = radCustom.Enabled = drpLayout.Enabled = radioButtonsEnabled;
        }

        if ((EditedObjectType == EditedObjectTypeEnum.Layout) && (AllowTypeSwitching || DialogMode))
        {
            pnlServer.Visible = false;
            pnlLayout.CategoryTitleResourceString = null;
        }

        RegisterInitScripts(pnlBody.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);

        if (QueryHelper.GetBoolean("refreshParent", false))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshParent", "window.refreshPageOnClose = true;", true);
        }

        if (DialogMode && QueryHelper.GetBoolean("wopenerrefresh", false) && !ValidationHelper.GetBoolean(hdnWOpenerRefreshed.Value, false))
        {
            RegisterWOpenerRefreshScript();
            hdnWOpenerRefreshed.Value = "1";
        }

        // Try to get page template
        EditForm.EnableByLockState();

        // Enable DDL and disable UIForm for shared layouts
        if (radShared.Checked)
        {
            DisableEditableFields();
        }
        else
        {
            drpLayout.Enabled = false;
        }

        // Check whether virtual objects are allowed
        if (!SettingsKeyInfoProvider.VirtualObjectsAllowed)
        {
            ShowWarning(GetString("VirtualPathProvider.NotRunning"));
        }
    }


    /// <summary>
    /// Handles the OnAfterAction event of the ObjectManager control.
    /// </summary>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (!e.IsValid)
        {
            MessagesPlaceHolder.WarningText = "";
            return;
        }

        if ((e.ActionName == ComponentEvents.SAVE) || (e.ActionName == ComponentEvents.CHECKIN))
        {
            // Register refresh script
            string refreshScript = ScriptHelper.GetScript("if ((wopener != null) && (wopener.RefreshPage != null)) {wopener.RefreshPage();}");
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "pageTemplateRefreshScript", refreshScript);

            // Register preview refresh
            RegisterRefreshScript();

            // Close if required
            if (ValidationHelper.GetBoolean(hdnClose.Value, false))
            {
                ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseDialogPreviewScript", ScriptHelper.GetScript("CloseDialog();"));
            }

            // Hide warning after save
            MessagesPlaceHolder.WarningText = "";
        }
        else if (e.ActionName == ComponentEvents.UNDO_CHECKOUT)
        {
            if (AllowTypeSwitching)
            {
                var url = RequestContext.CurrentURL;
                url = URLHelper.RemoveParameterFromUrl(url, "newshared");
                url = URLHelper.RemoveParameterFromUrl(url, "oldshared");
                url = URLHelper.AddParameterToUrl(url, "wopenerrefresh", "1");
                URLHelper.ResponseRedirect(url);
            }
        }
        else if (e.ActionName == ComponentEvents.CHECKOUT)
        {
            DisplayMessage(true);
        }

        switch (e.ActionName)
        {
            case ComponentEvents.SAVE:
            case ComponentEvents.CHECKOUT:
            case ComponentEvents.CHECKIN:
            case ComponentEvents.UNDO_CHECKOUT:
                if (DialogMode)
                {
                    RegisterWOpenerRefreshScript();
                }
                else if (dialog)
                {
                    ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "parentWOpenerRefresh", ScriptHelper.GetScript("if (parent && parent.wopener && parent.wopener.refresh) { parent.wopener.refresh(); }"));
                }
                break;
        }

        if (!AllowTypeSwitching && (EditedObjectType == EditedObjectTypeEnum.Layout) && (e.ActionName != ComponentEvents.CHECKOUT) && !DialogMode)
        {
            ScriptHelper.RefreshTabHeader(Page, EditForm.EditedObject.Generalized.ObjectDisplayName);
        }

        // No save for checkout
        if (e.ActionName != ComponentEvents.CHECKOUT)
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Layout DropDownlist change.
    /// </summary>
    protected void selectShared_Changed(object sender, EventArgs ea)
    {
        if (EditedObject != null)
        {
            URLHelper.ResponseRedirect(URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "newshared", drpLayout.Value + ""), "oldshared", EditedObject.Generalized.ObjectID.ToString()));
        }
    }


    /// <summary>
    /// Ensures full screen mode for preview.
    /// </summary>
    private void HandleFullScreen()
    {
        startWithFullScreen = ((previewState != 0) && editMenuElem.ObjectManager.IsObjectChecked());

        // Wrong calculation for these browsers, when div is hidden.
#pragma warning disable CS0618 // Type or member is obsolete
        bool hide = (BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
#pragma warning restore CS0618 // Type or member is obsolete
        pnlBody.Attributes["style"] = (startWithFullScreen && !hide) ? "display: none;" : "display: block;";
    }


    /// <summary>
    /// Registers script for header resize
    /// </summary>
    private void RegisterScripts()
    {
        // Register action script for dialog purposes
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "PreviewHierarchyPerformAction", ScriptHelper.GetScript("function actionPerformed(action) { if (action == 'saveandclose') { document.getElementById('" + hdnClose.ClientID + "').value = '1'; }; " + editMenuElem.ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null) + "; }"));
    }


    private void RegisterWOpenerRefreshScript()
    {
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "wOpenerRefresh", ScriptHelper.GetScript("if (wopener && wopener.refresh) { wopener.refresh(); }"));
    }


    /// <summary>
    /// Shows the shared template warning message for re-usable templates.
    /// </summary>
    /// <param name="pti">The page template info object</param>
    private void ShowSharedTemplateWarningMessage(PageTemplateInfo pti)
    {
        if ((pti != null) && ShowSharedLayoutWarnings)
        {
            ShowInformation(GetString("template.shared.warning"));
        }
    }


    /// <summary>
	/// Disables editable form fields according to appropriate object type.
	/// </summary>
    private void DisableEditableFields()
    {
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Layout:
                codeLayoutElem.Enabled = false;
                cssLayoutEditor.Editor.Enabled = false;
                break;
            case EditedObjectTypeEnum.Template:
                codeElem.Editor.Enabled = false;
                cssEditor.Editor.Enabled = false;
                break;
        }
    }

    #endregion
}