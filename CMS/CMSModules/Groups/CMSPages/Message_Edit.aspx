<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_CMSPages_Message_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalSimplePage.master" Title="Message - Edit"  Codebehind="Message_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Messages/MessageEdit.ascx" TagName="MessageEdit"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MessageEdit ID="messageEditElem" runat="server" IsLiveSite="true" ModalMode="true" />    
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
