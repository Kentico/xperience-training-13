using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Selectors_SelectObjectColumn : FormEngineUserControl
{
    private string selectedColumn = String.Empty;

    /// <summary>
    /// Gets or sets selected object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectType"), null);
        }
        set
        {
            SetValue("ObjectType", value);
        }
    }


    /// <summary>
    /// Gets or sets selected value.
    /// </summary>
    public override object Value
    {
        get
        {
            return (drpColumn.SelectedItem != null) ? drpColumn.SelectedItem.Value : String.Empty;
        }
        set
        {
            selectedColumn = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Indicates if all columns should be displayed (except primary key column).
    /// </summary>
    public bool ShowAllColumns
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAllColumns"), false);
        }
        set
        {
            SetValue("ShowAllColumns", value);
        }
    }


    /// <summary>
    /// Indicates if column caption should be used instead of column name.
    /// </summary>
    public bool UseColumnCaption
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseColumnCaption"), false);
        }
        set
        {
            SetValue("UseColumnCaption", value);
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    /// <summary>
    /// Fills dropdown list with column names
    /// </summary>
    protected void SetupControl()
    {
        drpColumn.Items.Clear();

        // Get object by object type
        GeneralizedInfo generalObject = ModuleManager.GetObject(ObjectType);
        if (generalObject != null)
        {
            FormInfo fi = FormHelper.GetFormInfo(generalObject.TypeInfo.ObjectClassName, false);
            if (fi != null)
            {
                // Get fields except the primary key
                var fields = fi.GetFields(true, ShowAllColumns).Where(f => !f.PrimaryKey);

                // Sort fields by visibility and caption or name
                fields = fields.OrderByDescending(f => f.Visible).ThenBy(f => UseColumnCaption ? (string.IsNullOrEmpty(f.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, ContextResolver)) ? f.Name : f.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, ContextResolver)) : f.Name);

                foreach (FormFieldInfo field in fields)
                {
                    string caption = field.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, ContextResolver);
                    if (UseColumnCaption && !String.IsNullOrEmpty(caption))
                    {
                        drpColumn.Items.Add(new ListItem(caption, field.Name));
                    }
                    else
                    {
                        drpColumn.Items.Add(field.Name);
                    }
                }
            }

            if (selectedColumn != String.Empty)
            {
                // Select selected column
                ListItem item = drpColumn.Items.FindByValue(ValidationHelper.GetString(selectedColumn, String.Empty));
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }
    }
}