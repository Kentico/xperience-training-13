<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Properties_DocCopyMoveProperties"  Codebehind="DocCopyMoveProperties.ascx.cs" %>
<asp:PlaceHolder ID="plcPropContent" runat="server">
    <div style="width: 100%; height: 500px; position: relative;" id="iframeDiv">
        <iframe id="innerFrame" runat="server" frameborder="0" marginheight="0" marginwidth="0"
            style="position: absolute; z-index: 5000; width: 100%; height: 100%;" enableviewstate="false" />
    </div>

    <script type="text/javascript" language="javascript">
        //<![CDATA[

        function expandIframe() {
            $cmsj('#iframeDiv').css('position', 'static');
            $cmsj('.DialogRightBlock').css('position', 'static');
            $cmsj('.DialogResizerVLine').css('visibility', 'hidden');
            var iFrame = $cmsj('#iframeDiv > iframe').css({ 'top': '0px', 'left': '0px', 'right': '0px', 'bottom': '0px' });
            var generalObj = iFrame[0].contentWindow.document.getElementById('ContentDiv');
            if (generalObj != null) {
                generalObj.style.display = 'none';
            }
        }

        function collapseIframe() {
            $cmsj('#iframeDiv').css('position', 'relative');
            $cmsj('.DialogRightBlock').css('position', 'relative');
            $cmsj('.DialogResizerVLine').css('visibility', 'visible');
            var iFrame = $cmsj('#iframeDiv > iframe').css({ 'top': '', 'left': '', 'right': '', 'bottom': '' });
            var generalObj = iFrame[0].contentWindow.document.getElementById('ContentDiv');
            if (generalObj != null) {
                generalObj.style.display = 'block';
            }
        }
        //]]>
    </script>

</asp:PlaceHolder>
