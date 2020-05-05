<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="EditToolbar.ascx.cs" Inherits="CMSModules_PortalEngine_Controls_OnSiteEdit_EditToolbar" %>
<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIToolbar.ascx" TagName="UIToolbar"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/AdvancedPopupHandler.ascx" TagName="AdvancedPopupHandler"
    TagPrefix="cms" %>
<asp:PlaceHolder ID="plcRfrWOpnrScript" runat="server">
    <script type="text/javascript">
        //<![CDATA[
        var refreshPageOnClose = false;

        function RefreshWOpener(w) {
            if (refreshPageOnClose || w.refreshPageOnClose) {
                var reloadUrl = '';

                if ((w.reloadPageUrl) && (w.reloadPageUrl.length > 0)) {
                    reloadUrl = w.reloadPageUrl;
                }
                else {
                    reloadUrl = window.location.href;
                }

                // Keep the information about unsaved content
                if (window.CMSContentManager) {
                    reloadUrl = $cmsj.param.querystring(reloadUrl, "cmscontentchanged=" + CMSContentManager.contentModified());
                    if (!CMSContentManager.contentModified()) {
                        // Remove cmscontentchanged from URL and preserve & or ? correctly
                        reloadUrl = reloadUrl.replace(/((&)|(\\?))cmscontentchanged=([^&]*)(&)?/gi, '$3$2');
                    }
                }

                if (window.CMSContentManager) { CMSContentManager.allowSubmit = true; }
                window.location = reloadUrl.replace('safemode=1', 'safemode=0');
            }
        }
        //]]>
    </script>
</asp:PlaceHolder>
<cms:AdvancedPopupHandler runat="server" ID="popupHandler" SetTitle="false" Visible="false" />
<asp:PlaceHolder runat="server" ID="plcEdit">
    <cms:CMSPanel ID="pnlToolbarSpace" ShortID="ts" runat="server" CssClass="OnSiteToolbarSpace">
    </cms:CMSPanel>
    <cms:CMSPanel ID="pnlToolbar" ShortID="t" runat="server" CssClass="OnSiteToolbar cms-bootstrap">
        <cms:CMSPanel ID="pnlBody" ShortID="b" runat="server" CssClass="OnSiteMenu">
            <cms:CMSPanel runat="server" ID="pnlLeft" ShortID="l" CssClass="OnSiteMenuLeft">
                <cms:UIToolbar ID="ucUIToolbar" ShortID="ut" runat="server" DisableScrollPanel="true"
                    GenerateElementCssClass="true" RememberSelectedItem="false"
                    HighlightFirstItem="false" QueryParameterName="resourcename" DisableEditIcon="true" />
            </cms:CMSPanel>
            <div style="clear: both">
            </div>
        </cms:CMSPanel>
    </cms:CMSPanel>
    <cms:CMSPanel runat="server" ID="pnlSlider" ShortID="sb" CssClass="OnSiteSlider cms-bootstrap"
        Visible="false">
        <cms:CMSPanel ID="pnlButton" runat="server" CssClass="OnSiteSliderButton">
            <cms:CMSIcon runat="server" ID="icon" />
            <br />
            <cms:LocalizedLabel ID="lblSliderText" runat="server" />
        </cms:CMSPanel>
    </cms:CMSPanel>
    <asp:Panel runat="server" ID="pnlUpdateProgress" CssClass="cms-bootstrap" Visible="false">
        <cms:CMSUpdateProgress ID="updateProgress" runat="server" />
    </asp:Panel>
    <asp:HiddenField ID="hdnPostbackValue" runat="server" />
</asp:PlaceHolder>

