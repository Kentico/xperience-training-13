<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_DebugAll"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - All"
     Codebehind="System_DebugAll.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcActions" runat="server">
    <div class="header-actions-container">
        <cms:CMSButton runat="server" ID="btnClear" OnClick="btnClear_Click" ButtonStyle="Default"
            EnableViewState="false" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="FloatLeft">
        <cms:CMSCheckBox runat="server" ID="chkCompleteContext" ResourceString="Debug.ShowCompleteContext"
            AutoPostBack="true" />
    </div>
    <br />
    <br />
    <cms:CMSPlaceHolder runat="server" ID="plcLogs" EnableViewState="false" />
</asp:Content>

