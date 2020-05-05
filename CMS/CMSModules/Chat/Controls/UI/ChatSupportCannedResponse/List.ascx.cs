using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Chat_Controls_UI_ChatSupportCannedResponse_List : CMSAdminListControl
{
    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return this.gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            this.gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }

    /// <summary>
    /// Indicates if the control is used from settings rather than edit control.
    /// </summary>
    public bool FromSettings { get; set; }


    /// <summary>
    /// SiteID
    /// </summary>
    public int SiteID { get; set; }

    #endregion


    #region "Methods"

    /// <summary>
    /// Display only canned responses created by specified chat user.
    /// </summary>
    /// <param name="userID">Chat user id</param>
    public void DisplayUserSpecific(int? userID)
    {
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        Grid.OnAction += new OnActionEventHandler(Grid_OnAction);
        Grid.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);

        if (FromSettings)
        {
            Grid.EditActionUrl = "~/CMSModules/Chat/Pages/Tools/ChatSupportCannedResponse/EditFromSettings.aspx?responseId={0}";

            Grid.WhereCondition = "ChatSupportCannedResponseChatUserID = " + ChatUserHelper.GetChatUserFromCMSUser().ChatUserID;
        }
        else
        {
            Grid.EditActionUrl = "~/CMSModules/Chat/Pages/Tools/ChatSupportCannedResponse/Edit.aspx?responseId={0}&siteid=" + SiteID;

            // Single site is selected.
            if (SiteID > 0)
            {
                Grid.WhereCondition = string.Format("ChatSupportCannedResponseSiteID = {0} AND ChatSupportCannedResponseChatUserID IS NULL", SiteID);
            }
            // Global is selected
            else if (SiteID == -4)
            {
                Grid.WhereCondition = "ChatSupportCannedResponseSiteID IS NULL AND ChatSupportCannedResponseChatUserID IS NULL";
            }
            // Global and current is selected
            else if (SiteID == -5)
            {
                Grid.WhereCondition = string.Format("(ChatSupportCannedResponseSiteID = {0} OR ChatSupportCannedResponseSiteID IS NULL) AND ChatSupportCannedResponseChatUserID IS NULL", SiteContext.CurrentSiteID);
            }
        }

        RegisterRefreshUsingPostBackScript();
    }


    object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();

        switch (sourceName)
        {
            case "delete":
                if (!FromSettings)
                {
                    DataRow row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

                    int? siteID = row.IsNull("ChatSupportCannedResponseSiteID") ? (int?)null : ValidationHelper.GetInteger(row["ChatSupportCannedResponseSiteID"], 0);

                    bool hasUserModifyPermission = ((CMSChatPage)Page).HasUserModifyPermission(siteID);

                    CMSGridActionButton button = ((CMSGridActionButton)sender);

                    if (!hasUserModifyPermission)
                    {
                        button.Enabled = false;
                    }
                }
                break;
        }

        return parameter;
    }


    void Grid_OnAction(string actionName, object actionArgument)
    {
        int cannedResponseID = ValidationHelper.GetInteger(actionArgument, 0);
        switch (actionName)
        {
            case "delete":
                ChatSupportCannedResponseInfo cannedResponse = ChatSupportCannedResponseInfoProvider.GetChatSupportCannedResponseInfo(cannedResponseID);

                if (cannedResponse == null)
                {
                    return;
                }

                CheckModifyPermissions(cannedResponse);

                ChatSupportCannedResponseInfoProvider.DeleteChatSupportCannedResponseInfo(cannedResponseID);
                break;
        }
    }


    void CheckModifyPermissions(ChatSupportCannedResponseInfo cannedResponse)
    {
        if (cannedResponse.ChatSupportCannedResponseChatUserID.HasValue)
        {
            var currentUser = MembershipContext.AuthenticatedUser;

            if (!currentUser.IsAuthorizedPerResource("CMS.Chat", "EnterSupport"))
            {
                RedirectToAccessDenied("CMS.Chat", "EnterSupport");
            }

            if (cannedResponse.ChatSupportCannedResponseChatUserID.Value != ChatUserHelper.GetChatUserFromCMSUser().ChatUserID)
            {
                RedirectToAccessDenied(GetString("general.modifynotallowed"));
            }
        }
        else
        {
            // In "per site mode" Page is always CMSChatPage
            ((CMSChatPage)Page).CheckModifyPermission(cannedResponse.ChatSupportCannedResponseSiteID);
        }
    }


    private void RegisterRefreshUsingPostBackScript()
    {
        string script = @"
function RefreshUsingPostBack()
{{
    {0};
}}";
        script = string.Format(script, ControlsHelper.GetPostBackEventReference(btnHiddenPostBackButton, null));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshUsingPostBack", script, true);
    }

    #endregion
}
