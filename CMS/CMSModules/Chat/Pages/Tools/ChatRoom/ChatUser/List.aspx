<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Chat user list"
    Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_ChatUser_List" Theme="Default"  Codebehind="List.aspx.cs" %>
    
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatRoomUser/List.ascx" TagName="ChatRoomUserList" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatRoomUserList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
