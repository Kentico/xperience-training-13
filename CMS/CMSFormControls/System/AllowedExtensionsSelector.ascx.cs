using System;
using System.Text;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


/// <summary>
/// This form control must be used with name 'extensions'. Another blank form control must be created with name 'allowed_extensions'. 
/// </summary>
public partial class CMSFormControls_System_AllowedExtensionsSelector : FormEngineUserControl, ICallbackEventHandler
{
    #region "Variables"

    protected string extensions = "";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return chkInehrit.Enabled;
        }
        set
        {
            chkInehrit.Enabled = value;
        }
    }


    /// <summary>
    /// Checkbox checked value. Possible options: 'custom' - for custom extensions, (nothing) - use site settings for allowed extensions.
    /// </summary>
    public override object Value
    {
        get
        {
            if (!chkInehrit.Checked)
            {
                return "custom";
            }
            else
            {
                return "inherit";
            }
        }
        set
        {
            string strValue = ValidationHelper.GetString(value, null);

            if ((strValue == "inherit") || String.IsNullOrEmpty(strValue))
            {
                chkInehrit.Checked = true;
                txtAllowedExtensions.Enabled = false;
            }
            else
            {
                chkInehrit.Checked = false;
                txtAllowedExtensions.Enabled = true;
            }
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Load allowed extensions
        if (!chkInehrit.Checked)
        {
            LoadOtherValues();
        }
            // Site extensions
        else
        {
            txtAllowedExtensions.Text = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSUploadExtensions");
        }

        // Registred scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DocAttachments_EnableDisableForm", GetScriptEnableDisableForm());
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DocAttachments_ReceiveExtensions", GetScriptReceiveExtensions());
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DocAttachments_LoadSiteSettings", ScriptHelper.GetScript("function GetExtensions(txtAllowedExtensions){ return " + Page.ClientScript.GetCallbackEventReference(this, "txtAllowedExtensions", "ReceiveExtensions", null) + " } \n"));

        // Initialize form
        chkInehrit.Attributes.Add("onclick", GetEnableDisableFormDefinition());
        EnableDisableForm();
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // User defined extensions
        if (ContainsColumn("allowed_extensions"))
        {
            txtAllowedExtensions.Text = ValidationHelper.GetString(Form.Data.GetValue("allowed_extensions"), null);
        }
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        // Set properties names
        object[,] values = new object[3,2];
        values[0, 0] = "allowed_extensions";
        values[0, 1] = txtAllowedExtensions.Text.Trim();
        return values;
    }


    /// <summary>
    /// Validates control.
    /// </summary>
    public override bool IsValid()
    {
        bool isValid = true;

        if (!ContainsColumn("extensions"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "extensions", GetString("general.text"));
            isValid = false;
        }

        if (!ContainsColumn("allowed_extensions"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "allowed_extensions", GetString("general.text"));
            isValid = false;
        }

        return isValid;
    }

    #endregion


    #region "Private methods"

    private string GetScriptReceiveExtensions()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("function ReceiveExtensions(rValue, context){");
        builder.Append("var extensions = rValue.split(\"|\");");
        builder.Append("var txtExtensions = document.getElementById(extensions[1]);\n");
        builder.Append("if (txtExtensions != null)\n");
        builder.Append("{ txtExtensions.value = extensions[0]; }}\n");

        return ScriptHelper.GetScript(builder.ToString());
    }


    private string GetScriptEnableDisableForm()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("function EnableDisableDocForm(chkAllowedExtensions, txtAllowedExtensions){");
        builder.Append("var txtAllowedExt = document.getElementById(txtAllowedExtensions);\n");
        builder.Append("var chkAllowedExt = document.getElementById(chkAllowedExtensions);\n");
        builder.Append("if (txtAllowedExt != null) {\n");
        builder.Append("var disabled = chkAllowedExt.checked;\n");
        builder.Append("txtAllowedExt.disabled = disabled;\n");
        builder.Append("if (disabled){ GetExtensions(txtAllowedExtensions); }\n");
        builder.Append("}}\n");

        return ScriptHelper.GetScript(builder.ToString());
    }


    private string GetEnableDisableFormDefinition()
    {
        return string.Format("EnableDisableDocForm('{0}', '{1}');", chkInehrit.ClientID, txtAllowedExtensions.ClientID);
    }


    private void EnableDisableForm()
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), "DocAttachments_EnableDisableFormStartup", ScriptHelper.GetScript(GetEnableDisableFormDefinition()));
    }

    #endregion


    #region "Callback handling"

    public string GetCallbackResult()
    {
        return extensions;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        // Get site settings
        extensions = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSUploadExtensions");

        // Returns site settings back to the client
        extensions = string.Format("{0}|{1}", extensions, eventArgument);
    }

    #endregion
}