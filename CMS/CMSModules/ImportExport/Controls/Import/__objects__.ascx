<%@ Control Language="C#" AutoEventWireup="false" Inherits="CMSModules_ImportExport_Controls_Import___objects__"
     Codebehind="__objects__.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>

<script type="text/javascript">
    //<![CDATA[
    function CheckChange() {
        for (i = 0; i < im_g_childIDs.length; i++) {
            var child = document.getElementById(im_g_childIDs[i]);
            if (child != null) {
                var name = im_g_childIDNames[i];
                if ((name == 'asbl') || (name == 'code')) {
                    child.checked = false;
                    if (im_g_isPrecompiled) {
                        child.disabled = true;
                    } else {
                        child.disabled = !im_g_parent.checked;
                    }
                }
                else {
                    child.checked = im_g_parent.checked;
                    child.disabled = !im_g_parent.checked;
                }
            }
        }
    }

    function InitCheckboxes() {
        if (!im_g_parent.checked) {
            for (i = 0; i < im_g_childIDs.length; i++) {
                var child = document.getElementById(im_g_childIDs[i]);
                if (child != null) {
                    child.disabled = true;
                }
            }
        }
    }
    //]]>
</script>

<asp:Panel runat="server" ID="pnlWarning" CssClass="wizard-section" Visible="false">
    <asp:Label ID="lblWarning" runat="server" EnableViewState="false" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlInfo" CssClass="wizard-section content-block-25">
    <cms:LocalizedLabel ID="lblInfo2" runat="server" EnableViewState="false" ResourceString="ImportObjects.Info2" />
    <cms:LocalizedLabel ID="lblInfo" runat="server" EnableViewState="false" ResourceString="ImportObjects.Info" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlSelection" CssClass="wizard-section content-block-25">
    <cms:LocalizedHeading ID="headSelection" runat="server" EnableViewState="false" Level="4" CssClass="listing-title" ResourceString="ImportObjects.Selection" />
    <div class="control-group-inline control-group-inline-wrap">
        <cms:LocalizedButton ID="lnkSelectDefault" runat="server" OnClick="lnkSelectDefault_Click" ButtonStyle="Default" ResourceString="ImportObjects.SelectDefault" EnableViewState="false" />
        <cms:LocalizedButton ID="lnkSelectAll" runat="server" OnClick="lnkSelectAll_Click" ButtonStyle="Default" ResourceString="ImportObjects.SelectAll" EnableViewState="false" />
        <cms:LocalizedButton ID="lnkSelectNew" runat="server" OnClick="lnkSelectNew_Click" ButtonStyle="Default" ResourceString="ImportObjects.SelectNew" EnableViewState="false" />
        <cms:LocalizedButton ID="lnkSelectNone" runat="server" OnClick="lnkSelectNone_Click" ButtonStyle="Default" ResourceString="ImportObjects.SelectNone" EnableViewState="false" />
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlMacroResigning" CssClass="wizard-section content-block-25">
    <cms:LocalizedHeading ID="headMacroResigning" Level="4" runat="server" EnableViewState="false" CssClass="listing-title" ResourceString="ImportObjects.MacroResigning" />
    <p>
        <cms:LocalizedLabel ID="lblMacroResigning" runat="server" EnableViewState="false" ResourceString="ImportObjects.MacroResigningInfo" />
    </p>
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblMacroResigningUser" runat="server" ResourceString="general.user" ToolTipResourceString="ImportObjects.MacroResigning.User.Tooltip"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="userSelectorMacroResigningUser" />
            </div>
            <div class="editing-form-value-cell ">
                <cms:SelectUser ID="userSelectorMacroResigningUser" runat="server" IsLiveSite="false" AllowAll="false" AllowEmpty="true" ShowSiteFilter="true"
                    DisplayUsersFromAllSites="true" SelectionMode="SingleTextBox" WhereCondition="UserName != N'public'" />
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlCheck" CssClass="wizard-section content-block-50">
    <cms:LocalizedHeading ID="headSettings" Level="4" runat="server" EnableViewState="false" CssClass="listing-title" ResourceString="ImportObjects.Settings" />
    <div class="form-horizontal">
        <div class="checkbox-list-vertical">
            <asp:PlaceHolder runat="server" ID="plcSite" Visible="false">
                <asp:PlaceHolder ID="plcExistingSite" runat="Server" Visible="false">
                    <cms:CMSCheckBox ID="chkUpdateSite" runat="server" />
                </asp:PlaceHolder>
                <cms:CMSCheckBox ID="chkBindings" runat="server" />
                <cms:CMSCheckBox ID="chkRunSite" runat="server" />
                <cms:CMSCheckBox ID="chkDeleteSite" runat="server" />
            </asp:PlaceHolder>
            <cms:CMSCheckBox ID="chkSkipOrfans" runat="server" ResourceString="ImportObjects.SkipOrfans" />
            <cms:CMSCheckBox ID="chkImportTasks" runat="server" ResourceString="ImportObjects.ImportTasks" />
            <cms:CMSCheckBox ID="chkLogSync" runat="server" ResourceString="ImportObjects.LogSynchronization" />
            <cms:CMSCheckBox ID="chkLogInt" runat="server" ResourceString="ImportObjects.LogIntegration" />
            <cms:CMSCheckBox ID="chkRebuildIndexes" runat="server" ResourceString="importobjects.rebuildsiteindexes" />
            <cms:CMSCheckBox ID="chkCopyFiles" runat="server" ResourceString="ImportObjects.CopyFiles" />
            <div class="selector-subitem">
                <div class="checkbox-list-vertical">
                    <cms:CMSCheckBox ID="chkCopyCodeFiles" runat="server" ResourceString="ImportObjects.CopyCodeFiles" />
                    <cms:CMSCheckBox ID="chkCopyAssemblies" runat="server" ResourceString="ImportObjects.CopyAssemblies" />
                    <cms:CMSCheckBox ID="chkCopyGlobalFiles" runat="server" ResourceString="ImportObjects.CopyGlobalFiles" />
                    <asp:PlaceHolder ID="plcSiteFiles" runat="server" Visible="false">
                        <cms:CMSCheckBox ID="chkCopySiteFiles" runat="server" ResourceString="ImportObjects.CopySiteFiles" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
