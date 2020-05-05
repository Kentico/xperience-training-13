using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_BannerManagement_FormControls_HitsClicksControl : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Resource string used to mark radio option meaning unlimited number.
    /// </summary>
    public string AllowUnlimitedResourceString
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AllowUnlimitedResourceString"), String.Empty);
        }
        set
        {
            SetValue("AllowUnlimitedResourceString", value);
        }
    }


    /// <summary>
    /// Resource string used to mark radio option meaning specific number.
    /// </summary>
    public string AllowSpecificResourceString
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AllowSpecificResourceString"), String.Empty);
        }
        set
        {
            SetValue("AllowSpecificResourceString", value);
        }
    }


    public string AddNumberResourceString
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AddNumberResourceString"), String.Empty);
        }
        set
        {
            SetValue("AddNumberResourceString", value);
        }
    }


    public string NumberToAddResourceString
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NumberToAddResourceString"), String.Empty);
        }
        set
        {
            SetValue("NumberToAddResourceString", value);
        }
    }


    public override object Value
    {
        get
        {
            if (radAllowSpecific.Checked)
            {
                int val;
                if (int.TryParse(txtNumberLeft.Text, out val))
                {
                    return val;
                }

                return null;
            }
            return null;
        }
        set
        {
            if (ValidationHelper.IsInteger(value))
            {
                radAllowSpecific.Checked = true;
                txtNumberLeft.Text = value.ToString();
            }
            else
            {
                // If incorrect format or null, set to unlimited
                radUnlimited.Checked = true;
                txtNumberLeft.Text = "0";
            }
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        string script = @"
function addNumberHitsClicks(clientID, numberToAdd)
{
    var control = $cmsj('#' + clientID); 
    control.val(parseInt(control.val()) + parseInt(numberToAdd));
}
";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "addNumberHitsClicks", script, true);
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterDialogScript(Page);

        radUnlimited.ResourceString = AllowUnlimitedResourceString;
        radAllowSpecific.ResourceString = AllowSpecificResourceString;

        string modalUrl = string.Format("{0}?numbercontrolid={1}&addnumberres={2}&numbertoaddres={3}", UrlResolver.ResolveUrl("~/CMSModules/BannerManagement/Tools/Banner/HitClickAddModal.aspx"), txtNumberLeft.ClientID, AddNumberResourceString, NumberToAddResourceString);

        btnAdd.OnClientClick = string.Format("modalDialog('{0}', 'AddNumber', 400, 170);", modalUrl);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlSpecific.Enabled = radAllowSpecific.Checked;
    }

    #endregion
}