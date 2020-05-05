<%@ Control Language="C#" AutoEventWireup="true"  CodeBehind="TranslationServiceSelector.ascx.cs"
    Inherits="CMSModules_Translations_Controls_TranslationServiceSelector" %>

<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>

<asp:Literal runat="server" ID="ltlServices" EnableViewState="false" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTranslateFromLang" ResourceString="translations.fromlanguage"
                EnableViewState="false" DisplayColon="true" ShowRequiredMark="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSUpdatePanel runat="server" ID="pnlUpdSourceLanguage">
                <ContentTemplate>
                    <cms:SiteCultureSelector runat="server" ID="selectCultureElem" AllowDefault="false" />
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcTargetLang" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTranslateToLang" ResourceString="translations.tolanguage"
                    EnableViewState="false" DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSUpdatePanel runat="server" ID="pnlUpdTargetLanguage">
                    <ContentTemplate>
                        <cms:SiteCultureSelector runat="server" ID="selectTargetCultureElem" SelectionMode="Multiple" AllowDefault="false" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
        </div>
    </asp:PlaceHolder>
    <div id="pnlSeparateSubmissions" class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="LocalizedLabel1" CssClass="control-label" runat="server" DisplayColon="True" AssociatedControlID="chkSeparateSubmissions" ResourceString="translations.useseparatesubmissions" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkSeparateSubmissions" runat="server" Checked="true" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group" id="pnlProcessBinary">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblProcessBinary" ResourceString="translations.processbinary" AssociatedControlID="chkProcessBinary"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkProcessBinary" Checked="false" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcPriority">
        <div class="form-group" id="pnlPriority">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblPriority" ResourceString="translations.priority"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:TranslationPriority runat="server" ID="drpPriority" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group" id="pnlDeadline">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDeadline" ResourceString="translations.deadline"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DateTimePicker runat="server" ID="dtDeadline" DisplayNow="false" />
        </div>
    </div>

    <div class="form-group" id="pnlInstructions">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblInstructions" ResourceString="translations.instructions"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextArea runat="server" ID="txtInstruction" Rows="4" MaxLength="500" />
        </div>
    </div>
</div>
<asp:HiddenField runat="server" ID="hdnSelectedName" />
