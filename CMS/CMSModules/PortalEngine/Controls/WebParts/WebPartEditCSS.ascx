<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="WebPartEditCSS.ascx.cs"
    Inherits="CMSModules_PortalEngine_Controls_WebParts_WebPartEditCSS" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
    <ContentTemplate>
        <asp:Button CssClass="HiddenButton" runat="server" ID="btnAction" OnClick="btnAction_Clicked" />
        <div runat="server" id="pnlMenu" class="PreviewMenu">
            <div class="cms-edit-menu">
                <cms:HeaderActions runat="server" ID="headerActions" IsLiveSite="false" PerformFullPostBack="false" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Panel ID="pnlContent" runat="server" CssClass="PreviewBody">
    <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false"
        OffsetY="10" OffsetX="15" />
    <div class="PageContent">
        <div style="width: 100%;">
            <cms:LocalizedLabel runat="server" ID="lblCss" ResourceString="Container_Edit.ContainerCSS"
                DisplayColon="true" EnableViewState="false" CssClass="Hidden control-label" />
            <cms:ExtendedTextArea ID="etaCSS" runat="server" EnableViewState="true" EditorMode="Advanced"
                Language="CSS" Width="98%" Height="500px" CssClass="form-control" />
        </div>
    </div>
</asp:Panel>
