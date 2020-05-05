<%@ Page Title="Replace" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" AutoEventWireup="true"  Codebehind="Replace.aspx.cs" Inherits="CMSAdminControls_CodeMirror_dialogs_Replace" %>

<asp:Content ID="plcContentContent" ContentPlaceHolderID="plcContent" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFindWhat" runat="server" EnableViewState="false" ResourceString="cmsreplacedialog.findwhat"
                    DisplayColon="true" AssociatedControlID="txtFindWhat" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtFindWhat" EnableViewState="false" Text="" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblReplaceWith" runat="server" EnableViewState="false" ResourceString="cmsreplacedialog.replacewith"
                    DisplayColon="true" AssociatedControlID="txtReplaceWith" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtReplaceWith" EnableViewState="false" Text="" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblReplaceMode" runat="server" EnableViewState="false" ResourceString="cmsreplacedialog.replacemode"
                    DisplayColon="true" AssociatedControlID="rblReplaceMode" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSRadioButtonList ID="rblReplaceMode" runat="server" EnableViewState="false"
                    RepeatDirection="Horizontal" UseResourceStrings="true">
                    <asp:ListItem Text="cmsreplacedialog.replacemode.replace" Value="down" Selected="True" />
                    <asp:ListItem Text="cmsreplacedialog.replacemode.replaceall" Value="up" />
                </cms:CMSRadioButtonList>
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
    <cms:LocalizedButton ID="btnFind" runat="server" EnableViewState="false" ButtonStyle="Primary"
        OnClientClick="findText(); return false;" ResourceString="general.find" />
    <cms:LocalizedButton ID="btnReplace" runat="server" EnableViewState="false" ButtonStyle="Primary"
        OnClientClick="replaceText(); return false;" ResourceString="general.replace" />
    <script type="text/javascript" src="SearchReplace.js"></script>
    <script type="text/javascript">
        var txtReplace = document.getElementById('<%=txtReplaceWith.ClientID%>');
        var rbAll = document.forms[0]['<%=rblReplaceMode.UniqueID%>'][1];
        var txtSearch = document.getElementById('<%=txtFindWhat.ClientID%>');
        var chkMatchCase = document.getElementById('<%=chkMatchCase.ClientID%>');
        var btnSearch = document.getElementById('<%=btnFind.ClientID%>');

        focusOnTextBox('<%=txtFindWhat.ClientID%>');
    </script>
</asp:Content>
