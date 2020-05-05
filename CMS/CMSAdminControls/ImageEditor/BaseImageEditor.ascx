<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_ImageEditor_BaseImageEditor"
     Codebehind="BaseImageEditor.ascx.cs" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/MetaDataEdit.ascx" TagName="MetaDataEditor"
    TagPrefix="cms" %>
<div class="image-editor-menu-column">
    <ajaxToolkit:Accordion ID="ajaxAccordion" runat="Server" CssClass="image-editor-main" ContentCssClass="image-editor-sub" HeaderCssClass="menu-header-item" HeaderSelectedCssClass="menu-header-item-selected">
        <Panes>
            <ajaxToolkit:AccordionPane ID="pnlAccordion1" runat="server">
                <Header>
                    <div class="header-inner editing-form-category collapsible-div">
                        <cms:LocalizedHeading ID="lchResize" runat="server" Level="4" ResourceString="img.resize" CssClass="editing-form-category-caption anchor" EnableViewState="false" />
                    </div>
                </Header>
                <Content>
                    <cms:LocalizedLabel ID="lblValidationFailedResize" runat="server" EnableViewState="false"
                        CssClass="ErrorLabel" ResourceString="img.errors.resize" />
                    <cms:CMSUpdatePanel ID="pnlAjax" runat="server" EnableViewState="false" UpdateMode="Always">
                        <ContentTemplate>
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="control-group-inline">
                                        <cms:CMSRadioButton ID="radByPercentage" runat="server" GroupName="Resize" Checked="true" AutoPostBack="true" />
                                    </div>
                                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                                        <cms:CMSTextBox ID="txtResizePercent" runat="server" AutoPostBack="true" CssClass="input-width-20 input-number" MaxLength="3" />
                                        <span class="form-control-text">%</span>
                                    </div>
                                    <div class="control-group-inline">
                                        <cms:CMSRadioButton ID="radByAbsolute" runat="server" GroupName="Resize" AutoPostBack="true" />
                                    </div>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblResizeWidth" runat="server" CssClass="control-label" AssociatedControlID="txtResizeWidth" EnableViewState="false" ResourceString="img.width" DisplayColon="true" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSTextBox ID="txtResizeWidth" runat="server" Enabled="false" CssClass="input-width-20 input-number" MaxLength="4" />
                                            <span class="form-control-text">px</span>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblResizeHeight" runat="server" CssClass="control-label" AssociatedControlID="txtResizeHeight" EnableViewState="false" ResourceString="img.height" DisplayColon="true" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSTextBox ID="txtResizeHeight" runat="server" Enabled="false" CssClass="input-width-20 input-number" MaxLength="4" />
                                            <span class="form-control-text">px</span>
                                        </div>
                                    </div>
                                    <div class="control-group-inline editing-form-value-cell-offset">
                                        <cms:CMSCheckBox ID="chkMaintainRatio" runat="server" AutoPostBack="true" OnCheckedChanged="chkMaintainRatioChanged" Enabled="false" Checked="true" />
                                    </div>
                                </div>
                                <div class="form-group form-group-submit">
                                    <cms:LocalizedButton ID="btnResize" runat="server" OnClick="btnResizeClick" ButtonStyle="Primary" EnableViewState="false" ResourceString="general.resize" />
                                </div>
                            </div>
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="pnlAccordion2" runat="server">
                <Header>
                    <div class="header-inner editing-form-category collapsible-div">
                        <cms:LocalizedHeading ID="LocalizedHeading1" runat="server" Level="4" ResourceString="img.rotation" CssClass="editing-form-category-caption anchor" EnableViewState="false" />
                    </div>
                </Header>
                <Content>
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="btns-vertical">
                                <cms:LocalizedButton ID="lblRotate90Left" runat="server" ButtonStyle="Primary" EnableViewState="false" OnClick="btnRotate90LeftClick" ResourceString="img.rotate90left" />
                                <cms:LocalizedButton ID="lblRotate90Right" runat="server" ButtonStyle="Primary" EnableViewState="false" OnClick="btnRotate90RightClick" ResourceString="img.rotate90right" />
                                <cms:LocalizedButton ID="lblFlipHorizontal" runat="server" ButtonStyle="Primary" EnableViewState="false" OnClick="btnFlipHorizontalClick" ResourceString="img.fliphorizontal" />
                                <cms:LocalizedButton ID="lblFlipVertical" runat="server" ButtonStyle="Primary" EnableViewState="false" OnClick="btnFlipVerticalClick" ResourceString="img.flipvertical" />
                            </div>
                        </div>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="pnlAccordion3" runat="server">
                <Header>
                    <div class="header-inner editing-form-category collapsible-div">
                        <cms:LocalizedHeading ID="LocalizedHeading2" runat="server" Level="4" ResourceString="img.convert" CssClass="editing-form-category-caption anchor" EnableViewState="false" />
                    </div>
                </Header>
                <Content>
                    <cms:LocalizedLabel ID="lblQualityFailed" runat="server" EnableViewState="false" CssClass="ErrorLabel" ResourceString="img.errors.quality" />
                    <cms:CMSUpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblFrom" runat="server" CssClass="control-label" ResourceString="img.from" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:LocalizedLabel ID="lblActualFormat" runat="server" CssClass="form-control-text" EnableViewState="false" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblTo" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="img.to" DisplayColon="true" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList ID="drpConvert" runat="server" AutoPostBack="false" CssClass="input-width-20" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblQuality" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="img.quality" DisplayColon="true" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox ID="txtQuality" runat="server" Text="100" Enabled="false" CssClass="input-width-20 input-number" MaxLength="3" />
                                        <span class="form-control-text">%</span>
                                    </div>
                                </div>
                                <div class="form-group form-group-submit">
                                    <cms:LocalizedButton ID="btnConvert" runat="server" OnClick="btnConvertClick" ButtonStyle="Primary" EnableViewState="false" ResourceString="img.convert" />
                                </div>
                            </div>
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="pnlAccordion4" runat="server">
                <Header>
                    <div class="header-inner editing-form-category collapsible-div js-trim-init">
                        <cms:LocalizedHeading ID="LocalizedHeading3" runat="server" Level="4" ResourceString="img.crop" CssClass="editing-form-category-caption anchor" EnableViewState="false" />
                    </div>
                </Header>
                <Content>
                    <asp:Panel ID="pnlCrop" runat="server" DefaultButton="btnCrop">
                        <cms:LocalizedLabel ID="lblCropError" runat="server" EnableViewState="false" CssClass="ErrorLabel" Visible="false" />
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lblCropX" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="img.cropX" DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSTextBox ID="txtCropX" runat="server" CausesValidation="true" CssClass="input-width-20 input-number" MaxLength="5" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lblCropY" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="img.cropY" DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSTextBox ID="txtCropY" runat="server" CausesValidation="true" CssClass="input-width-20 input-number" MaxLength="5" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lblCropWidth" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="img.cropWidth" DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSTextBox ID="txtCropWidth" runat="server" CausesValidation="true" CssClass="input-width-20 input-number" MaxLength="5" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lblCropHeight" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="img.cropHeight" DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSTextBox ID="txtCropHeight" runat="server" CausesValidation="true" CssClass="input-width-20 input-number" MaxLength="5" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="control-group-inline editing-form-value-cell-offset">
                                    <cms:CMSCheckBox ID="chkCropLock" runat="server" EnableViewState="false" ResourceString="img.cropLock" />
                                </div>
                            </div>
                            <div class="form-group form-group-submit">
                                <cms:LocalizedButton ID="btnCrop" runat="server" ButtonStyle="Primary" CssClass="js-btn-crop" ResourceString="img.crop" RenderScript="true"
                                    OnClick="btnCropClick" EnableViewState="false" />
                                <cms:LocalizedButton ID="btnCropReset" runat="server" ButtonStyle="Default" CssClass="js-btn-crop-reset" ResourceString="img.reset" RenderScript="true"
                                    EnableViewState="false" />
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:HiddenField ID="HiddenField1" runat="server" />
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="pnlAccordion5" runat="server">
                <Header>
                    <div class="header-inner editing-form-category collapsible-div">
                        <cms:LocalizedHeading ID="LocalizedHeading4" runat="server" Level="4" ResourceString="img.color" CssClass="editing-form-category-caption anchor" EnableViewState="false" />
                    </div>
                </Header>
                <Content>
                    <div class="form-horizontal">
                        <div class="form-group">
                            <cms:LocalizedButton ID="lblBtnGrayscale" runat="server" ButtonStyle="Primary" EnableViewState="false" OnClick="btnGrayscaleClick" ResourceString="img.grayscale" />
                        </div>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="pnlAccordion6" runat="server" ContentCssClass="image-editor-sub-empty">
                <Header>
                    <div class="header-inner editing-form-category collapsible-div">
                        <cms:LocalizedHeading ID="LocalizedHeading5" runat="server" Level="4" ResourceString="img.properties" CssClass="editing-form-category-caption anchor" EnableViewState="false" />
                    </div>
                </Header>
                <Content>
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>
    </ajaxToolkit:Accordion>
