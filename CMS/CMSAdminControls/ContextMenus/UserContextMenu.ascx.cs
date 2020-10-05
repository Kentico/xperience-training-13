using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;


public partial class CMSAdminControls_ContextMenus_UserContextMenu : CMSContextMenuControl, IPostBackEventHandler
{
    #region "Variables"

    private CurrentUserInfo currentUser = null;
    protected int requestedUserId = 0;

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

        // Group invitation
        script += "function ContextGroupInvitation(id) { \nmodalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/InviteToGroup.aspx") + "?invitedid=' + id , 'inviteToGroup', 500, 450); \n } \n";

        // Redirect to sign in URL
        string signInUrl = MacroResolver.Resolve(AuthenticationHelper.DEFAULT_LOGON_PAGE);
        if (signInUrl != "")
        {
            signInUrl = "window.location.replace('" + URLHelper.AddParameterToUrl(ResolveUrl(signInUrl), "ReturnURL", Server.UrlEncode(RequestContext.CurrentURL)) + "');";
        }

        script += "function ContextRedirectToSignInUrl() { \n" + signInUrl + "} \n";

        // Register menu management scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UserContextMenuManagement", ScriptHelper.GetScript(script));

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


    /// <summary>
    /// Postback handling.
    /// </summary>
    /// <param name="eventArgument">Argument of postback event</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if ((eventArgument == null))
        {
            return;
        }

        // Get ID of user
        int selectedId = ValidationHelper.GetInteger(ContextMenu.Parameter, 0);
        if (selectedId == 0)
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UserContextMenuError", ScriptHelper.GetScript("alert('No user was selected.');"));
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        requestedUserId = ValidationHelper.GetInteger(ContextMenu.Parameter, 0);

        DataTable table = new DataTable();
        table.Columns.Add("ActionDisplayName");
        table.Columns.Add("ActionScript");

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
