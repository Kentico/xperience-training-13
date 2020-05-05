using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_Settings_FormControls_GoToExternalURL : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets enabled state.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return btnSelect.Enabled;
        }
        set
        {
            txtURL.Enabled = value;
            btnSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value of access token.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtURL.Text;
        }
        set
        {
            txtURL.Text = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Gets or sets the URL where the button click should be redirected. If empty, control behaves like a textbox.
    /// </summary>
    public string ExternalURL
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the text of the button.
    /// </summary>
    public string ButtonText
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    public override object GetValue(string propertyName)
    {
        object result = base.GetValue(propertyName);
        if (result != null)
        {
            return result;
        }

        switch (propertyName.ToLowerCSafe())
        {
            case "externalurl":
                return ExternalURL;

            case "buttontext":
                return ButtonText;
        }

        return null;
    }


    public override bool SetValue(string propertyName, object value)
    {
        switch (propertyName.ToLowerCSafe())
        {
            case "externalurl":
                ExternalURL = ValidationHelper.GetString(value, "");
                break;

            case "buttontext":
                ButtonText = ValidationHelper.GetString(value, "");
                break;

            default:
                base.SetValue(propertyName, value);
                break;
        }
        return true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ExternalURL))
        {
            btnSelect.Visible = false;
        }
        else
        {
            btnSelect.OnClientClick = "window.open(" + ScriptHelper.GetString(ExternalURL.TrimEnd('/')) + "); return false;";
            if (!string.IsNullOrEmpty(ButtonText))
            {
                btnSelect.Text = ResHelper.LocalizeString(ButtonText);
            }
        }
    }

    #endregion
}