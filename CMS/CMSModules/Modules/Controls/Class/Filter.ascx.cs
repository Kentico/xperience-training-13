using System;

using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Modules_Controls_Class_Filter : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the filter condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Determines whether filter is set.
    /// </summary>
    public override bool FilterIsSet
    {
        get
        {
            return !string.IsNullOrEmpty(txtClassDisplayName.Text) || !string.IsNullOrEmpty(txtClassTableName.Text);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Initialize reset link button
        UniGrid grid = FilteredControl as UniGrid;
        if (grid == null || !grid.RememberState)
        {
            btnReset.Visible = false;
        }
        if (grid != null)
        {
            grid.HideFilterButton = true;
        }
    }


    /// <summary>
    /// Applies filter on associated UniGrid control.
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

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtClassDisplayName.Text = String.Empty;
        txtClassTableName.Text = String.Empty;
    }

    #endregion


    #region "Private methods"

    private string GetWhereCondition()
    {
        var where = new WhereCondition();

        // Display name/Code name
        string displayName = txtClassDisplayName.Text.Trim();
        if (!String.IsNullOrEmpty(displayName))
        {
            where
                .Where(w => 
                    w.WhereContains("ClassDisplayName", displayName)
                    .Or()
                    .WhereContains("ClassName", displayName));
        }

        // Table name
        string tableName = txtClassTableName.Text.Trim();
        if (!String.IsNullOrEmpty(tableName))
        {
            where
                .WhereContains("ClassTableName", tableName);
        }

        return where.ToString(true);
    }

    #endregion

}