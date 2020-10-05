<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Urls.aspx.cs" Inherits="CMSModules_Content_CMSDesk_Properties_Urls"
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
    <asp:Panel ID="pnlUrl" runat="server" DefaultButton="btnEnterPress">
        <cms:LocalizedHeading runat="server" ID="headUrl" Level="4" ResourceString="content.ui.urltitle" EnableViewState="false" />
        <asp:Panel ID="pnlPageUrlPath" runat="server" CssClass="form-horizontal url-form url-slug-form">
            <div class="form-group">
                <div class="editing-form-label-cell cms-pageurlpath-slug-label">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSlug" runat="server" EnableViewState="false"
                        ResourceString="content.ui.urllabel" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell-wide">
                    <cms:TextBoxWithPlaceholder ID="txtSlug" runat="server"/>
                    <cms:LocalizedLinkButton runat="server" ID="btnDisplaySlugs" ResourceString="content.ui.properties.displayslugs" CssClass="cms-pageurlpath-displayall" EnableViewState="false" />
                    <asp:Button CssClass="HiddenButton" ID="btnEnterPress" runat="server" OnClick="btnSave_Click" EnableViewState="false"/>
                </div>
            </div>
        </asp:Panel>  

        <asp:Panel runat="server" ID="pnlUrlLinks" CssClass="form-horizontal url-form url-links-form">
            <asp:PlaceHolder runat="server" ID="plcLive" Visible="false">
                <div class="form-group url-form">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblLiveURLTitle" runat="server" ResourceString="content.ui.properties.liveurl" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <a id="lnkLiveURL" runat="server" target="_blank" class="form-control-text" enableviewstate="false"></a>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcPreview" runat="server" Visible="false">
                <div class="form-group url-form">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblPreviewURLTitle" runat="server" ResourceString="content.ui.properties.previewurl" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSUpdatePanel ID="pnlUpdatePreviewUrl" runat="server" UpdateMode="conditional">
                            <ContentTemplate>
                                <a id="lnkPreviewURL" runat="server" target="_blank" class="form-control-text"><%= ResHelper.GetString("content.ui.properties.showpreview") %></a>
                                <cms:CMSAccessibleButton runat="server" ID="btnResetPreviewGuid" EnableViewState="false" IconOnly="True" IconCssClass="icon-rotate-right" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </div>
            </asp:PlaceHolder>
            <cms:LocalizedLiteral runat="server" ID="litMissingPattern" ResourceString="content.ui.properties.missingurlpattern" Visible="false"></cms:LocalizedLiteral>
        </asp:Panel>
        
    </asp:Panel>
    <asp:Panel ID="pnlAlternativeUrls" runat="server">
        <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="content.ui.propertiesalternativeurls" EnableViewState="false" />
        <div class="ClassFieldsButtonPanel">
            <cms:LocalizedButton ID="btnAddAlternativeUrl" runat="server" ResourceString="content.ui.properties.addalternativeurl" EnableViewState="false" ButtonStyle="Default" UseSubmitBehavior="false"/>
        </div>
        <div class="cms-urlpath-listing">
            <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
                <ContentTemplate>
                    <cms:UniGrid ID="gridUrls" ShortID="g" runat="server" ObjectType="cms.alternativeurl" IsLiveSite="false" Columns="AlternativeUrlID, AlternativeUrlUrl"
                        OrderBy="AlternativeUrlUrl" ShowExportMenu="true" ShowObjectMenu="false" OnOnAction="gridUrls_OnAction" OnOnExternalDataBound="gridUrls_OnExternalDataBound">
                        <GridActions>
                            <ug:Action Name="edit" Caption="$general.edit$" FontIconClass="icon-edit" FontIconStyle="Allow" ExternalSourceName="edit" />
                            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$general.confirmdelete$" />
                            <ug:Action Name="openurl" Caption="$alternativeurl.openurl$" FontIconClass="icon-eye" FontIconStyle="Allow" ExternalSourceName="openurl" />
                        </GridActions>
                        <GridColumns>
                            <ug:Column Source="##ALL##" Caption="$general.url$" ExternalSourceName="url" Wrap="false" MaxLength="150" Width="100%" CssClass="main-column-100 cms-urlpath-column" />
                        </GridColumns>
                        <GridOptions DisplayFilter="false" AllowSorting="false" />
                    </cms:UniGrid>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </div>
    </asp:Panel>

</asp:Content>
