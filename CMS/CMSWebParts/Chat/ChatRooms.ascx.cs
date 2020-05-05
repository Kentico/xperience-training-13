using System;

using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSWebParts_Chat_ChatRooms : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Gets or sets ChatRoomsTransformation property.
    /// </summary>
    public string ListItemTransformation
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatRoomsTransformation"), ChatSettingsProvider.TransformationRooms);
        }
        set
        {
            SetValue("ChatRoomsTransformation", value);
        }
    }


    /// <summary>
    /// Gets or sets GroupID property.
    /// </summary>
    public string GroupID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupID"), "DefaultGroup");
        }
        set
        {
            SetValue("GroupID", value);
        }
    }


    /// <summary>
    /// Gets or sets RedirectURL property.
    /// </summary>
    public string RedirectURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectURL"), "");
        }
        set
        {
            SetValue("RedirectURL", value);
        }
    }


    /// <summary>
    /// Gets or sets RedirectGroup property.
    /// </summary>
    public string RedirectGroup
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectGroup"), "");
        }
        set
        {
            SetValue("RedirectGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets EnablePaging property.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), false);
        }
        set
        {
            SetValue("EnablePaging", value);
        }
    }


    /// <summary>
    /// Gets or sets EnableFiltering property.
    /// </summary>
    public bool EnableFiltering
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableFiltering"), false);
        }
        set
        {
            SetValue("EnableFiltering", value);
        }
    }


    /// <summary>
    /// Gets or sets ShowFilterItems property.
    /// </summary>
    public int ShowFilterItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ShowFilterItems"), -1);
        }
        set
        {
            SetValue("ShowFilterItems", value);
        }
    }


    /// <summary>
    /// Gets or sets PagingItems property.
    /// </summary>
    public int PagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PagingItems"), -1);
        }
        set
        {
            SetValue("PagingItems", value);
        }
    }


    /// <summary>
    /// Gets or sets GroupPagesBy property.
    /// </summary>
    public int GroupPagesBy
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("GroupPagesBy"), -1);
        }
        set
        {
            SetValue("GroupPagesBy", value);
        }
    }


    /// <summary>
    /// Gets or sets EnablePopup property.
    /// </summary>
    public bool EnablePopup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePopup"), false);
        }
        set
        {
            SetValue("EnablePopup", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatMessageTransformationName property.
    /// </summary>
    public string ChatMessageTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatMessageTransformationName"), ChatSettingsProvider.TransformationRoomMessages);
        }
        set
        {
            SetValue("ChatMessageTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatErrorTransformationName property.
    /// </summary>
    public string ChatErrorTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatErrorTransformationName"), ChatSettingsProvider.TransformationErrors);
        }
        set
        {
            SetValue("ChatErrorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatErrorDeleteAllButtonTransformationName property.
    /// </summary>
    public string ChatErrorDeleteAllButtonTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatErrorDeleteAllButtonTransformationName"), ChatSettingsProvider.TransformationErrorsDeleteAll);
        }
        set
        {
            SetValue("ChatErrorDeleteAllButtonTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatRoomUserTransformationName property.
    /// </summary>
    public string ChatRoomUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatRoomUserTransformationName"), ChatSettingsProvider.TransformationRoomUsers);
        }
        set
        {
            SetValue("ChatRoomUserTransformationName", value);
        }
    }


    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }

    #endregion


    #region "Page events"


    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatUIHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeRooms", this, InnerContainerTitle, InnerContainerName);
        ChatCssHelper.RegisterStylesheet(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatScriptHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Insert script references
        ChatScriptHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatRooms_files/ChatRooms.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/ListPaging.js");

        if (EnableFiltering)
        {
            pnlChatRoomsFiltering.Visible = true;
        }
        // Prepare and run startup script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatRooms_" + ClientID, BuildStartupScript(), true);

        imgChatRoomsPrompt.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/lock24.png");
        imgChatRoomsCreatePrompt.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/message_add24.png");
        imgChatRoomsCreateError.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/error16.png");
        imgChatRoomsDeletePrompt.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/sign_forbidden24.png");
        imgChatRoomsAbandonPrompt.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/sign_forbidden24.png");
        imgChatRoomsEditPrompt.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/message_edit24.png");
        imgChatRoomsEditError.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/error16.png");
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Builds startup script.
    /// </summary>
    private string BuildStartupScript()
    {
        string redirectURL = RedirectURL.Length > 0 ? RedirectURL : ChatSettingsProvider.RedirectURLJoinSetting;

        // Set all the transformation settings for chat room window
        int roomSettingsId = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);

        string json = JsonConvert.SerializeObject(
            new
            {
                guid = roomSettingsId,
                clientID = ClientID,
                groupID = GroupID,
                pnlChatRoomsList = pnlChatRoomsList.ClientID,
                pnlChatRoomsPrompt = pnlChatRoomsPrompt.ClientID,
                btnChatRoomsPromptSubmit = btnChatRoomsPromptSubmit.ClientID,
                pnlChatRoomsCreatePrompt = pnlChatRoomsCreatePrompt.ClientID,
                txtChatRoomsCreateName = txtChatRoomsCreateName.ClientID,
                chkChatRoomsCreateIsPrivate = chkChatRoomsCreateIsPrivate.ClientID,
                txtChatRoomsCreatePassword =  txtChatRoomsCreatePassword.ClientID,
                chkChatRoomsCreateAllowAnonym =  chkChatRoomsCreateAllowAnonym.ClientID,
                btnChatRoomsCreatePromptSubmit = btnChatRoomsCreatePromptSubmit.ClientID,
                lblChatRoomsCreateError = lblChatRoomsCreateError.ClientID,
                txtChatRoomsPromptInput = txtChatRoomsPromptInput.ClientID,
                btnChatRoomsCreateRoom = btnChatRoomsCreateRoom.ClientID,
                templateListItem = ChatUIHelper.GetWebpartTransformation(ListItemTransformation, "chat.error.transformation.rooms"),
                loadingDiv = ChatUIHelper.GetWebpartLoadingDiv("ChatRoomsWPLoading", "chat.wploading.rooms"),
                txtChatRoomsCreateDescription = txtChatRoomsCreateDescription.ClientID,
                btnChatRoomsPromptClose = btnChatRoomsPromptClose.ClientID,
                btnChatRoomsCreatePromptClose = btnChatRoomsCreatePromptClose.ClientID,
                btnChatRoomsDeletePromptSubmit = btnChatRoomsDeletePromptSubmit.ClientID,
                btnChatRoomsDeletePromptClose = btnChatRoomsDeletePromptClose.ClientID,
                pnlChatRoomsDeletePrompt = pnlChatRoomsDeletePrompt.ClientID,
                btnChatRoomsAbandonPromptSubmit = btnChatRoomsAbandonPromptSubmit.ClientID,
                btnChatRoomsAbandonPromptClose = btnChatRoomsAbandonPromptClose.ClientID,
                pnlChatRoomsAbandonPrompt = pnlChatRoomsAbandonPrompt.ClientID,
                pnlChatRoomsEditPrompt = pnlChatRoomsEditPrompt.ClientID,
                txtChatRoomsEditName = txtChatRoomsEditName.ClientID,
                txtChatRoomsEditDescription = txtChatRoomsEditDescription.ClientID,
                chkChatRoomsEditIsPrivate = chkChatRoomsEditIsPrivate.ClientID,
                txtChatRoomsEditPassword = txtChatRoomsEditPassword.ClientID,
                chkChatRoomsEditAllowAnonym = chkChatRoomsEditAllowAnonym.ClientID,
                btnChatRoomsEditPromptSubmit = btnChatRoomsEditPromptSubmit.ClientID,
                btnChatRoomsEditPromptClose = btnChatRoomsEditPromptClose.ClientID,
                txtChatRoomsEditPasswordConfirm = txtChatRoomsEditPasswordConfirm.ClientID,
                lblChatRoomsEditError = lblChatRoomsEditError.ClientID,
                passwordTxt = ResHelper.GetString("chat.password") + ":",
                passwordNewTxt = ResHelper.GetString("chat.passwordnew") + ":",
                lblChatRoomsEditPasswordNew = lblChatRoomsEditPasswordNew.ClientID,
                txtChatRoomsCreatePasswordConfirm = txtChatRoomsCreatePasswordConfirm.ClientID,
                pnlChatRoomsEditError = pnlChatRoomsEditError.ClientID,
                chkChatRoomsHasPassword = chkChatRoomsHasPassword.ClientID,
                pnlChatRoomsCreateError = pnlChatRoomsCreateError.ClientID,
                pnlFilterClientID = pnlChatRoomsFiltering.ClientID,
                pnlPagingClientID = pnlChatRoomsPaging.ClientID,
                pagingItems = PagingItems > 0 ? PagingItems : ChatSettingsProvider.WPPagingItems,
                groupPagesBy = GroupPagesBy >= 0 ? GroupPagesBy : ChatSettingsProvider.WPGroupPagesBy,
                pagingEnabled = EnablePaging,
                btnFilter = btnChatRoomsFilter.ClientID,
                txtFilter = txtChatRoomsFilter.ClientID,
                filterEnabled = EnableFiltering,
                pnlInfo = pnlChatRoomsInfo.ClientID,
                resStrNoFound = ResHelper.GetString("chat.rooms.notfound"),
                resStrResults = ResHelper.GetString("chat.rooms.results"),
                resStrTooltipPopup = ResHelper.GetString("chat.enterwithpopup"),
                filterCount = ShowFilterItems >= 0 ? ShowFilterItems : ChatSettingsProvider.WPShowFilterLimit,
                passwordPromptError = pnlChatRoomsPromptPasswordError.ClientID,
                redirectURL = ChatHelper.GetDocumentAbsoluteUrl(redirectURL),
                redirectGroup = RedirectGroup,
                envelopeID = "envelope_" + ClientID,
                pnlErrorCreate = pnlChatRoomsCreateErrorConfirm.ClientID,
                pnlErrorEdit = pnlChatRoomsEditErrorConfirm.ClientID,
                enablePopup = EnablePopup,
                passwordNotMatchStr = ResHelper.GetString("administration-user_edit_password.passwordsdonotmatch")
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );
        return String.Format("InitChatRooms({0})", json);
    }

    #endregion
}
