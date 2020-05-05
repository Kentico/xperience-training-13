using System;
using System.Linq;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Helpers.Markup;
using CMS.Modules;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_Controls_UI_MVTest_Edit : CMSAdminEditControl
{
    #region "Variables"

    private string mPreviousTestName;
    private string mQueryAliasPath;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SetRedirectUrlAfterCreate();
    }


    protected void form_OnBeforeDataLoad(object sender, EventArgs e)
    {
        mQueryAliasPath = QueryHelper.GetString("AliasPath", string.Empty);
        if (!String.IsNullOrEmpty(mQueryAliasPath))
        {
            form.FieldsToHide.Add("MVTestPage");
        }
    }


    protected void form_OnBeforeSave(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(mQueryAliasPath))
        {
            form.Data.SetValue("MVTestPage", mQueryAliasPath);
        }

        form.Data.SetValue("MVTestSiteID", CurrentSite.SiteID);
        mPreviousTestName = (form.EditedObject as MVTestInfo).MVTestName;
    }


    protected void form_OnBeforeValidate(object sender, EventArgs e)
    {
        if (!Validate())
        {
            form.StopProcessing = true;
        }
    }


    protected void form_OnAfterSave(object sender, EventArgs e)
    {
        var info = form.EditedObject as MVTestInfo;
        var newName = info.MVTestName;
        if ((info.MVTestID != 0) && (mPreviousTestName != newName))
        {
            // Name has changed. Change analytics statistics data for existing object
            MVTestInfoProvider.RenameMVTestStatistics(mPreviousTestName, newName, SiteContext.CurrentSiteID);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ShowStatusInfo(form.EditedObject as MVTestInfo);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Displays status information (running/disabled/none) about given MV test.
    /// </summary>
    private void ShowStatusInfo(MVTestInfo info)
    {
        if ((info != null) && (info.MVTestSiteID != 0))
        {
            var status = MVTestInfoProvider.GetMVTestStatus(info);
            ShowInformation(GetString("general.status") + ": " + FormattedStatusText(status));
        }
    }


    /// <summary>
    /// Returns formatted text for status information.
    /// </summary>
    /// <param name="status">MVTestStatusEnum status</param>
    private string FormattedStatusText(MVTestStatusEnum status)
    {
        var formattedText = new FormattedText(GetString("mvtest.status." + status));

        if (status == MVTestStatusEnum.Running)
        {
            formattedText.ColorGreen();
        }
        if (status == MVTestStatusEnum.Disabled)
        {
            formattedText.ColorRed();
        }

        return formattedText.ToString();
    }


    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool Validate()
    {
        DateTime from = ValidationHelper.GetDateTime(form.GetFieldValue("MVTestOpenFrom"), DateTimeHelper.ZERO_TIME);
        DateTime to = ValidationHelper.GetDateTime(form.GetFieldValue("MVTestOpenTo"), DateTimeHelper.ZERO_TIME);

        if (!DateTimeHelper.IsValidFromTo(from, to))
        {
            ShowError(GetString("om.wrongtimeinterval"));
            return false;
        }

        MVTestInfo info = new MVTestInfo
        {
            MVTestEnabled = (bool)form.GetFieldValue("MVTestEnabled"),
            MVTestSiteID = CurrentSite.SiteID,
            MVTestID = ((MVTestInfo)form.EditedObject).MVTestID,
            MVTestOpenFrom = from,
            MVTestOpenTo = to,
            MVTestCulture = (string)form.GetFieldValue("MVTestCulture"),
            MVTestPage = String.IsNullOrEmpty(QueryHelper.GetString("AliasPath", null)) ? form.GetFieldValue("MVTestPage").ToString() : QueryHelper.GetString("AliasPath", null)
        };

        MVTestInfo conflictingTest = MVTestInfoProvider.GetConflicting(info).FirstOrDefault();
        if (conflictingTest != null)
        {
            ShowError(String.Format(GetString("om.twotestsonepageerror"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(conflictingTest.MVTestDisplayName)), info.MVTestPage));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Sets url for redirect after creating abtest.
    /// </summary>
    private void SetRedirectUrlAfterCreate()
    {
        string url = UIContextHelper.GetElementUrl("CMS.MVTest", "Detail");

        url = URLHelper.AddParameterToUrl(url, "objectID", "{%EditedObject.ID%}");
        url = URLHelper.AddParameterToUrl(url, "saved", "1");

        url = URLHelper.PropagateUrlParameters(url, "aliaspath", "nodeid", "displayTitle", "dialog");
        
        url = ApplicationUrlHelper.AppendDialogHash(url);

        form.RedirectUrlAfterCreate = url;
    }

    #endregion
}