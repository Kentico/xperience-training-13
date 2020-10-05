using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_Subscriptions : CMSAdminControl
{
    #region "Private variables"

    private bool mShowNewsletters = true;
    private bool mShowReports = true;
    private bool mSendConfirmationMail = true;
    private int mUserId = 0;
    private int mSiteId = 0;

    private CMSAdminControl ucNewsletters = null;
    private CMSAdminControl ucReports = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether newsletters subscriptions are shown.
    /// </summary>
    public bool ShowNewsletters
    {
        get
        {
            return mShowNewsletters;
        }
        set
        {
            mShowNewsletters = value;
        }
    }


    /// <summary>
    /// Indicates whether reporting subscriptions are shown.
    /// </summary>
    public bool ShowReports
    {
        get
        {
            return mShowReports;
        }
        set
        {
            mShowReports = value;
        }
    }


    /// <summary>
    /// Indicates whether send email when (un)subscribed.
    /// </summary>
    public bool SendConfirmationMail
    {
        get
        {
            return mSendConfirmationMail;
        }
        set
        {
            mSendConfirmationMail = value;
        }
    }


    /// <summary>
    /// Gets or sets user ID.
    /// </summary>
    public int UserID
    {
        get
        {
            return mUserId;
        }
        set
        {
            mUserId = value;
        }
    }


    /// <summary>
    /// Gets or sets site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
            if (ucNewsletters != null)
            {
                ucNewsletters.SetValue("siteid", value);
            }
        }
    }

    #endregion


    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        // Get current user if UserID is not defined
        if (UserID <= 0)
        {
            UserID = MembershipContext.AuthenticatedUser.UserID;
        }

        // Use current site if SiteID is not defined
        if (SiteID <= 0)
        {
            SiteID = SiteContext.CurrentSiteID;
        }
        string siteName = SiteInfoProvider.GetSiteName(SiteID);

        bool firstInserted = false;

        // Try to init newsletters subscriptions
        if (ShowNewsletters && ModuleManager.IsModuleLoaded(ModuleName.NEWSLETTER))
        {
            ucNewsletters = Page.LoadUserControl("~/CMSModules/Newsletters/Controls/MySubscriptions.ascx") as CMSAdminControl;
            if (ucNewsletters != null)
            {
                headNewsletters.Visible = true;
                pnlNewsletters.Visible = true;
                ucNewsletters.ID = "ucNewsletters";
                ucNewsletters.SetValue("externaluse", true);
                ucNewsletters.SetValue("forcedvisible", true);
                ucNewsletters.SetValue("userid", UserID);
                ucNewsletters.SetValue("siteid", SiteID);
                ucNewsletters.SetValue("sendconfirmationemail", SendConfirmationMail);
                ucNewsletters.StopProcessing = StopProcessing;
                ucNewsletters.IsLiveSite = IsLiveSite;

                pnlNewsletters.Controls.Clear();
                pnlNewsletters.Controls.Add(new LiteralControl("<div class=\"SubscriptionsGroup\">"));
                pnlNewsletters.Controls.Add(ucNewsletters);
                pnlNewsletters.Controls.Add(new LiteralControl("</div>"));


                firstInserted = true;

                ucNewsletters.OnCheckPermissions += ucNewsletters_OnCheckPermissions;
            }
        }

        // Try to init reports subscriptions
        if (ShowReports && ModuleManager.IsModuleLoaded(ModuleName.REPORTING) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleName.REPORTING, siteName))
        {
            ucReports = Page.LoadUserControl("~/CMSModules/Reporting/Controls/UserSubscriptions.ascx") as CMSAdminControl;
            if (ucReports != null)
            {
                headReports.Visible = true;
                pnlReports.Visible = true;
                ucReports.ID = "ucReports";
                ucReports.SetValue("userid", UserID);
                ucReports.SetValue("siteid", SiteID);
                ucReports.StopProcessing = StopProcessing;
                ucReports.OnCheckPermissions += ucReports_OnCheckPermissions;
                ucReports.IsLiveSite = IsLiveSite;

                if (firstInserted)
                {
                    pnlReports.Attributes.Add("class", "SubscriptionsPanel");
                }
                pnlReports.Controls.Clear();
                pnlReports.Controls.Add(new LiteralControl("<div class=\"SubscriptionsGroup\">"));
                pnlReports.Controls.Add(ucReports);
                pnlReports.Controls.Add(new LiteralControl("</div>"));
            }
        }
    }


    #region "Security"

    protected void ucNewsletters_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void ucReports_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);
        switch (propertyName.ToLowerCSafe())
        {
            case "userid":
                UserID = ValidationHelper.GetInteger(value, 0);
                break;

            case "siteid":
                SiteID = ValidationHelper.GetInteger(value, 0);
                break;

            case "shownewsletters":
                ShowNewsletters = ValidationHelper.GetBoolean(value, true);
                break;

            case "showreports":
                ShowReports = ValidationHelper.GetBoolean(value, true);
                break;

            case "sendconfirmationemail":
                SendConfirmationMail = ValidationHelper.GetBoolean(value, true);
                break;
        }

        return true;
    }
} 