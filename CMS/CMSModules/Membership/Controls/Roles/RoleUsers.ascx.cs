using System;
using System.Linq;
using System.Text;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_Roles_RoleUsers : CMSAdminEditControl
{
    #region "Variables"

    private string currentValues = String.Empty;
    private int mRoleId = 0;
    private bool reloadData = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the role ID for which the users should be displayed.
    /// </summary>
    public int RoleID
    {
        get
        {
            return mRoleId;
        }
        set
        {
            mRoleId = value;
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
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Hide site selector
        usUsers.ShowSiteFilter = false;

        // Set selector live site and hide special users
        usUsers.IsLiveSite = IsLiveSite;
        if (IsLiveSite)
        {
            usUsers.HideDisabledUsers = true;
            usUsers.HideHiddenUsers = true;
        }

        // Show only user belonging to role's site
        RoleInfo ri = RoleInfo.Provider.Get(RoleID);
        if (ri != null)
        {
            usUsers.SiteID = ri.SiteID == 0 ? -1 : ri.SiteID;
        }

        // Load data in administration
        if (!IsLiveSite)
        {
            currentValues = GetRoleUsers();

            if (!RequestHelper.IsPostBack())
            {
                usUsers.Value = currentValues;
            }
        }

        usUsers.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveUsers();
    }


    private string GetRoleUsers()
    {
        var data = UserRoleInfo.Provider.Get().Where("RoleID = " + RoleID).Columns("UserID");
        if (data.Any())
        {
            return TextHelper.Join(";", DataHelper.GetStringValues(data.Tables[0], "UserID"));
        }

        return String.Empty;
    }


    private void SaveUsers()
    {
        if (!CheckPermissions("cms.roles", PERMISSION_MODIFY))
        {
            return;
        }

        bool falseValues = false;
        bool saved = false;

        // Remove old items
        string newValues = ValidationHelper.GetString(usUsers.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        StringBuilder errorMessage = new StringBuilder();
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int userId = ValidationHelper.GetInteger(item, 0);

                    // Check permissions
                    string result = ValidateGlobalAndDeskAdmin(userId);
                    if (result != String.Empty)
                    {
                        errorMessage.Append(result);
                        falseValues = true;
                        continue;
                    }
                    else
                    {
                        var uri = UserRoleInfo.Provider.Get(userId, RoleID);
                        UserRoleInfo.Provider.Delete(uri);

                        saved = true;
                    }
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int userId = ValidationHelper.GetInteger(item, 0);

                    // Check permissions
                    string result = ValidateGlobalAndDeskAdmin(userId);
                    if (result != String.Empty)
                    {
                        errorMessage.Append(result);
                        falseValues = true;
                        continue;
                    }
                    else
                    {
                        UserRoleInfo.Provider.Add(userId, RoleID);
                        saved = true;
                    }
                }
            }
        }
        if (errorMessage.Length > 0)
        {
            ShowError(errorMessage.ToString());
        }

        if (falseValues)
        {
            currentValues = GetRoleUsers();
            usUsers.Value = currentValues;
            usUsers.Reload();
        }

        if (saved)
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Check whether current user is allowed to modify another user. Return "" or error message.
    /// </summary>
    /// <param name="userId">Modified user</param>
    protected string ValidateGlobalAndDeskAdmin(int userId)
    {
        string result = String.Empty;

        if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            return result;
        }

        UserInfo userInfo = UserInfo.Provider.Get(userId);
        if (userInfo == null)
        {
            result = GetString("Administration-User.WrongUserId");
        }
        else
        {
            if (userInfo.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                result = String.Format(GetString("Administration-User.NotAllowedToModifySpecific"), $"{HTMLHelper.HTMLEncode(userInfo.FullName)} ({userInfo.UserName})");
            }
        }
        return result;
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        reloadData = true;
        currentValues = GetRoleUsers();
    }


    /// <summary>
    /// Page PreRender.
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        if (reloadData)
        {
            currentValues = GetRoleUsers();
            usUsers.Value = currentValues;
            usUsers.ReloadData();
        }

        if (RequestHelper.IsPostBack())
        {
            pnlBasic.Update();
        }

        base.OnPreRender(e);
    }

    #endregion
}