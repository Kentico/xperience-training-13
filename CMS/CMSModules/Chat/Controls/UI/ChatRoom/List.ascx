<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Chat_Controls_UI_ChatRoom_List"  Codebehind="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:UniGrid runat="server" ID="gridElem" ObjectType="chat.room" OrderBy="ChatRoomDisplayName"
    Columns="ChatRoomID,ChatRoomPrivate,ChatRoomDisplayName,ChatRoomEnabled,ChatRoomIsSupport,ChatRoomCreatedWhen,ChatRoomCreatedByChatUserID,ChatRoomDescription,ChatRoomSiteID" 
    IsLiveSite="false" EditActionUrl="Frameset.aspx?roomId={0}" RememberStateByParam="">
    <GridActions Parameters="ChatRoomID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="approve" ExternalSourceName="approve" Caption="$General.Enable$" FontIconClass="icon-check-circle" FontIconStyle="Allow" />
        <ug:Action Name="safedelete" ExternalSourceName="safedelete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="ChatRoomDisplayName" Caption="$general.displayname$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="ChatRoomEnabled" Caption="$chat.enabled$" Wrap="false" ExternalSourceName="#yesno">
            <Filter Type="Bool" />
        </ug:Column>
        <ug:Column Source="ChatRoomPrivate" Caption="$chat.private$" Wrap="false" ExternalSourceName="#yesno">
            <Filter Type="Bool" />
        </ug:Column>
        <ug:Column Source="ChatRoomIsSupport" Caption="$chat.issupported$" ExternalSourceName="#yesno">
            <Filter Type="Bool" />
        </ug:Column>
        <ug:Column Source="ChatRoomCreatedWhen" Caption="$chat.createdwhen$" Wrap="false" />
        <ug:Column Source="ChatRoomCreatedByChatUserID" Caption="$chat.creator$" Wrap="false" ExternalSourceName="ChatRoomCreatedByChatUserID">
        </ug:Column>
        <ug:Column Source="ChatRoomDescription" Caption="$general.description$" Wrap="false" MaxLength="96" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>

<asp:Button ID="btnHiddenPostBackButton" runat="server" CssClass="HiddenButton" />