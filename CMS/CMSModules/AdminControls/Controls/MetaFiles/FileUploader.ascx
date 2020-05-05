<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_AdminControls_Controls_MetaFiles_FileUploader"  Codebehind="FileUploader.ascx.cs" %>

<script type="text/javascript">
<!--
    // Puts the uploaders to the correct order
    function reorderUploaders() {

        var parentNode = null;
        var nodes = new Array();

        // Save the array of nodes in DOM model to array
        for (i = 0; i < uploadersVisible.length; i++) {
            var node = document.getElementById(uploadersVisible[i]);
            if (node != null) {
                nodes[i] = node;
                parentNode = node.parentNode;
            }
        }

        // Remove visible nodes and append them again, but in correct order
        if (parentNode != null) {
            for (i = 0; i < nodes.length; i++) {
                parentNode.removeChild(nodes[i]);
                parentNode.appendChild(nodes[i]);
            }
        }

        // Set correct addlink text
        if ((addLinkElem != null) && (addLinkElemPanel != null)) {
            if (uploadersAvailable.length == 0) {
                addLinkElemPanel.style.display = 'none';
            } else if (uploadersVisible.length == 0) {
                addLinkElemPanel.style.display = 'block';
                addLinkElem.innerHTML = strAttachFile;
            } else {
                addLinkElemPanel.style.display = 'block';
                addLinkElem.innerHTML = strAddAnotherFile;
            }
        }
    }

    // Displays new uploader if any available
    function addFile() {

        // Get available uploader
        if (uploadersAvailable.length > 0) {
            var uploader = uploadersAvailable.pop();
            uploadersVisible.push(uploader);

            // Get the added panel and set it's visiblility to true
            var panel = document.getElementById(uploader);
            if (panel != null) {
                panel.style.display = 'block';
            }

            reorderUploaders();
        }
    }


    // Hides the specified uplaoder panel
    function removeFile(pnlUploaderPanelId, uploaderId) {

        // Remove from visible collection
        uploadersVisible.splice(pnlUploaderPanelId, 1);

        // Add to available collection
        uploadersAvailable.push(pnlUploaderPanelId);

        // Hide the panel
        var uploaderPanel = document.getElementById(pnlUploaderPanelId);
        if (uploaderPanel != null) {
            uploaderPanel.style.display = 'none';
        }

        // Remove the file from uploader
        var uploader = document.getElementById(uploaderId);
        if (uploader != null) {

            // FF
            uploader.value = '';

            // IE
            if (uploader.value != '') {
                uploader.outerHTML = uploader.outerHTML;
            }

        }

        reorderUploaders();
    }
-->
</script>

<asp:Panel ID="pnlUploaders" runat="server" CssClass="UploaderUploaders" EnableViewState="false" />
<asp:Panel ID="pnlAdd" runat="server" CssClass="UploaderAddLink" EnableViewState="false">
    <cms:CMSIcon runat="server" ID="imgAdd" EnableViewState="false" />
    <asp:HyperLink ID="lnkAdd" runat="server" CssClass="NewItemLink" EnableViewState="false" />
</asp:Panel>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
