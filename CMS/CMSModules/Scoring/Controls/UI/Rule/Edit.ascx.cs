using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Activities;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.UIControls;

using AjaxControlToolkit;

/// <remarks>
/// Both attribute and activity type of rule use additional form for detailed rule setting. On the other hand, 
/// macro uses only one Condition builder. Therefore methods working with macro condition are much simpler and 
/// manipulation with macro rule is quite different from the other types.
/// </remarks>
public partial class CMSModules_Scoring_Controls_UI_Rule_Edit : CMSAdminEditControl
{
    #region "Variables"

    private List<FormFieldInfo> mFields;
    private FormFieldInfo mSelectedAttribute;
    private string mRecalculationNeededResourceString = "om.score.recalculationrequired2";
    private string mNewRuleUrl;
    private string mResourceName = ModuleName.SCORING;
    private const string DEFAULT_ACTIVITY_TYPE = PredefinedActivityType.PAGE_VISIT;

    #endregion


    #region "Properties"

    /// <summary>
    /// Reference to currently edited rule.
    /// </summary>
    private RuleInfo Rule
    {
        get
        {
            return (RuleInfo)editForm.EditedObject;
        }
    }


    /// <summary>
    /// Gets or sets value indicating what item was selected in activity type drop-down list.
    /// </summary>
    private string PreviousSelectedActivity
    {
        get
        {
            return ValidationHelper.GetString(ViewState["PreviousActivityType"], DEFAULT_ACTIVITY_TYPE);
        }
        set
        {
            ViewState["PreviousActivityType"] = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating what item was selected in attribute type drop-down list.
    /// </summary>
    private int PreviousSelectedAttribute
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PreviousAttributeField"], 0);
        }
        set
        {
            ViewState["PreviousAttributeField"] = value;
        }
    }


    /// <summary>
    /// Gets radio button list containing all types of rule available.
    /// </summary>
    private CMSFormControls_Basic_RadioButtonsControl RadioButtonsListRuleType
    {
        get
        {
            // Currently it is not possible to obtain value directly from Field.EditingObject, thus reference to field from Form fields collection has to be used instead
            return (CMSFormControls_Basic_RadioButtonsControl)editForm.FieldControls["ruletype"];
        }
    }


    /// <summary>
    /// Gets value indicating what rule type was selected within radio button list.
    /// </summary>
    private RuleTypeEnum SelectedRuleType
    {
        get
        {
            return (RuleTypeEnum)ValidationHelper.GetInteger(RadioButtonsListRuleType.Value, 1);
        }
    }


    /// <summary>
    /// ID of parent score.
    /// </summary>
    public int ScoreId
    {
        get;
        set;
    }


    /// <summary>
    /// Specifies path to new rule page. When not set, default value is used.
    /// </summary>
    public string NewRuleUrl
    {
        get
        {
            if (mNewRuleUrl == null)
            {
                return "Tab_Rules_Edit.aspx?scoreid=" + ScoreId;
            }

            return mNewRuleUrl;
        }
        set
        {
            mNewRuleUrl = value;
        }
    }


