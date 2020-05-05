using System;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.WebAnalytics;


public partial class CMSWebParts_Viewers_PageViews : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets message text.
    /// </summary>
    public string MessageText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MessageText"), "{0}");
        }
        set
        {
            SetValue("MessageText", value);
        }
    }


    /// <summary>
    /// Gets or sets type of statistic type (last day, week, month, year).
    /// </summary>
    public int StatisticsType
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("StatisticsType"), 0);
        }
        set
        {
            SetValue("StatisticsType", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        // Check the site
        int siteId = SiteContext.CurrentSiteID;
        if (siteId > 0)
        {
            // Check current page
            PageInfo currentPage = DocumentContext.CurrentPageInfo;
            if (currentPage != null)
            {
                int objectId = currentPage.NodeID;

                DateTime fromDate = DateTime.Now;
                DateTime toDate = DateTime.Now;

                HitsIntervalEnum interval;

                // Prepare the parameters
                switch (StatisticsType)
                {
                    case 1: // Week
                        {
                            fromDate = DateTimeHelper.GetWeekStart(DateTime.Now);
                            toDate = fromDate.AddDays(7);
                            interval = HitsIntervalEnum.Week;
                        }
                        break;

                    case 2: // Month
                        {
                            fromDate = DateTimeHelper.GetMonthStart(DateTime.Now);
                            toDate = fromDate.AddMonths(1);
                            interval = HitsIntervalEnum.Month;
                        }
                        break;

                    case 3: // Year
                        {
                            fromDate = DateTimeHelper.GetYearStart(DateTime.Now);
                            toDate = fromDate.AddYears(1);
                            interval = HitsIntervalEnum.Year;
                        }
                        break;

                    case 4:
                        {
                            fromDate = DataTypeManager.MIN_DATETIME;
                            toDate = DataTypeManager.MAX_DATETIME;
                            interval = HitsIntervalEnum.Year;
                        }
                        break;

                    default: // Day
                        {
                            fromDate = DateTimeHelper.GetDayStart(DateTime.Now);
                            toDate = fromDate.AddDays(1);
                            interval = HitsIntervalEnum.Day;
                        }
                        break;
                }

                // Get the text
                lblInfo.Text = String.Format(MessageText, HitsInfoProvider.GetObjectHitCount(siteId, objectId, interval, "pageviews", fromDate, toDate));
            }
        }
    }
}