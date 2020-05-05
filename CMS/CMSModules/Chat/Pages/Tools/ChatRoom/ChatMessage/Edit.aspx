<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Chat message properties" Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_ChatMessage_Edit" Theme="Default"  Codebehind="Edit.aspx.cs" %>
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatMessage/Edit.ascx"
    TagName="ChatMessageEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatMessageEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>