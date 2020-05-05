using System.Web.UI.WebControls;

using CMS.UIControls;


public partial class CMSAdminControls_UI_UniSelector_Controls_SelectionDialog : SelectionDialog
{
    #region "Inner controls"

    /// <summary>
    /// Control's unigrid.
    /// </summary>
    public override UniGrid UniGrid => uniGrid;


    protected override HiddenField ItemsField => hidItem;


    protected override Panel DefaultFilterPanel => pnlSearch;


    protected override TextBox DefaultFilterInput => txtSearch;


    protected override Button DefaultFilterButton => btnSearch;


    protected override Panel CustomFilterPanel => pnlFilter;


    protected override Panel ActionsPanel => pnlAll;

    #endregion


    #region "Methods"

    protected override void OnMultiSelectionChanged(bool isSelectAllAction)
    {
        base.OnMultiSelectionChanged(isSelectAllAction);

        // Update hidden field with selection
        pnlHidden.Update();
    }

    #endregion
}
