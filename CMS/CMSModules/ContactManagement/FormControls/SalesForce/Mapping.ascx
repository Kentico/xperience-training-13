<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Mapping.ascx.cs" Inherits="CMSModules_ContactManagement_FormControls_SalesForce_Mapping" %>
<%@ Register TagPrefix="cms" TagName="Mapping" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Mapping.ascx" %>
<%@ Register TagPrefix="cms" TagName="SalesForceError" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Error.ascx" %>
<cms:CMSUpdatePanel ID="MainUpdatePanel" runat="server">
<ContentTemplate>
    <asp:HiddenField ID="MappingHiddenField" runat="server" EnableViewState="false" />
    <cms:SalesForceError ID="SalesForceError" runat="server" EnableViewState="false" />
    <asp:Panel ID="MappingPanel" runat="server" EnableViewState="false">
        <cms:Mapping ID="MappingControl" runat="server"></cms:Mapping>
    </asp:Panel>
    <p id="MessageLabel" runat="server" enableviewstate="false" visible="false"></p>
    <cms:LocalizedButton ID="EditMappingButton" runat="server" EnableViewState="false" ResourceString="general.edit" ButtonStyle="Default"></cms:LocalizedButton>
</ContentTemplate>
</cms:CMSUpdatePanel>