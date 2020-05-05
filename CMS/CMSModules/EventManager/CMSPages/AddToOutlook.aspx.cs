using System;
using System.Text;
using System.Web;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;

public partial class CMSModules_EventManager_CMSPages_AddToOutlook : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get event NodeID from querystring
        int eventNodeId = QueryHelper.GetInteger("eventid", 0);

        if (eventNodeId != 0)
        {
            TreeProvider mTree = new TreeProvider();
            TreeNode eventInfo = mTree.SelectSingleNode(eventNodeId);

            if (eventInfo != null && eventInfo.NodeClassName.EqualsCSafe("cms.bookingevent", true))
            {
                // Get file content.
                byte[] fileContent = GetContent(eventInfo);

                if (fileContent != null)
                {
                    // Clear response.
                    CookieHelper.ClearResponseCookies();
                    Response.Clear();

                    // Prepare response.
                    Response.ContentType = "text/calendar";
                    // Disposition type - For files "attachment" is default
                    Response.AddHeader("Content-Disposition", "attachment;filename=Reminder.ics");

                    Response.OutputStream.Write(fileContent, 0, fileContent.Length);

                    //RequestHelper.CompleteRequest();
                    RequestHelper.EndResponse();
                }
            }
        }
    }


    /// <summary>
    /// Gets iCalendar file content.
    /// </summary>
    /// <param name="data">Data container.</param>
    protected byte[] GetContent(IDataContainer data)
    {
        if (data != null)
        {
            // Get server time zone
            TimeZoneInfo serverTimeZone = TimeZoneHelper.ServerTimeZone;
            // Shift current time (i.e. server time) to GMT
            DateTime currentDateGMT = DateTime.Now;
            currentDateGMT = TimeZoneHelper.ConvertTimeToUTC(currentDateGMT, serverTimeZone);

            // Get event start time (i.e. server time)
            DateTime eventStart = ValidationHelper.GetDateTime(data.GetValue("EventDate"), DateTimeHelper.ZERO_TIME);

            // Get if it is all day event
            bool isAllDay = ValidationHelper.GetBoolean(data.GetValue("EventAllDay"), false);

            // Get Guid of the event, it's required and must be unique -> throw exception if null
            var eventGuid = (Guid)data.GetValue("NodeGUID");

            // Get current site
            var currentSite = SiteInfoProvider.GetSiteInfo(CurrentSiteName);

            // Create content
            var content = new StringBuilder();
            content.AppendLine("BEGIN:VCALENDAR");
            content.AppendLine("PRODID:-//Kentico Software//NONSGML Kentico CMS//EN");
            content.AppendLine("VERSION:2.0");
            content.AppendLine("BEGIN:VEVENT");
            content.Append("UID:").Append(eventGuid).Append("@").AppendLine(currentSite.DomainName);
            content.Append("DTSTAMP:").Append(currentDateGMT.ToString("yyyyMMdd'T'HHmmss")).AppendLine("Z");
            content.Append("DTSTART");
            if (isAllDay)
            {
                content.Append(";VALUE=DATE:").AppendLine(eventStart.ToString("yyyyMMdd"));
            }
            else
            {
                // Shift event start time to GMT
                eventStart = TimeZoneHelper.ConvertTimeToUTC(eventStart, serverTimeZone);
                content.Append(":").Append(eventStart.ToString("yyyyMMdd'T'HHmmss")).AppendLine("Z");
            }

            // Get event end time
            DateTime eventEnd = ValidationHelper.GetDateTime(data.GetValue("EventEndDate"), DateTimeHelper.ZERO_TIME);
            if (eventEnd != DateTimeHelper.ZERO_TIME)
            {
                content.Append("DTEND");
                if (isAllDay)
                {
                    content.Append(";VALUE=DATE:").AppendLine(eventEnd.AddDays(1).ToString("yyyyMMdd"));
                }
                else
                {
                    // Shift event end time to GMT
                    eventEnd = TimeZoneHelper.ConvertTimeToUTC(eventEnd, serverTimeZone);
                    content.Append(":").Append(eventEnd.ToString("yyyyMMdd'T'HHmmss")).AppendLine("Z");
                }
            }

            // Get location
            string location = ValidationHelper.GetString(data.GetValue("EventLocation"), string.Empty);

            // Include location if specified
            if (!String.IsNullOrEmpty(location))
            {
                content.Append("LOCATION:").AppendLine(HTMLHelper.StripTags(HttpUtility.HtmlDecode(location)));
            }

            content.Append("DESCRIPTION:").AppendLine(HTMLHelper.StripTags(HttpUtility.HtmlDecode(ValidationHelper.GetString(data.GetValue("EventDetails"), "")).Replace("\r\n", "").Replace("<br />", "\\n")) + "\\n\\n" + HTMLHelper.StripTags(HttpUtility.HtmlDecode(ValidationHelper.GetString(data.GetValue("EventLocation"), "")).Replace("\r\n", "").Replace("<br />", "\\n")));
            content.Append("SUMMARY:").AppendLine(HttpUtility.HtmlDecode(ValidationHelper.GetString(data.GetValue("EventName"), "")));
            content.AppendLine("PRIORITY:3");
            content.AppendLine("BEGIN:VALARM");
            content.AppendLine("TRIGGER:P0DT0H15M");
            content.AppendLine("ACTION:DISPLAY");
            content.AppendLine("DESCRIPTION:Reminder");
            content.AppendLine("END:VALARM");
            content.AppendLine("END:VEVENT");
            content.AppendLine("END:VCALENDAR");

            // Return byte array
            return Encoding.UTF8.GetBytes(content.ToString());
        }

        return null;
    }
}