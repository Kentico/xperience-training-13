using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Community;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_Community_Membership_MySentInvitations : CMSAbstractWebPart
{
    #region "Private variables"

    protected string mDeleteImageUrl = string.Empty;
    protected string mResendImageUrl = string.Empty;
    protected string mDeleteToolTip = string.Empty;
    protected string mResendToolTip = string.Empty;
    protected string mUserName = String.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets URL of the delete button image.
    /// </summary>
    public string DeleteImageUrl
    {
        get
        {
            return UrlResolver.ResolveUrl(DataHelper.GetNotEmpty(GetValue("DeleteImageUrl"), GetImageUrl("Design/Controls/UniGrid/Actions/delete.png")));
        }
        set
        {
            mDeleteImageUrl = value;
            SetValue("DeleteImageUrl", value);
        }
    }


    /// <summary>
    /// Gets or sets URL of the accept button image.
    /// </summary>
    public string ResendImageUrl
    {
        get
        {
            return UrlResolver.ResolveUrl(DataHelper.GetNotEmpty(GetValue("AcceptImageUrl"), GetImageUrl("Design/Controls/UniGrid/Actions/resendemail.png")));
        }
        set
        {
            mDeleteImageUrl = value;
            SetValue("AcceptImageUrl", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), rptMySentInvitations.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            rptMySentInvitations.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed when there is no data.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), rptMySentInvitations.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            rptMySentInvitations.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// User name.
    /// </summary>
    public string UserName
    {
        get
        {
            mUserName = ValidationHelper.GetString(GetValue("UserName"), String.Empty);
            if (mUserName != String.Empty)
            {
                return mUserName;
            }

            // Back compatibility
            int userID = ValidationHelper.GetInteger(GetValue("UserID"), 0);
            if (userID != 0)
            {
                if (userID == MembershipContext.AuthenticatedUser.UserID)
                {
                    return MembershipContext.AuthenticatedUser.UserName;
                }

                UserInfo ui = UserInfoProvider.GetUserInfo(userID);
                if (ui != null)
                {
                    return ui.UserName;
                }
            }

            return MembershipContext.AuthenticatedUser.UserName;
        }
        set
        {
            SetValue("UserName", value);
            mUserName = value;
        }
    }

    #endregion


    #region "Caption properties"

    public string ResendFailed
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ResendFailed"), GetString("groupinvitation.resendfailed"));
        }
    }

    public string ResendSuccess
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ResendSuccess"), GetString("groupinvitation.resendsuccess"));
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
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    protected void SetupControl()
    {
        if (StopProcessing)
        {
            Visible = false;
            // Do not load data
        }
        else
        {
            ltlMessage.Text = ScriptHelper.GetScript("var deleteMessage ='" + GetString("general.confirmdelete") + "';");
            rptMySentInvitations.ZeroRowsText = ZeroRowsText;
            rptMySentInvitations.HideControlForZeroRows = HideControlForZeroRows;
            mDeleteImageUrl = DeleteImageUrl;
            mResendImageUrl = ResendImageUrl;
            mResendToolTip = GetString("general.resend");
            mDeleteToolTip = GetString("general.delete");
            BindData();
        }
    }


    /// <summary>
    /// Deletes invitation.
    /// </summary>
    protected void btnDelete_OnCommand(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "delete")
        {
            int invitationId = ValidationHelper.GetInteger(e.CommandArgument, 0);
            InvitationInfoProvider.DeleteInvitationInfo(invitationId);
            BindData();
        }
    }


    /// <summary>
    /// Accepts invitation.
    /// </summary>
    protected void btnResend_OnCommand(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "resend")
        {
            try
            {
                // Get invitation info object
                int invitationId = ValidationHelper.GetInteger(e.CommandArgument, 0);
                InvitationInfo invitation = InvitationInfoProvider.GetInvitationInfo(invitationId);
                if (invitation != null)
                {
                    // Send invitation e-mail
                    InvitationInfoProvider.SendInvitationEmail(invitation, SiteContext.CurrentSiteName);
                    lblInfo.Text = ResendSuccess + "<br /><br />";
                }
            }
            catch
            {
                lblInfo.Text = ResendFailed + "<br /><br />";
                ;
                lblInfo.CssClass = "InvitationErrorLabel";
            }

            lblInfo.Visible = true;
        }
    }


    /// <summary>
    /// Binds data to repeater.
    /// </summary>
    private void BindData()
    {
        if (UserName != String.Empty)
        {
            DataSet invitations = InvitationInfoProvider.GetMySentInvitations("InvitedByUserID IN (SELECT UserID FROM CMS_User WHERE UserName='" + SqlHelper.GetSafeQueryString(UserName, false) + "')", "InvitationCreated");
            rptMySentInvitations.DataSource = invitations;
            rptMySentInvitations.DataBind();
            // Hide control if no data
            if (DataHelper.DataSourceIsEmpty(invitations) && (HideControlForZeroRows))
            {
                Visible = false;
                rptMySentInvitations.Visible = false;
            }
        }
    }


    /// <summary>
    /// Resolve text.
    /// </summary>
    /// <param name="value">Input value</param>
    public string ResolveText(object value)
    {
        return HTMLHelper.HTMLEncode(ValidationHelper.GetString(value, ""));
    }

    #endregion
}
