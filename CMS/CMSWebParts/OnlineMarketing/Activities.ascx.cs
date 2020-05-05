using System;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;

public partial class CMSWebParts_OnlineMarketing_Activities : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Site"), "");
        }
        set
        {
            SetValue("Site", value);
        }
    }


    /// <summary>
    /// Gets or sets the activity type name.
    /// </summary>
    public string ActivityType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ActivityType"), "");
        }
        set
        {
            SetValue("ActivityType", value);
        }
    }


    /// <summary>
    /// Gets or sets date interval (0=current day, -1=current week, -2=current month, >0 number of days).
    /// </summary>
    public int DateInterval
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DateInterval"), 0);
        }
        set
        {
            SetValue("DateInterval", value);
        }
    }


    /// <summary>
    /// Gets or sets page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 0);
        }
        set
        {
            SetValue("PageSize", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ACTIVITIES, "ReadActivities"))
        {
            SetupControl();
        }
        else
        {
            StopProcessing = true;
        }
    }


    protected void SetupControl()
    {
        if (StopProcessing)
        {
            listElem.Visible = false;
            return;
        }
        
        disabledModuleInfo.KeyScope = DisabledModuleScope.Both;
        disabledModuleInfo.TestSettingKeys = "CMSEnableOnlineMarketing;CMSCMActivitiesEnabled";

        listElem.Visible = true;
        listElem.OrderBy = "ActivityCreated DESC";

        string where = null;
        if (!String.IsNullOrEmpty(ActivityType))
        {
            where = SqlHelper.AddWhereCondition(where, "ActivityType='" + SqlHelper.EscapeQuotes(ActivityType) + "'");
        }

        // Get site ID from site name
        string siteName = (SiteName != null ? SiteName.ToLowerCSafe() : "");
        int siteId = -1;
        switch (siteName)
        {
            case "##all##":
                break;
            case "##currentsite##":
                siteId = SiteContext.CurrentSiteID;
                break;
            default:
                siteId = SiteInfoProvider.GetSiteID(siteName);
                break;
        }
        if (siteId != -1)
        {
            where = SqlHelper.AddWhereCondition(where, "ActivitySiteID=" + siteId);
        }

        // Get correct time interval
        int days = ValidationHelper.GetInteger(DateInterval, 0);
        DateTime dt = DateTimeHelper.GetDayStart(DateTime.Now);
        switch (days)
        {
            case -1:
                dt = DateTimeHelper.GetWeekStart(dt, CultureHelper.DefaultUICultureCode);
                break;
            case -2:
                dt = DateTimeHelper.GetMonthStart(dt);
                break;
            default:
                dt = dt.AddDays(-days);
                break;
        }
        where = SqlHelper.AddWhereCondition(where, "ActivityCreated >= '" + dt.ToString("s") + "'"); // "s" - ISO format (supported by DB regardless of culture)
        listElem.ShowSiteNameColumn = (siteId == -1); // Show site name column if activities from all sites are listed
        listElem.WhereCondition = where;

        if (!RequestHelper.IsPostBack())
        {
            // Init page size for the first time only
            listElem.PageSize = PageSize;
        }
    }
}