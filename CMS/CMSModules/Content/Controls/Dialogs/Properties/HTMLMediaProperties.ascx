<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Properties_HTMLMediaProperties"  Codebehind="HTMLMediaProperties.ascx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/WidthHeightSelector.ascx"
    TagPrefix="cms" TagName="WidthHeightSelector" %>
<%@ Register Src="~/CMSInlineControls/MediaControl.ascx" TagPrefix="cms" TagName="MediaPreview" %>
<%@ Register Src="~/CMSInlineControls/ImageControl.ascx" TagPrefix="cms" TagName="ImagePreview" %>

<asp:Panel runat="server" ID="pnlEmpty" Visible="true" CssClass="DialogInfoArea">
    <asp:Label runat="server" ID="lblEmpty" EnableViewState="false" />
</asp:Panel>
<asp:Panel ID="pnlPreviewType" runat="server">
    <cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="DialogElementHidden">
        <cms:JQueryTab ID="tabImageGeneral" runat="server" CssClass="media-properties-tab">
            <ContentTemplate>
                <div class="ImageGeneralTab PageContent form-horizontal media-properties">
                    <asp:PlaceHolder ID="plcUrlTxt" runat="server">
                        <div class="form-group">
                            <div class="editing-form-label-cell media-dialog-label-cell">
                                <cms:LocalizedLabel ID="lblUrl" runat="server" EnableViewState="false" DisplayColon="true"
                                    ResourceString="general.url" CssClass="control-label" />
                            </div>
                            <div class="editing-form-value-cell media-dialog-value-cell">
                                <cms:CMSUpdatePanel ID="pnlUpdateImgUrl" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <cms:CMSTextBox ID="txtUrl" runat="server" CssClass="form-control media-url-control" />
                                        <cms:CMSAccessibleButton ID="imgRefresh" IconCssClass="icon-rotate-right cms-icon-80" IconOnly="true" runat="server" />
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <div class="properties">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblAlt" runat="server" EnableViewState="false" DisplayColon="true"
                                    ResourceString="dialogs.image.altlabel" CssClass="control-label" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtAlt" runat="server" />
                            </div>
                        </div>
                        <cms:CMSUpdatePanel ID="pnlUpdateWidthHeight" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cms:WidthHeightSelector ID="widthHeightElem" runat="server" ShowLabels="true" TextBoxesClass="input-width-20" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblBorderWidth" runat="server" EnableViewState="false" DisplayColon="true"
                                    ResourceString="dialogs.image.borderwidthlabel" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtBorderWidth" runat="server" CssClass="input-width-20" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblColor" runat="server" EnableViewState="false" DisplayColon="true"
                                    ResourceString="dialogs.image.bordercolorlabel" AssociatedControlID="UpdatePanel1" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSUpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
                                        <cms:ColorPicker ID="colorElem" runat="server" AllowOnInitInitialization="true" />
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblHSpace" runat="server" EnableViewState="false" DisplayColon="true"
                                    ResourceString="dialogs.image.hspacelabel" AssociatedControlID="txtHSpace" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtHSpace" runat="server" CssClass="input-width-20" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblVSpace" runat="server" EnableViewState="false" DisplayColon="true"
                                    ResourceString="dialogs.image.vspacelabel" AssociatedControlID="txtVSpace" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtVSpace" runat="server" CssClass="input-width-20" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblAlign" runat="server" EnableViewState="false" DisplayColon="true"
                                    ResourceString="dialogs.image.alignlabel" AssociatedControlID="drpAlign" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSDropDownList ID="drpAlign" runat="server" CssClass="input-width-60" />
                            </div>
                        </div>
                    </div>
                    <div class="preview">
                        <div class="DialogPropertiesPreview" style="margin-right: 16px;">
                            <cms:CMSUpdatePanel ID="pnlImgPreview" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cms:ImagePreview ID="imagePreview" runat="server" />
                                    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque turpis lacus,
                                        convallis dignissim, consectetur vel, rutrum non, risus. Integer non risus et diam
                                        ultrices sollicitudin. Aliquam faucibus imperdiet massa. Vivamus eros. Cras eu dolor.
                                        Duis lacinia purus at massa. Praesent ornare nisl ac odio. Integer eget metus. Sed
                                        porttitor. Aliquam erat volutpat.
                                    <asp:Button ID="btnImagePreview" CssClass="HiddenButton" runat="server" EnableViewState="false" />
                                    <asp:Button ID="btnImageTxtPreview" CssClass="HiddenButton" runat="server" EnableViewState="false" />
                                </ContentTemplate>
                            </cms:CMSUpdatePanel>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <asp:HiddenField ID="hdnUpdateItemUrl" runat="server" />
            </ContentTemplate>
        </cms:JQueryTab>
        <cms:JQueryTab ID="tabImageLink" runat="server" CssClass="media-properties-tab">
            <ContentTemplate>
                <div class="ImageLinkTab PageContent media-properties">
                    <div class="form-horizontal">
                        <div class="form-group" id="js-link-url-info">
                            <cms:LocalizedLabel ID="lblLinkInfo" runat="server" EnableViewState="false" ResourceString="dialogs.link.url.info" AssociatedControlID="lblLinkUrl" />
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell media-dialog-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblLinkUrl" runat="server" EnableViewState="false" ResourceString="dialogs.link.url"
                                    DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtLinkUrl" />
                                <cms:LocalizedButton runat="server" ID="btnLinkBrowseServer" EnableViewState="false"
                                    ResourceString="dialogs.link.browseserver" ButtonStyle="Default" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell media-dialog-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblLinkTarget" runat="server" EnableViewState="false" ResourceString="dialogs.link.targetname"
                                    DisplayColon="true" AssociatedControlID="drpLinkTarget" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSDropDownList ID="drpLinkTarget" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </ContentTemplate>
        </cms:JQueryTab>
        <cms:JQueryTab ID="tabImageAdvanced" runat="server" CssClass="media-properties-tab">
            <ContentTemplate>
                <div class="ImageAdvancedTab PageContent media-properties">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell media-dialog-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblImageAdvID" runat="server" EnableViewState="false" ResourceString="dialogs.advanced.id"
                                    DisplayColon="true" AssociatedControlID="txtImageAdvId" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtImageAdvId" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell media-dialog-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblImageAdvTooltip" runat="server" EnableViewState="false"
                                    ResourceString="dialogs.advanced.tooltip" DisplayColon="true" AssociatedControlID="txtImageAdvTooltip" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtImageAdvTooltip" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell media-dialog-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblImageAdvStyleClass" runat="server" EnableViewState="false"
                                    ResourceString="dialogs.advanced.class" DisplayColon="true" AssociatedControlID="txtImageAdvClass" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtImageAdvClass" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell media-dialog-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblImageAdvStyle" runat="server" EnableViewState="false"
                                    ResourceString="dialogs.advanced.style" DisplayColon="true" AssociatedControlID="txtImageAdvStyle" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextArea runat="server" ID="txtImageAdvStyle" EnableViewState="false" Rows="4" />
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </ContentTemplate>
        </cms:JQueryTab>
        <cms:JQueryTab ID="tabImageBehavior" runat="server" CssClass="media-properties-tab">
            <ContentTemplate>
                <div class="ImageBehaviorTab PageContent behavior">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="radio-list-vertical">
                                <cms:CMSRadioButton ID="radImageNone" runat="server" ResourceString="dialogs.image.behaviornone"
                                    GroupName="imgBehavior" Checked="true" />
                                <cms:CMSRadioButton ID="radImageSame" runat="server" ResourceString="dialogs.image.behaviorsame"
                                    GroupName="imgBehavior" />
                                <asp:Panel runat="server" ID="pnlRemoveLink" CssClass="selector-subitem">
                                    (<cms:LocalizedLinkButton ID="btnRemoveLink" runat="server" ResourceString="dialogs.behavior.removelink" />
                                    <cms:LocalizedLabel ID="lblRemoveLinkText" runat="server" EnableViewState="false" ResourceString="dialogs.behavior.removelinktext" />)
                                </asp:Panel>
                                <cms:CMSRadioButton ID="radImageNew" runat="server" ResourceString="dialogs.image.behaviornew"
                                    GroupName="imgBehavior" />
                                <asp:Panel runat="server" ID="pnlRemoveLink2" CssClass="selector-subitem">
                                    (<cms:LocalizedLinkButton ID="btnRemoveLink2" runat="server" ResourceString="dialogs.behavior.removelink" />
                                    <cms:LocalizedLabel ID="lblRemoveLinkText2" runat="server" EnableViewState="false" ResourceString="dialogs.behavior.removelinktext" />)
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </ContentTemplate>
        </cms:JQueryTab>
        <cms:JQueryTab ID="tabVideoGeneral" runat="server" CssClass="media-properties-tab">
            <ContentTemplate>
                <div class="VideoGeneralTab PageContent media-properties">
                    <div class="form-horizontal">
                        <asp:PlaceHolder ID="plcVidUrl" runat="server">
                            <div class="form-group">
                                <div class="editing-form-label-cell media-dialog-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblVidUrl" runat="server" EnableViewState="false" DisplayColon="true"
                                        ResourceString="general.url" />
                                </div>
                                <div class="editing-form-value-cell media-dialog-value-cell">
                                    <cms:CMSUpdatePanel ID="pnlVidUrl" runat="server">
                                        <ContentTemplate>
                                            <cms:CMSTextBox ID="txtVidUrl" runat="server" CssClass="form-control media-url-control" />
                                            <cms:CMSAccessibleButton ID="imgVidRefresh" IconCssClass="icon-rotate-right cms-icon-80" IconOnly="true" runat="server" />
                                        </ContentTemplate>
                                    </cms:CMSUpdatePanel>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <div class="properties">
                            <cms:WidthHeightSelector ID="vidWidthHeightElem" runat="server" ShowLabels="true" TextBoxesClass="input-width-20"
                                Locked="false" />
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblVidAutoPlay" runat="server" ResourceString="dialogs.vid.autoplay"
                                        EnableViewState="false" DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSCheckBox ID="chkVidAutoPlay" runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblVidLoop" runat="server" ResourceString="dialogs.vid.loop"
                                        EnableViewState="false" DisplayColon="true" AssociatedControlID="chkVidLoop" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSCheckBox ID="chkVidLoop" runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblVidShowControls" runat="server" ResourceString="dialogs.vid.showcontrols"
                                        EnableViewState="false" DisplayColon="true" AssociatedControlID="chkVidShowControls" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSCheckBox ID="chkVidShowControls" runat="server" Checked="true" EnableViewState="false" />
                                </div>
                            </div>
                        </div>
                        <div class="preview">
                            <div class="DialogPropertiesPreview DialogMediaPreview">
                                <cms:CMSUpdatePanel ID="pnlVidPreview" runat="server">
                                    <ContentTemplate>
                                        <asp:Button ID="btnVideoPreview" CssClass="HiddenButton" runat="server" EnableViewState="false" />
                                        <cms:MediaPreview ID="videoPreview" runat="server" />
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                            </div>

                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </ContentTemplate>
        </cms:JQueryTab>
    </cms:JQueryTabContainer>
</asp:Panel>
<asp:Button ID="btnSizeRefreshHidden" runat="server" CssClass="HiddenButton" EnableViewState="false" />