<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_Controls_WebParts_WebpartProperties"
     Codebehind="WebpartProperties.ascx.cs" %>

<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<div id="CMSHeaderDiv">
    <div id="CKToolbar" class="CKToolbar">
    </div>
</div>
<script type="text/javascript">
    //<![CDATA[
    var resizeInterval = setInterval('if (window.ResizeToolbar) { ResizeToolbar(); }', 300);
    //]]>
</script>
<asp:Panel runat="server" ID="pnlTab" CssClass="TabsPageContent">
    <asp:Panel runat="server" ID="pnlFormArea" CssClass="WebPartForm">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
        <cms:BasicForm runat="server" ID="form" HtmlAreaToolbarLocation="Out:CKToolbar" Enabled="true"
            DefaultCategoryName="{$general.default$}" AllowMacroEditing="true"
            IsLiveSite="false" MarkRequiredFields="true" />
        <br class="ClearBoth" />
        <cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="5" IsOpened="false" />
        <asp:Panel runat="server" ID="pnlExport">
            <cms:LocalizedButton runat="server" ID="lnkExport" ButtonStyle="Default" ResourceString="WebpartProperties.Export" />
            <cms:LocalizedButton runat="server" ID="lnkLoadDefaults" ButtonStyle="Default" ResourceString="webpartproperties.loaddefaults"
                OnClick="lnkLoadDefaults_Click" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
<asp:HiddenField ID="hdnIsNewWebPart" runat="server" />
<asp:HiddenField ID="hdnInstanceGUID" runat="server" />
<div id="CMSFooterDiv">
</div>
<script type="text/javascript">
    //<![CDATA[
    // cmsedit.js function override for CKEditor
    function SaveDocument() { }
    //]]>
</script>
