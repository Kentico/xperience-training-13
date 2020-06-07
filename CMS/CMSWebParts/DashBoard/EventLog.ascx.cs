using System;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSWebParts_DashBoard_EventLog : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Site name
    ///  "": All sites with global events
    ///  "0": Global events only
    ///  or site name
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), "").Replace("##currentsite##", SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Show global events only.
    /// </summary>
    public bool ShowGlobalEventsOnly
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowGlobalEventsOnly"), false);
        }
        set
        {
            SetValue("ShowGlobalEventsOnly", value);
        }
    }


    /// <summary>
    /// Event type.
    /// </summary>
    public string EventType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventType"), "");
        }
        set
        {
            SetValue("EventType", value);
        }
    }


    /// <summary>
    /// Source.
    /// </summary>
    public string Source
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Source"), "");
        }
        set
        {
            SetValue("Source", value);
        }
    }


    /// <summary>
    /// Event code.
    /// </summary>
    public string EventCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventCode"), "");
        }
        set
        {
            SetValue("EventCode", value);
        }
    }


    /// <summary>
    /// Items per page (10,25,50).
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemsPerPage"), "25");
        }
        set
        {
            SetValue("ItemsPerPage", value);
        }
    }


    /// <summary>
    /// Order by.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "");
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Sorting (asc/desc)
    /// </summary>
    public string Sorting
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Sorting"), "asc");
        }
        set
        {
            SetValue("Sorting", value);
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            eventLog.StopProcessing = value;
        }
    }

    #endregion


    #region "Methods"

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
            // Do not process
        }
        else
        {
            eventLog.OnCheckPermissions += eventLog_OnCheckPermissions;
            eventLog.EventLogGrid.OrderBy = OrderBy + " " + Sorting;
            eventLog.EventLogGrid.WhereCondition = GenerateWhereCondition();

            if ((!RequestHelper.IsPostBack()) && (!string.IsNullOrEmpty(ItemsPerPage)))
            {
                eventLog.EventLogGrid.Pager.DefaultPageSize = ValidationHelper.GetInteger(ItemsPerPage, -1);
            }
        }
    }


    /// <summary>
    /// OnLoad handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        eventLog.ReloadData();
        base.OnLoad(e);
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        eventLog.ReloadData();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// OnCheckPermission event handler.
    /// </summary>
    /// <param name="permissionType">Type of the permission</param>
    /// <param name="sender">The sender</param>
    private void eventLog_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!CMSEventLogPage.CheckPermissions(false))
        {
            StopProcessing = false;
            eventLog.Visible = false;
            messageElem.Visible = true;
            messageElem.ErrorMessage = GetString("general.nopermission");
        }
    }


    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        string whereCond = "";

        if (ShowGlobalEventsOnly)
        {
            SiteName = "0";
        }

        if (!String.IsNullOrEmpty(Source))
        {
            whereCond = SqlHelper.AddWhereCondition(whereCond, "Source = N'" + SqlHelper.GetSafeQueryString(Source, false) + "'");
        }

        if (!String.IsNullOrEmpty(EventType))
        {
            whereCond = SqlHelper.AddWhereCondition(whereCond, "EventType = N'" + SqlHelper.GetSafeQueryString(EventType, false) + "'");
        }

        if (!String.IsNullOrEmpty(EventCode))
        {
            whereCond = SqlHelper.AddWhereCondition(whereCond, "EventCode = N'" + SqlHelper.GetSafeQueryString(EventCode, false) + "'");
        }

        // Append site condition if siteid given
        SiteInfo siteObj = null;
        int siteId = -1;

        if (SiteName != TreeProvider.ALL_SITES)
        {
            siteObj = SiteInfo.Provider.Get(SiteName);
        }

        // -1: All sites with global events
        //  0: Global events only
        //  or SiteName
        if (siteObj == null)
        {
            if (ShowGlobalEventsOnly)
            {
                siteId = 0;
            }
        }
        else
        {
            siteId = siteObj.SiteID;
        }

        // create where condition for SiteID
        if (!String.IsNullOrEmpty(whereCond) && (siteId >= 0))
        {
            whereCond += " AND ";
        }

        if (siteId > 0)
        {
            whereCond += " (SiteID=" + siteId + ")";
        }
        else if (siteId == 0)
        {
            whereCond += "(SiteID IS NULL)";
        }

        return whereCond;
    }

    #endregion
}