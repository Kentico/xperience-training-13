<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Groups - Delete group" Inherits="CMSModules_Groups_Tools_Group_Delete"
    Theme="Default"  Codebehind="Group_Delete.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:LocalizedLabel ID="lblMsg" runat="server" ResourceString="group.deletemessage" /><br />
    <br />
    <cms:CMSCheckBox ID="chkDeleteAll" runat="server"
        EnableViewState="false" Checked="true" /><br />
    <br />
    <br />
    <cms:LocalizedButton ID="btnDelete" runat="server" ButtonStyle="Default" ResourceString="general.yes"
        EnableViewState="false" />
    <cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Default" ResourceString="general.no"
        EnableViewState="false" />
</asp:Content>
