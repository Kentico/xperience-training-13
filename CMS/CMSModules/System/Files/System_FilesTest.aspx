<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Files_System_FilesTest"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Administration - System - Test files"
     Codebehind="System_FilesTest.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSButton runat="server" ID="btnTest" ButtonStyle="Primary" OnClick="btnTest_Click"
        EnableViewState="false" />
    <br />
    <br />
    <asp:Label runat="server" ID="ltlInfo" EnableViewState="false" />
</asp:Content>
