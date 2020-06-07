using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

/// <summary>
/// Modal dialog invoked from AB test Overview page.
/// Starts the AB test immediately or on the selected date and time.
/// Uses test ID from the query string.
/// </summary>
[Title("abtesting.starttest")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_StartABTest : CMSModalPage
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

            SiteInfo site = SiteInfo.Provider.Get(ABTest.ABTestSiteID);
            return site.SiteName;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        Save += (s, ea) => StartABTest();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (ABTest == null)
        {
            ShowWarning(GetString("abtesting.invalidtest"));
            return;
        }

        if (!CurrentUser.IsAuthorizedPerResource("CMS.ABTest", "Read", ABTestSiteName))
        {
            RedirectToAccessDenied("cms.abtest", "Read");
        }

        Title = GetString("abtesting.starttest");

        // If not postback and test is scheduled to be started, fill the start date
        if (!RequestHelper.IsPostBack() && (ABTest.ABTestOpenFrom != DateTimeHelper.ZERO_TIME))
        {
            radLater.Checked = true;
            radNow.Checked = false;
            calendarControl.SelectedDateTime = ABTest.ABTestOpenFrom;
            calendarControl.Enabled = radLater.Checked;
        }
    }


    protected void radGroupStart_CheckedChanged(object sender, EventArgs e)
    {
        calendarControl.Enabled = radLater.Checked;
    }


    protected void StartABTest()
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

        DateTime start = (radNow.Checked) ? DateTime.Now : calendarControl.SelectedDateTime;

        // Validate input
        if (radLater.Checked && !ABTestValidator.IsValidStart(start))
        {
            ShowError(GetString("abtesting.scheduled.invaliddate"));
            return;
        }

        // Create clone of the test to circumvent caching issues
        ABTestInfo testClone = ABTest.Clone();
        testClone.ABTestOpenFrom = start;

        // Check for collisions
        string collidingTestName = ABTestValidator.GetCollidingTestName(testClone);
        if (!String.IsNullOrEmpty(collidingTestName))
        {
            ShowError(String.Format(GetString("om.twotestsonepageerror"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(collidingTestName)), ABTest.ABTestOriginalPage));
            return;
        }

        // Save the test with the new start date
        ABTest.ABTestOpenFrom = start;
        ABTestInfo.Provider.Set(ABTest);

        ScriptHelper.RegisterWOpenerScript(this);

        // Refresh the window opener with parameter indicating that the refresh was caused by this modal window
        ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshPage", ScriptHelper.GetScript("wopener.RefreshPage(false); CloseDialog();"));
    }

    #endregion
}
