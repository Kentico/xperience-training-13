using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Filters_BooleanFilter : FormEngineUserControl
{
    #region "Variables"

    private string mSelectedValue = string.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpConditionValue.SelectedValue;
        }
        set
        {
            if (ValidationHelper.IsBoolean(value))
            {
                mSelectedValue = ValidationHelper.GetBoolean(value, false) ? "1" : "0";
            }
            else
            {
                mSelectedValue = string.Empty;
            }

            drpConditionValue.SelectedValue = mSelectedValue;
        }
    }
    
    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        CheckFieldEmptiness = false;
        InitFilterDropDown();
    }


    /// <summary>
    /// Initializes filter drop-down list.
    /// </summary>
    private void InitFilterDropDown()
    {
        if (drpConditionValue.Items.Count == 0)
        {
            drpConditionValue.Items.Add(new ListItem(GetString("general.selectall"), string.Empty));
            drpConditionValue.Items.Add(new ListItem(GetString("general.yes"), "1"));
            drpConditionValue.Items.Add(new ListItem(GetString("general.no"), "0"));
            drpConditionValue.SelectedValue = mSelectedValue;
        }
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        EnsureChildControls();

        string tempVal = ValidationHelper.GetString(Value, null);

        // Only boolean value
        if (string.IsNullOrEmpty(tempVal) || !ValidationHelper.IsBoolean(tempVal))
        {
            return null;
        }

        bool value = ValidationHelper.GetBoolean(tempVal, false);

        if (String.IsNullOrEmpty(WhereConditionFormat))
        {
            // Return default where condition
            return new WhereCondition(FieldInfo.Name, QueryOperator.Equals, value).ToString(true);
        }

        try
        {
            // Return custom where condition
            return String.Format(WhereConditionFormat, FieldInfo.Name, value ? "1" : "0", "=");
        }
        catch (Exception ex)
        {
            // Log exception
            Service.Resolve<IEventLogService>().LogException("BooleanFilter", "GetWhereCondition", ex);
        }

        return null;
    }

    #endregion
}