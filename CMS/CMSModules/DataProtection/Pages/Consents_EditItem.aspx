<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Consents_EditItem.aspx.cs" Inherits="CMSModules_DataProtection_Pages_Consents_EditItem" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>

<asp:Content ID="cntAdditionalActions" runat="server" ContentPlaceHolderID="plcSiteSelector" EnableViewState="false" >
    <div class="form-horizontal form-filter consent-edit-culture-selector">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblCultureSelector" runat="server" ResourceString="dataprotection.consents.consentlanguage" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SiteCultureSelector runat="server" ID="cultureSelector" AllowDefault="false" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent" EnableViewState="false">
    <asp:UpdatePanel ID="pnlUpdBody" runat="server">
        <ContentTemplate>
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedHeading runat="server" ID="headConsentShortText" Level="4" ResourceString="dataprotection.consents.consenttext.shorttext" DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="textarea-full-width editing-form-value-cell">
                        <cms:CMSHtmlEditor ID="htmlConsentShortText" runat="server" ToolbarSet="Consents_ShortText" Height="170px" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedHeading runat="server" ID="headConsentFullText" Level="4" ResourceString="dataprotection.consents.consenttext.fulltext" DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="textarea-full-width editing-form-value-cell">
                        <cms:CMSHtmlEditor ID="htmlConsentFullText" runat="server" ToolbarSet="Consents_FullText" Height="400px" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
