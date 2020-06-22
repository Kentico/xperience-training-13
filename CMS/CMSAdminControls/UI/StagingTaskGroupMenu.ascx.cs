using System;
using System.Collections.Generic;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Internal;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

using Newtonsoft.Json;

/// <summary>
/// Control that enables user to manage staging task groups from everywhere in UI.
/// </summary>
public partial class CMSAdminControls_UI_StagingTaskGroupMenu : CMSUserControl, ICallbackEventHandler
{
    #region "Variables"

    // Dictionary to save key-value data to be sent for the client.
    private readonly Dictionary<string, object> callbackResult = new Dictionary<string, object>();

    // String that is shown in title, when no task group is selected.
    private readonly string selectStagingTaskGroupText = HTMLHelper.HTMLEncode(ResHelper.GetString("staging.SelectStagingTaskGroup"));

    // Text that redirects user to staging app, when no task group is selected.
    private readonly string goToStagingAppText = HTMLHelper.HTMLEncode(ResHelper.GetString("staging.GoToStagingApp"));

    // Text used to redirect user into current task group.
    private readonly string goToCurrentTaskGroupText = HTMLHelper.HTMLEncode(ResHelper.GetString("staging.EditLink"));

    #endregion


    #region "Constants"

    /// Name of UI element where redirections from link will be.
    private const string STAGINGTASKGROUP_EDIT_ELEMENT = "EditTaskGroup";

    /// Name of UI element, that will be used for creating url for current task group, so that breadcrumbs works correctly.
    private const string STAGINGTASKGROUP_PARENT_TABS = "TaskGroups";

    /// Name of UI element, that will be used for creating url for staging application when no group is selected
    private const string STAGING_APPLICATION = "Staging";

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns uniselector's client id.
    /// </summary>
    private string UniSelectorClientID
    {
        get
        {
            return stagingTaskGroupSelector.InputClientID;
        }
    }


    /// <summary>
    /// Current task group that was sent from client.
    /// </summary>
    private TaskGroupInfo CurrentStagingTaskGroup
    {
        get;
        set;
    }

    #endregion


    #region "Life-Cycle methods"

    /// <summary>
    /// Loads page, sets currently used task group and registers javascript.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        stagingTaskGroupSelector.DisplayNameFormat = "{%" + TaskGroupInfo.TYPEINFO.CodeNameColumn + "%}";

        btnCreateTaskGroup.Text = HTMLHelper.HTMLEncode(GetString("staging.btnCreateTaskGroup"));
        lblStagingTaskGroupSelector.AssociatedControlClientID = UniSelectorClientID;

        // Hide possibility to create task group in dialog
        plcCreateTaskGroup.Visible = UserCanManageTaskGroups(CMSActionContext.CurrentUser);

        // Hides link to staging, if user does not have permission
        lnkEditTaskGroup.Visible = UserCanAccessStagingUI(CMSActionContext.CurrentUser);

