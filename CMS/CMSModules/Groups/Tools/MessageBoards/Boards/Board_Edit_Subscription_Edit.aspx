<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_MessageBoards_Boards_Board_Edit_Subscription_Edit"
    Title="Board - Subscription - Edit" Theme="Default"  Codebehind="Board_Edit_Subscription_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardSubscription.ascx"
    TagName="BoardSubscription" TagPrefix="cms" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:BoardSubscription ID="boardSubscription" runat="server" />
</asp:Content>
