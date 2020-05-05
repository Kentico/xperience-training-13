<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="cms_user.ascx.cs" Inherits="CMSModules_ImportExport_Controls_Import_cms_user" %>

<script type="text/javascript">
    //<![CDATA[
    function CheckChange() {
        if (dashboardChck2 != null) {
            dashboardChck2.disabled = !dashboardChck1.checked;
            dashboardChck2.checked = false;
        }
    }

    function InitCheckboxes() {
        if (!dashboardChck1.checked && (dashboardChck2 != null)) {
            dashboardChck2.disabled = true;
        }
    }
    //]]>
</script>
<asp:Panel runat="server" ID="pnlCheck" CssClass="wizard-section">
    <div class="checkbox-list-vertical">
        <cms:CMSCheckBox ID="chkObject" runat="server" />
        <cms:CMSCheckBox ID="chkSiteObjects" runat="server" />
    </div>
</asp:Panel>
<asp:Literal ID="ltlScript" EnableViewState="false" runat="Server" />