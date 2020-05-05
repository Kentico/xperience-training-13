<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Tools_Boards_Board_Edit_General"
    Title="Message board- Edit- General" Theme="Default"  Codebehind="Board_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardEdit.ascx" TagName="BoardEdit"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:BoardEdit ID="boardEdit" IsLiveSite="false" runat="server" />
</asp:Content>
