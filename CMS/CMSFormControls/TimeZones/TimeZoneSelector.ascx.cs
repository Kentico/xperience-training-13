using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;


public partial class CMSFormControls_TimeZones_TimeZoneSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseZoneNameForSelection = true;
    private bool mAddNoneItemsRecord = true;
    private string mTimeZoneName = "";
    private int mTimeZoneId = 0;

    #endregion


    #region "Methods"

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
            drpTimeZoneSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (UseZoneNameForSelection)
            {
                return TimeZoneName;
            }
            else
            {
                if (TimeZoneID == 0)
                {
                    return null;
                }

                return TimeZoneID;
            }
        }
        set
        {
            if (UseZoneNameForSelection)
            {
                TimeZoneName = ValidationHelper.GetString(value, "");
            }
            else
            {
                TimeZoneID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the TagGroup code name.
    /// </summary>
    public string TimeZoneName
    {
        get
        {
            if (UseZoneNameForSelection)
            {
                return ValidationHelper.GetString(drpTimeZoneSelector.SelectedValue, "");
            }
            else
            {
                TimeZoneInfo tzi = TimeZoneInfoProvider.GetTimeZoneInfo(ValidationHelper.GetInteger(drpTimeZoneSelector.SelectedValue, 0));
                if (tzi != null)
                {
                    return tzi.TimeZoneName;
                }
                return "";
            }
        }
        set
        {
            if (UseZoneNameForSelection)
            {
                SelectValue(value);
                mTimeZoneName = value;
            }
            else
            {
                TimeZoneInfo tzi = TimeZoneInfoProvider.GetTimeZoneInfo(value);
                if (tzi != null)
                {
                    SelectValue(tzi.TimeZoneID.ToString());
                    mTimeZoneId = tzi.TimeZoneID;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets the TagGroup ID.
    /// </summary>
    public int TimeZoneID
    {
        get
        {
            if (UseZoneNameForSelection)
            {
                string name = ValidationHelper.GetString(drpTimeZoneSelector.SelectedValue, "");

                var tzi = TimeZoneInfoProvider.GetTimeZoneInfo(name);
                if (tzi != null)
                {
                    return tzi.TimeZoneID;
                }
                return 0;
            }
            else
            {
                return ValidationHelper.GetInteger(drpTimeZoneSelector.SelectedValue, 0);
            }
        }
        set
        {
            if (UseZoneNameForSelection)
            {
                var tzi = TimeZoneInfoProvider.GetTimeZoneInfo(value);
                if (tzi != null)
                {
                    SelectValue(tzi.TimeZoneName);
                    mTimeZoneName = tzi.TimeZoneName;
                }
            }
            else
            {
                SelectValue(value.ToString());
                mTimeZoneId = value;
            }
        }
    }


    /// <summary>
    ///  If true, selected value is TimeZoneName, if false, selected value is TimeZoneID.
    /// </summary>
    public bool UseZoneNameForSelection
    {
        get
        {
            if ((FieldInfo != null) && (FieldInfo.DataType == FieldDataType.Integer))
            {
                mUseZoneNameForSelection = false;
            }

            return mUseZoneNameForSelection;
        }
        set
        {
            mUseZoneNameForSelection = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddNoneItemsRecord
    {
        get
        {
            return mAddNoneItemsRecord;
        }
        set
        {
            mAddNoneItemsRecord = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the DropDownList with timezone.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpTimeZoneSelector.ClientID;
        }
    }

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// CreateChildControls() override.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        if (!StopProcessing)
        {
            ReloadData();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads public status according to the control settings.
    /// </summary>
    public void ReloadData()
    {
        // Cleanup
        drpTimeZoneSelector.ClearSelection();
        drpTimeZoneSelector.SelectedValue = null;

        // Populate the dropdownlist
        if (UseZoneNameForSelection)
        {
            FillDropdown("TimeZoneName");
        }
        else
        {
            FillDropdown("TimeZoneID");
        }

        // Try to preselect the value
        if (UseZoneNameForSelection)
        {
            SelectValue(mTimeZoneName);
        }
        else
        {
            SelectValue(mTimeZoneId.ToString());
        }
    }


    /// <summary>
    /// Tries to select the specified value in drpTimeZoneSelector.
    /// </summary>
    private void SelectValue(string val)
    {
        try
        {
            drpTimeZoneSelector.SelectedValue = val;
        }
        catch
        {
        }
    }


    /// <summary>
    /// Fill drop down list.
    /// </summary>
    /// <param name="dataValue">Data value field</param>
    private void FillDropdown(string dataValue)
    {
        if (drpTimeZoneSelector.Items.Count == 0)
        {
            DataSet ds = TimeZoneInfoProvider.GetTimeZones().OrderBy("TimeZoneGMT").Columns("TimeZoneID ,TimeZoneGMT, TimeZoneName, TimeZoneDisplayName");

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                string text = null;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    text = String.Format("(UTC{0:+00.00;-00.00}) {1}", row["TimeZoneGMT"], row["TimeZoneDisplayName"]);
                    drpTimeZoneSelector.Items.Add(new ListItem(text, row[dataValue].ToString()));
                }
            }

            // Add none record if needed
            if (AddNoneItemsRecord)
            {
                drpTimeZoneSelector.Items.Insert(0, new ListItem(GetString("General.SelectNone"), ""));
            }
        }
    }

    #endregion
}