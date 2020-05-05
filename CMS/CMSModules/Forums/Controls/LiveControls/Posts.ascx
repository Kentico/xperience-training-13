<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_LiveControls_Posts"  Codebehind="Posts.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" tagname="HeaderActions" tagprefix="cms" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/PostTree.ascx" TagName="PostTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Posts/ForumPost.ascx" TagName="ForumPost" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Posts/PostEdit.ascx" TagName="PostEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>


<script type="text/javascript">
    //<![CDATA[
    function ReplyToPost(id) {
    }
    function SelectForumNode(nodeElem) {

    }
    //]]>
</script>

<asp:HiddenField ID="hdnPost" EnableViewState="true" runat="server" />
<table class="ForumPosts">
    <tr>
        <td style="vertical-align: top;">
            <cms:PostTree ID="treeElem" runat="server" ItemSelectedItemCssClass="ContentTreeSelectedItem" />
        </td>
        <td style="vertical-align: top;">
            <div class="PostPreview">
                <asp:PlaceHolder ID="plcPostEdit" runat="server">
                    <cms:PageTitle ID="titleEditElem" runat="server" />
                    <asp:Panel ID="pnlEditActions" runat="server">
                        <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" HideBreadcrumbs="false" EnableViewState="false" PropagateToMainNavigation="false" />
                        <asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
                    </asp:Panel>
                    <cms:PostEdit ID="postEdit" runat="server" RedirectEnabled="false" />
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcPostView" runat="server" Visible="false">
                    <cms:PageTitle ID="titleViewElem" runat="server" />
                    <br />
                    <asp:Panel runat="server" ID="pnlMenu" CssClass="ContentEditMenu" EnableViewState="false">
                        <cms:HeaderActions ID="actionsElem" runat="server" />
                    </asp:Panel>
                    <div class="ForumFlat">
                        <cms:ForumPost ID="postView" runat="server" DisplayOnly="true" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcPostNew" runat="server" Visible="false">
                    <cms:PostEdit ID="postNew" runat="server" RedirectEnabled="false" />
                </asp:PlaceHolder>
                <br />
                <cms:UniGrid runat="server" ID="UniGrid" GridName="~/CMSModules/Forums/Controls/Posts/AttachmentList.xml"
                    Visible="false" IsLiveSite="true" OrderBy="AttachmentFileName" />
            </div>
        </td>
    </tr>
</table>
