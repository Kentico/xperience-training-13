using System;

using CMS.UIControls;


[Title("chat.support.settingstitle")]

[Action(0, "chat.cannedresponse.chatsupportcannedresponse.new", "~/CMSModules/Chat/Pages/Tools/ChatSupportCannedResponse/EditFromSettings.aspx")]

public partial class CMSModules_Chat_Pages_ChatSupportSettings : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        listElem.FromSettings = true;
    }
}