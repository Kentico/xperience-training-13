using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;

public partial class CMSFormControls_Filters_DateTimeFilter : FormEngineUserControl
{
    protected string mSecondDateFieldName;


    #region "Properties"

    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (dtmTimeFrom.SelectedDateTime == DateTimeHelper.ZERO_TIME)
            {
                return null;
            }
            
            return dtmTimeFrom.SelectedDateTime;
        }
        set
        {
            if (GetValue("timezonetype") != null)
            {
                dtmTimeFrom.TimeZone = EnumStringRepresentationExtensions.ToEnum<TimeZoneTypeEnum>(GetValue("timezonetype", String.Empty));
            }
            if (GetValue("timezone") != null)
            {
                dtmTimeFrom.CustomTimeZone = CMS.Globalization.TimeZoneInfo.Provider.Get(GetValue("timezone", ""));
            }

            string strValue = ValidationHelper.GetString(value, "");

            dtmTimeFrom.SelectedDateTime = DateTimeHelper.IsNowOrToday(strValue) ? DateTime.Now : ValidationHelper.GetDateTimeSystem(value, DateTimeHelper.ZERO_TIME);
        }
    }


    /// <summary>
    /// Gets or sets if calendar control enables to edit time.
    /// </summary>
    public bool EditTime
    {
        get
        {
            return dtmTimeFrom.EditTime;
        }
        set
        {
            dtmTimeFrom.EditTime = value;
            dtmTimeTo.EditTime = value;
        }
    }


    /// <summary>
    /// Gets name of the field for second date value. Default value is '{FieldName}SecondDatetime' where {FieldName} is name of the current field.
    /// </summary>
    protected string SecondDateFieldName
    {
        get
        {
            if (string.IsNullOrEmpty(mSecondDateFieldName))
            {
                // Get name of the field for second date value
                mSecondDateFieldName = DataHelper.GetNotEmpty(GetValue("SecondDateFieldName"), Field + "SecondDatetime");
            }
            return mSecondDateFieldName;
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        CheckFieldEmptiness = false;
        InitFilterControls();

        LoadOtherValues();
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control.
    /// </summary>
    public override void LoadOtherValues()
    {
        // User defined extensions
        dtmTimeTo.SelectedDateTime = ValidationHelper.GetDateTime(GetColumnValue(SecondDateFieldName), DateTimeHelper.ZERO_TIME, CultureHelper.EnglishCulture);
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        if (Form.Data is DataRowContainer)
        {
            if (!ContainsColumn(SecondDateFieldName))
            {
                Form.DataRow.Table.Columns.Add(SecondDateFieldName);
            }
        }

        // Set properties names
        object[,] values = new object[3, 2];
        values[0, 0] = SecondDateFieldName;
        values[0, 1] = dtmTimeTo.SelectedDateTime;
        return values;
    }


    /// <summary>
    /// Initializes filter calendar controls.
    /// </summary>
    private void InitFilterControls()
    {
        if (FieldInfo != null)
        {
            dtmTimeFrom.AllowEmptyValue = FieldInfo.AllowEmpty;
            dtmTimeTo.AllowEmptyValue = FieldInfo.AllowEmpty;
        }

        dtmTimeFrom.DisplayNow = dtmTimeTo.DisplayNow = ValidationHelper.GetBoolean(GetValue("displaynow"), true);
        dtmTimeFrom.EditTime = dtmTimeTo.EditTime = ValidationHelper.GetBoolean(GetValue("edittime"), EditTime);
        dtmTimeFrom.IsLiveSite = dtmTimeTo.IsLiveSite = IsLiveSite;
        dtmTimeFrom.DateTimeTextBox.AddCssClass("EditingFormCalendarTextBox");
        dtmTimeTo.DateTimeTextBox.AddCssClass("EditingFormCalendarTextBox");

        if (!String.IsNullOrEmpty(CssClass))
        {
            dtmTimeFrom.AddCssClass(CssClass);
            dtmTimeTo.AddCssClass(CssClass);
            CssClass = null;
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            dtmTimeFrom.Attributes.Add("style", ControlStyle);
            dtmTimeTo.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Check value
        string strValueFrom = dtmTimeFrom.DateTimeTextBox.Text.Trim();
        string strValueTo = dtmTimeTo.DateTimeTextBox.Text.Trim();
        bool required = (FieldInfo != null) && !FieldInfo.AllowEmpty;
        bool checkEmptiness = (Form == null) || Form.CheckFieldEmptiness;

        if (required && checkEmptiness && (String.IsNullOrEmpty(strValueFrom) && String.IsNullOrEmpty(strValueTo)))
        {
            // Empty error
            if ((ErrorMessage != null) && !ErrorMessage.EqualsCSafe(ResHelper.GetString("BasicForm.InvalidInput"), true))
            {
                ValidationError = ErrorMessage;
            }
            else
            {
                ValidationError += ResHelper.GetString("BasicForm.ErrorEmptyValue");
            }
            return false;
        }

        bool checkForEmptiness = (required && checkEmptiness) || (!String.IsNullOrEmpty(strValueTo) && !String.IsNullOrEmpty(strValueFrom));

        if (checkForEmptiness)
        {
            if (!ValidationHelper.IsDateTime(strValueTo) || !ValidationHelper.IsDateTime(strValueFrom))
            {
                if (dtmTimeFrom.EditTime)
                {
                    // Error invalid DateTime
                    ValidationError += String.Format("{0} {1}.", ResHelper.GetString("BasicForm.ErrorInvalidDateTime"), DateTime.Now);
                }
                else
                {
                    // Error invalid date
                    ValidationError += String.Format("{0} {1}.", ResHelper.GetString("BasicForm.ErrorInvalidDate"), DateTime.Today.ToString("d"));
                }

                return false;
            }
        }

        if (!dtmTimeFrom.IsValidRange() || !dtmTimeTo.IsValidRange())
        {
            ValidationError += GetString("general.errorinvaliddatetimerange");
            return false;
        }

        return true;
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        EnsureChildControls();

        DateTime dateFrom = dtmTimeFrom.SelectedDateTime;
        DateTime dateTo = dtmTimeTo.SelectedDateTime;
        string fieldName = (FieldInfo != null) ? FieldInfo.Name : Field;
        string where = null;

        if (String.IsNullOrEmpty(WhereConditionFormat))
        {
            // Get default where condition
            WhereCondition condition = null;

            if (dateFrom != DateTimeHelper.ZERO_TIME)
            {
                condition = new WhereCondition(fieldName, QueryOperator.GreaterOrEquals, dateFrom);
            }

            if (dateTo != DateTimeHelper.ZERO_TIME)
            {
                if (condition == null)
                {
                    condition = new WhereCondition();
                }

                condition.And().WhereLessOrEquals(fieldName, dateTo);
            }

            if (condition != null)
            {
                where = condition.ToString(true);
            }
        }
        else
        {
            if (dateFrom != DateTimeHelper.ZERO_TIME)
            {
                // Add "from date" where condition
                where = String.Format(WhereConditionFormat, fieldName, dateFrom.ToString(CultureHelper.EnglishCulture), ">=");
            }

            if (dateTo != DateTimeHelper.ZERO_TIME)
            {
                // Add "to date" where condition
                where = SqlHelper.AddWhereCondition(where, String.Format(WhereConditionFormat, fieldName, dateTo.ToString(CultureHelper.EnglishCulture), "<="));
            }
        }

        return where;
    }

    #endregion
}