<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSModules_Newsletters_EmailBuilder_Email_Builder" Theme="Default"
    CodeBehind="Email_Builder.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" %>

<%@ Register Src="~/CMSModules/Newsletters/EmailBuilder/EmailProperties.ascx" TagPrefix="cms" TagName="EmailProperties" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/EmailBuilder/EmailABVariants.ascx" TagName="ABVariants" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="email-builder">
        <cms:AlertLabel runat="server" ID="alError" AlertType="Error" Text="&nbsp;" CssClass="alert-error-absolute hidden" EnableViewState="False" />
        <cms:AlertLabel runat="server" ID="alSuccess" AlertType="Confirmation" Text="&nbsp;" CssClass="alert-success-absolute hidden" EnableViewState="False" />
        <cms:AlertLabel runat="server" ID="alInfo" AlertType="Information" Text="&nbsp;" CssClass="alert-info-absolute hidden" EnableViewState="False" />
        <cms:AlertLabel runat="server" ID="alInfoRight" AlertType="Information" Text="&nbsp;" CssClass="alert-info-floating-right hidden" EnableViewState="False" />
        <div class="email-builder-right-section">
            <asp:PlaceHolder ID="plcVariantSelection" runat="server" Visible="false">
                <div class="variant-selection">
                    <div class="column variant-selector-label">
                        <cms:LocalizedLabel ID="lblVariantSelector" runat="server" ResourceString="emailbuilder.variantselector.label" />
                    </div>
                    <div class="column variant-selector">
                        <cms:CMSDropDownList ID="drpVariantsSelector" runat="server" EnableViewState="false" AutoPostBack="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:Panel ID="pnlTabs" runat="server" CssClass="tabs">
                <div class="tabs-nav">
                    <cms:LocalizedHyperlink runat="server" href="#" class="tab-link" ResourceString="emailbuilder.tab.widgets" />
                    <cms:LocalizedHyperlink runat="server" href="#" class="tab-link" ResourceString="emailbuilder.tab.emailproperties" />
                    <cms:LocalizedHyperlink runat="server" ID="lnkAbTesting" href="#" class="tab-link" ResourceString="emailbuilder.tab.abtesting" />
                </div>
                <div id="widgetsTab" class="tab-content">
                    <div class="widget-listing-container">
                        <cms:AlertLabel runat="server" ID="alNoRecords" AlertType="Warning" ResourceString="emailbuilder.nowidgetsassigned" EnableViewState="False" />
                        <cms:BasicRepeater ID="rptEmailWidgets" runat="server">
                            <ItemTemplate>
                                <div class="cms-email-widget" data-widget-id="<%# Eval("EmailWidgetGuid") %>" title="<%# HTMLHelper.EncodeForHtmlAttribute(GetWidgetTooltip(Eval<string>("EmailWidgetDisplayName"), Eval<string>("EmailWidgetDescription"))) %>">
                                    <div class="cms-email-widget-icon">
                                        <%# PortalHelper.GetIconHtml(Eval<Guid>("EmailWidgetThumbnailGUID"), $"{Eval<string>("EmailWidgetIconCssClass") ?? "icon-cogwheel-square"} cms-icon-150", 32) %>
                                    </div>
                                    <div class="cms-email-widget-label"><%# HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Eval<string>("EmailWidgetDisplayName"))) %></div>
                                </div>
                            </ItemTemplate>

                            <HeaderTemplate>
                                <div id="widget-listing">
                            </HeaderTemplate>

                            <FooterTemplate>
                                </div>
                            </FooterTemplate>
                        </cms:BasicRepeater>
                    </div>
                </div>
                <div id="emailPropertiesTab" class="tab-content">
                    <cms:EmailProperties ID="emailProperties" runat="server" Enabled="true" ShortID="ep" />
                </div>
                <asp:PlaceHolder ID="plcAbTestingTab" runat="server">
                    <div class="tab-content">
                        <cms:ABVariants runat="server" ID="abVariants" ShortID="abv" />
                    </div>
                </asp:PlaceHolder>
            </asp:Panel>
        </div>
        <div class="email-builder-content">
            <div class="header-actions-container">
                <cms:HeaderActions ID="headerActions" runat="server" UseSmallIcons="true" PanelCssClass="header-actions-main" />
            </div>
            <iframe id="builderIframe" runat="server" name="builderIframe" clientidmode="Static" enableviewstate="false" />
        </div>
    </div>
    <div class="widget-properties">
        <div class="widget-properties-wrapper">
            <div class="widget-properties-slidable">
                <div class="widget-properties-loader">
                    <%=ScriptHelper.GetLoaderHtml() %>
                </div>
                <iframe id="widgetPropertiesIframe"></iframe>
            </div>
        </div>
    </div>
</asp:Content>
