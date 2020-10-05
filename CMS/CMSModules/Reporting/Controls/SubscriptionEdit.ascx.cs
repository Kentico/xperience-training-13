using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_Reporting_Controls_SubscriptionEdit : CMSAdminControl
{
    #region "Variables"

    private ReportSubscriptionInfo mReportSubscriptionInfo;
    private DataRow mParameters;
    private bool mCheckLast = true;
    private readonly int mGraphID = QueryHelper.GetInteger("graphID", 0);
    private readonly int mTableID = QueryHelper.GetInteger("tableID", 0);
    private readonly int mValueID = QueryHelper.GetInteger("valueID", 0);
    private readonly string mIntervalStr = QueryHelper.GetString("interval", "none");

    #endregion


    #region "Properties"

    /// <summary>
    /// Get report info object
    /// </summary>
    public ReportInfo Report
    {
        get;
        private set;
    }


    /// <summary>
    /// Indicates if control is in simple mode. This mode is displayed in Web Analytics.
    /// </summary>
    public bool SimpleMode
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Security check
        UseCMSDeskAccessDeniedPage = !IsLiveSite;
        var cui = MembershipContext.AuthenticatedUser;
        bool haveModify = cui.IsAuthorizedPerResource("cms.reporting", "modify");
        if (!(cui.IsAuthorizedPerResource("cms.reporting", "subscribe") || haveModify))
        {
            RedirectToAccessDenied("cms.reporting", "Subscribe");
        }

        // Init validators
        rfvEmail.Text = GetString("om.contact.enteremail");
        rfvSubject.Text = GetString("bizformgeneral.emptyemailsubject");

        var subscriptionId = QueryHelper.GetInteger("SubscriptionID", 0);
        mReportSubscriptionInfo = subscriptionId > 0
            ? ReportSubscriptionInfoProvider.GetReportSubscriptionInfo(QueryHelper.GetInteger("SubscriptionID", 0))
            : new ReportSubscriptionInfo();

        // Set edited object
        EditedObject = mReportSubscriptionInfo;

        int reportID = QueryHelper.GetInteger("ReportID", 0);
        string reportName = QueryHelper.GetString("reportName", String.Empty);
        if (reportID != 0)
        {
            Report = ReportInfoProvider.GetReportInfo(reportID);
        }
        else if (!String.IsNullOrEmpty(reportName))
        {
            Report = ReportInfoProvider.GetReportInfo(reportName);
        }
        else
        {
            if (mReportSubscriptionInfo.ReportSubscriptionID > 0)
            {
                // If no report specified, get it from subscription
                Report = ReportInfoProvider.GetReportInfo(mReportSubscriptionInfo.ReportSubscriptionReportID);
            }
        }

        if ((mReportSubscriptionInfo.ReportSubscriptionID > 0) && !haveModify && !SimpleMode)
        {
            if (mReportSubscriptionInfo.ReportSubscriptionUserID != cui.UserID)
            {
                RedirectToAccessDenied(GetString("reportsubscription.onlymodifyusersallowed"));
            }
        }

        if (Report == null)
        {
            return;
        }

        // Set interval control for subscription mode
        ucInterval.DisplayStartTime = false;
        ucInterval.DisplayOnce = false;
        ucInterval.DisplaySecond = false;

        if (!RequestHelper.IsPostBack())
        {
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.hours"), "hour"));
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.days"), "day"));
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.weeks"), "week"));
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.months"), "month"));
            drpLast.Items.Add(new ListItem(GetString("reportsubscription.years"), "year"));
        }

        if (SimpleMode)
        {
            LoadAdHoc();
        }
        else
        {
            LoadData();
        }

        // Show save changes after new object created
        if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
        {
            ShowChangesSaved();
        }

        base.OnInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        rbAll.Text = GetString("reportsubscription.available");
        rbTime.Text = GetString("reportsubscription.time");

        // Register show/hide JS for limit date checkbox
        String script = @"
function disableLast(disable) { 
    document.getElementById ('" + txtLast.ClientID + @"').disabled = disable; 
    document.getElementById ('" + drpLast.ClientID + @"').disabled = disable; 
}";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "TypeDisable", ScriptHelper.GetScript(script));

        txtEmail.RegisterCustomValidator(rfvEmail);

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (RequestHelper.IsPostBack())
        {
            mCheckLast = rbTime.Checked;

            // Interval and last are disabled (no value posted from client), recount their init values
            if (rbAll.Checked)
            {
                SetTimeControls();
            }
        }
        else
        {
            // If "FromDate" is present in parameters, "Only chosen period" option should be selected as time range
            mCheckLast = mReportSubscriptionInfo.ReportSubscriptionParameters.IndexOf("FromDate", StringComparison.OrdinalIgnoreCase) != -1;
        }

        rbTime.Checked = mCheckLast;
        rbAll.Checked = !mCheckLast;
        drpLast.Enabled = mCheckLast;
        txtLast.Enabled = mCheckLast;

        base.OnPreRender(e);
    }


    /// <summary>
    /// Saves the control. Returns false, if error occurred.
    /// </summary>
    public bool Save()
    {
        // Validates input data
        String error = ValidateData();
        if (!String.IsNullOrEmpty(error))
        {
            ShowError(error);
            return false;
        }

        if (Report != null)
        {
            bool isNew = false;
            if (mReportSubscriptionInfo.ReportSubscriptionID <= 0)
            {
                // Insert mode - initialize reportSubscriptionID
                mReportSubscriptionInfo.ReportSubscriptionUserID = MembershipContext.AuthenticatedUser.UserID;
                mReportSubscriptionInfo.ReportSubscriptionSiteID = SiteContext.CurrentSiteID;
                mReportSubscriptionInfo.ReportSubscriptionSettings["ReportInterval"] = mIntervalStr;
                isNew = true;
            }

            if (!SimpleMode)
            {
                // Save basic form & validates basic form data
                if (!formElem.SaveData(null))
                {
                    return false;
                }

                // Store all parameters in basic form to string XML representation
                mParameters = formElem.DataRow;
                mReportSubscriptionInfo.ReportSubscriptionValueID = 0;
                mReportSubscriptionInfo.ReportSubscriptionTableID = 0;
                mReportSubscriptionInfo.ReportSubscriptionGraphID = 0;

                // If subscription is not for whole report, store item ID 
                string drpValue = drpItems.SelectedValue;
                if (drpValue != "all")
                {
                    int id = ValidationHelper.GetInteger(drpValue.Substring(1), 0);
                    if (drpValue.Contains('g'))
                    {
                        mReportSubscriptionInfo.ReportSubscriptionGraphID = id;
                    }
                    if (drpValue.Contains('t'))
                    {
                        mReportSubscriptionInfo.ReportSubscriptionTableID = id;
                    }
                    if (drpValue.Contains('v'))
                    {
                        mReportSubscriptionInfo.ReportSubscriptionValueID = id;
                    }
                }
            }
            else
            {
                mReportSubscriptionInfo.ReportSubscriptionGraphID = mGraphID;
                mReportSubscriptionInfo.ReportSubscriptionTableID = mTableID;
                mReportSubscriptionInfo.ReportSubscriptionValueID = mValueID;
            }

            if (mParameters != null)
            {
                // Find special 'from' and 'to' parameters. 
                DataColumn dcFrom = mParameters.Table.Columns["fromdate"];
                DataColumn dcTo = mParameters.Table.Columns["todate"];

                if (rbTime.Checked)
                {
                    if (dcTo != null)
                    {
                        // Convert column from datetime to string to enable store macros
                        mParameters.Table.Columns.Remove(dcTo.ColumnName);
                        mParameters.Table.Columns.Add(dcTo.ColumnName, typeof(String));

                        // Add current date time macro
                        mParameters[dcTo.ColumnName] = "{%CurrentDateTime%}";
                    }

                    // Create right macro datetime command based on given interval.
                    String dateAddCommand = String.Empty;
                    switch (drpLast.SelectedValue)
                    {
                        case "hour":
                            dateAddCommand = "AddHours";
                            break;

                        case "day":
                            dateAddCommand = "AddDays";
                            break;

                        case "week":
                            dateAddCommand = "AddWeeks";
                            break;

                        case "month":
                            dateAddCommand = "AddMonths";
                            break;

                        case "year":
                            dateAddCommand = "AddYears";
                            break;
                    }

                    // Create todate macro
                    int lastXTimeUnits = ValidationHelper.GetInteger(txtLast.Text.Trim(), 0);
                    String dateMacro = String.Format("{{%CurrentDateTime.{0}({1})%}}", dateAddCommand, -lastXTimeUnits);

                    // Convert fromdate to string
                    if (dcFrom != null)
                    {
                        mParameters.Table.Columns.Remove(dcFrom.ColumnName);
                        mParameters.Table.Columns.Add(dcFrom.ColumnName, typeof(String));
                        mParameters[dcFrom.ColumnName] = dateMacro;
                    }
                }
                else
                {
                    // Empty fromdate and todate for uncheck limit date
                    if (dcFrom != null)
                    {
                        mParameters[dcFrom.ColumnName] = DBNull.Value;
                    }

                    if (dcTo != null)
                    {
                        mParameters[dcTo.ColumnName] = DBNull.Value;
                    }
                }

                // Write parameters to XML string representation
                mReportSubscriptionInfo.ReportSubscriptionParameters = ReportHelper.WriteParametersToXml(mParameters);
            }

            String email = txtEmail.Text.Trim();
            bool emailChanged = mReportSubscriptionInfo.ReportSubscriptionEmail != email;

            mReportSubscriptionInfo.ReportSubscriptionEnabled = chkEnabled.Checked;
            mReportSubscriptionInfo.ReportSubscriptionReportID = Report.ReportID;
            mReportSubscriptionInfo.ReportSubscriptionInterval = ucInterval.ScheduleInterval;
            mReportSubscriptionInfo.ReportSubscriptionEmail = email;
            mReportSubscriptionInfo.ReportSubscriptionSubject = txtSubject.Text;
            mReportSubscriptionInfo.ReportSubscriptionCondition = ucMacroEditor.Text;
            mReportSubscriptionInfo.ReportSubscriptionOnlyNonEmpty = chkNonEmpty.Checked;
            mReportSubscriptionInfo.ReportSubscriptionNextPostDate = SchedulingHelper.GetFirstRunTime(ucInterval.TaskInterval);

            ReportSubscriptionInfoProvider.SetReportSubscriptionInfo(mReportSubscriptionInfo);

            // Check whether email changed (applies for new subscription also)
            if (emailChanged)
            {
                String siteName = SiteContext.CurrentSiteName;

                EmailTemplateInfo eti = EmailTemplateInfo.Provider.Get("Reporting_Subscription_information", SiteContext.CurrentSiteID);
                if (eti != null)
                {
                    // Send information email
                    EmailMessage em = new EmailMessage();
                    em.EmailFormat = EmailFormatEnum.Default;
                    em.From = EmailHelper.Settings.NotificationsSenderAddress(siteName);
                    em.Recipients = email;

                    MacroResolver resolver = ReportSubscriptionSender.CreateSubscriptionMacroResolver(Report, mReportSubscriptionInfo, SiteContext.CurrentSite, em.Recipients);
                    EmailSender.SendEmailWithTemplateText(siteName, em, eti, resolver, false);
                }
            }

            // For new item and advanced mode redirect to store ID in query string
            if ((isNew) && (!SimpleMode))
            {
                URLHelper.Redirect(RequestContext.CurrentURL + "&saved=1&subscriptionid=" + mReportSubscriptionInfo.ReportSubscriptionID);
            }

            ShowChangesSaved();
            return true;
        }

        return false;
    }


    /// <summary>
    /// Sets time controls (dropdown with interval and textbox with interval value). Returns true if time controls are to be hided.
    /// </summary>
    private bool SetTimeControls()
    {
        HitsIntervalEnum interval = HitsIntervalEnumFunctions.StringToHitsConversion(mIntervalStr);

        DateTime from = DateTimeHelper.ZERO_TIME;
        DateTime to = DateTimeHelper.ZERO_TIME;

        object dcFrom = null;
        object dcTo = null;

        if (mParameters != null)
        {
            // Load fromdate and todate from report parameters (passed from query string)
            dcFrom = mParameters.Table.Columns["FromDate"];
            dcTo = mParameters.Table.Columns["ToDate"];

            if (dcFrom != null)
            {
                from = ValidationHelper.GetDateTime(mParameters["FromDate"], DateTimeHelper.ZERO_TIME);
            }

            if (dcTo != null)
            {
                to = ValidationHelper.GetDateTime(mParameters["ToDate"], DateTimeHelper.ZERO_TIME);
            }
        }

        // If one contains zero time, set all time radio button. In such situation, report can maintain unlimited fromdate or todate.
        if ((from == DateTimeHelper.ZERO_TIME) || (to == DateTimeHelper.ZERO_TIME))
        {
            mCheckLast = false;
        }

        // If one is not set, hide limitdata panel 
        if ((dcFrom == null) || (dcTo == null))
        {
            ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_DAY;
            return true;
        }

        int diff = 0;
        bool noAddToDiff = false;

        // If interval is not known, but 'from' and 'to' is set (f.e. preview, webpart,..) - compute interval from date values
        if (interval == HitsIntervalEnum.None)
        {
            string sFrom = ValidationHelper.GetString(mParameters["FromDate"], string.Empty).ToLowerInvariant();
            string sTo = ValidationHelper.GetString(mParameters["ToDate"], string.Empty).ToLowerInvariant();
            mCheckLast = true;

            if (MacroProcessor.ContainsMacro(sFrom) && MacroProcessor.ContainsMacro(sTo))
            {
                if (sFrom.Contains("addhours"))
                {
                    interval = HitsIntervalEnum.Hour;
                }
                else if (sFrom.Contains("adddays"))
                {
                    interval = HitsIntervalEnum.Day;
                }
                else if (sFrom.Contains("addweeks"))
                {
                    interval = HitsIntervalEnum.Week;
                }
                else if (sFrom.Contains("addmonths"))
                {
                    interval = HitsIntervalEnum.Month;
                }
                else if (sFrom.Contains("addyears"))
                {
                    interval = HitsIntervalEnum.Year;
                }

                var macroResolverSettings = new MacroSettings
                {
                    AvoidInjection = false,
                    Culture = CultureHelper.EnglishCulture.Name
                };
                to = DateTime.Now;
                from = ValidationHelper.GetDateTime(MacroResolver.Resolve(sFrom, macroResolverSettings), DateTime.Now, macroResolverSettings.Culture);
                noAddToDiff = true;
            }
            else if ((from != DateTimeHelper.ZERO_TIME) && (to != DateTimeHelper.ZERO_TIME))
            {
                // Set interval as greatest possible interval (365+ days -> years, 30+days->months ,...)
                diff = (int)(to - from).TotalDays;
                if (diff >= 365)
                {
                    interval = HitsIntervalEnum.Year;
                }
                else if (diff >= 30)
                {
                    interval = HitsIntervalEnum.Month;
                }
                else if (diff >= 7)
                {
                    interval = HitsIntervalEnum.Week;
                }
                else if (diff >= 1)
                {
                    interval = HitsIntervalEnum.Day;
                }
                else
                {
                    interval = HitsIntervalEnum.Hour;
                }
            }
        }

        // Set default period and diff based on interval
        switch (interval)
        {
            case HitsIntervalEnum.Year:
                diff = to.Year - from.Year;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_MONTH;
                break;

            case HitsIntervalEnum.Month:
                diff = ((to.Year - from.Year) * 12) + to.Month - from.Month;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_MONTH;
                break;

            case HitsIntervalEnum.Week:
                diff = (int)(to - from).TotalDays / 7;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_WEEK;
                break;

            case HitsIntervalEnum.Day:
                diff = (int)(to - from).TotalDays;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_DAY;
                break;

            case HitsIntervalEnum.Hour:
                diff = (int)(to - from).TotalHours;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_HOUR;
                break;

            case HitsIntervalEnum.None:
                mCheckLast = false;
                break;
        }

        // Add current 
        if (!noAddToDiff)
        {
            diff++;
        }

        if (interval != HitsIntervalEnum.None)
        {
            drpLast.SelectedValue = HitsIntervalEnumFunctions.HitsConversionToString(interval);
        }

        if (!mCheckLast)
        {
            // Defaul settings for no time
            ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_DAY;
        }

        if (diff != 0)
        {
            txtLast.Text = diff.ToString();
        }

        return false;
    }


    /// <summary>
    /// Load data for simple mode
    /// </summary>
    private void LoadAdHoc()
    {
        // Hide controls for advanced mode
        pnlParametersEnvelope.Visible = false;
        pnlSubscription.Visible = false;
        pnlEnabled.Visible = false;

        if (Report != null)
        {
            GenerateInfoText();

            // Get report parameters
            string parameters = QueryHelper.GetString("parameters", String.Empty);
            mParameters = ReportHelper.GetReportParameters(Report, parameters, ReportHelper.PARAM_SEMICOLON, CultureHelper.EnglishCulture);

            if (!RequestHelper.IsPostBack())
            {
                // Create subject from report name
                txtSubject.Text = ResHelper.LocalizeString(Report.ReportDisplayName);

                // Fill dropdown with intervals
                txtEmail.Text = MembershipContext.AuthenticatedUser.Email;

                ucInterval.ScheduleInterval = String.Empty;
                chkNonEmpty.Checked = true;
                chkEnabled.Checked = true;

                pnlFromToParams.Visible = !SetTimeControls();
            }
        }
    }


    /// <summary>
    /// Displays info message containing report and object to which user has been subscribed.
    /// </summary>
    private void GenerateInfoText()
    {
        String type = String.Empty;
        if (mGraphID != 0)
        {
            type = GetString("ReportItemType.graph");
        }
        if (mTableID != 0)
        {
            type = GetString("ReportItemType.table");
        }
        if (mValueID != 0)
        {
            type = GetString("ReportItemType.value");
        }

        if (!String.IsNullOrEmpty(type))
        {
            ShowInformation(String.Format(GetString("reportsubscription.infotitleitem"), type.ToLowerInvariant(), ResHelper.LocalizeString(Report.ReportDisplayName)));
        }
        else
        {
            ShowInformation(String.Format("{0} <strong>{1}</strong>.", GetString("reportsubscription.infotitle"), ResHelper.LocalizeString(Report.ReportDisplayName)));
        }
    }


    /// <summary>
    /// Loads data for advanced mode
    /// </summary>
    private void LoadData()
    {
        if (!RequestHelper.IsPostBack())
        {
            if (mReportSubscriptionInfo.ReportSubscriptionID > 0)
            {
                // Initial settings
                ucInterval.ScheduleInterval = mReportSubscriptionInfo.ReportSubscriptionInterval;
                chkEnabled.Checked = mReportSubscriptionInfo.ReportSubscriptionEnabled;
                txtEmail.Text = mReportSubscriptionInfo.ReportSubscriptionEmail;
                txtSubject.Text = mReportSubscriptionInfo.ReportSubscriptionSubject;
                chkNonEmpty.Checked = mReportSubscriptionInfo.ReportSubscriptionOnlyNonEmpty;
                ucMacroEditor.Text = mReportSubscriptionInfo.ReportSubscriptionCondition;
            }
            else
            {
                // New item
                chkEnabled.Checked = true;
                txtEmail.Text = MembershipContext.AuthenticatedUser.Email;
                txtSubject.Text = ResHelper.LocalizeString(Report.ReportDisplayName);
                chkNonEmpty.Checked = true;
                ucInterval.DefaultPeriod = SchedulingHelper.PERIOD_DAY;
            }

            FillItems();
        }

        if ((Report != null) && !String.IsNullOrEmpty(Report.ReportParameters) && (Report.ReportParameters != "<form></form>"))
        {
            // Convert field "DateFrom" and "DateTo" to string to be able to display macros
            FormInfo fi = new FormInfo(Report.ReportParameters);
            FormFieldInfo ffi = fi.GetFormField("FromDate");

            if (ffi != null)
            {
                ffi.DataType = FieldDataType.Text;
                ffi.Settings["AllowMacros"] = true;
                ffi.Size = 400;
                ffi.AllowEmpty = true;
                ffi.Visible = false;
            }

            ffi = fi.GetFormField("ToDate");
            if (ffi != null)
            {
                ffi.DataType = FieldDataType.Text;
                ffi.Settings["AllowMacros"] = true;
                ffi.Size = 400;
                ffi.AllowEmpty = true;
                ffi.Visible = false;
            }

            // Get datarow from forminfo (it contains report parameters passed by querystring)
            DataRow defaultValues = fi.GetDataRow(false);

            // Load default values
            fi.LoadDefaultValues(defaultValues, true);

            // Display basic form, only if any item is visible
            pnlParametersEnvelope.Visible = fi.GetFields(true, false).Any();

            // Loop through all nodes is subscription's XML parameters and replace report's value with current subscription's ones.
            if (mReportSubscriptionInfo.ReportSubscriptionID > 0)
            {
                if (!String.IsNullOrEmpty(mReportSubscriptionInfo.ReportSubscriptionParameters))
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(mReportSubscriptionInfo.ReportSubscriptionParameters);
                    foreach (DataColumn col in defaultValues.Table.Columns)
                    {
                        XmlNode node = xml.SelectSingleNode("/Root/" + col.ColumnName);
                        if (node != null)
                        {
                            // In case of different data types use try catch block
                            try
                            {
                                defaultValues[col.ColumnName] = DataHelper.ConvertValue(node.InnerText, col.DataType);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                    }
                }
            }


            mParameters = defaultValues;
            pnlFromToParams.Visible = !SetTimeControls();

            // Set basic form parameters
            formElem.DataRow = defaultValues;
            formElem.SubmitButton.Visible = false;
            formElem.SubmitButton.RegisterHeaderAction = false;
            formElem.FormInformation = fi;
            formElem.SiteName = SiteContext.CurrentSiteName;
            formElem.Mode = FormModeEnum.Update;
            formElem.Visible = true;
        }
        else
        {
            pnlParametersEnvelope.Visible = false;            
            pnlFromToParams.Visible = false;
            mCheckLast = false;
        }
    }


    /// <summary>
    /// Fill items dropdown
    /// </summary>
    private void FillItems()
    {
        drpItems.Items.Add(new ListItem(GetString("reportsubscription.wholereport"), "all"));

        // Fill graphs
        DataSet ds = ReportGraphInfoProvider.GetReportGraphs().WhereEquals("GraphReportID", Report.ReportID);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String displayName = ValidationHelper.GetString(dr["GraphDisplayName"], String.Empty);
                String id = ValidationHelper.GetString(dr["GraphID"], String.Empty);
                drpItems.Items.Add(new ListItem(String.Format("{0} ({1})", displayName, GetString("reporting.graph")), "g" + id));
            }
        }

        // Fill tables
        ds = ReportTableInfoProvider.GetReportTables().WhereEquals("TableReportID", Report.ReportID);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String displayName = ValidationHelper.GetString(dr["TableDisplayName"], String.Empty);
                String id = ValidationHelper.GetString(dr["TableID"], String.Empty);
                drpItems.Items.Add(new ListItem(String.Format("{0} ({1})", displayName, GetString("reporting.table")), "t" + id));
            }
        }

        // Fill values
        ds = ReportValueInfoProvider.GetReportValues().WhereEquals("ValueReportID", Report.ReportID);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String displayName = ValidationHelper.GetString(dr["ValueDisplayName"], String.Empty);
                String id = ValidationHelper.GetString(dr["ValueID"], String.Empty);
                drpItems.Items.Add(new ListItem(String.Format("{0} ({1})", displayName, GetString("reporting.value")), "v" + id));
            }
        }

        // Select value in dropdown based on non empty key in object
        if (mReportSubscriptionInfo.ReportSubscriptionID > 0)
        {
            if (mReportSubscriptionInfo.ReportSubscriptionGraphID != 0)
            {
                drpItems.SelectedValue = "g" + mReportSubscriptionInfo.ReportSubscriptionGraphID;
            }

            if (mReportSubscriptionInfo.ReportSubscriptionTableID != 0)
            {
                drpItems.SelectedValue = "t" + mReportSubscriptionInfo.ReportSubscriptionTableID;
            }

            if (mReportSubscriptionInfo.ReportSubscriptionValueID != 0)
            {
                drpItems.SelectedValue = "v" + mReportSubscriptionInfo.ReportSubscriptionValueID;
            }
        }
    }


    /// <summary>
    /// Validates input data
    /// </summary>
    private String ValidateData()
    {
        String error = String.Empty;
        String email = txtEmail.Text.Trim();

        // Check for valid email
        if ((email == String.Empty) || !txtEmail.IsValid())
        {
            error = GetString("om.contact.enteremail");
        }

        // Check for non empty subject
        if (txtSubject.Text.Trim() == String.Empty)
        {
            error = GetString("bizformgeneral.emptyemailsubject");
        }

        // Check for non empty interval
        if (String.IsNullOrEmpty(ucInterval.ScheduleInterval))
        {
            error = GetString("task_edit.emptyinterval");
        }

        // Check 'Date from last' validity
        if (rbTime.Checked)
        {
            int last = ValidationHelper.GetInteger(txtLast.Text.Trim(), 0);
            if ((last <= 0) || (last > 999))
            {
                error = GetString("reportsubscription.positivenumber");
            }
        }

        return error;
    }

    #endregion
}

