using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.Search;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;
using CMS.UIControls.Internal;


public partial class CMSModules_AdminControls_Controls_Class_NewClassWizard : CMSUserControl
{
    #region "Constants"

    /// <summary>
    /// Default Icon CSS class for coupled document types.
    /// </summary>
    private const string DEFAULT_COUPLED_CLASS_ICON = "icon-doc-o";

    /// <summary>
    /// Default icon CSS class for container (doesn't contain any fields) document types.
    /// </summary>
    private const string DEFAULT_CLASS_ICON = "icon-folder-o";

    /// <summary>
    /// Column name suffix for LastModified column
    /// </summary>
    private const string LAST_MODIFIED_SUFFIX = "LastModified";

    /// <summary>
    /// Column name suffix for Guid column
    /// </summary>
    private const string GUID_SUFFIX = "Guid";

    private const int STEP_GENERAL = 0;
    private const int STEP_FEATURES = 1;
    private const int STEP_DATABASE_INFORMATION = 2;
    private const int STEP_FIELDS = 3;
    private const int STEP_PARENT_TYPES = 4;
    private const int STEP_SITES = 5;
    private const int STEP_FINAL_OVERVIEW = 6;

    #endregion


    #region "Private fields"

    private DataClassInfo mDataClassInfo;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Edited DataClassInfo
    /// </summary>
    private DataClassInfo DataClassInfo
    {
        get
        {
            if (mDataClassInfo == null)
            {
                mDataClassInfo = DataClassInfoProvider.GetDataClassInfo(ClassName);
                if (mDataClassInfo == null)
                {
                    mDataClassInfo = DataClassInfo.New(GetObjectType());
                    mDataClassInfo.ClassFormDefinition = FormInfo.GetEmptyFormDocument().OuterXml;
                }
            }
            return mDataClassInfo;
        }
    }


    /// <summary>
    /// Name of the new created class.
    /// </summary>
    private string ClassName
    {
        get
        {
            object obj = ViewState["ClassName"];
            return (obj == null) ? string.Empty : (string)obj;
        }
        set
        {
            ViewState["ClassName"] = value;
        }
    }


    /// <summary>
    /// Indicates that currently created document type is container.
    /// </summary>
    private bool IsContainer
    {
        get
        {
            object obj = ViewState["IsContainer"];
            return (obj != null) && (bool)obj;
        }

        set
        {
            ViewState["IsContainer"] = value;
        }
    }


    /// <summary>
    /// Returns whether new fields are created as system fields.
    /// </summary>
    private bool AllowSystemFields
    {
        get
        {
            return SystemContext.DevelopmentMode && (Mode != NewClassWizardModeEnum.DocumentType);
        }
    }


    /// <summary>
    /// Next step button control.
    /// </summary>
    private Control NextStepButton
    {
        get
        {
            return wzdNewDocType.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton");
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Current theme applied.
    /// </summary>
    public string Theme
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether wizard sets the inner field editor to development mode - can edit system fields.
    /// </summary>
    public bool SystemDevelopmentMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the wizard mode - what item is created.
    /// </summary>
    public NewClassWizardModeEnum Mode
    {
        get;
        set;
    } = NewClassWizardModeEnum.DocumentType;


    /// <summary>
    /// Description resource string used in sixth step.
    /// </summary>
    public string Step6Description
    {
        get;
        set;
    } = "documenttype_new_step6.description";


    /// <summary>
    /// Gets or sets the module where new class belongs to.
    /// </summary>
    public int ModuleID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set controls of the first step
        if (!RequestHelper.IsPostBack())
        {
            wzdStepGeneral.EnableViewState = true;
            wzdStepDatabaseInformation.EnableViewState = false;
            wzdStepFields.EnableViewState = false;
            wzdStepParentTypes.EnableViewState = false;
            wzdStepSites.EnableViewState = false;
            wzdStepFinalOverview.EnableViewState = false;

            switch (Mode)
            {
                // If the wizard is running as new document type wizard     
                case NewClassWizardModeEnum.DocumentType:
                    {
                        lblDisplayName.Text = GetString("DocumentType_New.DisplayName");
                        lblFullCodeName.Text = GetString("DocumentType_New.FullCodeName");
                        lblNamespaceName.Text = GetString("DocumentType_New.NamespaceName");
                        lblCodeName.Text = GetString("DocumentType_New.CodeName");

                        // Set validators' error messages
                        rfvDisplayName.ErrorMessage = GetString("DocumentType_New.ErrorEmptyDisplayName");
                        rfvCodeName.ErrorMessage = GetString("DocumentType_New.ErrorEmptyCodeName");
                        rfvNamespaceName.ErrorMessage = GetString("DocumentType_New.ErrorEmptyNamespaceName");
                        revNameSpaceName.ErrorMessage = GetString("DocumentType_New.general.NamespaceNameIdentifier");
                        revCodeName.ErrorMessage = GetString("DocumentType_New.general.CodeNameIdentifier");

                        txtNamespaceName.Text = "custom";

                        ucHeader.Description = GetString("DocumentType_New_Step1.Description");
                    }
                    break;

                // If the wizard is running as new data class wizard    
                case NewClassWizardModeEnum.Class:
                    {
                        lblDisplayName.Text = GetString("sysdev.class_new.DisplayName");
                        lblFullCodeName.Text = GetString("sysdev.class_new.FullCodeName");
                        lblNamespaceName.Text = GetString("DocumentType_New.NamespaceName");
                        lblCodeName.Text = GetString("sysdev.class_new.CodeName");

                        // Set validators' error messages
                        rfvDisplayName.ErrorMessage = GetString("sysdev.class_new.ErrorEmptyDisplayName");
                        rfvCodeName.ErrorMessage = GetString("sysdev.class_new.ErrorEmptyCodeName");
                        rfvNamespaceName.ErrorMessage = GetString("sysdev.class_new.ErrorEmptyNamespaceName");
                        revNameSpaceName.ErrorMessage = GetString("sysdev.class_new.general.NamespaceNameIdentifier");
                        revCodeName.ErrorMessage = GetString("sysdev.class_new.general.CodeNameIdentifier");

                        // Get current module name
                        string moduleName = ProviderHelper.GetCodeName(ResourceInfo.OBJECT_TYPE, ModuleID);

                        txtNamespaceName.Text = moduleName.Contains(".") ? moduleName.Substring(0, moduleName.IndexOf('.')) : moduleName;
                        ucHeader.Description = GetString("sysdev.class_new_Step1.Description");
                    }
                    break;

                // If the wizard is running as new custom table wizard
                case NewClassWizardModeEnum.CustomTable:
                    {
                        lblDisplayName.Text = GetString("customtable.newwizzard.DisplayName");
                        lblFullCodeName.Text = GetString("customtable.newwizzard.FullCodeName");
                        lblNamespaceName.Text = GetString("customtable.newwizzard.NamespaceName");
                        lblCodeName.Text = GetString("customtable.newwizzard.CodeName");

                        // Set validators' error messages
                        rfvDisplayName.ErrorMessage = GetString("customtable.newwizzard.ErrorEmptyDisplayName");
                        rfvCodeName.ErrorMessage = GetString("customtable.newwizzard.ErrorEmptyCodeName");
                        rfvNamespaceName.ErrorMessage = GetString("customtable.newwizzard.ErrorEmptyNamespaceName");
                        revNameSpaceName.ErrorMessage = GetString("customtable.newwizzard.general.NamespaceNameIdentifier");
                        revCodeName.ErrorMessage = GetString("customtable.newwizzard.general.CodeNameIdentifier");

                        txtNamespaceName.Text = "customtable";
                        ucHeader.Description = GetString("customtable.newwizzard.Step1Description");
                    }
                    break;
            }

            // Set regular expression for identifier validation
            revNameSpaceName.ValidationExpression = ValidationHelper.IdentifierRegExp.ToString();
            revCodeName.ValidationExpression = ValidationHelper.IdentifierRegExp.ToString();

            wzdStepGeneral.Title = GetString("general.general");
        }
        else
        {
            // Disable regular expression validations in next steps
            revNameSpaceName.Enabled = false;
            revCodeName.Enabled = false;
        }

        // Set edited object
        EditedObject = DataClassInfo;

        // Set FieldEditor's properties
        FieldEditor.ClassName = ClassName;
        FieldEditor.Mode = FieldEditorModeEnum.ClassFormDefinition;

        // Restrict controls if custom tables
        if (Mode == NewClassWizardModeEnum.CustomTable)
        {
            FieldEditor.Mode = FieldEditorModeEnum.CustomTable;
        }

        // Set field editor's development mode
        FieldEditor.DevelopmentMode = SystemDevelopmentMode;
        FieldEditor.IsWizard = true;

        if (!SystemDevelopmentMode && (Mode == NewClassWizardModeEnum.DocumentType))
        {
            FieldEditor.EnableSystemFields = true;
        }

        wzdNewDocType.ActiveStepChanged += wzdNewDocType_ActiveStepChanged;

        // Set buttons' text                
        wzdNewDocType.StartNextButtonText = GetString("general.next");
        wzdNewDocType.StepNextButtonText = wzdNewDocType.StartNextButtonText;
        wzdNewDocType.FinishCompleteButtonText = GetString("general.Finish");

        // Do not provide suffix for breadcrumbs
        UIHelper.SetBreadcrumbsSuffix("");
        ScriptHelper.RegisterTooltip(Page);

        // Register selection changed postback for the inherits class selector
        selInherits.DropDownSingleSelect.AutoPostBack = true;
        ControlsHelper.RegisterPostbackControl(selInherits.UniSelector);
    }


    /// <summary>
    /// Gets the object type of the created class
    /// </summary>
    private string GetObjectType()
    {
        string objectType = DataClassInfo.OBJECT_TYPE;

        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                objectType = DocumentTypeInfo.OBJECT_TYPE_DOCUMENTTYPE;
                break;

            case NewClassWizardModeEnum.CustomTable:
                objectType = CustomTableInfo.OBJECT_TYPE_CUSTOMTABLE;
                break;
        }

        return objectType;
    }


