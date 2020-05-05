<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Controls_GroupRegistration"  Codebehind="GroupRegistration.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:PlaceHolder ID="plcForm" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblDisplayName" runat="server" AssociatedControlID="txtDisplayName" EnableViewState="false" /></div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtDisplayName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName:cntrlContainer:textbox"
                    Display="Dynamic" ValidationGroup="GroupEdit" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblDescription" AssociatedControlID="txtDescription" runat="server" EnableViewState="false" /></div>
            <div class="editing-form-value-cell">
                <cms:CMSTextArea ID="txtDescription" runat="server" Rows="4" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblApproveMembers" runat="server" EnableViewState="false" /></div>
            <div class="editing-form-value-cell">
                <cms:CMSRadioButton ID="radMembersAny" runat="server" GroupName="approvemembers" Checked="true" />
                <cms:CMSRadioButton ID="radMembersApproved" runat="server" GroupName="approvemembers" />
                <cms:CMSRadioButton ID="radMembersInvited" runat="server" GroupName="approvemembers" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblContentAccess" runat="server" EnableViewState="false" /></div>
            <div class="editing-form-value-cell">
                <cms:CMSRadioButton ID="radAnybody" runat="server" GroupName="contentaccess" Checked="true" />
                <cms:CMSRadioButton ID="radSiteMembers" runat="server" GroupName="contentaccess" />
                <cms:CMSRadioButton ID="radGroupMembers" runat="server" GroupName="contentaccess" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton ID="btnSave" runat="server" OnClick="btnSave_Click"
                    ValidationGroup="GroupEdit" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:PlaceHolder>