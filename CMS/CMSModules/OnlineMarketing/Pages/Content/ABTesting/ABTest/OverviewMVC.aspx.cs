using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.OnlineMarketing;
using CMS.OnlineMarketing.Internal;
using CMS.OnlineMarketing.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

using Newtonsoft.Json;

/// <summary>
/// AB test Overview page for MVC sites, which shows graphs and listing for all variants for the test.
/// Shows data like conversion rate, conversion count, conversion value, and user can switch between cultures, conversions (because we log all conversions for all tests),
/// whether the graph will show data in cumulative or day-wise manner and other.
/// </summary>
/// <remarks>Saves state of selectors to a cookie</remarks>
[Security(Resource = ModuleName.ABTEST, UIElements = "OverviewMVC")]
[Security(Resource = ModuleName.ABTEST, UIElements = "Detail")]
[UIElement(ModuleName.ABTEST, "OverviewMVC")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_OverviewMVC : CMSABTestOverviewPage
{
    #region "Variables"

    /// <summary>
    /// Statistics data of the current AB test's variants.
    /// </summary>
    private Dictionary<Guid, ABVariantStatisticsData> mVariantsStatisticsData;


    /// <summary>
    /// Conversion rate intervals of the current AB test's variants.
    /// </summary>
    private Dictionary<Guid, ABConversionRateInterval> mABConversionRateIntervals;


    private string drpConversionsValue;


    private int samplingIndex, graphDataIndex, drpSuccessMetricIndex, drpCountingMethodologyIndex;


    private readonly IABTestManager abTestManager = Service.Resolve<IABTestManager>();
    private CMS.DocumentEngine.TreeNode mABTestPage;
    private bool abTestPageLoaded;


    private bool UserHasPermissions { get; } =
            MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ABTEST, "Manage") &&
            MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.CONTENT, "Modify");

    #endregion


    #region "Properties"

    /// <summary>
    /// Variants of the current AB test.
    /// </summary>
    private List<ABVariantDataInfo> ABVariants
    {
        get
        {
            return ABCachedObjects.GetVariantsData(ABTest);
        }
    }


    /// <summary>
    /// Original variant of the current AB test.
    /// </summary>
    private ABVariantDataInfo OriginalVariant
    {
        get
        {
            return ABVariants?.FirstOrDefault(v => v.ABVariantIsOriginal);
        }
    }


    /// <summary>
    /// Page the A/B test tests.
    /// </summary>
    private CMS.DocumentEngine.TreeNode ABTestPage
    {
        get
        {
            if (!abTestPageLoaded)
            {
                mABTestPage = ABTestHelper.GetABTestPage(ABTest);

                abTestPageLoaded = true;
            }

            return mABTestPage;
        }
    }


    /// <summary>
    /// Instance of IABVariantPerformanceCalculator implementation.
    /// </summary>
    private IABVariantPerformanceCalculator VariantPerformanceCalculator
    {
        get
        {
            if (mVariantPerformanceCalculator == null)
            {
                if (VariantsStatisticsData.ContainsKey(OriginalVariant.ABVariantGUID))
                {
                    var variantData = VariantsStatisticsData[OriginalVariant.ABVariantGUID];

                    if ((variantData.Visits > 0) && (variantData.ConversionsCount >= 0))
                    {
                        mVariantPerformanceCalculator = ABVariantPerformanceCalculatorFactory.GetImplementation(variantData.ConversionsCount, variantData.Visits);
                    }
                }
            }

            return mVariantPerformanceCalculator;
        }
    }


    /// <summary>
    /// Gets dictionary that holds statistics data of variants.
    /// </summary>
    private Dictionary<Guid, ABVariantStatisticsData> VariantsStatisticsData
    {
        get
        {
            return mVariantsStatisticsData ?? (mVariantsStatisticsData = new Dictionary<Guid, ABVariantStatisticsData>());
        }
    }


    /// <summary>
    /// Gets dictionary that holds conversion rate intervals of variants.
    /// </summary>
    private Dictionary<Guid, ABConversionRateInterval> ABConversionRateIntervals
    {
        get
        {
            return mABConversionRateIntervals ?? (mABConversionRateIntervals = new Dictionary<Guid, ABConversionRateInterval>());
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (ABTest == null)
        {
            RedirectToInformation(GetString("general.incorrectURLparams"));
        }

        InitSamplingButtons(samplingElem);
        InitGraphDataButtons(graphDataElem);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Controls.Add(DocumentManager as CMSDocumentManager);

        MessagesWriter.ShowABTestScheduleInformation(ABTest, TestStatus);

        RegisterScripts();
        InitializeSelectors();
        SetSelectorValues();
        EnsureVariantsStatisticsData();

        // Display test winner if there is one
        var winner = GetTestWinner();
        if (winner != null)
        {
            DisplayWinnerInformation(winner);
        }
        else
        {
            ShowPromoteWinnerInfoMessage();
        }

        // Hide summary and table if the test has not been started yet
        if (ABTestStatusEvaluator.ABTestNotStarted(TestStatus))
        {
            Filter.Visible = false;
            Report.Visible = false;
            Summary.Visible = false;
        }
        else if (DataAvailable(VariantsStatisticsData.Values, drpSuccessMetric.SelectedValue))
        {
            // Show all information after graph do postback
            if (RequestHelper.IsPostBack())
            {
                Summary.Visible = true;
                Report.Visible = true;

                // Hide NoDataFound panel
                pnlNoData.Visible = false;
            }
        }
        else
        {
            // -> Test is executed, but no results are present

            // Hide summary
            Summary.Visible = false;

            // Show report and NoDataFound panel
            Report.Visible = true;
            pnlNoData.Visible = true;
        }

        LoadSummaryBox();
        InitializeGraph();
        InitializeGrid();
        SetImprovementColumnCaption();
        ShowInvalidFilterCombinationWarning();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // OnPreRender because test status can be changed later, not only in the Load event
        InitHeaderActions();
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // Handle the grid action first because it doesn't require access to VariantsStatisticsData
        if (sourceName == "selectwinner")
        {
            if (parameter is GridViewRow gridViewRow && gridViewRow.DataItem is DataRowView dataRowView)
            {
                var action = sender as CMSAccessibleButtonBase;
                var variantGuid = ValidationHelper.GetGuid(dataRowView["ABVariantGUID"], Guid.Empty);

                InitializeGridAction(action, variantGuid);
            }

            return string.Empty;
        }

        var currentVariantGuid = ValidationHelper.GetGuid(parameter, Guid.Empty);

        if (currentVariantGuid == Guid.Empty || (OriginalVariant == null) || !VariantsStatisticsData.ContainsKey(currentVariantGuid))
        {
            return string.Empty;
        }

        var variantData = VariantsStatisticsData[currentVariantGuid];

        switch (sourceName)
        {
            case "name":
                var variant = ABVariants.FirstOrDefault(v => v.ABVariantGUID == currentVariantGuid);
                if (variant != null)
                {
                    return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(variant.ABVariantDisplayName));
                }
                break;

            case METRIC_CONVERSIONS_OVER_VISITS:
                return variantData.ConversionsCount + " / " + variantData.Visits;

            case METRIC_CHANCE_TO_BEAT_ORIGINAL:
                if ((currentVariantGuid != OriginalVariant.ABVariantGUID) && (VariantPerformanceCalculator != null) && (variantData.Visits > 0))
                {
                    double chanceToBeatOriginal = VariantPerformanceCalculator.GetChanceToBeatOriginal(variantData.ConversionsCount, variantData.Visits);

                    // Check whether the variant is most probably winning already and mark the row green
                    if ((chanceToBeatOriginal >= WINNING_VARIANT_MIN_CHANCETOBEAT) && (variantData.ConversionsCount >= WINNING_VARIANT_MIN_CONVERSIONS))
                    {
                        AddCSSToParentControl(sender as WebControl, "winning-variant-row");
                    }

                    return String.Format("{0:P2}", chanceToBeatOriginal);
                }
                break;

            case METRIC_CONVERSION_RATE:
                if ((VariantPerformanceCalculator != null) && (variantData.Visits > 0)
                    && ABConversionRateIntervals.ContainsKey(currentVariantGuid) && ABConversionRateIntervals.ContainsKey(OriginalVariant.ABVariantGUID))
                {
                    // Render the picture representing how the challenger variant is performing against the original variant
                    return new ABConversionRateIntervalVisualizer(
                        mMinConversionRateLowerBound, mConversionRateRange, ABConversionRateIntervals[currentVariantGuid], ABConversionRateIntervals[OriginalVariant.ABVariantGUID]);
                }
                break;

            case METRIC_CONVERSION_VALUE:
                return variantData.ConversionsValue;

            case METRIC_AVG_CONVERSION_VALUE:
                return String.Format("{0:#.##}", variantData.AverageConversionValue);

            case "improvement":
                if ((currentVariantGuid != OriginalVariant.ABVariantGUID) && VariantsStatisticsData.ContainsKey(OriginalVariant.ABVariantGUID))
                {
                    var originalData = VariantsStatisticsData[OriginalVariant.ABVariantGUID];
                    switch (drpSuccessMetric.SelectedValue)
                    {
                        case METRIC_CONVERSION_COUNT:
                            if (!originalData.ConversionsCount.Equals(0))
                            {
                                return GetPercentageImprovementPanel((variantData.ConversionsCount / (double)originalData.ConversionsCount) - 1);
                            }
                            break;

                        case METRIC_CONVERSION_VALUE:
                            if (!originalData.ConversionsValue.Equals(0))
                            {
                                return GetPercentageImprovementPanel((variantData.ConversionsValue / originalData.ConversionsValue) - 1);
                            }
                            break;

                        case METRIC_CONVERSION_RATE:
                            if (!originalData.ConversionRate.Equals(0))
                            {
                                return GetPercentageImprovementPanel((variantData.ConversionRate / originalData.ConversionRate) - 1);
                            }
                            break;

                        case METRIC_AVG_CONVERSION_VALUE:
                            if (!originalData.AverageConversionValue.Equals(0))
                            {
                                return GetPercentageImprovementPanel((variantData.AverageConversionValue / originalData.AverageConversionValue) - 1);
                            }
                            break;
                    }
                }
                break;
        }

        return string.Empty;
    }


    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (!IsUserAuthorizedToManageTest)
        {
            RedirectToAccessDenied(ModuleName.ABTEST, "Manage");
        }

        switch (actionName)
        {
            case "selectwinner":
                SelectWinner(ValidationHelper.GetGuid(actionArgument, Guid.Empty));
                break;
        }
    }

    #endregion

    #region "Protected methods"

    /// <summary>
    /// Creates a document manager for the page which is the subject of this A/B test.
    /// </summary>
    protected override ICMSDocumentManager CreateDocumentManager()
    {
        if (ABTestPage == null)
        {
            return base.CreateDocumentManager();
        }

        return new CMSDocumentManager
        {
            ID = "docMan",
            ShortID = "DM",
            IsLiveSite = false,
            NodeID = ABTestPage.DocumentNodeID,
            CultureCode = ABTest.ABTestCulture,
            DocumentID = ABTestPage.DocumentID,
            Tree = Tree,
            StopProcessing = true
        };
    }

    #endregion

    #region "Private methods"

    /// <summary>
    /// Initializes selectors.
    /// </summary>
    private void InitializeSelectors()
    {
        // Load conversion goals selector
        var testConversions = ABTest.ABTestConversionConfiguration.ABTestConversions;

        drpConversions.Items.Clear();
        drpConversions.Items.Add(new ListItem(GetString("general.selectall"), ""));

        foreach (var conversion in testConversions)
        {
            var conversionOriginalDisplayName = ResHelper.LocalizeString(ABTestConversionDefinitionRegister.Instance.Get(conversion.ConversionOriginalName)?.ConversionDisplayName);
            var conversionRelatedItemDisplayName = String.IsNullOrEmpty(conversion.RelatedItemDisplayName) ? String.Empty : $" '{conversion.RelatedItemDisplayName}'";
            var conversionDisplayName = $"{conversionOriginalDisplayName}{conversionRelatedItemDisplayName}";
            drpConversions.Items.Add(new ListItem(conversionDisplayName, conversion.ConversionName));
        }

        ParseSelectorCookie();
    }


    /// <summary>
    /// Initializes unigrid.
    /// </summary>
    private void InitializeGrid()
    {
        gridElem.DataSource = new InfoDataSet<ABVariantDataInfo>(ABVariants.ToArray());
        gridElem.GridView.AddCssClass("rows-middle-vertical-align");

        if (!DataAvailable(VariantsStatisticsData.Values, drpSuccessMetric.SelectedValue))
        {
            columnConversionsOverVisits.Visible = false;
        }

        if (drpSuccessMetric.SelectedValue == METRIC_CONVERSION_VALUE)
        {
            columnConversionValue.Visible = true;

            // Hide chance to beat original and conversion rate interval columns as they are related to conversion rate
            HideChanceToBeatAndRateColumns();
        }
        else if (drpSuccessMetric.SelectedValue == METRIC_AVG_CONVERSION_VALUE)
        {
            columnAvgConversionValue.Visible = true;

            // Hide chance to beat original and conversion rate interval columns as they are related to conversion rate
            HideChanceToBeatAndRateColumns();
        }

        if (ValidationHelper.GetString(drpConversions.SelectedValue, "") == "")
        {
            // Hide chance to beat original and conversion rate interval columns as they don't provide relevant data when all conversions are selected
            HideChanceToBeatAndRateColumns();

            if (drpSuccessMetric.SelectedValue == METRIC_CONVERSION_RATE)
            {
                columnImprovement.Visible = false;
            }
        }

        // If Visitors conversion methodology selected, use "Visitors" instead of "Visits" in unigrid
        if (drpCountingMethodology.SelectedValue == ABTestConstants.ABSESSIONCONVERSION_FIRST)
        {
            columnConversionsOverVisits.Caption = GetString("abtesting.conversionsovervisitors");
        }
    }


    private void InitializeGridAction(CMSAccessibleButtonBase action, Guid variantGuid)
    {
        if (action == null)
        {
            return;
        }

        var winner = GetTestWinner();

        if (winner == null)
        {
            // Check permissions to select winner
            if (!UserHasPermissions)
            {
                action.Enabled = false;
                action.ToolTip = GetString("abtesting.selectwinner.permission.tooltip");
            }
            else if (!ABTestStatusEvaluator.ABTestIsFinished(ABTest))
            {
                action.Enabled = false;
                action.ToolTip = GetString("abtesting.selectwinner.testnotfinished.tooltip");
            }
            else if (!DocumentManager.AllowSave)
            {
                action.Enabled = false;
                action.ToolTip = GetString("abtesting.selectwinner.underworkflow.tooltip");
            }
        }
        else
        {
            if (variantGuid == winner.ABVariantGUID)
            {
                // Disable action image for the winning variant
                action.Enabled = false;
                action.Text = ResHelper.GetString("abtesting.winner");
                action.ToolTip = ResHelper.GetString("abtesting.winningvariantselected.tooltip");
            }
            else
            {
                // Hide action image for other variants
                action.Visible = false;
            }
        }
    }


    /// <summary>
    /// Hides chance to beat original and conversion rate columns.
    /// </summary>
    private void HideChanceToBeatAndRateColumns()
    {
        columnChanceToBeatOriginal.Visible = false;
        columnConversionRateInterval.Visible = false;
    }


    /// <summary>
    /// Initializes graph.
    /// </summary>
    private void InitializeGraph()
    {
        var data = InitGraphColumns();
        var conversionName = String.IsNullOrEmpty(drpConversions.SelectedValue) ? ABTest.ABTestConversionConfiguration.ABTestConversions.Select(c => c.ConversionName).Join(";") : drpConversions.SelectedValue;

        object[] parameters =
        {
            ABTest.ABTestOpenFrom,
            GetFinishDate(),
            ABTest.ABTestName,
            conversionName,
            graphDataElem.SelectedActionName,
            ABTest.ABTestID,
            String.Empty,
            String.Empty,
            drpCountingMethodology.SelectedValue
        };

        var dataRow = data.Rows.Add(parameters);
        data.AcceptChanges();

        displayReport.Colors = ABVariantColorAssigner.GetColors(ABTest);
        displayReport.LoadFormParameters = false;
        displayReport.DisplayFilter = false;
        displayReport.GraphImageWidth = 100;
        displayReport.IgnoreWasInit = true;
        displayReport.UseExternalReload = true;
        displayReport.UseProgressIndicator = true;
        displayReport.SelectedInterval = samplingElem.SelectedActionName;
        displayReport.ReportParameters = dataRow;
        displayReport.ReportName = "mvcabtest" + drpSuccessMetric.SelectedValue + "." + samplingElem.SelectedActionName + "report";
    }


    /// <summary>
    /// Registers scripts.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterScriptFile(Page, "DesignMode/PortalManager.js");
        ScriptHelper.RegisterModule(Page, "CMS.OnlineMarketing/ABTestOverview", new
        {
            samplingElementId = samplingElem.ID,
            samplingElementClientId = samplingElem.ClientID,
            graphDataElementId = graphDataElem.ID,
            graphDataElementClientId = graphDataElem.ClientID,
            successMetricDropdownId = drpSuccessMetric.ID,
            successMetricDropdownClientId = drpSuccessMetric.ClientID,
            countingMethodologyDropdownId = drpCountingMethodology.ID,
            countingMethodologyDropdownClientId = drpCountingMethodology.ClientID,
            conversionsDropdownId = drpConversions.ID,
            conversionsDropdownClientId = drpConversions.ClientID,
            selectorsCookieKey = SelectorsCookieKey,
            showFiltersText = GetString(SHOW_FILTERS_TEXT),
            hideFiltersText = GetString(HIDE_FILTERS_TEXT)
        });
    }


    /// <summary>
    /// Gets winning variant of the test.
    /// </summary>
    private ABVariantDataInfo GetTestWinner()
    {
        return ABVariants.FirstOrDefault(v => v.ABVariantGUID == ABTest.ABTestWinnerGUID);
    }


    /// <summary>
    /// Selects variant as winner.
    /// </summary>
    /// <param name="winnerGuid">Winner variant GUID</param>
    private void SelectWinner(Guid winnerGuid)
    {
        if (winnerGuid == Guid.Empty || OriginalVariant == null)
        {
            ShowError(GetString("abtesting.pushwinner.error.general"));
            return;
        }

        var variant = ABVariants.FirstOrDefault(v => v.ABVariantGUID == winnerGuid);
        if (variant == null)
        {
            ShowError(GetString("abtesting.pushwinner.error.general"));
            return;
        }

        if (!UserHasPermissions || !DocumentManager.AllowSave)
        {
            ShowError(GetString("abtesting.pushwinner.error.general"));
            return;
        }

        // Set winner GUID to ABTest
        ABTest.ABTestWinnerGUID = variant.ABVariantGUID;

        using (var transaction = new CMSTransactionScope())
        {
            abTestManager.PromoteVariant(DocumentManager.Node, variant.ABVariantGUID);
            DocumentManager.SaveDocument();

            transaction.Commit();
        }

        DisplayWinnerInformation(variant);

        // Reload data because HeaderActions_ActionPerformed event is too late to change action tooltip
        gridElem.ReloadData();

        // Reload summary box based on new status
        LoadSummaryBox();
    }


    /// <summary>
    /// Displays info label about the winner of the test.
    /// </summary>
    /// <param name="winner">AB test winner</param>
    private void DisplayWinnerInformation(ABVariantDataInfo winner)
    {
        ShowInformation(String.Format(GetString("abtesting.winningvariantselected"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(winner.ABVariantDisplayName))));
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        if (ABTestPage == null)
        {
            ShowWarning(GetString("abtesting.missingpage"));

            return;
        }

        switch (TestStatus)
        {
            case ABTestStatusEnum.NotStarted:
            case ABTestStatusEnum.Scheduled:
                AddStartTestButton();
                break;
        }
    }


    /// <summary>
    /// Adds the "Start test" button. If the tested page is under workflow, the button's enabled state is based on the page's published state.
    /// </summary>
    private void AddStartTestButton()
    {
        string testStartUrl = ResolveUrl("~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABTest/StartABTest.aspx?testid=" + ABTest.ABTestID);

        var pageIsPublished = DocumentManager.Step?.StepIsPublished ?? true;

        var tooltip = !IsUserAuthorizedToManageTest ? GetString("abtesting.starttest.tooltip.permissionrequired") :
            pageIsPublished ? GetString("abtesting.starttest.tooltip") : GetString("abtesting.starttest.tooltip.disabled");

        var btnStartTest = new HeaderAction
        {
            Tooltip = tooltip,
            Text = GetString("abtesting.starttest"),
            OnClientClick = "modalDialog('" + testStartUrl + @"', '', 670, 320);",
            Enabled = IsUserAuthorizedToManageTest && pageIsPublished
        };

        HeaderActions.AddAction(btnStartTest);
    }


    /// <summary>
    /// Gets selector values from the cookie.
    /// </summary>
    private void ParseSelectorCookie()
    {
        // Get cookie value
        string json = CookieHelper.GetValue(SelectorsCookieKey);

        if (json == null)
        {
            return;
        }

        try
        {
            // Deserialize cookie value
            var selectorsState = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var selectorConversionValue = selectorsState[drpConversions.ID];

            // Parse cookie value
            samplingIndex = ValidationHelper.GetInteger(selectorsState[samplingElem.ID], 0);
            drpSuccessMetricIndex = ValidationHelper.GetInteger(selectorsState[drpSuccessMetric.ID], 0);
            drpCountingMethodologyIndex = ValidationHelper.GetInteger(selectorsState[drpCountingMethodology.ID], 0);
            graphDataIndex = ValidationHelper.GetInteger(selectorsState[graphDataElem.ID], 0);
            drpConversionsValue = drpConversions.Items.FindByValue(selectorConversionValue) != null ? selectorConversionValue : String.Empty;
        }
        catch
        {
            // If something failed, delete the cookie
            CookieHelper.Remove(SelectorsCookieKey);
        }
    }


    /// <summary>
    /// Sets selector values to the values used in the previous page visit.
    /// </summary>
    private void SetSelectorValues()
    {
        samplingElem.SelectedActionName = SamplingActions[samplingIndex];
        drpSuccessMetric.SelectedIndex = drpSuccessMetricIndex;
        drpCountingMethodology.SelectedIndex = drpCountingMethodologyIndex;
        graphDataElem.SelectedActionName = DataFormatActions[graphDataIndex];
        drpConversions.SelectedValue = drpConversionsValue;
    }


    /// <summary>
    /// Loads summary box in the right upper corner.
    /// </summary>
    private void LoadSummaryBox()
    {
        var selectionParameters = new NodeSelectionParameters
        {
            AliasPath = ABTest.ABTestOriginalPage,
            CultureCode = ABTest.ABTestCulture,
            SiteName = SiteContext.CurrentSiteName,
            SelectOnlyPublished = true,
            CombineWithDefaultCulture = false
        };

        var node = new TreeProvider().SelectSingleNode(selectionParameters);
        var shortUrl = ShortenUrl(ABTest.ABTestOriginalPage);
        var encodedShortUrl = HTMLHelper.HTMLEncode(shortUrl);

        if (node == null)
        {
            lblTest.Text = encodedShortUrl;
            lblTest.Visible = true;
        }
        else
        {
            lnkTest.HRef = DocumentURLProvider.GetAbsoluteUrl(node);
            lnkTest.InnerText = encodedShortUrl;
            lnkTest.Target = "_blank";
            lnkTest.Visible = true;
        }

        // If Visitors conversion methodology selected, use "Visitors" instead of "Visits"
        if (drpCountingMethodology.SelectedValue == ABTestConstants.ABSESSIONCONVERSION_FIRST)
        {
            lblVisits.ResourceString = "abtesting.overview.summary.visitors";
        }

        lblStatus.Text = ABTestStatusEvaluator.GetFormattedStatus(TestStatus).ToString();
        int visits = VariantsStatisticsData.Sum(d => d.Value.Visits);
        int conversions = VariantsStatisticsData.Sum(d => d.Value.ConversionsCount);
        lblTotalVisitors.Text = String.Format("{0:N0}", visits);
        lblTotalConversions.Text = String.Format("{0:N0}", conversions);

        if (TestStatus == ABTestStatusEnum.Finished)
        {
            txtDuration.ResourceString = "abtesting.daysrun";
        }

        DateTime start = ABTest.ABTestOpenFrom;
        DateTime finish = GetFinishDate();
        lblDuration.Text = (finish - start).Days.ToString();
    }


    /// <summary>
    /// Ensures statistics data for AB variants.
    /// </summary>
    private void EnsureVariantsStatisticsData()
    {
        if (OriginalVariant == null)
        {
            return;
        }

        // Ensure data for the original variant so that challenger variants can be compared to it
        EnsureVariantStatisticsData(OriginalVariant.ABVariantGUID);

        foreach (var variant in ABVariants)
        {
            EnsureVariantStatisticsData(variant.ABVariantGUID);
        }

        // Calculate minimum lower bound and rate range for interval visualization
        if (ABConversionRateIntervals.Count > 0)
        {
            double minConversionRateLowerBound = ABConversionRateIntervals.Values.Min(i => i.ConversionRateLowerBound);
            double maxConversionRateUpperBound = ABConversionRateIntervals.Values.Max(i => i.ConversionRateUpperBound);
            mConversionRateRange = maxConversionRateUpperBound - minConversionRateLowerBound;
            mMinConversionRateLowerBound = minConversionRateLowerBound;
        }
    }


    /// <summary>
    /// Ensures statistics data for specific AB variant.
    /// </summary>
    /// <param name="variantGuid">Variant GUID</param>
    private void EnsureVariantStatisticsData(Guid variantGuid)
    {
        if (!VariantsStatisticsData.ContainsKey(variantGuid))
        {
            // Select both abvisitfirst and abvisitreturn by default
            string visitType = "abvisit%";

            // If counting methodology is set to visitor conversion, select abvisitfirst only
            string countingMethodology = drpCountingMethodology.Items[drpCountingMethodologyIndex].Value;
            if (countingMethodology == ABTestConstants.ABSESSIONCONVERSION_FIRST)
            {
                visitType = "abvisitfirst";
            }

            string conversionsCodename = countingMethodology + ";" + ABTest.ABTestName + ";" + variantGuid;
            string visitsCodename = visitType + ";" + ABTest.ABTestName + ";" + variantGuid;
            string conversion = ValidationHelper.GetString(drpConversions.SelectedValue, string.Empty);
            var testConversions = ABTest.ABTestConversionConfiguration.ABTestConversions.Select(c => c.ConversionName);

            // Get conversions count and value
            DataRow conversions = GetHits(conversionsCodename, "Sum(HitsCount), Sum(HitsValue)", null, GetConversionCondition(conversion, testConversions));
            int conversionsCount = ValidationHelper.GetInteger(conversions[0], 0);
            double conversionsValue = ValidationHelper.GetDouble(conversions[1], 0);

            // Get visits count
            int visits = ValidationHelper.GetInteger(GetHits(visitsCodename, "Sum(HitsCount)", null)[0], 0);

            // Add statistics data
            VariantsStatisticsData.Add(variantGuid, new ABVariantStatisticsData(conversionsCount, conversionsValue, visits));

            // Add conversion rate intervals
            if ((VariantPerformanceCalculator != null) && (visits > 0) && (conversionsCount <= visits))
            {
                ABConversionRateIntervals.Add(variantGuid, VariantPerformanceCalculator.GetConversionRateInterval(conversionsCount, visits));
            }
        }
    }


    /// <summary>
    /// Sets caption of the percentage improvement column.
    /// </summary>
    private void SetImprovementColumnCaption()
    {
        string caption = string.Empty;

        // Set improvement caption according to the selected success metric
        switch (drpSuccessMetric.SelectedValue)
        {
            case METRIC_CONVERSION_COUNT:
                caption = "$abtesting.improvement.conversioncount$";
                break;

            case METRIC_CONVERSION_VALUE:
                caption = "$abtesting.improvement.conversionvalue$";
                break;

            case METRIC_CONVERSION_RATE:
                caption = "$abtesting.improvement.conversionrate$";
                break;

            case METRIC_AVG_CONVERSION_VALUE:
                caption = "$abtesting.improvement.averageconversionvalue$";
                break;
        }

        // Set caption to improvement column
        columnImprovement.Caption = caption;
    }


    /// <summary>
    /// Shows warning image on invalid filter combination.
    /// Invalid filter combination is "All conversion goals" and "Conversion rate".
    /// </summary>
    private void ShowInvalidFilterCombinationWarning()
    {
        // Do not show for not started test
        if (ABTestStatusEvaluator.ABTestNotStarted(TestStatus))
        {
            return;
        }

        // Do not show on first load
        if (!RequestHelper.IsPostBack())
        {
            return;
        }

        if ((ValidationHelper.GetString(drpConversions.SelectedValue, "") == "") && (drpSuccessMetric.SelectedValue == METRIC_CONVERSION_RATE))
        {
            ShowWarning(GetString("abtesting.invalidfiltercombination"));
        }
    }


    /// <summary>
    /// Shows info message to prompt the user to select the winning A/B variant after the A/B test has finished.
    /// </summary>
    private void ShowPromoteWinnerInfoMessage()
    {
        if (TestStatus != ABTestStatusEnum.Finished || !UserHasPermissions)
        {
            return;
        }

        string message;

        if (DocumentManager.AllowSave || IsDialog)
        {
            message = GetString("abtesting.finishedtest.promotewinnerpromptmessage");
        }
        else
        {
            string siteName = SiteInfoProvider.GetSiteName(ABTest.ABTestSiteID);
            int nodeId = TreePathUtils.GetNodeIdByAliasPath(siteName, ABTest.ABTestOriginalPage);
            string editPageLink = ApplicationUrlHelper.GetPageEditLink(nodeId, ABTest.ABTestCulture);
            var link = URLHelper.ResolveUrl(editPageLink);

            message = ResHelper.GetStringFormat("abtesting.finishedtest.promotewinnerpromptmessage.warning", link);
        }

        ShowInformation(message);
    }

    #endregion
}