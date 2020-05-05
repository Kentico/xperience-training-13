using System;

using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSAdminControls_UI_Selectors_SelectValidity : FormEngineUserControl
{
    #region "Backing fields"

    private string mValidForFieldName = "ValidFor";
    private string mValidUntilFieldName = "ValidUntil";
    private bool mAutoPostBack = false;
    private bool mEnableTimeZones = false;

    #endregion


    #region "Properties - values"

    /// <summary>
    /// Gets or sets the validity period value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return DateTimeHelper.GetValidityString(Validity);
        }
        set
        {
            EnsureChildControls();
            this.Validity = DateTimeHelper.GetValidityEnum(ValidationHelper.GetString(value, null));
        }
    }


    /// <summary>
    /// Gets or sets the validity period value.
    /// </summary>
    public ValidityEnum Validity
    {
        get
        {
            if (radDays.Checked)
            {
                return ValidityEnum.Days;
            }
            else if (radWeeks.Checked)
            {
                return ValidityEnum.Weeks;
            }
            else if (radYears.Checked)
            {
                return ValidityEnum.Years;
            }
            else if (radMonths.Checked)
            {
                return ValidityEnum.Months;
            }
            else
            {
                return ValidityEnum.Until;
            }
        }
        set
        {
            radDays.Checked = (value == ValidityEnum.Days);
            radWeeks.Checked = (value == ValidityEnum.Weeks);
            radMonths.Checked = (value == ValidityEnum.Months);
            radYears.Checked = (value == ValidityEnum.Years);
            radUntil.Checked = (value == ValidityEnum.Until);

            DisableInactiveControl();
        }
    }


    /// <summary>
    /// Gets or sets the "valid for" units.
    /// </summary>
    public int? ValidFor
    {
        get
        {
            if (string.IsNullOrEmpty(txtValidFor.Text))
            {
                return null;
            }

            return ValidationHelper.GetInteger(txtValidFor.Text.Trim(), 0);
        }
        set
        {
            txtValidFor.Text = value.HasValue ? value.ToString() : null;
        }
    }


    /// <summary>
    /// Gets or sets the "valid until" date and time.
    /// </summary>
    public DateTime? ValidUntil
    {
        get
        {
            if (untilDateElem.SelectedDateTime == DateTimeHelper.ZERO_TIME)
            {
                return null;
            }

            return untilDateElem.SelectedDateTime;
        }
        set
        {
            untilDateElem.SelectedDateTime = value.HasValue ? value.Value : DateTimeHelper.ZERO_TIME;
        }
    }


    /// <summary>
    /// Gets or sets the name of the "valid for" form field.
    /// Default value is "ValidFor".
    /// </summary>
    public string ValidForFieldName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ValidForFieldName"), mValidForFieldName);
        }
        set
        {
            SetValue("ValidForFieldName", value);
            mValidForFieldName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the "valid until" form field.
    /// Default value is "ValidUntil".
    /// </summary>
    public string ValidUntilFieldName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ValidUntilFieldName"), mValidUntilFieldName);
        }
        set
        {
            SetValue("ValidUntilFieldName", value);
            mValidUntilFieldName = value;
        }
    }

    #endregion


    #region "Properties - general"

    /// <summary>
    /// Indicates if validation error message is displayed by the control itself.
    /// </summary>
    public bool DisplayErrorMessage
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if post back automatically occurs when validity is changed.
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoPostBack"), mAutoPostBack);
        }
        set
        {
            SetValue("AutoPostBack", value);
            mAutoPostBack = value;
        }
    }


    /// <summary>
    /// Indicates if time zones should be enabled in date time picker.
    /// </summary>
    public bool EnableTimeZones
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableTimeZones"), mEnableTimeZones);
        }
        set
        {
            SetValue("EnableTimeZones", value);
            mEnableTimeZones = value;
        }
    }


    /// <summary>
    /// Automatically disables inactive control. If time interval validity is selected (day, week, month, year)
    /// then time interval text box is enabled and date time picker is disabled and vice versa.
    /// </summary>
    public bool AutomaticallyDisableInactiveControl
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    public event EventHandler OnValidityChanged;

    #endregion


    #region "Lifecycle"

    protected override void CreateChildControls()
    {
        DisableInactiveControl();
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Use time zones for date time picker
        if (EnableTimeZones)
        {
            untilDateElem.EditTime = true;
            untilDateElem.TimeZone = TimeZoneTypeEnum.Custom;
            untilDateElem.CustomTimeZone = TimeZoneHelper.GetTimeZoneInfo(MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
        }

        EnsureChildControls();
        InitByForm();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Set auto postback
        radDays.AutoPostBack = AutoPostBack;
        radWeeks.AutoPostBack = AutoPostBack;
        radMonths.AutoPostBack = AutoPostBack;
        radYears.AutoPostBack = AutoPostBack;
        radUntil.AutoPostBack = AutoPostBack;


        // Display error message if required
        lblError.Text = ErrorMessage;
        lblError.Visible = DisplayErrorMessage && !string.IsNullOrEmpty(lblError.Text);
    }

    #endregion


    #region "Initialization"

    /// <summary>
    /// Attempts to initialize the control by form data.
    /// </summary>
    private void InitByForm()
    {
        if (Form == null)
        {
            return;
        }

        LoadOtherValues();
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        if (ContainsColumn(ValidForFieldName))
        {
            ValidFor = GetColumnValue(ValidForFieldName) as int?;
        }

        if (ContainsColumn(ValidUntilFieldName))
        {
            ValidUntil = GetColumnValue(ValidUntilFieldName) as DateTime?;
        }
    }

    #endregion


    #region "Validation"

    public override bool IsValid()
    {
        return string.IsNullOrEmpty(Validate());
    }


    /// <summary>
    /// Validates values and returns error message if there is any.
    /// </summary>
    public string Validate()
    {
        ErrorMessage = null;

        // Validate valid for multiplier
        if (string.IsNullOrEmpty(ErrorMessage) && !radUntil.Checked)
        {
            if (!ValidationHelper.IsInteger(txtValidFor.Text.Trim()) || !(ValidationHelper.GetInteger(txtValidFor.Text.Trim(), 0) >= 1))
            {
                ErrorMessage = GetString("general.selectvalidity.validforerror");
            }
        }

        // Validate until date and time
        if (string.IsNullOrEmpty(ErrorMessage) && radUntil.Checked)
        {
            if (!string.IsNullOrEmpty(untilDateElem.DateTimeTextBox.Text) && (!untilDateElem.IsValidRange() || !ValidationHelper.IsDateTime(untilDateElem.DateTimeTextBox.Text)))
            {
                ErrorMessage = GetString("general.selectvalidity.validuntilerror");
            }
        }

        return ErrorMessage;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns an array of values of related fields.
    /// The first dimension of the array is the field name and the second dimension is its value.
    /// </summary>
    public override object[,] GetOtherValues()
    {
        EnsureChildControls();

        object[,] values = new object[1, 2];

        // Set only value relevant to actual validity
        if (Validity == ValidityEnum.Until)
        {
            values[0, 0] = ValidUntilFieldName;
            values[0, 1] = ValidUntil;
        }
        else
        {
            values[0, 0] = ValidForFieldName;
            values[0, 1] = ValidFor;
        }

        return values;
    }


    protected void ValidityRadioGroup_CheckedChanged(object sender, EventArgs e)
    {
        DisableInactiveControl();

        if (OnValidityChanged != null)
        {
            OnValidityChanged(this, null);
        }
    }


    /// <summary>
    /// Enables/disables inactive control.
    /// </summary>
    private void DisableInactiveControl()
    {
        if (AutomaticallyDisableInactiveControl && AutoPostBack)
        {
            bool dateSelected = radUntil.Checked;
            txtValidFor.Enabled = !dateSelected;
            untilDateElem.Enabled = dateSelected;
        }
    }

    #endregion
}