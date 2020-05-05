<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_CountrySettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_CountrySettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTwoLetterCode" ResourceString="clonning.settings.country.twolettercode"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtTwoLetterCode" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtTwoLetterCode" MaxLength="2" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblThreeLetterCode" ResourceString="clonning.settings.country.threelettercode"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtThreeLetterCode" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtThreeLetterCode" MaxLength="3" />
        </div>
    </div>
</div>