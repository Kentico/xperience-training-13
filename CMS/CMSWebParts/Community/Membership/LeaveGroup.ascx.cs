using System;
using System.Data;
using System.Web;
using System.Web.UI;

using CMS.Community;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Community_Membership_LeaveGroup : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Leave confirmation text or resource string.
    /// </summary>
    public string LeaveText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LeaveText"), "");
        }
        set
        {
            SetValue("LeaveText", value);
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed on join dialog after successful join action.
    /// </summary>
    public string SuccessfullLeaveText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SuccessfullLeaveText"), "");
        }
        set
        {
            SetValue("SuccessfullLeaveText", value);
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed on join dialog if join actin was unsuccessful.
    /// </summary>
    public string UnSuccessfullLeaveText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UnSuccessfullLeaveText"), "");
        }
        set
        {
            SetValue("UnSuccessfullLeaveText", value);
        }
    }

    #endregion


    #region "Public methods"

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
    public void SetupControl()
    {
        if ((CommunityContext.CurrentGroup != null) && GroupMemberInfoProvider.IsMemberOfGroup(MembershipContext.AuthenticatedUser.UserID, CommunityContext.CurrentGroup.GroupID))
        {
            groupLeave.IsOnModalPage = false;
            groupLeave.LeaveText = LeaveText;
            groupLeave.SuccessfulLeaveText = SuccessfullLeaveText;
            groupLeave.UnSuccessfulLeaveText = UnSuccessfullLeaveText;
            groupLeave.Group = CommunityContext.CurrentGroup;
        }
        else
        {
            Visible = false;
        }
    }

    #endregion
}