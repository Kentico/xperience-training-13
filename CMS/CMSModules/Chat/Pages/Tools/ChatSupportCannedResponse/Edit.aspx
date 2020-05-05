<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Edit.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Chat support canned response properties" Inherits="CMSModules_Chat_Pages_Tools_ChatSupportCannedResponse_Edit" Theme="Default" %>
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatSupportCannedResponse/Edit.ascx"
    TagName="ChatSupportCannedResponseEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatSupportCannedResponseEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>