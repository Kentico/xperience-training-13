using System;

using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[Help("new_chat_room")]

[Breadcrumbs()]
[Breadcrumb(0, "chat.chatroom.list", "~/CMSModules/Chat/Pages/Tools/ChatRoom/List.aspx?siteid={?siteid?}", null)]
[Breadcrumb(1, ResourceString = "chat.chatroom.new")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_New : CMSChatPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        int siteID = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);

        editElem.SiteID = siteID;

        CheckReadPermission(siteID);

        base.OnInit(e);
    }

    #endregion
}
