<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_MacroRuleDesigner"
     Codebehind="MacroRuleDesigner.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlMain" DefaultButton="btnFilter">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false" />
            <div class="editor">
                <cms:LocalizedHeading runat="server" ID="headRuleEdit" Level="4" ResourceString="macros.macrorule.ruleifno" DisplayColon="true" EnableViewState="false" />
                <asp:Panel runat="server" ID="pnlButtons" CssClass="MacroRuleToolbar">
                    <div class="MacroRuleToolbarButton">
                        <cms:CMSAccessibleLinkButton runat="server" ID="btnDelete" IconCssClass="icon-times cms-icon-80" CssClass="btn-icon" />
                    </div>
                    <div class="MacroRuleToolbarButton">
                        <cms:CMSAccessibleLinkButton runat="server" ID="btnClearAll" IconCssClass="icon-broom cms-icon-80" CssClass="btn-icon" />
                    </div>
                    <div class="MacroRuleToolbarSeparator">
                        &nbsp;
                    </div>
                    <div class="MacroRuleToolbarButton">
                        <cms:CMSAccessibleLinkButton runat="server" ID="btnUnindent" IconCssClass="icon-outdent cms-icon-80" CssClass="btn-icon" />
                    </div>
                    <div class="MacroRuleToolbarButton">
                        <cms:CMSAccessibleLinkButton runat="server" ID="btnIndent" IconCssClass="icon-indent cms-icon-80" CssClass="btn-icon" />
                    </div>
                    <div class="MacroRuleToolbarSeparator">
                        &nbsp;
                    </div>
                    <div class="MacroRuleToolbarButton">
                        <cms:CMSAccessibleLinkButton runat="server" ID="btnAutoIndent" IconCssClass="icon-tree-structure cms-icon-80" CssClass="btn-icon" />
                    </div>
                    <div class="MacroRuleToolbarSeparator">
                        &nbsp;
                    </div>
                    <div class="MacroRuleToolbarButton">
                        <cms:CMSAccessibleLinkButton runat="server" ID="btnViewCode" IconCssClass="icon-eye cms-icon-80" CssClass="btn-icon" />
                    </div>
                </asp:Panel>
                <div id="scrollDiv" class="MacroRuleAreaBorder">
                    <asp:Panel runat="server" ID="pnlCondtion" CssClass="MacroRuleArea">
                        <asp:Literal runat="server" ID="ltlText" EnableViewState="false" />
                    </asp:Panel>
                </div>
            </div>
            <div class="add-clause">
                <cms:CMSAccessibleButton runat="server" ID="btnAddaClause" EnableViewState="false" IconCssClass="icon-chevron-left" CssClass="btn-first btn-last" />
            </div>
            <div class="rules">
                <asp:Panel runat="server" ID="pnlRules">
                    <cms:LocalizedHeading runat="server" ID="headRuleList" Level="4" ResourceString="macros.macrorule.ruleslist" DisplayColon="true" EnableViewState="false" />
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel runat="server" ID="lblFilter" ResourceString="macros.macrorule.searchrule"
                                    DisplayColon="true" CssClass="control-label" AssociatedControlID="txtFilter" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtFilter" CssClass="form-control input-width-82" />
                                <cms:CMSButton runat="server" ID="btnFilter" ButtonStyle="Default" />
                            </div>
                        </div>
                    </div>
                    <cms:CMSListBox runat="server" ID="lstRules" AutoPostBack="false" SelectionMode="Single" />
                </asp:Panel>
            </div>
            <cms:ModalPopupDialog ID="mdlDialog" runat="server" BackgroundCssClass="ModalBackground"
                CssClass="ModalPopupDialog" Visible="false">
                <asp:Panel runat="server" ID="pnlParameterPopup" Width="800px">
                    <div style="height: auto; min-height: 0px;">
                        <div class="PageHeader">
                            <cms:PageTitle ID="titleElem" runat="server" EnableViewState="false" />
                        </div>
                    </div>
                    <asp:Panel runat="server" ID="pnlModalProperty" Visible="false" CssClass="DialogPageBody">
                        <div class="MacroRuleDialogBody">
                            <asp:Panel ID="pnlFormControl" runat="server">
                                <cms:BasicForm runat="server" ID="formElem" Mode="Update" EnsureFirstLetterUpperCase="true" />
                            </asp:Panel>
                        </div>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlFooter" Visible="false" CssClass="dialog-footer control-group-inline">           
                        <cms:CMSButton runat="server" ID="btnCancel" ButtonStyle="Default" />
                        <cms:CMSButton runat="server" ID="btnSetParameter" ButtonStyle="Primary" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlViewCode" Visible="false" CssClass="DialogPageBody">
                        <div class="MacroRuleDialogBody">
                            <cms:MacroEditor runat="server" ID="viewCodeElem" Width="100%" ReadOnly="true" />
                            <div class="FloatRight MacroRuleDialogButton">
                                <cms:CMSButton runat="server" ID="btnCodeOK" ButtonStyle="Primary" />
                            </div>
                    </asp:Panel>
                </asp:Panel>
            </cms:ModalPopupDialog>
            <asp:HiddenField runat="server" ID="hdnScroll" EnableViewState="false" />
            <asp:HiddenField runat="server" ID="hdnSelected" EnableViewState="false" />
            <asp:HiddenField runat="server" ID="hdnParamSelected" EnableViewState="false" />
            <asp:HiddenField runat="server" ID="hdnLastParam" EnableViewState="false" />
            <asp:HiddenField runat="server" ID="hdnLastSelected" EnableViewState="false" />
            <asp:HiddenField runat="server" ID="hdnOpSelected" EnableViewState="false" />
            <asp:HiddenField runat="server" ID="hdnParam" EnableViewState="false" />
            <asp:HiddenField runat="server" ID="hdnParamEditShown" EnableViewState="false" />
            <asp:Button runat="server" ID="btnChangeOperator" CssClass="HiddenButton" />
            <asp:Button runat="server" ID="btnChangeParameter" CssClass="HiddenButton" />
            <asp:Button runat="server" ID="btnMove" CssClass="HiddenButton" />
            <asp:Button runat="server" ID="btnAddClause" CssClass="HiddenButton" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>