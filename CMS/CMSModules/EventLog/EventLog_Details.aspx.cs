using System;
using System.Collections;
using System.Text;

using CMS.Base;
using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("EventLogDetails.Header")]
public partial class CMSModules_EventLog_EventLog_Details : CMSEventLogPage
{
    #region "Protected variables"

    protected int prevId = 0;
    protected int nextId = 0;
    protected Hashtable mParameters = null;
    private EventLogInfo mEventInfo = null;
    private const string EOL_REPLACEMENT = "#EOL#";
    private const int DESCRIPTION_MAX_LENGTH = 65536;

    #endregion


    #region "Properties"

    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }


    /// <summary>
    /// EventLogInfo being displayed as a detail.
    /// </summary>
    private EventLogInfo EventInfo
    {
        get
        {
            if (mEventInfo == null)
            {
                mEventInfo = EventLogProvider.GetEventLogInfo(QueryHelper.GetInteger("eventid", 0));
            }
            return mEventInfo;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash", "eventid"))
        {
            CheckPermissions(true);

            if (EventInfo != null)
            {
                // Get EventID value
                LoadData();

                var url = ResolveUrl("GetEventDetail.aspx?eventid=" + EventInfo.EventID);
                if (SiteID > 0)
                {
                    url = URLHelper.AddParameterToUrl(url, "siteid", SiteID.ToString());
                }
                btnExport.OnClientClick = "window.open('" + url + "');";
                btnExport.Visible = true;

                // Show report bug button for global administrator if event is warning or error
                if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && ((EventInfo.EventType == "W") || (EventInfo.EventType == "E")))
                {
                    btnReportBug.Visible = true;
                    btnReportBug.NavigateUrl = ApplicationUIHelper.REPORT_BUG_URL;
                }

                if (Parameters != null)
                {
                    // Get the ORDER BY column and starting event ID
                    string orderBy = ValidationHelper.GetString(Parameters["orderby"], "EventID DESC");

                    // ORDER BY with semicolon is considered to be dangerous
                    if ((orderBy == string.Empty) || (orderBy.IndexOfCSafe(';') >= 0))
                    {
                        orderBy = "EventID DESC";
                    }
                    string whereCondition = ValidationHelper.GetString(Parameters["where"], string.Empty);

                    // Initialize next/previous buttons
                    int[] prevNext = EventLogProvider.GetPreviousNext(EventInfo.EventID, whereCondition, orderBy);
                    if (prevNext != null)
                    {
                        prevId = prevNext[0];
                        nextId = prevNext[1];

                        btnPrevious.Enabled = (prevId != 0);
                        btnNext.Enabled = (nextId != 0);

                        btnPrevious.Click += btnPrevious_Click;
                        btnNext.Click += btnNext_Click;
                    }
                }
                else
                {
                    btnNext.Visible = false;
                    btnPrevious.Visible = false;
                }
            }

            RegisterModalPageScripts();
            RegisterEscScript();
        }
    }

    #endregion


    #region "Button handling"

    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        // Redirect to previous
        URLHelper.Redirect(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "eventId", string.Empty + prevId));
    }


    protected void btnNext_Click(object sender, EventArgs e)
    {
        // Redirect to next
        URLHelper.Redirect(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "eventId", string.Empty + nextId));
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Loads data of specific EventLog from DB.
    /// </summary>
    protected void LoadData()
    {
        EventLogInfo ev = EventInfo;

        //Set edited object
        EditedObject = ev;

        if (ev != null)
        {
            // Rewrite event type text.
            lblEventTypeValue.Text = EventLogHelper.GetEventTypeText(ev.EventType);
            lblEventIDValue.Text = ev.EventID.ToString();
            lblEventTimeValue.Text = ev.EventTime.ToString();
            lblSourceValue.Text = HTMLHelper.HTMLEncode(ev.Source);
            lblEventCodeValue.Text = HTMLHelper.HTMLEncode(ev.EventCode);

            lblUserIDValue.Text = ev.UserID.ToString();
            plcUserID.Visible = (ev.UserID > 0);

            lblUserNameValue.Text = HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(ev.UserName));
            plcUserName.Visible = !string.IsNullOrEmpty(lblUserNameValue.Text);

            lblIPAddressValue.Text = ev.IPAddress;

            lblNodeIDValue.Text = ev.NodeID.ToString();
            plcNodeID.Visible = (ev.NodeID > 0);

            lblNodeNameValue.Text = HTMLHelper.HTMLEncode(ev.DocumentName);
            plcNodeName.Visible = !string.IsNullOrEmpty(lblNodeNameValue.Text);

            // Replace HTML except line endings (<br /> or \r\n)
            var newLineRegex = RegexHelper.GetRegex(@"(<br[ ]?/>)|([\r]?\n)");
            string description = TruncateDescription(ev.EventDescription);
            description = HTMLHelper.StripTags(newLineRegex.Replace(description, EOL_REPLACEMENT));
            lblEventDescriptionValue.Text = HTMLHelper.HTMLEncode(description).Replace(EOL_REPLACEMENT, "<br />");

            if (ev.SiteID > 0)
            {
                SiteInfo si = SiteInfoProvider.GetSiteInfo(ev.SiteID);
                if (si != null)
                {
                    lblSiteNameValue.Text = HTMLHelper.HTMLEncode(si.DisplayName);
                }
            }
            else
            {
                plcSite.Visible = false;
            }

            lblMachineNameValue.Text = HTMLHelper.HTMLEncode(ev.EventMachineName);
            lblEventUrlValue.Text = HTMLHelper.HTMLEncode(ev.EventUrl);
            lblUrlReferrerValue.Text = HTMLHelper.HTMLEncode(ev.EventUrlReferrer);
            lblUserAgentValue.Text = HTMLHelper.HTMLEncode(ev.EventUserAgent);
        }
    }


    /// <summary>
    /// Truncates the event description if it exceeds the allowed length of <see cref="DESCRIPTION_MAX_LENGTH"/>.
    /// </summary>
    private string TruncateDescription(string eventDescription)
    {
        if (eventDescription.Length <= DESCRIPTION_MAX_LENGTH)
        {
            return eventDescription;
        }

        var sb = new StringBuilder();
        sb.AppendFormat(GetString("eventlogdetails.description.truncated"), DESCRIPTION_MAX_LENGTH, GetString("eventlogdetails.export"));
        sb.AppendLine().AppendLine();
        sb.Append(eventDescription.Substring(0, DESCRIPTION_MAX_LENGTH));
        sb.Append("...");
        
        return sb.ToString();
    }

    #endregion
}
