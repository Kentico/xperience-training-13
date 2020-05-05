<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="List.ascx.cs" Inherits="CMSModules_Chat_Controls_UI_ChatSupportCannedResponse_List" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:UniGrid runat="server" ID="gridElem" ObjectType="chat.supportcannedresponse" OrderBy="ChatSupportCannedResponseID" Columns="ChatSupportCannedResponseID,ChatSupportCannedResponseTagName,ChatSupportCannedResponseText,ChatSupportCannedResponseSiteID"
    IsLiveSite="false" RememberStateByParam="">
    <GridActions Parameters="ChatSupportCannedResponseID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" ExternalSourceName="delete" Confirmation="$General.ConfirmDelete$" /> <%-- Permissions are checked in OnAction handler --%>
    </GridActions>
    <GridColumns>
        <ug:Column Source="ChatSupportCannedResponseTagName" Caption="$chat.cannedresponse.tagname$" Wrap="false" />
        <ug:Column Source="ChatSupportCannedResponseText" Caption="$chat.cannedresponse.replacement$" Wrap="true" CssClass="main-column-100" />
    </GridColumns>
</cms:UniGrid>

<asp:Button ID="btnHiddenPostBackButton" runat="server" CssClass="HiddenButton" />