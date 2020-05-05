using System;

using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSWebParts_Chat_ChatRoomName : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Chat room name transformation
    /// </summary>
    public string ChatRoomNameTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatRoomNameTransformationName"), ChatSettingsProvider.TransformationRoomName);
        }
        set
        {
            this.SetValue("ChatRoomNameTransformationName", value);
        }
    }


    /// <summary>
    /// Group ID
    /// </summary>
    public string GroupID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupID"), "DefaultGroup");
        }
        set
        {
            this.SetValue("GroupID", value);
        }
    }


    /// <summary>
    /// Wheather to display initial title or not
    /// </summary>
    public bool DisplayInitialTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayInitialTitle"), true);
        }
        set
        {
            this.SetValue("DisplayInitialTitle", value);
        }
    }


    /// <summary>
    /// Initial title text
    /// </summary>
    public string InitialTitle
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("InitialTitle"), ResHelper.GetString("chat.roomname.initialtitle"));
        }
        set
        {
            this.SetValue("InitialTitle", value);
        }
    }


    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }

    #endregion


    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatUIHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeRoomName", this, InnerContainerTitle, InnerContainerName);
        ChatCssHelper.RegisterStylesheet(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatScriptHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Script references insertion
        ChatScriptHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatRoomName_files/ChatRoomName.js");

        // Run script
        string json = JsonConvert.SerializeObject(
            new
            {
                roomNameTemplate = ChatUIHelper.GetWebpartTransformation(ChatRoomNameTransformationName,"chat.error.transformation.namewp.error"),
                contentClientID = "#" + pnlChatRoomName.ClientID,
                clientID = ClientID,
                conversationTitle = ResHelper.GetString("chat.title.privateconversation"),
                groupID = GroupID,
                displayInitialTitle = DisplayInitialTitle,
                noRoomTitle = InitialTitle,
                loadingDiv = ChatUIHelper.GetWebpartLoadingDiv("ChatRoomNameWPLoading", "chat.wploading.roomname"),
                envelopeID = "#envelope_" + ClientID
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );
        string startupScript = String.Format("InitChatRoomNameWebpart({0});", json);

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatRoomName_" + ClientID, startupScript, true);
    }
}
