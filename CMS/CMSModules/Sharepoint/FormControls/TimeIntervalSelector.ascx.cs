using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;

/// <summary>
/// FormEngine control to select a period of time
/// </summary>
public partial class CMSModules_SharePoint_FormControls_TimeIntervalSelector : FormEngineUserControl
{
    private const string MINUTES = "minutes";
    private const string HOURS = "hours";

    private bool mDropDownPopulated = false;


    /// <summary>
    /// Gets or sets the value of the control - Integer number of minutes
    /// </summary>
    public override object Value
    {
        get
        {
            return GetValue();
        }
        set
        {
            SetValue(value);
        }
    }


    /// <summary>
    /// Validates the input.
    /// Sets ValidationError if input is not valid.
    /// </summary>
    /// <returns>True if input is valid.</returns>
    public override bool IsValid()
    {
        // Verify the number
        int quantity = ValidationHelper.GetInteger(txtQuantity.Text, -1);
        if (quantity < 0)
        {
            ValidationError = GetString("basicform.errornotinteger");

            return false;
        }

        // Verify that scale wasn't forged
        string scale = drpScale.SelectedValue;
        switch (scale)
        {
            case MINUTES:
            case HOURS:
                return true;

            default:
                return false;
        }
    }


    /// <summary>
    /// Ensures the nested controls are set up properly
    /// </summary>
    /// <param name="sender">Ignored</param>
    /// <param name="e">Ignored</param>
    protected void Page_Init(object sender, EventArgs e)
    {
        EnsureDropDownItems();
    }


    /// <summary>
    /// Fills the drop-down menu with options. Only once.
    /// </summary>
    private void EnsureDropDownItems()
    {
        if (!mDropDownPopulated)
        {
            drpScale.Items.AddRange(new ListItem[] { 
                new ListItem(ResHelper.GetString("TimeIntervalSelector.Scale.Minutes"),MINUTES),
                new ListItem(ResHelper.GetString("TimeIntervalSelector.Scale.Hours"),HOURS),
            });
            mDropDownPopulated = true;
        }
    }


    /// <summary>
    /// Gets the number of minutes
    /// </summary>
    /// <returns>Number of minutes</returns>
    private object GetValue()
    {
        int quantity = ValidationHelper.GetInteger(txtQuantity.Text, -1);
        if (quantity > 0)
        {
            var scale = drpScale.SelectedValue;
            switch (scale)
            {
                case MINUTES:
                    return quantity;

                case HOURS:
                    return TimeSpan.FromHours(quantity).TotalMinutes;

                default:
                    return null;
            }
        }

        return null;
    }


    /// <summary>
    /// Sets nested controls' values to reflect the provided value
    /// </summary>
    /// <param name="value">Number of minutes</param>
    private void SetValue(object value)
    {
        EnsureDropDownItems();
        int minutes = ValidationHelper.GetInteger(value, -1);
        if (minutes < 0)
        {
            return;
        }

        TimeSpan timeSpan = TimeSpan.FromMinutes(minutes);
        if (timeSpan.Minutes == 0)
        {
            drpScale.SelectedValue = HOURS;
            txtQuantity.Text = timeSpan.TotalHours.ToString();
        }
        else
        {
            drpScale.SelectedValue = MINUTES;
            txtQuantity.Text = timeSpan.TotalMinutes.ToString();
        }
    }

}