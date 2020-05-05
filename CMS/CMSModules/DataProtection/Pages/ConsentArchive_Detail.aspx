<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConsentArchive_Detail.aspx.cs" Inherits="CMSModules_DataProtection_Pages_ConsentArchive_Detail" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>

<asp:Content ID="cntAdditionalActions" runat="server" ContentPlaceHolderID="plcSiteSelector" EnableViewState="false">
    <div class="form-horizontal form-filter">
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

<asp:Content ID="content" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="updatePanel" runat="server" EnableViewState="false" UpdateMode="Always">
        <ContentTemplate>
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedHeading runat="server" ID="headConsentShortText" Level="4" ResourceString="dataprotection.consents.consentarchive.consenttext.shorttext" DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="textarea-full-width editing-form-value-cell">
                        <cms:CMSHtmlEditor ID="htmlConsentShortText" runat="server" Height="170px" Enabled="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedHeading runat="server" ID="headConsentFullText" Level="4" ResourceString="dataprotection.consents.consentarchive.consenttext.fulltext" DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="textarea-full-width editing-form-value-cell">
                        <cms:CMSHtmlEditor ID="htmlConsentFullText" runat="server" Height="400px" Enabled="false" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