</div>
<div class="image-editor-image-column">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" UseRelativePlaceHolder="true" />
    <iframe id="frameImg" name="imageFrame" scrolling="auto" runat="server" class="image-editor-frame" />
    <asp:Image ID="imgMain" runat="server" Visible="false" CssClass="editing-image" />
</div>
<div class="image-editor-properties-column">
    <asp:Panel ID="pnlProperties" runat="server">
        <div id="divProperties" class="image-editor-properties">
            <cms:CMSUpdatePanel ID="updPanelProperties" runat="server">
                <ContentTemplate>
                    <asp:HiddenField ID="hdnShowProperties" runat="server" Value="false" />
                    <cms:MessagesPlaceHolder ID="plcMessProperties" runat="server" />
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblFileName" runat="server" CssClass="control-label" AssociatedControlID="txtFileName" EnableViewState="false" ResourceString="general.filename" DisplayColon="true" ShowRequiredMark="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtFileName" runat="server" MaxLength="250" />
                            </div>
                        </div>
                        <cms:MetaDataEditor ID="metaDataEditor" runat="server" RenderTableTag="false" ShowOnlyTitleAndDescription="true" RenderAsForm="false" />
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblExtensionText" runat="server" CssClass="control-label" AssociatedControlID="lblExtensionValue" EnableViewState="false" ResourceString="img.extension" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:LocalizedLabel ID="lblExtensionValue" CssClass="form-control-text" runat="server" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblImageSizeText" runat="server" CssClass="control-label" AssociatedControlID="lblImageSizeValue" ResourceString="img.imagesize" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:LocalizedLabel ID="lblImageSizeValue" CssClass="form-control-text" runat="server" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblWidthText" runat="server" CssClass="control-label" AssociatedControlID="lblWidthValue" EnableViewState="false" ResourceString="img.width" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <span class="form-control-text">
<cms:LocalizedLabel ID="lblWidthValue" runat="server" />
<span>px</span>
</span>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblHeightText" runat="server" CssClass="control-label" AssociatedControlID="lblHeightValue" EnableViewState="false" ResourceString="img.height" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <span class="form-control-text">
<cms:LocalizedLabel ID="lblHeightValue" runat="server" />
<span>px</span>
</span>
                            </div>
                        </div>
                        <div class="form-group form-group-submit">
                            <cms:LocalizedButton ID="btnChangeMetaData" runat="server" OnClick="btnChangeMetaDataClick" EnableViewState="false" ButtonStyle="Primary" ResourceString="general.update" />
                        </div>
                    </div>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </div>
    </asp:Panel>
</div>
