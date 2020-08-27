<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_System_Debug_System_DebugViewState" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - ViewState"  Codebehind="System_DebugViewState.aspx.cs" %>


<asp:Content ContentPlaceHolderID="plcActions" runat="server">
    <div class="header-actions-container">
        <cms:CMSButton runat="server" ID="btnClear" OnClick="btnClear_Click" ButtonStyle="Default" EnableViewState="false" />
    </div>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="FloatLeft">
        <cms:CMSCheckBox runat="server" ID="chkOnlyDirty" ResourceString="DebugViewState.ShowOnlyDirty" AutoPostBack="true" Checked="true" />
    </div>
    <br/>
    <br/>
    <asp:PlaceHolder runat="server" ID="plcLogs" />
</asp:Content>

