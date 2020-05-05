<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Controls_PollProperties"
     Codebehind="PollProperties.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.general"></cms:LocalizedHeading>
    <asp:Panel ID="pnlGeneral" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDisplayName" EnableViewState="false" ResourceString="general.displayname"
                        DisplayColon="true" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtDisplayName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName:cntrlContainer:textbox"
                        ValidationGroup="required" Display="Dynamic" EnableViewState="false" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcCodeName" runat="Server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCodeName" EnableViewState="false" ResourceString="general.codename"
                            DisplayColon="true" ShowRequiredMark="True" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CodeName ID="txtCodeName" runat="server" MaxLength="200" />
                        <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" ControlToValidate="txtCodeName"
                            ValidationGroup="required" Display="Dynamic" EnableViewState="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTitle" EnableViewState="false" ResourceString="Polls_Edit.PollTitleLabel"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtTitle" runat="server" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblQuestion" EnableViewState="false" ResourceString="Polls_Edit.PollQuestionLabel"
                        DisplayColon="true" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtQuestion" runat="server" TextMode="MultiLine" MaxLength="450" />
                    <cms:CMSRequiredFieldValidator ID="rfvQuestion" runat="server" ControlToValidate="txtQuestion:cntrlContainer:textbox"
                        ValidationGroup="required" Display="Dynamic" EnableViewState="false" />
                    <cms:CMSRegularExpressionValidator ID="rfvMaxLength" Display="Dynamic" ControlToValidate="txtQuestion:cntrlContainer:textbox"
                        runat="server" ValidationExpression="^[\s\S]{0,450}$" ValidationGroup="required"
                        EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblResponseMessage" EnableViewState="false"
                        ResourceString="Polls_Edit.PollResponseMessageLabel" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtResponseMessage" runat="server" TextMode="MultiLine" MaxLength="450" />
                    <cms:CMSRegularExpressionValidator ID="rfvMaxLengthResponse" Display="Dynamic" ControlToValidate="txtResponseMessage:cntrlContainer:textbox"
                        runat="server" ValidationExpression="^[\s\S]{0,450}$" ValidationGroup="required"
                        EnableViewState="false" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.advancedsettings"></cms:LocalizedHeading>
    <asp:Panel ID="pnlAdvanced" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOpenFrom" EnableViewState="false" ResourceString="Polls_Edit.PollOpenFromLabel"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DateTimePicker ID="dtPickerOpenFrom" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOpenTo" EnableViewState="false" ResourceString="Polls_Edit.PollOpenToLabel"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DateTimePicker ID="dtPickerOpenTo" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAllowMultipleAnswers" EnableViewState="false"
                        ResourceString="Polls_Edit.PollAllowMultipleAnswersLabel" DisplayColon="true"
                        AssociatedControlID="chkAllowMultipleAnswers" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkAllowMultipleAnswers" runat="server" CssClass="CheckBoxMovedLeft"
                        AutoPostBack="false" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcOnline" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="LocalizedLabel1" EnableViewState="false" ResourceString="Polls_Edit.PollLogActivity"
                            DisplayColon="true" AssociatedControlID="chkLogActivity" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkLogActivity" runat="server" CssClass="CheckBoxMovedLeft" AutoPostBack="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                        ResourceString="General.OK" ValidationGroup="required" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Panel>