        // If too many objects are in UniSelector and dialog window is open, change user's task group
        stagingTaskGroupSelector.OnSelectionChanged += (s, ev) =>
        {
            // Register script so in iframe under header, we will stay on the same page
            PortalScriptHelper.RegisterAdminRedirectScript(Page);
            TaskGroupInfoProvider.SetTaskGroupForUser(Convert.ToInt32(stagingTaskGroupSelector.Value), CMSActionContext.CurrentUser.UserID);
        };
    }


    /// <summary>
    /// Calls base class OnPreRender and registers javascript into page.
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Set selector and link name to work with current staging task group
        var currentTaskGroup = TaskGroupInfoProvider.GetUserTaskGroupInfo(CMSActionContext.CurrentUser.UserID);
        if (currentTaskGroup != null)
        {
            stagingTaskGroupSelector.Value = currentTaskGroup.TaskGroupID;
            lnkEditTaskGroup.Text = goToCurrentTaskGroupText + " " + currentTaskGroup.TaskGroupCodeName;
            lblStagingTaskGroupMenuText.Text = currentTaskGroup.TaskGroupCodeName;
        }
        else
        {
            lnkEditTaskGroup.Text = goToStagingAppText;
            lblStagingTaskGroupMenuText.Text = selectStagingTaskGroupText;
        }

        JavascriptRegistration();
    }

    #endregion


    #region "ICallbackEventHandler"

    /// <summary>
    /// Returns JSON serialized data prepared in <see cref="RaiseCallbackEvent"/>.
    /// </summary>
    /// <returns>Returns JSON serialized data</returns>
    public string GetCallbackResult()
    {
        return JsonConvert.SerializeObject(callbackResult, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
    }


    /// <summary>
    /// Callback from client. Callback result data are prepared to be sent back to the client.
    /// </summary>
    /// <param name="eventArgument">Event arguments in JSON, like which control raised callback, and another data</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        try
        {
            Dictionary<string, string> callbackData = GetCallbackData(eventArgument);

            // User has changed the task group
            if (GetRaisedCallbackControlID(callbackData).Equals(UniSelectorClientID, StringComparison.InvariantCultureIgnoreCase))
            {
                // If given task group does not exists on the server anymore null is set, and return that selector is inconsistent
                CurrentStagingTaskGroup = TaskGroupInfo.Provider.Get(GetRaisedCallbackTaskGroupID(callbackData));

                if ((CurrentStagingTaskGroup == null) && (GetRaisedCallbackTaskGroupID(callbackData) != 0))
                {
                    // Task group is selected that no longer exists on server
                    ShowMessageInCallback(GetString("stagingTaskGroup.SelectorInconsistent"));
                }
            }
            // User clicked the edit link, we have to check if task group still exists
            else if (GetRaisedCallbackControlID(callbackData).Equals(lnkEditTaskGroup.ClientID, StringComparison.InvariantCultureIgnoreCase))
            {
                PrepareLinkForRedirect(callbackData);
                return;
            }
            else
            {
                // User created new task group
                CurrentStagingTaskGroup = CreateNewTaskGroup(callbackData["Name"]);
            }

            // Return ID of staging task group to client, to change selector and menu text
            callbackResult.Add("stagingTaskGroupID", CurrentStagingTaskGroup == null ? UniSelector.US_NONE_RECORD : CurrentStagingTaskGroup.TaskGroupID);
            TaskGroupInfoProvider.SetTaskGroupForUser(CurrentStagingTaskGroup == null ? UniSelector.US_NONE_RECORD : CurrentStagingTaskGroup.TaskGroupID, CMSActionContext.CurrentUser.UserID);
        }
        catch (Exception ex)
        {
            ShowMessageInCallback(GetString("stagingTaskGroupMenu.ExceptionMesasge") + " " + ex.Message);
            Service.Resolve<IEventLogService>().LogException("StagingTaskGroupMenu", "TASKGROUP", ex);
        }
    }

    #endregion


    #region "Helper Methods"

    /// <summary>
    /// Gets key-value pairs from JSON object sent by client
    /// </summary>
    /// <param name="eventArgument">JSON sent by client</param>
    private Dictionary<string, string> GetCallbackData(string eventArgument)
    {
        Dictionary<string, string> callbackData = JsonConvert.DeserializeObject<Dictionary<string, string>>(eventArgument);
        return callbackData;
    }


    /// <summary>
    /// Returns url for currently used staging task group.
    /// </summary>
    /// <param name="CurrentStagingTaskGroup">Current task group</param>
    private static string GetUrlTostagingTaskGroup(TaskGroupInfo CurrentStagingTaskGroup)
    {
        return UrlResolver.ResolveUrl(Service.Resolve<IUILinkProvider>().GetSingleObjectLink(StagingTaskInfo.TYPEINFO.ModuleName, STAGINGTASKGROUP_EDIT_ELEMENT, new ObjectDetailLinkParameters
        {
            ObjectIdentifier = CurrentStagingTaskGroup.TaskGroupID,
            ParentTabName = STAGINGTASKGROUP_PARENT_TABS,
            AllowNavigationToListing = true,
            TabName = "TaskGroup"
        }));
    }


    /// <summary>
    /// Registers javascript module and callback.
    /// </summary>
    private void JavascriptRegistration()
    {
        ScriptHelper.RegisterModule(this, "AdminControls/StagingTaskGroupMenu", new
        {
            btnCreateTaskGroup = btnCreateTaskGroup.ClientID,
            taskGroupSelector = UniSelectorClientID,
            taskGroupMenuText = lblStagingTaskGroupMenuText.ClientID,
            lnkEditTaskGroup = lnkEditTaskGroup.ClientID,
            inputTaskGroup = inputTaskGroup.ClientID,
            noneOption = UniSelector.US_NONE_RECORD,
            stagingTaskGroupMenuDropdown = stagingTaskGroupMenuDropdown.ClientID,
            codeNameMessage = pnlCodeNameMessage.ClientID,
            noneText = selectStagingTaskGroupText
        });

        string cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "receiveStagingTaskGroupMenuData", "context", "taskGroupMenuError", true);

        string callbackScript = "function taskGroupMenuCallback(arg, context)" + "{ " + cbReference + ";} ";

        string clientFunction = @"
        function taskGroupMenuError(errorMessage){
            alert(errorMessage);
            return false;
        }
           
        function receiveStagingTaskGroupMenuData(callBackResult, context) {
            callBackResult = JSON.parse(callBackResult);

            if (callBackResult.message !== undefined && callBackResult.message.length > 0) {
                alert(callBackResult.message);
                return;
            }

            if (callBackResult.redirect !== undefined && callBackResult.redirect == true) {
                window.location.replace(callBackResult.stagingTaskGroupUrl);
                return;
            }

            var selector = $cmsj('#" + UniSelectorClientID + @"')[0];
            var optionIndex = selector.length;
            if (callBackResult.stagingTaskGroupCreated == true) {
                if (selector.options[optionIndex - 1].value == " + UniSelector.US_MORE_RECORDS + @") {
                    optionIndex = optionIndex - 1;
                }
                selector.options.add(new Option(callBackResult.stagingTaskGroupDisplayName, callBackResult.stagingTaskGroupID), optionIndex);
            }

            selector.value = callBackResult.stagingTaskGroupID;
            var menuTitle = selector.options[selector.selectedIndex].text;

            var link = $cmsj('#" + lnkEditTaskGroup.ClientID + @"')[0];
            var linkText = '" + goToCurrentTaskGroupText + @" ' + menuTitle;
            if (selector.value == 0) {
                linkText = '" + goToStagingAppText + @"';
                menuTitle = '" + selectStagingTaskGroupText + @"';
            }

            if (" + UserCanAccessStagingUI(CMSActionContext.CurrentUser).ToString().ToLowerCSafe()+ @") {
                link.textContent = linkText;
            }

            $cmsj('#" + lblStagingTaskGroupMenuText.ClientID + @"').text(menuTitle);
            return false;
        }";

        ScriptHelper.RegisterStartupScript(this, typeof(string), ClientID, ScriptHelper.GetScript(callbackScript + clientFunction));
    }


    /// <summary>
    /// Gets ID of control which raised callback from callback data.
    /// </summary>
    /// <param name="callbackData">Data sent from browser in key-value pair format</param>
    public string GetRaisedCallbackControlID(Dictionary<string, string> callbackData)
    {
        string controlID;
        callbackData.TryGetValue("Control", out controlID);
        return controlID ?? String.Empty;
    }


    /// <summary>
    /// Gets ID staging task group sent from client.
    /// </summary>
    /// <param name="callbackData">Data sent from browser in key-value pair format</param>
    public int GetRaisedCallbackTaskGroupID(Dictionary<string, string> callbackData)
    {
        string taskGroupID;
        callbackData.TryGetValue("ID", out taskGroupID);
        return ValidationHelper.GetInteger(taskGroupID, 0);
    }


    /// <summary>
    /// Handles the click on edit link and redirects user to correct task group.
    /// </summary>
    /// <param name="callbackData">Data sent from browser in key-value pair format</param>
    private void PrepareLinkForRedirect(Dictionary<string, string> callbackData)
    {
        CurrentStagingTaskGroup = TaskGroupInfo.Provider.Get(GetRaisedCallbackTaskGroupID(callbackData));
        callbackResult.Add("redirect", true);

        if (CurrentStagingTaskGroup != null)
        {
            callbackResult.Add("stagingTaskGroupUrl", GetUrlTostagingTaskGroup(CurrentStagingTaskGroup));
        }
        else
        {
            // There is no such task group or (none) is selected so we will redirect to staging application
            callbackResult.Add("stagingTaskGroupUrl", UrlResolver.ResolveUrl(Service.Resolve<IUILinkProvider>().GetSingleObjectLink(StagingTaskInfo.TYPEINFO.ModuleName, STAGING_APPLICATION)));
        }
    }

    /// <summary>
    /// Creates new task group and prepares callback result.
    /// </summary>
    /// <param name="nameOfCreatedstagingTaskGroup">Name of staging task group</param>
    private TaskGroupInfo CreateNewTaskGroup(string nameOfCreatedstagingTaskGroup)
    {
        TaskGroupInfo taskGroup = TaskGroupInfo.Provider.Get(nameOfCreatedstagingTaskGroup);

        if (taskGroup != null)
        {
            // If task group already exists, return that task group
            return taskGroup;
        }

        if (ValidationHelper.IsCodeName(nameOfCreatedstagingTaskGroup))
        {
            taskGroup = new TaskGroupInfo
            {
                TaskGroupGuid = Guid.NewGuid(),
                TaskGroupCodeName = nameOfCreatedstagingTaskGroup.Truncate(TaskGroupInfo.TYPEINFO.MaxCodeNameLength),
            };
            TaskGroupInfo.Provider.Set(taskGroup);
            callbackResult.Add("stagingTaskGroupCreated", true);
            callbackResult.Add("stagingTaskGroupDisplayName", HTMLHelper.HTMLEncode(taskGroup.TaskGroupCodeName));
        }
        else
        {
            ShowMessageInCallback(GetString("staging.IsNotValidTaskGroupName"));
        }

        return taskGroup;
    }


    /// <summary>
    /// Returns message for user in callback data, that will be shown via alert dialog.
    /// </summary>
    /// <param name="messageText">Text to be displayed</param>
    private void ShowMessageInCallback(string messageText)
    {
        callbackResult.Add("message", HTMLHelper.EncodeForHtmlAttribute(messageText));
    }


    /// <summary>
    /// Checks if user has permissions for staging app.
    /// </summary>
    private bool UserCanAccessStagingUI(IUserInfo user)
    {
        return (user.IsAuthorizedPerResource(StagingTaskInfo.TYPEINFO.ModuleName, StagingTaskInfo.PERMISSION_MANAGE_ALL_TASKS, SiteContext.CurrentSiteName, false)
            || user.IsAuthorizedPerResource(StagingTaskInfo.TYPEINFO.ModuleName, StagingTaskInfo.PERMISSION_MANAGE_DOCUMENT_TASKS, SiteContext.CurrentSiteName, false)
            || user.IsAuthorizedPerResource(StagingTaskInfo.TYPEINFO.ModuleName, StagingTaskInfo.PERMISSION_MANAGE_OBJECT_TASKS, SiteContext.CurrentSiteName, false)
            || user.IsAuthorizedPerResource(StagingTaskInfo.TYPEINFO.ModuleName, StagingTaskInfo.PERMISSION_MANAGE_DATA_TASKS, SiteContext.CurrentSiteName, false));
    }


    /// <summary>
    /// Can user create, edit or delete task groups.
    /// </summary>
    private bool UserCanManageTaskGroups(IUserInfo user)
    {
        return user.IsAuthorizedPerResource(StagingTaskInfo.TYPEINFO.ModuleName, TaskGroupInfo.PERMISSION_MANAGE_TASK_GROUPS, SiteContext.CurrentSiteName, false);
    }

    #endregion
}
