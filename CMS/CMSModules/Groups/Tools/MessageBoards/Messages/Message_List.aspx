<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_MessageBoards_Messages_Message_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" EnableEventValidation="false"  Codebehind="Message_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Messages/MessageList.ascx" TagName="MessageList" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MessageList ID="messageList" runat="server" IsLiveSite="false"  />
</asp:Content>
