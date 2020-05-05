<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_Tasks_Tree"
    EnableEventValidation="false" Theme="Default"  Codebehind="Tree.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/ContentTree.ascx" TagName="ContentTree" TagPrefix="cms" %>
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
                    <cms:ContentTree ID="treeContent" runat="server" IsLiveSite="false" />
                </div>
            </div>
            <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        </asp:Panel>

        <script type="text/javascript">
            //<![CDATA[
            var currentNode = document.getElementById(currentNodeId);

            // Refresh node action
            function RefreshNode(nodeId, selectNodeId) {
                if (selectNodeId == null) {
                    selectNodeId = currentNodeId;
                }
                document.location.replace(treeUrl + "?expandnodeid=" + nodeId + "&stagingnodeid=" + selectNodeId);
            }

            // Highlight selected node
            function SelectTree(nodeId) {
                if ((currentNode != null) && (nodeId != null)) {
                    currentNode.className = 'ContentTreeItem';
                }

                if (nodeId != null) {
                    currentNode = document.getElementById(nodeId);
                    if (currentNode != null) {
                        currentNode.className = 'ContentTreeSelectedItem';
                    }
                }
            }

            // Select node action
            function SelectNode(nodeId, selectMore) {
                SelectTree(nodeId);
                var selectDocuments = parent.frames['tasksContent'].selectDocuments;
                var completeObj = parent.frames['tasksContent'].document.getElementById('pnlComplete');
                if (selectDocuments || selectMore) {
                    if (IsLogContentChangesEnabled()) {
                        parent.frames['tasksContent'].SelectDocNode(parent.frames['tasksContent'].currentServerId, nodeId);
                        parent.frames['tasksContent'].selectDocuments = true;
                    }
                    if (completeObj != null) {
                        completeObj.style.display = 'none';
                    }
                }
                else {
                    if (IsLogContentChangesEnabled()) {
                        parent.frames['tasksContent'].SelectNode(parent.frames['tasksContent'].currentServerId, nodeId);
                    }
                    if (completeObj != null) {
                        completeObj.style.display = 'block';
                    }
                }
                currentNodeId = nodeId;
            }

            function IsLogContentChangesEnabled() {
                return parent.frames['tasksContent'].SelectDocNode && parent.frames['tasksContent'].SelectNode;
            }
            //]]>
        </script>

    </form>
</body>
</html>
