<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="SettingsGroupViewer.ascx.cs" Inherits="CMSModules_Settings_Controls_SettingsGroupViewer" %>
<cms:LocalizedLabel ID="lblNoData" runat="server" CssClass="InfoLabel" EnableViewState="false" Visible="false" ResourceString="settingskey.nodata" />
<div class="form-horizontal">
    <script type="text/javascript">
        // Reloads the category tree with the URL parameters to select the required category
        function selectCategory(categoriesUrl) {
            if ((window.parent != null) && (window.parent.frames['categories'] != null)) {
                var categoriesFrame = window.parent.frames['categories'];
                categoriesFrame.window.location.href = categoriesUrl;
                return true;
            }
            return false;
        }
    </script>
    <cms:CMSPlaceHolder ID="plcContent" runat="server" />
</div>
