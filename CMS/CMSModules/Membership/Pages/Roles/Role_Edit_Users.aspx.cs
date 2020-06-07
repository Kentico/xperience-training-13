using System;
using System.Data;
using System.Linq;
using System.Text;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Roles_Role_Edit_Users : CMSRolesPage
{
    private string currentValues = String.Empty;
    private int roleID = 0;


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        var user = MembershipContext.AuthenticatedUser;
        if (user != null)
        {
            if (!user.IsAuthorizedPerUIElement("CMS.Roles", "Users"))
            {
                RedirectToUIElementAccessDenied("CMS.Roles", "Users");
            }

            // Check "read" permissions
            if (!user.IsAuthorizedPerResource("CMS.Users", "Read"))
            {
                RedirectToAccessDenied("CMS.Users", "Read");
            }
        }

        usUsers.AdditionalColumns = "UserID,ValidTo";
        usUsers.GridName = "~/CMSModules/Membership/Pages/Users/UsersValidTo.xml";
        roleID = QueryHelper.GetInteger("roleid", 0);
        usUsers.IsLiveSite = false;
        usUsers.DialogWindowHeight = 760;

        // Show only user belonging to role's site
        RoleInfo ri = RoleInfo.Provider.Get(roleID);
        if (ri != null)
        {
            usUsers.WhereCondition = (ri.SiteID > 0) ? $"UserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID = {ri.SiteID})" : String.Empty;
            usUsers.ListingWhereCondition = "RoleID = " + ri.RoleID;
        }

        usUsers.DynamicColumnName = false;
        usUsers.SelectItemPageUrl = "~/CMSModules/Membership/Pages/Users/User_Edit_Add_Item_Dialog.aspx";

        currentValues = GetRoleUsers();
        if (!RequestHelper.IsPostBack())
        {
            usUsers.Value = currentValues;
        }

        usUsers.OnSelectionChanged += UniSelector_OnSelectionChanged;
        usUsers.OnAdditionalDataBound += usUsers_OnAdditionalDataBound;

        string script = "function setNewDateTime(date) {$cmsj('#" + hdnDate.ClientID + "').val(date);}";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "key", ScriptHelper.GetScript(script));

        string eventTarget = Request[postEventSourceID];
        string eventArgument = Request[postEventArgumentID];
        if (eventTarget == ucCalendar.DateTimeTextBox.UniqueID)
        {
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "ManageUserRoles"))
            {
                RedirectToAccessDenied("CMS.Users", "ManageUserRoles");
            }

            int id = ValidationHelper.GetInteger(hdnDate.Value, 0);
            if (id != 0)
            {
                DateTime dt = ValidationHelper.GetDateTime(eventArgument, DateTimeHelper.ZERO_TIME);
                UserRoleInfo uri = UserRoleInfo.Provider.Get(id, ri.RoleID);
                if (uri != null)
                {
                    uri.ValidTo = dt;
                    UserRoleInfo.Provider.Set(uri);

                    // Invalidate user
                    UserInfo.TYPEINFO.ObjectInvalidated(id);

                    ShowChangesSaved();
                }
            }
        }
    }


    /// <summary>
    /// Callback event for create calendar icon.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="sourceName">Event source name</param>
    /// <param name="parameter">Event parameter</param>
    /// <param name="val">Value from basic external data bound event</param>
    private object usUsers_OnAdditionalDataBound(object sender, string sourceName, object parameter, object val)
    {
        DataRowView drv = null;
        switch (sourceName.ToLowerCSafe())
        {
            case "calendar":
                drv = (parameter as DataRowView);
                string itemID = drv[usUsers.ReturnColumnName].ToString();
                string iconID = "icon_" + itemID;
                string date = drv["ValidTo"].ToString();
                string postback = ControlsHelper.GetPostBackEventReference(ucCalendar.DateTimeTextBox, "#").Replace("'#'", "$cmsj('#" + ucCalendar.DateTimeTextBox.ClientID + "').val()");
                string onClick = String.Empty;

                ucCalendar.DateTimeTextBox.Attributes["OnChange"] = postback;

                if (!ucCalendar.UseCustomCalendar)
                {
                    onClick = $"$cmsj('#{hdnDate.ClientID}').val('{itemID}');{ucCalendar.GenerateNonCustomCalendarImageEvent()}";
                }
                else
                {
                    onClick = $"$cmsj('#{hdnDate.ClientID}').val('{itemID}'); var dt = $cmsj('#{ucCalendar.DateTimeTextBox.ClientID}'); dt.val('{date}'); dt.cmsdatepicker('setLocation','{iconID}'); dt.cmsdatepicker('show');";
                }

                var button = new CMSGridActionButton
                {
                    ToolTip = GetString("membership.changevalidity"),
                    IconCssClass = "icon-calendar",
                    OnClientClick = onClick + "return false;",
                    ID = iconID
                };

                val = button.GetRenderedHTML();

                break;

            // Resolve User name
            case "name":
                drv = (parameter as DataRowView);
                string name = ValidationHelper.GetString(drv["UserName"], String.Empty);
                string fullName = ValidationHelper.GetString(drv["FullName"], String.Empty);

                val = HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(name, fullName, String.Empty, false));
                break;
        }

        return val;
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveUsers();
    }


    private string GetRoleUsers()
    {
        var data = UserRoleInfo.Provider.Get().Where($"RoleID = {roleID}").Columns("UserID");
        if (data.Any())
        {
            return TextHelper.Join(";", DataHelper.GetStringValues(data.Tables[0], "UserID"));
        }

        return String.Empty;
    }


    private void SaveUsers()
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "ManageUserRoles"))
        {
            RedirectToAccessDenied("CMS.Users", "ManageUserRoles");
        }

        bool falseValues = false;
        bool saved = false;
        StringBuilder errors = new StringBuilder();

        // Remove old items
        string newValues = ValidationHelper.GetString(usUsers.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
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
                        errors.AppendLine(result);
                        falseValues = true;
                        continue;
                    }
                    else
                    {
                        var uri = UserRoleInfo.Provider.Get(userId, roleID);
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
                DateTime dt = ValidationHelper.GetDateTime(hdnDate.Value, DateTimeHelper.ZERO_TIME);

                // Add all new items to user
                foreach (string item in newItems)
                {
                    int userId = ValidationHelper.GetInteger(item, 0);

                    // Check permissions
                    string result = ValidateGlobalAndDeskAdmin(userId);
                    if (result != String.Empty)
                    {
                        errors.AppendLine(result);
                        falseValues = true;
                        continue;
                    }
                    else
                    {
                        UserRoleInfo.Provider.Add(userId, roleID, dt);
                        saved = true;
                    }
                }
            }
        }

        if (errors.Length > 0)
        {
            ShowError(GetString("general.saveerror"), errors.ToString(), null);
        }

        if (falseValues)
        {
            currentValues = GetRoleUsers();
            usUsers.Value = currentValues;
        }

        if (saved)
        {
            ShowChangesSaved();
        }

        usUsers.Reload(true);
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
    /// Page PreRender.
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        if (RequestHelper.IsPostBack())
        {
            pnlBasic.Update();
        }

        base.OnPreRender(e);
    }

    #endregion
}
