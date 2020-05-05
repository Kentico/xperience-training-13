using System;

using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using CMS.Notifications.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Notifications_Controls_UserNotifications : CMSAdminControl
{
    #region "Variables"

    private int mDisplayNameLength = 50;
    private string mZeroRowsText = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Maximum length of the displayname (whole display name is displayed in tooltip).
    /// </summary>
    public int DisplayNameLength
    {
        get
        {
            return mDisplayNameLength;
        }
        set
        {
            mDisplayNameLength = value;
        }
    }


    /// <summary>
    /// User id.
    /// </summary>
    public int UserId
    {
        get;
        set;
    }


    /// <summary>
    /// Text displayed when no notifications exist.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            if (mZeroRowsText == "")
            {
                mZeroRowsText = GetString("notifications.userhasnonotifications");
            }
            return mZeroRowsText;
        }
        set
        {
            mZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets directory path for images.
    /// </summary>
    public string UnigridImageDirectory
    {
        get
        {
            return ValidationHelper.GetString(ViewState["UnigridImageDirectory"], null);
        }
        set
        {
            ViewState["UnigridImageDirectory"] = value;
        }
    }


    /// <summary>
    /// Site ID (If this value is set, then only subscriptions for specified site and global subscriptions are
    /// displayed, If this value equals to zero then all subscriptions are displayed).
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.IsLiveSite = IsLiveSite;

        // In design mode is pocessing of control stoped
        if (StopProcessing)
        {
            // Do nothing
            gridElem.StopProcessing = true;
            gridElem.Visible = false;
        }
        else
        {
            if (UnigridImageDirectory != null)
            {
                gridElem.ImageDirectoryPath = UnigridImageDirectory;
            }

            gridElem.ZeroRowsText = ZeroRowsText;
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
            gridElem.OnAction += gridElem_OnAction;
            gridElem.WhereCondition = "(SubscriptionUserID = " + UserId + (SiteID > 0 ? " AND (SubscriptionSiteID IS NULL OR SubscriptionSiteID = " + SiteID + "))" : ")");

            // Set pager links text on live site
            if (IsLiveSite)
            {
                gridElem.Pager.FirstPageText = "&lt;&lt;";
                gridElem.Pager.LastPageText = "&gt;&gt;";
                gridElem.Pager.PreviousPageText = "&lt;";
                gridElem.Pager.NextPageText = "&gt;";
                gridElem.Pager.PreviousGroupText = "...";
                gridElem.Pager.NextGroupText = "...";
            }
        }
    }

    #endregion


    #region "UniGrid Events"

    /// <summary>
    /// Handles Unigrid's OnExternalDataBound event.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "subscriptioneventdisplayname":
                string displayName = Convert.ToString(parameter);

                if (displayName.Length <= DisplayNameLength)
                {
                    return HTMLHelper.HTMLEncode(displayName);
                }
                else
                {
                    return HTMLHelper.HTMLEncode(displayName.Substring(0, DisplayNameLength)) + " ...";
                }
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            try
            {
                if (!RaiseOnCheckPermissions(PERMISSION_MODIFY, this))
                {
                    var cui = MembershipContext.AuthenticatedUser;
                    if ((cui == null) || ((UserId != cui.UserID) && !cui.IsAuthorizedPerResource("CMS.Users", PERMISSION_MODIFY)))
                    {
                        RedirectToAccessDenied("CMS.Users", PERMISSION_MODIFY);
                    }
                }

                NotificationSubscriptionInfoProvider.DeleteNotificationSubscriptionInfo(Convert.ToInt32(actionArgument));
            }
            catch (Exception ex)
            {
                // Show error message
                ShowError(ex.Message);
            }
        }
    }

    #endregion


    /// <summary>
    /// Overridden SetValue - because of MyAccount webpart.
    /// </summary>
    /// <param name="propertyName">Name of the property to set</param>
    /// <param name="value">Value to set</param>
    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerCSafe())
        {
            case "userid":
                UserId = ValidationHelper.GetInteger(value, 0);
                break;
            case "unigridimagedirectory":
                UnigridImageDirectory = ValidationHelper.GetString(value, "");
                break;
        }

        return true;
    }
}