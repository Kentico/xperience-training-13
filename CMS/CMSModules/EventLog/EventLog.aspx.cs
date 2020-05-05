using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.EventLog;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_EventLog_EventLog : CMSEventLogPage
{
    #region "Protected variables"

    private int siteID = -1;

    private HeaderAction btnClearLog;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        string elementName = (SiteID > 0) ? "EventLog" : "Administration.EventLog";
        var uiElement = new UIElementAttribute(EventLogInfo.OBJECT_TYPE, elementName);
        uiElement.Check(this);

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        eventLog.OnCheckPermissions += eventLog_OnCheckPermissions;

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);

        // Setup page title text and image
        PageTitle.TitleText = GetString("EventLogList.Header");

        if (SiteID > 0)
        {
            pnlSites.Visible = false;
            siteID = SiteID;
        }
        else
        {
            // Set site selector
            CurrentMaster.DisplaySiteSelectorPanel = true;
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.OnlyRunningSites = false;
            siteSelector.AllowAll = true;
            siteSelector.UniSelector.SpecialFields.Add(new SpecialField { Text = GetString("EventLogList.GlobalEvents"), Value = "0" });
            siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
            if (RequestHelper.IsPostBack())
            {
                siteID = ValidationHelper.GetInteger(siteSelector.Value, 0);
            }
        }
        eventLog.SiteID = siteID;
        UIContext["SiteID"] = siteID;

        // Set actions
        CurrentMaster.HeaderActions.ActionsList.Add(btnClearLog = new HeaderAction
        {
            Text = GetString("EventLogList.Clear"),
            CommandName = "clear",
            OnClientClick = "return confirm(" + ScriptHelper.GetString(SiteID > 0 ? GetString("EventLogList.ClearAllConfirmation") : GetString("EventLogList.ClearConfirmation")) + ");"
        });
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // If user is not authorized, hide the clear button
        btnClearLog.Visible = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.EventLog", "ClearLog");
    }

    #endregion


    #region "UI Handlers"

    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Update eventLog
        eventLog.EventLogGrid.Pager.UniPager.CurrentPage = 1;
        eventLog.ReloadData();
        eventLog.Update();
    }


    /// <summary>
    /// Handles clear button action.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
        {
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.EventLog", "ClearLog"))
            {
                RedirectToAccessDenied("CMS.EventLog", "ClearLog");
            }

            UserInfo ui = MembershipContext.AuthenticatedUser;

            // Deletes event logs of specific site from DB
            EventLogHelper.ClearEventLog(ui.UserID, ui.UserName, RequestContext.UserHostAddress, siteID);

            eventLog.ReloadData();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// OnCheckPermission event handler.
    /// </summary>
    /// <param name="permissionType">Type of the permission</param>
    /// <param name="sender">The sender</param>
    private void eventLog_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckPermissions(true);
    }

    #endregion
}