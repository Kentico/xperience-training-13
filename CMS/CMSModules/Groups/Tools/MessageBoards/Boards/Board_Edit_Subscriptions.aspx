<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_MessageBoards_Boards_Board_Edit_Subscriptions"
    Title="Board Subscriptions" Theme="Default"  Codebehind="Board_Edit_Subscriptions.aspx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardSubscriptions.ascx"
    TagName="BoardSubscriptions" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:BoardSubscriptions ID="boardSubscriptions" runat="server" />
</asp:Content>
