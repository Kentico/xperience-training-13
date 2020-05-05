<%@ Page Language="C#" AutoEventWireup="false" Codebehind="Urls_UrlPathCulturePaths.aspx.cs" Inherits="CMSModules_Content_CMSDesk_Properties_Urls_UrlPathCulturePaths"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" MaintainScrollPositionOnPostback="true" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server">
        <div class="header-actions-container">
            <asp:Panel ID="pnlMenu" runat="server" CssClass="header-actions-main">
                <cms:HeaderActions ID="menu" runat="server" />
            </asp:Panel>
        </div>
        <div class="Clear">
        </div>
    </cms:CMSPanel>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <div class="cms-urlpath-listing">
        <h4><cms:LocalizedLiteral runat="server" ResourceString="content.ui.properties.pageurlpaths.heading" /></h4>
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:UniGrid ID="gridUrlsPaths" ShortID="g" runat="server" ObjectType="cms.pageurlpath" IsLiveSite="false" Columns="PageUrlPathID, PageUrlPathCulture, PageUrlPathUrlPath, PageUrlPathSiteID"
                    OrderBy="PageUrlPathCulture" ShowExportMenu="true" ShowObjectMenu="false" OnOnExternalDataBound="gridUrlPaths_OnExternalDataBound" OnOnAction="gridUrlPaths_OnAction">
                    <GridActions>
                        <ug:Action Name="edit" Caption="$general.edit$" FontIconClass="icon-edit" FontIconStyle="Allow" ExternalSourceName="edit" />
                        <ug:Action Name="openlivesite" FontIconClass="icon-eye" FontIconStyle="Allow" ExternalSourceName="openlivesite" />
                    </GridActions>
                    <GridColumns>
                        <ug:Column Source="PageUrlPathCulture" Caption="$general.language$" ExternalSourceName="#culturenamewithflag" Wrap="false" />
                        <ug:Column Source="##ALL##" Caption="$general.url$" ExternalSourceName="urlpath" Wrap="false" CssClass="main-column-100 cms-urlpath-column"  />
                    </GridColumns>
                    <GridOptions DisplayFilter="false" AllowSorting="false" />
                </cms:UniGrid>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</asp:Content>
