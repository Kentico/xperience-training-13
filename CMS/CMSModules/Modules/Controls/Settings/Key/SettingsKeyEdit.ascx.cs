using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_Modules_Controls_Settings_Key_SettingsKeyEdit : CMSAdminEditControl
{
    #region "Private Members"

    private SettingsKeyInfo mSettingsKeyObj;
    private int mSettingsKeyId;
    private int mSelectedGroupId = -1;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets RootCategoryID. Specifies SettingsCategory which should be set up as the root of the SettingsCategorySelector.
    /// </summary>
    public int RootCategoryID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets SelectedGroupID. Specifies SettingsCategory for new record. If set, SettingsCategorySelector is not shown.
    /// </summary>
    public int SelectedGroupID
    {
        get
        {
            return mSelectedGroupId;
        }
        set
        {
            mSelectedGroupId = value;
        }
    }


    /// <summary>
    /// Gets or sets SettingsKeyID. Specifies ID of SettingsKey object.
    /// </summary>
    public int SettingsKeyID
    {
        get
        {
            return mSettingsKeyId;
        }
        set
        {
            mSettingsKeyId = value;
            mSettingsKeyObj = null;
        }
    }


    /// <summary>
    /// Gets or sets SettingsKey object. Specifies SettingsKey object which should be edited.
    /// </summary>
    public SettingsKeyInfo SettingsKeyObj
    {
        get
        {
            return mSettingsKeyObj ?? (mSettingsKeyObj = SettingsKeyInfoProvider.GetSettingsKeyInfo(mSettingsKeyId));
        }
        set
        {
            mSettingsKeyObj = value;
            mSettingsKeyId = (value != null) ? value.KeyID : 0;
        }
    }


    /// <summary>
    /// Default key value from/for appropriate control (text box or check box)
    /// </summary>
    protected string DefaultValue
    {
        get
        {
            switch (drpKeyType.SelectedValue)
            {
                case "boolean":
                    return chkKeyValue.Checked ? "True" : "False";

                case "longtext":
                    return txtLongTextKeyValue.Text.Trim();

                default:
                    return txtKeyValue.Text.Trim();
            }
        }
        set
        {
            switch (drpKeyType.SelectedValue)
            {
                case "boolean":
                    chkKeyValue.Checked = ValidationHelper.GetBoolean(value, false);
                    break;

                case "longtext":
                    txtLongTextKeyValue.Text = value;
                    break;

                default:
                    txtKeyValue.Text = value;
                    break;
            }
        }
    }


    /// <summary>
    /// Indicates current selected module.
    /// </summary>
    public int ModuleID
    {
        get;
        set;
    }


    /// <summary>
    /// Returns last time selected control name.
    /// </summary>
    private string ControlName
    {
        get
        {
            var controlName = ViewState["ControlName"];

            return ValidationHelper.GetString(controlName ?? ucSettingsKeyControlSelector.Value, null);
        }
        set
        {
            ViewState["ControlName"] = value;
        }
    }

    #endregion


    #region "Control Events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            return;
        }

        InitControls();

        // Load the form data
        if (!RequestHelper.IsPostBack())
        {
            LoadData();
        }
        else
        {
            ucSettingsKeyControlSelector.SetSelectorProperties(drpKeyType.SelectedValue);
        }

        // Set edited object
        EditedObject = (SettingsKeyID > 0) ? SettingsKeyObj : new SettingsKeyInfo();

        // Init form control settings
        LoadControlSettings(ControlName);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        SelectDefaultValueControl();

        RegisterKeyTypeChangeConfirmationModule();

        ControlName = ValidationHelper.GetString(ucSettingsKeyControlSelector.Value, null);
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Validate input
        var isValid = Validate();
        if (!isValid)
        {
            return;
        }

        // Update key
        SettingsKeyID = UpdateKey();
        RaiseOnSaved();

        // Show the info message
        ShowChangesSaved();

        // Show category selection
        trCategory.Visible = true;
    }


    protected void drpKeyType_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectDefaultValueControl();
    }


    protected void ucSettingsKeyControlSelector_Changed(object sender, EventArgs e)
    {
        // Reload control settings
        var controlName = ValidationHelper.GetString(ucSettingsKeyControlSelector.Value, null);
        LoadControlSettings(controlName);
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Initialization of controls.
    /// </summary>
    private void InitControls()
    {
        // Init validations
        rfvKeyDisplayName.ErrorMessage = ResHelper.GetString("general.requiresdisplayname");
        rfvKeyName.ErrorMessage = ResHelper.GetString("general.requirescodename");

        // Set the root category
        if (RootCategoryID > 0)
        {
            drpCategory.RootCategoryId = RootCategoryID;
        }

        // Enable only groups which belong to selected module
        drpCategory.EnabledCondition = "{% " + (!SystemContext.DevelopmentMode ? "(CategoryResourceID == " + ModuleID + ") && " : String.Empty) + "(CategoryIsGroup)%}";

        // If category specified programmatically
        if (SelectedGroupID >= 0)
        {
            // Set category selector value
            if (!RequestHelper.IsPostBack())
            {
                drpCategory.SelectedCategory = SelectedGroupID;
            }

            // Hide category selector
            trCategory.Visible = false;
        }
        else
        {
            // Set category selector value
            if (!RequestHelper.IsPostBack() && (SettingsKeyObj != null) && (SettingsKeyObj.KeyCategoryID > 0))
            {
                drpCategory.SelectedCategory = SettingsKeyObj.KeyCategoryID;
            }
        }

        if (!RequestHelper.IsPostBack())
        {
            LoadKeyTypes();
        }

        // Disable editing for keys not assigned to current module
        if (SettingsKeyObj != null)
        {
            SettingsCategoryInfo parentCategory = SettingsCategoryInfo.Provider.Get(SettingsKeyObj.KeyCategoryID);
            ResourceInfo resource = ResourceInfo.Provider.Get(ModuleID);
            plnEdit.Enabled = btnOk.Enabled = (resource != null) && (((parentCategory != null) && (parentCategory.CategoryResourceID == resource.ResourceID) && resource.ResourceIsInDevelopment) || SystemContext.DevelopmentMode);

        }

        ucControlSettings.BasicForm.EnsureMessagesPlaceholder(MessagesPlaceHolder);
    }


    /// <summary>
    /// Loads key types into the CMSDropDownList control.
    /// </summary>
    private void LoadKeyTypes()
    {
        drpKeyType.Items.Clear();
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.Boolean"), "boolean"));
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.Integer"), "int"));
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.Float"), "double"));
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.Text"), "string"));
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.LongText"), "longtext"));

        SelectDefaultValueControl();
    }


    /// <summary>
    /// Loads the data into the form.
    /// </summary>
    private void LoadData()
    {
        if (SettingsKeyObj == null)
        {
            return;
        }

        // Load the form from the Info object
        txtKeyName.Text = SettingsKeyObj.KeyName;
        txtKeyDisplayName.Text = SettingsKeyObj.KeyDisplayName;
        txtKeyDescription.Text = SettingsKeyObj.KeyDescription;
        txtKeyExplanationText.Text = SettingsKeyObj.KeyExplanationText;
        drpCategory.SelectedCategory = SettingsKeyObj.KeyCategoryID;
        drpKeyType.SelectedValue = SettingsKeyObj.KeyType;
        DefaultValue = SettingsKeyObj.KeyDefaultValue;
        txtKeyValidation.Text = SettingsKeyObj.KeyValidation;
        ucSettingsKeyControlSelector.SetSelectorProperties(SettingsKeyObj.KeyType, SettingsKeyObj.KeyEditingControlPath);

        chkKeyIsGlobal.Checked = SettingsKeyObj.KeyIsGlobal;
        chkKeyIsHidden.Checked = SettingsKeyObj.KeyIsHidden;
    }


    private void LoadControlSettings(string controlName)
    {
        var controlInfo = FormUserControlInfoProvider.GetFormUserControlInfo(controlName);
        if (controlInfo != null)
        {
            pnlControlSettings.Visible = true;

            ucControlSettings.Settings = new Hashtable();
            ucControlSettings.BasicForm.Mode = FormModeEnum.Insert;

            ucControlSettings.FormInfo = FormHelper.GetFormControlParameters(controlName, controlInfo.UserControlMergedParameters, true);

            if (SettingsKeyObj != null)
            {
                // Load form control settings
                if (!string.IsNullOrEmpty(SettingsKeyObj.KeyFormControlSettings))
                {
                    var formFieldInfo = FormHelper.GetFormControlSettingsFromXML(SettingsKeyObj.KeyFormControlSettings);
                    ucControlSettings.Settings = formFieldInfo.Settings;
                    ucControlSettings.MacroTable = formFieldInfo.SettingsMacroTable;
                }

                if (SettingsKeyObj.KeyEditingControlPath.EqualsCSafe(controlName, true))
                {
                    ucControlSettings.BasicForm.Mode = FormModeEnum.Update;
                }
            }

            ucControlSettings.Reload(true);

            pnlControlSettings.Visible = ucControlSettings.CheckVisibility();
        }
        else
        {
            pnlControlSettings.Visible = false;
        }
    }


    /// <summary>
    /// Updates settings key for all sites (or only global if the IsGlobal checkbox is checked).
    /// </summary>
    /// <returns>CodeName of the SettingsKey objects.</returns>
    private int UpdateKey()
    {
        // Try to get the key
        var keyObj = (SettingsKeyID > 0) ? SettingsKeyInfoProvider.GetSettingsKeyInfo(SettingsKeyID) : null;
        if (keyObj == null)
        {
            // Create new
            keyObj = new SettingsKeyInfo();
        }

        var oldKeyCategoryID = keyObj.KeyCategoryID;
        bool keyTypeChanged = (keyObj.KeyType != drpKeyType.SelectedValue);

        // Set values
        keyObj.KeyName = txtKeyName.Text.Trim();
        keyObj.KeyDisplayName = txtKeyDisplayName.Text.Trim();
        keyObj.KeyDescription = txtKeyDescription.Text.Trim();
        keyObj.KeyExplanationText = txtKeyExplanationText.Text.Trim();
        keyObj.KeyCategoryID = SelectedGroupID >= 0 ? SelectedGroupID : drpCategory.SelectedCategory;
        keyObj.KeyIsGlobal = chkKeyIsGlobal.Checked;
        keyObj.KeyIsHidden = chkKeyIsHidden.Checked;
        keyObj.KeyType = drpKeyType.SelectedValue;
        keyObj.KeyValidation = (string.IsNullOrEmpty(txtKeyValidation.Text.Trim()) ? null : txtKeyValidation.Text.Trim());
        keyObj.KeyDefaultValue = (string.IsNullOrEmpty(DefaultValue) ? null : DefaultValue);

        var path = ValidationHelper.GetString(ucSettingsKeyControlSelector.ControlPath, string.Empty);
        keyObj.KeyEditingControlPath = (string.IsNullOrEmpty(path.Trim()) ? null : path.Trim());

        // Update form control settings
        if (ucSettingsKeyControlSelector.IsFormControlSelected)
        {
            var formFieldInfo = new FormFieldInfo();
            ucControlSettings.SaveData();

            formFieldInfo.SettingsMacroTable = ucControlSettings.MacroTable;

            if ((ucControlSettings.FormData != null) && (ucControlSettings.FormData.ItemArray.Length > 0))
            {
                foreach (DataColumn column in ucControlSettings.FormData.Table.Columns)
                {
                    formFieldInfo.Settings[column.ColumnName] = ucControlSettings.FormData.Table.Rows[0][column.Caption];
                }
            }

            var settings = FormHelper.GetFormControlSettingsXml(formFieldInfo);
            keyObj.KeyFormControlSettings = settings;
        }
        else
        {
            keyObj.KeyFormControlSettings = null;
        }

        if (keyObj.KeyID == 0)
        {
            keyObj.KeyValue = DefaultValue;
        }

        if (chkKeyIsGlobal.Checked)
        {
            keyObj.SiteID = 0;
        }

        // If category changed set new order or if new set on the end of key list
        if (keyObj.KeyCategoryID != oldKeyCategoryID)
        {
            var keys = SettingsKeyInfoProvider.GetSettingsKeys(keyObj.KeyCategoryID)
                .OrderByDescending("KeyOrder")
                .Column("KeyOrder");

            keyObj.KeyOrder = keys.GetScalarResult(0) + 1;
        }

        SettingsKeyInfoProvider.SetSettingsKeyInfo(keyObj);

        if (keyTypeChanged)
        {
            UpdateKeyValuesAfterTypeChanged(keyObj);
        }

        // Update property
        mSettingsKeyObj = keyObj;

        return keyObj.KeyID;
    }


    /// <summary>
    /// Updates key values to default after type change.
    /// </summary>
    /// <param name="keyObj">Global key that changed</param>
    private void UpdateKeyValuesAfterTypeChanged(SettingsKeyInfo keyObj)
    {
        // Loop through all the keys (site and global) and set the value to the selected default one
        foreach (var settingKey in SettingsKeyInfoProvider.GetSettingsKeys().WhereEquals("KeyName", keyObj.KeyName))
        {
            settingKey.KeyValue = keyObj.KeyDefaultValue;
            SettingsKeyInfoProvider.SetSettingsKeyInfo(settingKey);
        }
    }


    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool Validate()
    {
        bool isValid = true;

        // Validate form fields
        string errMsg = new Validator().NotEmpty(txtKeyName.Text.Trim(), ResHelper.GetString("general.requirescodename"))
            .NotEmpty(txtKeyDisplayName.Text.Trim(), ResHelper.GetString("general.requiresdisplayname"))
            .IsIdentifier(txtKeyName.Text.Trim(), GetString("general.erroridentifierformat"))
            .Result;

        // Validate default value format
        if (!String.IsNullOrEmpty(DefaultValue))
        {
            switch (drpKeyType.SelectedValue)
            {
                case "double":
                    if (!ValidationHelper.IsDouble(DefaultValue))
                    {
                        lblDefValueError.Text = ResHelper.GetString("settings.validationdoubleerror");
                    }
                    break;

                case "int":
                    if (!ValidationHelper.IsInteger(DefaultValue))
                    {
                        lblDefValueError.Text = ResHelper.GetString("settings.validationinterror");
                    }
                    break;
            }

            if (!String.IsNullOrEmpty(lblDefValueError.Text))
            {
                lblDefValueError.Visible = true;
                isValid = false;
            }
        }

        if (!ucSettingsKeyControlSelector.IsValid())
        {
            errMsg = GetString("settingskeyedit.selectcustomcontrol");
        }

        if (String.IsNullOrEmpty(errMsg))
        {
            // Check if the code name is unique
            var key = SettingsKeyInfoProvider.GetSettingsKeyInfo(txtKeyName.Text.Trim());
            if ((key != null) && (key.KeyID != SettingsKeyID))
            {
                errMsg = GetString("general.codenameexists");
            }
        }

        // Set up error message
        if (!String.IsNullOrEmpty(errMsg))
        {
            ShowError(errMsg);
            isValid = false;
        }

        return isValid;
    }


    /// <summary>
    /// Shows suitable default value edit control according to key type from drpKeyType.
    /// </summary>
    private void SelectDefaultValueControl()
    {
        chkKeyValue.Visible = drpKeyType.SelectedValue == "boolean";
        txtLongTextKeyValue.Visible = drpKeyType.SelectedValue == "longtext";
        txtKeyValue.Visible = !chkKeyValue.Visible && !txtLongTextKeyValue.Visible;
    }


    /// <summary>
    /// Registers JS module that adds check for key type change.
    /// </summary>
    private void RegisterKeyTypeChangeConfirmationModule()
    {
        // Do not register module if the postback is partial
        if (!RequestHelper.IsAsyncPostback() && (mSettingsKeyObj != null))
        {
            ScriptHelper.RegisterModule(this, "CMS/SettingsKeyTypeConfirmation", new
            {
                SaveActionClass = btnOk.CssClass,
                KeyTypeID = drpKeyType.ClientID,
                ConfirmMessage = ScriptHelper.GetString(GetString("settingskey.resettodefaultvalues"), false),
                InitialKeyType = mSettingsKeyObj.KeyType
            });
        }
    }

    #endregion
}
