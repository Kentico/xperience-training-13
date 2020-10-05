using System;

using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_Roles_RoleList : CMSAdminListControl
{
    #region "Variables"

    private int mSiteId;
    private int mGlobalRecordValue = UniSelector.US_GLOBAL_RECORD;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets global record value (value for global item selected in drop down).
    /// </summary>
    public int GlobalRecordValue
    {
        get
        {
            return mGlobalRecordValue;
        }
        set
        {
            mGlobalRecordValue = value;
            gridElem.WhereCondition = CreateWhereCondition();
        }
    }


    /// <summary>
    /// Gets or sets the site ID for which the roles should be displayed.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
            gridElem.WhereCondition = CreateWhereCondition();
        }
    }


    /// <summary>
    /// Gets or sets whether list is showed in group UI.
    /// </summary>
    public bool IsGroupList
    {
        get;
        set;
    }

    #endregion


    /// <summary>
    /// Creates where condition for unigrid according to the parameters.
    /// </summary>
    private string CreateWhereCondition()
    {
        string where = "";

        if (mSiteId > 0)
        {
            where += "(SiteID = " + mSiteId + ")";
        }
        else
            // Global selected
            if ((mSiteId == GlobalRecordValue) && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                where += "(SiteID IS NULL)";
            }
            else
            {
                where += "(SiteID =" + SiteContext.CurrentSiteID + ")";
            }

        return where;
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        if (StopProcessing)
        {
            return;
        }

        // Unigrid
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.WhereCondition = CreateWhereCondition();
        gridElem.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            SelectedItemID = Convert.ToInt32(actionArgument);
            RaiseOnEdit();
        }
        else if (actionName == "delete")
        {
            if (!CheckPermissions("CMS.Roles", PERMISSION_MODIFY))
            {
                return;
            }

            RoleInfo.Provider.Delete(RoleInfo.Provider.Get(ValidationHelper.GetInteger(actionArgument, 0)));
        }
        RaiseOnAction(actionName, actionArgument);
    }
}