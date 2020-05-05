<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Chat message properties" Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_ChatUser_Edit"
    Theme="Default"  Codebehind="Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatRoomUser/Edit.ascx" TagName="ChatRoomUserEdit" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatRoomUserEdit ID="editElem" runat="server" />
</asp:Content>
