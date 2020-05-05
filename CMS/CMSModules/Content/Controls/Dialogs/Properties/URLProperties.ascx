<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Properties_URLProperties"
    CodeBehind="URLProperties.ascx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/WidthHeightSelector.ascx"
    TagPrefix="cms" TagName="WidthHeightSelector" %>
<%@ Register Src="~/CMSInlineControls/ImageControl.ascx" TagPrefix="cms" TagName="ImagePreview" %>
<%@ Register Src="~/CMSInlineControls/MediaControl.ascx" TagPrefix="cms" TagName="MediaPreview" %>

<asp:PlaceHolder ID="plcPropContent" runat="server">
    <asp:Panel runat="server" ID="pnlEmpty" Visible="true" CssClass="DialogInfoArea">
        <asp:Label runat="server" ID="lblEmpty" EnableViewState="false" />
    </asp:Panel>
    <asp:Panel ID="pnlPreview" runat="server" CssClass="small-preview">
        <cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="DialogElementHidden">
            <cms:JQueryTab ID="tabImageGeneral" runat="server" CssClass="media-properties-tab">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlGeneralTab" CssClass="ImageGeneralTab PageContent" DefaultButton="imgRefresh">
                        <div class="form-horizontal media-properties">
                            <asp:PlaceHolder runat="server" ID="plcUrl" Visible="true">
                                <div class="form-group">
                                    <div class="editing-form-label-cell media-dialog-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" ID="lblUrl" runat="server" EnableViewState="false" DisplayColon="true"
                                            ResourceString="general.url" />
                                    </div>
                                    <div class="editing-form-value-cell media-dialog-value-cell">
                                        <cms:CMSUpdatePanel ID="pnlUpdateImgUrl" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <cms:CMSTextBox ID="txtUrl" runat="server" CssClass="form-control media-url-control" />
                                                <cms:CMSAccessibleButton ID="imgRefresh" IconCssClass="icon-rotate-right cms-icon-80" IconOnly="true" runat="server" OnClick="imgRefresh_Click" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="properties">
                                <asp:PlaceHolder runat="server" ID="plcSelectPath" Visible="false">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel CssClass="control-label" ID="lblSelectPah" runat="server" EnableViewState="false" ResourceString="generalproperties.aliaspath" AssociatedControlID="txtSelectPath" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSTextBox ID="txtSelectPath" runat="server" ReadOnly="true" />
                                        </div>
                                    </div>
                                    <asp:PlaceHolder runat="server" ID="plcIncludeSubitems" Visible="false">
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel ID="lblIncludeSubItems" ResourceString="pathselection.inlcudesubitems" runat="server" CssClass="control-label" AssociatedControlID="chkItems" DisplayColon="true" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <cms:CMSCheckBox ID="chkItems" runat="server" EnableViewState="false" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcAltText" runat="server">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel CssClass="control-label" ID="lblAlt" runat="server" EnableViewState="false" DisplayColon="true"
                                                ResourceString="dialogs.image.altlabel" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSTextBox ID="txtAlt" runat="server" />
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcSizeSelector" runat="server">
                                    <cms:CMSUpdatePanel ID="pnlUpdateWidthHeight" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <cms:WidthHeightSelector ID="widthHeightElem" runat="server" ShowActions="false"
                                                ShowLabels="true" Locked="false" TextBoxesClass="input-width-20" />
                                        </ContentTemplate>
                                    </cms:CMSUpdatePanel>
                                </asp:PlaceHolder>
                            </div>
                            <div class="preview">
                                <cms:CMSUpdatePanel runat="server" ID="pnlPreviewArea" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:PlaceHolder ID="plcPreviewArea" runat="server">
                                            <asp:Panel runat="server" ID="pnlImagePreview" CssClass="DialogPropertiesPreview DialogLongPreview">
                                                <cms:ImagePreview ID="imagePreview" runat="server" />
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="pnlMediaPreview" CssClass="DialogPropertiesPreview DialogLongPreview">
                                                <cms:MediaPreview runat="server" ID="mediaPreview" />
                                            </asp:Panel>
                                        </asp:PlaceHolder>
                                        <asp:Button ID="btnHidden" runat="server" CssClass="HiddenButton" EnableViewState="false" />
                                        <asp:Button ID="btnTxtHidden" runat="server" CssClass="HiddenButton" EnableViewState="false" />                                        
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                                <asp:Button ID="btnHiddenSize" runat="server" CssClass="HiddenButton" EnableViewState="false" />
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </cms:JQueryTab>
        </cms:JQueryTabContainer>
    </asp:Panel>
    <script type="text/javascript">
        //<![CDATA[

        // Called on checkbox checked changed
        function chkItemsChecked_Changed(checked) {
            var aliasPathElem = document.getElementById('<%=txtSelectPath.ClientID%>');
            if (aliasPathElem != null) {
                var length = aliasPathElem.value.length;

                if (checked) {
                    // Check last two letters
                    if (aliasPathElem.value == "/") {
                        aliasPathElem.value = "/%";
                    }
                    else if (aliasPathElem.value.substring(length - 2, length) != "/%") {
                        aliasPathElem.value += "/%";
                    }
                }
                else {
                    // Check last two letters                                                          
                    if (aliasPathElem.value == "/%") {
                        aliasPathElem.value = "/";
                    }
                    else if (aliasPathElem.value.substring(length - 2, length) == "/%") {
                        aliasPathElem.value = aliasPathElem.value.substring(0, length - 2);
                    }
                }

                // save alias path
                aliasPath = aliasPathElem.value;
            }
        }

        //]]>
    </script>
</asp:PlaceHolder>
