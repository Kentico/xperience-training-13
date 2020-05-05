using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;


public partial class CMSModules_AdminControls_Controls_Class_ClassFields : FormEngineUserControl
{
    #region "Variables"

    private string mClassName;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            drpField.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpField.SelectedValue;
        }
        set
        {
            // Reload data
            ReloadData();
            if (drpField.Items.Count > 0)
            {
                // Try to select specified field
                ListItem li = drpField.Items.FindByValue(ValidationHelper.GetString(value, string.Empty));
                if (li != null)
                {
                    drpField.ClearSelection();
                    li.Selected = true;
                }
            }
        }
    }


    /// <summary>
    /// Gets the display name of the value item. Returns null if display name is not available.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            // Return the selected text
            if (drpField.SelectedIndex >= 0)
            {
                return drpField.Items[drpField.SelectedIndex].Text;
            }
            else
            {
                return drpField.Text;
            }
        }
    }


    /// <summary>
    /// Gets or sets name of the class which fields should be displayed.
    /// </summary>
    public string ClassName
    {
        get
        {
            if (string.IsNullOrEmpty(mClassName))
            {
                mClassName = ValidationHelper.GetString(GetValue("ClassName"), null);
            }
            return mClassName;
        }
        set
        {
            mClassName = value;
        }
    }


    /// <summary>
    /// If true the "(none)" item will be added to the selector.
    /// </summary>
    public bool AllowNone
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowNone"), true);
        }
        set
        {
            SetValue("AllowNone", value);
        }
    }


    /// <summary>
    /// Gets or sets data type of fields that should be offered. Optional.
    /// </summary>
    public string FieldDataType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FieldDataType"), CMS.DataEngine.FieldDataType.Unknown);
        }
        set
        {
            SetValue("FieldDataType", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the control.
    /// </summary>
    protected void ReloadData()
    {
        if (drpField.Items.Count == 0)
        {
            // Load dropdownlist with fields of specified class
            FormInfo fi = FormHelper.GetFormInfo(ClassName, false);
            if (fi != null)
            {
                if (CMSString.Equals(ClassName, "cms.user", true))
                {
                    // Combine user fields with those of user settings
                    FormInfo coupledInfo = FormHelper.GetFormInfo("cms.usersettings", false);
                    if (coupledInfo != null)
                    {
                        fi.CombineWithForm(coupledInfo, false);
                    }
                }

                IEnumerable<FormFieldInfo> fields;
                if (FieldDataType != CMS.DataEngine.FieldDataType.Unknown)
                {
                    fields = fi.GetFields(FieldDataType);
                }
                else
                {
                    fields = fi.GetFields(true, true);
                }

                foreach (var fieldInfo in fields)
                {
                    drpField.Items.Add(new ListItem(fieldInfo.GetDisplayName(MacroResolver.GetInstance()), fieldInfo.Name));
                }
            }

            if (AllowNone)
            {
                // Add '(none)' item
                drpField.Items.Insert(0, new ListItem(GetString("general.selectnone"), string.Empty));
            }
        }
    }

    #endregion
}