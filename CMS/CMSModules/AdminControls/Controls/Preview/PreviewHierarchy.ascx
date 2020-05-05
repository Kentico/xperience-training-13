<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="PreviewHierarchy.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Preview_PreviewHierarchy" %>

<script type="text/javascript">
    function CM_FullScreen(isFullScreen, elemsrc) {
        PreviewEditorFullScreen(isFullScreen, elemsrc, '<%=PaneContent.ClientID %>');
    }
</script>
<cms:UILayout runat="server" ID="mainLayout" OnResizeEndScript="LayoutResized">
    <Panes>
        <cms:UILayoutPane ID="paneTitle" runat="server" Direction="North" Size="48" SpacingClosed="0" SpacingOpen="0" UseUpdatePanel="false" />
        <cms:UILayoutPane ID="mainPane" runat="server" Direction="Center" UseUpdatePanel="false">
            <Template>
                <cms:UILayout runat="server" ID="layoutElem" OnResizeEndScript="LayoutResized">
                    <Panes>
                        <cms:UILayoutPane ID="paneContent" runat="server" Direction="North" RenderAs="Div"
                            UseUpdatePanel="false" Size="50%" PaneClass="PreviewContentMain scroll-area" Visible="true" />
                        <cms:UILayoutPane ID="paneContentMain" runat="server" Direction="Center" RenderAs="Div"
                            MaskContents="true" UseUpdatePanel="false" ResizerClass="header-panel">
                            <Template>
                                <cms:UILayout runat="server" ID="layoutElem">
                                    <Panes>
                                        <cms:UILayoutPane ID="panePreview" runat="server" Direction="Center" RenderAs="Iframe"
                                            UseUpdatePanel="false" PaneClass="ui-layout-pane-scroll" Visible="true" Resizable="false" />
                                    </Panes>
                                </cms:UILayout>
                            </Template>
                        </cms:UILayoutPane>
                        <cms:UILayoutPane ID="paneFooter" runat="server" Direction="South" RenderAs="Div"
                            Size="64" Closable="false" FrameBorder="false" ControlPath="~/CMSModules/AdminControls/Controls/Preview/PreviewFooter.ascx"
                            Resizable="false" />
                    </Panes>
                </cms:UILayout>
            </Template>
        </cms:UILayoutPane>
    </Panes>
</cms:UILayout>
<asp:Button ID="btnHidden" CssClass="HiddenButton" runat="server" />