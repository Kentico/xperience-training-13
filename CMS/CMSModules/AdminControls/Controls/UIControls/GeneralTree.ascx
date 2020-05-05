<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/GeneralTree.ascx.cs" Inherits="CMSModules_AdminControls_Controls_UIControls_GeneralTree" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/UniTree.ascx" TagName="UniTree" TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlMain">
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="paneMenu" runat="server" Direction="North" RenderAs="Div" SpacingOpen="0" PaneClass="ui-layout-pane-visible">
                <Template>
                    <div class="tree-buttons-panel">
                        <cms:CMSMoreOptionsButton ID="btnAdd" runat="server" />
                        <cms:CMSAccessibleButton runat="server" ID="btnDelete" IconCssClass="icon-bin" IconOnly="true" />
                        <cms:CMSAccessibleButton runat="server" ID="btnExport" IconCssClass="icon-arrow-right-rect" IconOnly="true" />
                        <cms:CMSAccessibleButton runat="server" ID="btnClone" IconCssClass="icon-doc-copy" IconOnly="true" />
                    </div>
                </Template>
            </cms:UILayoutPane>
            <cms:UILayoutPane ID="paneTree" runat="server" Direction="Center" RenderAs="Div" PaneClass="ContentTreeArea" SpacingOpen="0">
                <Template>
                    <div class="TreeAreaTree">
                        <cms:MessagesPlaceHolder runat="server" ID="pnlMessages" />
                        <cms:UniTree runat="server" ShortID="t" ID="treeElem" CollapseAll="false" GeneralIDs="true" MultipleRoots="true" />
                    </div>
                </Template>
            </cms:UILayoutPane>
        </Panes>
    </cms:UILayout>
</asp:Panel>
