using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

public partial class CMSWebParts_DashBoard_System : CMSAbstractWebPart
{
    /// <summary>
    /// Gets or sets the timer interval (seconds) for the page refresh.
    /// </summary>
    public int RefreshInterval
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RefreshInterval"), 2);
        }
        set
        {
            SetValue("RefreshInterval", value);
            sysInfo.RefreshInterval = value;
        }
    }


    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            // System control properties
            sysInfo.RefreshInterval = RefreshInterval;
            sysInfo.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(sysInfo_OnCheckPermissions);
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    /// <summary>
    /// OnCheckPermission event handler
    /// </summary>
    /// <param name="permissionType">Type of the permission.</param>
    /// <param name="sender">The sender.</param>
    private void sysInfo_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            sender.StopProcessing = true;
            sysInfo.Visible = false;
            messageElem.Visible = true;
            messageElem.ErrorMessage = GetString("general.nopermission");
        }
    }
}