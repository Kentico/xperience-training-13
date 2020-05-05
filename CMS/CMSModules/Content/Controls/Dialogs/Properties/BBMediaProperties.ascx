<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Properties_BBMediaProperties"  Codebehind="BBMediaProperties.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/WidthHeightSelector.ascx" TagPrefix="cms"
    TagName="WidthHeightSelector" %>
<%@ Register Src="~/CMSInlineControls/ImageControl.ascx" TagPrefix="cms" TagName="ImagePreview" %>
<asp:Panel runat="server" ID="pnlEmpty" Visible="true" CssClass="DialogInfoArea">
    <asp:Label runat="server" ID="lblEmpty" EnableViewState="false" />
</asp:Panel>
<asp:Panel ID="pnlPreviewSize" runat="server">
    <cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="DialogElementHidden">
        <cms:JQueryTab ID="tabImageGeneral" runat="server" CssClass="media-properties-tab">
            <ContentTemplate>
                <asp:Panel runat="server" ID="pnlGeneralTab" CssClass="ImageGeneralTab PageContent" DefaultButton="imgRefresh">
                    <div class="form-horizontal media-properties">
                        <asp:PlaceHolder ID="plcUrlBox" runat="server">
                            <div class="form-group">
                                <div class="editing-form-label-cell media-dialog-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblUrl" runat="server" EnableViewState="false" DisplayColon="true"
                                        ResourceString="general.url" />
                                </div>
                                <div class="editing-form-value-cell media-dialog-value-cell">
                                    <div style="width: 100%;">
                                        <cms:CMSUpdatePanel ID="pnlUpdateImgUrl" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <cms:CMSTextBox ID="txtUrl" runat="server" CssClass="form-control media-url-control" />
                                                <cms:CMSAccessibleButton ID="imgRefresh" IconCssClass="icon-rotate-right cms-icon-80" IconOnly="true" runat="server" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <div class="properties">
                            <cms:CMSUpdatePanel ID="pnlUpdateWidthHeight" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cms:WidthHeightSelector ID="widthHeightElem" runat="server" ShowActions="false"
                                        ShowLabels="true" Locked="false" TextBoxesClass="input-width-20" />
                                </ContentTemplate>
                            </cms:CMSUpdatePanel>
                        </div>
                        <div class="preview">
                            <cms:CMSUpdatePanel ID="pnlImgPreview" runat="server">
                                <ContentTemplate>
                                    <div class="DialogPropertiesPreview DialogLongPreview">
                                        <cms:ImagePreview ID="imagePreview" runat="server" />Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque turpis lacus,
                                        convallis dignissim, consectetur vel, rutrum non, risus. Integer non risus et diam
                                        ultrices sollicitudin. Aliquam faucibus imperdiet massa. Vivamus eros. Cras eu dolor.
                                        Duis lacinia purus at massa. Praesent ornare nisl ac odio. Integer eget metus. Sed
                                        porttitor. Aliquam erat volutpat.
                                        <asp:Button ID="btnHidden" runat="server" CssClass="HiddenButton" EnableViewState="false" />
                                        <asp:Button ID="btnTxtHidden" runat="server" CssClass="HiddenButton" EnableViewState="false" />
                                        <asp:Button ID="btnHiddenSize" runat="server" CssClass="HiddenButton" EnableViewState="false" />
                                    </div>
                                </ContentTemplate>
                            </cms:CMSUpdatePanel>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </cms:JQueryTab>
    </cms:JQueryTabContainer>
</asp:Panel>
