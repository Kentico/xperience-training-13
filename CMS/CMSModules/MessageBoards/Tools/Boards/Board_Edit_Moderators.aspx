<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MessageBoards_Tools_Boards_Board_Edit_Moderators" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="default" Title="Message board - moderators"  Codebehind="Board_Edit_Moderators.aspx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardModerators.ascx"
    TagName="BoardModerators" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:BoardModerators ID="boardModerators" runat="server" />
</asp:Content>
