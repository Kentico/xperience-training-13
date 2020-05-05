<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_FolderActions"  Codebehind="FolderActions.ascx.cs" %>
<cms:CMSUpdatePanel ID="pnlUpdateSelectors" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="tree-actions-panel">
            <div class="tree-actions">
                <asp:PlaceHolder ID="plcActions" runat="server">
                    <cms:CMSAccessibleButton runat="server" ID="btnAdd" IconCssClass="icon-plus" IconOnly="true" EnableViewState="false" />
                    <cms:CMSAccessibleButton runat="server" ID="btnDelete" IconCssClass="icon-bin" IconOnly="true" EnableViewState="false" />
                    <cms:CMSAccessibleButton runat="server" ID="btnCopy" IconCssClass="icon-doc-copy" IconOnly="true" EnableViewState="false" />
                    <cms:CMSAccessibleButton runat="server" ID="btnMove" IconCssClass="icon-doc-move" IconOnly="true" EnableViewState="false" />
                </asp:PlaceHolder>
            </div>
        </div>
        <asp:Literal ID="ltlScrip" runat="server" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
