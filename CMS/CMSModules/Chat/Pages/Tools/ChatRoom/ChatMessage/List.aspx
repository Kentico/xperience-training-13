<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Chat message list"
    Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_ChatMessage_List" Theme="Default"  Codebehind="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatMessage/List.ascx" TagName="ChatMessageList" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatMessageList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
