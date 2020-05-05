<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Pages_Users_General_User_WaitingForApproval"
    Title="Untitled Page" Theme="Default"  Codebehind="User_WaitingForApproval.aspx.cs" EnableEventValidation="false" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlUsers" runat="server" CssClass="UsersList">
        <asp:HiddenField ID="hdnReason" runat="server" />
        <asp:HiddenField ID="hdnSendEmail" runat="server" />
        <asp:HiddenField ID="hdnConfirmDelete" runat="server" />
        <asp:HiddenField ID="hdnUser" runat="server" />
        <cms:UniGrid ID="gridElem" runat="server" GridName="../User_List_Approval.xml" OrderBy="UserName"
            Columns="UserID, UserName, FullName, Email, UserNickName, UserCreated, UserEnabled"
            IsLiveSite="false" ShowObjectMenu="false" />
        <asp:Literal ID="ltlScript" runat="server" />
        <asp:Button ID="btnUpdate" runat="server" Text="Button" CssClass="HiddenButton"
            OnClick="btnUpdate_Click" />
    </asp:Panel>
</asp:Content>
