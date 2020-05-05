using System;

using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Help("chat_edit_canned_response")]

[EditedObject(ChatSupportCannedResponseInfo.OBJECT_TYPE, "responseId")]

[Breadcrumbs()]
[Breadcrumb(0, "chat.cannedresponse.chatsupportcannedresponse.list", "~/CMSModules/Chat/Pages/Tools/ChatSupportCannedResponse/List.aspx?siteid={?siteid?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.ChatSupportCannedResponseTagName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "chat.cannedresponse.chatsupportcannedresponse.new", NewObject = true)]

public partial class CMSModules_Chat_Pages_Tools_ChatSupportCannedResponse_Edit : CMSChatPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        ChatSupportCannedResponseInfo editedObject = (ChatSupportCannedResponseInfo)UIContext.EditedObject;
        string url = URLHelper.AddParameterToUrl("~/CMSModules/Chat/Pages/Tools/ChatSupportCannedResponse/Edit.aspx", "siteid", "{?siteid?}");
        url = URLHelper.AddParameterToUrl(url, "responseid", "{%EditedObject.ID%}");
        editElem.EditForm.RedirectUrlAfterCreate = URLHelper.AddParameterToUrl(url, "saved", "1");

        if (editedObject != null)
        {
            CheckReadPermission(editedObject.ChatSupportCannedResponseSiteID);
        }

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.Personal = false;
    }

    #endregion
}
