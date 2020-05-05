using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.StrandsRecommender;
using CMS.UIControls;

using CultureInfo = System.Globalization.CultureInfo;


/// <summary>
/// Form control used to specify how often Strands Recommender should download a XML feed containing products stored in CMS. 
/// This form control should not be used elsewhere, because its side effect is that it connects to the event of the User control
/// encapsulating settings keys and thus it depends on it being placed in settings.
/// </summary>
public partial class CMSModules_StrandsRecommender_FormControls_StrandsCatalogUploadFrequency : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets state enable.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return ddlMainFrequency.Enabled;
        }
        set
        {
            ddlMainFrequency.Enabled = value;
            ddlHourlyExtendedFrequency.Enabled = value;
            ddlDailyExtendedFrequency.Enabled = value;
            ddlWeeklyExtendedFrequency.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or set the upload frequency. The frequency is represented in this form:
    /// d0 - d23 (daily at 0h - daily at 23h)
    /// w0 - w6 (weekly mondays - weekly sundays)
    /// h1 - h12 (every hour - every 12 hours)
    /// </summary>
    public override object Value
    {
        get
        {
            // Join values from the main drop down and extended one
            return ddlMainFrequency.SelectedValue + GetExtendedDropDownList(ddlMainFrequency.SelectedValue).SelectedValue;
        }
        set
        {
            EnsureChildControls();

            // h14 for example
            string frequency = value.ToString();
            // h for example
            string mainFrequency = frequency[0].ToString();
            // 14 for example
            string extendedFrequency = frequency.Substring(1);

            SelectMainFrequency(mainFrequency);
            SelectExtendedFrequency(mainFrequency, extendedFrequency);
            DisplayCorrectExtendedDropDownList();
        }
    }


    /// <summary>
    /// Determines whether the server control contains child controls. Inserts all items in the dropdowns if they are not inserted yet.
    /// </summary>
    protected override void EnsureChildControls()
    {
        if (ddlMainFrequency.Items.Count == 0)
        {
            // Fill "1 hour" - "12 hours"
            for (int hour = 1; hour <= 12; hour++)
            {
                string resourceString = "strands.uploadfrequency." + (hour == 1 ? "hour" : "hours");
                ddlHourlyExtendedFrequency.Items.Add(new ListItem(hour + " " + GetString(resourceString), hour.ToString()));
            }

            CultureInfo culture = new CultureInfo(LocalizationContext.CurrentUICulture.CultureCode);
            // Fill "12 AM" - "11 PM"
            for (int hour = 0; hour < 24; hour++)
            {
                DateTime dt = new DateTime(1, 1, 1, hour, 0, 0);
                ddlDailyExtendedFrequency.Items.Add(new ListItem(dt.ToString("h tt", culture), hour.ToString()));
            }

            // Fill "Mondays" - "Sundays"
            for (int weekday = 0; weekday < 7; weekday++)
            {
                ddlWeeklyExtendedFrequency.Items.Add(new ListItem(GetString("strands.uploadfrequency.weekdaynames." + weekday), weekday.ToString()));
            }

            ddlMainFrequency.Items.Add(new ListItem(GetString("strands.uploadfrequency.hourly"), "h"));
            ddlMainFrequency.Items.Add(new ListItem(GetString("strands.uploadfrequency.daily"), "d"));
            ddlMainFrequency.Items.Add(new ListItem(GetString("strands.uploadfrequency.weekly"), "w"));
        }
    }


    public void Page_Init(object sender, EventArgs e)
    {
        // When user selects item in the first dropdown (hourly, daily, weekly), display correct second drop down
        ddlMainFrequency.SelectedIndexChanged += (s, ea) => DisplayCorrectExtendedDropDownList();

        // Bind to the event when settings page is being saved and perform a Strands setup call after page is saved
        SettingsGroupViewerControl settingsGroupViewer = ControlsHelper.GetParentControl<SettingsGroupViewerControl>(this);
        // Do nothing if this form control is not placed under SettingsGroupViewerControl control (can happen in form control preview for example)
        if (settingsGroupViewer != null)
        {
            settingsGroupViewer.SettingsSave.After += (s, ea) =>
            {
                if (StrandsSettings.IsStrandsEnabled(SiteContext.CurrentSiteName))
                {
                    try
                    {
                        var setupCall = new StrandsSetupCall();
                        setupCall.DoWithDefaults();
                    }
                    catch (Exception ex)
                    {
                        Service.Resolve<IEventLogService>().LogException("Strands Recommender", "SETUPCALL", ex);
                    }
                }
            };
        }
    }


    /// <summary>
    /// Selects values in the main frequency drop down list.
    /// </summary>
    /// <param name="mainFrequency">Hourly (h), Daily (d) or Weekly (w)</param>
    private void SelectMainFrequency(string mainFrequency)
    {
        SelectItemIfFound(ddlMainFrequency, mainFrequency);
    }


    /// <summary>
    /// Selects value in the drop down list specified by <paramref name="mainFrequency"/>.
    /// </summary>
    /// <param name="mainFrequency">Hourly (h), Daily (d) or Weekly (w)</param>
    /// <param name="extendedFrequency">1 - 12 for Hourly, 0 - 23 for Daily and 0 - 6 for Weekly</param>
    private void SelectExtendedFrequency(string mainFrequency, string extendedFrequency)
    {
        CMSDropDownList extendedDownList = GetExtendedDropDownList(mainFrequency);
        if (extendedDownList != null)
        {
            SelectItemIfFound(extendedDownList, extendedFrequency);
        }
    }


    /// <summary>
    /// Selects item with specified value in a drop down list if drop down list contains that item.
    /// </summary>
    /// <param name="dropDownList">Drop down list</param>
    /// <param name="value">Value of the item to be selected</param>
    private void SelectItemIfFound(CMSDropDownList dropDownList, string value)
    {
        ListItem mainItem = dropDownList.Items.FindByValue(value);
        if (mainItem != null)
        {
            dropDownList.ClearSelection();
            mainItem.Selected = true;
        }
    }


    /// <summary>
    /// Returns drop down list which extends frequency specified by <paramref name="mainFrequency"/>.
    /// </summary>
    /// <param name="mainFrequency">Hourly (h), Daily (d) or Weekly (w)</param>
    private CMSDropDownList GetExtendedDropDownList(string mainFrequency)
    {
        if (mainFrequency == "h")
        {
            return ddlHourlyExtendedFrequency;
        }
        if (mainFrequency == "d")
        {
            return ddlDailyExtendedFrequency;
        }
        if (mainFrequency == "w")
        {
            return ddlWeeklyExtendedFrequency;
        }
        return null;
    }


    /// <summary>
    /// Displays drop down list which extends frequency selected in main drop down and sets correct preposition (at, on, every).
    /// </summary>
    private void DisplayCorrectExtendedDropDownList()
    {
        string mainFrequency = ddlMainFrequency.SelectedValue;

        ddlHourlyExtendedFrequency.Visible = mainFrequency == "h";
        ddlDailyExtendedFrequency.Visible = mainFrequency == "d";
        ddlWeeklyExtendedFrequency.Visible = mainFrequency == "w";

        // Display PST label only if daily frequency is selected
        lblPSTTimeZone.Visible = mainFrequency == "d";

        if (mainFrequency == "h")
        {
            litExtendedFrequencySpecifier.ResourceString = "strands.uploadfrequency.every";
        }
        else if (mainFrequency == "d")
        {
            litExtendedFrequencySpecifier.ResourceString = "strands.uploadfrequency.at";
        }
        else if (mainFrequency == "w")
        {
            litExtendedFrequencySpecifier.ResourceString = "strands.uploadfrequency.on";
        }
    }
}