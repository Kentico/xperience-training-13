<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocTypeSelection.ascx.cs"
    Inherits="CMSModules_Content_Controls_DocTypeSelection" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:PlaceHolder runat="server" ID="plcDocumentTypeSelection">
    <div class="ContentNewClasses">
        <cms:LocalizedHeading runat="server" ID="mainCaptionDocumentTypeSelection" EnableViewState="false" />
        <cms:LocalizedHeading runat="server" ID="headDocumentTypeSelection" EnableViewState="false" />
        <asp:Label ID="lblError" runat="server" CssClass="ContentLabel" ForeColor="Red" EnableViewState="false" />
    </div>

    <div class="new-page-container">
        <div class="ContentNewClasses UniGridClearPager content-block-50 page-type-selection-grid">
            <cms:UniGrid runat="server" ID="gridClasses" IsLiveSite="false" ZeroRowsText="" ShowActionsMenu="false">
                <GridColumns>
                    <ug:Column Source="##ALL##" ExternalSourceName="classname" Caption="$general.codename$"
                        Wrap="false" />
                    <ug:Column Source="ClassDisplayName" Caption="$documenttype.name$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                </GridColumns>
                <GridOptions DisplayFilter="true" />
                <PagerConfig DefaultPageSize="25" ShowPageSize="false" />
            </cms:UniGrid>
        </div>

        <cms:UIPlaceHolder runat="server" ID="plcNewLinkNew" ElementName="New" ModuleName="CMS.Content">
            <cms:UIPlaceHolder runat="server" ID="plcNewLink" ElementName="New.LinkExistingDocument" ModuleName="CMS.Content">
                <asp:Panel runat="server" ID="pnlVerticalSeparator" CssClass="vertical-divider">
                    <div class="vertical-line"></div>
                    <h4 class="label-or">or</h4>
                    <div class="vertical-line"></div>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlFooter" CssClass="content-link-panel">
                    <asp:HyperLink runat="server" ID="lnkNewLink" CssClass="content-new-link cms-icon-link" EnableViewState="false">
                        <cms:CMSIcon runat="server" ID="iconNewLink" EnableViewState="false" CssClass="icon-chain cms-icon-150 content-new-link-icon" /><br />
                        <asp:Label ID="lblNewLink" runat="server" EnableViewState="false" CssClass="content-new-link-label" />
                    </asp:HyperLink>
                </asp:Panel>
            </cms:UIPlaceHolder>
        </cms:UIPlaceHolder>
    </div>
</asp:PlaceHolder>