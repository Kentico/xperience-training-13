using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Filters_NumberFilter : FormEngineUserControl
{
    protected string mOperatorFieldName;


    #region "Properties"

    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtText.Text;
        }
        set
        {            
            // Convert the value to a proper type
            value = ConvertInputValue(value);

            txtText.Text = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Gets name of the field for operator value. Default value is '{FieldName}Operator' where {FieldName} is name of the current field.
    /// </summary>
    protected string OperatorFieldName
    {
        get
        {
            if (string.IsNullOrEmpty(mOperatorFieldName))
            {
                // Get name of the field for operator value
                mOperatorFieldName = DataHelper.GetNotEmpty(GetValue("OperatorFieldName"), Field + "Operator");
            }
            return mOperatorFieldName;
        }
    }


    /// <summary>
    /// Gets or sets default operator to use for the first initialization of the control.
    /// </summary>
    public string DefaultOperator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultOperator"), "=");
        }
        set
        {
            SetValue("DefaultOperator", value);
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        CheckFieldEmptiness = false;
        InitFilterDropDown();

        LoadOtherValues();
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control.
    /// </summary>
    public override void LoadOtherValues()
    {
        drpOperator.SelectedValue = ValidationHelper.GetString(GetColumnValue(OperatorFieldName), DefaultOperator);
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        if (Form.Data is DataRowContainer)
        {
            if (!ContainsColumn(OperatorFieldName))
            {
                Form.DataRow.Table.Columns.Add(OperatorFieldName);
            }
        }

        // Set properties names
        object[,] values = new object[3, 2];

        values[0, 0] = OperatorFieldName;
        values[0, 1] = drpOperator.SelectedValue;

        return values;
    }


    /// <summary>
    /// Initializes operator filter drop-down list.
    /// </summary>
    private void InitFilterDropDown()
    {
        ListItemCollection items = drpOperator.Items;
        if (items.Count == 0)
        {
            items.Add(new ListItem(GetString("filter.equals"), "="));
            items.Add(new ListItem(GetString("filter.notequals"), "<>"));
            items.Add(new ListItem(GetString("filter.lessthan"), "<"));
            items.Add(new ListItem(GetString("filter.lessorequal"), "<="));
            items.Add(new ListItem(GetString("filter.greaterthan"), ">"));
            items.Add(new ListItem(GetString("filter.greaterorequal"), ">="));
        }
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        EnsureChildControls();

        var value = ValidationHelper.GetString(Value, String.Empty);
        string op = drpOperator.SelectedValue;
        bool isTimeSpan = false;

        // No condition
        if (String.IsNullOrWhiteSpace(value) || String.IsNullOrEmpty(op) || !(ValidationHelper.IsDouble(value) || (isTimeSpan = ValidationHelper.IsTimeSpan(value))))
        {
            return null;
        }

        // Convert value to default culture format
        value = DataHelper.ConvertValueToDefaultCulture(value, !isTimeSpan ? typeof (double) : typeof (TimeSpan));

        if (String.IsNullOrEmpty(WhereConditionFormat))
        {
            // Get default where condition
            object typedValue;
            if (isTimeSpan)
            {
                typedValue = ValidationHelper.GetTimeSpanSystem(value, TimeSpan.MinValue);
            }
            else
            {
                typedValue = ValidationHelper.GetDoubleSystem(value, Double.NaN);
            }

            return new WhereCondition(FieldInfo.Name, EnumStringRepresentationExtensions.ToEnum<QueryOperator>(op), typedValue).ToString(true);
        }

        try
        {
            // Return custom where condition
            return String.Format(WhereConditionFormat, FieldInfo.Name, value, op);
        }
        catch (Exception ex)
        {
            // Log exception
            Service.Resolve<IEventLogService>().LogException("NumberFilter", "GetWhereCondition", ex);
        }

        return null;
    }

    #endregion
}