<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_InnerFileSystemView"
     Codebehind="InnerFileSystemView.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false"></asp:Literal>
<div class="DialogViewArea">
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false"></asp:Label>
    <asp:PlaceHolder ID="plcViewArea" runat="server">
        <asp:PlaceHolder ID="plcListView" runat="server" Visible="false">
            <div class="ListView">
                <cms:UniGrid ID="gridList" ShortID="g" runat="server" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcThumbnailsView" runat="server" Visible="false">
            <div class="dialog-content">
                <cms:BasicRepeater ID="repThumbnailsView" ShortID="th" runat="server">
                    <ItemTemplate>
                        <div class="DialogThumbnailItemShadow">
                            <div id="<%#GetID(Container.DataItem)%>" class="DialogThumbnailItem">
                                <asp:Panel ID="pnlThumbnails" runat="server" CssClass="DialogThumbnailItemBox" EnableViewState="false">
                                    <asp:Panel ID="pnlImageContainer" runat="server" CssClass="DialogThumbItemImageContainer"
                                        EnableViewState="false">
                                        <table class="DialogThumbnailItemImage">
                                            <tr>
                                                <td>
                                                    <asp:Image ID="imgElem" runat="server" />
                                                    <asp:Literal ID="ltrImage" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <div class="DialogThumbnailItemInfo">
                                        <asp:Label ID="lblFileName" runat="server" EnableViewState="false" />
                                    </div>
                                </asp:Panel>
                                <div class="DialogThumbnailActions">
                                    <cms:CMSAccessibleButton ID="btnDelete" runat="server" EnableViewState="false" IconOnly="True" IconCssClass="icon-bin" />
                                    <cms:CMSAccessibleButton ID="btnEdit" runat="server" EnableViewState="false" IconOnly="True" IconCssClass="icon-edit" />
                                    <div class="clearfix"></div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </cms:BasicRepeater>
                <div class="clearfix"></div>
                <cms:UIPager ID="pagerElemThumbnails" ShortID="pth" runat="server" />
            </div>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
</div>
<asp:HiddenField ID="hdnItemToColorize" runat="server" />
