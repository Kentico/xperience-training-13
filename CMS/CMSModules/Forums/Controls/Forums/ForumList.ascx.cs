using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.Forums;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Forums_ForumList : CMSAdminListControl
{
    #region "Variables"

    protected int mGroupId = 0;
    private int? mCommunityGroupID;
    private bool process = true;
    private bool reloadUnigrid = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the ID of the forum group for which the forums should be loaded.
    /// </summary>
    public int GroupID
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
    /// Gets the ID of the community group which the forums should be loaded for.
    /// </summary>
    private int CommunityGroupID
    {
        get
        {
            if (mCommunityGroupID == null)
            {
                mCommunityGroupID = 0;
                ForumGroupInfo forumGroup = ForumGroupInfoProvider.GetForumGroupInfo(GroupID);

                if ((forumGroup != null) && (forumGroup.GroupGroupID > 0))
                {
                    BaseInfo communityGroupInfo = ModuleCommands.CommunityGetGroupInfo(forumGroup.GroupGroupID);
                    mCommunityGroupID = (communityGroupInfo != null) ? communityGroupInfo.Generalized.ObjectID : 0;
                }
            }

            return mCommunityGroupID.Value;
        }
    }

    #endregion


    protected void Page_Init(object sender, EventArgs e)
    {
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        process = true;
        if (!Visible || StopProcessing)
        {
            EnableViewState = false;
            process = false;
        }

        // Initialize this.gridElem control
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.GridView.DataBound += new EventHandler(GridView_DataBound);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.OrderBy = "ForumOrder";
        gridElem.GridView.AllowSorting = false;
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.WhereCondition = "ForumGroupID=" + mGroupId;
        gridElem.GroupObject = (CommunityGroupID > 0);
        gridElem.ZeroRowsText = GetString("general.nodatafound");
    }


    protected void gridElem_OnBeforeDataReload()
    {
        if (CommunityGroupID > 0)
        {
            gridElem.ObjectType = ForumInfo.OBJECT_TYPE_GROUP;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!IsLiveSite && process)
        {
            ReloadData();
            gridElem.ReloadData();
        }
        else if (reloadUnigrid)
        {
            gridElem.ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the grid.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        if (GroupID > 0)
        {
            gridElem.WhereCondition = "ForumGroupID=" + mGroupId;
        }

        reloadUnigrid = true;
    }


    #region "UniGrid events handling"

    private void GridView_DataBound(object sender, EventArgs e)
    {
        //convert boolean values from DB to user-friendly information strings in the list
        for (int i = 0; i < gridElem.GridView.Rows.Count; i++)
        {
            // Date time string
            string dateTime = String.Empty;

            // Change timezone for live site
            DateTime dt = TimeZoneUIMethods.ConvertDateTime(ValidationHelper.GetDateTime(gridElem.GridView.Rows[i].Cells[6].Text, DateTimeHelper.ZERO_TIME), this);
            if (dt != DateTimeHelper.ZERO_TIME)
            {
                dateTime = dt.ToString();
            }

            // Set value to the grid
            gridElem.GridView.Rows[i].Cells[6].Text = dateTime;
        }
    }


    /// <summary>
    /// Unigrid external bind event handler.
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "open":
                if (ValidationHelper.GetBoolean(parameter, false))
                {
                    return GetString("Forum_List.Open");
                }
                else
                {
                    return GetString("Forum_List.Close");
                }

            case "moderated":
                return UniGridFunctions.ColoredSpanYesNo(parameter);
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrids's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
            case "up":
            case "down":
                if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
                {
                    return;
                }
                break;
        }

        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                ForumInfoProvider.DeleteForumInfo(Convert.ToInt32(actionArgument));
                break;

            case "up":
                ForumInfoProvider.MoveForumUp(Convert.ToInt32(actionArgument));
                break;

            case "down":
                ForumInfoProvider.MoveForumDown(Convert.ToInt32(actionArgument));
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }

    #endregion
}