<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_PortalEngine_Controls_Layout_LayoutFlatSelector"  Codebehind="LayoutFlatSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniFlatSelector.ascx" TagName="UniFlatSelector"
    TagPrefix="cms" %>

<script type="text/javascript">
    //<![CDATA[
    // Javacript after async postback
    function pageLoad() {
        // Resizes area
        if ($cmsj.isFunction(window.resizearea)) {
            resizearea();
        }

        // Uniflat search
        setTimeout('Focus()', 100);
        var timer = null;
        SetupSearch();
    }

</script>

<asp:Panel ID="Panel1" runat="server" CssClass="ItemSelector">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="LayoutSelector">
                <cms:UniFlatSelector ID="flatElem" runat="server">
                    <HeaderTemplate>
                        <div class="SelectorFlatItems">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="SelectorEnvelope" style="overflow: hidden">
                            <div class="SelectorFlatImage">
                                <%#PortalHelper.GetIconHtml(EvalGuid("LayoutThumbnailGUID"), ValidationHelper.GetString(Eval("LayoutIconClass"), PortalHelper.DefaultPageLayoutIconClass))%>
                            </div>
                            <span class="SelectorFlatText">
                                <%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(Eval("LayoutDisplayName"))))%></span>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <div style="clear: both">
                        </div>
                        </div>
                    </FooterTemplate>
                </cms:UniFlatSelector>
                <div class="selector-flat-description">
                    <asp:Literal runat="server" ID="litCategory" EnableViewState="false"></asp:Literal>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
