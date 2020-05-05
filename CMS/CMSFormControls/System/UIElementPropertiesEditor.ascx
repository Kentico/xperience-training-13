<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="UIElementPropertiesEditor.ascx.cs"
    Inherits="CMSFormControls_System_UIElementPropertiesEditor" %>
<cms:CMSUpdatePanel runat="server" UpdateMode="Always" ID="pnlUpdate">
    <ContentTemplate>
        <asp:Table runat="server" ID="tblData" CellPadding="-1" CellSpacing="-1" CssClass="table table-hover" EnableViewState="false" />
        <asp:Label runat="server" ID="lblError" CssClass="EditingFormErrorLabel" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
