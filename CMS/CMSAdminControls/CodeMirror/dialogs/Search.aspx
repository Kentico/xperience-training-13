<%@ Page Title="Search" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" AutoEventWireup="true"  Codebehind="Search.aspx.cs" Inherits="CMSAdminControls_CodeMirror_dialogs_Search" %>

<asp:Content ID="plcContentContent" ContentPlaceHolderID="plcContent" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSearchFor" runat="server" EnableViewState="false" ResourceString="cmssearchdialog.searchfor"
                    AssociatedControlID="txtSearchFor" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSearchFor" runat="server" EnableViewState="false" Text="" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblMatchCase" runat="server" EnableViewState="false" ResourceString="general.matchcase"
                    DisplayColon="true" AssociatedControlID="chkMatchCase" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkMatchCase" runat="server" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="plcFooterContent" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnSearch" runat="server" EnableViewState="false" ButtonStyle="Primary"
        OnClientClick="findText(); return false;" ResourceString="general.search" />
    <script type="text/javascript" src="SearchReplace.js"></script>
    <script type="text/javascript">
        var txtSearch = document.getElementById('<%=txtSearchFor.ClientID%>');
        var chkMatchCase = document.getElementById('<%=chkMatchCase.ClientID%>');
        var btnSearch = document.getElementById('<%=btnSearch.ClientID%>');

        focusOnTextBox('<%=txtSearchFor.ClientID%>');
    </script>
</asp:Content>
