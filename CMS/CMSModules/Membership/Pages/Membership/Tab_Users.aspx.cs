using System;
using System.Data;
using System.Linq;
using System.Text;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Membership_Tab_Users : CMSMembershipPage
{
    #region "Variables"

    /// <summary>
    /// Membership ID.
    /// </summary>
    private int membershipID = 0;

    /// <summary>
    /// User IDs of edited membership.
    /// </summary>
    private string currentValues = String.Empty;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        membershipID = QueryHelper.GetInteger("membershipID", 0);
        MembershipInfo mi = MembershipInfo.Provider.Get(membershipID);

        EditedObject = mi;

        // Test permissions
        CheckMembershipPermissions(mi);

        usUsers.AdditionalColumns = "UserID,ValidTo";
        usUsers.GridName = "~/CMSModules/Membership/Pages/Users/UsersValidTo.xml";
        usUsers.DynamicColumnName = false;
        usUsers.SelectItemPageUrl = "~/CMSModules/Membership/Pages/Users/User_Edit_Add_Item_Dialog.aspx";
        usUsers.AdditionalUrlParameters = "&UseSendNotification=1";
        usUsers.WhereCondition = (mi.MembershipSiteID > 0) ? $"UserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID={mi.MembershipSiteID})" : String.Empty;
        usUsers.ListingWhereCondition = "MembershipID =" + membershipID;
        usUsers.ReturnColumnName = "UserID";
        usUsers.DialogWindowHeight = 790;

        // Load data in administration
        currentValues = GetMembershipUsers();
        if (!RequestHelper.IsPostBack())
        {
            usUsers.Value = currentValues;
        }


        string script = "function setNewDateTime(date) {$cmsj('#" + hdnDate.ClientID + "').val(date);}";
        script += "function setNewSendNotification(sendNotification) {$cmsj('#" + hdnSendNotification.ClientID + "').val(sendNotification);}";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "key", ScriptHelper.GetScript(script));

        usUsers.OnSelectionChanged += UniSelector_OnSelectionChanged;
        usUsers.OnAdditionalDataBound += usUsers_OnAdditionalDataBound;

        // Manage single item valid to change by calendar
        string eventTarget = Request[postEventSourceID];
        string eventArgument = Request[postEventArgumentID];
        if (eventTarget == ucCalendar.DateTimeTextBox.UniqueID)
        {
            // Check "modify" permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Membership", "ManageUserMembership"))
            {
                RedirectToAccessDenied("CMS.Membership", "Manage user membership");
            }

            int id = ValidationHelper.GetInteger(hdnDate.Value, 0);

            if (id != 0)
            {
                DateTime dt = ValidationHelper.GetDateTime(eventArgument, DateTimeHelper.ZERO_TIME);
                MembershipUserInfo mui = MembershipUserInfo.Provider.Get(mi.MembershipID, id);
                if (mui != null)
                {
                    mui.ValidTo = dt;
                    MembershipUserInfo.Provider.Set(mui);

                    // Invalidate changes
                    UserInfoProvider.InvalidateUser(mui.UserID);

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
            // Resolve calendar image
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
                    onClick = $"$cmsj('#{hdnDate.ClientID}').val('{itemID}'); var dt = $cmsj('#{ucCalendar.DateTimeTextBox.ClientID}'); dt.val('{date}'); dt.cmsdatepicker('setLocation','{iconID}'); dt.cmsdatepicker ('show');";
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


    private void SaveUsers()
    {
        // Check "manage user membership" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Membership", "ManageUserMembership"))
        {
            RedirectToAccessDenied("CMS.Membership", "Manage user membership");
        }

        // Remove old items
        string newValues = ValidationHelper.GetString(usUsers.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        bool reload = false;
        bool saved = false;
        StringBuilder errors = new StringBuilder();

        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int userId = ValidationHelper.GetInteger(item, 0);
                    string result = ValidateGlobalAndDeskAdmin(userId);

                    if (result != String.Empty)
                    {
                        errors.AppendLine(result);
                        reload = true;
                        continue;
                    }
                    else
                    {
                        MembershipUserInfo.Provider.Remove(membershipID, userId);

                        // Invalidate UserInfo
                        UserInfoProvider.InvalidateUser(userId);

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
            DateTime dt = ValidationHelper.GetDateTime(hdnDate.Value, DateTimeHelper.ZERO_TIME);
            bool sendNotification = ValidationHelper.GetBoolean(hdnSendNotification.Value, false);

            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int userId = ValidationHelper.GetInteger(item, 0);
                    string result = ValidateGlobalAndDeskAdmin(userId);

                    if (result != String.Empty)
                    {
                        errors.AppendLine(result);
                        reload = true;
                        continue;
                    }
                    else
                    {
                        MembershipUserInfo.Provider.Add(membershipID, userId, dt, sendNotification);

                        // Invalidate UserInfo
                        UserInfoProvider.InvalidateUser(userId);

                        saved = true;
                    }
                }
            }
        }

        if (errors.Length > 0)
        {
            ShowError(GetString("general.saveerror"), errors.ToString(), null);
        }

        if (reload)
        {
            currentValues = GetMembershipUsers();
            usUsers.Value = currentValues;
        }

        if (saved)
        {
            ShowChangesSaved();
        }

        usUsers.Reload(true);
    }


    /// <summary>
    /// Returns users related to membership.
    /// </summary>
    private string GetMembershipUsers()
    {
        var data = MembershipUserInfo.Provider.Get().Where("MembershipID = " + membershipID).Columns("UserID");
        if (data.Any())
        {
            return TextHelper.Join(";", DataHelper.GetStringValues(data.Tables[0], "UserID"));
        }

        return String.Empty;
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
