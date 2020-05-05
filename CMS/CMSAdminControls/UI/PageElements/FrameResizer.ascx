<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_PageElements_FrameResizer"
     Codebehind="FrameResizer.ascx.cs" %>
<asp:PlaceHolder runat="server" ID="plcStandard" EnableViewState="false">
    <asp:Panel runat="server" ID="pnlResizer" EnableViewState="False">
        <img src="<%=minimizeUrl%>" alt="" id="imgMinimize" onclick="Minimize();" onmouseover="document.body.style.cursor='pointer';"
            onmouseout="document.body.style.cursor='auto';" />
        <img src="<%=maximizeUrl%>" alt="" id="imgMaximize" onclick="Maximize();" style="display: none;"
            onmouseover="document.body.style.cursor='pointer';" onmouseout="document.body.style.cursor='auto';" />
    </asp:Panel>
    <div id="resizerBorder" class="<%=Direction + "ResizerBorder"%>" style="display: none;">
        &nbsp;</div>
    <input type="hidden" id="originalSize" value="<%=HTMLHelper.EncodeForHtmlAttribute(originalSize)%>" />
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="plcAll" EnableViewState="false" Visible="false">
    <a class="<%=Direction + "AllFrameResizer"%>" href="#"><span class="ResizerContent">
        <img src="<%=minimizeUrl%>" alt="" id="imgMinimizeAll" onclick="MinimizeAll();" onmouseover="document.body.style.cursor='pointer';"
            onmouseout="document.body.style.cursor='auto';" />
        <img src="<%=maximizeUrl%>" alt="" id="imgMaximizeAll" onclick="MaximizeAll();" style="display: none;"
            onmouseover="document.body.style.cursor='pointer';" onmouseout="document.body.style.cursor='auto';" />
    </span></a></asp:PlaceHolder>
