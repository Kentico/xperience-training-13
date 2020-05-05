<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_InnerMediaView"
     Codebehind="InnerMediaView.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>

<script type="text/javascript">
    //<![CDATA[
    // Confirm mass delete
    function MassConfirm(dropdown, msg) {
        var drop = document.getElementById(dropdown);
        if (drop != null) {
            if (drop.value == "delete") {
                return confirm(msg);
            }
            return true;
        }
        return true;
    }

    function SetLibParentAction(argument) {
        // Raise select action
        SetAction('morefolderselect', argument);
        RaiseHiddenPostBack();
    }
    //]]>
</script>

<div id="<%=ClientID%>">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <div class="DialogViewArea">
        <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
        <asp:PlaceHolder ID="plcViewArea" runat="server">
            <asp:PlaceHolder ID="plcListView" runat="server" Visible="false">
                <div class="ListView">
                    <cms:UniGrid ID="gridList" ShortID="g" runat="server" RememberState="false" DelayedReload="true" AllowShiftKeySelection="false" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcThumbnailsView" runat="server" Visible="false">
                <div class="ThumbnailsView">
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
                                                        <asp:Image ID="imgFile" runat="server" />
                                                        <asp:Label ID="imgFileIcon" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <div class="DialogThumbnailItemInfo">
                                            <asp:Label ID="lblFileName" runat="server" EnableViewState="false"></asp:Label>
                                        </div>
                                    </asp:Panel>
                                    <div class="DialogThumbnailActions" enableviewstate="false">
                                        <table>
                                            <tr>
                                                <asp:PlaceHolder ID="plcWarning" runat="server" EnableViewState="false">
                                                    <td>
                                                        <cms:CMSGridActionButton ID="btnWarning" runat="server" EnableViewState="false" IconCssClass="icon-exclamation-triangle" IconStyle="Warning" />
                                                    </td>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plcSelectSubDocs" runat="server" EnableViewState="false">
                                                    <td>
                                                        <cms:CMSGridActionButton ID="btnSelectSubDocs" runat="server" EnableViewState="false" IconCssClass="icon-arrow-crooked-right" />
                                                    </td>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plcSelectSubFolders" runat="server">
                                                    <td>
                                                        <cms:CMSGridActionButton ID="imgSelectSubFolders" runat="server" EnableViewState="false" IconCssClass="icon-arrow-crooked-right" />
                                                    </td>
                                                </asp:PlaceHolder>
                                                <td>
                                                    <cms:CMSGridActionButton ID="btnView" runat="server" EnableViewState="false" IconCssClass="icon-eye" IconStyle="Allow" />
                                                </td>
                                                <asp:PlaceHolder ID="plcContentEdit" runat="server" EnableViewState="false">
                                                    <td>
                                                        <cms:CMSGridActionButton ID="btnContentEdit" runat="server" IconCssClass="icon-edit" IconStyle="Allow" />
                                                    </td>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plcAttachmentActions" runat="server">
                                                    <asp:PlaceHolder ID="plcAttachmentUpdtAction" runat="server" EnableViewState="false">
                                                        <td>
                                                            <cms:CMSGridActionButton ID="btnEdit" runat="server" IconCssClass="icon-edit" IconStyle="Allow" />
                                                        </td>
                                                        <td>
                                                            <cms:DirectFileUploader ID="dfuElem" runat="server" UploadMode="DirectSingle" Width="16px" Height="16px" />
                                                        </td>
                                                    </asp:PlaceHolder>
                                                    <td>
                                                        <asp:PlaceHolder ID="plcExtEdit" runat="server" EnableViewState="false"></asp:PlaceHolder>
                                                    </td>
                                                    <asp:PlaceHolder ID="plcLibraryUpdtAction" runat="server" EnableViewState="false">
                                                        <td>
                                                            <cms:DirectFileUploader ID="dfuElemLib" runat="server" />
                                                            <asp:Panel ID="pnlDisabledUpdate" runat="server">
                                                            </asp:Panel>
                                                        </td>
                                                        <td>
                                                            <asp:PlaceHolder ID="plcExtEditMfi" runat="server" EnableViewState="false"></asp:PlaceHolder>
                                                        </td>
                                                    </asp:PlaceHolder>
                                                    <td>
                                                        <cms:CMSGridActionButton ID="btnDelete" runat="server" EnableViewState="false" IconCssClass="icon-bin" IconStyle="Critical" />
                                                    </td>
                                                </asp:PlaceHolder>
                                            </tr>
                                        </table>
                                        <div class="clearfix"></div>
                                    </div>
                                </div>
                                <asp:PlaceHolder ID="plcSelectionBox" runat="server" Visible="false">
                                    <cms:CMSCheckBox ID="chkSelected" CssClass="ThumbMultipleSelection" EnableViewState="false"
                                        runat="server" />
                                    <asp:HiddenField ID="hdnItemName" runat="server" />
                                </asp:PlaceHolder>
                            </div>
                        </ItemTemplate>
                    </cms:BasicRepeater>
                    <div class="clearfix"></div>
                    <cms:UIPager ID="pagerElemThumbnails" ShortID="pth" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:Panel ID="pnlMassAction" runat="server" Visible="false" CssClass="form-horizontal mass-action">
                <div class="form-group">
                    <div class="mass-action-value-cell">
                        <cms:CMSDropDownList ID="drpActionFiles" runat="server" />
                        <cms:CMSDropDownList ID="drpActions" runat="server" />
                        <cms:LocalizedButton ID="btnActions" runat="server" ButtonStyle="Primary" EnableViewState="false"
                            ResourceString="general.ok" />
                    </div>
                </div>
            </asp:Panel>
        </asp:PlaceHolder>
    </div>
    <asp:HiddenField ID="hdnItemToColorize" runat="server" />
    <input id="hdnFileOrigName" type="hidden" />
</div>