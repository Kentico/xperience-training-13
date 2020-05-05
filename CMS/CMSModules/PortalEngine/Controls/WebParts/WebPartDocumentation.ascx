<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="WebPartDocumentation.ascx.cs"
    Inherits="CMSModules_PortalEngine_Controls_WebParts_WebPartDocumentation" %>
<div class="WebpartTabsPageHeader LightTabs">
    <cms:UITabs ID="tabControlElem" runat="server" OnOnTabClicked="tabControlElem_clicked" />
    <div class="HeaderSeparatorEnvelope">
        <div class="HeaderSeparator">
            &nbsp;
        </div>
    </div>
</div>
<div class="DocumentationScrollableDiv" id="divScrolable" runat="server">
    <asp:Panel runat="server" ID="pnlDoc" Visible="true">
        <div class="PageContent">
            <!-- Teaser + description -->
            <table class="DocumentationWebPartsDescription" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="DocumentationWebPartColumn">
                        <asp:Literal ID="ltrImage" runat="server"></asp:Literal>
                    </td>
                    <td class="DocumentationWebPartColumnNoLine">
                        <asp:Literal runat="server" ID="ltlDescription" />
                    </td>
                </tr>
                <!-- Documentation -->
                <tr>
                    <td colspan="2">
                        <div class="DocumentationAdditionalText">
                            <asp:Literal runat="server" ID="ltlContent" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlProperties" Visible="false" EnableViewState="false">
        <!-- web part properties -->
        <div class="DocumentationWebPartsProperties">
            <asp:Literal runat="server" ID="ltlProperties" />
        </div>
    </asp:Panel>
</div>
