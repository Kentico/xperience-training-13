<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_DocumentTypeSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_DocumentTypeSettings" %>

<div class="form-horizontal">
    <asp:PlaceHolder runat="server" ID="plcTableName">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTableName" ResourceString="clonning.settings.class.tablename"
                    EnableViewState="false" DisplayColon="true" AssociatedControlID="txtTableName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtTableName" MaxLength="100" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblIcons" ResourceString="clonning.settings.documenttype.icons"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkIcons" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkIcons" Checked="true" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcAlternativeForms">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCloneAlternativeForms" ResourceString="clonning.settings.class.alternativeform"
                    EnableViewState="false" DisplayColon="true" AssociatedControlID="chkCloneAlternativeForms" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkCloneAlternativeForms" Checked="true" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>