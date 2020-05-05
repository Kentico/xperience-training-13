<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSFormControls/System/CacheItems.ascx.cs" Inherits="CMSFormControls_System_CacheItems" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:CMSPanel runat="server" ID="pnlItems">
            <cms:CMSTextBox ID="txtItems" runat="server" Enabled="false" ReadOnly="true" />
            <cms:LocalizedButton ID="btnSelect" runat="server" ButtonStyle="Default" ResourceString="general.edit" />
        </cms:CMSPanel>
        <cms:CMSPanel runat="server" ID="pnlList" Visible="false">
            <asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel" Visible="false" />
            <cms:CMSCheckBoxList runat="server" ID="chkList" RepeatDirection="Vertical" />
        </cms:CMSPanel>
    </ContentTemplate>
</cms:CMSUpdatePanel>

