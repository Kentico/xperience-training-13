<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ContentItemCodeGenerator.ascx.cs" Inherits="CMSAdminControls_UI_Development_Generators_ContentItemCodeGenerator" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagName="FileSystemSelector" TagPrefix="cms" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="Classes.Code.SavePath" DisplayColon="True" AssociatedControlID="ucSavePath" />
        </div>
        <div class="editing-form-value-cell">
            <cms:FileSystemSelector runat="server" ID="ucSavePath" ShowFolders="True" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:LocalizedButton runat="server" ID="btnSaveCode" ButtonStyle="Primary" ResourceString="Classes.Code.SaveCode" />
        </div>
    </div>
</div>

<asp:Panel ID="pnlGeneratedCode" runat="server" CssClass="layout-2-columns">
    <asp:Panel ID="pnlItemCode" runat="server" CssClass="col-50">
        <cms:LocalizedHeading runat="server" ID="headItemCode" Level="4" EnableViewState="false" ResourceString="Classes.Code.ItemCode" DisplayColon="True" />
        <cms:ExtendedTextArea ID="txtItemCode" runat="server" ReadOnly="true" EditorMode="Advanced" Language="CSharp" Height="450px" />
    </asp:Panel>
    <asp:Panel ID="pnlProviderCode" runat="server" CssClass="col-50">
        <cms:LocalizedHeading runat="server" ID="headProviderCode" Level="4" EnableViewState="false" ResourceString="Classes.Code.ProviderCode" DisplayColon="True" />
        <cms:ExtendedTextArea ID="txtProviderCode" runat="server" ReadOnly="true" EditorMode="Advanced" Language="CSharp" Height="450px" />
    </asp:Panel>
</asp:Panel>