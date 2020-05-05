using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

[ValidationProperty("Value")]
public partial class CMSFormControls_System_CodeName : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets whether the control is read only
    /// </summary>
    public bool ReadOnly
    {
        get
        {
            return textbox.ReadOnly;
        }
        set
        {
            textbox.ReadOnly = value;
        }
    }


    /// <summary>
    /// If set the code name has to be in identifier format (without dots, spaces and special characters).
    /// </summary>
    public bool RequireIdentifier
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RequireIdentifier"), false);
        }
        set
        {
            SetValue("RequireIdentifier", value);
        }
    }


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
            if (textbox != null)
            {
                textbox.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Get the text
            string text = textbox.Text;

            if (String.IsNullOrEmpty(text) || (text == textbox.WatermarkText))
            {
                // Automatic code name in case of empty value
                text = InfoHelper.CODENAME_AUTOMATIC;
            }

            return text;
        }
        set
        {
            string stringValue = ValidationHelper.GetString(value, "");

            textbox.Text = stringValue;
        }
    }


    /// <summary>
    /// Returns client ID of the textbox.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return textbox.ClientID;
        }
    }


    /// <summary>
    /// Publicly visible textbox
    /// </summary>
    public CMSTextBox TextBox
    {
        get
        {
            return textbox;
        }
    }


    /// <summary>
    /// Maximum length of plain text or resource string key. Validates in IsValid() method.
    /// </summary>
    public int MaxLength
    {
        get
        {
            return textbox.MaxLength;
        }
        set
        {
            textbox.MaxLength = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site. Default value is FALSE for localizable text box.
    /// </summary>
    public override bool IsLiveSite
    {
        get;
        set;
    }


    /// <summary>
    /// If true, the hint is shown
    /// </summary>
    public bool ShowHint
    {
        get
        {
            return iconHelp.Visible;
        }
        set
        {
            iconHelp.Visible = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Apply CSS style
        if (!String.IsNullOrEmpty(CssClass))
        {
            textbox.CssClass = CssClass;
            CssClass = null;
        }

        textbox.WatermarkText = ResHelper.GetString("general.automatic");
        textbox.WatermarkCssClass = "CodeNameTextBoxInactive";
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string toolTip = GetString("codename.tooltip");

        if (Enabled)
        {
            toolTip += " " + GetString("codename.tooltipedit");
        }

        spanScreenReader.Text = toolTip;
        iconHelp.ToolTip = toolTip;

        textbox.TabIndex = -1;

        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");
    }


    /// <summary>
    /// Validates control.
    /// </summary>
    public override bool IsValid()
    {
        // Check for maximum length
        if (MaxLength > 0)
        {
            return (textbox.Text.Length <= MaxLength);
        }

        var stringValue = (string)Value;
        if ((stringValue != InfoHelper.CODENAME_AUTOMATIC) && !String.IsNullOrEmpty(stringValue))
        {
            if (!RequireIdentifier && !ValidationHelper.IsCodeName(stringValue))
            {
                ValidationError = ResHelper.GetStringFormat("general.codenamenotvalid", HTMLHelper.HTMLEncode(stringValue));
                return false;
            }

            if (RequireIdentifier && !ValidationHelper.IsIdentifier(stringValue))
            {
                ValidationError = ResHelper.GetStringFormat("general.erroridentifierformat", HTMLHelper.HTMLEncode(stringValue));
                return false;
            }
        }

        return base.IsValid();
    }

    #endregion
}