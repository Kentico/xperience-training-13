using System;
using System.Web.UI.WebControls;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_FieldTypeSelector : CMSUserControl
{
    #region "Events"

    /// <summary>
    /// Event raised when field type is selected.
    /// </summary>
    public event EventHandler OnSelectionChanged;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if system field can be created.
    /// </summary>
    public bool EnableSystemFields
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if field without representation in database can be created.
    /// </summary>
    public bool AllowDummyFields
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if extra fields can be created.
    /// </summary>
    public bool AllowExtraFields
    {
        get;
        set;
    }


    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if field editor works in development mode.
    /// </summary>
    public bool DevelopmentMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets value indicating new item is being edited.
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
        }
    }


    /// <summary>
    /// Returns selected field type.
    /// </summary>
    public FieldTypeEnum SelectedFieldType
    {
        get
        {
            return EnumStringRepresentationExtensions.ToEnum<FieldTypeEnum>(drpFieldType.SelectedItem.Value);
        }
    }


    /// <summary>
    /// Indicates if current form is used as alternative form.
    /// </summary>
    private bool IsAlternativeForm
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
                    return true;

                default:
                    return false;
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the control.
    /// </summary>
    public void Reload()
    {
        pnlTypeSelector.Visible = false;

        if (IsNewItemEdited)
        {
            drpFieldType.Items.Clear();

            ListItem li = null;

            if (!IsAlternativeForm && !IsInheritedForm)
            {
                li = new ListItem(GetString("fieldeditor.fieldtype.standard"), FieldTypeEnum.Standard.ToStringRepresentation());
                drpFieldType.Items.Add(li);

                if (DevelopmentMode)
                {
                    li = new ListItem(GetString("fieldeditor.fieldtype.primary"), FieldTypeEnum.Primary.ToStringRepresentation());
                    drpFieldType.Items.Add(li);
                }

                if (EnableSystemFields)
                {
                    li = new ListItem(GetString("fieldeditor.fieldtype.document"), FieldTypeEnum.Document.ToStringRepresentation());
                    drpFieldType.Items.Add(li);
                }
            }

            if (AllowDummyFields)
            {
                li = new ListItem(GetString("fieldeditor.fieldtype.dummy"), FieldTypeEnum.Dummy.ToStringRepresentation());
                drpFieldType.Items.Add(li);
            }

            if (AllowExtraFields)
            {
                li = new ListItem(GetString("fieldeditor.fieldtype.extra"), FieldTypeEnum.Extra.ToStringRepresentation());
                drpFieldType.Items.Add(li);
            }

            // Display only for two or more options
            if (drpFieldType.Items.Count > 1)
            {
                pnlTypeSelector.Visible = true;
            }
            else if (AllowDummyFields)
            {
                MessagesPlaceHolder.ShowInformation(GetString("fieldeditor.fieldtype.currentfieldisdummy"));
            }

            drpFieldType.SelectedIndex = 0;
        }
    }


    protected void drpFieldType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (OnSelectionChanged != null)
        {
            OnSelectionChanged(this, EventArgs.Empty);
        }
    }

    #endregion
}
