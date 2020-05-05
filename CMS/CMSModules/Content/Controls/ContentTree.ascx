<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_ContentTree"
     Codebehind="ContentTree.ascx.cs" %>
<asp:Label runat="server" ID="lblError" ForeColor="Red" EnableViewState="false" />
<asp:PlaceHolder runat="server" ID="plcDrag" EnableViewState="false">
    <asp:Literal runat="server" ID="ltlCaptureCueCtrlShift" EnableViewState="false" />
    <script type="text/javascript">
        //<![CDATA[
        var recursiveDragAndDrop = true;
        var dropAfter = true;
        var dragYOffset = 15;
        var dragKeepCopy = true;
        var leftCueOffset = 16;
        var captureCueCtrl = true;
        var tagDraggedElem = true;

        function OnDropNode(sender, e) {
            var item = e.get_droppedItem();
            var target = item.parentNode;

            var itemNodeId = item.id.substring(5);
            var targetNodeId = target.id.substring(7);

            if (/^[0-9]+$/.test(targetNodeId) && /^[0-9]+$/.test(itemNodeId)) {
                var copy = false;
                var link = false;
                if (window._event != null) {
                    copy = _event.ctrlKey;
                    link = (_event.ctrlKey && _event.shiftKey);
                }

                MoveNodeAsync(itemNodeId, targetNodeId, target.isOnLeft, copy, link);
            }
        }

        function DragActionDone(rvalue, context) {
            if ((rvalue != null) && (rvalue != '')) {
                setTimeout(rvalue, 0);
            }
        }
        
        //]]>
    </script>
    <asp:Panel runat="server" ID="pnlCue" CssClass="DDCue" EnableViewState="false">
        <div class="DDCueInside">
            <div class="MoveHere">
                <i aria-hidden="true" class="icon-doc-move"></i>
                <cms:LocalizedLabel runat="server" ID="lblMoveHere" ResourceString="ContentTree.MoveHere" />
            </div>
            <div class="CopyHere">
                <i aria-hidden="true" class="icon-doc-copy"></i>
                <cms:LocalizedLabel runat="server" ID="lblCopyHere" ResourceString="ContentTree.CopyHere" />
            </div>
            <div class="LinkHere">
                <i aria-hidden="true" class="icon-chain"></i>
                <cms:LocalizedLabel runat="server" ID="lblLinkHere" ResourceString="ContentTree.LinkHere" />
            </div>
            &nbsp;</div>
    </asp:Panel>
</asp:PlaceHolder>
<asp:Panel runat="server" ID="pnlTree" CssClass="ContentTree">
    <cms:UITreeView ID="treeElem" ShortID="t" runat="server" PopulateNodesFromClient="True"
        ShowLines="True" CssClass="TreePadding" OnTreeNodePopulate="treeElem_TreeNodePopulate"
        EnableViewState="False" />
</asp:Panel>
