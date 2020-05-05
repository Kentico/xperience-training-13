using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Scheduler;


public partial class CMSFormControls_Inputs_DueDateSelector : FormEngineUserControl
{
    public override object Value
    {
        get
        {
            return txtQuantity.Text + ";" + drpScale.SelectedValue;
        }
        set
        {
            EnsureChildControls();

            string str = ValidationHelper.GetString(value, "");
            if (!string.IsNullOrEmpty(str))
            {
                string[] strs = str.Split(';');
                if (strs.Length == 2)
                {
                    txtQuantity.Text = strs[0];
                    try
                    {
                        drpScale.SelectedValue = strs[1];
                    }
                    catch { }
                }
            }
        }
    }


    public override bool IsValid()
    {
        bool isRequired = (FieldInfo != null) && !FieldInfo.AllowEmpty;
        bool isPositiveNumber = ValidationHelper.GetInteger(txtQuantity.Text, 0) > 0;
        bool isEmpty = string.IsNullOrEmpty(txtQuantity.Text);

        if (isRequired)
        {
            // Value cannot be empty and only positive numbers are allowed
            return !isEmpty && isPositiveNumber;
        }

        // Field is not required, both positive number or empty value are allowed
        return isEmpty || isPositiveNumber;
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        if (drpScale.Items.Count == 0)
        {
            drpScale.Items.Add(new ListItem(GetString("timeoutselector.hours"), SchedulingHelper.PERIOD_HOUR));
            drpScale.Items.Add(new ListItem(GetString("timeoutselector.days"), SchedulingHelper.PERIOD_DAY));
            drpScale.Items.Add(new ListItem(GetString("timeoutselector.weeks"), SchedulingHelper.PERIOD_WEEK));
            drpScale.Items.Add(new ListItem(GetString("timeoutselector.months"), SchedulingHelper.PERIOD_MONTH));
            drpScale.Items.Add(new ListItem(GetString("timeoutselector.years"), SchedulingHelper.PERIOD_YEAR));
        }
    }
}