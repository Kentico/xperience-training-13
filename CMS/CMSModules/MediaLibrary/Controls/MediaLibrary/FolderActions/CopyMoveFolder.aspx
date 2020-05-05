<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_CopyMoveFolder"
     Codebehind="CopyMoveFolder.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Media Library Copy Move</title>

    <script type="text/javascript">
        //<![CDATA[
        // Initialize gray overlay in dialog
        function InitializeLog() {
            var content = window.GetTop().frames['selectFolderContent'];
            var footer = window.GetTop().frames['selectFolderFooter'];
            if (content != null) {
                var header = content.document.getElementById('selectFolder_pnlBody');
                if (header != null) {
                    var headerOverlay = content.document.createElement('DIV');
                    headerOverlay.id = 'headerOverlay';
                    headerOverlay.style.zIndex = '2500';
                    headerOverlay.style.height = header.offsetHeight + 'px';
                    headerOverlay.className = 'async-log-bg';
                    content.document.body.insertBefore(headerOverlay, content.document.body.firstChild);
                }
            }
            if (footer != null) {
                var footerOverlay = footer.document.createElement('DIV');
                footerOverlay.id = 'footerOverlay';
                footerOverlay.style.zIndex = '2500';
                footerOverlay.className = 'async-log-bg';
                footer.document.body.insertBefore(footerOverlay, footer.document.body.firstChild);
            }
            if (window.parent.expandIframe) {
                window.parent.expandIframe();
            }
        }

        // Remove gray overlay in dialog
        function DestroyLog() {
            var content = window.GetTop().frames['selectFolderContent'];
            var footer = window.GetTop().frames['selectFolderFooter'];
            if (content != null) {
                var header = content.document.getElementById('selectFolder_pnlBody');
                if (header != null) {
                    var headerOverlay = content.document.getElementById('headerOverlay');
                    if (headerOverlay != null) {
                        content.document.body.removeChild(headerOverlay);
                    }
                }
            }
            if (footer != null) {
                var footerOverlay = footer.document.getElementById('footerOverlay');
                if (footerOverlay != null) {
                    footer.document.body.removeChild(footerOverlay);
                }
            }
            if (window.parent.collapseIframe) {
                window.parent.collapseIframe();
            }
        }
        //]]>
    </script>

</head>
<body class="<%=mBodyClass%>">
    <form id="form1" runat="server">
        <div>
            <asp:Panel runat="server" ID="pnlLog" Visible="false">
                <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="MediaLibrary" />
            </asp:Panel>
            <div id="ContentDiv">
                <asp:Panel runat="server" ID="pnlEmpty" Visible="true" CssClass="DialogInfoArea">
                    <asp:Label runat="server" ID="lblEmpty" EnableViewState="false" />
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlInfo" CssClass="DialogInfoArea">
                    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblFolderName" runat="server" CssClass="control-label" DisplayColon="true"
                                    ResourceString="media.folder.targetfolder" EnableViewState="false" />
                            </div>
                            <div class="editing-form-value-cell">
                                <asp:Label ID="lblFolder" runat="server" CssClass="form-control-text" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblFilesToCopy" runat="server" CssClass="control-label" DisplayColon="true"
                                    ResourceString="media.folder.filestocopy" EnableViewState="false" />
                            </div>
                            <div class="editing-form-value-cell">
                                <div class="form-control vertical-scrollable-list">
                                    <asp:Label ID="lblFileList" runat="server" CssClass="FieldLabel" EnableViewState="false" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
            </div>
        </div>
    </form>
</body>
</html>
