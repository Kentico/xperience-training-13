<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    Theme="Default" Inherits="CMSModules_Groups_CMSPages_InviteToGroup"  Codebehind="InviteToGroup.aspx.cs" %>

<%@ Register Src="~/CMSModules/Groups/Controls/GroupInvite.ascx" TagName="GroupInvite"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" runat="server" ContentPlaceHolderID="plcContent">
    <cms:GroupInvite ID="groupInviteElem" IsLiveSite="true" runat="server" DisplayButtons="false" />
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton runat="server" ButtonStyle="Primary" ID="btnInvite" EnableViewState="false" /><cms:LocalizedButton
            ButtonStyle="Primary" ID="btnCancel" OnClientClick="Close();" runat="server"
            ResourceString="General.cancel" EnableViewState="false" />
    </div>
</asp:Content>
