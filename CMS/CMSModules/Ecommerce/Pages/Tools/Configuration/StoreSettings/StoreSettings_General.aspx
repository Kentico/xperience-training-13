<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_General"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="StoreSettings_General.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="SettingsGroupViewer" Src="~/CMSModules/Settings/Controls/SettingsGroupViewer.ascx" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <%-- Anchor links --%>
    <cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="5" />
    <div class="form-horizontal">
        <cms:FormCategoryHeading runat="server" ID="headCurrencies" Level="4" ResourceString="com.storesettings.currencies" IsAnchor="True" />
        <div class="editing-form-category-fields">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblCurrentMainCurrency" runat="server" EnableViewState="false" CssClass="control-label editing-form-label"
                        ResourceString="Configuration_StoreSettings.lblMainCurrency" DisplayColon="True" AssociatedControlID="lblMainCurrency" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="settings-group-inline">
                        <asp:Label ID="lblMainCurrency" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        <cms:LocalizedButton ID="btnChangeCurrency" runat="server" EnableViewState="false"
                            ButtonStyle="Default" ResourceString="general.change" />
                        <cms:LocalizedLabel ID="lblHdnChangeCurrency" runat="server" ResourceString="general.change" CssClass="sr-only" AssociatedControlID="btnChangeCurrency" />
                        <div class="settings-info-group">
                            <span class="info-icon">
                                <cms:CMSIcon ID="icoHelp" runat="server" CssClass="icon-question-circle" data-html="true" />
                            </span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <cms:SettingsGroupViewer ID="SettingsGroupViewer" runat="server" AllowGlobalInfoMessage="false" />
</asp:Content>
