<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Controls_ReportValue"
     Codebehind="ReportValue.ascx.cs" %>
<cms:ContextMenuContainer runat="server" ID="menuCont" MenuID="">
    <asp:Literal ID="lblValue" runat="server" />
    <asp:Label runat="server" ID="lblError" EnableViewState="false" CssClass="InlineControlError"
        Visible="false" />
</cms:ContextMenuContainer>
<asp:Literal runat="server" ID="ltlEmail" Visible="false" EnableViewState="false" />
