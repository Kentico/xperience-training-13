using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.OnlineMarketing;
using CMS.OnlineMarketing.Web.UI;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_Controls_UI_AbTest_Edit : CMSAdminEditControl
{
    #region "Variables"

    private string mQueryAliasPath;
    private ABTestMessagesWriter mMessagesWriter;
    private ABTestStatusEnum? mTestStatus;

    #endregion


    #region "Properties"

    /// <summary>
    /// Strongly typed EditedObject.
    /// </summary>
    private ABTestInfo ABTest
    {
        get
        {
            return form.EditedObject as ABTestInfo;
        }
    }


    /// <summary>
    /// Status of the current test.
    /// </summary>
    private ABTestStatusEnum TestStatus
    {
        get
        {
            if (!mTestStatus.HasValue)
            {
                mTestStatus = ABTestStatusEvaluator.GetStatus(ABTest);
            }

            return mTestStatus.Value;
        }
    }


    /// <summary>
    /// Gets class that writes info messages into the page.
    /// </summary>
    private ABTestMessagesWriter MessagesWriter
    {
        get
        {
            return mMessagesWriter ?? (mMessagesWriter = new ABTestMessagesWriter(ShowMessage));
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Page.LoadComplete += Page_LoadComplete;

        SetRedirectUrlAfterCreate();

        if (ABTest.ABTestID > 0)
        {
            // If this is GET request or POST request called by modal dialog, use test values from DB
            if (!RequestHelper.IsPostBack() || (Request["__EVENTARGUMENT"] == "modalClosed"))
            {
                form.FieldControls["ABTestOpenFrom"].Value = ABTest.ABTestOpenFrom;
                form.FieldControls["ABTestOpenTo"].Value = ABTest.ABTestOpenTo;
            }
        }
    }


    protected void form_OnCreate(object sender, EventArgs e)
    {
        var editedObject = UIContext.EditedObject as ABTestInfo;
        if (editedObject != null && editedObject.ABTestID > 0)
        {
            form.AlternativeFormName = "UpdateMvc";
        }
    }


    protected void form_OnBeforeDataLoad(object sender, EventArgs e)
    {
        mQueryAliasPath = QueryHelper.GetString("AliasPath", string.Empty);
        if (!String.IsNullOrEmpty(mQueryAliasPath))
        {
            form.FieldsToHide.Add("ABTestOriginalPage");
        }
    }


    protected void form_OnBeforeSave(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(mQueryAliasPath))
        {
            form.Data.SetValue("ABTestOriginalPage", mQueryAliasPath);
        }

        form.Data.SetValue("ABTestSiteID", CurrentSite.SiteID);
    }


    protected void form_OnBeforeValidate(object sender, EventArgs e)
    {
        if (!form.IsInsertMode)
        {
            // Visitor targeting value is filled in by the condition builder so prevent it from being erased on failed validation
            form.FieldControls["ABTestVisitorTargeting"].Value = ValidationHelper.GetString(form.GetFieldValue("ABTestVisitorTargeting"), string.Empty);
        }

        if (!IsValid())
        {
            form.StopProcessing = true;
        }
    }


    protected void form_OnAfterSave(object sender, EventArgs e)
    {
        // Set the test status to null so it is re-evaluated using the new values.
        mTestStatus = null;
    }


    private void Page_LoadComplete(object sender, EventArgs e)
    {
        InitHeaderActions();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Show info labels when editing existing test 
        if (ABTest.ABTestID > 0)
        {
            MessagesWriter.ShowStatusInfo(ABTest);
            MessagesWriter.ShowABTestScheduleInformation(ABTest, TestStatus);

            DisableFieldsByTestStatus();
        }

        form.SubmitButton.Visible = false;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Disables fields not allowed in specific states of the test.
    /// </summary>
    private void DisableFieldsByTestStatus()
    {
        // Disable fields not allowed to be changed once set
        form.FieldControls["ABTestOriginalPage"].Enabled = false;

        // Disable fields not allowed if the test has been started
        bool notStarted = ((TestStatus == ABTestStatusEnum.NotStarted) || (TestStatus == ABTestStatusEnum.Scheduled));
        form.FieldControls["ABTestOpenFrom"].Enabled = notStarted;
        form.FieldControls["ABTestCulture"].Enabled = notStarted;
        form.FieldControls["ABTestConversions"].Enabled = notStarted;

        // Disable fields not allowed if the test has been finished
        bool notFinished = (TestStatus != ABTestStatusEnum.Finished);
        form.FieldControls["ABTestOpenTo"].Enabled = notFinished;
        form.FieldControls["ABTestVisitorTargeting"].Enabled = notFinished;
        form.FieldControls["ABTestIncludedTraffic"].Enabled = notFinished;
    }


    /// <summary>
    /// Checks whether the form is valid.
    /// </summary>
    private bool IsValid()
    {
        // If the test is finished, no validation is needed
        if (TestStatus == ABTestStatusEnum.Finished)
        {
            return true;
        }

        // Get page of the test so we can check for collisions
        string page = QueryHelper.GetString("AliasPath", null);
        if (String.IsNullOrEmpty(page))
        {
            page = form.GetFieldValue("ABTestOriginalPage").ToString();
        }

        // Validate original page of the test
        if (!PageExists(page))
        {
            ShowError(GetString("abtesting.testpath.pagenotfound"));
            return false;
        }

        // Create temporary test used for validation of the new values
        ABTestInfo updatedTest = new ABTestInfo
        {
            ABTestID = ABTest.ABTestID,
            ABTestOriginalPage = page,
            ABTestCulture = form.GetFieldValue("ABTestCulture")?.ToString(),
            ABTestSiteID = CurrentSite.SiteID,
        };

        updatedTest.ABTestOpenFrom = ValidationHelper.GetDateTime(form.GetFieldValue("ABTestOpenFrom"), DateTimeHelper.ZERO_TIME);

        // Validate start time if the test is not already running
        if (TestStatus != ABTestStatusEnum.Running)
        {
            if (!ABTestValidator.IsValidStart(updatedTest.ABTestOpenFrom))
            {
                ShowError(GetString("om.wrongtimeinterval"));
                return false;
            }
        }

        updatedTest.ABTestOpenTo = ValidationHelper.GetDateTime(form.GetFieldValue("ABTestOpenTo"), DateTimeHelper.ZERO_TIME);

        // Validate finish time of the test
        if (!ABTestValidator.IsValidFinish(updatedTest.ABTestOpenFrom, updatedTest.ABTestOpenTo))
        {
            ShowError(GetString("om.wrongtimeinterval"));
            return false;
        }

        // Find out possible collision - another test running on the same page, culture and at the same time
        string collidingTestName = ABTestValidator.GetCollidingTestName(updatedTest);
        if (!String.IsNullOrEmpty(collidingTestName))
        {
            ShowError(String.Format(GetString("om.twotestsonepageerror"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(collidingTestName)), updatedTest.ABTestOriginalPage));
            return false;
        }

        // If we get here, all fields are valid
        return true;
    }


    /// <summary>
    /// Sets url for redirect after creating abtest.
    /// </summary>
    private void SetRedirectUrlAfterCreate()
    {
        string url = UIContextHelper.GetElementUrl("CMS.ABTest", "Detail");

        url = URLHelper.AddParameterToUrl(url, "objectid", "{%EditedObject.ID%}");
        url = URLHelper.AddParameterToUrl(url, "tabname", "Settings");
        url = URLHelper.AddParameterToUrl(url, "saved", "1");

        url = URLHelper.PropagateUrlParameters(url, "aliaspath", "nodeid", "displayTitle", "dialog");

        url = ApplicationUrlHelper.AppendDialogHash(url);

        form.RedirectUrlAfterCreate = url;
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        var btnSave = new SaveAction
        {
            Permission = "Manage",
            ResourceName = "CMS.ABTest"
        };

        HeaderActions.AddAction(btnSave);
    }


    /// <summary>
    /// Checks if the page specified by the original variant path exists.
    /// </summary>
    /// <param name="originalVariantPath">Original variant path</param>
    private bool PageExists(string originalVariantPath)
    {
        return DocumentHelper.GetDocuments()
                               .PublishedVersion()
                               .TopN(1)
                               .All()
                               .OnCurrentSite()
                               .Culture(ABTest.ABTestCulture)
                               .CombineWithDefaultCulture(false)
                               .Columns("NodeID")
                               .WhereEquals("NodeAliasPath", originalVariantPath)
                               .Any();
    }

    #endregion
}