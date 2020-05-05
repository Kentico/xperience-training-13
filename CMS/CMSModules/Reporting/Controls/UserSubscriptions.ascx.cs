using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

using Action = CMS.UIControls.UniGridConfig.Action;


public partial class CMSModules_Reporting_Controls_UserSubscriptions : CMSAdminControl
{
    #region "Public properties"


    /// <summary>
    /// User ID.
    /// </summary>
    public int UserID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("userid"), MembershipContext.AuthenticatedUser.UserID);
        }
        set
        {
            SetValue("userid", value);
        }
    }


    /// <summary>
    /// Site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("siteid"), SiteContext.CurrentSiteID);
        }
        set
        {
            SetValue("siteid", value);
        }
    }


    /// <summary>
    /// Is live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("islivesite"), base.IsLiveSite);
        }
        set
        {
            base.IsLiveSite = value;
            SetValue("islivesite", value);
        }
    }

    #endregion


    #region "Event handlers"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide info label for empty unigrid
        if (reportSubscriptions.RowsCount == 0)
        {
            lblMessage.Visible = false;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
        }
        else
        {
            // Disable unigrid if StopProcessing
            reportSubscriptions.Visible = false;
            reportSubscriptions.StopProcessing = true;
        }

    }


    void reportSubscriptions_OnAction(string actionName, object actionArgument)
    {
        int id = ValidationHelper.GetInteger(actionArgument, 0);

        if (CheckSecurity(id))
        {
            switch (actionName.ToLowerCSafe())
            {
                case "reportunsubscribe":
                    ReportSubscriptionInfoProvider.DeleteReportSubscriptionInfo(id);
                    break;
            }
        }
    }


    #endregion


    #region "Private methods"

    /// <summary>
    /// Checks if given subscription is for current user.
    /// </summary>
    /// <param name="subscriptionID">Subscription ID</param>
    /// <returns>True if subscription belongs to user, false if not</returns>
    private bool CheckSecurity(int subscriptionID)
    {
        ReportSubscriptionInfo rsi = ReportSubscriptionInfoProvider.GetReportSubscriptionInfo(subscriptionID);
        return (rsi.ReportSubscriptionUserID == UserID);
    }


    /// <summary>
    /// Setup controls.
    /// </summary>
    private void SetupControls()
    {
        Action act = reportSubscriptions.GridActions.Actions[1] as Action;
        if (act != null)
        {
	        ScriptHelper.RegisterDialogScript(Page);
            String subscriptionUrl = IsLiveSite ? "~/CMSModules/Reporting/LiveDialogs/EditSubscription.aspx?subscriptionId={0}&mode=liveedit" : "~/CMSModules/Reporting/Dialogs/EditSubscription.aspx?subscriptionId={0}&mode=edit";
            act.OnClick = String.Format("modalDialog('{0}','Subscription',{1},{2});return false;", ResolveUrl(subscriptionUrl), AnalyticsHelper.SUBSCRIPTION_WINDOW_WIDTH + 70, AnalyticsHelper.SUBSCRIPTION_WINDOW_HEIGHT + 100);
        }

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "RefreshScript", ScriptHelper.GetScript("function refreshCurrentPage() {" + ControlsHelper.GetPostBackEventReference(btnPostback, String.Empty) + "}"));

        reportSubscriptions.WhereCondition = "(ReportSubscriptionUserID = " + UserID + ") AND (ReportSubscriptionSiteID = " + SiteID + ")";
        reportSubscriptions.IsLiveSite = IsLiveSite;
        reportSubscriptions.ZeroRowsText = GetString("reports.userhasnosubscriptions");
        reportSubscriptions.OnAction += new OnActionEventHandler(reportSubscriptions_OnAction);
        reportSubscriptions.Pager.DefaultPageSize = 10;
    }

    #endregion
}
