<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="HtmlBarGraph.ascx.cs"
    Inherits="CMSModules_Reporting_Controls_HtmlBarGraph" %>
<asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
    Visible="false" />
<cms:ContextMenuContainer runat="server" ID="menuCont" MenuID="">
    <asp:Label runat="server" ID="lblInfo" EnableViewState="False" Visible="False" />
    <div>
        <asp:Literal runat="server" ID="ltlGraph" EnableViewState="false" />
    </div>
</cms:ContextMenuContainer>
<asp:Literal runat="server" ID="ltlEmail" EnableViewState="false" Visible="false" />