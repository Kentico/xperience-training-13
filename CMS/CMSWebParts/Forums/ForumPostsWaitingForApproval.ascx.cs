using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.SiteProvider;


public partial class CMSWebParts_Forums_ForumPostsWaitingForApproval : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Groups for displayed forums.
    /// </summary>
    public string ForumGroups
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ForumGroups"), String.Empty);
        }
        set
        {
            SetValue("ForumGroups", value);
        }
    }


    /// <summary>
    /// Site name filter.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), String.Empty).Replace("##currentsite##", SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Gets or sets the value of items per page.
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
    /// Text if no data found.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), "No data found");
        }
        set
        {
            SetValue("ZeroRowsText", value);
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
            ucPosts.StopProcessing = true;
        }
        else
        {
            ucPosts.GroupNames = ForumGroups;
            ucPosts.ItemsPerPage = ItemsPerPage;
            ucPosts.ZeroRowText = ZeroRowsText;
            ucPosts.SiteName = SiteName;
            ucPosts.IsLiveSite = (PortalContext.ViewMode != ViewModeEnum.DashboardWidgets);
        }
    }

    #endregion
}