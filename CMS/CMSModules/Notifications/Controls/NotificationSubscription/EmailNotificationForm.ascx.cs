using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.Notifications.Web.UI;


public partial class CMSModules_Notifications_Controls_NotificationSubscription_EmailNotificationForm : CMSNotificationGatewayForm
{
    #region "Variables"

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the e-mail(s) from/to textbox.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtEmail.Text.Trim();
        }
        set
        {
            txtEmail.Text = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to allow multiple e-mails separated with semicolon.
    /// </summary>
    public bool EnableMultipleEmails
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        lblEmail.Text = (EnableMultipleEmails ? GetString("general.emails") : GetString("general.email")) + ResHelper.Colon;

        // Fill in the default e-mail
        if ((!RequestHelper.IsPostBack()) && (MembershipContext.AuthenticatedUser != null) && (!String.IsNullOrEmpty(MembershipContext.AuthenticatedUser.Email)))
        {
            txtEmail.Text = MembershipContext.AuthenticatedUser.Email;
        }
    }


    #region "Public methods"

    /// <summary>
    /// Checks whether the input is correct e-mail address (or multiple e-mail addresses).
    /// </summary>
    public override string Validate()
    {
        if (EnableMultipleEmails)
        {
            if (!ValidationHelper.AreEmails(txtEmail.Text.Trim()))
            {
                return GetString("notifications.emailgateway.formats");
            }
        }
        else
        {
            if (!ValidationHelper.IsEmail(txtEmail.Text.Trim()))
            {
                return GetString("notifications.emailgateway.format");
            }
        }

        return String.Empty;
    }


    /// <summary>
    /// Clears the e-mail textbox field.
    /// </summary>
    public override void ClearForm()
    {
        txtEmail.Text = String.Empty;
    }

    #endregion
}