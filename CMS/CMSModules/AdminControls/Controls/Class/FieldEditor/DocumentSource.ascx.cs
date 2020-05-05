using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_DocumentSource : CMSUserControl
{
    #region "Variables"

    private bool mEnabled = true;

    #endregion


    #region "Events"

    /// <summary>
    /// Fired when source field DDL changes.
    /// </summary>
    public event EventHandler OnSourceFieldChanged;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets FormInfo object.
    /// </summary>
    public FormInfo FormInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets class name.
    /// </summary>
    public string ClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value indicating if field editor is used as alternative form editor.
    /// </summary>
    public bool IsAlternativeForm
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value indicating if field editor is used as inherited form editor.
    /// </summary>
    public bool IsInheritedForm
    {
        get;
        set;
    }


    /// <summary>
    /// Gets value which is selected in DDL Source field.
    /// </summary>
    public string SourceFieldValue
    {
        get
        {
            return drpSourceField.SelectedValue;
        }
    }


    /// <summary>
    /// Gets value which is selected in DDL Alias Source field.
    /// </summary>
    public string SourceAliasFieldValue
    {
        get
        {
            return drpSourceAliasField.SelectedValue;
        }
    }


    /// <summary>
    /// Gets value indicating if any content is displayed.
    /// </summary>
    public bool VisibleContent
    {
        get
        {
            return pnlSourceField.Visible;
        }
    }


    /// <summary>
    /// Indicates if inner controls are enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
            drpSourceAliasField.Enabled = drpSourceField.Enabled = mEnabled;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            Reload();
        }
    }


    /// <summary>
    /// Reloads control with data.
    /// </summary>
    public void Reload()
    {
        // Check if provided class name exists
        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
        if (dci == null)
        {
            return;
        }

        // Display or hide source field selection
        if (dci.ClassIsDocumentType && (FormInfo != null) && !IsAlternativeForm && !IsInheritedForm)
        {
            // Fill source field drop down list
            pnlSourceField.Visible = true;

            // Add document name source field
            drpSourceField.Items.Clear();
            drpSourceField.Items.Add(new ListItem(GetString(dci.ClassIsProduct ? "TemplateDesigner.ImplicitProductSourceField" : "TemplateDesigner.ImplicitSourceField"), ""));

            // Add alias name source field
            drpSourceAliasField.Items.Clear();
            drpSourceAliasField.Items.Add(new ListItem(GetString("TemplateDesigner.DefaultSourceField"), ""));

            AddField(drpSourceAliasField, "NodeID");
            AddField(drpSourceAliasField, "DocumentID");

            var columnNames = FormInfo.GetColumnNames();
            if (columnNames != null)
            {
                // Add attribute list item to the list of attributes
                foreach (string name in columnNames)
                {
                    FormFieldInfo ffiColumn = FormInfo.GetFormField(name);

                    // Add only text fields
                    if (ffiColumn.IsNodeNameSourceCandidate())
                    {
                        AddField(drpSourceField, name);
                    }

                    // Add all fields which allow to be used as alias
                    var dataType = DataTypeManager.GetDataType(TypeEnum.Field, ffiColumn.DataType);
                    if ((dataType != null) && dataType.AllowAsAliasSource)
                    {
                        AddField(drpSourceAliasField, name);
                    }
                }
            }

            // Set selected value
            if (drpSourceField.Items.FindByValue(dci.ClassNodeNameSource) != null)
            {
                drpSourceField.SelectedValue = dci.ClassNodeNameSource;
            }

            if (drpSourceAliasField.Items.FindByValue(dci.ClassNodeAliasSource) != null)
            {
                drpSourceAliasField.SelectedValue = dci.ClassNodeAliasSource;
            }
        }
    }


    /// <summary>
    /// Adds the given field to dropdown
    /// </summary>
    /// <param name="drp">Dropdown list</param>
    /// <param name="column">Field to add</param>
    private void AddField(CMSDropDownList drp, string column)
    {
        drp.Items.Add(new ListItem(column, column));
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Called when source field selected index changed.
    /// </summary>
    protected void drpSourceField_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (OnSourceFieldChanged != null)
        {
            OnSourceFieldChanged(sender, EventArgs.Empty);
        }
    }

    #endregion
}