<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Dashboard.ascx.cs" Inherits="CMSModules_Widgets_Controls_Dashboard" %>
<script type="text/javascript">
        //<![CDATA[
    window.useDraggedClass = false;
                //]]>
</script>
<asp:PlaceHolder runat="server" ID="plcManagers">
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
    <cms:ContextMenuPlaceHolder ID="plcCtx" runat="server" />
    <cms:CMSPortalManager ID="manPortal" runat="server" EnableViewState="false" />
    <asp:ScriptManager ID="manScript" runat="server" ScriptMode="Release"
        EnableViewState="false" />
</asp:PlaceHolder>
<div style="width: 100%" class="DashBoard">
    <cms:CMSPagePlaceholder ID="plc" runat="server" />
</div>
