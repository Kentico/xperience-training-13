using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_UI_UniGrid_UniGrid : UniGrid
{
    #region "Controls properties"

    /// <summary>
    /// Gets filter placeHolder from UniGrid.
    /// </summary>
    public override PlaceHolder FilterPlaceHolder
    {
        get
        {
            return plcFilter;
        }
    }


    /// <summary>
    /// Gets the menu placeholder
    /// </summary>
    protected override PlaceHolder MenuPlaceHolder
    {
        get
        {
            return plcContextMenu;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Returns the header panel
    /// </summary>
    protected override Panel HeaderPanel
    {
        get
        {
            return pnlHeader;
        }
    }


    /// <summary>
    /// Grid information label
    /// </summary>
    protected override Label InfoLabel
    {
        get
        {
            return lblInfo;
        }
    }


    /// <summary>
    /// Gets <see cref="UIGridView"/> control of UniGrid.
    /// </summary>
    public override UIGridView GridView
    {
        get
        {
            return UniUiGridView;
        }
    }


    /// <summary>
    /// Gets <see cref="MassActions"/> control of UniGrid.
    /// </summary>
    public override MassActions MassActions
    {
        get
        {
            return ctrlMassActions;
        }
    }


    /// <summary>
    /// Hidden field containing selected items.
    /// </summary>
    public override HiddenField SelectionHiddenField
    {
        get
        {
            return hidSelection;
        }
    }


    /// <summary>
    /// Hidden field containing selected items hash.
    /// </summary>
    protected override HiddenField SelectionHashHiddenField
    {
        get
        {
            return hidSelectionHash;
        }
    }


    /// <summary>
    /// Hidden field for the command name
    /// </summary>
    protected override HiddenField CmdNameHiddenField
    {
        get
        {
            return hidCmdName;
        }
    }


    /// <summary>
    /// Hidden field for the command argument
    /// </summary>
    protected override HiddenField CmdArgHiddenField
    {
        get
        {
            return hidCmdArg;
        }
    }


    /// <summary>
    /// Gets <see cref="UIPager"/> control of UniGrid.
    /// </summary>
    public override UIPager Pager
    {
        get
        {
            return pagerElem;
        }
    }


    /// <summary>
    /// Hidden field with action ids.
    /// </summary>
    protected override HiddenField ActionsHidden
    {
        get
        {
            return hidActions;
        }
    }


    /// <summary>
    /// Hidden field with hashed action ids.
    /// </summary>
    protected override HiddenField ActionsHashHidden
    {
        get
        {
            return hidActionsHash;
        }
    }


    /// <summary>
    /// Gets page size Drop-down from UniGrid Pager.
    /// </summary>
    public override DropDownList PageSizeDropdown
    {
        get
        {
            return Pager.PageSizeDropdown;
        }
    }


    /// <summary>
    /// Returns the filter form placeholder
    /// </summary>
    protected override PlaceHolder FilterFormPlaceHolder
    {
        get
        {
            return plcFilterForm;
        }
    }


    /// <summary>
    /// Gets filter form.
    /// </summary>
    public override FilterForm FilterForm
    {
        get
        {
            return filterForm;
        }
    }


    /// <summary>
    /// Returns the advanced export control of the current grid
    /// </summary>
    protected override AdvancedExport AdvancedExportControl
    {
        get
        {
            return advancedExportElem;
        }
    }

    #endregion
}