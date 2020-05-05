using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Inputs_EmailInput : FormEngineUserControl
{
    #region "Properties"

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
            txtEmailInput.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return GetInputFromEmailTextField();
        }
        set
        {
            txtEmailInput.Text = (string)value;
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox with emailinput.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtEmailInput.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets if multiple e-mail addresses can be entered.
    /// </summary>
    public bool AllowMultipleAddresses
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets string that should be used to delimit multiple addresses (default separator is semicolon - ;).
    /// </summary>
    public string EmailSeparator
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set control style and css class
        if (!string.IsNullOrEmpty(ControlStyle))
        {
            txtEmailInput.Attributes.Add("style", ControlStyle);
        }
        if (!string.IsNullOrEmpty(CssClass))
        {
            txtEmailInput.CssClass = CssClass;
            CssClass = null;
        }

        // Set additional properties
        AllowMultipleAddresses = ValidationHelper.GetBoolean(GetValue("allowmultipleaddresses"), AllowMultipleAddresses);
        EmailSeparator = ValidationHelper.GetString(GetValue("emailseparator"), EmailSeparator);
        SetInputMaximumLength();
    }


    /// <summary>
    /// Returns <c>true</c> if email input is empty or has correct format.
    /// </summary>
    /// <remarks>Also returns <c>true</c> if email input contains macro and is not used on live site.</remarks>>
    public override bool IsValid()
    {
        const bool CHECK_LENGTH = true;
        string emailInput = ValidationHelper.GetString(Value, String.Empty);

        if (string.IsNullOrEmpty(emailInput))
        {
            return true;
        }

        // Check if valid e-mail addresses were entered
        bool isValidEmail = (AllowMultipleAddresses
            ? ValidationHelper.AreEmails(emailInput, EmailSeparator, CHECK_LENGTH) 
            : ValidationHelper.IsEmail(emailInput, CHECK_LENGTH));
        if (isValidEmail)
        {
            return true;
        }

        ValidationError = GetString("EmailInput.ValidationError");
        return false;
    }


    /// <summary>
    /// Enables client side validation for email format.
    /// </summary>
    /// <param name="validationGroup">Validation group identifier. If no identifier is provided, <see cref="String.Empty"/> is used.</param>
    /// <param name="errorMessageResourceString">Resource string of a message shown in case of invalid e-mail. If no value is provided, general message is displayed.</param>
    public CMSFormControls_Inputs_EmailInput EnableClientSideEmailFormatValidation(string validationGroup = null, string errorMessageResourceString = null)
    {
        revEmailValid.Enabled = true;
        txtEmailInput.ValidationGroup = revEmailValid.ValidationGroup = (validationGroup ?? String.Empty);
        revEmailValid.ValidationExpression = ValidationHelper.EmailRegExp.ToString();
        revEmailValid.ErrorMessage = GetString(errorMessageResourceString ?? "EmailInput.ValidationError");

        return this;
    }


    /// <summary>
    /// Registers another email validator for client side validation by setting <see cref="BaseValidator.ControlToValidate"/> property.
    /// </summary>
    /// <param name="validator">Validator to register</param>
    public CMSFormControls_Inputs_EmailInput RegisterCustomValidator(BaseValidator validator)
    {
        validator.ControlToValidate = txtEmailInput.GetUniqueIDRelativeTo(validator);

        return this;
    }


    /// <summary>
    /// Sets length of the field according to <see cref="AllowMultipleAddresses"/>
    /// </summary>
    private void SetInputMaximumLength()
    {
        txtEmailInput.MaxLength = AllowMultipleAddresses
            ? ValidationHelper.MULTIPLE_EMAILS_LENGTH
            : ValidationHelper.SINGLE_EMAIL_LENGTH;
    }


    private string GetInputFromEmailTextField()
    {
        var input = txtEmailInput.Text.Trim();
        if (String.IsNullOrEmpty(EmailSeparator))
        {
            return input.Trim(';', ' ');
        }

        return EmailSeparator.Length == 1 ? input.Trim(EmailSeparator.First(), ' ') : input;
    }

    #endregion
}