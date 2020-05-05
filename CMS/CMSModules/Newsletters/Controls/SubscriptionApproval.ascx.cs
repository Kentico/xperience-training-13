using System;

using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Newsletters_Controls_SubscriptionApproval : CMSUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets successful approval text.
    /// </summary>
    public string SuccessfulApprovalText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets unsuccessful approval text.
    /// </summary>
    public string UnsuccessfulApprovalText
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If StopProcessing flag is set, do nothing
        if (StopProcessing)
        {
            Visible = false;
            return;
        }

        string subscriptionHash = QueryHelper.GetString("subscriptionhash", string.Empty);
        string requestTime = QueryHelper.GetString("datetime", string.Empty);
        DateTime datetime = DateTimeHelper.ZERO_TIME;

        // Get date and time
        if (!string.IsNullOrEmpty(requestTime))
        {
            try
            {
                datetime = DateTimeUrlFormatter.Parse(requestTime);
            }
            catch
            {
                lblInfo.Text = ResHelper.GetString("newsletter.approval_failed");
                return;
            }
        }

        if (string.IsNullOrEmpty(subscriptionHash))
        {
            Visible = false;
            return;
        }

        var approvalService = Service.Resolve<ISubscriptionApprovalService>();
        var result = approvalService.ApproveSubscription(subscriptionHash, false, SiteContext.CurrentSiteName, datetime);

        switch (result)
        {
            case ApprovalResult.Success:
                lblInfo.Text = !String.IsNullOrEmpty(SuccessfulApprovalText) ? SuccessfulApprovalText : ResHelper.GetString("newsletter.successful_approval");
                break;

            case ApprovalResult.Failed:
                lblInfo.Text = !String.IsNullOrEmpty(UnsuccessfulApprovalText) ? UnsuccessfulApprovalText : ResHelper.GetString("newsletter.approval_failed");
                break;

            case ApprovalResult.TimeExceeded:
                lblInfo.Text = !String.IsNullOrEmpty(UnsuccessfulApprovalText) ? UnsuccessfulApprovalText : ResHelper.GetString("newsletter.approval_timeexceeded");
                break;

            case ApprovalResult.AlreadyApproved:
                lblInfo.Text = !String.IsNullOrEmpty(SuccessfulApprovalText) ? SuccessfulApprovalText : ResHelper.GetString("newsletter.successful_approval");
                break;

            // Subscription not found
            default:
                lblInfo.Text = !String.IsNullOrEmpty(UnsuccessfulApprovalText) ? UnsuccessfulApprovalText : ResHelper.GetString("newsletter.approval_invalid");
                break;
        }
    }

    #endregion
}