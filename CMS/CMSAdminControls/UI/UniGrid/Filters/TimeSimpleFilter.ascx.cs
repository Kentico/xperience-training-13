using System;
using System.ComponentModel;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_UniGrid_Filters_TimeSimpleFilter : CMSUserControl
{
    #region "Private variables"

    private string mColumn = null;
    private bool mShowTime = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the name of the column that is used to create filtering condition.
    /// </summary>
    [Browsable(true)]
    [Category("Data")]
    [Description("Determines the name of the column that is used to create filtering condition")]
    public string Column
    {
        get
        {
            return mColumn;
        }
        set
        {
            mColumn = value;
        }
    }


    /// <summary>
    /// Enables/disables showing time.
    /// </summary>
    public bool ShowTime
    {
        get
        {
            return mShowTime;
        }
        set
        {
            mShowTime = value;
        }
    }


    /// <summary>
    /// Value of the first textbox - from time.
    /// </summary>
    public DateTime ValueFromTime
    {
        get
        {
            return dtmTimeFrom.SelectedDateTime;
        }
        set
        {
            dtmTimeFrom.SelectedDateTime = value;
        }
    }


    /// <summary>
    /// Value of the second textbox - to time.
    /// </summary>
    public DateTime ValueToTime
    {
        get
        {
            return dtmTimeTo.SelectedDateTime;
        }
        set
        {
            dtmTimeTo.SelectedDateTime = value;
        }
    }


    /// <summary>
    /// Clears both text fields.
    /// </summary>
    public bool Clear()
    {
        dtmTimeTo.SelectedDateTime = DateTimeHelper.ZERO_TIME;
        dtmTimeFrom.SelectedDateTime = DateTimeHelper.ZERO_TIME;
        return true;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        dtmTimeFrom.EditTime = ShowTime;
        dtmTimeTo.EditTime = ShowTime;
    }


    #region "Public methods"

    public string GetCondition()
    {
        string resultCondition = null;

        // Make sure that fromTime precedes toTime
        DateTime fromTime = dtmTimeFrom.SelectedDateTime;
        DateTime toTime = dtmTimeTo.SelectedDateTime;
        if ((fromTime != DateTimeHelper.ZERO_TIME) && (toTime != DateTimeHelper.ZERO_TIME))
        {
            if (fromTime > toTime)
            {
                DateTime tmp = fromTime;
                fromTime = toTime;
                toTime = tmp;
            }
        }

        // Apply fromTime limit
        if (fromTime != DateTimeHelper.ZERO_TIME)
        {
            resultCondition = mColumn + " >= '" + fromTime.ToString("s") + "'";
        }


        // Apply toTime limit
        if (toTime != DateTimeHelper.ZERO_TIME)
        {
            if (!String.IsNullOrEmpty(resultCondition))
            {
                resultCondition += " AND ";
            }
            resultCondition += mColumn + " <= '" + toTime.ToString("s") + "'";
        }

        return resultCondition;
    }

    #endregion
}