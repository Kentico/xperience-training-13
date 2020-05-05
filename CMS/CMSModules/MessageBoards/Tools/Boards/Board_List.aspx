<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Tools_Boards_Board_List"
    Title="Message boards - List" Theme="Default"  Codebehind="Board_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardList.ascx" TagName="BoardList"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:BoardList ID="boardList" runat="server" IsLiveSite="false" />
</asp:Content>
