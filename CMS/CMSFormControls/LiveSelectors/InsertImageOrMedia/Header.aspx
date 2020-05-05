<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/TabsHeader.master" Theme="Default"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSFormControls_LiveSelectors_InsertImageOrMedia_Header"  Codebehind="Header.aspx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/DialogHeader.ascx" TagName="DialogHeader" TagPrefix="cms" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <cms:DialogHeader ID="header" runat="server" IsLiveSite="true" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>

