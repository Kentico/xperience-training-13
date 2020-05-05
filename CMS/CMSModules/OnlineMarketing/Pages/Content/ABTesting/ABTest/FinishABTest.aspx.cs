using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;


/// <summary>
/// Modal dialog invoked from AB test Overview page.
/// Finishes the AB test immediately or on the selected date and time.
/// Uses test ID from the query string.
/// </summary>
[Title("abtesting.finishtest")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_FinishABTest : CMSModalPage
{
    #region "Variables"

    /// <summary>
    /// Current AB test.
    /// </summary>
    private ABTestInfo mABTest;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current AB test.
    /// </summary>
    private ABTestInfo ABTest
    {
        get
        {
            return mABTest ?? (mABTest = ABTestInfo.Provider.Get(QueryHelper.GetInteger("testid", 0)));
        }
    }


    /// <summary>
    /// Name of the AB test site.
    /// </summary>
    private string ABTestSiteName
    {
        get
        {
            if (ABTest == null)
            {
                return null;
            }

            SiteInfo site = SiteInfoProvider.GetSiteInfo(ABTest.ABTestSiteID);
            return site.SiteName;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        Save += (s, ea) => FinishABTest();

        if (ABTest == null)
        {
            ShowWarning(GetString("abtesting.invalidtest"));
            return;
        }

        if (!CurrentUser.IsAuthorizedPerResource("CMS.ABTest", "Read", ABTestSiteName))
        {
            RedirectToAccessDenied("cms.abtest", "Read");
        }

        Title = GetString("abtesting.finishtest");

        // If not postback and test is scheduled to be finished, fill the finish date
        if (!RequestHelper.IsPostBack() && (ABTest.ABTestOpenTo != DateTimeHelper.ZERO_TIME))
        {
            radLater.Checked = true;
            radNow.Checked = false;
            calendarControl.SelectedDateTime = ABTest.ABTestOpenTo;
            calendarControl.Enabled = radLater.Checked;
        }
    }


    protected void radGroupFinish_CheckedChanged(object sender, EventArgs e)
    {
        calendarControl.Enabled = radLater.Checked;
    }


    protected void FinishABTest()
    {
        if (ABTest == null)
        {
            ShowWarning(GetString("abtesting.invalidtest"));
            return;
        }

        if (!CurrentUser.IsAuthorizedPerResource("CMS.ABTest", "Manage", ABTestSiteName))
        {
            RedirectToAccessDenied("cms.abtest", "Manage");
        }

        DateTime finish = (radNow.Checked) ? DateTime.Now : calendarControl.SelectedDateTime;

        // Validate input
        if (radLater.Checked && !ABTestValidator.IsValidFinish(ABTest.ABTestOpenFrom, finish))
        {
            ShowError(GetString("abtesting.scheduled.invaliddate"));
            return;
        }

        // Save the test with the new finish date
        ABTest.ABTestOpenTo = finish;
        ABTestInfo.Provider.Set(ABTest);

        ScriptHelper.RegisterWOpenerScript(this);

        // Refresh the window opener with parameter indicating that the refresh was caused by this modal window
        ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshPage", ScriptHelper.GetScript("wopener.RefreshPage(false); CloseDialog();"));
    }

    #endregion
}
