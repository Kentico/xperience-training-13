using System;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_Layout : CMSUserControl
{
    #region "Private variables"

    private int mOldObjectId;
    private int mDataClassId;
    private string mFormControlScript;
    private AlternativeFormInfo altFormInfo;
    private DataClassInfo classInfo;
    private HeaderAction layoutAction;
    private ObjectEditMenu mObjectMenu;
    private bool? mEditedObjectSupportsLocking;

    #endregion


    #region "Public properties"

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
    /// Indicates if control is used on live site
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
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets object id (document type id, alternative form id).
    /// </summary>
    public int ObjectID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ObjectID"], 0);
        }
        set
        {
            ViewState["ObjectID"] = value;
        }
    }


    /// <summary>
    /// Gets or sets layout.
    /// </summary>
    public string FormLayout
    {
        get
        {
            if (htmlEditor.Visible)
            {
                return (htmlEditor.Value != null) ? htmlEditor.ResolvedValue : null;
            }
            else
            {
                return txtCode.Text;
            }
        }
        set
        {
            htmlEditor.ResolvedValue = txtCode.Text = value;
        }
    }


    /// <summary>
    /// Returns form layout type.
    /// </summary>
    public LayoutTypeEnum LayoutType
    {
        get
        {
            LayoutTypeEnum result = LayoutTypeEnum.Html;
            if (drpLayoutType.Items.Count > 0)
            {
                result = LayoutHelper.GetLayoutTypeEnum(drpLayoutType.SelectedValue);
            }

            return result;
        }
    }


    /// <summary>
    /// Determines whether layout was set.
    /// </summary>
    public bool LayoutIsSet
    {
        get
        {
            return !string.IsNullOrEmpty(FormLayout);
        }
    }


    /// <summary>
    /// Gets or sets state of custom layout checkbox.
    /// </summary>
    public bool CustomLayoutEnabled
    {
        get
        {
            return radCustomLayout.Checked;
        }
        set
        {
            if (value)
            {
                radCustomLayout.Checked = true;
            }
            else
            {
                radAutoLayout.Checked = true;
            }
        }
    }


    /// <summary>
    /// Gets or sets type of form layout.
    /// </summary>
    public FormLayoutTypeEnum FormLayoutType
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets alternative form behavior.
    /// </summary>
    public bool IsAlternative
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return pnlContent.Enabled;
        }
        set
        {
            pnlContent.Enabled = value;
            btnSave.Enabled = value;
            htmlEditor.Enabled = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets appropriate id (document, alternative form)according to layout.
    /// </summary>
    private int DataClassID
    {
        get
        {
            if (mOldObjectId != ObjectID)
            {
                mOldObjectId = ObjectID;

                if (!IsAlternative)
                {
                    switch (FormLayoutType)
                    {
                        case FormLayoutTypeEnum.Document:
                        case FormLayoutTypeEnum.SystemTable:
                        case FormLayoutTypeEnum.CustomTable:
                            mDataClassId = ObjectID;
                            break;
                    }
                }
                else
                {
                    AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(ObjectID);
                    if (afi != null)
                    {
                        mDataClassId = afi.FormClassID;
                    }
                }
            }
            return mDataClassId;
        }
    }


    /// <summary>
    /// Script with fields' form controls that is used in ASCX layout.
    /// </summary>
    private string FormControlScript
    {
        get
        {
            if (radCustomLayout.Checked && drpLayoutType.SelectedValue.EqualsCSafe(LayoutTypeEnum.Ascx.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return mFormControlScript;
            }
            return null;
        }
        set
        {
            mFormControlScript = value;
        }
    }


    /// <summary>
    /// Object edit menu control.
    /// </summary>
    public ObjectEditMenu ObjectEditMenu
    {
        get
        {
            if (mObjectMenu == null)
            {
                mObjectMenu = ControlsHelper.GetChildControl<ObjectEditMenu>(Page);
            }
            return mObjectMenu;
        }
    }


    /// <summary>
    /// Returns if edited object supports locking.
    /// </summary>
    private bool EditedObjectSupportsLocking
    {
        get
        {
            if (mEditedObjectSupportsLocking == null)
            {
                GeneralizedInfo info = null;
                if (UIContext.EditedObject is BaseInfo)
                {
                    info = ((BaseInfo)UIContext.EditedObject).Generalized;
                }
                else if (UIContext.EditedObject is GeneralizedInfo)
                {
                    info = (GeneralizedInfo)UIContext.EditedObject;
                }
                if (info != null)
                {
                    mEditedObjectSupportsLocking = info.TypeInfo.SupportsLocking;
                }
            }

            return (bool)mEditedObjectSupportsLocking;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();

        InitHeaderActions();

        // Register save button handler
        if (EditedObjectSupportsLocking && (pnlObjectLocking.ObjectManager != null))
        {
            pnlObjectLocking.ObjectManager.OnSaveData += ObjectManager_OnSaveData;
            pnlObjectLocking.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;
        }
        else
        {
            ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, lnkSave_Click);
        }

        UIHelper.SetBreadcrumbsSuffix(GetString("objecttype.cms_form"));
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Register JS scripts
        RegisterScripts();

        // Show/hide layout dialog
        pnlCustomLayout.Visible = radCustomLayout.Checked;
        pnlLayoutType.Visible = radCustomLayout.Checked;

        base.OnPreRender(e);
    }


    protected void radLayout_CheckedChanged(object sender, EventArgs e)
    {
        // Show/hide layout dialog
        if (radCustomLayout.Checked)
        {
            LoadData();
        }
    }


    protected void drpLayoutType_SelectedIndexChanged(object sender, EventArgs e)
    {
        // If user switch layout type, set form layout empty
        FormLayout = String.Empty;

        // Switch visibility of layout editors based on layout type
        plcHTML.Visible = drpLayoutType.SelectedValue.Equals(LayoutTypeEnum.Html.ToString(), StringComparison.InvariantCultureIgnoreCase);
        plcASCX.Visible = !plcHTML.Visible;

        LoadData();
    }


    protected void ObjectEditMenu_OnGetClientValidationScript(object sender, EditMenuEventArgs e)
    {
        // Set validation script for save and checkin actions
        if (e.ActionName.Equals(ComponentEvents.SAVE, StringComparison.InvariantCultureIgnoreCase) ||
            e.ActionName.Equals(ComponentEvents.CHECKIN, StringComparison.InvariantCultureIgnoreCase))
        {
            // Set save validation script
            e.ValidationScript = GetValidationScript();
        }
    }


    /// <summary>
    /// Handle Save's OnClick event if edited object supports locking.
    /// </summary>
    protected void ObjectManager_OnSaveData(object sender, SimpleObjectManagerEventArgs e)
    {
        if (!SaveData())
        {
            e.IsValid = false;
        }
    }


    /// <summary>
    /// Reloads data after save.
    /// </summary>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName != ComponentEvents.SAVE)
        {
            LoadData();
        }

        if (layoutAction != null)
        {
            layoutAction.Visible = !IsEditedObjectLocked();
        }
    }


    /// <summary>
    /// Handle Save's OnClick event if edited object does not support locking.
    /// </summary>
    protected void lnkSave_Click(object sender, EventArgs e)
    {
        SaveData();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    private void ReloadData()
    {
        LoadData();

        if (radCustomLayout.Checked)
        {
            FillFieldsList();
            FillFieldTypeList();
        }
    }


    private void InitHeaderActions()
    {
        // Add extra action to object menu
        layoutAction = new HeaderAction
        {
            Text = GetString("documenttype_edit_form.generatedefaultlayout"),
            Enabled = Enabled && radCustomLayout.Checked,
            OnClientClick = (LayoutTypeEnum.Html.ToString() == drpLayoutType.SelectedValue) ? "SetContent(GenerateHtmlLayout()); return false;" : "SetContent(GenerateAscxLayout()); return false;",
            Visible = !IsEditedObjectLocked(),
            ButtonStyle = ButtonStyle.Default,
        };
        AddExtraHeaderAction(layoutAction);

        if (EditedObjectSupportsLocking && (ObjectEditMenu != null))
        {
            // Register for get client validation script event
            ObjectEditMenu.OnGetClientValidationScript += ObjectEditMenu_OnGetClientValidationScript;
        }
        else
        {
            // Get save validation script
            string script = GetValidationScript();
            if (!string.IsNullOrEmpty(script))
            {
                btnSave.OnClientClick = string.Format("if (!{0}) {{ return false; }}", script);
            }
        }
    }


    /// <summary>
    /// Adds extra action to the object edit menu.
    /// </summary>
    /// <param name="action">Header action</param>
    private void AddExtraHeaderAction(HeaderAction action)
    {
        if ((action != null) && (ObjectEditMenu != null))
        {
            // Add extra action to object menu
            ObjectEditMenu.AddExtraAction(action);
        }
    }


    /// <summary>
    /// Returns if object is locked for editing by current user.
    /// </summary>
    private bool IsEditedObjectLocked()
    {
        return pnlObjectLocking.IsObjectLocked();
    }


    /// <summary>
    /// Loads form layout of document, systemtable or alternative form.
    /// </summary>
    private void LoadData()
    {
        if (DataClassID > 0)
        {
            if (!IsAlternative)
            {
                classInfo = DataClassInfoProvider.GetDataClassInfo(DataClassID);
                UIContext.EditedObject = classInfo;
            }
            else
            {
                altFormInfo = AlternativeFormInfoProvider.GetAlternativeFormInfo(ObjectID);
                UIContext.EditedObject = altFormInfo;
            }

            if (!RequestHelper.IsPostBack())
            {
                LayoutTypeEnum layoutType = LayoutTypeEnum.Html;

                InitTypeSelector();

                if (!IsAlternative)
                {
                    if (classInfo != null)
                    {
                        // Load layout of document or systemtable
                        FormLayout = classInfo.ClassFormLayout;
                        layoutType = classInfo.ClassFormLayoutType;
                    }
                }
                else
                {
                    if (altFormInfo != null)
                    {
                        // Load layout of alternative form
                        FormLayout = altFormInfo.FormLayout;
                        layoutType = altFormInfo.FormLayoutType;
                    }
                }

                radCustomLayout.Checked = LayoutIsSet;
                drpLayoutType.SelectedValue = layoutType.ToString();

                // Switch visibility of layout editors based on layout type
                plcHTML.Visible = drpLayoutType.SelectedValue.EqualsCSafe(LayoutTypeEnum.Html.ToString(), StringComparison.InvariantCultureIgnoreCase);
                plcASCX.Visible = !plcHTML.Visible;
            }

            if (radCustomLayout.Checked)
            {
                InitDialog();
            }

            radAutoLayout.Checked = !radCustomLayout.Checked;
        }
        else
        {
            UIContext.EditedObject = null;
        }
    }


    /// <summary>
    /// Saves form layout.
    /// </summary>
    private bool SaveData()
    {
        bool saved = false;
        bool deleted = false;

        // Get form layout
        string layout = FormLayout;

        // Delete layout if editor is hidden
        if (!CustomLayoutEnabled)
        {
            deleted = LayoutIsSet;
            layout = null;
        }

        if (DataClassID > 0)
        {
            if (!IsAlternative)
            {
                classInfo = DataClassInfoProvider.GetDataClassInfo(DataClassID);
                UIContext.EditedObject = classInfo;
                if (classInfo != null)
                {
                    // Update dataclass form layout and save object
                    classInfo.ClassFormLayout = layout;
                    classInfo.ClassFormLayoutType = LayoutHelper.GetLayoutTypeEnum(drpLayoutType.SelectedValue);
                    DataClassInfoProvider.SetDataClassInfo(classInfo);
                    saved = true;
                }
            }
            else
            {
                altFormInfo = AlternativeFormInfoProvider.GetAlternativeFormInfo(ObjectID);
                UIContext.EditedObject = altFormInfo;
                if (altFormInfo != null)
                {
                    // Update alternative form layout and save object
                    altFormInfo.FormLayout = layout;
                    altFormInfo.FormLayoutType = LayoutHelper.GetLayoutTypeEnum(drpLayoutType.SelectedValue);
                    AlternativeFormInfoProvider.SetAlternativeFormInfo(altFormInfo);
                    saved = true;
                }
            }

            // Display info if successfully saved
            if (saved)
            {
                if (!deleted)
                {
                    ShowChangesSaved();
                }
                else
                {
                    ShowConfirmation(GetString("DocumentType_Edit_Form.LayoutDeleted"));
                }
            }
        }
        else
        {
            UIContext.EditedObject = null;
        }

        return true;
    }


    /// <summary>
    /// Initializes layout type selector.
    /// </summary>
    private void InitTypeSelector()
    {
        if (drpLayoutType.Items.Count == 0)
        {
            drpLayoutType.Items.Add(new ListItem(GetString("TransformationType.Ascx"), LayoutTypeEnum.Ascx.ToString()));
            drpLayoutType.Items.Add(new ListItem(GetString("TransformationType.Html"), LayoutTypeEnum.Html.ToString()));
        }
    }


    /// <summary>
    /// Initializes dialog based on layout type information.
    /// </summary>
    private void InitDialog()
    {
        switch (drpLayoutType.SelectedValue.ToLowerCSafe())
        {
            case "html":
                InitHTMLeditor();
                break;

            case "ascx":
                InitASCXeditor();
                break;
        }
    }


    /// <summary>
    /// Initializes HTML editor's settings.
    /// </summary>
    private void InitHTMLeditor()
    {
        htmlEditor.AutoDetectLanguage = false;
        htmlEditor.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlEditor.ToolbarSet = "BizForm";
    }


    /// <summary>
    /// Initializes ASCX editor.
    /// </summary>
    private void InitASCXeditor()
    {
        txtCode.Editor.Language = LanguageEnum.ASPNET;
        txtCode.Editor.Height = new Unit("300px");

        string lang = DataHelper.GetNotEmpty(Service.Resolve<IAppSettingsService>()["CMSProgrammingLanguage"], "C#");
        lblDirectives.Text = string.Concat("&lt;%@ Control Language=\"", lang, "\" AutoEventWireup=\"true\" Inherits=\"CMS.FormEngine.Web.UI.CMSAbstractFormLayout\" %&gt;<br />&lt;%@ Register TagPrefix=\"cms\" Namespace=\"CMS.FormEngine.Web.UI\" Assembly=\"CMS.FormEngine.Web.UI\" %&gt;");
    }


    /// <summary>
    /// Fills field list with available fields.
    /// </summary>
    private void FillFieldsList()
    {
        drpAvailableFields.Items.Clear();

        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(DataClassID);
        if (dci != null)
        {
            // Load form definition
            string formDefinition = dci.ClassFormDefinition;
            if (IsAlternative)
            {
                // Get alternative form definition and merge if with the original one
                AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(ObjectID);

                if (afi.FormCoupledClassID > 0)
                {
                    // If coupled class is defined combine form definitions
                    DataClassInfo coupledDci = DataClassInfoProvider.GetDataClassInfo(afi.FormCoupledClassID);
                    if (coupledDci != null)
                    {
                        formDefinition = FormHelper.MergeFormDefinitions(formDefinition, coupledDci.ClassFormDefinition);
                    }
                }

                // Merge class and alternative form definitions
                formDefinition = FormHelper.MergeFormDefinitions(formDefinition, afi.FormDefinition);
            }
            FormInfo fi = new FormInfo(formDefinition);
            // Get visible fields
            var visibleFields = fi.GetFields(true, false);

            drpAvailableFields.Items.Clear();

            // Prepare arrays for JavaScript functions
            string script = "var formControlName = new Array(); var formControlCssClass = new Array();\n";

            if (FormLayoutType == FormLayoutTypeEnum.Document)
            {
                if (string.IsNullOrEmpty(dci.ClassNodeNameSource)) //if node name source is not set
                {
                    string docName = "DocumentName";
                    drpAvailableFields.Items.Add(new ListItem(GetString("DocumentType_Edit_Form.DocumentName"), docName));

                    // Add document name field form control to the script
                    script += String.Format("formControlName['{0}'] = '{1}';\n", docName, "TextBoxControl");
                    script += String.Format("formControlCssClass['{0}'] = '';\n", docName);
                }
            }

            if (visibleFields != null)
            {
                // Add public visible fields to the list
                foreach (FormFieldInfo ffi in visibleFields)
                {
                    drpAvailableFields.Items.Add(new ListItem(ffi.GetDisplayName(MacroResolver.GetInstance()), ffi.Name));

                    // Add fields' form controls to the script
                    script += String.Format("formControlName['{0}'] = '{1}';\n", ffi.Name, ffi.GetControlName());
                    script += String.Format("formControlCssClass['{0}'] = '{1}';\n", ffi.Name, ffi.GetPropertyValue(FormFieldPropertyEnum.ControlCssClass));
                }
            }

            if (FormLayoutType == FormLayoutTypeEnum.Document)
            {
                if (dci.ClassUsePublishFromTo)
                {
                    // Add publish from/to fields if they are not already in predefined fields
                    if (drpAvailableFields.Items.FindByValue("DocumentPublishFrom", true) == null)
                    {
                        string publishFrom = "DocumentPublishFrom";
                        drpAvailableFields.Items.Add(new ListItem(GetString("DocumentType_Edit_Form.DocumentPublishFrom"), publishFrom));

                        // Add Publish From field form control to the script
                        script += String.Format("formControlName['{0}'] = '{1}';\n", publishFrom, "CalendarControl");
                        script += String.Format("formControlCssClass['{0}'] = '';\n", publishFrom);
                    }
                    if (drpAvailableFields.Items.FindByValue("DocumentPublishTo", true) == null)
                    {
                        string publishTo = "DocumentPublishTo";
                        drpAvailableFields.Items.Add(new ListItem(GetString("DocumentType_Edit_Form.DocumentPublishTo"), publishTo));

                        // Add Publish To field form control to the script
                        script += String.Format("formControlName['{0}'] = '{1}';\n", publishTo, "CalendarControl");
                        script += String.Format("formControlCssClass['{0}'] = '';\n", publishTo);
                    }
                }
            }

            // Set script - it is registered on pre-render
            FormControlScript = script;
        }
    }


    /// <summary>
    /// Fill field type list with available types.
    /// </summary>
    private void FillFieldTypeList()
    {
        drpFieldType.Items.Clear();

        if (drpLayoutType.SelectedValue == LayoutTypeEnum.Html.ToString())
        {
            drpFieldType.Items.Add(new ListItem { Text = "general.label", Value = "label" });
            drpFieldType.Items.Add(new ListItem { Text = "layout.input", Value = "input" });
            drpFieldType.Items.Add(new ListItem { Text = "documenttype_edit_form.validationlabel", Value = "validation" });

            // Custom table forms use default submit button in header actions
            if (FormLayoutType != FormLayoutTypeEnum.CustomTable)
            {
                drpFieldType.Items.Add(new ListItem { Text = "documenttype_edit_form.submitbutton", Value = "submitbutton" });
            }
        }
        else if (drpLayoutType.SelectedValue == LayoutTypeEnum.Ascx.ToString())
        {
            drpFieldType.Items.Add(new ListItem { Text = "templatedesigner.section.field", Value = "field" });
            drpFieldType.Items.Add(new ListItem { Text = "general.label", Value = "label" });
            drpFieldType.Items.Add(new ListItem { Text = "layout.input", Value = "input" });
            drpFieldType.Items.Add(new ListItem { Text = "documenttype_edit_form.errorlabel", Value = "errorlabel" });

            // Custom table forms use default submit button in header actions
            if (FormLayoutType != FormLayoutTypeEnum.CustomTable)
            {
                drpFieldType.Items.Add(new ListItem { Text = "documenttype_edit_form.submitbutton", Value = "submitbutton" });
            }
        }
    }


    private void RegisterScripts()
    {
        // If user select "submitbutton" field, than hide availbale fields
        drpFieldType.Attributes.Add("onchange", "ShowHideAvailableFields()");

        // Add confirmation message before user change layout type
        drpLayoutType.Attributes.Add("onchange", String.Format(@"
if(!confirm({0})) {{ 
    this.value = originalLayoutType;
    return false;
}}", ScriptHelper.GetLocalizedString("documenttype_edit_form.confirmlayoutchange")));

        // Alert messages for JavaScript
        string script = String.Format(@"
var msgAltertExist = {0};
var msgAlertExistFinal = {1};
var msgConfirmDelete = {2};",
                              ScriptHelper.GetLocalizedString("DocumentType_Edit_Form.AlertExist"),
                              ScriptHelper.GetLocalizedString("DocumentType_Edit_Form.AlertExistFinal"),
                              ScriptHelper.GetLocalizedString("DocumentType_Edit_Form.ConfirmDelete"));

        // Element IDs
        script += String.Format(@"
var drpAvailableFields = document.getElementById('{0}');
var drpFieldType = document.getElementById('{1}');
var drpLayoutType = document.getElementById('{2}');
var lblForField = document.getElementById('{3}');
var ckEditorID = '{4}';
var originalLayoutType = '{5}';",
                        drpAvailableFields.ClientID,
                        drpFieldType.ClientID,
                        drpLayoutType.ClientID,
                        lblForField.ClientID,
                        htmlEditor.ClientID,
                        LayoutType);

        if (!string.IsNullOrEmpty(FormControlScript))
        {
            script += FormControlScript;
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "LayoutScript_" + ClientID, script, true);

        // Register a script file
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/AdminControls/Controls/Class/layout.js");
    }


    /// <summary>
    /// Returns validation script for save action.
    /// </summary>
    private string GetValidationScript()
    {
        string script = null;
        if (!CustomLayoutEnabled)
        {
            if (LayoutIsSet)
            {
                script = "ConfirmDelete()";
            }
        }
        else
        {
            script = "CheckContent()";
        }

        return script;
    }

    #endregion
}