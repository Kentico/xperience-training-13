<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ExportSettingsPanel"
     Codebehind="ExportSettingsPanel.ascx.cs" %>

<script type="text/javascript">
    //<![CDATA[
    function CheckChange() {
        for (i = 0; i < ex_g_childIDs.length; i++) {
            var child = document.getElementById(ex_g_childIDs[i]);
            if (child != null) {
                child.disabled = !ex_g_parent.checked;
                child.checked = ex_g_parent.checked;
            }
        }
    }

    function InitCheckboxes() {
        if (!ex_g_parent.checked) {
            for (i = 0; i < ex_g_childIDs.length; i++) {
                var child = document.getElementById(ex_g_childIDs[i]);
                if (child != null) {
                    child.disabled = true;
                }
            }
        }
    }
    //]]>
</script>
<div class="wizard-section">
    <asp:Panel runat="server" ID="pnlCheck" CssClass="content-block-50" EnableViewState="false">
        <cms:LocalizedHeading runat="server" ID="headExportSettings" ResourceString="ExportObjects.Settings" Level="4" EnableViewState="true" />
        <div class="form-horizontal">
            <div class="checkbox-list-vertical content-block-50">
                <cms:CMSCheckBox ID="chkExportTasks" runat="server" />
                <cms:CMSCheckBox ID="chkCopyFiles" runat="server" />
                <div class="selector-subitem">
                    <div class="checkbox-list-vertical">
                        <cms:CMSCheckBox ID="chkCopyGlobalFiles" runat="server" />
                        <cms:CMSCheckBox ID="chkCopyAssemblies" runat="server" />
                        <asp:PlaceHolder ID="plcSiteFiles" runat="server">
                            <cms:CMSCheckBox ID="chkCopySiteFiles" runat="server" />
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</div>
<asp:Literal ID="ltlScript" EnableViewState="false" runat="Server" />