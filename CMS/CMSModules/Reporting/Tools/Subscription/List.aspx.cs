using System;

using CMS.Base;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement("CMS.Reporting", "Subscriptions")]
public partial class CMSModules_Reporting_Tools_Subscription_List : CMSReportingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var cui = MembershipContext.AuthenticatedUser;
        if (!(cui.IsAuthorizedPerResource("cms.reporting", "subscribe") || cui.IsAuthorizedPerResource("cms.reporting", "modify")))
        {
            RedirectToAccessDenied("cms.reporting", "Subscribe");
        }

        InitActions();

        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.EditActionUrl = "Edit.aspx?subscriptionId={0}&reportid=" + QueryHelper.GetInteger("reportid", 0);
        gridElem.WhereCondition = String.Format("ReportSubscriptionReportID={0} AND ReportSubscriptionSiteID={1}", QueryHelper.GetInteger("ReportID", 0), SiteContext.CurrentSiteID);
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
    }


    void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            int id = ValidationHelper.GetInteger(actionArgument, 0);

            var cui = MembershipContext.AuthenticatedUser;
            bool haveModify = cui.IsAuthorizedPerResource("cms.reporting", "modify");
            if (!(cui.IsAuthorizedPerResource("cms.reporting", "subscribe") || haveModify))
            {
                RedirectToAccessDenied("cms.reporting", "Subscribe");
            }

            if (!cui.IsAuthorizedPerResource("cms.reporting", "modify"))
            {
                ReportSubscriptionInfo rsi = ReportSubscriptionInfoProvider.GetReportSubscriptionInfo(id);
                if ((rsi != null) && (rsi.ReportSubscriptionUserID != cui.UserID))
                {
                    RedirectToAccessDenied(GetString("reportsubscription.onlymodifyusersallowed"));
                }
            }

            ReportSubscriptionInfoProvider.DeleteReportSubscriptionInfo(id);
        }
    }


    object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "enabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "username":
                String userName = ValidationHelper.GetString(parameter, String.Empty);
                return Functions.GetFormattedUserName(userName);
        }
        return sender;
    }


    /// <summary>
    /// Gets string array representing header actions.
    /// </summary>
    /// <returns>Array of strings</returns>
    private void InitActions()
    {
        HeaderAction checkin = new HeaderAction
        {
            Text = GetString("reportsubscription.new"),
            RedirectUrl = "Edit.aspx?reportID=" + QueryHelper.GetInteger("reportID", 0)
        };
        CurrentMaster.HeaderActions.ActionsList.Add(checkin);
    }
}
