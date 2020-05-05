<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSAdminControls_UI_System_RequireScript"  Codebehind="RequireScript.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>

<div class="RequireScript" id="scriptCheck">
    <div class="PageBody">
        <asp:Panel runat="server" ID="pnlTitle" CssClass="PageHeader">
            <cms:PageTitle ID="ScriptTitle" runat="server" />
        </asp:Panel>
        <asp:Panel ID="PanelContent" runat="server" CssClass="PageContent">
            <asp:Label ID="lblInfo" runat="server" />
            <br />
            <br />
            <cms:CMSButton runat="server" ButtonStyle="Primary" OnClientClick="HideCheck(); return false;" ID="btnContinue" />
        </asp:Panel>
    </div>
</div>

<script type="text/javascript">
//<![CDATA[
    function HideCheck()
    {
        document.getElementById('scriptCheck').style.display = 'none';
    }
    
    HideCheck();
//]]>
</script>

