<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ImageEditorInnerPage.aspx.cs"
    Inherits="CMSAdminControls_ImageEditor_ImageEditorInnerPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Image editor content</title>
</head>
<body class="<%=mBodyClass%> image-editor-inner-page">
    <form id="form1" runat="server">
    <asp:Image ID="imgContent" runat="server" EnableViewState="false" CssClass="image-editor-image" />
    </form>

    <script type="text/javascript" language="javascript">
        //<![CDATA[
        var jcrop_api = null;
        $cmsj(window).bind('load',
            function() {
                // Initialize crop after image is loaded
                if (window.parent.initializeCrop) {
                    initCrop();
                }
            });
        //]]>
    </script>

</body>
</html>
