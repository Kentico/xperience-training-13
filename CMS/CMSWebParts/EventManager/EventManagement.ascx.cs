using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_EventManager_EventManagement : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the order by condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "eventdate");
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
    /// Reloads control data.
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
            EventManager.StopProcessing = true;
        }
        else
        {
            EventManager.OrderBy = OrderBy + " " + Sorting;
            EventManager.ItemsPerPage = ItemsPerPage;
            EventManager.OnCheckPermissions += EventManager_OnCheckPermissions;
        }
    }


    /// <summary>
    /// Check permissions.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="sender">Sender</param>
    private void EventManager_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.eventmanager", permissionType))
        {
            EventManager.Visible = false;
            messageElem.Visible = true;
            sender.StopProcessing = true;
            messageElem.ErrorMessage = GetString("general.nopermission");
        }
    }

    #endregion
}