    /// <summary>
    /// URL page will be redirect after the rule is saved.
    /// </summary>
    public string RedirectUrlAfterCreate
    {
        get
        {
            return editForm.RedirectUrlAfterCreate;
        }
        set
        {
            editForm.RedirectUrlAfterCreate = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the resource used in permission check. If not set, uses CMS.Scoring by default.
    /// </summary>
    public string ResourceName
    {
        get
        {
            return mResourceName;
        }
        set
        {
            mResourceName = value;
        }
    }


    public UIForm UIForm
    {
        get
        {
            return editForm;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        editForm.OnBeforeSave += EditForm_OnBeforeSave;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        var score = ScoreInfo.Provider.Get(ScoreId);
        if (score == null)
        {
            RedirectToInformation("Internal error. Unknown score.");
        }

        // Check permissions
        if (!score.CheckPermissions(PermissionsEnum.Modify, CurrentSite.SiteName, CurrentUser))
        {
            UIForm.Enabled = false;
            ShowError(GetString("ui.notauthorizemodified"));
            UIForm.StopProcessing = true;
        }

        InitControls();
        InitHeaderActions();

        // Note: controls has to be initialized here with previously selected values to avoid "Must match the control tree" error when loading ViewState
        switch (SelectedRuleType)
        {
            case RuleTypeEnum.Activity:
                InitActivitySettings(PreviousSelectedActivity);
                break;

            case RuleTypeEnum.Attribute:
                InitAttributeSettings(PreviousSelectedAttribute);
                break;
            case RuleTypeEnum.Macro:
                InitMacroSettings();
                break;
        }

        if (QueryHelper.GetBoolean("saved", false))
        {
            InitWarnings();
        }

        // Use mode always to update header actions to show warning info all the time.
        HeaderActions.UpdatePanel.UpdateMode = UpdatePanelUpdateMode.Always;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide all changing parts
        HideAllPanels();

        switch (SelectedRuleType)
        {
            case RuleTypeEnum.Activity:
                ActivityRuleTypeSelected();
                break;

            case RuleTypeEnum.Attribute:
                AttributeRuleTypeSelected();
                break;

            case RuleTypeEnum.Macro:
                MacroRuleTypeSelected();
                CheckMacros();
                break;
        }
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// UIForm OnBeforeSave event handler.
    /// </summary>
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Store parent score ID
        editForm.Data["RuleScoreID"] = ScoreId;

        // Store attribute or activity rule
        editForm.Data["RuleType"] = (int)SelectedRuleType;

        switch (SelectedRuleType)
        {
            case RuleTypeEnum.Activity:
                StoreActivityData();
                return;

            case RuleTypeEnum.Attribute:
                StoreAttributeData();
                return;

            case RuleTypeEnum.Macro:
                StoreMacroData();
                return;
        }
    }


    /// <summary>
    /// Activity drop-down SelectionChanged event handler.
    /// </summary>
    private void ActivitySelectionChanged(object sender, EventArgs e)
    {
        // Init rule condition when changing activity type
        if (Rule != null)
        {
            Rule.RuleCondition = null;
        }

        // Save current selected index to ViewState (needed to avoid "Must match the control tree" error when loading ViewState)
        PreviousSelectedActivity = ucActivityType.SelectedValue;

        // Initialize form using new (current) activity type
        InitActivitySettings(ucActivityType.SelectedValue);
    }


    /// <summary>
    /// Attribute drop-down SelectionChanged event handler.
    /// </summary>
    private void AttributeSelectionChanged(object sender, EventArgs e)
    {
        // Save current selected index to ViewState (needed to avoid "Must match the control tree" error when loading ViewState)
        PreviousSelectedAttribute = drpAttribute.SelectedIndex;

        // Load form specific for currently selected attribute
        LoadAttributeForm(mFields[drpAttribute.SelectedIndex]);
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        CheckPermissions(ResourceName, "Modify");
        switch (e.CommandName.ToLowerCSafe())
        {
            // Save rule
            case "save":
                if (ValidateForm())
                {
                    editForm.SaveData(null);
                    InitWarnings();
                }
                break;
        }
    }

    #endregion


    #region "General methods"

    /// <summary>
    /// Checks whether given form control is available. If so, sets it to referring control name.
    /// </summary>
    /// <param name="formControl">Codename of form control</param>
    /// <param name="controlName">Referring control name to which will be set codename of control, if exists</param>
    private void CheckFormControl(string formControl, ref string controlName)
    {
        // Check if user defined control exists
        FormUserControlInfo fui = FormUserControlInfoProvider.GetFormUserControlInfo(formControl);
        if (fui != null)
        {
            controlName = formControl;
        }
    }


    /// <summary>
    /// Clears validity data that could be stored in EditForm data collection.
    /// This data is only needed when Activity type of rule is selected, otherwise it is irrelevant.
    /// </summary>
    private void ClearStoredValidityData()
    {
        editForm.Data["RuleValidity"] = null;
        editForm.Data["RuleValidUntil"] = null;
        editForm.Data["RuleValidFor"] = null;
    }


    /// <summary>
    /// Checks datatype of given form field and return name of control which should be used with fields; sets up form field info 
    /// with DataType related setting.
    /// </summary>
    /// <param name="ffi">Form field to be examined</param>
    /// <returns>Name of appropriate control name</returns>
    private static string GetControlNameForFieldDataType(FormFieldInfo ffi)
    {
        string controlName = null;

        switch (ffi.DataType)
        {
            case FieldDataType.Text:
            case FieldDataType.LongText:
                controlName = "textfilter";
                ffi.Settings["OperatorFieldName"] = ffi.Name + ".operator";
                break;

            case FieldDataType.DateTime:
                controlName = "datetimefilter";
                ffi.Settings["SecondDateFieldName"] = ffi.Name + ".seconddatetime";
                break;

            case FieldDataType.Integer:
            case FieldDataType.LongInteger:
                controlName = "numberfilter";
                ffi.Settings["OperatorFieldName"] = ffi.Name + ".operator";
                break;
        }
        return controlName;
    }


    /// <summary>
    /// Hides all panels with fields specific to rule types.
    /// </summary>
    private void HideAllPanels()
    {
        pnlActivityPlaceHolder.Visible = false;
        plcActivitySettings.Visible = false;

        plcAttributeSettings.Visible = false;

        pnlMacroSettings.Visible = false;
    }


    /// <summary>
    /// Loads basicform with filter controls.
    /// </summary>
    /// <param name="bf">BasicForm control</param>
    /// <param name="fi">Form definition</param>
    /// <param name="activityType">Activity type in case the rule type is activity</param>
    private void LoadForm(BasicForm bf, FormInfo fi, string activityType = null)
    {
        bf.FormInformation = fi;
        bf.Data = RuleHelper.GetDataFromCondition(Rule, fi.GetDataRow().Table, ref activityType);
        bf.SubmitButton.Visible = false;
        bf.SiteName = SiteContext.CurrentSiteName;

        if (Rule != null)
        {
            bf.Mode = FormModeEnum.Update;
        }

        bf.ReloadData();
    }


    /// <summary>
    /// Finds form field by given name and changes its behavior to accepts only digits and negative sign; limits maximal length of field. This changes cannot be done in markup.
    /// </summary>
    /// <param name="name">Name of field</param>
    private void NumbersOnlyTextBox(string name)
    {
        // Get bottom text box object 
        var textbox = editForm.FieldControls[name] as CMSFormControls_Basic_TextBoxControl;
        if (textbox == null)
        {
            return;
        }

        // These settings are propagated to javascript, thus not only wrong data does not pass validation check, but wrong data cannot be inserted in the first place
        textbox.MaxLength = 11;
        textbox.FilterType = FilterTypes.Numbers | FilterTypes.Custom;
        textbox.ValidChars = "-";
    }


    /// <summary>
    /// Displays error message in header section of page.
    /// </summary>
    /// <param name="errorMessage">Error message to be displayed</param>
    private void DisplayError(string errorMessage)
    {
        ShowError(TextHelper.LimitLength(errorMessage, 200), (errorMessage.Length > 200 ? errorMessage : String.Empty));
    }


    /// <summary>
    /// Sets the caption of field, if resource with given caption key is available.
    /// </summary>
    /// <param name="ffi">Form field info</param>
    /// <param name="captionKey">Resource key of caption</param>
    private void SetCaptionToField(FormFieldInfo ffi, string captionKey)
    {
        // Set detailed caption
        string caption = GetString(captionKey);
        if (!caption.EqualsCSafe(captionKey, true))
        {
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, caption);
        }
    }


    /// <summary>
    /// Performs custom validation and displays error in top of the page.
    /// </summary>
    /// <returns>Returns true if validation is successful.</returns>
    private bool ValidateForm()
    {
        ScoreInfo info = ScoreInfo.Provider.Get(ScoreId);
        if (info.ScoreStatus == ScoreStatusEnum.Recalculating)
        {
            ShowError(GetString("om.score.editrulefailedrecalculating"));
            return false;
        }

        switch (SelectedRuleType)
        {
            case RuleTypeEnum.Activity:
                return ValidateActivityData();

            case RuleTypeEnum.Attribute:
                return ValidateAttributeData();

            case RuleTypeEnum.Macro:
                return ValidateMacroData();
        }

        return true;
    }

    #endregion


    #region "Overriding UI Texts"

    /// <summary>
    /// This class holds information about text displayed on the page which will be displayed instead the default texts.
    /// </summary>
    public class UITexts
    {
        public string DisplayNameTooltipResourceString
        {
            get;
            set;
        }
        public string ScoreValueTooltipResourceString
        {
            get;
            set;
        }
        public string ScoreValueLabelResourceString
        {
            get;
            set;
        }
        public string RecalculationNeededResourceString
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Sets different values for the specified labels and tooltips of the UI form. This method has to be called on the Load phase!
    /// </summary>
    /// <param name="uiTexts">New texts</param>
    public void OverrideUITexts(UITexts uiTexts)
    {
        if (uiTexts == null)
        {
            throw new ArgumentNullException("uiTexts");
        }

        fDisplayName.ToolTipResourceString = uiTexts.DisplayNameTooltipResourceString;
        fValue.ResourceString = uiTexts.ScoreValueLabelResourceString;
        fValue.ToolTipResourceString = uiTexts.ScoreValueTooltipResourceString;

        mRecalculationNeededResourceString = uiTexts.RecalculationNeededResourceString;
    }

    #endregion


    #region "Initialize methods"

    /// <summary>
    /// Initializes controls, preselects items in Activity Validity field set.
    /// </summary>
    private void InitControls()
    {
        // Set validity setting information from edited rule. Because specific form control is used, FFI is not able to set data itself
        if ((!RequestHelper.IsPostBack()) && (Rule != null))
        {
            validity.ValidUntil = Rule.RuleValidUntil;
            validity.Validity = Rule.RuleValidity;
            validity.ValidFor = Rule.RuleValidFor;
        }

        // Sets up fields with specific numbers-only settings
        NumbersOnlyTextBox("rulevalue");
        NumbersOnlyTextBox("rulemaxpoints");

        drpAttribute.SelectedIndexChanged += AttributeSelectionChanged;
        ucActivityType.OnSelectedIndexChanged += ActivitySelectionChanged;

        // Update rule settings panel when changing selected rule type
        RadioButtonsListRuleType.Changed += (s, e) => upnlSettings.Update();

        // Form can be saved not only by header actions. It can be saved also using ENTER key for example.
        UIForm.ObjectManager.OnBeforeAction += (s, e) =>
        {
            e.IsValid = ValidateForm();
        };
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        if ((Rule != null) && (Rule.RuleID != 0))
        {
            AddHeaderAction(new HeaderAction
            {
                Text = GetString("om.score.newrule"),
                RedirectUrl = ResolveUrl(NewRuleUrl),
                ButtonStyle = ButtonStyle.Default
            });
        }

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Shows warning message if score needs to be rebuilt.
    /// </summary>
    private void InitWarnings()
    {
        ScoreInfo info = ScoreInfo.Provider.Get(ScoreId);

        // Do nothing if recalculation is already scheduled
        if (info.ScoreScheduledTaskID > 0)
        {
            TaskInfo taskInfo = TaskInfo.Provider.Get(info.ScoreScheduledTaskID);
            if (taskInfo != null && taskInfo.TaskEnabled)
            {
                return;
            }
        }

        if (info.ScoreStatus == ScoreStatusEnum.RecalculationRequired)
        {
            ShowInformation(GetString(mRecalculationNeededResourceString));
        }
    }

    #endregion


    #region "Attribute rule type methods"

    /// <summary>
    /// Handles content tree changes performed when Attribute type of rule is selected.
    /// </summary>
    private void AttributeRuleTypeSelected()
    {
        // Show markup containing controls related to attribute settings
        plcAttributeSettings.Visible = true;

        if (attributeFormCondition.FieldLabels != null)
        {
            // Change visual style of form label
            LocalizedLabel ll = attributeFormCondition.FieldLabels[mSelectedAttribute.Name];
            if (ll != null)
            {
                ll.ResourceString = "om.score.condition";
            }
        }
    }


    /// <summary>
    /// Initializes controls for the Attribute type of rule.
    /// </summary>
    /// <param name="selectedAttribute">Index of selected field in attribute drop-down menu</param>
    private void InitAttributeSettings(int selectedAttribute)
    {
        // This method can be called several times, if mFields are already filled, there is no need to call it again
        if (mFields == null)
        {
            // Load all possible attributes from FormInfo specified for score rules
            FormInfo filterFieldsForm = FormHelper.GetFormInfo(ContactInfo.OBJECT_TYPE + ".ScoringAttributeRule", false);

            var fields = (from formField in filterFieldsForm.GetFields(true, false)
                          let caption = ResHelper.LocalizeString(formField.GetDisplayName(MacroContext.CurrentResolver))
                          orderby caption
                          select new
                          {
                              Caption = caption,
                              formField
                          }).ToList();


            if (drpAttribute.Items.Count == 0)
            {
                // Fill fields to attribute drop-down menu, if not already filled
                fields.ForEach(field => drpAttribute.Items.Add(new ListItem(field.Caption, field.formField.Name)));
            }

            if ((Rule != null) && !RequestHelper.IsPostBack())
            {
                // When editing existing rule, specify selected field in drop-down menu and override the previous selected one
                drpAttribute.SelectedValue = ValidationHelper.GetString(editForm.Data["RuleParameter"], null);
                selectedAttribute = drpAttribute.SelectedIndex;
                PreviousSelectedAttribute = selectedAttribute;
            }

            // Store all available fields for further use in current request
            mFields = fields.Select(c => c.formField).ToList();
        }

        // Loads form for selected attribute
        LoadAttributeForm(mFields[selectedAttribute]);
    }


    /// <summary>
    /// Loads form with specific settings for rule of type Attribute.
    /// </summary>
    /// <param name="selectedAttribute">Reference to field selected in attribute drop-down menu</param>
    private void LoadAttributeForm(FormFieldInfo selectedAttribute)
    {
        // Store current selected attribute for further usage within current request
        mSelectedAttribute = selectedAttribute;

        // Create FormInfo which will be passed to basic form already set in markup
        // According to selected attribute specific fields will be loaded into the basic form
        var fi = new FormInfo();
        fi.AddFormItem(selectedAttribute);
        LoadForm(attributeFormCondition, fi);
    }


    /// <summary>
    /// Stores Attribute form data to main Edit form.
    /// </summary>
    private void StoreAttributeData()
    {
        // For attribute rule don't store validity
        ClearStoredValidityData();

        // Only activity rules can be recurring and have max points
        editForm.Data["RuleIsRecurring"] = false;
        editForm.Data["RuleMaxPoints"] = null;

        // Store contact column for attribute rule
        editForm.Data["RuleParameter"] = mSelectedAttribute.Name;

        // Store xml with where condition
        attributeFormCondition.SaveData(null);
        editForm.Data["RuleCondition"] = RuleHelper.GetAttributeCondition(attributeFormCondition.Data, attributeFormCondition.GetWhereCondition());
    }


    /// <summary>
    /// Validates fields related to Attribute rule type.
    /// </summary>
    /// <returns>True, if validation check was successful, false otherwise</returns>
    private bool ValidateAttributeData()
    {
        // Check the attribute custom form
        if (!attributeFormCondition.ValidateData())
        {
            DisplayError(attributeFormCondition.ValidationErrorMessage);
            return false;
        }

        return true;
    }

    #endregion


    #region "Activity rule type methods"

    /// <summary>
    /// Handles content tree changes performed when Activity type of rule is selected.
    /// </summary>
    private void ActivityRuleTypeSelected()
    {
        pnlActivityPlaceHolder.Visible = true;
        plcActivitySettings.Visible = true;
    }


    /// <summary>
    /// Checks form field name and tries to find appropriate form control for field representation.
    /// </summary>
    /// <param name="ffi">Form field info</param>
    /// <param name="ati">Activity type info</param>
    /// <param name="selectedActivity">Activity selected in drop-down menu</param>
    /// <param name="activitiesWithValue">Collection of activities with values</param>
    /// <param name="controlName">Referring control name</param>
    /// <returns>
    /// False, if activity form control is not defined or activity should has value and is not present in given
    /// collection; true otherwise. Sets referring control name, if proper form control is found.
    /// </returns>
    private bool GetControlNameForActivities(FormFieldInfo ffi, ActivityTypeInfo ati, string selectedActivity, IEnumerable<string> activitiesWithValue, ref string controlName)
    {
        string nameToLower = (ffi.Name ?? "").ToLowerInvariant();
        switch (nameToLower)
        {
            case "activityitemid":
                if (string.IsNullOrEmpty(ati.ActivityTypeMainFormControl))
                {
                    return false;
                }

                CheckFormControl(ati.ActivityTypeMainFormControl, ref controlName);
                SetCaptionToField(ffi, "activityitem." + selectedActivity);
                break;

            case "activityitemdetailid":
                if (string.IsNullOrEmpty(ati.ActivityTypeDetailFormControl))
                {
                    return false;
                }

                CheckFormControl(ati.ActivityTypeDetailFormControl, ref controlName);
                SetCaptionToField(ffi, "activityitemdetail." + selectedActivity);
                break;

            case "activitycreated":
                SetCaptionToField(ffi, "om.activity.createdbetween");
                break;

            case "activitynodeid":
                // Document selector for NodeID
                controlName = "selectdocument";
                break;

            case "activityculture":
                // Culture selector for culture
                controlName = "sitecultureselector";
                break;

            case "activityvalue":
                // Show activity value only for relevant activity types
                if ((!ati.ActivityTypeIsCustom) && (!activitiesWithValue.Contains(selectedActivity)))
                {
                    return false;
                }
                break;
        }

        return true;
    }


    /// <summary>
    /// Initializes controls for activity rule.
    /// </summary>
    /// <param name="selectedActivity">Activity selected in drop-down menu</param>
    private void InitActivitySettings(string selectedActivity)
    {
        // Init activity selector from  edited object if any
        LoadEditedActivityRule(ref selectedActivity);

        List<string> ignoredColumns = new List<string>
        {
            "activitytype",
            "activitysiteid",
            "activitycontactid",
        };

        string[] activitiesWithValue =
        {
            PredefinedActivityType.PURCHASE,
            PredefinedActivityType.PURCHASEDPRODUCT,
            PredefinedActivityType.PRODUCT_ADDED_TO_SHOPPINGCART
        };

        // Get columns from OM_Activity (i.e. base table for all activities)
        ActivityTypeInfo ati = ActivityTypeInfo.Provider.Get(selectedActivity);
        var fi = new FormInfo();

        // Get the activity form elements
        FormInfo filterFieldsForm = FormHelper.GetFormInfo(ActivityInfo.OBJECT_TYPE, true);
        IList<IDataDefinitionItem> elements = filterFieldsForm.GetFormElements(true, false);

        if ((selectedActivity != PredefinedActivityType.PAGE_VISIT) && (selectedActivity != PredefinedActivityType.LANDING_PAGE))
        {
            // Show these fields only for 'Page visit' and 'Landing page'
            ignoredColumns.AddRange(new[] { "activityabvariantname" });
        }

        FormCategoryInfo newCategory = null;

        foreach (IDataDefinitionItem elem in elements)
        {
            if (elem is FormCategoryInfo)
            {
                // Form category
                newCategory = (FormCategoryInfo)elem;
            }
            else if (elem is FormFieldInfo)
            {
                // Form field
                var ffi = (FormFieldInfo)elem;

                // Skip ignored columns
                if (ignoredColumns.Contains(ffi.Name, StringComparer.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!ffi.PrimaryKey && (fi.GetFormField(ffi.Name) == null))
                {
                    // Skip fields with Guid data type
                    if (ffi.DataType == FieldDataType.Guid)
                    {
                        continue;
                    }

                    // Sets control name based on given datatype of field. Can be overwritten if more proper control is available
                    string controlName = GetControlNameForFieldDataType(ffi);
                    if (!GetControlNameForActivities(ffi, ati, selectedActivity, activitiesWithValue, ref controlName))
                    {
                        continue;
                    }

                    if (controlName != null)
                    {
                        // SKU selector for product
                        ffi.SetControlName(controlName);
                        ffi.Settings["allowempty"] = ffi.HasFormFieldControlWithName("skuselector");
                    }

                    // Ensure the category
                    if (newCategory != null)
                    {
                        fi.AddFormCategory(newCategory);
                        newCategory = null;
                    }

                    fi.AddFormItem(ffi);
                }
            }
        }

        LoadActivityForm(fi, selectedActivity);
    }


    /// <summary>
    /// Loads form with specific settings for rule of type Activity.
    /// </summary>
    /// <param name="fi">FormInfo containing information about activity</param>
    /// <param name="activityType">Type of activity</param>
    private void LoadActivityForm(FormInfo fi, string activityType)
    {
        LoadForm(activityFormCondition, fi, activityType);
    }


    /// <summary>
    /// Checks if editing already existing rule. If so, and the request is not postback,
    /// preselects proper activity and sets previous selected value to ViewState.
    /// </summary>
    /// <param name="selectedActivity">
    /// Referring selected activity. If editing already existing rule, its value is replaced with the saved one
    /// </param>
    private void LoadEditedActivityRule(ref string selectedActivity)
    {
        if ((Rule != null) && !RequestHelper.IsPostBack())
        {
            // When editing existing rule, specify selected field in drop-down menu and override the previous selected one
            ucActivityType.Value = ValidationHelper.GetString(editForm.Data["RuleParameter"], DEFAULT_ACTIVITY_TYPE);
            selectedActivity = ucActivityType.SelectedValue;
            PreviousSelectedActivity = selectedActivity;
        }

        // Select the default value when there is no value selected in the activity type selector
        if (string.IsNullOrEmpty(ucActivityType.Value.ToString(null)))
        {
            ucActivityType.Value = DEFAULT_ACTIVITY_TYPE;
        }
    }


    /// <summary>
    /// Stores Activity form data to main Edit form.
    /// </summary>
    private void StoreActivityData()
    {
        editForm.Data["RuleValidity"] = (int)validity.Validity;
        editForm.Data["RuleValidUntil"] = null;

        if (validity.Validity == ValidityEnum.Until)
        {
            if (validity.ValidUntil != DateTimeHelper.ZERO_TIME)
            {
                editForm.Data["RuleValidUntil"] = validity.ValidUntil;
            }
        }
        else
        {
            editForm.Data["RuleValidFor"] = validity.ValidFor;
        }

        // Store contact column for attribute rule
        var activityType = ucActivityType.SelectedValue;
        var activityTypeInfo = ActivityTypeInfo.Provider.Get(activityType);
        editForm.Data["RuleParameter"] = activityType;
        editForm.Data["RuleActivityItemObjectType"] = activityTypeInfo?.ActivityTypeItemObjectType;
        editForm.Data["RuleActivityItemDetailObjectType"] = activityTypeInfo?.ActivityTypeItemDetailObjectType;

        // Store xml with Where condition
        activityFormCondition.SaveData(null);

        editForm.Data["RuleActivityItemID"] = activityFormCondition.Data["ActivityItemID"];
        editForm.Data["RuleActivityItemDetailID"] = activityFormCondition.Data["ActivityItemDetailID"];

        string whereCond = "ActivityType='" + SqlHelper.EscapeQuotes(ucActivityType.SelectedValue) + "'";
        whereCond = SqlHelper.AddWhereCondition(whereCond, activityFormCondition.GetWhereCondition());        

        editForm.Data["RuleCondition"] = RuleHelper.GetActivityCondition(activityFormCondition.Data, whereCond, ucActivityType.SelectedValue);
    }


    /// <summary>
    /// Validates fields related to Activity rule type.
    /// </summary>
    /// <returns>True, is validation check was successful, false otherwise</returns>
    private bool ValidateActivityData()
    {
        bool isValid = true;
        string errorMessage = validity.Validate();
        if (!string.IsNullOrEmpty(errorMessage))
        {
            DisplayError(errorMessage);
            isValid = false;
        }

        // Checks if score value is not outside the boundaries specified in max rule points field
        if (!ValidateMaximumValueField())
        {
            isValid = false;
        }

        // Check the activity custom form
        if (!activityFormCondition.ValidateData())
        {
            DisplayError(activityFormCondition.ValidationErrorMessage);
            isValid = false;
        }

        return isValid;
    }


    /// <summary>
    /// Validates maximum value field, if rule is set to be recurring.
    /// Compares values entered in the Score value and the Max. rule points fields.
    /// </summary>
    /// <returns>True, if rule is not recurring, or if value is between boundaries declared by max value field; false otherwise</returns>
    private bool ValidateMaximumValueField()
    {
        var recurringField = editForm.FieldControls["RuleIsRecurring"];
        var maxValueField = editForm.FieldControls["RuleMaxPoints"];
        var valueField = editForm.FieldControls["RuleValue"];

        // Validate maximum value field
        if (ValidationHelper.GetBoolean(recurringField.Value, false))
        {
            string maxValueStr = maxValueField.Text.Trim();
            if (!String.IsNullOrEmpty(maxValueStr))
            {
                if (!ValidationHelper.IsInteger(maxValueStr))
                {
                    DisplayError(GetString("om.score.enterintegervalue"));
                    return false;
                }

                // Check that maximum value is greater than value
                int value = ValidationHelper.GetInteger(valueField.Text, 0);
                int maxValue = ValidationHelper.GetInteger(maxValueStr, 0);
                if ((value != 0) && (maxValue / value < 1)) // Values can be both negative and positive numbers, this condition ensures value is not outside the boundaries defined by maxValue
                {
                    DisplayError(GetString("om.score.smallmaxvalue"));
                    return false;
                }
            }
        }
        return true;
    }

    #endregion


    #region "Macro rule type methods"

    /// <summary>
    /// Handles content tree changes performed when Macro type of rule is selected.
    /// </summary>
    private void MacroRuleTypeSelected()
    {
        // Show markup containing controls related to macro settings
        pnlMacroSettings.Visible = true;
    }


    /// <summary>
    /// Initializes macro editor for macro rule, loads its default value.
    /// </summary>
    private void InitMacroSettings()
    {
        if ((Rule != null) && !RequestHelper.IsPostBack())
        {
            macroEditor.Text = RuleHelper.GetMacroConditionFromRule(Rule);
        }
    }


    /// <summary>
    /// Stores Macro condition to main Edit form.
    /// </summary>
    private void StoreMacroData()
    {
        // For macro rule don't store validity
        ClearStoredValidityData();

        // Only activity rules can be recurring and have max points
        editForm.Data["RuleIsRecurring"] = false;
        editForm.Data["RuleMaxPoints"] = null;

        // Store null value as rule parameter
        editForm.Data["RuleParameter"] = null;

        // Store xml with macro condition
        editForm.Data["RuleCondition"] = RuleHelper.GetMacroCondition(macroEditor.Text);
    }


    /// <summary>
    /// Validates fields related to Macro rule type.
    /// </summary>
    /// <returns>True, is validation check was successful, false otherwise</returns>
    private bool ValidateMacroData()
    {
        if (string.IsNullOrEmpty(macroEditor.Text))
        {
            editForm.AddError(GetString("om.score.macroisempty"));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Check whether all macros in macro rule are optimized.
    /// Shows warning when not.
    /// </summary>
    private void CheckMacros()
    {
        var macroCondition = RuleHelper.GetMacroConditionFromRule(Rule);
        if (string.IsNullOrEmpty(macroCondition))
        {
            return;
        }

        var macroTree = CachedMacroRuleTrees.GetParsedTree(macroCondition);
        if ((macroTree == null) || !MacroRuleTreeAnalyzer.CanTreeBeTranslated(macroTree))
        {
            var text = string.Format(ResHelper.GetString("om.macros.macro.slow"), DocumentationHelper.GetDocumentationTopicUrl("om_macro_performance"));
            ShowWarning(text);
        }
    }

    #endregion
}