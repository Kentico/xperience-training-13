using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_AlternativeFormFieldEditor : CMSUserControl
{
    #region "Events"

    public event EventHandler OnBeforeSave;


    public event EventHandler OnAfterSave;

    #endregion


    #region "Variables"

    protected int mAlternativeFormId = 0;
    protected FieldEditorControlsEnum mDisplayedControls = FieldEditorControlsEnum.None;

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
            fieldEditor.IsLiveSite = value;
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates if system fields (node and document fields) are enabled.
    /// </summary>
    public bool EnableSystemFields
    {
        get
        {
            return fieldEditor.EnableSystemFields;
        }
        set
        {
            fieldEditor.EnableSystemFields = value;
        }
    }


    /// <summary>
    /// Form id (with alternative form definition).
    /// </summary>
    public int AlternativeFormID
    {
        get
        {
            return mAlternativeFormId;
        }
        set
        {
            mAlternativeFormId = value;
        }
    }


    /// <summary>
    /// Specify set of controls which should be offered for field types.
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
        }
    }


    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get
        {
            return fieldEditor.Mode;
        }
        set
        {
            fieldEditor.Mode = value;
        }
    }


    /// <summary>
    /// Enables or disables to edit <see cref='CMS.FormEngine.FormFieldInfo.Inheritable'> settings.
    /// </summary>
    public bool ShowInheritanceSettings
    {
        get
        {
            return fieldEditor.ShowInheritanceSettings;
        }
        set
        {
            fieldEditor.ShowInheritanceSettings = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(mAlternativeFormId);
        UIContext.EditedObject = afi;

        if (afi == null)
        {
            return;
        }

        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(afi.FormClassID);
        if (dci == null)
        {
            ShowError(GetString("general.invalidid"));
        }
        else
        {
            string formDef = dci.ClassFormDefinition;
            string coupledClassName = null;

            if (afi.FormCoupledClassID > 0)
            {
                // If coupled class is defined combine form definitions
                DataClassInfo coupledDci = DataClassInfoProvider.GetDataClassInfo(afi.FormCoupledClassID);
                if (coupledDci != null)
                {
                    formDef = FormHelper.MergeFormDefinitions(formDef, coupledDci.ClassFormDefinition);
                    coupledClassName = coupledDci.ClassName;
                }
            }

            var resource = ResourceInfo.Provider.Get(QueryHelper.GetInteger("moduleid", 0));

            // Allow development mode only for non-system tables
            fieldEditor.DevelopmentMode = (resource != null) && resource.IsEditable;

            // Set original form definition
            fieldEditor.OriginalFormDefinition = formDef;

            // Merge class and alternative form definitions
            formDef = FormHelper.MergeFormDefinitions(formDef, afi.FormDefinition);

            // Initialize field editor mode and load form definition
            fieldEditor.AlternativeFormFullName = afi.FullName;
            fieldEditor.FormDefinition = formDef;

            // Specify set of controls which should be offered for field types
            fieldEditor.DisplayedControls = mDisplayedControls;
            fieldEditor.ClassName = dci.ClassName;
            fieldEditor.CoupledClassName = coupledClassName;

            // Handle definition update (move up, move down, delete, OK button)
            fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
            fieldEditor.OnFieldNameChanged += fieldEditor_OnFieldNameChanged;
        }
    }


    /// <summary>
    /// Form field name changed event handler.
    /// </summary>
    void fieldEditor_OnFieldNameChanged(object sender, string oldFieldName, string newFieldName)
    {
        FormHelper.RenameFieldInAlternativeFormLayout(mAlternativeFormId, oldFieldName, newFieldName);
    }


    /// <summary>
    /// After form definition update event handler.
    /// </summary>
    private void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        // Perform OnBeforeSave if defined
        if (OnBeforeSave != null)
        {
            OnBeforeSave(this, EventArgs.Empty);
        }

        // Stop processing if set from outside
        if (StopProcessing)
        {
            return;
        }

        // Get alternative form info object and data class info object
        AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(mAlternativeFormId);

        if (afi != null)
        {
            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(afi.FormClassID);

            if (dci != null)
            {
                string formDefinition = dci.ClassFormDefinition;

                if (afi.FormCoupledClassID > 0)
                {
                    // Combine form definitions of class and its coupled class
                    DataClassInfo coupledDci = DataClassInfoProvider.GetDataClassInfo(afi.FormCoupledClassID);
                    if (coupledDci != null)
                    {
                        formDefinition = FormHelper.MergeFormDefinitions(formDefinition, coupledDci.ClassFormDefinition);
                    }
                }

                // Compare original and alternative form definitions - store differences only
                afi.FormDefinition = FormHelper.GetFormDefinitionDifference(formDefinition, fieldEditor.FormDefinition, true);
                // Update alternative form info in database
                AlternativeFormInfoProvider.SetAlternativeFormInfo(afi);
            }
            else
            {
                ShowError(GetString("general.invalidid"));
            }
        }

        // Perform OnAfterSave if defined
        if (OnAfterSave != null)
        {
            OnAfterSave(this, EventArgs.Empty);
        }
    }
    
    #endregion
}