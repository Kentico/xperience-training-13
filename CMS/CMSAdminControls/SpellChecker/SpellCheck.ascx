<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_SpellChecker_SpellCheck"
     Codebehind="SpellCheck.ascx.cs" %>
<asp:Panel ID="SpellingBody" runat="server">
    <asp:HiddenField ID="WordIndex" runat="server" Value="0" />
    <asp:HiddenField ID="CurrentText" runat="server" />
    <asp:HiddenField ID="IgnoreList" runat="server" />
    <asp:HiddenField ID="ReplaceKeyList" runat="server" />
    <asp:HiddenField ID="ReplaceValueList" runat="server" />
    <asp:HiddenField ID="ElementIndex" runat="server" Value="-1" />
    <asp:HiddenField ID="SpellMode" runat="server" Value="load" />
    <asp:Panel runat="server" ID="SuggestionForm" CssClass="form-horizontal">
        <asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel"
            Visible="false" />
        <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" EnableViewState="false" />
        <asp:Panel CssClass="control-group-inline" ID="pnlCurrentWord" runat="server">
            <asp:Label ID="CurrentWord" runat="server" CssClass="form-control-text" />
        </asp:Panel>
        <div class="control-group-inline form-group">
            <cms:CMSButton ID="IgnoreButton" OnClick="IgnoreButton_Click" runat="server" EnableViewState="False"
                Enabled="False" ButtonStyle="Default"></cms:CMSButton>
            <cms:CMSButton ID="IgnoreAllButton" OnClick="IgnoreAllButton_Click" runat="server"
                EnableViewState="False" Enabled="False" ButtonStyle="Default"></cms:CMSButton>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblChangeTo" runat="server" />
            </div>
            <div class="editing-form-value-cell control-group-inline">
                <cms:CMSTextBox ID="ReplacementWord" runat="server" EnableViewState="False" Enabled="False" CssClass="input-width-60" />
                <cms:LocalizedButton ID="btnAdd" OnClick="AddButton_Click" runat="server" EnableViewState="False"
                    Enabled="False" ButtonStyle="Default" ResourceString="general.add" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblSuggestions" runat="server" />
            </div>
            <div class="editing-form-value-cell  control-group-inline">
                <cms:CMSListBox ID="Suggestions" runat="server" EnableViewState="False" Enabled="False"
                    CssClass="input-width-60" Rows="5" />
                <div class="btns-vertical">
                    <cms:CMSButton ID="ReplaceButton" OnClick="ReplaceButton_Click" runat="server" EnableViewState="False"
                        Enabled="False" ButtonStyle="Default"></cms:CMSButton>
                    <cms:CMSButton ID="ReplaceAllButton" OnClick="ReplaceAllButton_Click" runat="server"
                        EnableViewState="False" Enabled="False" ButtonStyle="Default"></cms:CMSButton>
                    <cms:CMSButton ID="RemoveButton" OnClick="RemoveButton_Click" runat="server" EnableViewState="False"
                        Enabled="False" ButtonStyle="Default"></cms:CMSButton>
                </div>
            </div>
        </div>
        <div class="control-group-inline">
            <asp:Label ID="StatusText" runat="Server" CssClass="form-control-text" />
        </div>
    </asp:Panel>
</asp:Panel>
