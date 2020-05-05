<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DocumentList.ascx.cs"
    Inherits="CMSModules_Content_Controls_DocumentList" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Controls/MassActions.ascx" TagName="MassActions" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlGrid" runat="server">
    <ContentTemplate>
        <div class="Listing">
            <cms:UniGrid ID="gridDocuments" runat="server" ShortID="g" GridName="Listing.xml"
                EnableViewState="true" DelayedReload="true" IsLiveSite="false" ExportFileName="cms_document" />
        </div>
        <cms:MassActions ID="ctrlMassActions" runat="server" />
        <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
        <asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hdnMoveId" runat="server" />
    </ContentTemplate>
</cms:CMSUpdatePanel>