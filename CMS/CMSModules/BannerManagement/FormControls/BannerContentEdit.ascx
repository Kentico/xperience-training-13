<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="BannerContentEdit.ascx.cs" Inherits="CMSModules_BannerManagement_FormControls_BannerContentEdit" %>

<%@ Register Src="~/CMSFormControls/Macros/MacroEditor.ascx" TagName="MacroEditor" TagPrefix="cms" %>
<asp:PlaceHolder ID="plcHtml" runat="server" Visible="false">
    <div class="form-group">
        <div class="editing-form-label-cell label-full-width">
            <cms:LocalizedLabel ID="lblHtmlContent" runat="server" ResourceString="banner.bannernew.content" DisplayColon="true" EnableViewState="false" CssClass="control-label editing-form-label"
                AssociatedControlID="htmlBanner" ShowRequiredMark="true" />
        </div>
        <div class="editing-form-value-cell textarea-full-width">
            <cms:CMSHtmlEditor ID="htmlBanner" runat="server" CssClass="HtmlEditor" ToolbarSet="Full" />
        </div>
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcPlain" runat="server" Visible="false">
    <div class="form-group">
        <div class="editing-form-label-cell label-full-width">
            <cms:LocalizedLabel ID="lblPlainContent" runat="server" ResourceString="banner.bannernew.content" DisplayColon="true" EnableViewState="false" CssClass="control-label editing-form-label"
                AssociatedControlID="macroEditorPlain" ShowRequiredMark="true" />
        </div>
        <div class="editing-form-value-cell textarea-full-width">
            <cms:MacroEditor runat="server" ID="macroEditorPlain" />
        </div>
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcImage" runat="server">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblImageContent" runat="server" ResourceString="banner.bannernew.image" DisplayColon="true" EnableViewState="false" CssClass="control-label editing-form-label"
                AssociatedControlID="txtImage" ShowRequiredMark="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtImage" runat="server" CssClass="ImageSelector" IsLiveSite="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblImgTitle" runat="server" ResourceString="banner.bannernew.imgtitle" DisplayColon="true" EnableViewState="false" CssClass="control-label editing-form-label"
                AssociatedControlID="txtImgTitle" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtImgTitle" runat="server" MaxLength="2000" EnableViewState="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblImgAlt" runat="server" ResourceString="banner.bannernew.imgalt" DisplayColon="true" EnableViewState="false" CssClass="control-label editing-form-label"
                AssociatedControlID="txtImgAlt" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtImgAlt" runat="server" MaxLength="2000" EnableViewState="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblImgClass" runat="server" ResourceString="banner.bannernew.imgclass" DisplayColon="true" EnableViewState="false" CssClass="control-label editing-form-label"
                AssociatedControlID="txtImgClass" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtImgClass" runat="server" MaxLength="2000" EnableViewState="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblImgStyle" runat="server" ResourceString="banner.bannernew.imgstyle" DisplayColon="true" EnableViewState="false" CssClass="control-label editing-form-label"
                AssociatedControlID="txtImgStyle" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtImgStyle" runat="server" MaxLength="2000" EnableViewState="true" />
        </div>
    </div>
</asp:PlaceHolder>