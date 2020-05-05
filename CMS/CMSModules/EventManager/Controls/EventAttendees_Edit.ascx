<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/EventManager/Controls/EventAttendees_Edit.ascx.cs"
    Inherits="CMSModules_EventManager_Controls_EventAttendees_Edit" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:Panel ID="pnlContent" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblFirstName" AssociatedControlID="txtFirstName" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtFirstName" runat="server" MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblLastName" AssociatedControlID="txtLastName" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtLastName" runat="server" MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEmail" AssociatedControlID="txtEmail" EnableViewState="false"
                    ResourceString="general.email" DisplayColon="true" ShowRequiredMark="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtEmail" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvEmail" runat="server" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" AssociatedControlID="txtPhone" ID="lblPhone" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtPhone" runat="server" MaxLength="50" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false"
                    OnClick="btnOK_Click" ResourceString="general.ok" />
            </div>
        </div>
    </div>
</asp:Panel>