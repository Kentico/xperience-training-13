<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Controls_PollNew"  Codebehind="PollNew.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" AssociatedControlID="txtDisplayName" ID="lblDisplayName"
                    EnableViewState="false" ResourceString="general.displayname" DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtDisplayName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName:cntrlContainer:textbox"
                    ValidationGroup="Required" EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcCodeName" runat="Server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" AssociatedControlID="txtCodeName" ID="lblCodeName"
                        EnableViewState="false" ResourceString="general.codename" DisplayColon="true" ShowRequiredMark="True" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="control-group-inline">
                        <cms:CodeName ID="txtCodeName" runat="server" MaxLength="200" />
                        <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" ControlToValidate="txtCodeName"
                            ValidationGroup="Required" EnableViewState="false" />
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblTitle" AssociatedControlID="txtTitle" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtTitle" runat="server" MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblQuestion" AssociatedControlID="txtQuestion" EnableViewState="false" ShowRequiredMark="true"  />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtQuestion" runat="server" TextMode="MultiLine" MaxLength="450" />
                <cms:CMSRequiredFieldValidator Display="Dynamic" ID="rfvQuestion" runat="server" ControlToValidate="txtQuestion:cntrlContainer:textbox"
                    ValidationGroup="Required" EnableViewState="false" />
                <cms:CMSRegularExpressionValidator ID="rfvMaxLength" Display="Dynamic" ControlToValidate="txtQuestion:cntrlContainer:textbox"
                    runat="server" ValidationExpression="^[\s\S]{0,450}$" ValidationGroup="Required"
                    EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    ValidationGroup="Required" ResourceString="General.OK" />
            </div>
        </div>
    </div>
</asp:Panel>
