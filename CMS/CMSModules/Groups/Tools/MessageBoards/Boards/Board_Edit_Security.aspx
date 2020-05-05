<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Tools_MessageBoards_Boards_Board_Edit_Security" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="default" Title="Message board - security"  Codebehind="Board_Edit_Security.aspx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardSecurity.ascx" TagName="BoardSecurity" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:BoardSecurity ID="boardSecurity" runat="server" IsLiveSite="false" />
</asp:Content>
