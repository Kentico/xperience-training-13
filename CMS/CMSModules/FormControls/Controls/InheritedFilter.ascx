<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="InheritedFilter.ascx.cs"
    Inherits="CMSModules_FormControls_Controls_InheritedFilter" %>

<asp:Panel CssClass="Filter" runat="server" ID="pnlSearch">
    <cms:CMSDropDownList runat="server" ID="drpDownList" UseResourceStrings="true" CssClass="DropDownField">
        <asp:ListItem Text="general.selectall" Value="all" />
        <asp:ListItem Text="general.yes" Value="yes" />
        <asp:ListItem Text="general.no" Value="no" />
    </cms:CMSDropDownList>
</asp:Panel>
