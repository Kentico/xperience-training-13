<%@ Page Title="Search" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" AutoEventWireup="true"  Codebehind="EditSource.aspx.cs" Inherits="CMSAdminControls_CodeMirror_dialogs_EditSource" %>

<asp:Content ID="plcContentContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSTextArea runat="server" ID="txtSource" Rows="37" />

    <script type="text/javascript" src="SearchReplace.js"></script>
    <script type="text/javascript">
        var txtSource = document.getElementById('<%=txtSource.ClientID%>');

        setInterval('doResize();', 500);
        getSource();
        focusOnTextBox();
    </script>
</asp:Content>