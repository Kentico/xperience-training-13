<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SystemTables_Controls_Views_SQLEdit"
     Codebehind="SQLEdit.ascx.cs" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:PlaceHolder ID="plcGenerate" runat="server" Visible="false">
    <cms:LocalizedButton ID="btnGenerate" runat="server" ButtonStyle="Default" OnClick="btnGenerate_Click"
        ResourceString="sysdev.views.generateskeleton" EnableViewState="false" />
    <br />
    <br />
</asp:PlaceHolder>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
    <ContentTemplate>
        <cms:LocalizedHeading ID="lblHeading" runat="server" Level="3" ResourceString = "sysdev.views.code" />
        <asp:Label ID="lblCreateLbl" runat="server" />
        <cms:CMSTextBox ID="txtObjName" runat="server" MaxLength="128" />
        <div class="control-group-inline">
            <cms:CMSCheckBox runat="server" ID="chkWithBinding" Visible="False" Text="WITH SCHEMABINDING" AutoPostBack="true" OnCheckedChanged="chkWithBinding_CheckedChanged" />
        </div>
        <br />
        <asp:PlaceHolder ID="plcParams" runat="server">
        <cms:ExtendedTextArea runat="server" ID="txtParams" EnableViewState="false" EditorMode="Advanced"
            Language="SQL" Height="100" Rows="7" Width="100%" />
        <br />
        </asp:PlaceHolder>
        <asp:Literal ID="lblBegin" runat="server" />
        <cms:ExtendedTextArea runat="server" ID="txtSQLText" EnableViewState="false" EditorMode="Advanced"
            Language="SQL" Width="100%" />
        <asp:Label ID="lblEnd" runat="server" />
        <br />
        <br />
        <asp:PlaceHolder runat="server" ID="plcIndexes" Visible="false">
            <cms:LocalizedHeading ID="lblIndexes" runat="server" ResourceString="sysdev.views.indexes" Level="3" />
            <cms:ExtendedTextArea runat="server" ID="txtIndexes" EnableViewState="false" EditorMode="Advanced"
                Language="SQL" Width="100%" />
        </asp:PlaceHolder>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<br />
<cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false" OnClick="btnOK_Click" />