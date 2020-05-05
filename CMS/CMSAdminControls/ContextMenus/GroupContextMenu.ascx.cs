using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;


public partial class CMSAdminControls_ContextMenus_GroupContextMenu : CMSContextMenuControl
{
    #region "Variables"

    private CurrentUserInfo currentUser = null;
    protected int requestedGroupId = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if the community module is loaded.
    /// </summary>
    public bool CommunityPresent
    {
        get
        {
            if (!RequestStockHelper.Contains("commPresent"))
            {
                RequestStockHelper.Add("commPresent", ModuleManager.IsModuleLoaded(ModuleName.COMMUNITY));
            }
            return ValidationHelper.GetBoolean(RequestStockHelper.GetItem("commPresent"), false);
        }
    }

    #endregion


    #region "Events handling"

    /// <summary>
    /// OnLoad event.
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        repItem.ItemDataBound += repItem_ItemDataBound;

        currentUser = MembershipContext.AuthenticatedUser;
        string script = "";

        // Join the group
        script += "function ContextJoinTheGroup(id) { \n" +
                  "modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/JoinTheGroup.aspx") + "?groupid=' + id, 'joinTheGroup', 500, 180); \n" +
                  " } \n";
        // Leave the group
        script += "function ContextLeaveTheGroup(id) { \n" +
                  "modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/LeaveTheGroup.aspx") + "?groupid=' + id , 'leaveTheGroup', 500, 180); \n" +
                  " } \n";

        // Redirect to sign in URL
        string signInUrl = AuthenticationHelper.DEFAULT_LOGON_PAGE;
        if (!string.IsNullOrEmpty(signInUrl))
        {
            signInUrl = "window.location.replace('" + URLHelper.AddParameterToUrl(ResolveUrl(signInUrl), "ReturnURL", Server.UrlEncode(RequestContext.CurrentURL)) + "');";
        }
        script += "function ContextRedirectToSignInUrl() { \n" + signInUrl + "} \n";
        script += "function ReloadPage(){ window.location.replace(window.location.href); }";

        // Register menu management scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "GroupContextMenuManagement", ScriptHelper.GetScript(script));
        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);
    }


    /// <summary>
    /// Bounding event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected void repItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Panel pnlItem = (Panel)e.Item.FindControl("pnlItem");
        if (pnlItem != null)
        {
            int count = ValidationHelper.GetInteger(((DataRowView)e.Item.DataItem)["Count"], 0) - 1;
            if (e.Item.ItemIndex == count)
            {
                pnlItem.CssClass = "item-last";
            }

            string action = (string)((DataRowView)e.Item.DataItem)["ActionScript"];
            pnlItem.Attributes.Add("onclick", action + ";");
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        requestedGroupId = ValidationHelper.GetInteger(ContextMenu.Parameter, 0);

        DataTable table = new DataTable();
        table.Columns.Add("ActionDisplayName");
        table.Columns.Add("ActionScript");

        // Add only if community is present
        if (CommunityPresent)
        {
            // Get resource strings prefix
            string resourcePrefix = ContextMenu.ResourcePrefix;

            // View group profile
            string profileUrl = "";

            // Get group profile URL
            GeneralizedInfo infoObj = ModuleCommands.CommunityGetGroupInfo(requestedGroupId);

            table.Rows.Add(new object[] { ResHelper.GetString(resourcePrefix + ".viewgroup|group.viewgroup"), "window.location.replace('" + profileUrl + "');" });
            if (!currentUser.IsGroupMember(requestedGroupId))
            {
                table.Rows.Add(new object[] { ResHelper.GetString(resourcePrefix + ".joingroup|group.joingroup"), !currentUser.IsPublic() ? "ContextJoinTheGroup(GetContextMenuParameter('" + ContextMenu.MenuID + "'))" : "ContextRedirectToSignInUrl()" });
            }
            else
            {
                table.Rows.Add(new object[] { ResHelper.GetString(resourcePrefix + ".leavegroup|group.leavegroup"), !currentUser.IsPublic() ? "ContextLeaveTheGroup(GetContextMenuParameter('" + ContextMenu.MenuID + "'))" : "ContextRedirectToSignInUrl()" });
            }
        }

        // Add count column
        DataColumn countColumn = new DataColumn();
        countColumn.ColumnName = "Count";
        countColumn.DefaultValue = table.Rows.Count;

        table.Columns.Add(countColumn);
        repItem.DataSource = table;
        repItem.DataBind();
    }

    #endregion
}
