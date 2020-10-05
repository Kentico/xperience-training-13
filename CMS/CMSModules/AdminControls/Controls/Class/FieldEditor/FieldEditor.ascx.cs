using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_FieldEditor : BaseFieldEditor
{
    #region "Events"

    /// <summary>
    /// Event raised when new field is created and form definition is saved.
    /// </summary>
    public event OnFieldCreatedEventHandler OnFieldCreated;


    /// <summary>
    /// Event raised when selected item (field or category) is deleted. Second parameter is of <see cref="CMS.UIControls.FieldEditorEventArgs"/> type.
    /// </summary>
    public event AfterItemDeletedEventHandler AfterItemDeleted;

    #endregion


    #region "Variables"

    private FormFieldInfo ffi;
    private FormCategoryInfo fci;
    private bool mAllowDummyFields;
    private bool mAllowExtraFields;
    private bool mDevelopmentMode;
    private int mWebPartId;
    private FieldEditorModeEnum mMode;
    private FieldEditorControlsEnum mDisplayedControls = FieldEditorControlsEnum.ModeSelected;
    private bool mEnableSystemFields;
    private SaveAction btnSave;
    private HeaderAction btnReset;
    private bool mIsWizard;
    private bool disableSaveAction;
    private string mOriginalFormDefinition;
    private string mFormDefinition;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether new item is edited.
    /// </summary>
    public bool IsNewItemEdited
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsNewItemEdited"], false);
        }
        set
        {
            ViewState["IsNewItemEdited"] = value;
            fieldTypeSelector.IsNewItemEdited = value;
            databaseConfiguration.IsNewItemEdited = value;
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


    /// <summary>
    /// Indicates if system fields from tables CMS_Tree and CMS_Document are offered to the user.
    /// </summary>
    public bool EnableSystemFields
    {
        get
        {
            return mEnableSystemFields;
        }
        set
        {
            mEnableSystemFields = value;
            databaseConfiguration.EnableSystemFields = value;
            fieldTypeSelector.EnableSystemFields = value;
        }
    }


    /// <summary>
    /// Indicates if field editor works in development mode.
    /// </summary>
    public bool DevelopmentMode
    {
        get
        {
            return mDevelopmentMode;
        }
        set
        {
            mDevelopmentMode = value;
            fieldAppearance.DevelopmentMode = value;
            fieldTypeSelector.DevelopmentMode = value;
        }
    }


    /// <summary>
    /// Class name.
    /// </summary>
    public override string ClassName
    {
        get
        {
            return base.ClassName;
        }
        set
        {
            base.ClassName = value;
            fieldAppearance.ClassName = value;

            FormDefinitionChanged();
        }
    }


    /// <summary>
    /// Coupled class name.
    /// </summary>
    public string CoupledClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Header actions control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            if (!UseCustomHeaderActions)
            {
                return base.HeaderActions;
            }

            return hdrActions;
        }
    }


    /// <summary>
    /// Webpart ID.
    /// </summary>
    public int WebPartId
    {
        get
        {
            return mWebPartId;
        }
        set
        {
            mWebPartId = value;

            FormDefinitionChanged();
        }
    }


    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get
        {
            return mMode;
        }
        set
        {
            mMode = value;
            fieldAppearance.Mode = value;
            fieldTypeSelector.Mode = value;
        }
    }


    /// <summary>
    /// Type of custom controls that can be selected from the control list in FieldEditor.
    /// </summary>
    public FieldEditorControlsEnum DisplayedControls
    {
        get
        {
            return mDisplayedControls;
        }
        set
        {
            mDisplayedControls = value;
            fieldAppearance.DisplayedControls = value;
        }
    }


    /// <summary>
    /// Form XML definition.
    /// </summary>
    public string FormDefinition
    {
        get
        {
            return mFormDefinition;
        }
        set
        {
            mFormDefinition = value;

            FormDefinitionChanged();
        }
    }

    /// <summary>
    /// Form XML definition of original object.
    /// </summary>
    public string OriginalFormDefinition
    {
        get
        {
            return mOriginalFormDefinition;
        }
        set
        {
            mOriginalFormDefinition = value;

            FormDefinitionChanged();
        }
    }


    /// <summary>
    /// Gets or sets the current edited form
    /// </summary>
    private new FormInfo FormInfo
    {
        get
        {
            if (!EnsureFormDefinition())
            {
                base.FormInfo = null;
            }
            return base.FormInfo;
        }
        set
        {
            base.FormInfo = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if control is placed in wizard.
    /// </summary>
    public bool IsWizard
    {
        get
        {
            return mIsWizard;
        }
        set
        {
            mIsWizard = value;
            fieldAdvancedSettings.IsWizard = value;
        }
    }


    /// <summary>
    /// Indicates if current form definition is inherited.
    /// </summary>
    private bool IsInheritedForm
    {
        get
        {
            switch (Mode)
            {
                case FieldEditorModeEnum.InheritedFormControl:
                case FieldEditorModeEnum.Widget:
                case FieldEditorModeEnum.InheritedWebPartProperties:
                case FieldEditorModeEnum.SystemWebPartProperties:
                    return true;

                default:
                    return false;
            }
        }
    }


    /// <summary>
    /// Indicates if fields without database representation can be created.
    /// </summary>
    public bool AllowDummyFields
    {
        get
        {
            return mAllowDummyFields;
        }
        set
        {
            mAllowDummyFields = value;
            fieldTypeSelector.AllowDummyFields = value;
        }
    }


    /// <summary>
    /// Indicates if extra fields can be created.
    /// </summary>
    public bool AllowExtraFields
    {
        get
        {
            return mAllowExtraFields;
        }
        set
        {
            mAllowExtraFields = value;
            fieldTypeSelector.AllowExtraFields = value;
        }
    }


    /// <summary>
    /// Indicates if Field Editor is used as alternative form.
    /// </summary>
    public bool IsAlternativeForm
    {
        get
        {
            switch (Mode)
            {
                case FieldEditorModeEnum.AlternativeClassFormDefinition:
                case FieldEditorModeEnum.AlternativeCustomTable:
                case FieldEditorModeEnum.AlternativeSystemTable:
                    return true;

                default:
                    return false;
            }
        }
    }


    /// <summary>
    /// Gets or sets alternative form full name.
    /// </summary>
    public string AlternativeFormFullName
    {
        get;
        set;
    }


    /// <summary>
    /// Enables or disables to edit <see cref='CMS.FormEngine.FormFieldInfo.Inheritable' /> settings.
    /// </summary>
    public bool ShowInheritanceSettings
    {
        get
        {
            return fieldAppearance.ShowInheritanceSettings;
        }
        set
        {
            fieldAppearance.ShowInheritanceSettings = value;
        }
    }


    /// <summary>
    /// Indicates if custom header actions should be used. Actions of current master page are used by default.
    /// </summary>
    public bool UseCustomHeaderActions
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the control is enabled. When set False, the control is in read-only mode.
    /// </summary>
    public bool Enabled
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Indicates if quick links can be displayed under the attribute list for selected fields.
    /// </summary>
    public bool ShowQuickLinks
    {
        get;
        set;
    } = true;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns True if system fields are enabled and one of them is selected.
    /// </summary>
    private bool IsSystemFieldSelected
    {
        get
        {
            return databaseConfiguration.IsSystemFieldSelected;
        }
    }


    /// <summary>
    /// Selected item name.
    /// </summary>
    private string SelectedItemName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SelectedItemName"], string.Empty);
        }
        set
        {
            ViewState["SelectedItemName"] = value;
        }
    }


    /// <summary>
    /// Selected item type.
    /// </summary>
    private FieldEditorSelectedItemEnum SelectedItemType
    {
        get
        {
            object obj = ViewState["SelectedItemType"];
            return (obj == null) ? 0 : (FieldEditorSelectedItemEnum)obj;
        }
        set
        {
            ViewState["SelectedItemType"] = value;
        }
    }


    /// <summary>
    /// Is field primary.
    /// </summary>
    private bool IsPrimaryField
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsPrimaryField"], false);
        }
        set
        {
            ViewState["IsPrimaryField"] = value;
        }
    }


    /// <summary>
    /// Indicates if field has no representation in database.
    /// </summary>
    private bool IsDummyField
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsDummyField"], false);
        }
        set
        {
            ViewState["IsDummyField"] = value;
        }
    }


    /// <summary>
    /// Indicates if dummy field is in original or alternative form
    /// </summary>
    private bool IsDummyFieldFromMainForm
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsDummyFieldFromMainForm"], false);
        }
        set
        {
            ViewState["IsDummyFieldFromMainForm"] = value;
        }
    }


    /// <summary>
    /// Indicates if field is extra field (field is not in original form definition).
    /// </summary>
    private bool IsExtraField
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsExtraField"], false);
        }
        set
        {
            ViewState["IsExtraField"] = value;
        }
    }


    /// <summary>
    /// Returns True if system fields are enabled and one of them is selected.
    /// </summary>
    private bool IsSystemField
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the form is not a derivation (i.e. inherited or alternative form) of some other form.
    /// </summary>
    private bool IsMainForm
    {
        get
        {
            return !IsAlternativeForm && !IsInheritedForm;
        }
    }


    /// <summary>
    /// Indicates if document type is edited.
    /// </summary>
    private bool IsDocumentType
    {
        get
        {
            object obj = ViewState["IsDocumentType"];
            if ((obj == null) && (!string.IsNullOrEmpty(ClassName)))
            {
                DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
                ViewState["IsDocumentType"] = ((dci != null) && dci.ClassIsDocumentType);
            }
            return ValidationHelper.GetBoolean(ViewState["IsDocumentType"], false);
        }
    }


    /// <summary>
    /// Gets or sets value indicating what item was selected in field type drop-down list.
    /// </summary>
    private string PreviousField
    {
        get
        {
            return ValidationHelper.GetString(ViewState["PreviousValue"], string.Empty);
        }
        set
        {
            ViewState["PreviousValue"] = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if detailed controls are visible.
    /// </summary>
    private bool FieldDetailsVisible
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["FieldDetailsVisible"], false);
        }
        set
        {
            ViewState["FieldDetailsVisible"] = value;
        }
    }


    /// <summary>
    /// Gets macro resolver name.
    /// </summary>
    private string ResolverName
    {
        get
        {
            string formName = string.IsNullOrEmpty(AlternativeFormFullName) ? ClassName : AlternativeFormFullName;
            return !string.IsNullOrEmpty(formName) ? FormHelper.FORM_PREFIX + formName : FormHelper.FORMDEFINITION_PREFIX + FormDefinition;
        }
    }

    #endregion


    #region "Global definitions"

    // Constants
    private const string newCategPreffix = "#categ##new#";
    private const string newFieldPreffix = "#field##new#";
    private const string categPreffix = "#categ#";
    private const string fieldPreffix = "#field#";
    private const int preffixLength = 7; // Length of categPreffix = length of fieldPreffix = 7

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (StopProcessing)
        {
            return;
        }

        ControlsHelper.EnsureScriptManager(Page);
        ScriptHelper.RegisterRestoreFocusScript(Page);

        // Set method delegates
        fieldAppearance.GetControls = GetControls;
    }


    protected override void LoadViewState(object savedState)
    {
        base.LoadViewState(savedState);

        if (ViewState["CurrentFormFields"] != null)
        {
            // Refresh uicontext data
            UIContext["CurrentFormFields"] = ViewState["CurrentFormFields"];
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            return;
        }

        CreateHeaderActions();

        fieldAdvancedSettings.ResolverName = controlSettings.ResolverName = cssSettings.ResolverName = htmlEnvelope.ResolverName = ResolverName;
        validationSettings.ResolverName = fieldAppearance.ResolverName = databaseConfiguration.ResolverName = categoryEdit.ResolverName = ResolverName;

        fieldAdvancedSettings.ShowDisplayInSimpleModeCheckBox = ((Mode == FieldEditorModeEnum.FormControls) || (Mode == FieldEditorModeEnum.InheritedFormControl));

        pnlDevelopmentMode.Visible = SystemContext.DevelopmentMode && !IsWizard;

        // Set images url
        btnDeleteItem.ToolTip = GetString("TemplateDesigner.DeleteItem");
        btnDownAttribute.ToolTip = GetString("TemplateDesigner.DownAttribute");
        btnUpAttribute.ToolTip = GetString("TemplateDesigner.UpAttribute");

        ltlConfirmText.Text = "<input type=\"hidden\" id=\"confirmdelete\" value=\"" + GetString("TemplateDesigner.ConfirmDelete") + "\"/>";
        btnDeleteItem.OnClientClick = "if (!confirmDelete()) { return false; }";
        btnUpAttribute.Enabled = true;
        btnDownAttribute.Enabled = true;
        btnDeleteItem.Enabled = true;

        btnAdd.Actions.Add(new CMSButtonAction
        {
            Text = GetString("TemplateDesigner.NewAttribute"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(btnNewAttribute) + "; return false;"
        });
        btnAdd.Actions.Add(new CMSButtonAction
        {
            Text = GetString("TemplateDesigner.NewCategory"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(btnNewCategory) + "; return false;"
        });

        databaseConfiguration.LoadGroupField();

        if (!RequestHelper.IsPostBack())
        {
            // Preselect field if query-string parameter is set
            string preselectedField = QueryHelper.GetString("selectedfield", string.Empty).Replace("%23", "#");
            Reload(preselectedField);
        }
        else
        {
            if (FormInfo == null)
            {
                // Form definition was not loaded
                return;
            }

            ffi = FormInfo.GetFormField(SelectedItemName);
            LoadControlSettings(PreviousField);
            LoadValidationSettings();
        }

        // Register event handlers
        fieldTypeSelector.OnSelectionChanged += fieldTypeSelector_OnSelectionChanged;
        databaseConfiguration.DropChanged += databaseConfiguration_DropChanged;
        databaseConfiguration.AttributeChanged += databaseConfiguration_AttributeChanged;
        fieldAppearance.OnFieldSelected += control_FieldSelected;
        documentSource.OnSourceFieldChanged += documentSource_OnSourceFieldChanged;

        plcValidation.Visible = true;
        plcQuickValidation.Visible = true;
        plcSettings.Visible = true;
        pnlContent.Enabled = true;

        // Ensure save action and disable it if necessary
        EnsureHeaderActions(false);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display controls and quick links according to current mode
        bool displayDetails = FieldDetailsVisible && chkDisplayInForm.Checked;

        fieldAppearance.Visible = plcQuickAppearance.Visible = displayDetails;
        fieldAdvancedSettings.Visible = displayDetails;
        cssSettings.Visible = plcQuickStyles.Visible = displayDetails;
        htmlEnvelope.Visible = plcQuickHtmlEnvelope.Visible = displayDetails;
        validationSettings.DisplayControls();
        validationSettings.Visible = plcQuickValidation.Visible = displayDetails && validationSettings.Visible;
        controlSettings.CheckVisibility();
        controlSettings.Visible = plcQuickSettings.Visible = displayDetails && controlSettings.Visible;

        plcFieldType.Visible = IsNewItemEdited;

        // Display action buttons
        DisplayOrHideActions();

        // Display and store last value
        PreviousField = fieldAppearance.FieldType;

        // Hide quick links if selected item is not field or of only 'database quick link' should be displayed
        pnlQuickSelect.Visible = ShowQuickLinks && (SelectedItemType == FieldEditorSelectedItemEnum.Field) && (plcQuickAppearance.Visible || plcQuickSettings.Visible || plcQuickValidation.Visible || plcQuickStyles.Visible);

        var master = Page.Master as ICMSMasterPage;
        if (master != null)
        {
            var contentPanel = master.PanelContent;
            if (!IsWizard && (contentPanel != null))
            {
                contentPanel.CssClass = string.Empty;
            }
        }

        // Highlight attribute list items
        HighlightListItems();

        // Ensure save action and disable it if necessary
        EnsureHeaderActions(true);
    }

    #endregion


    #region "Action events"

    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerInvariant())
        {
            case "save":
                SaveField();
                break;
            case "reset":
                ResetFormElementToOriginal();
                break;
        }
    }


    /// <summary>
    /// Handle Save's OnClick event if edited object supports locking.
    /// </summary>
    protected void ObjectManager_OnSaveData(object sender, SimpleObjectManagerEventArgs e)
    {
        SaveField();
    }


    /// <summary>
    /// Reloads data after undocheckout.
    /// </summary>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        switch (e.ActionName)
        {
            case ComponentEvents.UNDO_CHECKOUT:
                // Reload the page and preselect current field/category
                URLHelper.ResponseRedirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "selectedfield", lstAttributes.SelectedValue.Replace("#", "%23")));
                break;
            case ComponentEvents.CHECKOUT:
                // Set control settings section
                controlSettings.AllowModeSwitch = true;
                controlSettings.SimpleMode = true;
                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reload field editor.
    /// </summary>
    /// <param name="selectedValue">Selected field in field list</param>
    /// <param name="partialReload">Indicates if not all controls need to be reloaded</param>
    public void Reload(string selectedValue, bool partialReload = false)
    {
        bool isModeSelected = false;
        bool isItemSelected = false;

        // Check for alternative form mode
        if ((IsAlternativeForm) || (IsInheritedForm))
        {
            if (!string.IsNullOrEmpty(FormDefinition))
            {
                isModeSelected = true;
            }
            else
            {
                // Clear item list
                lstAttributes.Items.Clear();
            }
        }
        else
        {
            switch (Mode)
            {
                case FieldEditorModeEnum.General:
                case FieldEditorModeEnum.FormControls:
                case FieldEditorModeEnum.ProcessActions:
                case FieldEditorModeEnum.EmailWidget:
                    if (!string.IsNullOrEmpty(FormDefinition))
                    {
                        isModeSelected = true;
                    }
                    else
                    {
                        // Clear item list
                        lstAttributes.Items.Clear();
                    }
                    break;

                case FieldEditorModeEnum.ClassFormDefinition:
                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.CustomTable:
                    if (!string.IsNullOrEmpty(ClassName))
                    {
                        isModeSelected = true;
                    }
                    else
                    {
                        ShowError(GetString("fieldeditor.noclassname"));
                    }
                    break;

                case FieldEditorModeEnum.WebPartProperties:
                case FieldEditorModeEnum.SystemWebPartProperties:
                    if ((WebPartId > 0))
                    {
                        isModeSelected = true;
                    }
                    else
                    {
                        ShowError(GetString("fieldeditor.nowebpartid"));
                    }
                    break;

                case FieldEditorModeEnum.PageTemplateProperties:
                    isModeSelected = true;
                    break;

                default:
                    ShowError(GetString("fieldeditor.nomode"));
                    break;
            }
        }

        if (!partialReload)
        {
            // Display controls if mode is determined
            ShowOrHideFieldDetails(false);
        }

        if (isModeSelected)
        {
            isItemSelected = LoadInnerControls(selectedValue, partialReload);
        }

        // Hide controls when item isn't selected
        if ((!partialReload) && (!isItemSelected))
        {
            HideAllPanels();
            disableSaveAction = true;
            btnUpAttribute.Enabled = false;
            btnDownAttribute.Enabled = false;
            btnDeleteItem.Enabled = false;

            // Enable action buttons only when the form definition was parsed successfully
            if (FormInfo != null)
            {
                // Only new items can be added
                btnNewAttribute.Enabled = true;

                // Only new items can be added
                btnNewCategory.Enabled = true;

                ShowInformation(GetString("fieldeditor.nofieldsdefined"));
            }
        }

        if (documentSource.VisibleContent)
        {
            documentSource.Reload();
        }

        // Prepare Reset button and information messages
        PrepareResetButton();
        DisplayFieldInformation();
    }


    /// <summary>
    /// Creates header actions - save and reset buttons.
    /// </summary>
    private void CreateHeaderActions()
    {
        if (!UseCustomHeaderActions && HeaderActions == null)
        {
            return;
        }

        // Create Reset button
        btnReset = new HeaderAction
        {
            CommandName = "reset",
            Visible = false
        };

        if (UseCustomHeaderActions)
        {
            // Add save action
            btnSave = new SaveAction();
            btnSave.Enabled = Enabled;
            HeaderActions.AddAction(btnSave);
            HeaderActions.AddAction(btnReset);
            pnlHeaderActions.Visible = pnlHeaderPlaceHolder.Visible = true;
            pnlContentPadding.CssClass += " Menu";
        }
        else
        {
            ObjectEditMenu menu = ControlsHelper.GetChildControl<ObjectEditMenu>(Page);
            if (menu != null)
            {
                menu.AddExtraAction(btnReset);
                menu.AllowSave = Enabled;
            }
            else if (HeaderActions != null)
            {
                HeaderActions.AddAction(btnReset);
            }
        }

        controlSettings.BasicForm.EnsureMessagesPlaceholder(MessagesPlaceHolder);
    }


    /// <summary>
    /// Ensures header actions.
    /// </summary>
    private void EnsureHeaderActions(bool reload)
    {
        if (HeaderActions == null)
        {
            return;
        }

        if (FormInfo == null)
        {
            // Disable filed editor when the form definition was not parsed successfully
            HeaderActions.Enabled = false;
            pnlButtons.Enabled = false;
            return;
        }

        if (HeaderActions.ActionsList.Exists(a => (a is SaveAction && a.Visible)))
        {
            // Get save from default page actions
            var saveAction = (SaveAction)HeaderActions.ActionsList.Single(a => a is SaveAction);
            if ((btnSave == null) || (btnSave != saveAction))
            {
                btnSave = saveAction;
            }
        }

        if (btnSave == null)
        {
            // Create new action
            btnSave = new SaveAction();
            btnSave.Enabled = Enabled;

            HeaderActions.InsertAction(-2, btnSave);
            if (reload)
            {
                HeaderActions.ReloadData();
            }
        }

        if (!UseCustomHeaderActions)
        {
            if (HeaderActions.MessagesPlaceHolder != null)
            {
                HeaderActions.MessagesPlaceHolder.OffsetX = 250;
            }

            var manager = CMSObjectManager.GetCurrent(Page);
            if (manager != null)
            {
                manager.ShowPanel = true;
                manager.OnSaveData += ObjectManager_OnSaveData;
                manager.OnAfterAction += ObjectManager_OnAfterAction;
            }
        }

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        if (reload)
        {
            if (btnSave != null && disableSaveAction)
            {
                // Disable save action
                btnSave.Enabled = false;
            }

            // Hide action buttons if edited object is checked in or checked out by another user
            if (((btnSave == null || !btnSave.Enabled) && (IsObjectChecked())) || !Enabled)
            {
                // Disable control elements
                pnlContent.Enabled = btnAdd.Enabled = documentSource.Enabled = btnUpAttribute.Enabled = btnDownAttribute.Enabled = btnNewCategory.Enabled
                    = btnNewAttribute.Enabled = btnDeleteItem.Enabled = false;

                // Disable mode switch for disabled dialog
                controlSettings.AllowModeSwitch = false;
                controlSettings.SimpleMode = false;
            }
        }
    }


    /// <summary>
    /// Returns true if edited object is checked in or checked out by other user.
    /// </summary>
    private bool IsObjectChecked()
    {
        bool result = false;

        ObjectEditMenu menu = ControlsHelper.GetChildControl<ObjectEditMenu>(Page);
        if (menu != null)
        {
            result = ((menu.ShowCheckOut || ((menu.InfoObject != null) && (menu.InfoObject.Generalized.IsCheckedOut)))
                && SynchronizationHelper.UseCheckinCheckout);
        }

        return result;
    }


    /// <summary>
    /// Highlights attribute list items like categories, primary keys and hidden fields.
    /// </summary>
    private void HighlightListItems()
    {
        if (FormInfo == null)
        {
            // Form definition was not loaded
            return;
        }

        if (lstAttributes.Items.Count > 0)
        {
            foreach (ListItem li in lstAttributes.Items)
            {
                // Mark category item with different color
                string cssClass;
                if (li.Value.StartsWith(categPreffix, StringComparison.OrdinalIgnoreCase))
                {
                    cssClass = "field-editor-category-item";

                    FormCategoryInfo formCategory = FormInfo.GetFormCategory(li.Value.Substring(preffixLength));
                    if ((formCategory != null) && (formCategory.IsDummy || formCategory.IsExtra))
                    {
                        // Highlight dummy categories
                        cssClass += " field-editor-dummy-category";
                    }

                    li.Attributes.Add("class", cssClass);
                }
                else
                {
                    cssClass = string.Empty;

                    // Get form field info
                    FormFieldInfo formField = FormInfo.GetFormField(li.Value.Substring(preffixLength));
                    if (formField != null)
                    {
                        if (DevelopmentMode && formField.PrimaryKey)
                        {
                            // Highlight primary keys in the list
                            cssClass = "field-editor-primary-attribute";
                        }
                        if (!formField.Visible)
                        {
                            if (!string.IsNullOrEmpty(cssClass))
                            {
                                cssClass += " ";
                            }
                            // Highlight fields that are not visible
                            cssClass += "field-editor-hidden-item";
                        }
                        if ((formField.IsDummyField) || (formField.IsExtraField))
                        {
                            if (!string.IsNullOrEmpty(cssClass))
                            {
                                cssClass += " ";
                            }
                            // Highlight dummy fields
                            cssClass += "field-editor-dummy-field";
                        }
                        if (!string.IsNullOrEmpty(cssClass))
                        {
                            li.Attributes.Add("class", cssClass);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Loads inner FieldEditor controls.
    /// </summary>
    /// <returns>Returns TRUE if any item is selected</returns>
    private bool LoadInnerControls(string selectedValue, bool partialReload)
    {
        bool isItemSelected = false;

        if (FormInfo == null)
        {
            // Form definition was not loaded
            return false;
        }

        if (!partialReload)
        {
            LoadAttributesList(selectedValue);
        }

        documentSource.FormInfo = FormInfo;
        documentSource.IsAlternativeForm = IsAlternativeForm;
        documentSource.IsInheritedForm = IsInheritedForm;
        documentSource.ClassName = ClassName;

        if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
        {
            isItemSelected = true;
            DisplaySelectedTabContent();
            ffi = FormInfo.GetFormField(SelectedItemName);
            LoadSelectedField(partialReload);
        }
        else if (SelectedItemType == FieldEditorSelectedItemEnum.Category)
        {
            isItemSelected = true;
            LoadSelectedCategory();
        }
        return isItemSelected;
    }


    /// <summary>
    /// Ensures Form definition is loaded. Returns false on fail.
    /// </summary>
    /// <returns>True on success, false on failure</returns>
    private bool EnsureFormDefinition()
    {
        return (base.FormInfo != null) || LoadFormDefinition();
    }


    /// <summary>
    /// Load xml definition of the form.
    /// </summary>
    /// <returns>True if the load is successful.</returns>
    private bool LoadFormDefinition()
    {
        bool isError = false;

        if (!IsAlternativeForm && !IsInheritedForm)
        {
            switch (Mode)
            {
                case FieldEditorModeEnum.General:
                    // Definition is loaded from external xml
                    break;

                case FieldEditorModeEnum.WebPartProperties:
                    // Load xml definition from web part info
                    WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(WebPartId);
                    if (wpi != null)
                    {
                        FormDefinition = wpi.WebPartProperties;
                    }
                    else
                    {
                        isError = true;
                    }
                    break;

                case FieldEditorModeEnum.ClassFormDefinition:
                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.CustomTable:
                    // Load xml definition from Class info
                    DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
                    if (dci != null)
                    {
                        FormDefinition = dci.ClassFormDefinition;
                    }
                    else
                    {
                        isError = true;
                    }
                    break;
            }
        }
        else
        {
            isError = String.IsNullOrEmpty(FormDefinition);
        }

        try
        {
            FormInfo = new FormInfo(FormDefinition);
        }
        catch
        {
            // No need to log event - it has been already logged
            isError = true;
        }

        if (isError)
        {
            ShowError("[FieldEditor.LoadFormDefinition()]: " + GetString("FieldEditor.XmlDefinitionNotLoaded"));
            return false;
        }

        InitUIContext(FormInfo);

        return true;
    }


    /// <summary>
    /// Fill attribute list.
    /// </summary>
    /// <param name="selectedValue">Selected value in attribute list, null if first item is selected</param>
    private void LoadAttributesList(string selectedValue)
    {
        // Reload list only if new item is not edited
        if (!IsNewItemEdited)
        {
            // Clear item list
            lstAttributes.Items.Clear();

            // Get all list items (fields and categories)        
            var itemList = FormInfo.GetFormElements(true, true);

            if (itemList != null)
            {
                MacroResolver resolver = MacroResolverStorage.GetRegisteredResolver(ResolverName);
                foreach (IDataDefinitionItem item in itemList)
                {
                    string itemDisplayName = null;
                    string itemCodeName = null;
                    if (item is FormFieldInfo)
                    {
                        FormFieldInfo formField = ((FormFieldInfo)(item));

                        itemDisplayName = formField.Name;
                        if (!formField.AllowEmpty)
                        {
                            itemDisplayName += ResHelper.RequiredMark;
                        }

                        itemCodeName = fieldPreffix + formField.Name;
                    }
                    else if (item is FormCategoryInfo)
                    {
                        FormCategoryInfo formCategory = ((FormCategoryInfo)(item));

                        itemDisplayName = ResHelper.LocalizeString(formCategory.GetPropertyValue(FormCategoryPropertyEnum.Caption, resolver));
                        if (String.IsNullOrEmpty(itemDisplayName))
                        {
                            itemDisplayName = formCategory.CategoryName;
                        }
                        itemCodeName = categPreffix + formCategory.CategoryName;
                    }

                    // Load list box
                    if (!String.IsNullOrEmpty(itemDisplayName) && !String.IsNullOrEmpty(itemCodeName))
                    {
                        lstAttributes.Items.Add(new ListItem(itemDisplayName, itemCodeName));
                    }
                }
            }

            // Set selected item
            if (lstAttributes.Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(selectedValue) && (lstAttributes.Items.FindByValue(selectedValue) != null))
                {
                    lstAttributes.SelectedValue = selectedValue;
                }
                else
                {
                    // Select first item of the list       
                    lstAttributes.SelectedIndex = 0;
                }
            }

            // Default values - list is empty
            SelectedItemName = null;
            SelectedItemType = 0;

            // Save selected item info
            if (lstAttributes.SelectedValue != null)
            {
                if (lstAttributes.SelectedValue.StartsWith(fieldPreffix, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedItemName = lstAttributes.SelectedValue.Substring(preffixLength);
                    SelectedItemType = FieldEditorSelectedItemEnum.Field;
                }
                else if (lstAttributes.SelectedValue.StartsWith(categPreffix, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedItemName = lstAttributes.SelectedValue.Substring(preffixLength);
                    SelectedItemType = FieldEditorSelectedItemEnum.Category;
                }
            }
        }
    }


    /// <summary>
    /// Sets all values of the category edit form to defaults.
    /// </summary>
    private void LoadDefaultCategoryEditForm()
    {
        plcCategory.Visible = true;
        plcField.Visible = false;
        categoryEdit.Reload();
    }


    /// <summary>
    /// Sets all values of form to defaults.
    /// </summary>
    /// <param name="partialReload">True - indicates that only some controls should be loaded, False - reload all controls</param>
    private void LoadDefaultAttributeEditForm(bool partialReload)
    {
        ffi = null;
        plcCategory.Visible = false;
        chkDisplayInForm.Checked = true;

        if (!partialReload)
        {
            databaseConfiguration.DevelopmentMode = DevelopmentMode;
            databaseConfiguration.ShowSystemFields = IsSystemField;
            databaseConfiguration.IsDocumentType = IsDocumentType;
            databaseConfiguration.Mode = Mode;
            databaseConfiguration.ClassName = ClassName;
            databaseConfiguration.CoupledClassName = CoupledClassName;
            databaseConfiguration.IsAlternativeForm = IsAlternativeForm;
            databaseConfiguration.IsInheritedForm = IsInheritedForm;
            databaseConfiguration.IsDummyField = IsDummyField;
            databaseConfiguration.IsDummyFieldFromMainForm = IsDummyFieldFromMainForm;
            databaseConfiguration.IsExtraField = IsExtraField;
            databaseConfiguration.IsFieldPrimary = IsPrimaryField;
            databaseConfiguration.Reload(null, IsNewItemEdited);
            databaseConfiguration.ShowDefaultControl();
        }

        if (IsSystemField)
        {
            LoadSystemField();
        }

        var defaultControl = FormHelper.GetFormFieldDefaultControlName(databaseConfiguration.AttributeType, GetControls(DisplayedControls, Mode, DevelopmentMode));

        fieldAppearance.AttributeType = databaseConfiguration.AttributeType;
        fieldAppearance.FieldType = defaultControl;
        fieldAppearance.LoadFieldTypes(IsPrimaryField);
        fieldAppearance.ClassName = ClassName;
        fieldAppearance.Reload();
        fieldAdvancedSettings.Reload();
        cssSettings.Reload();
        htmlEnvelope.Reload();
        LoadValidationSettings();
        validationSettings.DisplayControls();
        validationSettings.Reload();
    }


    /// <summary>
    /// Fill form with selected category data.
    /// </summary>    
    private void LoadSelectedCategory()
    {
        plcField.Visible = false;
        plcCategory.Visible = true;

        fci = FormInfo.GetFormCategory(SelectedItemName);

        categoryEdit.CategoryInfo = fci;
        categoryEdit.Reload();

        if (fci != null)
        {
            HandleInherited(fci.IsInherited);
            IsDummyField = fci.IsDummy;
            IsExtraField = fci.IsExtra;
            IsDummyFieldFromMainForm = false;
        }
        else
        {
            LoadDefaultCategoryEditForm();
        }
    }


    /// <summary>
    /// Displays controls for field editing.
    /// </summary>
    private void ShowFieldOptions()
    {
        plcCategory.Visible = false;
        databaseConfiguration.Visible = true;
        controlSettings.Visible = true;
        fieldAppearance.Visible = true;
        fieldAdvancedSettings.Visible = true;
        validationSettings.Visible = true;
        cssSettings.Visible = true;
        htmlEnvelope.Visible = true;
        FieldDetailsVisible = true;
    }


    /// <summary>
    /// Handles the inheritance of the field.
    /// </summary>
    /// <param name="inherited">Indicates if field is inherited</param>
    /// <param name="isPrimaryKey">Indicates if field is primary key</param>
    private void HandleInherited(bool inherited, bool isPrimaryKey = false)
    {
        pnlField.Enabled = true;
        btnDeleteItem.Visible = true;

        ShowInformation(String.Empty);

        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
        if (dci == null)
        {
            return;
        }

        if (inherited && !IsAlternativeForm)
        {
            // Get information on inherited class
            DataClassInfo parentCi = DataClassInfoProvider.GetDataClassInfo(dci.ClassInheritsFromClassID);
            if (parentCi != null)
            {
                pnlField.Enabled = false;
                btnDeleteItem.Visible = false;
                disableSaveAction = true;

                ShowInformation(String.Format(GetString("DocumentType.FieldIsInherited"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(parentCi.ClassDisplayName))));
            }
        }
        else if (!isPrimaryKey && databaseConfiguration.HasDependentChildren)
        {
            ShowInformation(String.Format(GetString("documenttype.datatypechangeerror"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(databaseConfiguration.DependentClassDisplayName))));
        }
    }


    /// <summary>
    /// Handles inheritance of selected form item.
    /// </summary>
    private void HandleItemInheritance()
    {
        bool inherited = false;
        bool isPrimaryKey = false;

        switch (SelectedItemType)
        {
            case FieldEditorSelectedItemEnum.Field:
                var field = FormInfo.GetFormField(SelectedItemName);
                inherited = field.IsInherited;
                isPrimaryKey = field.PrimaryKey;
                break;
            case FieldEditorSelectedItemEnum.Category:
                inherited = FormInfo.GetFormCategory(SelectedItemName).IsInherited;
                break;
        }

        HandleInherited(inherited, isPrimaryKey);
    }


    /// <summary>
    /// Fill form with selected attribute data.
    /// </summary>    
    /// <param name="partialReload">Indicates if only some controls should be reloaded</param>
    private void LoadSelectedField(bool partialReload)
    {
        // Fill form
        if (ffi != null)
        {
            if (!partialReload)
            {
                databaseConfiguration.DevelopmentMode = DevelopmentMode;
                databaseConfiguration.ShowSystemFields = ffi.External;
                databaseConfiguration.FieldInfo = ffi;
                databaseConfiguration.IsDocumentType = IsDocumentType;
                databaseConfiguration.Mode = Mode;
                databaseConfiguration.IsAlternativeForm = IsAlternativeForm;
                databaseConfiguration.IsInheritedForm = IsInheritedForm;
                databaseConfiguration.ClassName = ClassName;
                databaseConfiguration.CoupledClassName = CoupledClassName;
                databaseConfiguration.IsDummyField = ffi.IsDummyField;
                databaseConfiguration.IsDummyFieldFromMainForm = ffi.IsDummyFieldFromMainForm;
                databaseConfiguration.IsExtraField = ffi.IsExtraField;
                databaseConfiguration.IsFieldPrimary = ffi.PrimaryKey;
                databaseConfiguration.Reload(ffi.Name, IsNewItemEdited);
            }

            HandleInherited(ffi.IsInherited, ffi.PrimaryKey);

            IsPrimaryField = ffi.PrimaryKey;
            IsDummyField = ffi.IsDummyField;
            IsDummyFieldFromMainForm = ffi.IsDummyFieldFromMainForm;
            IsExtraField = ffi.IsExtraField;

            if (!partialReload)
            {
                // Control is not valid if it is visible but has no control name set
                var controlIsInvalid = !ffi.Visible || !String.IsNullOrEmpty(ffi.GetControlName());

                chkDisplayInForm.Checked = ffi.Visible && controlIsInvalid;
            }

            DisplaySelectedTabContent();
            ShowFieldOptions();

            if (chkDisplayInForm.Checked && fieldAppearance.Visible)
            {
                fieldAppearance.Mode = Mode;
                fieldAppearance.ClassName = ClassName;
                fieldAppearance.FieldInfo = ffi;
                fieldAppearance.AttributeType = databaseConfiguration.AttributeType;
                fieldAppearance.Reload();
            }

            if (chkDisplayInForm.Checked && fieldAdvancedSettings.Visible)
            {
                fieldAdvancedSettings.FieldInfo = ffi;
                fieldAdvancedSettings.Reload();
            }

            if (chkDisplayInForm.Checked && validationSettings.Visible)
            {
                LoadValidationSettings();
                validationSettings.DisplayControls();
                validationSettings.Reload();
            }

            if (chkDisplayInForm.Checked && cssSettings.Visible)
            {
                cssSettings.FieldInfo = ffi;
                cssSettings.Reload();
                cssSettings.Enabled = true;
            }

            if (chkDisplayInForm.Checked && htmlEnvelope.Visible)
            {
                htmlEnvelope.FieldInfo = ffi;
                htmlEnvelope.Reload();
                htmlEnvelope.Enabled = true;
            }
        }
        else
        {
            IsSystemField = false;
            LoadDefaultAttributeEditForm(partialReload);
        }

        LoadControlSettings();
    }


    /// <summary>
    /// Loads validation settings.
    /// </summary>
    private void LoadValidationSettings()
    {
        validationSettings.IsPrimary = IsPrimaryField;
        validationSettings.FieldInfo = ffi;
        validationSettings.Mode = Mode;
    }


    /// <summary>
    /// Displays or hides actions according to the selected mode.
    /// </summary>
    protected void DisplayOrHideActions()
    {
        // Hide actions if new fields are not allowed
        if ((!AllowDummyFields && IsAlternativeForm) || (!AllowExtraFields && IsInheritedForm))
        {
            plcMainButtons.Visible = false;
        }
        else
        {
            // Deny delete original field in alternative or inherited form
            if (((!IsDummyField || IsDummyFieldFromMainForm) && IsAlternativeForm) || (!IsExtraField && IsInheritedForm))
            {
                btnDeleteItem.Visible = false;
            }
        }

        if (Mode == FieldEditorModeEnum.SystemWebPartProperties)
        {
            // Hide all buttons in webpart system properties
            pnlButtons.Visible = false;
        }
    }


    protected void lnkFormDef_Click(object sender, EventArgs e)
    {
        GetFormDefinition();
    }


    /// <summary>
    /// Outputs the form definition XML of the current view
    /// </summary>
    protected void GetFormDefinition()
    {
        if (FormInfo == null)
        {
            // Form definition was not loaded
            return;
        }

        string xml = HTMLHelper.ReformatHTML(FormInfo.GetXmlDefinition());

        // Write directly to response
        Response.Clear();
        Response.AddHeader("Content-Disposition", "attachment; filename=formDefinition.xml");
        Response.ContentType = "text/xml";
        Response.Write(xml);
        Response.End();
    }


    protected void lnkHideAllFields_OnClick(object sender, EventArgs e)
    {
        // Make visible fields invisible
        ChangeFieldsVisibility(false);
    }


    protected void lnkShowAllFields_OnClick(object sender, EventArgs e)
    {
        // Make invisible fields visible
        ChangeFieldsVisibility(true);
    }


    /// <summary>
    /// Changes visible attribute of all fields in the form definition (except the primary-key field).
    /// </summary>
    /// <param name="makeVisible">Makes fields visible if true</param>
    private void ChangeFieldsVisibility(bool makeVisible)
    {
        RaiseBeforeDefinitionUpdate();

        if (FormInfo == null)
        {
            // Form definition was not loaded
            return;
        }

        // Get visible or invisible fields
        var fields = FormInfo.GetFields(!makeVisible, makeVisible);

        foreach (FormFieldInfo field in fields)
        {
            if (!makeVisible || !field.PrimaryKey)
            {
                field.Visible = makeVisible;
            }
        }

        SaveFormDefinition();

        ClearHashtables();

        ShowChangesSaved();

        RaiseAfterDefinitionUpdate();

        // Reload to apply visibility and inheriting changes
        Reload(lstAttributes.SelectedValue);
    }


    /// <summary>
    /// Sets properties of Reset button.
    /// </summary>
    protected void PrepareResetButton()
    {
        if (IsAlternativeForm || IsInheritedForm)
        {
            btnReset.Visible = true;
            btnReset.Enabled = false;

            if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
            {
                btnReset.Text = GetString("FieldEditor.ResetField");
                btnReset.Tooltip = GetString("FieldEditor.ResetFieldTooltip");
                btnReset.OnClientClick = "return confirm('" + GetString("fieldeditor.ResetFieldToDefaultConfirmation") + "')";
            }
            else
            {
                btnReset.Text = GetString("FieldEditor.ResetCategory");
                btnReset.Tooltip = GetString("FieldEditor.ResetCategoryTooltip");
                btnReset.OnClientClick = "return confirm('" + GetString("fieldeditor.ResetCategoryToDefaultConfirmation") + "')";
            }

            if ((!String.IsNullOrEmpty(SelectedItemName)) && (!string.IsNullOrEmpty(OriginalFormDefinition)))
            {
                if ((!IsDummyField || IsDummyFieldFromMainForm) && !IsExtraField)
                {
                    // Get difference between current and original form definition
                    string diff = FormHelper.GetFormDefinitionDifference(OriginalFormDefinition, FormDefinition, true);

                    if (!string.IsNullOrEmpty(diff))
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(diff);

                        // Select proper node
                        XmlNode item = document.SelectSingleNode((SelectedItemType == FieldEditorSelectedItemEnum.Field ? "//field[@column='" : "//category[@name='") + SelectedItemName + "']");
                        if (item != null)
                        {
                            // Display Reset button if item can be reset
                            btnReset.Enabled = Enabled && IsInheritedItemModified(item);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Returns true if the item contains any changes.
    /// Item is modified if there are any child nodes or if there are any attributes besides field name, order and guid.
    /// </summary>
    private bool IsInheritedItemModified(XmlNode item)
    {
        if (item.ChildNodes.Count > 0)
        {
            return true;
        }

        var attrs = item.Attributes;
        if (attrs == null)
        {
            return false;
        }

        var notRelevantAttributes = new[]
        {
            "column",
            "guid",
            "name",
            "order"
        };

        foreach (XmlAttribute attribute in attrs)
        {
            if (!notRelevantAttributes.Contains(attribute.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Resets form element properties to original form.
    /// </summary>
    protected void ResetFormElementToOriginal()
    {
        // Get difference between current and original form definitions
        string diff = FormHelper.GetFormDefinitionDifference(OriginalFormDefinition, FormDefinition, true);

        if (!string.IsNullOrEmpty(diff))
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(diff);

            // Select proper node
            var element = (XmlElement)document.SelectSingleNode((SelectedItemType == FieldEditorSelectedItemEnum.Field ? "//field[@column='" : "//category[@name='") + SelectedItemName + "']");
            if (element != null)
            {
                // Check if the element is in the original definition
                if (ElementInOriginalDefinition())
                {
                    // Get 'order attribute if set
                    int order = ValidationHelper.GetInteger(XmlHelper.GetAttributeValue(element, "order"), -1);

                    // Clear the element
                    element.RemoveAll();
                    element.Attributes.RemoveAll();

                    // Add new attributes
                    var attributes = new Dictionary<string, string>();

                    var colName = (SelectedItemType == FieldEditorSelectedItemEnum.Field ? "column" : "name");
                    attributes[colName] = SelectedItemName;

                    if (order >= 0)
                    {
                        attributes["order"] = order.ToString();
                    }

                    element.AddAttributes(attributes);
                }
                else
                {
                    // Remove corrupted element
                    document.FirstChild.RemoveChild(element);
                }

                diff = document.OuterXml;
            }
            FormDefinition = FormHelper.MergeFormDefinitions(OriginalFormDefinition, diff);

            RaiseAfterDefinitionUpdate();
        }

        Reload(lstAttributes.SelectedValue);
        ShowChangesSaved();
    }


    /// <summary>
    /// Returns true if currently selected element (field/category) is in the original form definition.
    /// </summary>
    private bool ElementInOriginalDefinition()
    {
        // Try to find the element in the original definition
        XmlDocument origDocument = new XmlDocument();
        origDocument.LoadXml(OriginalFormDefinition);

        XmlNode origElem = origDocument.SelectSingleNode((SelectedItemType == FieldEditorSelectedItemEnum.Field ? "//field[@column='" : "//category[@name='") + SelectedItemName + "']");

        return (origElem != null);
    }


    /// <summary>
    /// Displays information messages for form elements.
    /// </summary>
    protected void DisplayFieldInformation()
    {
        bool isField = SelectedItemType == FieldEditorSelectedItemEnum.Field;

        if (IsDummyField)
        {
            AddInformation(isField ? GetString("fieldeditor.dummyfield") : GetString("fieldeditor.extracategory"));
        }

        if (IsExtraField)
        {
            AddInformation(isField ? GetString("fieldeditor.extrafield") : GetString("fieldeditor.extracategory"));
        }

        if ((!IsExtraField && (!IsDummyField || IsDummyFieldFromMainForm)) && (IsAlternativeForm || IsInheritedForm))
        {
            // Special text for system webpart(widget) properties
            if (Mode == FieldEditorModeEnum.SystemWebPartProperties)
            {
                if (btnReset.Enabled)
                {
                    AddInformation(isField ? GetString("fieldeditor.propertysystemismodified") : GetString("fieldeditor.categorysystemismodified"), " ");
                }
                else
                {
                    AddInformation(isField ? GetString("fieldeditor.propertydefaultsystemsettings") : GetString("fieldeditor.categorydefaultsystemsettings"), " ");
                }
            }
            else
            {
                AddInformation(isField ? GetString("FieldEditor.FieldIsInherited") : GetString("FieldEditor.CategoryIsInherited"), " ");

                if (btnReset.Enabled)
                {
                    AddInformation(isField ? GetString("FieldEditor.FieldIsModified") : GetString("FieldEditor.CategoryIsModified"), " ");
                }
            }
        }
    }


    /// <summary>
    /// Saves the form definition and refreshes the form.
    /// </summary>
    protected void SaveFormDefinition()
    {
        // Update form definition
        FormDefinition = FormInfo.GetXmlDefinition();

        if ((!IsAlternativeForm) && (!IsInheritedForm))
        {
            switch (Mode)
            {
                case FieldEditorModeEnum.WebPartProperties:
                    // Save xml string to CMS_WebPart table
                    var wpi = WebPartInfoProvider.GetWebPartInfo(WebPartId);
                    if (wpi != null)
                    {
                        wpi.WebPartProperties = FormDefinition;
                        WebPartInfoProvider.SetWebPartInfo(wpi);
                    }
                    else
                    {
                        ShowError("[FieldEditor.UpdateFormDefinition]: " + GetString("FieldEditor.WebpartNotFound"));
                    }
                    break;

                case FieldEditorModeEnum.ClassFormDefinition:
                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.CustomTable:
                    // Save xml string to CMS_Class table
                    var dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
                    if (dci != null)
                    {
                        dci.ClassFormDefinition = FormDefinition;

                        using (CMSActionContext context = new CMSActionContext { CreateVersion = !IsWizard })
                        {
                            // Save the class data
                            DataClassInfoProvider.SetDataClassInfo(dci);

                            // Update inherited classes with new fields
                            FormHelper.UpdateInheritedClasses(dci);
                        }
                    }
                    else
                    {
                        ShowError("[FieldEditor.UpdateFormDefinition]: " + GetString("FieldEditor.ClassNotFound"));
                    }
                    break;
            }
        }

        // Reload attribute list
        LoadAttributesList(lstAttributes.SelectedValue);
    }


    /// <summary>
    /// When attribute up button is clicked.
    /// </summary>
    protected void btnUpAttribute_Click(Object sender, EventArgs e)
    {
        RaiseBeforeDefinitionUpdate();

        if (FormInfo == null)
        {
            // Form definition was not loaded
            return;
        }

        // First item of the attribute list cannot be moved higher
        if (string.IsNullOrEmpty(lstAttributes.SelectedValue) || (lstAttributes.SelectedIndex == 0))
        {
            return;
        }
        // 'new (not saved)' attribute cannot be moved
        else if ((SelectedItemName == newCategPreffix) || (SelectedItemName == newFieldPreffix))
        {
            ShowMessage(GetString("TemplateDesigner.AlertNewAttributeCannotBeMoved"));
            return;
        }

        if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
        {
            // Move attribute up in attribute list                        
            FormInfo.MoveFormFieldUp(SelectedItemName);
        }
        else if (SelectedItemType == FieldEditorSelectedItemEnum.Category)
        {
            // Move category up in attribute list                        
            FormInfo.MoveFormCategoryUp(SelectedItemName);
        }

        // Update the form definition
        SaveFormDefinition();

        RaiseAfterDefinitionUpdate();

        // Prepare Reset button and information messages
        PrepareResetButton();
        DisplayFieldInformation();
        HandleItemInheritance();
    }


    /// <summary>
    /// When attribute down button is clicked.
    /// </summary>
    protected void btnDownAttribute_Click(Object sender, EventArgs e)
    {
        RaiseBeforeDefinitionUpdate();

        if (FormInfo == null)
        {
            // Form definition was not loaded
            return;
        }

        // Last item of the attribute list cannot be moved lower
        if (string.IsNullOrEmpty(lstAttributes.SelectedValue) || lstAttributes.SelectedIndex >= lstAttributes.Items.Count - 1)
        {
            return;
        }
        // 'new and not saved' attribute cannot be moved
        else if ((SelectedItemName == newCategPreffix) || (SelectedItemName == newFieldPreffix))
        {
            ShowMessage(GetString("TemplateDesigner.AlertNewAttributeCannotBeMoved"));
            return;
        }

        if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
        {
            // Move attribute down in attribute list                        
            FormInfo.MoveFormFieldDown(SelectedItemName);
        }
        else if (SelectedItemType == FieldEditorSelectedItemEnum.Category)
        {
            // Move category down in attribute list                        
            FormInfo.MoveFormCategoryDown(SelectedItemName);
        }

        // Update the form definition
        SaveFormDefinition();

        RaiseAfterDefinitionUpdate();

        // Prepare Reset button and information messages
        PrepareResetButton();
        DisplayFieldInformation();
        HandleItemInheritance();
    }


    /// <summary>
    /// When chkDisplayInForm checkbox checked changed.
    /// </summary>
    protected void chkDisplayInForm_CheckedChanged(Object sender, EventArgs e)
    {
        ShowOrHideFieldDetails();
    }


    /// <summary>
    /// Selected attribute changed event handler.
    /// </summary>
    protected void lstAttributes_SelectedIndexChanged(Object sender, EventArgs e)
    {
        bool isNewCreated = false;

        // Check if new attribute is edited -> select it and avoid selecting another attribute
        foreach (ListItem item in lstAttributes.Items)
        {
            switch (item.Value)
            {
                case newCategPreffix:
                    isNewCreated = true;
                    lstAttributes.SelectedValue = newCategPreffix;
                    break;

                case newFieldPreffix:
                    isNewCreated = true;
                    lstAttributes.SelectedValue = newFieldPreffix;
                    break;
            }

            if (isNewCreated)
            {
                ShowMessage(GetString("TemplateDesigner.AlertSaveNewItemOrDeleteItFirst"));
                if (IsSystemFieldSelected)
                {
                    databaseConfiguration.DisableFieldEditing(true, false);
                }
                else
                {
                    databaseConfiguration.EnableFieldEditing();
                }
                return;
            }
        }

        // Reload data
        Reload(lstAttributes.SelectedValue);
    }


    /// <summary>
    /// Show or hide details according to chkDisplayInForm checkbox is checked or not.
    /// </summary>   
    /// <param name="reload">Indicates if reload is required</param>
    private void ShowOrHideFieldDetails(bool reload = true)
    {
        // Hide or display controls because checkbox 'display in form' was checked
        FieldDetailsVisible = chkDisplayInForm.Checked;

        if (FieldDetailsVisible && reload)
        {
            Reload(lstAttributes.SelectedValue, true);
        }
    }


    /// <summary>
    /// Saves currently edited field.
    /// </summary>
    private void SaveField()
    {
        RaiseBeforeDefinitionUpdate();

        string errorMessage = String.Empty;

        if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
        {
            errorMessage += ValidateForm();
        }

        // Check occurred errors
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
        }
        else
        {
            if (ValidateControlForms())
            {
                // Save selected field
                if (SaveSelectedField())
                {
                    ClearHashtables();

                    RaiseAfterDefinitionUpdate();

                    if (!StopProcessing)
                    {
                        ShowChangesSaved();
                    }
                }
            }
        }
    }


    /// <summary>
    /// Save selected field.
    /// </summary>
    private bool SaveSelectedField()
    {
        DataClassInfo dci = null;
        bool updateInherited = false;

        // Ensure the transaction
        using (var tr = new CMSLateBoundTransaction())
        {
            // FormFieldInfo structure with data from updated form
            FormFieldInfo ffiUpdated = null;

            // FormCategoryInfo structure with data from updated form
            FormCategoryInfo fciUpdated = null;

            if (IsMainForm)
            {
                switch (Mode)
                {
                    // Do nothing for WebPart
                    case FieldEditorModeEnum.ClassFormDefinition:
                    case FieldEditorModeEnum.SystemTable:
                    case FieldEditorModeEnum.CustomTable:
                        {
                            // Fill ClassInfo structure with data from database 
                            dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
                            if (dci != null)
                            {
                                // We are using clone because of potential rollback, so we don't want to change cached instance
                                dci = dci.Clone();
                                tr.BeginTransaction();
                            }
                            else
                            {
                                ShowError(GetString("fieldeditor.notablename"));

                                return false;
                            }
                        }
                        break;

                }
            }

            // Load current XML form definition
            if (FormInfo == null)
            {
                // Form definition was not loaded
                return false;
            }

            string error = null;

            switch (SelectedItemType)
            {
                case FieldEditorSelectedItemEnum.Field:
                    // Fill FormFieldInfo structure with original data
                    ffi = FormInfo.GetFormField(SelectedItemName);

                    // Fill FormFieldInfo structure with updated form data
                    ffiUpdated = FillFormFieldInfoStructure(ffi);

                    try
                    {
                        error = UpdateFormField(ffiUpdated);
                    }
                    catch (Exception ex)
                    {
                        Service.Resolve<IEventLogService>().LogException("FieldEditor", "SAVE", ex);

                        // User friendly message for not null setting of column
                        if (!IsNewItemEdited && ffi.AllowEmpty && !ffiUpdated.AllowEmpty)
                        {
                            ShowError(GetString("FieldEditor.ColumnNotAcceptNull"), ex.Message);
                        }
                        else
                        {
                            ShowError(GetString("general.saveerror"), ex.Message);
                        }
                        return false;
                    }
                    break;

                case FieldEditorSelectedItemEnum.Category:
                    // Fill FormCategoryInfo structure with original data
                    fci = FormInfo.GetFormCategory(SelectedItemName);

                    // Initialize new FormCategoryInfo structure
                    fciUpdated = new FormCategoryInfo
                    {
                        IsDummy = IsDummyField,
                        IsExtra = IsExtraField
                    };

                    error = UpdateFormCategory(fciUpdated);
                    break;
            }

            if (!String.IsNullOrEmpty(error))
            {
                ShowError(error);

                return false;
            }

            // Make changes in database
            if (SelectedItemType != 0)
            {
                // Get updated definition
                FormDefinition = FormInfo.GetXmlDefinition();

                if (IsMainForm)
                {
                    switch (Mode)
                    {
                        case FieldEditorModeEnum.WebPartProperties:
                            error = UpdateWebPartProperties();
                            break;

                        case FieldEditorModeEnum.ClassFormDefinition:
                        case FieldEditorModeEnum.SystemTable:
                        case FieldEditorModeEnum.CustomTable:
                            error = UpdateDependencies(dci, ffiUpdated, out updateInherited);
                            break;

                    }
                }

                if (!String.IsNullOrEmpty(error))
                {
                    ShowError("[FieldEditor.SaveSelectedField()]: " + error);
                    return false;
                }
            }

            // All done and new item, fire OnFieldCreated  event
            RaiseOnFieldCreated(ffiUpdated);

            // Reload field/category
            switch (SelectedItemType)
            {
                case FieldEditorSelectedItemEnum.Category:
                    if (fciUpdated != null)
                    {
                        Reload(categPreffix + fciUpdated.CategoryName);
                    }
                    break;

                case FieldEditorSelectedItemEnum.Field:
                    if (ffiUpdated != null)
                    {
                        Reload(fieldPreffix + ffiUpdated.Name);
                    }
                    break;
            }

            // Commit the transaction
            tr.Commit();
        }

        // Update inherited classes with new fields
        if (updateInherited)
        {
            FormHelper.UpdateInheritedClasses(dci);
        }

        return true;
    }


    /// <summary>
    /// New category button clicked.
    /// </summary>
    protected void btnNewCategory_Click(Object sender, EventArgs e)
    {
        IsDummyField = IsAlternativeForm;
        IsExtraField = IsInheritedForm;

        if (IsNewItemEdited)
        {
            // Display error - Only one new item can be edited
            ShowMessage(GetString("TemplateDesigner.ErrorCannotCreateAnotherNewItem"));
        }
        else
        {
            HandleInherited(false);

            // Create #categ##new# item in list
            ListItem newListItem = new ListItem(GetString("TemplateDesigner.NewCategory"), newCategPreffix);

            if ((lstAttributes.Items.Count > 0) && (lstAttributes.SelectedIndex >= 0))
            {
                // Add behind the selected item 
                lstAttributes.Items.Insert(lstAttributes.SelectedIndex + 1, newListItem);
            }
            else
            {
                // Add at the end of the item collection
                lstAttributes.Items.Add(newListItem);
            }

            // Select new item 
            lstAttributes.SelectedIndex = lstAttributes.Items.IndexOf(newListItem);

            SelectedItemType = FieldEditorSelectedItemEnum.Category;
            SelectedItemName = newCategPreffix;

            LoadDefaultCategoryEditForm();

            IsNewItemEdited = true;
        }

        PrepareResetButton();
    }

    /// <summary>
    /// New attribute button clicked.
    /// </summary>
    protected void btnNewAttribute_Click(Object sender, EventArgs e)
    {
        NewAttribute();
    }


    /// <summary>
    /// Creates new attribute.
    /// </summary>
    private void NewAttribute()
    {
        if (IsNewItemEdited)
        {
            // Only one new item can be edited
            ShowMessage(GetString("TemplateDesigner.ErrorCannotCreateAnotherNewItem"));
            return;
        }

        IsNewItemEdited = true;

        HandleInherited(false);

        FieldDetailsVisible = true;

        // Set field type
        fieldTypeSelector.Reload();
        SetFieldType();

        // Create #new# attribute in attribute list
        ListItem newListItem = new ListItem(GetString("TemplateDesigner.NewAttribute"), newFieldPreffix);

        if ((lstAttributes.Items.Count > 0) && (lstAttributes.SelectedIndex >= 0))
        {
            // Add behind the selected item 
            lstAttributes.Items.Insert(lstAttributes.SelectedIndex + 1, newListItem);
        }
        else
        {
            // Add at the end of the item collection
            lstAttributes.Items.Add(newListItem);
        }

        // Select new item 
        lstAttributes.SelectedIndex = lstAttributes.Items.IndexOf(newListItem);

        // Get type of previously selected item
        FieldEditorSelectedItemEnum oldItemType = SelectedItemType;

        // Initialize currently selected item type and name
        SelectedItemType = FieldEditorSelectedItemEnum.Field;
        SelectedItemName = newFieldPreffix;

        databaseConfiguration.AttributeName = string.Empty;

        bool newItemBlank = ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSClearFieldEditor"], true);
        if (newItemBlank || (oldItemType != FieldEditorSelectedItemEnum.Field))
        {
            LoadDefaultAttributeEditForm(false);
        }

        LoadControlSettings();

        DisplaySelectedTabContent();

        PrepareResetButton();
    }


    private void SetFieldType()
    {
        IsDummyField = false;
        IsDummyFieldFromMainForm = false;
        IsExtraField = false;
        IsPrimaryField = false;
        IsSystemField = false;

        switch (fieldTypeSelector.SelectedFieldType)
        {
            case FieldTypeEnum.Dummy:
                IsDummyField = true;
                IsDummyFieldFromMainForm = !(IsAlternativeForm || IsInheritedForm);
                break;

            case FieldTypeEnum.Document:
                IsSystemField = true;
                break;

            case FieldTypeEnum.Extra:
                IsExtraField = true;
                break;

            case FieldTypeEnum.Primary:
                IsPrimaryField = true;
                break;
        }
    }


    /// <summary>
    /// Gets available controls.
    /// </summary>
    /// <returns>Returns FieldEditorControlsEnum</returns>
    private static FieldEditorControlsEnum GetControls(FieldEditorControlsEnum displayedControls, FieldEditorModeEnum mode, bool developmentMode)
    {
        FieldEditorControlsEnum controls = FieldEditorControlsEnum.None;

        // Get displayed controls
        if (displayedControls == FieldEditorControlsEnum.ModeSelected)
        {
            switch (mode)
            {
                case FieldEditorModeEnum.ClassFormDefinition:
                case FieldEditorModeEnum.AlternativeClassFormDefinition:
                    controls = developmentMode ? FieldEditorControlsEnum.All : FieldEditorControlsEnum.DocumentTypes;
                    break;

                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.AlternativeSystemTable:
                    controls = FieldEditorControlsEnum.SystemTables;
                    break;

                case FieldEditorModeEnum.CustomTable:
                case FieldEditorModeEnum.AlternativeCustomTable:
                    controls = FieldEditorControlsEnum.CustomTables;
                    break;

                case FieldEditorModeEnum.WebPartProperties:
                case FieldEditorModeEnum.Widget:
                case FieldEditorModeEnum.InheritedWebPartProperties:
                case FieldEditorModeEnum.SystemWebPartProperties:
                    controls = FieldEditorControlsEnum.Controls;
                    break;

                case FieldEditorModeEnum.General:
                case FieldEditorModeEnum.FormControls:
                case FieldEditorModeEnum.ProcessActions:
                case FieldEditorModeEnum.InheritedFormControl:
                case FieldEditorModeEnum.PageTemplateProperties:
                case FieldEditorModeEnum.EmailWidget:
                    controls = FieldEditorControlsEnum.All;
                    break;
            }
        }
        else
        {
            controls = displayedControls;
        }

        return controls;
    }


    /// <summary>
    /// Delete attribute button clicked.
    /// </summary>
    protected void btnDeleteItem_Click(Object sender, EventArgs e)
    {
        DataClassInfo dci = null;
        bool updateInherited = false;
        bool clearCacheAfterTransaction = false;

        try
        {
            // Ensure the transaction
            using (var tr = new CMSLateBoundTransaction())
            {
                RaiseBeforeDefinitionUpdate();

                string errorMessage = null;
                string newSelectedValue = null;
                string deletedItemPreffix = null;

                switch (Mode)
                {
                    // Do nothing for WebPart
                    case FieldEditorModeEnum.ClassFormDefinition:
                    case FieldEditorModeEnum.SystemTable:
                    case FieldEditorModeEnum.CustomTable:
                        {
                            // Get the DataClass
                            dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
                            if (dci == null)
                            {
                                ShowError(GetString("FieldEditor.ClassNotFound"));
                                return;
                            }

                            tr.BeginTransaction();
                        }
                        break;
                }

                // Load current XML form definition
                if (FormInfo == null)
                {
                    return;
                }

                if (!String.IsNullOrEmpty(SelectedItemName) && !IsNewItemEdited)
                {
                    switch (SelectedItemType)
                    {
                        case FieldEditorSelectedItemEnum.Field:
                            {
                                FormFieldInfo ffiSelected = FormInfo.GetFormField(SelectedItemName);
                                deletedItemPreffix = fieldPreffix;

                                if (ffiSelected != null)
                                {
                                    // Do not allow deleting of the primary key except for external fields
                                    if (ffiSelected.PrimaryKey && !ffiSelected.External)
                                    {
                                        if (!DevelopmentMode)
                                        {
                                            ShowError(GetString("TemplateDesigner.ErrorCannotDeletePK"));
                                            return;
                                        }

                                        // Check if at least one primary key stays
                                        if (FormInfo.GetFields(true, true, false, true).Count() < 2)
                                        {
                                            ShowError(GetString("TemplateDesigner.ErrorCannotDeletePK"));
                                            return;
                                        }
                                    }

                                    // Check if at least two fields stay in document type definition
                                    if ((Mode == FieldEditorModeEnum.ClassFormDefinition) && (FormInfo.GetFields(true, true, true, false, false).Count() < 3) && !ffiSelected.IsDummyField)
                                    {
                                        ShowError(GetString("TemplateDesigner.ErrorCannotDeleteAllCustomFields"));
                                        return;
                                    }

                                    // Do not allow deleting of the system field
                                    if (ffiSelected.System && !ffiSelected.External && !DevelopmentMode)
                                    {
                                        ShowError(GetString("TemplateDesigner.ErrorCannotDeleteSystemField"));
                                        return;
                                    }

                                    // Remove specific field from xml form definition
                                    FormInfo.RemoveFormField(SelectedItemName);

                                    // Get updated definition
                                    FormDefinition = FormInfo.GetXmlDefinition();

                                    switch (Mode)
                                    {
                                        case FieldEditorModeEnum.WebPartProperties:
                                            errorMessage = UpdateWebPartProperties();
                                            break;

                                        case FieldEditorModeEnum.ClassFormDefinition:
                                        case FieldEditorModeEnum.SystemTable:
                                        case FieldEditorModeEnum.CustomTable:
                                            {
                                                // Update xml definition
                                                if (dci != null)
                                                {
                                                    dci.ClassFormDefinition = FormDefinition;

                                                    // Deleted field is used as ClassNodeNameSource -> remove node name source
                                                    if (dci.ClassNodeNameSource == SelectedItemName)
                                                    {
                                                        dci.ClassNodeNameSource = String.Empty;
                                                    }

                                                    // Update changes in database
                                                    try
                                                    {
                                                        using (CMSActionContext context = new CMSActionContext { CreateVersion = !IsWizard })
                                                        {
                                                            // Save the data class
                                                            DataClassInfoProvider.SetDataClassInfo(dci);

                                                            updateInherited = true;

                                                            // Update alternative forms of form class
                                                            FormHelper.RemoveFieldFromAlternativeForms(dci, SelectedItemName, lstAttributes.SelectedIndex);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Service.Resolve<IEventLogService>().LogException("FieldEditor", "SAVE", ex);
                                                        errorMessage = ex.Message;

                                                        clearCacheAfterTransaction = true;

                                                        break;
                                                    }

                                                    // Refresh queries only if changes to DB were made
                                                    if (!ffiSelected.External)
                                                    {
                                                        QueryInfoProvider.ClearDefaultQueries(dci, true);
                                                    }
                                                }

                                                // Clear hashtables and search settings
                                                ClearHashtables();
                                            }
                                            break;
                                    }
                                }
                            }
                            break;

                        case FieldEditorSelectedItemEnum.Category:
                            deletedItemPreffix = categPreffix;

                            // Remove specific category from xml form definition
                            FormInfo.RemoveFormCategory(SelectedItemName);

                            // Get updated form definition
                            FormDefinition = FormInfo.GetXmlDefinition();

                            switch (Mode)
                            {
                                case FieldEditorModeEnum.WebPartProperties:
                                    errorMessage = UpdateWebPartProperties();
                                    break;

                                case FieldEditorModeEnum.ClassFormDefinition:
                                case FieldEditorModeEnum.SystemTable:
                                case FieldEditorModeEnum.CustomTable:
                                    // Standard classes
                                    {
                                        // Update xml definition
                                        if (dci != null)
                                        {
                                            dci.ClassFormDefinition = FormDefinition;

                                            // Update changes in database
                                            try
                                            {
                                                using (CMSActionContext context = new CMSActionContext { CreateVersion = !IsWizard })
                                                {
                                                    // Save the data class
                                                    DataClassInfoProvider.SetDataClassInfo(dci);

                                                    updateInherited = true;

                                                    FormHelper.RemoveFieldFromAlternativeForms(dci, SelectedItemName, lstAttributes.SelectedIndex);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Service.Resolve<IEventLogService>().LogException("FieldEditor", "SAVE", ex);
                                                errorMessage = ex.Message;
                                            }
                                        }
                                    }
                                    break;

                            }
                            break;

                    }

                    if (!String.IsNullOrEmpty(errorMessage))
                    {
                        ShowError("[ FieldEditor.btnDeleteItem_Click() ]: " + errorMessage);

                        return;
                    }
                }
                else
                {
                    // "delete" new item from the list
                    IsNewItemEdited = false;
                }

                RaiseAfterDefinitionUpdate();

                // Raise on after item delete
                if (AfterItemDeleted != null)
                {
                    AfterItemDeleted(this, new FieldEditorEventArgs(SelectedItemName, SelectedItemType, lstAttributes.SelectedIndex));
                }

                // Commit the transaction
                try
                {
                    tr.Commit();
                }
                catch (Exception ex)
                {
                    Service.Resolve<IEventLogService>().LogException("FieldEditor", "DELETEOBJ", ex);
                    ShowError(ex.Message);

                    return;
                }

                // Set new selected value
                ListItem deletedItem = lstAttributes.Items.FindByValue(deletedItemPreffix + SelectedItemName);
                int deletedItemIndex = lstAttributes.Items.IndexOf(deletedItem);

                if (deletedItemIndex > 0)
                {
                    var item = lstAttributes.Items[deletedItemIndex - 1];
                    if (item != null)
                    {
                        newSelectedValue = item.Value;
                    }
                }

                // Reload data
                Reload(newSelectedValue);
            }
        }
        finally
        {
            if (clearCacheAfterTransaction)
            {
                // Clear hashtables and search settings
                ClearHashtables();
            }
        }

        // Update inherited classes with new fields if necessary
        if (updateInherited)
        {
            FormHelper.UpdateInheritedClasses(dci);
        }
    }


    /// <summary>
    /// Show javascript alert message.
    /// </summary>
    /// <param name="message">Message to show</param>
    private void ShowMessage(string message)
    {
        ltlScript.Text = ScriptHelper.GetScript("alert(" + ScriptHelper.GetString(message) + ");");
    }


    /// <summary>
    /// Called when source field selected index changed.
    /// </summary>
    protected void documentSource_OnSourceFieldChanged(object sender, EventArgs e)
    {
        if (Mode == FieldEditorModeEnum.ClassFormDefinition)
        {
            string errorMessage = null;

            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
            if (dci != null)
            {
                // Set document name source field
                dci.ClassNodeNameSource = string.IsNullOrEmpty(documentSource.SourceFieldValue) ? "" : documentSource.SourceFieldValue;

                // Set document alias source field
                dci.ClassNodeAliasSource = string.IsNullOrEmpty(documentSource.SourceAliasFieldValue) ? "" : documentSource.SourceAliasFieldValue;

                try
                {
                    using (CMSActionContext context = new CMSActionContext { CreateVersion = !IsWizard })
                    {
                        DataClassInfoProvider.SetDataClassInfo(dci);
                    }

                    ShowConfirmation(GetString("TemplateDesigner.SourceFieldSaved"));

                    WebControl control = sender as WebControl;
                    if (control != null && (control.ID == "drpSourceAliasField"))
                    {
                        ShowConfirmation(GetString("TemplateDesigner.SourceAliasFieldSaved"));
                    }
                }
                catch (Exception ex)
                {
                    Service.Resolve<IEventLogService>().LogException("FieldEditor", "SAVE", ex);
                    errorMessage = ex.Message;
                }
            }
            else
            {
                errorMessage = GetString("FieldEditor.ClassNotFound");
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowError("[ FieldEditor.drpSourceField_SelectedIndexChanged() ]: " + errorMessage);
            }
        }
    }


    /// <summary>
    /// Validates form and returns validation error message.
    /// </summary>
    private string ValidateForm()
    {
        const string INVALIDCHARACTERS = @".,;'`:/\*|?""&%$!-+=()[]{} ";

        string attributeName = databaseConfiguration.GetAttributeName();
        string control = fieldAppearance.FieldType;

        // Check if attribute name isn't empty
        if (string.IsNullOrEmpty(attributeName))
        {
            return GetString("TemplateDesigner.ErrorEmptyAttributeName") + " ";
        }

        // Check if attribute name starts with a letter or '_' (if it is an identifier)
        if (!ValidationHelper.IsIdentifier(attributeName))
        {
            return GetString("TemplateDesigner.ErrorAttributeNameDoesNotStartWithLetter") + " ";
        }

        // Check attribute name for invalid characters
        if (attributeName.IndexOfAny(INVALIDCHARACTERS.ToCharArray()) > 0)
        {
            return GetString("TemplateDesigner.ErrorInvalidCharacter") + INVALIDCHARACTERS + ". ";
        }

        if (chkDisplayInForm.Checked)
        {
            // Check if control is selected
            if (String.IsNullOrEmpty(control))
            {
                return GetString("fieldeditor.selectformcontrol");
            }
        }

        databaseConfiguration.Mode = Mode;
        databaseConfiguration.ClassName = ClassName;
        databaseConfiguration.IsDocumentType = IsDocumentType;

        return databaseConfiguration.Validate(ffi);
    }


    /// <summary>
    /// Validates basic forms for generated properties.
    /// </summary>
    /// <returns>TRUE if form is valid. FALSE is form is invalid</returns>
    private bool ValidateControlForms()
    {
        if (chkDisplayInForm.Checked)
        {
            return controlSettings.SaveData();
        }

        return true;
    }


    /// <summary>
    /// Returns FormFieldInfo structure with form data.
    /// </summary>   
    /// <param name="ffiOriginal">Original field info</param>
    private FormFieldInfo FillFormFieldInfoStructure(FormFieldInfo ffiOriginal)
    {
        FormFieldInfo formFieldInfo;

        if (ffiOriginal != null)
        {
            // Field info with original information
            formFieldInfo = (FormFieldInfo)ffiOriginal.Clone();

            if (chkDisplayInForm.Checked)
            {
                // Reset control settings (hidden field's settings are preserved)
                formFieldInfo.Settings.Clear();
                formFieldInfo.SettingsMacroTable.Clear();
            }
        }
        else
        {
            formFieldInfo = new FormFieldInfo();
            formFieldInfo.PrimaryKey = IsPrimaryField;
        }

        // Set data type first to ensure proper further processing
        SetFieldDataType(formFieldInfo);

        formFieldInfo.IsDummyField = IsDummyField;
        formFieldInfo.IsDummyFieldFromMainForm = IsDummyFieldFromMainForm;
        formFieldInfo.IsExtraField = IsExtraField;

        // Load FormFieldInfo with data from database configuration section
        databaseConfiguration.FieldInfo = formFieldInfo;
        databaseConfiguration.Save();

        SetFieldAppearance(formFieldInfo);

        // Determine if it is external column
        formFieldInfo.External |= IsSystemFieldSelected;

        return formFieldInfo;
    }


    /// <summary>
    /// Sets the field appearance
    /// </summary>
    /// <param name="formFieldInfo">Form field info</param>
    private void SetFieldAppearance(FormFieldInfo formFieldInfo)
    {
        DataRow settingsData = null;

        // Do not save additional field settings if field is hidden
        formFieldInfo.Visible = chkDisplayInForm.Checked;

        if (formFieldInfo.Visible)
        {
            // Field appearance section
            fieldAppearance.FieldInfo = formFieldInfo;
            fieldAppearance.Save();

            string selectedType = fieldAppearance.FieldType;

            // Validation section
            validationSettings.FieldInfo = formFieldInfo;
            validationSettings.Save();

            // Design section
            cssSettings.FieldInfo = formFieldInfo;
            cssSettings.Save();

            // HtmlEnvelope section
            htmlEnvelope.FieldInfo = formFieldInfo;
            htmlEnvelope.Save();

            // Field advanced section
            formFieldInfo.SetPropertyValue(FormFieldPropertyEnum.VisibleMacro, fieldAdvancedSettings.VisibleMacro, true);
            formFieldInfo.SetPropertyValue(FormFieldPropertyEnum.EnabledMacro, fieldAdvancedSettings.EnabledMacro, true);
            formFieldInfo.DisplayInSimpleMode = fieldAdvancedSettings.DisplayInSimpleMode;
            formFieldInfo.HasDependingFields = fieldAdvancedSettings.HasDependingFields;
            formFieldInfo.DependsOnAnotherField = fieldAdvancedSettings.DependsOnAnotherField;

            // Get control settings data
            settingsData = controlSettings.FormData;

            // Store macro table
            formFieldInfo.SettingsMacroTable = controlSettings.MacroTable;

            formFieldInfo.SetControlName(selectedType);
        }

        formFieldInfo.DisplayIn = String.Empty;

        // Store settings
        if ((settingsData != null) && (settingsData.ItemArray.Length > 0))
        {
            foreach (DataColumn column in settingsData.Table.Columns)
            {
                formFieldInfo.Settings[column.ColumnName] = settingsData.Table.Rows[0][column.Caption];
            }
        }
    }


    /// <summary>
    /// Sets the field data type based on the configuration
    /// </summary>
    /// <param name="formFieldInfo">Form field info</param>
    private void SetFieldDataType(FormFieldInfo formFieldInfo)
    {
        var mode = Mode;

        if ((databaseConfiguration.AttributeType == FieldDataType.File) && ((mode == FieldEditorModeEnum.SystemTable) || (mode == FieldEditorModeEnum.AlternativeSystemTable)))
        {
            // Allow to save <guid>.<extension>
            formFieldInfo.DataType = FieldDataType.Text;
            formFieldInfo.Size = 500;
        }
        else
        {
            formFieldInfo.DataType = databaseConfiguration.AttributeType;
            formFieldInfo.Size = ValidationHelper.GetInteger(databaseConfiguration.AttributeSize, 0);
            formFieldInfo.Precision = ValidationHelper.GetInteger(databaseConfiguration.AttributePrecision, -1);
        }
    }


    /// <summary>
    /// Displays selected tab content.
    /// </summary>
    protected void DisplaySelectedTabContent()
    {
        plcField.Visible = true;
        plcCategory.Visible = false;
    }


    /// <summary>
    /// Hides all editing panels.
    /// </summary>
    protected void HideAllPanels()
    {
        plcCategory.Visible = false;
        plcField.Visible = false;
    }


    /// <summary>
    /// Adds new form item (field or category) to the form definition.
    /// </summary>
    /// <param name="formItem">Form item to add</param>
    protected void InsertFormItem(IDataDefinitionItem formItem)
    {
        // Set new item prefix
        string newItemPreffix = (formItem is FormFieldInfo) ? newFieldPreffix : newCategPreffix;

        ListItem newItem = lstAttributes.Items.FindByValue(newItemPreffix);
        int newItemIndex = lstAttributes.Items.IndexOf(newItem);

        if ((newItemIndex > 0) && (lstAttributes.Items[newItemIndex - 1] != null))
        {
            // Add new item at the specified position
            FormInfo.AddFormItem(formItem, newItemIndex);
        }
        else
        {
            if (formItem is FormFieldInfo)
            {
                // Add new field at the end of the collection
                FormInfo.AddFormItem(formItem);
            }
            else
            {
                var catObj = formItem as FormCategoryInfo;
                if (catObj != null)
                {
                    // Add new category at the end of the collection
                    FormInfo.AddFormCategory(catObj);
                }
            }
        }
    }


    /// <summary>
    /// Returns FormInfo for given form user control.
    /// </summary>
    /// <param name="controlName">Code name of form control</param>
    /// <returns>Form info</returns>
    private FormInfo GetUserControlSettings(string controlName)
    {
        FormUserControlInfo control = FormUserControlInfoProvider.GetFormUserControlInfo(controlName);
        if (control != null)
        {
            // Get complete form info for current control
            FormInfo formInfo = FormHelper.GetFormControlParameters(controlName, control.UserControlMergedParameters, true);

            return formInfo;
        }
        return null;
    }


    /// <summary>
    /// Clears hashtables.
    /// </summary>
    private void ClearHashtables()
    {
        // Clear the classes hashtable
        AbstractProviderDictionary.ReloadDictionaries("cms.class", true);

        if (!String.IsNullOrEmpty(ClassName))
        {
            // Clear the object type hashtable
            AbstractProviderDictionary.ReloadDictionaries(ClassName, true);

            // Clear class structures
            ClassStructureInfo.Remove(ClassName, true);
        }

        // Clear form resolver
        FormEngineWebUIResolvers.ClearResolvers(true);

        // Invalidate objects based on object type
        ObjectTypeInfo ti = ObjectTypeManager.GetTypeInfo(ClassName);
        if ((ti != null) && (ti.ProviderType != null))
        {
            ti.InvalidateColumnNames();
            ti.InvalidateAllObjects();

            // If edited class is user settings, clear the user structure info which contains the user settings columns
            if ((Mode == FieldEditorModeEnum.SystemTable) && ClassName.Equals(UserSettingsInfo.OBJECT_TYPE, StringComparison.InvariantCultureIgnoreCase))
            {
                ObjectTypeInfo userTypeInfo = UserInfo.TYPEINFO;
                userTypeInfo.InvalidateColumnNames();
                userTypeInfo.InvalidateAllObjects();
            }
        }
    }


    private string UpdateFormCategory(FormCategoryInfo updatedCategoryInfo)
    {
        categoryEdit.CategoryInfo = updatedCategoryInfo;
        categoryEdit.Save();

        bool captionIsMacro;
        var caption = updatedCategoryInfo.GetPropertyValue(FormCategoryPropertyEnum.Caption, out captionIsMacro);

        if (String.IsNullOrEmpty(caption))
        {
            return GetString("TemplateDesigner.ErrorCategoryNameEmpty");
        }

        if (IsNewItemEdited)
        {
            if (captionIsMacro && !MacroProcessor.IsLocalizationMacro(caption))
            {
                updatedCategoryInfo.CategoryName = GenerateCategoryName();
            }
            else
            {
                updatedCategoryInfo.CategoryName = ValidationHelper.GetCodeName(caption);
            }
        }
        else
        {
            updatedCategoryInfo.CategoryName = fci.CategoryName;
        }

        if ((IsNewItemEdited || updatedCategoryInfo.CategoryName != fci.CategoryName) && FormInfo.GetCategoryNames().Exists(x => x == updatedCategoryInfo.CategoryName))
        {
            return GetString("TemplateDesigner.ErrorExistingCategoryName");
        }

        if (IsNewItemEdited)
        {
            // Insert new category
            InsertFormItem(updatedCategoryInfo);
        }
        else
        {
            // Update current 
            FormInfo.UpdateFormCategory(fci.CategoryName, updatedCategoryInfo);
        }

        // No error occurred
        return null;
    }


    private string UpdateFormField(FormFieldInfo updatedFieldInfo)
    {
        // Validate whether column with this name already exists
        string errorMessage = ValidateFieldColumn(updatedFieldInfo);
        if (!String.IsNullOrEmpty(errorMessage))
        {
            return errorMessage;
        }

        errorMessage = ValidatePrimaryKey(updatedFieldInfo);
        if (!String.IsNullOrEmpty(errorMessage))
        {
            return errorMessage;
        }

        if (IsNewItemEdited)
        {
            // Insert new field
            InsertFormItem(updatedFieldInfo);

            // Hide new field in alternative and inherited forms if necessary
            if (IsMainForm)
            {
                DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
                FormHelper.HideFieldInAlternativeForms(updatedFieldInfo, dci);
            }
        }
        else
        {
            // Update the DB column
            if (IsHandledFieldChange(ffi, updatedFieldInfo))
            {
                RaiseOnFieldNameChanged(ffi.Name, updatedFieldInfo.Name);
            }


            // Update current field
            FormInfo.UpdateFormField(ffi.Name, updatedFieldInfo);
        }

        return null;
    }


    private static bool IsHandledFieldChange(FormFieldInfo originalField, FormFieldInfo updatedField)
    {
        if (originalField.PrimaryKey)
        {
            return originalField.Name != updatedField.Name;
        }

        return IsDatabaseChangeRequired(originalField, updatedField) || (updatedField.DataType == FieldDataType.Double);
    }


    private string ValidateFieldColumn(FormFieldInfo fieldInfo)
    {
        // Check if the attribute name already exists
        if (IsNewItemEdited || (!ffi.Name.Equals(fieldInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
        {
            var columnNames = FormInfo.GetColumnNames();

            // If name already exists
            if ((columnNames != null) && (columnNames.Any(c => fieldInfo.Name.Equals(c, StringComparison.InvariantCultureIgnoreCase))))
            {
                return GetString("TemplateDesigner.ErrorExistingColumnName");
            }

            // Check column name duplicity in JOINed tables
            if (IsMainForm && !IsSystemFieldSelected)
            {
                // Check whether current column already exists in main document view
                if (IsDocumentType && DocumentHelper.ColumnExistsInDocumentView(fieldInfo.Name))
                {
                    return GetString("TemplateDesigner.ErrorExistingColumnInJoinedTable");
                }
            }
        }

        return null;
    }


    private string UpdateDependencies(DataClassInfo dci, FormFieldInfo updatedFieldInfo, out bool updateInheritedForms)
    {
        updateInheritedForms = false;
        string error = null;

        if (dci != null)
        {
            // Update XML definition
            dci.ClassFormDefinition = FormDefinition;

            // When updating existing field
            if ((ffi != null) && (dci.ClassNodeNameSource == ffi.Name))
            {
                dci.ClassNodeNameSource = updatedFieldInfo.IsNodeNameSourceCandidate() ? updatedFieldInfo.Name : String.Empty;
            }

            // Update changes in DB
            try
            {
                // Do not create a new version if field editor is in the wizard
                using (new CMSActionContext { CreateVersion = !IsWizard })
                {
                    // Save the data class
                    DataClassInfoProvider.SetDataClassInfo(dci);
                }

                updateInheritedForms = true;
            }
            catch (Exception ex)
            {
                Service.Resolve<IEventLogService>().LogException("FieldEditor", "SAVE", ex);

                return ex.Message;
            }

            if ((SelectedItemType == FieldEditorSelectedItemEnum.Field) && !updatedFieldInfo.IsDummyField)
            {
                QueryInfoProvider.ClearDefaultQueries(dci, true);
            }
        }
        else
        {
            error = GetString("FieldEditor.ClassNotFound");
        }

        return error;
    }


    private string UpdateWebPartProperties()
    {
        string error = null;
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(WebPartId);
        if (wpi != null)
        {
            // Update XML definition
            wpi.WebPartProperties = FormDefinition;

            try
            {
                WebPartInfoProvider.SetWebPartInfo(wpi);
            }
            catch (Exception ex)
            {
                Service.Resolve<IEventLogService>().LogException("FieldEditor", "SAVE", ex);
                error = ex.Message;
            }
        }
        else
        {
            error = GetString("FieldEditor.WebpartNotFound");
        }

        return error;
    }


    private string ValidatePrimaryKey(FormFieldInfo updatedFieldInfo)
    {
        string errorMessage = null;

        bool isPrimaryKey = ffi?.PrimaryKey ?? updatedFieldInfo.PrimaryKey;

        // If attribute is a primary key
        if (isPrimaryKey)
        {
            // Check if the attribute type is integer number
            if (updatedFieldInfo.DataType != FieldDataType.Integer)
            {
                errorMessage += ResHelper.GetString("TemplateDesigner.ErrorPKNotInteger") + " ";
            }

            // Check if allow empty is disabled
            if (updatedFieldInfo.AllowEmpty)
            {
                errorMessage += ResHelper.GetString("TemplateDesigner.ErrorPKAllowsNulls") + " ";
            }

            // Check that the field type is label or nothing (does not apply for alt.forms)
            if (!IsAlternativeForm && !IsCorrectPrimaryKeyControlSelected(updatedFieldInfo))
            {
                errorMessage += ResHelper.GetString("TemplateDesigner.ErrorPKisNotLabel") + " ";
            }

            // An error has occurred
            if (!String.IsNullOrEmpty(errorMessage))
            {
                errorMessage = ResHelper.GetString("TemplateDesigner.ErrorPKThisIsPK") + " " + errorMessage;
            }
        }

        return errorMessage;
    }


    /// <summary>
    /// Returns whether <paramref name="fieldInfo"/> has it's control set to <see cref="FormFieldControlName.LABEL"></see> or nothing.
    /// </summary>
    private static bool IsCorrectPrimaryKeyControlSelected(FormFieldInfo fieldInfo)
    {
        return fieldInfo.HasFormFieldControlWithName(FormFieldControlName.LABEL) || fieldInfo.HasFormFieldControlWithName(String.Empty);
    }


    /// <summary>
    /// Raises OnFieldCreated event if the field has been just created.
    /// </summary>
    /// <param name="newField">Newly created field</param>
    protected void RaiseOnFieldCreated(FormFieldInfo newField)
    {
        if (IsNewItemEdited && (newField != null) && (OnFieldCreated != null))
        {
            OnFieldCreated(this, newField);
        }

        IsNewItemEdited = false;
    }

    #endregion


    #region "Support for system fields"

    /// <summary>
    /// Group changed event handler.
    /// </summary>
    private void databaseConfiguration_DropChanged(object sender, EventArgs e)
    {
        LoadSystemField();
    }


    /// <summary>
    /// Database attribute has changed.
    /// </summary>
    private void databaseConfiguration_AttributeChanged(object sender, EventArgs e)
    {
        databaseConfiguration.Mode = Mode;
        databaseConfiguration.IsDocumentType = IsDocumentType;
        databaseConfiguration.DevelopmentMode = DevelopmentMode;
        databaseConfiguration.IsAlternativeForm = IsAlternativeForm;
        databaseConfiguration.IsInheritedForm = IsInheritedForm;
        databaseConfiguration.IsDummyField = IsDummyField;
        databaseConfiguration.IsDummyFieldFromMainForm = IsDummyFieldFromMainForm;
        databaseConfiguration.IsExtraField = IsExtraField;
        databaseConfiguration.IsFieldPrimary = IsPrimaryField;
        databaseConfiguration.EnableOrDisableAttributeSize();

        fieldAppearance.Mode = Mode;
        fieldAppearance.ClassName = ClassName;
        fieldAppearance.AttributeType = databaseConfiguration.AttributeType;
        fieldAppearance.FieldType = FormHelper.GetFormFieldDefaultControlName(databaseConfiguration.AttributeType, GetControls(DisplayedControls, Mode, DevelopmentMode));
        fieldAppearance.LoadFieldTypes(IsPrimaryField);

        ShowFieldOptions();
        LoadValidationSettings();
        databaseConfiguration.ShowDefaultControl();
        LoadControlSettings();
    }


    /// <summary>
    /// Field control changed event handler.
    /// </summary>
    private void control_FieldSelected(object sender, EventArgs e)
    {
        LoadControlSettings();
        LoadValidationSettings();
    }


    /// <summary>
    /// FieldTypeSelector field type selection event handler.
    /// </summary>
    private void fieldTypeSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SetFieldType();
        LoadDefaultAttributeEditForm(false);
        LoadControlSettings();
    }


    /// <summary>
    /// Loads control with new FormInfo data.
    /// </summary>
    /// <param name="selectedFieldType">Selected field</param>
    private void LoadControlSettings(string selectedFieldType = null)
    {
        if (String.IsNullOrEmpty(selectedFieldType))
        {
            selectedFieldType = fieldAppearance.FieldType;
        }

        // Get properties definition for selected form control
        controlSettings.FormInfo = GetUserControlSettings(selectedFieldType);

        // Reset settings
        controlSettings.Settings = new Hashtable();
        controlSettings.BasicForm.Mode = FormModeEnum.Insert;

        if (ffi != null)
        {
            // Provide current field's settings when updating existing field
            controlSettings.Settings = ffi.Settings;
            controlSettings.MacroTable = ffi.SettingsMacroTable;

            var form = controlSettings.BasicForm;

            // Provide data type of the edited field to the underlying settings (primarily due to dropdown list options)
            form.ContextResolver.SetNamedSourceData("FieldEditorDataType", databaseConfiguration.AttributeType);

            // Check if field's editing control changed
            // If yes, the settings form is set to insert mode and it loads default values of the new control visible properties
            var selectionChanged = !ffi.HasFormFieldControlWithName(selectedFieldType);
            if (!selectionChanged)
            {
                form.Mode = FormModeEnum.Update;
            }
        }

        controlSettings.Reload(true);
    }


    /// <summary>
    /// Loads system field either from database column data or from field XML definition.
    /// </summary>
    private void LoadSystemField()
    {
        string tableName = databaseConfiguration.GroupValue;
        string columnName = databaseConfiguration.SystemValue;

        if (!SelectedItemName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
        {
            // Get field info from database column
            var dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
            ffi = FormHelper.GetFormFieldInfo(dci, tableName, columnName);
        }
        else
        {
            // Get field info from XML definition
            if (FormInfo == null)
            {
                // Form definition was not loaded
                return;
            }

            ffi = FormInfo.GetFormField(SelectedItemName);
        }

        LoadSelectedField(false);
    }


    /// <summary>
    /// Resets FormInformation to ensure its reload.
    /// </summary>
    private void FormDefinitionChanged()
    {
        FormInfo = null;
    }

    #endregion
}