using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.OnlineForms;
using CMS.SiteProvider;


/// <summary>
/// Form control for selecting specified form's field.
/// Returns data in "sitename;formname;fieldname" format.
/// Contains one parameter "FieldsDataType" that can be used to filter fields by data type.
/// </summary>
public partial class CMSModules_BizForms_FormControls_FormFieldSelector : FormEngineUserControl
{
    private const char SEPARATOR = ';';
    private string mSavedValue;

    /// <summary>
    /// Can be used to filter by data type.
    /// "Text" by default.
    /// </summary>
    public string FieldsDataType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FieldsDataType"), "Text");
        }
        set
        {
            SetValue("FieldsDataType", value);
        }
    }


    /// <summary>
    /// Value in "sitename;formname;fieldname" format.
    /// </summary>
    public override object Value
    {
        get
        {
            if (string.IsNullOrEmpty(drpFields.SelectedValue))
            {
                return null;
            }

            return string.Format("{0}{3}{1}{3}{2}", SiteContext.CurrentSiteName, selectForm.Value, drpFields.SelectedValue, SEPARATOR);
        }
        set
        {
            mSavedValue = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Value display name in "formdisplayname - fielddisplayname" format.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            if (string.IsNullOrEmpty(drpFields.SelectedValue))
            {
                return null;
            }

            return string.Format("{0} - {1}", selectForm.ValueDisplayName, drpFields.SelectedItem);
        }
    }


    private bool Loaded
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["Loaded"], false);
        }
        set
        {
            ViewState["Loaded"] = value;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!Loaded)
        {
            selectForm.DropDownSingleSelect.AutoPostBack = true;
            selectForm.Reload(false);

            LoadData();

            Loaded = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        drpFields.Enabled = (drpFields.Items.Count > 0);
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        LoadFields();
    }


    /// <summary>
    /// Loads data and tries to preset existing values.
    /// </summary>
    private void LoadData()
    {
        string formName = null;
        string fieldName = null;
        
        if (!string.IsNullOrEmpty(mSavedValue))
        {
            var valueNames = mSavedValue.Split(SEPARATOR);

            if (valueNames.Length == 3)
            {
                formName = valueNames[1];
                fieldName = valueNames[2];
            }
        }

        if ((formName != null) && (selectForm.DropDownItems.FindByValue(formName) != null))
        {
            selectForm.Value = formName;
        }
        
        LoadFields();

        if ((fieldName != null) && (drpFields.Items.FindByValue(fieldName) != null))
        {
            drpFields.SelectedValue = fieldName;
        }
    }


    /// <summary>
    /// Loads all the fields from form selected in the first dropdown (selectForm) into the second dropdown (drpFields).
    /// </summary>
    private void LoadFields()
    {
        string className = ValidationHelper.GetString(selectForm.Value, null);
        if (className == null)
        {
            return;
        }

        var form = BizFormInfo.Provider.Get(className, SiteContext.CurrentSiteID);
        if (form == null)
        {
            return;
        }

        var classInfo = DataClassInfoProvider.GetDataClassInfo(form.FormClassID);
        if (classInfo == null)
        {
            return;
        }

        var formInfo = FormHelper.GetFormInfo(classInfo.ClassName, false);
        if (formInfo == null)
        {
            return;
        }

        drpFields.Items.Clear();

        IEnumerable<FormFieldInfo> fields;
        if (FieldsDataType != FieldDataType.Unknown && FieldsDataType != FieldDataType.ALL)
        {
            fields = formInfo.GetFields(FieldsDataType).Where(x => x.Visible);
        }
        else
        {
            fields = formInfo.GetFields(true, true);
        }

        var resolver = MacroResolver.GetInstance();
        foreach (var fieldInfo in fields)
        {
            drpFields.Items.Add(new ListItem(fieldInfo.GetDisplayName(resolver), fieldInfo.Name));
        }
    }
}