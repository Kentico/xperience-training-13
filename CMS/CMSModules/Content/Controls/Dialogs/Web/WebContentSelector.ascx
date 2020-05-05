<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Web_WebContentSelector"  Codebehind="WebContentSelector.ascx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/HTMLMediaProperties.ascx"
    TagName="MediaProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/BBMediaProperties.ascx" TagName="BBMediaProperties"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/URLProperties.ascx" TagName="URLProperties"
    TagPrefix="cms" %>

<script type="text/javascript" >
    function insertItem() {
        RaiseHiddenPostBack();
    }
</script>

<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="DialogViewContent">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblUrl" runat="server" ResourceString="dialogs.link.url"
                            EnableViewState="false" DisplayColon="true" AssociatedControlID="txtUrl" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox runat="server" ID="txtUrl" />
                        <asp:PlaceHolder ID="plcRefresh" runat="server">
                            <cms:CMSAccessibleButton ID="imgRefresh" IconCssClass="icon-rotate-right cms-icon-80" IconOnly="true" runat="server" />
                        </asp:PlaceHolder>
                    </div>
                </div>
                <asp:PlaceHolder ID="plcAlternativeText" runat="server">
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
                <asp:PlaceHolder ID="plcMediaType" runat="server">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblMediaType" runat="server" ResourceString="dialogs.web.mediatype"
                                EnableViewState="false" DisplayColon="true" AssociatedControlID="drpMediaType" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpMediaType" runat="server" AutoPostBack="true" CssClass="DropDownField" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
        <div id="divDialogResizer" class="DialogResizerVLine" runat="server" enableviewstate="false">
            &nbsp;
        </div>
        <asp:Panel ID="pnlProperties" runat="server" CssClass="DialogProperties DialogWebProperties">
            <asp:PlaceHolder runat="server" ID="plcInfo" Visible="true">
                <div class="DialogInfoArea LeftAlign">
                    <cms:LocalizedLabel runat="server" ID="lblInfo" EnableViewState="false" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcHTMLMediaProp" Visible="true">
                <cms:MediaProperties runat="server" ID="propMedia" DisplayUrlTextbox="false" />
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcBBMediaProp" Visible="true">
                <cms:BBMediaProperties runat="server" ID="propBBMedia" HideUrlBox="true" IsWeb="true" />
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcURLProp" Visible="true">
                <div class="DialogWebUrlProp">
                    <cms:URLProperties runat="server" ID="propURL" IsWeb="true" />
                </div>
            </asp:PlaceHolder>
        </asp:Panel>
        <asp:Button ID="hdnButton" runat="server" OnClick="hdnButton_Click" CssClass="HiddenButton" />
        <asp:Button ID="hdnButtonUrl" runat="server" OnClick="hdnButtonUrl_Click" CssClass="HiddenButton" />
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
