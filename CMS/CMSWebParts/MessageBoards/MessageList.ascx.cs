using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_MessageBoards_MessageList : CMSAbstractWebPart
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
            return ValidationHelper.GetString(GetValue("OrderBy"), "MessageInserted");
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
    public int MessageBoard
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MessageBoard"), 0);
        }
        set
        {
            SetValue("MessageBoard", value);
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
            ucMessageList.StopProcessing = true;
        }
        else
        {
            ucMessageList.SiteName = SiteName;
            ucMessageList.IsApproved = IsApproved;
            ucMessageList.IsSpam = IsSpam;
            ucMessageList.ShowFilter = false;
            ucMessageList.ItemsPerPage = ItemsPerPage;
            ucMessageList.OrderBy = OrderBy + " " + Sorting;
            ucMessageList.AllowMassActions = false;
            ucMessageList.BoardID = MessageBoard;
            ucMessageList.IsLiveSite = true;
            ucMessageList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(ucMessageList_OnCheckPermissions);
        }
    }


    /// <summary>
    /// Check permissions.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="sender">Sender</param>
    private void ucMessageList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.messageboards", permissionType))
        {
            ucMessageList.ShowPermissionMessage = true;
            sender.StopProcessing = true;
        }
    }
}