<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MessageBoards_Controls_Boards_BoardSecurity"  Codebehind="BoardSecurity.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Roles/securityAddRoles.ascx" TagName="AddRoles" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <cms:LocalizedHeading runat="server" ID="lblTitleGeneral" EnableViewState="false" Level="4" ResourceString="general.general"
        Visible="true" DisplayColon="False" />
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" ResourceString="board.security.usecaptcha" DisplayColon="True" CssClass="control-label" AssociatedControlID="chkUseCaptcha"></cms:LocalizedLabel>
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkUseCaptcha" />
        </div>
    </div>
</div>
<div class="form-horizontal">
    <cms:LocalizedHeading runat="server" ID="lblTitleComments" CssClass="listing-title" EnableViewState="false" Level="4" ResourceString="board.security.commentstitle"
        Visible="true" />
    <div class="form-group">
        <div class="radio-list-vertical">
            <cms:CMSRadioButton ID="radAllUsers" runat="server" GroupName="board" AutoPostBack="True"
                OnCheckedChanged="radAllUsers_CheckedChanged" EnableViewState="false" />
            <cms:CMSRadioButton ID="radOnlyUsers" runat="server" GroupName="board" AutoPostBack="True"
                OnCheckedChanged="radOnlyUsers_CheckedChanged" EnableViewState="false" />
            <asp:PlaceHolder ID="plcGroupMembers" runat="server">
                <cms:CMSRadioButton ID="radGroupMembers" runat="server" GroupName="board" AutoPostBack="True"
                    OnCheckedChanged="radGroupMembers_CheckedChanged" EnableViewState="false" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcOnlyOwner" runat="server">
                <cms:CMSRadioButton ID="radOnlyOwner" runat="server" GroupName="board" AutoPostBack="True"
                    OnCheckedChanged="radOnlyOwner_CheckedChanged" EnableViewState="false" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcOnlyGroupAdmin" runat="server">
                <cms:CMSRadioButton ID="radOnlyGroupAdmin" runat="server" GroupName="board" AutoPostBack="True"
                    OnCheckedChanged="radOnlyGroupAdmin_CheckedChanged" />
            </asp:PlaceHolder>
            <cms:CMSRadioButton ID="radOnlyRoles" runat="server" GroupName="board" AutoPostBack="True"
                OnCheckedChanged="radOnlyRoles_CheckedChanged" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="control-group-inline">
            <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <cms:CMSListBox ID="lstRoles" runat="server" CssClass="PermissionsListBox" SelectionMode="Multiple"
                        DataTextField="RoleDisplayName" DataValueField="RoleID" Rows="12" />
                </ContentTemplate>
            </cms:CMSUpdatePanel>
            <div class="btns-vertical">
                <cms:AddRoles ID="addRoles" runat="server" />
                <cms:CMSButton ID="btnRemoveRole" runat="server" Text="" ButtonStyle="Default" OnClick="btnRemoveRole_Click" />
            </div>
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnOk" runat="server" Text="" OnClick="btnOk_Click"
                EnableViewState="false" ResourceString="general.ok" />
        </div>
    </div>
</div>
