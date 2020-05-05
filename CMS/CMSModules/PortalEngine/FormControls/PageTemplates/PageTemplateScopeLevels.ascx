<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_PortalEngine_FormControls_PageTemplates_PageTemplateScopeLevels"  Codebehind="PageTemplateScopeLevels.ascx.cs" %>
<%@ Register Src="~/CMSModules/PortalEngine/FormControls/PageTemplates/LevelTree.ascx" TagPrefix="cms"
    TagName="LevelTree" %>

<script type="text/javascript">
    //<![CDATA[
    function ShowOrHideTree(show) {
        if (show) {
            document.getElementById('treeSpan').style.display = "inline";
        }
        else {
            document.getElementById('treeSpan').style.display = "none";
        }
    }
    //]]>
</script>

<cms:CMSRadioButton runat="server" ID="radAllowAll" GroupName="Allow" />
<cms:CMSRadioButton runat="server" ID="radSelect" GroupName="Allow" />
<div id="treeSpan" style="display: none;">
    <cms:LevelTree runat="server" ID="treeElem" />
</div>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />