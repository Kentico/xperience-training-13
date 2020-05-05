using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Polls;
using CMS.UIControls;


public partial class CMSModules_Polls_Controls_PollsList : CMSAdminListControl
{
    #region "Variables"

    private int mGroupId = 0;
    private string mWhereCondition = String.Empty;
    private bool mDeleteEnabled = true;
    private bool mDeleteGlobalEnabled = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets ID of current group.
    /// </summary>
    public int GroupId
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }


    /// <summary>
    /// Additional WHERE condition to filter data.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            mWhereCondition = value;
        }
    }


    /// <summary>
    /// Indicates if DelayedReload for UniGrid should be used.
    /// </summary>
    public bool DelayedReload
    {
        get
        {
            return UniGrid.DelayedReload;
        }
        set
        {
            UniGrid.DelayedReload = value;
        }
    }


    /// <summary>
    /// Indicates if global polls should be marked.
    /// </summary>
    public bool DisplayGlobalColumn
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether delete button will be enabled for site polls.
    /// </summary>
    public bool DeleteEnabled
    {
        get { return mDeleteEnabled; }
        set { mDeleteEnabled = value; }
    }


    /// <summary>
    /// Indicates whether delete button will be enabled for global polls.
    /// </summary>
    public bool DeleteGlobalEnabled
    {
        get { return mDeleteGlobalEnabled; }
        set { mDeleteGlobalEnabled = value; }
    }
	

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the grid
        UniGrid.IsLiveSite = IsLiveSite;
        UniGrid.OnAction += new OnActionEventHandler(UniGrid_OnAction);
        UniGrid.HideControlForZeroRows = false;
        UniGrid.OnBeforeSorting += new OnBeforeSorting(UniGrid_OnBeforeSorting);
        UniGrid.OnPageChanged += new EventHandler<EventArgs>(UniGrid_OnPageChanged);
        UniGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(UniGrid_OnExternalDataBound);
        UniGrid.ZeroRowsText = GetString("general.nodatafound");
        UniGrid.OnBeforeDataReload += new OnBeforeDataReload(UniGrid_OnBeforeDataReload);
        UniGrid.GroupObject = (GroupId > 0);
        if (UniGrid.GroupObject)
        {
            UniGrid.ObjectType = "polls.grouppolllist";
        }
        SetupControl();
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        bool isglobal;
        DataRowView drv;
        switch (sourceName)
        {
            case "isglobal":
                drv = (DataRowView)parameter;
                isglobal = (ValidationHelper.GetInteger(drv["PollSiteID"], 0) <= 0);
                if (isglobal)
                {
                    return "<span class=\"StatusEnabled\">" + GetString("general.yes") + "</span>";
                }
                else
                {
                    return "<span class=\"StatusDisabled\">" + GetString("general.no") + "</span>";
                }

            case "delete":
                drv = (parameter as GridViewRow).DataItem as DataRowView;
                isglobal = (ValidationHelper.GetInteger(drv["PollSiteID"], 0) <= 0);
                if ((isglobal && !DeleteGlobalEnabled) || (!isglobal && !DeleteEnabled))
                {
                    var btn = (CMSGridActionButton)sender;
                    btn.Enabled = false;
                }
                break;
        }

        return parameter;
    }


    protected void UniGrid_OnBeforeDataReload()
    {
        UniGrid.GridView.Columns[5].Visible = DisplayGlobalColumn;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            SelectedItemID = Convert.ToInt32(actionArgument);
            RaiseOnEdit();
        }
        else if (actionName == "delete")
        {
            PollInfo pi = PollInfoProvider.GetPollInfo(Convert.ToInt32(actionArgument));
            if (pi != null)
            {
                if ((pi.PollSiteID > 0) && !CheckPermissions("cms.polls", PERMISSION_MODIFY) ||
                    (pi.PollSiteID <= 0) && !CheckPermissions("cms.polls", PERMISSION_GLOBALMODIFY))
                {
                    return;
                }

                // Delete PollInfo object from database with it's dependences
                PollInfoProvider.DeletePollInfo(Convert.ToInt32(actionArgument));
            }

            ReloadData();
        }
    }


    private void UniGrid_OnPageChanged(object sender, EventArgs e)
    {
        if (IsLiveSite)
        {
            ReloadData();
        }
    }


    private void UniGrid_OnBeforeSorting(object sender, EventArgs e)
    {
        if (IsLiveSite)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Setups control.
    /// </summary>
    private void SetupControl()
    {
        if (GroupId > 0)
        {
            UniGrid.WhereCondition = "PollGroupID='" + GroupId.ToString() + "'";
        }

        // Add where condition from property
        if (WhereCondition != String.Empty)
        {
            if (!String.IsNullOrEmpty(UniGrid.WhereCondition) && (UniGrid.WhereCondition != WhereCondition))
            {
                UniGrid.WhereCondition += " AND " + WhereCondition;
            }
            else
            {
                UniGrid.WhereCondition = WhereCondition;
            }
        }
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        SetupControl();
        UniGrid.ReloadData();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (UniGrid.RowsCount > 0)
        {
            int i = 0;
            bool isglobal = false;
            DataView view = (DataView)UniGrid.GridView.DataSource;
            foreach (DataRow row in view.Table.Rows)
            {
                // Hide object menu to system activity types (only custom activity types may be exported)
                isglobal = (ValidationHelper.GetInteger(row["PollSiteID"], 0) <= 0);
                if ((isglobal && !DeleteGlobalEnabled) || (!isglobal && !DeleteEnabled))
                {
                    if ((UniGrid.GridView.Rows[i].Cells.Count > 0) && (UniGrid.GridView.Rows[i].Cells[0].Controls.Count > 2)
                        && (UniGrid.GridView.Rows[i].Cells[0].Controls[2] is ContextMenuContainer))
                    {
                        UniGrid.GridView.Rows[i].Cells[0].Controls[2].Visible = false;
                    }
                }
                i++;
            }
        }
    }
}