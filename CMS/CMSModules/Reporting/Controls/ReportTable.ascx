<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Controls_ReportTable"
     Codebehind="ReportTable.ascx.cs" %>
<cms:contextmenucontainer runat="server" id="menuCont" MenuID="">
    <asp:PlaceHolder runat="server" ID="plcGrid" />
    <asp:Label runat="server" ID="lblError" EnableViewState="false" CssClass="InlineControlError"
        Visible="false" />
    <asp:Label runat="server" ID="lblInfo" EnableViewState="False" Visible="False" />
</cms:contextmenucontainer>
<asp:Literal runat="server" ID="ltlEmail" Visible="false" EnableViewState="false" />