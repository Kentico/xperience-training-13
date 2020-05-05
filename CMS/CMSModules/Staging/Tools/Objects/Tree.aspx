<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_Objects_Tree"
    EnableEventValidation="false" Theme="Default"  Codebehind="Tree.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Trees/ObjectTree.ascx" TagName="ObjectTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/TreeBorder.ascx" TagName="TreeBorder" TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Content - Tree</title>
</head>
<body class="TreeBody <%=mBodyClass%>">
    <form id="form1" runat="server">
        <asp:Panel runat="server" ID="pnlBody" CssClass="ContentTree">
            <cms:TreeBorder ID="borderElem" runat="server" FramesetName="colsFrameset" />
            <div class="TreeArea">
                <div class="TreeAreaTree">
                    <cms:ObjectTree ID="objectTree" runat="server" />
                </div>
            </div>
        </asp:Panel>

        <script type="text/javascript">
            //<![CDATA[
            var currentNode = document.getElementById('treeSelectedNode'),
                currentNodeId = "",
                currentSiteId = 0;

            // Select node action
            function SelectNode(nodeId, siteId, nodeElem) {
                if ((currentNode != null) && (nodeElem != null)) {
                    currentNode.className = 'ContentTreeItem';
                }

                if (parent.frames['tasksContent'].SelectNode) {
                    parent.frames['tasksContent'].SelectNode(parent.frames['tasksContent'].currentServerId, nodeId, siteId);
                }
                currentNodeId = nodeId;
                currentSiteId = siteId;

                if (nodeElem != null) {
                    currentNode = nodeElem;
                    if (currentNode != null) {
                        currentNode.className = 'ContentTreeSelectedItem';
                    }
                }
            }
            //]]>
        </script>
    </form>
</body>
</html>
