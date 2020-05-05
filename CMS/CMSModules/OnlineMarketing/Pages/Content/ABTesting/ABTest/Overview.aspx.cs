using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.OnlineMarketing.Internal;
using CMS.OnlineMarketing.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

using Newtonsoft.Json;

using Action = CMS.UIControls.UniGridConfig.Action;

/// <summary>
/// AB test Overview page, which shows graphs and listing for all variants for the test.
/// Shows data like conversion rate, conversion count, conversion value, and user can switch between cultures, conversions (because we log all conversions for all tests),
/// whether the graph will show data in cumulative or day-wise manner and other.
/// </summary>
/// <remarks>Saves state of selectors to a cookie</remarks>
[Security(Resource = "CMS.ABTest", UIElements = "Overview")]
[Security(Resource = "CMS.ABTest", UIElements = "Detail")]
[UIElement("CMS.ABTest", "Overview")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_Overview : CMSABTestOverviewPage
{
    #region "Variables"

    /// <summary>
    /// Variants of the current AB test.
    /// </summary>
    private List<ABVariantInfo> mABVariants;


    /// <summary>
    /// Statistics data of the current AB test's variants.
    /// </summary>
    private Dictionary<string, ABVariantStatisticsData> mVariantsStatisticsData;


    /// <summary>
    /// Conversion rate intervals of the current AB test's variants.
    /// </summary>
    private Dictionary<string, ABConversionRateInterval> mABConversionRateIntervals;


    /// <summary>
    /// Original variant of the current AB test.
    /// </summary>
    private ABVariantInfo mOriginalVariant;


    private string mDrpConversionsValue, mDrpCultureValue;


    private int mSamplingIndex, mGraphDataIndex, mDrpSuccessMetricIndex, mDrpCountingMethodologyIndex;


    private bool mAdvancedcontrolsVisible;

    #endregion


    #region "Constants"

    /// <summary>
    /// Smart tip identifier. If this smart tip is collapsed, this ID is stored in DB.
    /// </summary>
    private const string SMART_TIP_IDENTIFIER = "howtovideo|abtest|overview";

    #endregion


    #region "Properties"

    /// <summary>
    /// Variants of the current AB test.
    /// </summary>
    private List<ABVariantInfo> ABVariants
    {
        get
        {
            return mABVariants ?? (mABVariants = ABCachedObjects.GetVariants(ABTest));
        }
    }


    /// <summary>
    /// Original variant of the current AB test.
    /// </summary>
    private ABVariantInfo OriginalVariant
    {
        get
        {
            if (mOriginalVariant == null)
            {
                if (ABVariants != null)
                {
                    foreach (var variant in ABVariants)
                    {
                        if (variant.ABVariantPath == ABTest.ABTestOriginalPage)
                        {
                            mOriginalVariant = variant;
                            break;
                        }
                    }
                }
            }

            return mOriginalVariant;
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
                if (VariantsStatisticsData.ContainsKey(OriginalVariant.ABVariantName))
                {
                    var variantData = VariantsStatisticsData[OriginalVariant.ABVariantName];

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
    private Dictionary<string, ABVariantStatisticsData> VariantsStatisticsData
    {
        get
        {
            return mVariantsStatisticsData ?? (mVariantsStatisticsData = new Dictionary<string, ABVariantStatisticsData>());
        }
    }


    /// <summary>
    /// Gets dictionary that holds conversion rate intervals of variants.
    /// </summary>
    private Dictionary<string, ABConversionRateInterval> ABConversionRateIntervals
    {
        get
        {
            return mABConversionRateIntervals ?? (mABConversionRateIntervals = new Dictionary<string, ABConversionRateInterval>());
        }
    }

    #endregion


    #region "Events"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (ABTest == null)
        {
            RedirectToInformation(GetString("general.incorrectURLparams"));
        }

        InitSamplingButtons(samplingElem);
        InitGraphDataButtons(graphDataElem);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        MessagesWriter.ShowABTestScheduleInformation(ABTest, TestStatus);
        MessagesWriter.ShowMissingVariantsTranslationsWarning(ABTest);

        ScriptHelper.RegisterDialogScript(Page);
        EnsureModuleRegistration();
        InitializeSelectors();
        InitSmartTip();

        // Hide summary and table if the test has not been started yet
        if ((ABTest.ABTestOpenFrom > DateTime.Now) || (ABTest.ABTestOpenFrom == DateTimeHelper.ZERO_TIME))
        {
            Summary.Visible = false;
            gridElem.Visible = false;
            return;
        }

        // Display test winner if there is one
        var winner = GetTestWinner();
        if (winner != null)
        {
            DisplayWinnerInformation(winner);
            SetWinnerTooltip();
        }

        EnsureVariantsStatisticsData();
        if (DataAvailable(VariantsStatisticsData.Values, drpSuccessMetric.SelectedValue))
        {
            // Add class to the report because graph with data requires special positioning
            // Show all information after graph do postback
            if (RequestHelper.IsPostBack())
            {
                Summary.Visible = true;
                gridElem.Visible = true;
                gridElem.GridView.AddCssClass("rows-middle-vertical-align");

                // Hide NoDataFound panel
                pnlNoData.Visible = false;
            }
        }
        else
        {
            // Hide summary and table
            Summary.Visible = false;
            gridElem.Visible = false;

            // Show NoDataFound panel
            pnlNoData.Visible = true;
            return;
        }

        LoadSummaryBox();
        InitializeGraph();
        InitializeGrid();
        SetImprovementColumnCaption();
        ShowInvalidFilterCombinationImage();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Reload conversions list to show only conversions with data
        drpConversions.ReloadData(false);

        // Pre-select first conversion, load selector state might change it later
        // It has to be in PreRender, because items in DropDownSingleSelect are not yet loaded in PageLoad event
        if (drpConversions.UniSelector.DropDownSingleSelect.Items.Count > 1)
        {
            drpConversions.Value = drpConversions.UniSelector.DropDownSingleSelect.Items[1].Value;
        }

        SetSelectorValues();

        // Select test culture in the selector
        if (!String.IsNullOrEmpty(ABTest.ABTestCulture))
        {
            drpCulture.Value = ABTest.ABTestCulture;
            drpCulture.Enabled = false;
        }

        // OnPreRender because test status can be changed later, not only in the Load event
        InitHeaderActions();

        ScriptHelper.RegisterScriptFile(Page, "DesignMode/PortalManager.js");
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (!RequestHelper.IsPostBack())
        {
            return string.Empty;
        }

        // Handle the grid action first because it doesn't require access to VariantsStatisticsData
        if (sourceName == "selectwinner")
        {
            var gridViewRow = parameter as GridViewRow;
            if (gridViewRow != null)
            {
                var dataRowView = gridViewRow.DataItem as DataRowView;
                if (dataRowView != null)
                {
                    var img = sender as CMSGridActionButton;
                    if (img != null)
                    {
                        // Check permissions to select winner
                        if (!IsUserAuthorizedToManageTest)
                        {
                            img.Enabled = false;
                            img.ToolTip = GetString("abtesting.selectwinner.permission.tooltip");
                        }
                        else
                        {
                            var winner = GetTestWinner();
                            if (winner != null)
                            {
                                string variantName = (ValidationHelper.GetString(dataRowView["ABVariantName"], ""));
                                if (variantName == winner.ABVariantName)
                                {
                                    // Disable action image for the winning variant
                                    img.Enabled = false;
                                }
                                else
                                {
                                    // Hide action image for other variants
                                    img.Visible = false;
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        string currentVariantName = parameter.ToString();

        if (String.IsNullOrEmpty(currentVariantName) || (OriginalVariant == null) || !VariantsStatisticsData.ContainsKey(currentVariantName))
        {
            return string.Empty;
        }

        var variantData = VariantsStatisticsData[currentVariantName];

        switch (sourceName)
        {
            case "name":
                var variant = ABVariants.FirstOrDefault(v => v.ABVariantName == currentVariantName);
                if (variant != null)
                {
                    var link = new HtmlAnchor();
                    link.InnerText = ResHelper.LocalizeString(variant.ABVariantDisplayName);
                    link.HRef = "";
                    link.Target = "_blank";
                    return link;
                }
                break;

            case METRIC_CONVERSIONS_OVER_VISITS:
                return variantData.ConversionsCount + " / " + variantData.Visits;

            case METRIC_CHANCE_TO_BEAT_ORIGINAL:
                if ((currentVariantName != OriginalVariant.ABVariantName) && (VariantPerformanceCalculator != null) && (variantData.Visits > 0))
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
                    && ABConversionRateIntervals.ContainsKey(currentVariantName) && ABConversionRateIntervals.ContainsKey(OriginalVariant.ABVariantName))
                {
                    // Render the picture representing how the challenger variant is performing against the original variant
                    return new ABConversionRateIntervalVisualizer(
                        mMinConversionRateLowerBound, mConversionRateRange, ABConversionRateIntervals[currentVariantName], ABConversionRateIntervals[OriginalVariant.ABVariantName]);
                }
                break;

            case METRIC_CONVERSION_VALUE:
                return variantData.ConversionsValue;

            case METRIC_AVG_CONVERSION_VALUE:
                return String.Format("{0:#.##}", variantData.AverageConversionValue);

            case "improvement":
                if ((currentVariantName != OriginalVariant.ABVariantName) && VariantsStatisticsData.ContainsKey(OriginalVariant.ABVariantName))
                {
                    var originalData = VariantsStatisticsData[OriginalVariant.ABVariantName];
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
            RedirectToAccessDenied("cms.abtest", "Manage");
        }

        switch (actionName)
        {
            case "selectwinner":
                FinishTestAndSelectVariantAsWinner(actionArgument as string);
                break;
        }
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "selectwinner":
                FinishTestAndSelectVariantAsWinner(e.CommandArgument as string);
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes selectors.
    /// </summary>
    private void InitializeSelectors()
    {
        // Turn off autocomplete to allow only predefined options (conversions) to be selected
        drpConversions.UniSelector.UseUniSelectorAutocomplete = false;

        var testConversions = ABTest.ABTestConversions.ToList();
        if (testConversions.Any())
        {
            drpConversions.WhereCondition = SqlHelper.GetWhereCondition("ConversionName", testConversions);
        }
        drpConversions.AllRecordValue = "";
        drpConversions.ABTestName = ABTest.ABTestName;

        // Turn on autopostback for the culture selector
        drpCulture.DropDownSingleSelect.AutoPostBack = true;

        // Load ShowAdvancedFilters content
        spnShowAdvancedFilters.InnerText = GetString(SHOW_FILTERS_TEXT);

        ParseSelectorCookie();
    }


    /// <summary>
    /// Initializes unigrid.
    /// </summary>
    private void InitializeGrid()
    {
        gridElem.DataSource = new InfoDataSet<ABVariantInfo>(ABVariants.ToArray());

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

        if (ValidationHelper.GetString(drpConversions.Value, "") == "")
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
        var conversionName = (string)(String.IsNullOrEmpty(drpConversions.Value as string) ? ABTest.ABTestConversions.Join(";") : drpConversions.Value);

        object[] parameters =
        {
            ABTest.ABTestOpenFrom,
            GetFinishDate(),
            ABTest.ABTestName,
            conversionName,
            graphDataElem.SelectedActionName,
            ABTest.ABTestID,
            string.Empty,
            drpCulture.Value,
            drpCountingMethodology.SelectedValue
        };

        data.Rows.Add(parameters);
        data.AcceptChanges();

        displayReport.Colors = ABVariantColorAssigner.GetColors(ABTest);
        displayReport.LoadFormParameters = false;
        displayReport.DisplayFilter = false;
        displayReport.GraphImageWidth = 100;
        displayReport.IgnoreWasInit = true;
        displayReport.UseExternalReload = true;
        displayReport.UseProgressIndicator = true;
        displayReport.SelectedInterval = samplingElem.SelectedActionName;
        displayReport.ReportParameters = data.Rows[0];
        displayReport.ReportName = "abtest" + drpSuccessMetric.SelectedValue + "." + samplingElem.SelectedActionName + "report";
    }


    /// <summary>
    /// Registers ABTestOverview module.
    /// </summary>
    private void EnsureModuleRegistration()
    {
        ScriptHelper.RegisterModule(Page, "CMS.OnlineMarketing/ABTestOverview", new
        {
            samplingElementId = samplingElem.ID,
            samplingElementClientId = samplingElem.ClientID,
            graphDataElementId = graphDataElem.ID,
            graphDataElementClientId = graphDataElem.ClientID,
            advancedControlsId = AdvancedControls.ID,
            advancedControlsClientId = AdvancedControls.ClientID,
            showAdvancedFiltersSpanClientId = spnShowAdvancedFilters.ClientID,
            successMetricDropdownId = drpSuccessMetric.ID,
            successMetricDropdownClientId = drpSuccessMetric.ClientID,
            countingMethodologyDropdownId = drpCountingMethodology.ID,
            countingMethodologyDropdownClientId = drpCountingMethodology.ClientID,
            cultureDropdownId = drpCulture.ID,
            cultureDropdownClientId = drpCulture.ClientID,
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
    private ABVariantInfo GetTestWinner()
    {
        return ABVariants.FirstOrDefault(v => v.ABVariantGUID == ABTest.ABTestWinnerGUID);
    }


    /// <summary>
    /// Finishes test and selects variant as winner.
    /// </summary>
    /// <param name="winnerName">Winner variant name</param>
    private void FinishTestAndSelectVariantAsWinner(string winnerName)
    {
        if (String.IsNullOrEmpty(winnerName) || (OriginalVariant == null))
        {
            ShowError(GetString("abtesting.pushwinner.error.general"));
            return;
        }

        var variant = ABVariants.FirstOrDefault(v => v.ABVariantName == winnerName);
        if (variant == null)
        {
            ShowError(GetString("abtesting.pushwinner.error.general"));
            return;
        }

        // Set winner GUID to ABTest
        ABTest.ABTestWinnerGUID = variant.ABVariantGUID;

        // If the test is running, finish it
        if (TestStatus == ABTestStatusEnum.Running)
        {
            ABTest.ABTestOpenTo = DateTime.Now;
        }

        ABTestInfo.Provider.Set(ABTest);

        DisplayWinnerInformation(variant);
        SetWinnerTooltip();

        // Reload data because HeaderActions_ActionPerformed event is too late to change action tooltip
        gridElem.ReloadData();

        // Reload summary box based on new status
        LoadSummaryBox();
    }


    /// <summary>
    /// Displays info label about the winner of the test.
    /// </summary>
    /// <param name="winner">AB test winner</param>
    private void DisplayWinnerInformation(ABVariantInfo winner)
    {
        ShowInformation(String.Format(GetString("abtesting.winningvariantselected"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(winner.ABVariantDisplayName))));
    }


    /// <summary>
    /// Updates tooltip of the grid action to reflect that winner has been selected.
    /// </summary>
    private void SetWinnerTooltip()
    {
        var action = gridElem.GridActions.Actions.Cast<Action>().FirstOrDefault(t => t.Name.ToLowerCSafe() == "selectwinner");
        if (action != null)
        {
            action.Caption = "$abtesting.winningvariantselected.tooltip$";
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        switch (TestStatus)
        {
            case ABTestStatusEnum.NotStarted:
            case ABTestStatusEnum.Scheduled:
                AddStartTestButton();
                break;

            case ABTestStatusEnum.Running:
                AddFinishAndSelectAsWinnerButton();
                break;

            case ABTestStatusEnum.Finished:
                if (ABTest.ABTestWinnerGUID == Guid.Empty)
                {
                    AddSelectAsWinnerButton();
                }
                break;
        }
    }


    /// <summary>
    /// Adds the "Start test" button.
    /// </summary>
    private void AddStartTestButton()
    {
        string testStartUrl = ResolveUrl("~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABTest/StartABTest.aspx?testid=" + ABTest.ABTestID);
        var btnStartTest = new HeaderAction
        {
            Tooltip = GetString("abtesting.starttest.tooltip"),
            Text = GetString("abtesting.starttest"),
            OnClientClick = "modalDialog('" + testStartUrl + @"', '', 670, 320);",
            Enabled = IsUserAuthorizedToManageTest
        };
        HeaderActions.AddAction(btnStartTest);
    }


    /// <summary>
    /// Adds the "Finish test" with alternative actions button.
    /// </summary>
    private void AddFinishAndSelectAsWinnerButton()
    {
        var actions = new List<HeaderAction>();

        AddSelectAsWinnerOptions(actions);

        string testFinishUrl = ResolveUrl("~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABTest/FinishABTest.aspx?testid=" + ABTest.ABTestID);
        HeaderActions.AddAction(new HeaderAction
        {
            Tooltip = GetString("abtesting.finishtest.tooltip"),
            Text = GetString("abtesting.finishtest"),
            AlternativeActions = actions,
            OnClientClick = "modalDialog('" + testFinishUrl + @"', '', 670, 320);",
            Enabled = IsUserAuthorizedToManageTest
        });
    }


    /// <summary>
    /// Adds the "Select as winner" button.
    /// </summary>
    private void AddSelectAsWinnerButton()
    {
        var actions = new List<HeaderAction>();
        AddSelectAsWinnerOptions(actions);

        HeaderActions.AddAction(new HeaderAction
        {
            Tooltip = GetString("abtesting.selectwinner.tooltip"),
            Text = GetString("abtesting.selectwinner"),
            Inactive = true,
            AlternativeActions = actions,
            Enabled = IsUserAuthorizedToManageTest
        });
    }


    /// <summary>
    /// Adds the "Select as winner" option for each variant of the test.
    /// </summary>
    /// <param name="actions">Header actions list to be filled</param>
    private void AddSelectAsWinnerOptions(List<HeaderAction> actions)
    {
        foreach (var variant in ABVariants)
        {
            actions.Add(new HeaderAction
            {
                Text = String.Format(GetString("abtesting.selectvariantaswinner"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(variant.ABVariantDisplayName))),
                Tooltip = String.Format(GetString("abtesting.selectvariantaswinner.tooltip"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(variant.ABVariantDisplayName))),
                CommandName = "selectwinner",
                CommandArgument = variant.ABVariantName,
                OnClientClick = "return confirm('" + String.Format(GetString("abtesting.winnerselectionconfirmation"), ScriptHelper.GetString(ResHelper.LocalizeString(variant.ABVariantDisplayName), false)) + "');",
                Enabled = IsUserAuthorizedToManageTest
            });
        }
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

            // Parse cookie value
            mSamplingIndex = ValidationHelper.GetInteger(selectorsState[samplingElem.ID], 0);
            mDrpSuccessMetricIndex = ValidationHelper.GetInteger(selectorsState[drpSuccessMetric.ID], 0);
            mDrpCountingMethodologyIndex = ValidationHelper.GetInteger(selectorsState[drpCountingMethodology.ID], 0);
            mGraphDataIndex = ValidationHelper.GetInteger(selectorsState[graphDataElem.ID], 0);
            mDrpCultureValue = selectorsState[drpCulture.ID];
            mDrpConversionsValue = selectorsState[drpConversions.ID];

            // Load ShowAdvancedFilters content
            mAdvancedcontrolsVisible = ValidationHelper.GetBoolean(selectorsState[AdvancedControls.ID], false);
        }
        catch
        {
            // If something failed, delete the cookie by setting its expiration to the past
            CookieHelper.SetValue(SelectorsCookieKey, string.Empty, DateTime.Now.AddDays(-1));
        }
    }


    /// <summary>
    /// Sets selector values to the values used in the previous page visit.
    /// </summary>
    private void SetSelectorValues()
    {
        samplingElem.SelectedActionName = SamplingActions[mSamplingIndex];
        drpSuccessMetric.SelectedIndex = mDrpSuccessMetricIndex;
        drpCountingMethodology.SelectedIndex = mDrpCountingMethodologyIndex;
        graphDataElem.SelectedActionName = DataFormatActions[mGraphDataIndex];
        drpCulture.Value = mDrpCultureValue;
        drpConversions.Value = mDrpConversionsValue;

        // Load ShowAdvancedFilters content
        spnShowAdvancedFilters.InnerText = mAdvancedcontrolsVisible ? GetString(HIDE_FILTERS_TEXT) : GetString(SHOW_FILTERS_TEXT);
        AdvancedControls.Style[HtmlTextWriterStyle.Display] = mAdvancedcontrolsVisible ? "block" : "none";
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
        EnsureVariantStatisticsData(OriginalVariant.ABVariantName);

        foreach (var variant in ABVariants)
        {
            EnsureVariantStatisticsData(variant.ABVariantName);
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
    /// <param name="variantName">Variant name</param>
    private void EnsureVariantStatisticsData(string variantName)
    {
        if (!VariantsStatisticsData.ContainsKey(variantName))
        {
            // Select both abvisitfirst and abvisitreturn by default
            string visitType = "abvisit%";

            // If counting methodology is set to visitor conversion, select abvisitfirst only
            string countingMethodology = drpCountingMethodology.Items[mDrpCountingMethodologyIndex].Value;
            if (countingMethodology == ABTestConstants.ABSESSIONCONVERSION_FIRST)
            {
                visitType = "abvisitfirst";
            }

            string conversionsCodename = countingMethodology + ";" + ABTest.ABTestName + ";" + variantName;
            string visitsCodename = visitType + ";" + ABTest.ABTestName + ";" + variantName;
            string conversion = ValidationHelper.GetString(drpConversions.Value, string.Empty);
            var testConversions = ABTest.ABTestConversions;

            // Get conversions count and value
            DataRow conversions = GetHits(conversionsCodename, "Sum(HitsCount), Sum(HitsValue)", mDrpCultureValue, GetConversionCondition(conversion, testConversions));
            int conversionsCount = ValidationHelper.GetInteger(conversions[0], 0);
            double conversionsValue = ValidationHelper.GetDouble(conversions[1], 0);

            // Get visits count
            int visits = ValidationHelper.GetInteger(GetHits(visitsCodename, "Sum(HitsCount)", mDrpCultureValue)[0], 0);

            // Add statistics data
            VariantsStatisticsData.Add(variantName, new ABVariantStatisticsData(conversionsCount, conversionsValue, visits));

            // Add conversion rate intervals
            if ((VariantPerformanceCalculator != null) && (visits > 0) && (conversionsCount <= visits))
            {
                ABConversionRateIntervals.Add(variantName, VariantPerformanceCalculator.GetConversionRateInterval(conversionsCount, visits));
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
    private void ShowInvalidFilterCombinationImage()
    {
        // Do not show on first load
        if (!RequestHelper.IsPostBack())
        {
            return;
        }

        if ((ValidationHelper.GetString(drpConversions.Value, "") == "") && (drpSuccessMetric.SelectedValue == METRIC_CONVERSION_RATE))
        {
            ShowWarning(GetString("abtesting.invalidfiltercombination"));
        }
    }


    /// <summary>
    /// Initializes the smart tip with the how to video.
    /// </summary>
    private void InitSmartTip()
    {
        var linkBuilder = new MagnificPopupYouTubeLinkBuilder();
        var linkID = Guid.NewGuid().ToString();
        var link = linkBuilder.GetLink("EGzYxXggueM", linkID, GetString("abtesting.howto.howtoevaluateabtest.link"));

        new MagnificPopupYouTubeJavaScriptRegistrator().RegisterMagnificPopupElement(this, linkID);

        tipHowToOverview.Content = string.Format("{0} {1}", GetString("abtesting.howto.howtoevaluateabtest.overview.text"), link);
        tipHowToOverview.CollapsedStateIdentifier = SMART_TIP_IDENTIFIER;
        tipHowToOverview.ExpandedHeader = GetString("abtesting.howto.howtoevaluateabtest.title");
    }

    #endregion
}