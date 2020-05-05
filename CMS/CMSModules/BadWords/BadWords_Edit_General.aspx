<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_BadWords_BadWords_Edit_General" Theme="Default"  Codebehind="BadWords_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/BadWords/FormControls/SelectBadWordAction.ascx" TagPrefix="cms"
    TagName="SelectBadWordAction" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWordExpression" EnableViewState="false"
                    DisplayColon="true" ResourceString="BadWords_Edit.WordExpressionLabel" AssociatedControlID="txtWordExpression" ShowRequiredMark="True"/>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtWordExpression" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rqfWordExpression" runat="server" Display="Dynamic"
                    ControlToValidate="txtWordExpression" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblIsRegular" runat="server" DisplayColon="true" EnableViewState="false"
                    ResourceString="badwords_edit.wordisregularexpressionlabel" AssociatedControlID="chkIsRegular" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkIsRegular" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblMatchWholeWord" runat="server" DisplayColon="true" EnableViewState="false"
                    ResourceString="badwords_edit.matchwholewordlabel" AssociatedControlID="chkMatchWholeWord" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkMatchWholeWord" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWordAction" EnableViewState="false" DisplayColon="true"
                    ResourceString="BadWords_Edit.WordActionLabel" AssociatedControlID="chkInheritAction" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkInheritAction" runat="server" ResourceString="BadWords_Edit.ActionInherit"
                    AutoPostBack="true" Checked="true" CssClass="field-value-override-checkbox" />
                <cms:SelectBadWordAction ID="SelectBadWordActionControl" runat="server" AllowAutoPostBack="true"
                    ReloadDataOnPostback="false" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcReplacement" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWordReplacement" EnableViewState="false"
                        ResourceString="BadWords_Edit.WordReplacementLabel" DisplayColon="true" AssociatedControlID="chkInheritReplacement" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkInheritReplacement" runat="server" ResourceString="BadWords_Edit.ActionInherit"
                        AutoPostBack="true" Checked="true" CssClass="field-value-override-checkbox" />
                    <cms:CMSTextBox ID="txtWordReplacement" runat="server" MaxLength="200" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    />
            </div>
        </div>
    </div>
</asp:Content>