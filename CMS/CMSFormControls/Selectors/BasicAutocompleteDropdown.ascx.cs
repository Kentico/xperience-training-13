using System;

using CMS.Base;
using CMS.Helpers;

using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;


public partial class CMSFormControls_Selectors_BasicAutocompleteDropdown : FormEngineUserControl
{
    #region "Variables"

    private ListItemCollection mItems;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return SelectedValue;
        }
        set
        {
            SelectedValue = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Indicates whether control makes postback after item change
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoPostBack"), false);
        }
        set
        {
            SetValue("AutoPostBack", value);
        }
    }


    /// <summary>
    /// Selected value property
    /// </summary>
    public String SelectedValue
    {
        get
        {
            return hdnValue.Value;
        }
        set
        {
            ListItem li = Items.FindByValue(value);
            if (li != null)
            {
                txtAutocomplete.Text = li.Text;
                hdnValue.Value = value;
            }
        }
    }


    /// <summary>
    /// Collection of items
    /// </summary>
    public ListItemCollection Items
    {
        get
        {
            return mItems ?? (mItems = new ListItemCollection());
        }
    }


    /// <summary>
    /// Client java-script code before event 'onchange'.
    /// </summary>
    public virtual string OnBeforeClientChanged
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OnBeforeClientChanged"), null);
        }
        set
        {
            SetValue("OnBeforeClientChanged", value);
        }
    }


    /// <summary>
    /// Client java-script code after event 'onchange'.
    /// </summary>
    public virtual string OnAfterClientChanged
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OnAfterClientChanged"), null);
        }
        set
        {
            SetValue("OnAfterClientChanged", value);
        }
    }


    /// <summary>
    /// Indicates whether control is enabled/disabled
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Enabled"), txtAutocomplete.Enabled);
        }
        set
        {
            txtAutocomplete.Enabled = value;
            SetValue("Enabled", value);
        }
    }


    /// <summary>
    /// Specifies, whether the selector allows empty selection.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEmpty"), false);
        }
        set
        {
            SetValue("AllowEmpty", value);
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Enabled by default
        Enabled = true;

        base.OnInit(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Visible)
        {
            AutoPostBack |= HasDependingFields;

            ScriptHelper.RegisterJQuery(Page);
            ScriptHelper.RegisterJQueryUI(Page);
            ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/Controls/Autocomplete.js");

            // Events creation
            String events = String.Empty;
            if (!String.IsNullOrEmpty(OnBeforeClientChanged))
            {
                events += String.Format("$cmsj('#{0}_txtAutocomplete').bind('onBeforeChange', function (e, value) {{{1}}});", ClientID, OnBeforeClientChanged);
            }

            if (!String.IsNullOrEmpty(OnAfterClientChanged))
            {
                events += String.Format("$cmsj('#{0}_txtAutocomplete').bind('onAfterChange', function (e, value) {{{1}}});", ClientID, OnAfterClientChanged);
            }

            // Display '(none)' item
            if (AllowEmpty)
            {
                Items.Insert(0, new ListItem()
                {
                    Value = "",
                    Text = GetString("general.empty")
                });
            }

            // Conver Items collection to JSON format
            StringBuilder sb = new StringBuilder();
            foreach (ListItem li in Items)
            {
                sb.Append("{", CreateItemJSON("Text", li.Text, true), ",", CreateItemJSON("Value", li.Value));
                foreach (String key in li.Attributes.Keys)
                {
                    sb.Append(",", CreateItemJSON(key, li.Attributes[key]));
                }

                sb.Append("},");
            }

            String strSet = "[" + sb.ToString().TrimEnd(',') + "]";

            // Initial javascripts
            String init = String.Format(@"
$cmsj(document).ready(function () {{
    setUpSelector('{0}', {1},'{2}',{3});
    {4}
}});
", ClientID, AutoPostBack.ToString().ToLowerCSafe(), GetString("general.nodatafound"), strSet, events);

            ScriptHelper.RegisterClientScriptBlock(this, typeof(String), ClientID + "InitAutocompleteDropDown", ScriptHelper.GetScript(init));

            // Select first item
            if (String.IsNullOrEmpty(txtAutocomplete.Text) && String.IsNullOrEmpty(hdnValue.Value) && (Items.Count > 0))
            {
                txtAutocomplete.Text = Items[0].Text;
                hdnValue.Value = Items[0].Value;
            }
        }
    }


    /// <summary>
    /// Creates item's JSON representation
    /// </summary>
    /// <param name="key">Item's key</param>
    /// <param name="value">Item's value</param>
    /// <param name="encode">Indicate whether value should be encoded</param>
    private String CreateItemJSON(String key, String value, bool encode = false)
    {
        value = encode ? HTMLHelper.HTMLEncode(value) : value;
        return String.Format("\"{0}\":\"{1}\"", key, value);
    }

    #endregion
}