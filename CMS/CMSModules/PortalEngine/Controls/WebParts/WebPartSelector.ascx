<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_Controls_WebParts_WebPartSelector"
     Codebehind="WebPartSelector.ascx.cs" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/WebPartFlatSelector.ascx"
    TagName="WebPartFlatSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/WebPartTree.ascx" TagName="WebPartTree"
    TagPrefix="cms" %>
<asp:Panel runat="server" CssClass="ItemSelector">
    <table class="SelectorTable" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <div class="SelectorTree">
                    <div class="TreePadding">
                        <cms:WebPartTree EnableViewState="false" UsePostBack="false" ID="treeElem" runat="server"
                            SelectWebParts="false" ShowRecentlyUsed="true" />
                        <cms:WebPartTree EnableViewState="false" UsePostBack="false" ID="treeUI" runat="server"
                            SelectWebParts="false" ShowRecentlyUsed="false" RootPath="UIWebparts" />
                    </div>
                </div>
            </td>
            <td class="SelectorBorder">
                <div class="SelectorBorderGlue">
                </div>
            </td>
            <td class="ItemSelectorArea">
                <div class="ItemSelector">
                    <cms:WebPartFlatSelector ID="flatElem" runat="server" />
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>
