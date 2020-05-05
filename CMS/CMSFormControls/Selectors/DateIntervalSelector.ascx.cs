using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Selectors_DateIntervalSelector : FormEngineUserControl
{
    #region "Public properties"

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
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return GetValue();
        }
        set
        {
            SetValue(ValidationHelper.GetInteger(value, 0));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (radLast.Checked && ValidationHelper.GetInteger(txtLast.Text, -1) < 0)
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// Returns selected interval (0=current day, -1=current week, -2=current month, >0 number of days)
    /// </summary>
    private int GetValue()
    {
        int result = 0;
        if (radCurrentWeek.Checked)
        {
            result = -1;
        }
        else if (radCurrentMonth.Checked)
        {
            result = -2;
        }
        else if (radLast.Checked)
        {
            result = ValidationHelper.GetInteger(txtLast.Text, 0);
        }

        return result;
    }


    /// <summary>
    /// Sets value
    /// </summary>
    /// <param name="i">Value</param>
    private void SetValue(int i)
    {
        radToday.Checked = (i == 0);
        radCurrentWeek.Checked = (i == -1);
        radCurrentMonth.Checked = (i == -2);
        radLast.Checked = (i > 0);
        if (radLast.Checked)
        {
            txtLast.Text = i.ToString();
        }
        else
        {
            txtLast.Text = "";
        }
    }

    #endregion
}