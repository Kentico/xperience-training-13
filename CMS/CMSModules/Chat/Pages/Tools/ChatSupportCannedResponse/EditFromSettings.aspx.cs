using System;

using CMS.Chat;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject(ChatSupportCannedResponseInfo.OBJECT_TYPE, "responseId")]

[Title("chat.support.settingstitle")]

[Breadcrumbs]
[Breadcrumb(0, "chat.support.settingstitle", "~/CMSModules/Chat/Pages/ChatSupportSettings.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.ChatSupportCannedResponseTagName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "chat.cannedresponse.chatsupportcannedresponse.new", NewObject = true)]

public partial class CMSModules_Chat_Pages_Tools_ChatSupportCannedResponse_EditFromSettings : CMSModalPage
{
    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        editElem.Personal = true;

        // Register the Save and close button as the form submit button
        HeaderActions.Visible = true;
        editElem.EditForm.SubmitButton.Visible = false;

        string url = URLHelper.AddParameterToUrl("~/CMSModules/Chat/Pages/ChatSupportSettings.aspx", "siteid", "{?siteid?}");
        url = URLHelper.AddParameterToUrl(url, "responseid", "{%EditedObject.ID%}");
        url = URLHelper.AddParameterToUrl(url, "saved", "1");
        editElem.EditForm.RedirectUrlAfterSave = url;
        Save += (s, ea) => { editElem.EditForm.SaveData(null); };
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        CurrentMaster.HeaderActions.ActionsList.RemoveAll(p => true);
        CurrentMaster.DisplayActionsPanel = false;
    }

    #endregion
}
