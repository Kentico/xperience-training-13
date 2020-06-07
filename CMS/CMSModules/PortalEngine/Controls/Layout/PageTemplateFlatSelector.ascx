<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_PortalEngine_Controls_Layout_PageTemplateFlatSelector"  Codebehind="PageTemplateFlatSelector.ascx.cs" %>
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
        var timer = null;
        SetupSearch();
    }
    //]]>
</script>

<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniFlatSelector ID="flatElem" ShortID="f" runat="server" RememberSelectedItem="true">
            <HeaderTemplate>
                <div class="SelectorFlatItems">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="SelectorEnvelope" style="overflow: hidden">
                    <div class="SelectorFlatImage">
                        <%#PortalHelper.GetIconHtml(EvalGuid("PageTemplateThumbnailGUID"), ValidationHelper.GetString(Eval("PageTemplateIconClass"), PortalHelper.DefaultPageTemplateIconClass))%>
                    </div>
                    <span class="SelectorFlatText">
                        <%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(Eval("PageTemplateDisplayName"))))%></span>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                <div style="clear: both">
                </div>
                </div>
            </FooterTemplate>
        </cms:UniFlatSelector>
        <div class="selector-flat-description">
            <asp:Literal runat="server" ID="ltrDescription" EnableViewState="false"></asp:Literal>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
