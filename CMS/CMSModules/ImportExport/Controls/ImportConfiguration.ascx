<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ImportConfiguration"
     Codebehind="ImportConfiguration.ascx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>

<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:MessagesPlaceHolder runat="server" ID="pnlMessages" />
        <div class="content-block-50 control-group-inline">
            <div class="upload-control-button">
                <cms:DirectFileUploader ID="newImportPackage" runat="server" />
            </div>
            <cms:LocalizedButton runat="server" ID="btnDelete" ResourceString="importconfiguration.deletepackage" OnClick="btnDelete_Click" ButtonStyle="Default" />
            <cms:LocalizedButton runat="server" ID="btnRefresh" ResourceString="general.refresh" OnClick="btnRefresh_Click" ButtonStyle="Default" />
        </div>
        <div class="content-block-50">
            <cms:CMSListBox runat="server" ID="lstImports" CssClass="ContentListBoxLow" Enabled="false" Rows="7" />
        </div>
        <div class="content-block-50">
            <div class="radio-list-vertical">
                <cms:CMSRadioButton ID="radAll" runat="server" GroupName="Import" Checked="true" CssClass="RadioImport" />
                <cms:CMSRadioButton ID="radNew" runat="server" GroupName="Import" CssClass="RadioImport" />
            </div>
        </div>
        <asp:Literal ID="ltlScripts" runat="server" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>