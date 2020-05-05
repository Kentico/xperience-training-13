<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Controls_ReportGraph"
     Codebehind="ReportGraph.ascx.cs" %>
<div id="graphDiv" runat="server" class="ReportGraphDiv">
    <cms:ContextMenuContainer runat="server" ID="menuCont" MenuID="">
        <asp:Label runat="server" ID="lblInfo" EnableViewState="False" Visible="False" />
        <asp:Chart runat="server" ID="ucChart" EnableViewState="false" />
        <asp:HiddenField ID="hdnValues" runat="server" EnableViewState="true" />
        <asp:Button runat="server" ID="btnRefresh" CssClass="HiddenButton" />
        <asp:Image runat="server" ID="imgGraph" AlternateText="Report graph" EnableViewState="false"
            Visible="false" />
        <asp:Label runat="server" ID="lblError" EnableViewState="false" CssClass="InlineControlError"
            Visible="false" />
    </cms:ContextMenuContainer>
</div>
<asp:PlaceHolder runat="server" ID="pnlImage" Visible="false">
    <asp:Literal runat="server" ID="ltlEmail" />
</asp:PlaceHolder>
