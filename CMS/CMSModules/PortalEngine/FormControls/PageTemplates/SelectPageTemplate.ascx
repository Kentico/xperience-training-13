<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_FormControls_PageTemplates_SelectPageTemplate"
     Codebehind="SelectPageTemplate.ascx.cs" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="control-group-inline">
            <cms:CMSTextBox ID="txtTemplate" runat="server" MaxLength="200" ReadOnly="true" /><cms:LocalizedButton
                ID="btnSelect" runat="server" ButtonStyle="Default" OnClick="btnSelect_Click"
                EnableViewState="false" ResourceString="general.select" /><cms:LocalizedButton ID="btnClear" runat="server" ButtonStyle="Default"
                    OnClick="btnClear_clicked" EnableViewState="false" RenderScript="true" ResourceString="general.clear" />
        </div>
        <asp:HiddenField ID="hdnSelected" runat="server" />
        <asp:PlaceHolder runat="server" ID="pnlButtons">
            <div class="btns-vertical btns-vertical-page-template">
                <cms:LocalizedButton ID="btnEditTemplateProperties" runat="server" ButtonStyle="Default" ResourceString="PageProperties.EditTemplateProperties" EnableViewState="false" ToolTipResourceString="pagetemplateselector.edit" />
            </div>
        </asp:PlaceHolder>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:HiddenField runat="server" ID="hdnTemplateChanged" />
