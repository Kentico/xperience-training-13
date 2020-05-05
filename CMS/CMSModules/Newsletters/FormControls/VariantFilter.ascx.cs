using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Newsletters;


public partial class CMSModules_Newsletters_FormControls_VariantFilter : FormEngineUserControl
{
    // ID of the main A/B test issue issue
    private int mainIssueID = 0;

    // ID of the winner variant issue
    private int winnerIssueID = -1;


    #region "Properties"

    /// <summary>
    /// Gets or sets issue ID of or to variant selector. Used for A/B test issues.
    /// </summary>
    public override object Value
    {
        get
        {
            if (drpVariants.Items.Count == 0)
            {
                return 0;
            }

            return ValidationHelper.GetInteger(drpVariants.SelectedValue, 0);
        }
        set
        {
            mainIssueID = (int)value;

            // Init selector values
            InitVariantSelector(mainIssueID);
        }
    }


    /// <summary>
    /// Gets ID of the winner variant issue.
    /// </summary>
    private int WinnerIssueID
    {
        get
        {
            if (winnerIssueID < 0)
            {
                ABTestInfo test = ABTestInfoProvider.GetABTestInfoForIssue(mainIssueID);
                if (test != null)
                {
                    winnerIssueID = test.TestWinnerIssueID;
                }
            }

            return winnerIssueID;
        }
        set
        {
            winnerIssueID = value;
        }
    }


    /// <summary>
    /// Indicates if (all) option will be available. Default value is True.
    /// </summary>
    public bool AllowSelectAll { get; set; } = true;

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes selector with variants of the main issue that is specified by its ID.
    /// </summary>
    /// <param name="issueId">Issue ID</param>
    protected void InitVariantSelector(int issueId)
    {
        if (drpVariants.Items.Count == 0)
        {
            if (AllowSelectAll)
            {
                // Add (all) option
                drpVariants.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
            }

            // Get A/B test variants
            var variants = IssueHelper.GetIssueVariants(issueId);
            if (variants?.Count > 0)
            {
                // Initialize selector
                foreach (var variant in variants)
                {
                    if (variant.IsWinner)
                    {
                        WinnerIssueID = variant.IssueID;
                        drpVariants.Items.Add(new ListItem(variant.IssueVariantName + " " + GetString("newsletterabtest.winner"), mainIssueID.ToString()));
                    }
                    else
                    {
                        drpVariants.Items.Add(new ListItem(variant.IssueVariantName, variant.IssueID.ToString()));
                    }
                }
            }

            if (!RequestHelper.IsPostBack() && drpVariants.Items.FindByValue(issueId.ToString()) != null)
            {
                // Preselect main issue
                drpVariants.SelectedValue = issueId.ToString();
            }
        }
    }


    public override string GetWhereCondition()
    {
        string issueIds = string.Empty;

        // Get selected value
        int selectedIssue = ValidationHelper.GetInteger(drpVariants.SelectedValue, 0);
        if (selectedIssue < 0)
        {
            // Get IDs of all involved issues (main and variant issues)
            foreach (ListItem item in drpVariants.Items)
            {
                if (ValidationHelper.GetInteger(item.Value, 0) > 0)
                {
                    issueIds += item.Value + ",";
                }
            }
            issueIds += WinnerIssueID;
        }
        else if (selectedIssue == mainIssueID)
        {
            // Get IDs of main issue and winner variant
            issueIds = (WinnerIssueID > 0) ? string.Format("{0},{1}", mainIssueID, WinnerIssueID) : mainIssueID.ToString();
        }

        if (string.IsNullOrEmpty(issueIds))
        {
            return string.Empty;
        }

        if (String.IsNullOrEmpty(WhereConditionFormat))
        {
            WhereConditionFormat = "[{0}] {2} {1}";
        }

        return string.Format(WhereConditionFormat, "OpenedEmailIssueID", "(" + issueIds + ")", "IN");
    }

    #endregion
}
