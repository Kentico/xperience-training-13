using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

using Newtonsoft.Json;

public partial class CMSFormControls_Basic_DropDownListControl : ListFormControl
{
    private bool? mEditText;
    private string mOnChangeScript;


    protected override ListControl ListControl => dropDownList;
    

    protected override ListSelectionMode SelectionMode => ListSelectionMode.Single;


    protected override string FormControlName => FormFieldControlName.DROPDOWNLIST;


    protected override string DefaultCssClass => null;
    
    
    /// <summary>
    /// Returns display name of the value.
    /// </summary>
    public override string ValueDisplayName => (EditText || (dropDownList.SelectedItem == null) ? txtCombo.Text : dropDownList.SelectedItem.Text);


    /// <summary>
    /// Gets or sets selected string value.
    /// </summary>
    public string SelectedValue
    {
        get
        {
            return (EditText) ? txtCombo.Text : dropDownList.SelectedValue;
        }

        set
        {
            if (EditText)
            {
                txtCombo.Text = value;
            }
            else
            {
                dropDownList.SelectedValue = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets selected index. Returns -1 if no element is selected.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            if (EditText)
            {
                if (dropDownList.Items.FindByValue(txtCombo.Text) != null)
                {
                    return dropDownList.SelectedIndex;
                }
                return -1;
            }

            return dropDownList.SelectedIndex;
        }

        set
        {
            dropDownList.SelectedIndex = value;
            if (EditText)
            {
                txtCombo.Text = dropDownList.SelectedValue;
            }
        }
    }


    /// <summary>
    /// Enables to edit text from textbox and select values from dropdown list.
    /// </summary>
    public bool EditText
    {
        get
        {
            return mEditText ?? ValidationHelper.GetBoolean(GetValue("edittext"), false);
        }

        set
        {
            mEditText = value;
        }
    }


    /// <summary>
    /// Gets dropdown list control.
    /// </summary>
    public CMSDropDownList DropDownList => dropDownList;


    /// <summary>
    /// Gets textbox control.
    /// </summary>
    public CMSTextBox TextBoxControl => txtCombo;


    /// <summary>
    /// Gets or sets Javascript code that is executed when selected item is changed.
    /// </summary>
    public string OnChangeClientScript
    {
        get
        {
            return mOnChangeScript ?? ValidationHelper.GetString(GetValue("OnChangeClientScript"), String.Empty);
        }

        set
        {
            mOnChangeScript = value;
        }
    }


    /// <summary>
    /// Indicates whether actual value (that is not present among options) will be displayed as DDL item.
    /// </summary>
    public bool DisplayActualValueAsItem
    {
        get
        {
            return GetValue("DisplayActualValueAsItem", false);
        }

        set
        {
            SetValue("DisplayActualValueAsItem", value);
        }
    }

    
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (EditText)
        {
            ApplyCssClassAndStyles(txtCombo);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // It's always necessary to determine visible control
        txtCombo.Visible = EditText;
        dropDownList.Visible = !EditText;

        if (StopProcessing || !Enabled)
        {
            // Do nothing
            return;
        }

        if (EditText)
        {
            btnAutocomplete.Visible = true;

            string dependingScriptPart = HasDependingFields ? string.Format(
@",
change: function (event, ui) {{
    __doPostBack('#{0}', '');
}},
close: function(event, ui) {{
    if (event.originalEvent && event.originalEvent.type === 'menuselect') {{
        __doPostBack('#{0}', '');
    }}
}}", txtCombo.ClientID) : "";

            ScriptHelper.RegisterJQueryUI(Page);
            ScriptHelper.RegisterStartupScript(Page, typeof (string), "Autocomplete_" + ClientID, ScriptHelper.GetScript(string.Format(
@"var txtCombo{0} = $cmsj('#{0}');

// Perform autocomplete
txtCombo{0}.autocomplete({{
    source: {1},
    minLength: 0,
    appendTo: '#{2}'{4}
}});

// Open dropdown list
txtCombo{0}.add($cmsj('#{3}')).on('click', function () {{
    txtCombo{0}.autocomplete('search', '');
    txtCombo{0}.focus();
}});

// Close dropdown list if scrolled outside the list
$cmsj(document).bind('mousewheel DOMMouseScroll', function (e) {{
    if (!txtCombo{0}.autocomplete('widget').is(':hover')) {{
            txtCombo{0}.autocomplete('close');
    }}
}});", txtCombo.ClientID, GetDataAsJsArray(), autoComplete.ClientID, btnAutocomplete.ClientID, dependingScriptPart)));
        }
        else
        {
            if (!String.IsNullOrEmpty(OnChangeClientScript))
            {
                dropDownList.Attributes.Add("onchange", OnChangeClientScript);
            }
        }
    }


    /// <summary>
    /// Returns string with selected value of the <see cref="DropDownList"/> or text of the <see cref="TextBoxControl"/>.
    /// </summary>
    protected override object GetControlValue()
    {
        return (EditText) ? txtCombo.Text : dropDownList.SelectedValue;
    }


    /// <summary>
    /// Sets selected item of the <see cref="DropDownList"/> or text of the <see cref="TextBoxControl"/> based on the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to be set</param>
    protected override void SetControlValue(object value)
    {
        LoadAndSelectList();

        value = ConvertInputValue(value);

        string selectedValue = ValidationHelper.GetString(value, String.Empty);

        if (value != null)
        {
            EnsureActualValueAsItem(selectedValue);
        }

        if (EditText)
        {
            txtCombo.Text = selectedValue;
        }
        else
        {
            dropDownList.ClearSelection();

            SelectSingleValue(selectedValue);
        }
    }


    /// <summary>
    /// Returns data as JavaScript array (e.g.: ['value1', 'value2']).
    /// </summary>
    private string GetDataAsJsArray()
    {
        var array = new List<string>();

        foreach (var item in dropDownList.Items)
        {
            array.Add(((ListItem)item).Text);
        }

        return JsonConvert.SerializeObject(array, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
    }


    /// <summary>
    /// Ensures that a value which is not among DDL items but is present in the database is added to DDL items collection.
    /// </summary>
    private void EnsureActualValueAsItem(string value)
    {
        if (DisplayActualValueAsItem)
        {
            var item = dropDownList.Items.FindByValue(value);
            if (item == null)
            {
                dropDownList.Items.Add(new ListItem(value));

                if (SortItems)
                {
                    dropDownList.SortItems();
                }
            }
        }
    }
}