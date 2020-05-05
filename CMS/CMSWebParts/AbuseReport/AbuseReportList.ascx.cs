using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_AbuseReport_AbuseReportList : CMSAbstractWebPart
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
    /// Filter status of report abuse.
    /// </summary>
    public int Status
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Status"), -1);
        }
        set
        {
            SetValue("Status", value);
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
            ucAbuseReportList.StopProcessing = value;
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
    /// Reload date override.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            ucAbuseReportList.StopProcessing = true;
        }
        else
        {
            ucAbuseReportList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(ucAbuseReportList_OnCheckPermissions);
            ucAbuseReportList.OrderBy = OrderBy + " " + Sorting;
            ucAbuseReportList.SiteName = SiteName;
            ucAbuseReportList.Status = Status;
            ucAbuseReportList.ItemsPerPage = ItemsPerPage;
        }
    }


    /// <summary>
    /// Check permissions.
    /// </summary>
    /// <param name="permissionType">Permissions</param>
    /// <param name="sender">Sender</param>
    protected void ucAbuseReportList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.AbuseReport", permissionType))
        {
            sender.StopProcessing = true;
            ucAbuseReportList.Visible = false;
            messageElem.Visible = true;
            messageElem.ErrorMessage = GetString("general.nopermission");
        }
    }
}