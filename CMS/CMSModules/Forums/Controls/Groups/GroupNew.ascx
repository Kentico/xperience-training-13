<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Groups_GroupNew"  Codebehind="GroupNew.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblGroupDisplayName" EnableViewState="false" AssociatedControlID="txtGroupDisplayName" ResourceString="Group_General.GroupDisplayNameLabel" ShowRequiredMark="True" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtGroupDisplayName" runat="server" MaxLength="200" />
            <cms:CMSRequiredFieldValidator ID="rfvGroupDisplayName" runat="server" ControlToValidate="txtGroupDisplayName:cntrlContainer:textbox"
                ValidationGroup="vgForumGroup" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcCodeName" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblGroupName" EnableViewState="false" AssociatedControlID="txtGroupName" ResourceString="Group_General.GroupNameLabel" ShowRequiredMark="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtGroupName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvGroupName" runat="server" ControlToValidate="txtGroupName"
                    ValidationGroup="vgForumGroup" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDescription" EnableViewState="false" ResourceString="general.description"
                DisplayColon="true" AssociatedControlID="txtGroupDescription" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtGroupDescription" runat="server" TextMode="MultiLine" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcBaseAndUnsubUrl" runat="server" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblForumBaseUrl" EnableViewState="false" AssociatedControlID="chkInheritBaseUrl" ResourceString="Group_General.ForumBaseUrlLabel" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox runat="server" ID="chkInheritBaseUrl" Checked="true" ResourceString="Forums.InheritBaseUrl" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtForumBaseUrl" runat="server" MaxLength="200" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUnsubscriptionUrl" EnableViewState="false" AssociatedControlID="chkInheritUnsubUrl" ResourceString="Group_General.UnsubscriptionUrlLabel" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox runat="server" ID="chkInheritUnsubUrl" Checked="true" ResourceString="Forums.InheritUnsubsUrl" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtUnsubscriptionUrl" runat="server" MaxLength="200" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnOk" runat="server" EnableViewState="false" OnClick="btnOK_Click"
                ValidationGroup="vgForumGroup" ResourceString="General.OK" />
        </div>
    </div>
</div>
