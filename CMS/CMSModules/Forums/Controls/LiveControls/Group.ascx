<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_LiveControls_Group"  Codebehind="Group.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumList.ascx" TagName="ForumList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/LiveControls/Forum.ascx" TagName="ForumEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumNew.ascx" TagName="ForumNew" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Groups/GroupEdit.ascx" TagName="GroupEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<cms:BasicTabControl ID="tabElem" runat="server" TabControlLayout="Horizontal" UseClientScript="true" UsePostback="true" />
<asp:PlaceHolder ID="tabForums" runat="server">
    <asp:PlaceHolder ID="plcForumList" runat="server">
        <div class="TabBody">
            <asp:Panel ID="pnlListHeader" runat="server" CssClass="PageHeader">
                <cms:PageTitle ID="titleElem" runat="server" />
            </asp:Panel>
            <asp:Panel ID="pnlListActions" runat="server">
                <cms:HeaderActions ID="actionsElem" runat="server" />
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlTab">
                <cms:ForumList ID="forumList" runat="server" />
            </asp:Panel>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcForumEdit" runat="server">
        <div class="TabBody">
            <asp:Panel ID="pnlEditTitle" runat="server" CssClass="PageHeader">
                <cms:PageTitle ID="titleElemEdit" runat="server" />
            </asp:Panel>
            <asp:Panel ID="pnlEditActions" runat="server">
                <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" HideBreadcrumbs="false" EnableViewState="false" PropagateToMainNavigation="false" />
            </asp:Panel>
            <cms:ForumEdit ID="forumEdit" runat="server" />
        </div>
    </asp:PlaceHolder>
</asp:PlaceHolder>
<asp:Panel ID="tabNewForum" runat="server" CssClass="GroupForums">
    <asp:Panel ID="pnlNewHeader" runat="server">
        <cms:Breadcrumbs ID="ucBreadcrumbsNewForum" runat="server" HideBreadcrumbs="false" EnableViewState="false" PropagateToMainNavigation="false" />
    </asp:Panel>
    <cms:ForumNew ID="forumNew" runat="server" />
</asp:Panel>
<asp:PlaceHolder ID="tabGeneral" runat="server">
    <asp:Panel runat="server" ID="pnlEditContent" CssClass="GroupForums">
        <cms:GroupEdit ID="groupEdit" runat="server" />
    </asp:Panel>
</asp:PlaceHolder>
<asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />