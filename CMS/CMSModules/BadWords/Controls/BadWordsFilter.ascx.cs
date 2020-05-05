using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_BadWords_Controls_BadWordsFilter : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    public override string WhereCondition
    {
        get
        {
            return BuildWhereCondition();
        }
        set
        {
            base.WhereCondition = value;
        }
    }
    
    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        
        ucBadWordAction.NoSelectionText = GetString("general.allactions");

        // Initialize reset link button
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null && grid.RememberState)
        {
            btnReset.Text = GetString("general.reset");
            btnReset.Click += btnReset_Click;
        }
        else
        {
            btnReset.Visible = false;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Builds complete where condition to filter.
    /// </summary>
    private string BuildWhereCondition()
    {
        string where = null;

        // Create WHERE condition with 'Expression'
        string txt = txtExpression.Text.Trim().Replace("'", "''");
        if (!string.IsNullOrEmpty(txt))
        {
            where = "(WordExpression LIKE N'%" + txt + "%')";
        }

        // Create WHERE condition with 'Action'
        int action = ValidationHelper.GetInteger(ucBadWordAction.Value, -1);
        if (action != -1)
        {
            if (!String.IsNullOrEmpty(where))
            {
                where += " AND ";
            }

            // Select also bad words that ihnerit action from settings
            if (action == (int)BadWordsHelper.BadWordsAction(SiteContext.CurrentSiteName))
            {
                where += "(WordAction = " + action + " OR WordAction IS NULL)";
            }
            else
            {
                where += "(WordAction = " + action + ")";
            }
        }
        return where;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtExpression.Text = String.Empty;
        ucBadWordAction.Value = ucBadWordAction.NoSelectionText; 
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

    #endregion
}
