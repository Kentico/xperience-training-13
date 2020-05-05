<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="FieldMacroRuleEditor.ascx.cs" Inherits="CMSFormControls_System_FieldMacroRuleEditor" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" ObjectType="cms.macrorule" SelectionMode="SingleDropDownList" ShortID="uM"
            WhereCondition="MacroRuleResourceName = 'cms.formengine'" ReturnColumnName="MacroRuleName" AllowEmpty="false" OnOnSelectionChanged="uniSelector_OnSelectionChanged" />
        <asp:Label runat="server" ID="ltlDescription" EnableViewState="false" CssClass="rule-description" />
        <cms:BasicForm runat="server" ID="formProperties" Mode="Update" EnsureFirstLetterUpperCase="true" FormButtonPanelCssClass="rule-button" CssClass="form-properties" />
        <div class="form-horizontal">
            <div class="form-group">
                <cms:LocalizedLabel ID="lblErrorMsg" CssClass="control-label-top" runat="server" ResourceString="general.errormessage" EnableViewState="false" DisplayColon="true" AssociatedControlID="tE" />
                <cms:LocalizableTextBox ID="txtErrorMsg" CssClass="form-control" runat="server" ShortID="tE" TextMode="MultiLine" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>