    void wzdNewDocType_ActiveStepChanged(object sender, EventArgs e)
    {
        // Field editor needs to be visible to be able to reload controls properly
        if (wzdNewDocType.ActiveStep == wzdStepFields)
        {
            FieldEditor.Reload(null);
        }
    }


    protected void Page_PreRender()
    {
        var stepIndex = wzdNewDocType.ActiveStepIndex;
        var pager = new NewClassWizardPageNumbersProvider(Mode);

        ucHeader.Header = GetCurrentStepTitle(stepIndex);
        ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), pager.GetStepNumber(stepIndex), pager.GetStepsCount());
    }


    protected void radExistingTable_CheckedChanged(object sender, EventArgs e)
    {
        lblTableNameError.Visible = false;

        if (radNewTable.Checked)
        {
            txtPKName.Text = "ItemID";

            txtTableName.Visible = true;
            drpExistingTables.Visible = false;

            chkItemGUID.Checked = true;
            chkItemOrder.Checked = true;
            chkItemCreatedBy.Checked = true;
            chkItemCreatedWhen.Checked = true;
            chkItemModifiedBy.Checked = true;
            chkItemModifiedWhen.Checked = true;
        }
        else
        {
            txtPKName.Text = ResHelper.GetString("General.Automatic");

            txtTableName.Visible = false;
            drpExistingTables.Visible = true;

            LoadAvailableTables();

            chkItemGUID.Checked = false;
            chkItemOrder.Checked = false;
            chkItemCreatedBy.Checked = false;
            chkItemCreatedWhen.Checked = false;
            chkItemModifiedBy.Checked = false;
            chkItemModifiedWhen.Checked = false;
        }
    }


    #endregion


    #region "Step processing"

    /// <summary>
    /// 'Next' button is clicked.
    /// </summary>
    protected void wzdNewDocType_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        // Do not create new version in the wizard, version is created explicitly at the end
        using (new CMSActionContext { CreateVersion = false, LogEvents = false })
        {
            switch (e.CurrentStepIndex)
            {
                case STEP_GENERAL:
                    ProcessGeneralInfoAndNavigateToFeaturesStep(e);
                    break;

                case STEP_FEATURES:
                    ProcessFeaturesAndNavigateToDatabaseInformationStep(e);
                    break;

                case STEP_DATABASE_INFORMATION:
                    ProcessDatabaseInformationStepAndNavigateFieldsStep(e);
                    break;

                case STEP_FIELDS:
                    ProcessFieldsAndNavigateToAdditionalSettingsStep(e);
                    break;

                case STEP_PARENT_TYPES:
                    ProcessParentTypesAndNavigateToSites(e);
                    break;

                case STEP_SITES:
                    ProcessSitesAndNavigateToFinalOverview(e);
                    break;
            }
        }
    }


    protected void btnContinueWithoutCustomFields_Click(object sender, EventArgs e)
    {
        using (new CMSActionContext { CreateVersion = false, LogEvents = false })
        {
            DataClassInfo.ClassIsCoupledClass = false;
            DataClassInfoProvider.SetDataClassInfo(DataClassInfo);

            IsContainer = true;

            var simulatedCurrentStep = GetPreviousStepOf(STEP_PARENT_TYPES);
            DisablePreviousStepsViewStates(simulatedCurrentStep);
            EnableNextStepViewState(simulatedCurrentStep);

            PrepareParentTypesStep();
            wzdNewDocType.ActiveStepIndex = STEP_PARENT_TYPES;

            HideSkipFieldsButton();

            DataClassInfo.SetValue("ClassIconClass", DEFAULT_CLASS_ICON);
        }
    }


    /// <summary>
    /// Processes the step 1 of the wizard
    /// </summary>
    private void ProcessGeneralInfoAndNavigateToFeaturesStep(WizardNavigationEventArgs e)
    {
        // Actions after next button click

        // Validate checkboxes first
        string errorMessage = null;

        var codeName = txtCodeName.Text.Trim();
        var namespaceName = txtNamespaceName.Text.Trim();

        // Display proper error message based on development mode wizard setting
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                errorMessage = new Validator().NotEmpty(txtDisplayName.Text.Trim(), GetString("DocumentType_New.ErrorEmptyDisplayName")).
                    NotEmpty(codeName, GetString("DocumentType_New.ErrorEmptyCodeName")).
                    NotEmpty(namespaceName, GetString("DocumentType_New.ErrorEmptyNamespaceName")).
                    IsCodeName(codeName, GetString("DocumentType_New.general.CodeNameIdentifier")).
                    IsIdentifier(namespaceName, GetString("DocumentType_New.general.NamespaceNameIdentifier")).Result;
                break;

            case NewClassWizardModeEnum.Class:
                errorMessage = new Validator().NotEmpty(txtDisplayName.Text.Trim(), GetString("sysdev.class_new.ErrorEmptyDisplayName")).
                    NotEmpty(codeName, GetString("sysdev.class_new.ErrorEmptyCodeName")).
                    NotEmpty(namespaceName, GetString("sysdev.class_new.ErrorEmptyNamespaceName")).
                    IsCodeName(codeName, GetString("sysdev.class_new.general.CodeNameIdentifier")).
                    IsIdentifier(namespaceName, GetString("sysdev.class_new.general.NamespaceNameIdentifier")).Result;
                break;

            case NewClassWizardModeEnum.CustomTable:
                errorMessage = new Validator().NotEmpty(txtDisplayName.Text.Trim(), GetString("customtable.newwizzard.ErrorEmptyDisplayName")).
                    NotEmpty(codeName, GetString("customtable.newwizzard.ErrorEmptyCodeName")).
                    NotEmpty(namespaceName, GetString("customtable.newwizzard.ErrorEmptyNamespaceName")).
                    IsCodeName(codeName, GetString("customtable.newwizzard.general.CodeNameIdentifier")).
                    IsIdentifier(namespaceName, GetString("customtable.newwizzard.general.NamespaceNameIdentifier")).Result;
                break;
        }


        if (String.IsNullOrEmpty(errorMessage))
        {
            string className = namespaceName + "." + codeName;
            if (DataClassInfoProvider.GetDataClassInfo(className) != null)
            {
                errorMessage = GetString("sysdev.class_edit_gen.codenameunique");
            }
            else
            {
                // Set new class info
                DataClassInfo.ClassDisplayName = txtDisplayName.Text.Trim();
                DataClassInfo.ClassName = className;

                // Use class namespace in code generators
                SetCodeGenerationSettingsNamespace(namespaceName);

                // Set class type according development mode setting
                switch (Mode)
                {
                    case NewClassWizardModeEnum.DocumentType:
                        DataClassInfo.ClassIsDocumentType = true;
                        DataClassInfo.ClassUsePublishFromTo = true;
                        break;

                    case NewClassWizardModeEnum.Class:
                        DataClassInfo.ClassShowAsSystemTable = false;
                        DataClassInfo.ClassShowTemplateSelection = false;
                        DataClassInfo.ClassIsDocumentType = false;
                        DataClassInfo.ClassIsProduct = false;
                        DataClassInfo.ClassIsMenuItemType = false;
                        DataClassInfo.ClassUsesVersioning = false;
                        DataClassInfo.ClassUsePublishFromTo = false;
                        DataClassInfo.ClassResourceID = ModuleID;
                        break;

                    case NewClassWizardModeEnum.CustomTable:
                        DataClassInfo.ClassShowAsSystemTable = false;
                        DataClassInfo.ClassShowTemplateSelection = false;
                        DataClassInfo.ClassIsDocumentType = false;
                        DataClassInfo.ClassIsProduct = false;
                        DataClassInfo.ClassIsMenuItemType = false;
                        DataClassInfo.ClassUsesVersioning = false;
                        DataClassInfo.ClassUsePublishFromTo = false;
                        // Sets custom table
                        DataClassInfo.ClassIsCustomTable = true;
                        break;
                }

                var errorMsg = String.Empty;

                try
                {
                    using (var tr = new CMSTransactionScope())
                    {
                        // Insert new class into DB
                        using (new CMSActionContext { LogEvents = true })
                        {
                            DataClassInfoProvider.SetDataClassInfo(DataClassInfo);
                        }

                        // Set permissions and queries
                        switch (Mode)
                        {
                            case NewClassWizardModeEnum.DocumentType:
                                // Ensure default permissions
                                PermissionNameInfoProvider.CreateDefaultClassPermissions(DataClassInfo.ClassID);
                                break;

                            case NewClassWizardModeEnum.Class:
                                break;

                            case NewClassWizardModeEnum.CustomTable:
                                // Ensure default custom table permissions
                                PermissionNameInfoProvider.CreateDefaultCustomTablePermissions(DataClassInfo.ClassID);
                                break;
                        }

                        tr.Commit();
                    }
                }
                catch (Exception ex)
                {
                    // No movement to the next step
                    e.Cancel = true;

                    // Class with the same class name already exists
                    pnlMessages1.Visible = true;

                    errorMsg = ex.Message;

                    pnlMessages1.ShowError(errorMsg);

                    Service.Resolve<IEventLogService>().LogException("NewClassWizard", "CREATE", ex);
                }

                // Prepare next step (2)  
                if (errorMsg == "")
                {
                    // Disable previous steps' view states
                    DisablePreviousStepsViewStates(e.CurrentStepIndex);

                    // Enable next step's view state
                    EnableNextStepViewState(e.CurrentStepIndex);

                    // Save parameters to viewstate to use in the next steps
                    ClassName = DataClassInfo.ClassName;

                    // Prefill textboxes in the next step with default values
                    txtTableName.Text = namespaceName + "_" + codeName;
                    txtPKName.Text = TextHelper.FirstLetterToUpper(codeName + "ID");

                    wzdStepDatabaseInformation.Title = GetString("documenttype_new_step_database_information.Title");

                    switch (Mode)
                    {
                        case NewClassWizardModeEnum.DocumentType:
                            {
                                wzdStepFeatures.Title = GetString("documenttype_new_step_features.title");
                                ucHeader.Description = GetString("documenttype_new_step_features.description");

                                lblPageBuilder.Text = GetString("documenttype_new_step_features.pagebuilder.headline");
                                ScriptHelper.AppendTooltip(iconHelpPageBuilder, GetString("documenttype_new_step_features.pagebuilder.tooltip"), null);

                                lblNavigationItem.Text = GetString("documenttype_new_step_features.navigationitem.headline");
                                ScriptHelper.AppendTooltip(iconHelpNavigationItem, GetString("documenttype_new_step_features.navigationitem.tooltip"), null);

                                lblUrl.Text = GetString("documenttype_new_step_features.url.headline");
                                ScriptHelper.AppendTooltip(iconHelpUrl, GetString("documenttype_new_step_features.url.tooltip"), null);

                                lblMetadata.Text = GetString("documenttype_new_step_features.metadata.headline");
                                ScriptHelper.AppendTooltip(iconHelpMetadata, GetString("documenttype_new_step_features.metadata.tooltip"), null);

                                ScriptHelper.RegisterModule(this, "CMS/PageTypeFeatures", new { highlightSelection = true });
                            }
                            break;
                        case NewClassWizardModeEnum.Class:
                            {
                                // Standard class
                                lblFullCodeName.ResourceString = "sysdev.class_new.fullcodename";
                                lblPKName.Text = GetString("sysdev.class_new.PrimaryKeyName");
                                lblTableName.Text = GetString("sysdev.class_new.TableName");
                                lblIsMNTable.Text = GetString("sysdev.class_new.MNTable");

                                lblClassGuid.Text = String.Format(GetString("sysdev.class_new.lblClassGuid"), TextHelper.FirstLetterToUpper(codeName));
                                lblClassLastModified.Text = String.Format(GetString("sysdev.class_new.lblClassLastModified"), TextHelper.FirstLetterToUpper(codeName));

                                plcMNClassOptions.Visible = true;

                                ucHeader.Description = GetString("sysdev.class_new_Step2.Description");

                                SkipFeaturesStep();
                            }
                            break;

                        case NewClassWizardModeEnum.CustomTable:
                            {
                                // Custom table
                                lblFullCodeName.ResourceString = "customtable.newwizzard.FullCodeName";
                                lblPKName.Text = GetString("customtable.newwizzard.PrimaryKeyName");
                                lblTableName.Text = GetString("customtable.newwizzard.TableName");

                                radNewTable.Text = GetString("customtable.newwizard.newtable");
                                radExistingTable.Text = GetString("customtable.newwizard.existingtable");

                                plcExisting.Visible = true;

                                // Custom tables have always ItemID as primary key
                                txtPKName.Text = "ItemID";

                                // Primary key name can't be edited
                                txtPKName.Enabled = false;

                                // Show custom tables columns options
                                plcCustomTablesOptions.Visible = true;

                                lblItemGUID.Text = GetString("customtable.newwizzard.lblItemGUID");
                                lblItemCreatedBy.Text = GetString("customtable.newwizzard.lblItemCreatedBy");
                                lblItemCreatedWhen.Text = GetString("customtable.newwizzard.lblItemCreatedWhen");
                                lblItemModifiedBy.Text = GetString("customtable.newwizzard.lblItemModifiedBy");
                                lblItemModifiedWhen.Text = GetString("customtable.newwizzard.lblItemModifiedWhen");
                                lblItemOrder.Text = GetString("customtable.newwizzard.lblItemOrder");

                                ucHeader.Description = GetString("customtable.newwizzard.Step2Description");

                                SkipFeaturesStep();
                            }
                            break;
                    }
                }
            }
        }

        if (!String.IsNullOrEmpty(errorMessage))
        {
            // No movement to the next step
            e.Cancel = true;

            // Textboxes are not filled correctly
            pnlMessages1.Visible = true;
            pnlMessages1.ShowError(errorMessage);
        }
    }

    private void SkipFeaturesStep()
    {
        var simulatedCurrentStep = GetPreviousStepOf(STEP_DATABASE_INFORMATION);
        DisablePreviousStepsViewStates(simulatedCurrentStep);
        EnableNextStepViewState(simulatedCurrentStep);

        wzdNewDocType.ActiveStepIndex = STEP_DATABASE_INFORMATION;
    }


    /// <summary>
    /// Sets given namespace to ClassCodeGenerationSettings of underlying data class.
    /// </summary>
    /// <remarks>
    /// Namespace can be set only when none other settings has been set.
    /// </remarks>
    private void SetCodeGenerationSettingsNamespace(string namespaceName)
    {
        if (DataClassInfo == null || !string.IsNullOrEmpty(DataClassInfo.ClassCodeGenerationSettings))
        {
            return;
        }

        // Column preselection for code generator works only for first get in ClassCodeGenerationSettingsInfo property.
        // Therefore all precedent setting assignments should be done via ClassCodeGenerationSettings property.
        var settings = new ClassCodeGenerationSettings
        {
            NameSpace = namespaceName
        };
        DataClassInfo.ClassCodeGenerationSettings = settings.ToString();
    }


    private void ProcessFeaturesAndNavigateToDatabaseInformationStep(WizardNavigationEventArgs e)
    {
        DataClassInfo.ClassUsesPageBuilder = chkPageBuilderFeature.Checked;
        DataClassInfo.ClassIsNavigationItem = chkNavigationItem.Checked;
        DataClassInfo.ClassHasURL = chkUrl.Checked;
        DataClassInfo.ClassHasMetadata = chkMetadata.Checked;

        //Prepare next step
        // Disable previous steps' view states
        DisablePreviousStepsViewStates(e.CurrentStepIndex);

        // Enable next step's view state
        EnableNextStepViewState(e.CurrentStepIndex);

        // Prepare next step by mode setting
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                {
                    // Document type
                    lblFullCodeName.ResourceString = "DocumentType_New.FullCodeName";
                    lblPKName.Text = GetString("DocumentType_New.PrimaryKeyName");
                    lblTableName.Text = GetString("DocumentType_New.TableName");

                    var btnContinueWithoutCustomFields = wzdNewDocType.FindControl("StepNavigationTemplateContainerID").FindControl("btnContinueWithoutCustomFields");
                    btnContinueWithoutCustomFields.Visible = true;
                    ucHeader.Description = GetString("documenttype_new_step_database_information.Description");

                    // Setup the inheritance selector
                    plcDocTypeOptions.Visible = true;

                    var where = new WhereCondition()
                        .WhereTrue("ClassIsCoupledClass")
                        .WhereNotEquals("ClassID", DataClassInfo.ClassID)
                        .Where(new WhereCondition()
                            .WhereNull("ClassInheritsFromClassID")
                            .Or().WhereNotEquals("ClassInheritsFromClassID", DataClassInfo.ClassID)
                        );

                    selInherits.WhereCondition = where.ToString(true);
                    selInherits.ReloadData();

                    NextStepButton.Visible = true;
                }
                break;
        }
    }


    /// <summary>
    /// Processes the step 2 of the wizard
    /// </summary>
    private void ProcessDatabaseInformationStepAndNavigateFieldsStep(WizardNavigationEventArgs e)
    {
        if (DataClassInfo != null)
        {
            var tm = new TableManager(null);

            using (var tr = new CMSTransactionScope())
            {
                bool fromExisting = (Mode == NewClassWizardModeEnum.CustomTable) && radExistingTable.Checked;

                string tableName = (fromExisting) ? drpExistingTables.SelectedValue : txtTableName.Text.Trim();

                var tableNameValidator = new Validator()
                    .NotEmpty(tableName, GetString("DocumentType_New.ErrorEmptyTableName"))
                    .IsIdentifier(tableName, GetString("class.ErrorIdentifier"));
                if (fromExisting)
                {
                    // Custom table from existing table - given table should exist in database
                    tableNameValidator = tableNameValidator.MatchesCondition(tableName, x => tm.TableExists(x), GetString("customtable.newwizard.tablenotexists"));
                }
                else
                {
                    // Given table should not exist in database
                    tableNameValidator = tableNameValidator.MatchesCondition(tableName, x => !tm.TableExists(x), GetString("sysdev.class_edit_gen.tablenameunique"));
                }

                string tableNameError = tableNameValidator.Result;

                string trimmedPrimaryKeyName = txtPKName.Text.Trim();
                string primaryKeyNameValidationResult = null;

                // Check whether primary key name is in identifier format for new table. 
                // For existing table is validation executed against the existing PK in database.
                if (!fromExisting)
                {
                    primaryKeyNameValidationResult = new Validator()
                        .NotEmpty(trimmedPrimaryKeyName, GetString("DocumentType_New.ErrorEmptyPKName"))
                        .IsIdentifier(trimmedPrimaryKeyName, GetString("class.pkerroridentifier"))
                        .Result;
                }

                // Check whether the column for page types is not in collision with columns used in document view
                bool columnExists = (Mode == NewClassWizardModeEnum.DocumentType) && DocumentHelper.ColumnExistsInDocumentView(trimmedPrimaryKeyName);

                // Textboxes are filled correctly or the values will be checked automatically from existing table
                if ((String.IsNullOrEmpty(tableNameError)) && (String.IsNullOrEmpty(primaryKeyNameValidationResult)) && (!columnExists))
                {
                    try
                    {
                        if (fromExisting)
                        {
                            // Check primary key
                            List<string> primaryKeys = tm.GetPrimaryKeyColumns(tableName);
                            if ((primaryKeys == null) || (primaryKeys.Count != 1))
                            {
                                e.Cancel = true;

                                ShowError(GetString("customtable.newwizard.musthaveprimarykey"));
                            }
                            else if (!IsIdentityColumn(tableName, primaryKeys.First()))
                            {
                                e.Cancel = true;
                                ShowError(GetString("customtable.newwizard.mustbeidentitypk"));
                            }
                            else if (!ValidationHelper.IsIdentifier(primaryKeys[0]))
                            {
                                e.Cancel = true;
                                ShowError(GetString("customtable.newwizard.pkerroridentifier"));
                            }
                        }
                        else if (Mode == NewClassWizardModeEnum.Class)
                        {
                            // Standard class in development mode
                            tm.CreateTable(tableName, trimmedPrimaryKeyName, !chbIsMNTable.Checked);
                        }
                        else
                        {
                            tm.CreateTable(tableName, trimmedPrimaryKeyName);
                        }
                    }
                    catch (Exception ex)
                    {
                        // No movement to the next step
                        e.Cancel = true;

                        // Show error message if something caused unhandled exception
                        ShowError(ex.Message);
                    }

                    if ((pnlMessages2.ErrorLabel.Text == "") && !e.Cancel)
                    {
                        FormInfo fi;
                        if (fromExisting)
                        {
                            fi = new FormInfo();
                            try
                            {
                                fi.LoadFromDataStructure(tableName, tm, AllowSystemFields);
                            }
                            catch (Exception ex)
                            {
                                e.Cancel = true;

                                // Show error message if something caused unhandled exception
                                ShowError(ex.Message);
                                return;
                            }
                        }
                        else
                        {
                            // Create empty form info
                            fi = CreateEmptyFormInfo();
                        }

                        DataClassInfo.ClassTableName = tableName;
                        DataClassInfo.ClassFormDefinition = fi.GetXmlDefinition();
                        DataClassInfo.ClassIsCoupledClass = true;

                        DataClassInfo.ClassInheritsFromClassID = ValidationHelper.GetInteger(selInherits.Value, 0);

                        // Update class in DB
                        DataClassInfoProvider.SetDataClassInfo(DataClassInfo);

                        UpdateInheritedClass(DataClassInfo);

                        if (Mode == NewClassWizardModeEnum.CustomTable)
                        {
                            try
                            {
                                InitCustomTable(DataClassInfo, fi);
                            }
                            catch (Exception ex)
                            {
                                // Do not move to next step
                                e.Cancel = true;

                                Service.Resolve<IEventLogService>().LogException("NewClassWizard", "CREATE", ex);

                                string message;
                                var missingSqlType = ex as MissingSQLTypeException;
                                if (missingSqlType != null)
                                {
                                    if (DataTypeManager.IsType<byte[]>(TypeEnum.SQL, missingSqlType.RecommendedType))
                                    {
                                        message = String.Format(GetString("customtable.sqltypenotsupportedwithoutreplacement"), missingSqlType.UnsupportedType, missingSqlType.ColumnName);
                                    }
                                    else
                                    {
                                        message = String.Format(GetString("customtable.sqltypenotsupported"), missingSqlType.UnsupportedType, missingSqlType.ColumnName, missingSqlType.RecommendedType);
                                    }
                                }
                                else
                                {
                                    message = ex.Message;
                                }

                                pnlMessages2.ShowError(message);
                                pnlMessages2.Visible = true;
                            }
                        }
                        else if (Mode == NewClassWizardModeEnum.Class)
                        {
                            try
                            {
                                InitClass(DataClassInfo, fi);
                            }
                            catch (Exception ex)
                            {
                                // Do not move to next step
                                e.Cancel = true;

                                Service.Resolve<IEventLogService>().LogException("NewClassWizard", "CREATE", ex);

                                pnlMessages2.ShowError(ex.Message);
                                pnlMessages2.Visible = true;
                            }
                        }

                        if (!e.Cancel)
                        {
                            HideSkipFieldsButton();

                            DisablePreviousStepsViewStates(e.CurrentStepIndex);
                            EnableNextStepViewState(e.CurrentStepIndex);

                            // Set field editor class name
                            FieldEditor.ClassName = ClassName;

                            // Fill field editor in the next step
                            FieldEditor.Reload(null);

                            wzdStepFields.Title = GetString("general.fields");

                            // Set new step header based on the development mode setting
                            switch (Mode)
                            {
                                case NewClassWizardModeEnum.DocumentType:
                                    ucHeader.Description = GetString("DocumentType_New_Step3.Description");
                                    break;

                                case NewClassWizardModeEnum.Class:
                                    ucHeader.Description = GetString("sysdev.class_new_Step3.Description");
                                    break;

                                case NewClassWizardModeEnum.CustomTable:
                                    ucHeader.Description = GetString("customtable.newwizzard.Step3Description");
                                    break;
                            }
                        }
                    }
                }
                // Some textboxes are not filled correctly
                else
                {
                    // Prepare current step (2)

                    // No movement to the next step
                    e.Cancel = true;

                    // Show errors
                    if (!String.IsNullOrEmpty(tableNameError))
                    {
                        lblTableNameError.Text = tableNameError;
                        lblTableNameError.Visible = true;
                    }
                    else
                    {
                        lblTableNameError.Visible = false;
                    }

                    if (!String.IsNullOrEmpty(primaryKeyNameValidationResult))
                    {
                        lblPKNameError.Visible = true;
                        lblPKNameError.Text = primaryKeyNameValidationResult;
                    }
                    else
                    {
                        lblPKNameError.Visible = false;
                    }

                    if (columnExists)
                    {
                        pnlMessages2.ShowError(GetString("documenttype_new_step_database_information.errorcolumnexists"));
                        pnlMessages2.Visible = true;
                    }


                    // Reset the header
                    switch (Mode)
                    {
                        case NewClassWizardModeEnum.DocumentType:
                            ucHeader.Description = GetString("documenttype_new_step_database_information.Description");
                            break;

                        case NewClassWizardModeEnum.Class:
                            ucHeader.Description = GetString("sysdev.class_new_Step2.Description");
                            break;

                        case NewClassWizardModeEnum.CustomTable:
                            ucHeader.Description = GetString("customtable.newwizzard.Step2Description");
                            break;
                    }
                }

                if (Mode == NewClassWizardModeEnum.DocumentType)
                {
                    DataClassInfo.SetValue("ClassIconClass", DEFAULT_COUPLED_CLASS_ICON);
                }

                if (!e.Cancel)
                {
                    tr.Commit();
                }
            }
        }
    }


    private void HideSkipFieldsButton()
    {
        var btnContinueWithoutCustomFields = wzdNewDocType.FindControl("StepNavigationTemplateContainerID").FindControl("btnContinueWithoutCustomFields");
        btnContinueWithoutCustomFields.Visible = false;
    }


    /// <summary>
    /// Updates the inherited class fields if the class is inherited
    /// </summary>
    /// <param name="dci">DataClassInfo to update</param>
    private static void UpdateInheritedClass(DataClassInfo dci)
    {
        // Ensure inherited fields
        if (dci.ClassInheritsFromClassID > 0)
        {
            var parentCi = DataClassInfoProvider.GetDataClassInfo(dci.ClassInheritsFromClassID);
            if (parentCi != null)
            {
                FormHelper.UpdateInheritedClass(parentCi, dci);
            }
        }
    }


    /// <summary>
    /// Initializes the custom table
    /// </summary>
    /// <param name="dci">DataClassInfo of the custom table</param>
    /// <param name="fi">Form info</param>
    private void InitCustomTable(DataClassInfo dci, FormInfo fi)
    {
        // Created by
        if (chkItemCreatedBy.Checked && !fi.FieldExists("ItemCreatedBy"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemCreatedBy";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Created by");
            ffi.DataType = FieldDataType.Integer;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // Created when
        if (chkItemCreatedWhen.Checked && !fi.FieldExists("ItemCreatedWhen"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemCreatedWhen";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Created when");
            ffi.DataType = FieldDataType.DateTime;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.Precision = DataTypeManager.GetDataType(TypeEnum.Field, FieldDataType.DateTime).DefaultPrecision;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // Modified by
        if (chkItemModifiedBy.Checked && !fi.FieldExists("ItemModifiedBy"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemModifiedBy";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Modified by");
            ffi.DataType = FieldDataType.Integer;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // Modified when
        if (chkItemModifiedWhen.Checked && !fi.FieldExists("ItemModifiedWhen"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemModifiedWhen";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Modified when");
            ffi.DataType = FieldDataType.DateTime;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.Precision = DataTypeManager.GetDataType(TypeEnum.Field, FieldDataType.DateTime).DefaultPrecision;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // Item order
        if (chkItemOrder.Checked && !fi.FieldExists("ItemOrder"))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = "ItemOrder";
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Order");
            ffi.DataType = FieldDataType.Integer;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.AllowEmpty = true;

            fi.AddFormItem(ffi);
        }

        // GUID
        if (chkItemGUID.Checked && !fi.FieldExists("ItemGUID"))
        {
            fi.AddFormItem(CreateGuidField("ItemGUID"));
        }

        // Update definition
        dci.ClassFormDefinition = fi.GetXmlDefinition();

        DataClassInfoProvider.SetDataClassInfo(dci);
    }


    /// <summary>
    /// Initializes class.
    /// </summary>
    /// <param name="dci">DataClassInfo</param>
    /// <param name="fi">Form info</param>
    private void InitClass(DataClassInfo dci, FormInfo fi)
    {
        // Get class code name
        var pkName = txtPKName.Text.Trim();
        var codeName = pkName.Substring(0, pkName.Length - 2);

        // Guid
        if (chkClassGuid.Checked && !fi.FieldExists(codeName + GUID_SUFFIX))
        {
            fi.AddFormItem(CreateGuidField(codeName + GUID_SUFFIX));
        }

        // Last modified
        if (chkClassLastModified.Checked && !fi.FieldExists(codeName + LAST_MODIFIED_SUFFIX))
        {
            FormFieldInfo ffi = new FormFieldInfo();

            // Fill FormInfo object
            ffi.Name = codeName + LAST_MODIFIED_SUFFIX;
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Last modified");
            ffi.DataType = FieldDataType.DateTime;
            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, String.Empty);
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, String.Empty);
            ffi.PrimaryKey = false;
            ffi.System = true;
            ffi.Visible = false;
            ffi.Size = 0;
            ffi.Precision = DataTypeManager.GetDataType(TypeEnum.Field, FieldDataType.DateTime).DefaultPrecision;
            ffi.AllowEmpty = false;

            fi.AddFormItem(ffi);
        }

        // Update definition
        dci.ClassFormDefinition = fi.GetXmlDefinition();

        DataClassInfoProvider.SetDataClassInfo(dci);
    }


    /// <summary>
    /// Creates the GUID field
    /// </summary>
    /// <param name="name">Form field name.</param>
    private static FormFieldInfo CreateGuidField(string name)
    {
        var ffiGuid = new FormFieldInfo();

        // Fill FormInfo object
        ffiGuid.Name = name;
        ffiGuid.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "GUID");
        ffiGuid.DataType = FieldDataType.Guid;
        ffiGuid.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, String.Empty);
        ffiGuid.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, String.Empty);
        ffiGuid.PrimaryKey = false;
        ffiGuid.System = true;
        ffiGuid.Visible = false;
        ffiGuid.Size = 0;
        ffiGuid.AllowEmpty = false;

        return ffiGuid;
    }


    /// <summary>
    /// Creates an empty form info for the new class
    /// </summary>
    private FormInfo CreateEmptyFormInfo()
    {
        // Create empty form definition
        var fi = new FormInfo();

        var ffiPK = new FormFieldInfo();

        // Fill FormInfo object
        ffiPK.Name = txtPKName.Text;
        ffiPK.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, txtPKName.Text);
        ffiPK.DataType = FieldDataType.Integer;
        ffiPK.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
        ffiPK.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, string.Empty);
        ffiPK.PrimaryKey = true;
        ffiPK.System = AllowSystemFields;
        ffiPK.Visible = false;
        ffiPK.AllowEmpty = false;

        // Add field to form definition
        fi.AddFormItem(ffiPK);

        return fi;
    }


    /// <summary>
    /// Processes the step 3 of the wizard
    /// </summary>
    private void ProcessFieldsAndNavigateToAdditionalSettingsStep(WizardNavigationEventArgs e)
    {
        // Check if there is unsaved field
        if (FieldEditor.IsNewItemEdited)
        {
            e.Cancel = true;
            FieldEditor.ShowError(GetString("templatedesigner.alertsavenewitemordeleteitfirst"));
            return;
        }

        // Ensure actual form info
        FormHelper.ClearFormInfos(true);

        // Get and load form definition
        var fi = FormHelper.GetFormInfo(DataClassInfo.ClassName, false);

        // System columns for a document type can be added in the field editor, which do not have separate database representation and should not count
        var includeSystem = Mode != NewClassWizardModeEnum.DocumentType;

        if (fi.GetFields(true, true, includeSystem: includeSystem, includeDummyFields: false).Count() < 2)
        {
            e.Cancel = true;
            FieldEditor.ShowError(GetString("DocumentType_New_Step3.TableMustHaveCustomField"));
        }
        else
        {
            // Different behavior by mode
            switch (Mode)
            {
                case NewClassWizardModeEnum.DocumentType:
                    {
                        var simulatedCurrentStep = GetPreviousStepOf(STEP_PARENT_TYPES);
                        DisablePreviousStepsViewStates(simulatedCurrentStep);
                        EnableNextStepViewState(simulatedCurrentStep);

                        SelectPageNameSource(fi);

                        PrepareParentTypesStep();

                        wzdNewDocType.ActiveStepIndex = STEP_PARENT_TYPES;
                    }
                    break;

                case NewClassWizardModeEnum.Class:
                    {
                        DataClassInfoProvider.SetDataClassInfo(DataClassInfo);

                        var simulatedCurrentStep = GetPreviousStepOf(STEP_FINAL_OVERVIEW);
                        DisablePreviousStepsViewStates(simulatedCurrentStep);
                        EnableNextStepViewState(simulatedCurrentStep);

                        PrepareFinalOverview();
                        wzdNewDocType.ActiveStepIndex = STEP_FINAL_OVERVIEW;
                    }
                    break;

                case NewClassWizardModeEnum.CustomTable:
                    {
                        // Update class in DB
                        DataClassInfoProvider.SetDataClassInfo(DataClassInfo);

                        var simulatedCurrentStep = GetPreviousStepOf(STEP_SITES);
                        DisablePreviousStepsViewStates(simulatedCurrentStep);
                        EnableNextStepViewState(simulatedCurrentStep);

                        PrepareStepSites();
                        wzdNewDocType.ActiveStepIndex = STEP_SITES;
                    }
                    break;
            }
        }
    }


    private int GetPreviousStepOf(int step)
    {
        return step - 1;
    }


    private void SelectPageNameSource(FormInfo fi)
    {
        // Get all fields
        List<FormFieldInfo> ffiFields = fi.GetFields(true, true);

        string selectedFieldName = string.Empty;
        if (ffiFields != null)
        {
            // Add all text fields' names to the list except primary-key field
            foreach (FormFieldInfo ffi in ffiFields)
            {
                if (!ffi.PrimaryKey && !ffi.AllowEmpty && ((ffi.DataType == FieldDataType.Text) || (ffi.DataType == FieldDataType.LongText)))
                {
                    // Select the first text field
                    if (string.IsNullOrEmpty(selectedFieldName))
                    {
                        // Preselect only textbox
                        if (ffi.HasFormFieldControlWithName(FormFieldControlName.TEXTBOX))
                        {
                            selectedFieldName = ffi.Name;
                        }
                    }
                }
            }
        }
        DataClassInfo.ClassNodeNameSource = selectedFieldName;

        DataClassInfoProvider.SetDataClassInfo(DataClassInfo);
    }


    /// <summary>
    /// Processes the step 5 of the wizard
    /// </summary>
    private void ProcessParentTypesAndNavigateToSites(WizardNavigationEventArgs e)
    {
        int childClassID = DataClassInfo.ClassID;

        // Add parent classes
        string selectedClasses = ValidationHelper.GetString(usParentTypes.Value, String.Empty);
        if (!String.IsNullOrEmpty(selectedClasses))
        {
            string[] classes = selectedClasses.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in classes)
            {
                int parentClassID = ValidationHelper.GetInteger(item, 0);
                AllowedChildClassInfo.Provider.Add(parentClassID, childClassID);
            }
        }

        if (Mode == NewClassWizardModeEnum.DocumentType)
        {
            ClassSiteInfo.Provider.Add(childClassID, SiteContext.CurrentSiteID);

            if (DataClassInfo.ClassHasURL)
            {
                SetSearchSettings();
            }

            var simulatedCurrentStep = GetPreviousStepOf(STEP_FINAL_OVERVIEW);
            DisablePreviousStepsViewStates(simulatedCurrentStep);
            EnableNextStepViewState(simulatedCurrentStep);

            PrepareFinalOverview();

            wzdNewDocType.ActiveStepIndex = STEP_FINAL_OVERVIEW;

            // Explicitly log the synchronization with all changes and create version
            using (new CMSActionContext { CreateVersion = true })
            {
                SynchronizationHelper.LogObjectUpdate(DataClassInfo);
            }
        }
        else
        {
            DisablePreviousStepsViewStates(e.CurrentStepIndex);
            EnableNextStepViewState(e.CurrentStepIndex);
            PrepareStepSites();
        }
    }


    private void SetSearchSettings()
    {
        SetPageTypeSearchDataSource();

        DataClassInfo.ClassSearchEnabled = true;
        DataClassInfo.ClassSearchSettings = SearchHelper.GetDefaultSearchSettings(DataClassInfo);
        SearchHelper.SetDefaultClassSearchColumns(DataClassInfo);
        DataClassInfo.Generalized.SetObject();
    }


    private void SetPageTypeSearchDataSource()
    {
        if (Mode != NewClassWizardModeEnum.DocumentType)
        {
            return;
        }

        if (!DataClassInfo.ClassIsCoupledClass)
        {
            DataClassInfo.ClassSearchIndexDataSource = SearchIndexDataSourceEnum.HTMLOutput;
        }
        else
        {
            DataClassInfo.ClassSearchIndexDataSource = DataClassInfo.ClassUsesPageBuilder ? SearchIndexDataSourceEnum.Both : SearchIndexDataSourceEnum.ContentFields;
        }
    }


    private void ProcessSitesAndNavigateToFinalOverview(WizardNavigationEventArgs e)
    {
        int classId = DataClassInfo.ClassID;
        bool isCustomTable = DataClassInfo.ClassIsCustomTable;
        bool licenseCheck = true;

        string selectedSite = ValidationHelper.GetString(usSites.Value, String.Empty);
        if (selectedSite == "0")
        {
            selectedSite = "";
        }
        string[] sites = selectedSite.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string site in sites)
        {
            int siteId = ValidationHelper.GetInteger(site, 0);

            SiteInfo si = SiteInfo.Provider.Get(siteId);
            if (si != null)
            {
                if (isCustomTable)
                {
                    if (!CustomTableItemProvider.LicenseVersionCheck(si.DomainName, ObjectActionEnum.Insert))
                    {
                        pnlMessages6.Visible = true;
                        pnlMessages6.ShowError(GetString("LicenseVersion.CustomTables"));
                        e.Cancel = true;
                        licenseCheck = false;
                    }
                }

                if (licenseCheck)
                {
                    ClassSiteInfo.Provider.Add(classId, siteId);

                    // Clear custom tables count
                    if (isCustomTable)
                    {
                        CustomTableItemProvider.ClearLicensesCount(true);
                    }
                }
            }
        }

        if (!licenseCheck)
        {
            PrepareParentTypesStep();
            return;
        }

        // Save default search settings
        if (((Mode == NewClassWizardModeEnum.DocumentType) && DataClassInfo.ClassHasURL) || (Mode == NewClassWizardModeEnum.CustomTable))
        {
            SetSearchSettings();
        }

        DisablePreviousStepsViewStates(e.CurrentStepIndex);
        EnableNextStepViewState(e.CurrentStepIndex);
        PrepareFinalOverview();

        // Explicitly log the synchronization with all changes and create version
        using (new CMSActionContext { CreateVersion = true })
        {
            SynchronizationHelper.LogObjectUpdate(DataClassInfo);
        }
    }


    /// <summary>
    /// Finish button is clicked.
    /// </summary>
    protected void wzdNewDocType_FinishButtonClick(object sender, EventArgs e)
    {
        string editUrl = "";
        int newObjId = DataClassInfo.ClassID;

        // Redirect to the document type edit site
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                editUrl = GetEditUrl("CMS.DocumentEngine", "EditDocumentType", newObjId);
                break;

            case NewClassWizardModeEnum.Class:
                editUrl = URLHelper.AppendQuery(GetEditUrl("CMS", "EditClass", newObjId, ModuleID), "&moduleid=" + ModuleID);
                break;

            case NewClassWizardModeEnum.CustomTable:
                editUrl = GetEditUrl("CMS.CustomTables", "EditCustomTable", newObjId);
                break;
        }

        URLHelper.Redirect(editUrl);
    }

    #endregion


    #region "Step preparing methods"

    /// <summary>
    /// Prepares and fills controls in the step 5.
    /// </summary>
    private void PrepareParentTypesStep()
    {
        wzdStepParentTypes.Title = GetString("DocumentType_New_Step5.Title");
        ucHeader.Description = GetString("DocumentType_New_Step5.Description");

        if (Mode == NewClassWizardModeEnum.DocumentType)
        {
            // Preselect Menu items document types
            var docTypeIDs = DataClassInfoProvider.GetClasses()
                .WhereTrue("ClassIsDocumentType")
                .WhereTrue("ClassIsMenuItemType")
                .Column("ClassID")
                .GetListResult<int>();

            usParentTypes.Value = TextHelper.Join(";", docTypeIDs);
            usParentTypes.Reload(true);
        }
    }


    /// <summary>
    /// Prepares and fills controls in the step 6.
    /// </summary>
    private void PrepareStepSites()
    {
        wzdStepSites.Title = GetString("DocumentType_New_Step6.Title");
        ucHeader.Description = GetString(Step6Description);

        // Preselect current site
        usSites.Value = SiteContext.CurrentSiteID;

        // Reload to have preselect site visible
        usSites.Reload(true);
    }


    /// <summary>
    /// Prepares and fills controls in the step 7.
    /// </summary>
    private void PrepareFinalOverview()
    {
        ucHeader.DescriptionVisible = false;
        wzdStepFinalOverview.Title = GetString("documenttype_new_step8.title");

        // Display final messages based on mode
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                lblDocumentCreated.Text = GetString("documenttype_new_step8_finished.documentcreated");
                lblTableCreated.Text = GetString("documenttype_new_step8_finished.tablecreated");
                lblChildTypesAdded.Text = GetString("documenttype_new_step8_finished.parenttypesadded");
                lblSitesSelected.Text = GetString("documenttype_new_step8_finished.sitesselected");
                lblPermissionNameCreated.Text = GetString("documenttype_new_step8_finished.permissionnamecreated");
                lblDefaultIconCreated.Text = GetString("documenttype_new_step8_finished.defaulticoncreated");
                lblSearchSpecificationCreated.Text = GetString("documenttype_new_step8_finished.searchspecificationcreated");

                // Hide some messages if creating container document type
                lblSearchSpecificationCreated.Visible = !IsContainer;
                lblTableCreated.Visible = !IsContainer;
                break;

            case NewClassWizardModeEnum.Class:
                lblDocumentCreated.Text = GetString("sysdev.class_new_step8_finished.documentcreated");
                lblTableCreated.Text = GetString("sysdev.class_new_step8_finished.tablecreated");
                break;

            case NewClassWizardModeEnum.CustomTable:
                lblDocumentCreated.Text = GetString("customtable.newwizzard.CustomTableCreated");
                lblSitesSelected.Text = GetString("customtable.newwizzard.SitesSelected");
                lblPermissionNameCreated.Text = GetString("customtable.newwizzard.PermissionNameCreated");
                lblSearchSpecificationCreated.Text = GetString("customtable.newwizzard.searchspecificationcreated");
                break;
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    /// <param name="elementName">Element name</param>
    /// <param name="newID">ID of current created table</param>
    /// <param name="parentID">ID of parent object type</param>
    /// <param name="displayTitle">Indicates if 'displaytitle=false' should be part of the URL</param>
    private String GetEditUrl(string resourceName, string elementName, int newID, int parentID = 0, bool displayTitle = false)
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo(resourceName, elementName);
        if (uiChild != null)
        {
            var url = UIContextHelper.GetElementUrl(uiChild, UIContext);
            url = URLHelper.AddParameterToUrl(url, "objectid", newID.ToString());

            if (parentID > 0)
            {
                url = URLHelper.AddParameterToUrl(url, "parentobjectid", parentID.ToString());
            }

            if (!displayTitle)
            {
                url = URLHelper.AddParameterToUrl(url, "displaytitle", "false");
            }

            return url;
        }

        return String.Empty;
    }


    /// <summary>
    /// Disable Viewstates of the current and previous steps.
    /// </summary>
    private void DisablePreviousStepsViewStates(int currentStep)
    {
        switch (currentStep)
        {
            case STEP_GENERAL:
                wzdStepGeneral.EnableViewState = false;
                break;

            case STEP_DATABASE_INFORMATION:
                wzdStepGeneral.EnableViewState = false;
                wzdStepDatabaseInformation.EnableViewState = false;
                break;

            case STEP_FIELDS:
                wzdStepGeneral.EnableViewState = false;
                wzdStepDatabaseInformation.EnableViewState = false;
                wzdStepFields.EnableViewState = false;
                break;

            case STEP_PARENT_TYPES:
                wzdStepGeneral.EnableViewState = false;
                wzdStepDatabaseInformation.EnableViewState = false;
                wzdStepFields.EnableViewState = false;
                wzdStepParentTypes.EnableViewState = false;
                break;

            case STEP_SITES:
                wzdStepGeneral.EnableViewState = false;
                wzdStepDatabaseInformation.EnableViewState = false;
                wzdStepFields.EnableViewState = false;
                wzdStepParentTypes.EnableViewState = false;
                wzdStepSites.EnableViewState = false;
                break;

            case STEP_FINAL_OVERVIEW:
                wzdStepGeneral.EnableViewState = false;
                wzdStepDatabaseInformation.EnableViewState = false;
                wzdStepFields.EnableViewState = false;
                wzdStepParentTypes.EnableViewState = false;
                wzdStepSites.EnableViewState = false;
                wzdStepFinalOverview.EnableViewState = false;
                break;
        }
    }


    /// <summary>
    /// Enable Viewstate of the next step.
    /// </summary>
    private void EnableNextStepViewState(int currentStep)
    {
        switch (currentStep)
        {
            case STEP_GENERAL:
                wzdStepDatabaseInformation.EnableViewState = true;
                break;

            case STEP_DATABASE_INFORMATION:
                wzdStepFields.EnableViewState = true;
                break;

            case STEP_FIELDS:
                wzdStepParentTypes.EnableViewState = true;
                break;

            case STEP_PARENT_TYPES:
                wzdStepSites.EnableViewState = true;
                break;

            case STEP_SITES:
                wzdStepFinalOverview.EnableViewState = true;
                break;
        }
    }


    /// <summary>
    /// Returns title of the current step.
    /// </summary>
    /// <param name="currentStep">Current step index (counted from 0)</param>
    private string GetCurrentStepTitle(int currentStep)
    {
        string currentStepTitle = "";

        switch (currentStep)
        {
            case STEP_GENERAL:
                currentStepTitle = wzdStepGeneral.Title;
                break;

            case STEP_FEATURES:
                currentStepTitle = wzdStepFeatures.Title;
                break;

            case STEP_DATABASE_INFORMATION:
                currentStepTitle = wzdStepDatabaseInformation.Title;
                break;

            case STEP_FIELDS:
                currentStepTitle = wzdStepFields.Title;
                break;

            case STEP_PARENT_TYPES:
                currentStepTitle = wzdStepParentTypes.Title;
                break;

            case STEP_SITES:
                currentStepTitle = wzdStepSites.Title;
                break;

            case STEP_FINAL_OVERVIEW:
                currentStepTitle = wzdStepFinalOverview.Title;
                break;
        }

        return currentStepTitle;
    }


    protected void LoadAvailableTables()
    {
        var tm = new TableManager(null);

        var where = new WhereCondition()
            .WhereNotIn("TABLE_NAME", new ObjectQuery<DataClassInfo>().Column("ClassTableName").WhereNotNull("ClassTableName"))
            .WhereNotIn("TABLE_NAME", new List<string> { "Analytics_Index", "sysdiagrams", "CI_Migration" });

        drpExistingTables.DataSource = tm.GetTables(where.ToString());
        drpExistingTables.DataBind();
    }


    /// <summary>
    /// Indicates if table has identity column defined
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <param name="columnName">Column name</param>
    /// <returns>Returns TRUE if table has identity column</returns>
    private static bool IsIdentityColumn(string tableName, string columnName)
    {
        const string queryText = @"SELECT COLUMNPROPERTY(OBJECT_ID(@tableName), @columnName, 'IsIdentity')";

        var queryData = new QueryDataParameters
        {
            { "tableName", tableName },
            { "columnName", columnName }
        };

        var result = ConnectionHelper.ExecuteScalar(queryText, queryData, QueryTypeEnum.SQLQuery);
        return ValidationHelper.GetBoolean(result, false);
    }

    #endregion
}