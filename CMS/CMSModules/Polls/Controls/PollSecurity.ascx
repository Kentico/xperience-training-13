<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Controls_PollSecurity"  Codebehind="PollSecurity.ascx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Roles/securityAddRoles.ascx" TagName="AddRoles" TagPrefix="cms" %>

<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" ID="lblTitle" Level="4" CssClass="listing-title" ResourceString="Poll_Security.Title"
            EnableViewState="false" Visible="true" />
        <div class="form-group">
            <div class="radio-list-vertical">
                <cms:CMSRadioButton ID="radAllUsers" runat="server" GroupName="polls" AutoPostBack="True"
                    ResourceString="Poll_Security.AllUsers" OnCheckedChanged="radAllUsers_CheckedChanged" />

                <cms:CMSRadioButton ID="radOnlyUsers" runat="server" GroupName="polls" AutoPostBack="True"
                    ResourceString="Poll_Security.OnlyUsers" OnCheckedChanged="radOnlyUsers_CheckedChanged" />

                <asp:PlaceHolder ID="plcGroupMembers" runat="server" Visible="false">
                    <cms:CMSRadioButton ID="radGroupMembers" runat="server" GroupName="polls" AutoPostBack="True"
                        ResourceString="Poll_Security.OnlyGroupMembers" OnCheckedChanged="radGroupMembers_CheckedChanged" />
                </asp:PlaceHolder>
                <cms:CMSRadioButton ID="radOnlyRoles" runat="server" GroupName="polls" AutoPostBack="True"
                    ResourceString="Poll_Security.OnlyRoles" OnCheckedChanged="radOnlyRoles_CheckedChanged" />
            </div>
        </div>
        <div class="form-group">
            <div class="control-group-inline">
                <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cms:CMSListBox ID="lstRoles" runat="server" CssClass="PermissionsListBox" SelectionMode="Multiple" Rows="12" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
                <div class="btns-vertical">
                    <cms:AddRoles ID="addRoles" runat="server" />
                    <cms:LocalizedButton ID="btnRemoveRole" runat="server" ButtonStyle="Default" ResourceString="general.remove"
                        OnClick="btnRemoveRole_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnOk" runat="server" Text="" ResourceString="general.ok"
                OnClick="btnOk_Click" />
        </div>
    </div>
</asp:Panel>