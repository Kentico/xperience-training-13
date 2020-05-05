<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="NewCultureVersion.ascx.cs" Inherits="CMSModules_Content_Controls_NewCultureVersion" %>
<%@ Register TagPrefix="cms" TagName="SiteCultureSelector" Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" %>
<%@ Register TagPrefix="cms" TagName="TranslationServiceSelector" Src="~/CMSModules/Translations/Controls/TranslationServiceSelector.ascx" %>

<cms:LocalizedHeading runat="server" ID="headNewCultureVersion" EnableViewState="false" Level="3" ResourceString="ContentNewCultureVersion.Info" />
<asp:Panel runat="server" ID="pnlNewVersion">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="radio-list-vertical">
                <cms:CMSRadioButton ID="radEmpty" runat="server" GroupName="NewVersion" EnableViewState="false" />
                <cms:CMSRadioButton ID="radCopy" runat="server" GroupName="NewVersion" EnableViewState="false" Checked="true" />
                <div id="divCultures" class="selector-subitem">
                    <div class="control-group-inline">
                        <div class="form-group">
                            <cms:SiteCultureSelector runat="server" ID="copyCulturesElem" AllowDefault="false" />
                        </div>
                        <div class="form-group">
                            <cms:CMSCheckBox runat="server" ID="chkSaveBeforeEditing" Checked="true" ResourceString="ContentNewCultureVersion.SaveBeforeEditing" />
                        </div>
                    </div>
                </div>
                <asp:PlaceHolder runat="server" ID="plcTranslationServices">
                    <cms:CMSRadioButton ID="radTranslate" runat="server" GroupName="NewVersion" EnableViewState="false" />
                    <div id="divTranslations" style="display: none;" class="selector-subitem">
                        <cms:TranslationServiceSelector runat="server" ID="translationElem" DisplayOnlyServiceName="true" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
        <div>
            <div id="divTranslate" style="display: none;">
                <cms:CMSButton runat="server" ButtonStyle="Primary" ID="btnTranslate" />
            </div>
            <div id="divCreate">
                <cms:CMSButton runat="server" ButtonStyle="Primary" ID="btnCreateDocument" />
            </div>
        </div>
    </div>
</asp:Panel>
