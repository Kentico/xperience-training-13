using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Documents_OutdatedDocumentsFilter : CMSAbstractBaseFilterControl
{
    #region "Constants"

    private const string SOURCE_MODIFIEDWHEN = "DocumentModifiedWhen";

    private const string SOURCE_CLASSDISPLAYNAME = "ClassDisplayName";

    private const string SOURCE_DOCUMENTNAME = "DocumentName";

    #endregion


    #region "Public properties"

    public override string WhereCondition
    {
        get
        {
            return GenerateWhereCondition();
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Applies the filter.
    /// </summary>
    protected void btnShow_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    /// <summary>
    /// Resets all controls to default.
    /// </summary>
    public override void ResetFilter()
    {
        txtFilter.Text = "1";
        drpFilter.SelectedIndex = 3;
        drpDocumentName.SelectedIndex = 0;
        drpDocumentType.SelectedIndex = 0;
        txtDocumentType.Text = String.Empty;
        txtDocumentName.Text = String.Empty;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Setups filter controls.
    /// </summary>
    private void SetupControl()
    {
        // Initialize controls
        if (!RequestHelper.IsPostBack())
        {
            // Fill the dropdown list
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Days"));
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Weeks"));
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Months"));
            drpFilter.Items.Add(GetString("MyDesk.OutdatedDocuments.Years"));

            // Load default value
            if (String.IsNullOrEmpty(txtFilter.Text))
            {
                txtFilter.Text = "1";
                drpFilter.SelectedIndex = 3;
            }

            // Bind dropdown lists
            BindDropDowns();
        }

        // Initialize Reset button
        UniGrid grid = FilteredControl as UniGrid;
        if (grid == null || !grid.RememberState)
        {
            btnReset.Visible = false;
        }
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    private string GenerateWhereCondition()
    {
        // Get older than value
        DateTime olderThan = DateTime.Now.Date.AddDays(1);
        int dateTimeValue = ValidationHelper.GetInteger(txtFilter.Text, 0);

        switch (drpFilter.SelectedIndex)
        {
            case 0:
                olderThan = olderThan.AddDays(-dateTimeValue);
                break;

            case 1:
                olderThan = olderThan.AddDays(-dateTimeValue * 7);
                break;

            case 2:
                olderThan = olderThan.AddMonths(-dateTimeValue);
                break;

            case 3:
                olderThan = olderThan.AddYears(-dateTimeValue);
                break;
        }

        var where = new WhereCondition().WhereLessOrEquals(SOURCE_MODIFIEDWHEN, olderThan);

        // Add where condition
        if (!string.IsNullOrEmpty(txtDocumentName.Text))
        {
            AddOutdatedWhereCondition(where, SOURCE_DOCUMENTNAME, drpDocumentName, txtDocumentName);
        }

        if (!string.IsNullOrEmpty(txtDocumentType.Text))
        {
            AddOutdatedWhereCondition(where, SOURCE_CLASSDISPLAYNAME, drpDocumentType, txtDocumentType);
        }

        return where.ToString(true);
    }


    /// <summary>
    /// Gets where condition based on value of given controls.
    /// </summary>
    /// <param name="where">Where condition</param>
    /// <param name="column">Column to compare</param>
    /// <param name="drpOperator">List control with operator</param>
    /// <param name="valueBox">Text control with value</param>
    /// <returns>Where condition for outdated documents</returns>
    private void AddOutdatedWhereCondition(WhereCondition where, string column, ListControl drpOperator, ITextControl valueBox)
    {
        var value = TextHelper.LimitLength(valueBox.Text, 100);

        if (!String.IsNullOrEmpty(value))
        {
            string condition = drpOperator.SelectedValue;
            
            // Create condition based on operator
            switch (condition)
            {
                case WhereBuilder.LIKE:
                    where.WhereContains(column, value);
                    break;

                case WhereBuilder.NOT_LIKE:
                    where.WhereNotContains(column, value);
                    break;

                case WhereBuilder.EQUAL:
                    where.WhereEquals(column, value);
                    break;

                case WhereBuilder.NOT_EQUAL:
                    where.WhereNotEquals(column, value);
                    break;
            }
        }
    }


    /// <summary>
    /// Binds filter dropdown lists with conditions.
    /// </summary>
    private void BindDropDowns()
    {
        ControlsHelper.FillListWithTextSqlOperators(drpDocumentName);
        ControlsHelper.FillListWithTextSqlOperators(drpDocumentType);
    }

    #endregion
}
