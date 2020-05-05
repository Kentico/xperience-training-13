using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;


public partial class CMSWebParts_Blogs_BlogComment : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the site name. If is empty, documents from all sites are displayed.
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
    /// Gets or sets the order by condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "ReportWhen");
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Gets or sets the sorting direction.
    /// </summary>
    public string Sorting
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Sorting"), "ASC");
        }
        set
        {
            SetValue("Sorting", value);
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
    /// Indicates whether displayed comment is approved.
    /// </summary>
    public string IsApproved
    {
        get
        {
            return ValidationHelper.GetString(GetValue("IsApproved"), "no");
        }
        set
        {
            SetValue("IsApproved", value);
        }
    }


    /// <summary>
    /// Indicates whether displayed comment is spam.
    /// </summary>
    public string IsSpam
    {
        get
        {
            return ValidationHelper.GetString(GetValue("IsSpam"), "all");
        }
        set
        {
            SetValue("IsSpam", value);
        }
    }


    /// <summary>
    /// Blog name filter.
    /// </summary>
    public string BlogName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BlogName"), "##myblogs##");
        }
        set
        {
            SetValue("BlogName", value);
        }
    }


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
            ucComments.StopProcessing = value;
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
    /// Initializes control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            ucComments.ShowFilter = false;
            ucComments.BlogName = BlogName;
            ucComments.IsSpam = IsSpam;
            ucComments.IsApproved = IsApproved;
            ucComments.ItemsPerPage = ItemsPerPage;
            ucComments.OrderBy = OrderBy + " " + Sorting;
            ucComments.SiteName = SiteName;
        }
    }
}