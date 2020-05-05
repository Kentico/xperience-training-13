<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_Messages_MessageList"
     Codebehind="MessageList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MessageBoards/FormControls/SelectBoard.ascx" TagName="BoardSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<script type="text/javascript">
    //<![CDATA[

    // Confirm mass delete
    function MassConfirm(dropdown, msg) {
        var drop = document.getElementById(dropdown);
        if (drop != null) {
            if (drop.value == "DELETE") {
                return confirm(msg);
            }
            return true;
        }
        return true;
    }

    //]]>
</script>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<asp:PlaceHolder ID="plcNewMessageGroups" runat="server" Visible="false">
    <div class="cms-edit-menu">
        <cms:HeaderActions ID="headerActions" runat="server" />
    </div>
</asp:PlaceHolder>

<%-- Filter --%>
<asp:PlaceHolder runat="server" ID="plcFilter">
    <div class="form-horizontal form-filter">
        <%-- Site --%>
        <asp:PlaceHolder ID="plcSite" runat="server" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSiteName" runat="server" ResourceString="board.messagelist.sitename"
                        EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:SiteSelector ID="siteSelector" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <%-- Board --%>
        <asp:PlaceHolder ID="plcBoard" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblBoardName" runat="server" ResourceString="board.messagelist.boardname"
                        EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSUpdatePanel ID="pnlUpdateBoardSelector" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="siteSelector" />
                        </Triggers>
                        <ContentTemplate>
                            <cms:BoardSelector ID="boardSelector" runat="server" AddAllItemsRecord="true" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </div>
        </asp:PlaceHolder>
        <%-- User name --%>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblUserName" runat="server" AssociatedControlID="txtUserName"
                    ResourceString="general.username" DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtUserName" runat="server" EnableViewState="false" />
            </div>
        </div>
        <%-- Text --%>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblMessage" AssociatedControlID="txtMessage" runat="server"
                    ResourceString="board.messagelist.message" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtMessage" runat="server" EnableViewState="false" />
            </div>
        </div>
        <%-- Is approved --%>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblApproved" AssociatedControlID="drpApproved" runat="server"
                    ResourceString="board.messagelist.approved" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSDropDownList ID="drpApproved" runat="server" CssClass="DropDownFieldSmall" />
            </div>
        </div>
        <%-- Is spam --%>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSpam" AssociatedControlID="drpSpam" runat="server" ResourceString="board.messagelist.spam"
                    EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSDropDownList ID="drpSpam" runat="server" CssClass="DropDownFieldSmall" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:CMSButton ID="btnFilter" runat="server" ButtonStyle="Primary" EnableViewState="false" OnClick="btnFilter_Click" />
            </div>
        </div>
    </div>
</asp:PlaceHolder>

<%-- Messages grid --%>
<cms:UniGrid runat="server" ID="gridElem" OrderBy="MessageInserted DESC" Columns="MessageID, MessageUserName, BoardID, MessageText, MessageApproved, MessageIsSpam, BoardDisplayName, MessageInserted, BoardSiteID"
    ObjectType="board.boardmessagelist" RememberState="false">
    <GridActions>
        <ug:Action Name="edit" ExternalSourceName="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$general.confirmdelete$" />
        <ug:Action Name="approve" ExternalSourceName="approve" Caption="$General.Approve$"
            FontIconClass="icon-check-circle" FontIconStyle="Allow" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="MessageUserName" Caption="$General.UserName$" Wrap="false" />
        <ug:Column Source="MessageText" IsText="true" Caption="$Unigrid.Board.Message.Columns.MessageText$" Wrap="false" MaxLength="30">
            <Tooltip Source="MessageText" Encode="true" />
        </ug:Column>
        <ug:Column Source="MessageApproved" ExternalSourceName="#yesno" Caption="$Unigrid.Board.Message.Columns.MessageApproved$"
            Wrap="false" />
        <ug:Column Source="MessageIsSpam" ExternalSourceName="MessageIsSpam" Caption="$Unigrid.Board.Message.Columns.MessageIsSpam$"
            Wrap="false" />
        <ug:Column Source="BoardDisplayName" Name="BoardName" Caption="$Unigrid.Board.Message.Columns.BoardName$"
            Wrap="false" />
        <ug:Column Source="MessageInserted" ExternalSourceName="MessageInserted" Caption="$Unigrid.Board.Message.Columns.MessageInserted$"
            Wrap="false" />
        <ug:Column Source="BoardSiteID" ExternalSourceName="#sitename" Name="SiteName" Caption="$General.SiteName$" Wrap="false" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="false" ShowSelection="true" />
</cms:UniGrid>

<%-- Mass actions --%>
<asp:PlaceHolder runat="server" ID="plcActions">
    <div class="form-horizontal mass-action">
        <div class="form-group">
            <div class="mass-action-label-cell">
                <cms:LocalizedLabel ID="lblActions" AssociatedControlID="drpActions" ResourceString="board.messagelist.actions"
                    runat="server" EnableViewState="false" CssClass="control-label" />
            </div>
            <div class="mass-action-value-cell">
                <cms:CMSDropDownList ID="drpActions" runat="server" />
                <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOk_Clicked"
                    EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:PlaceHolder>
<asp:Button ID="btnRefreshHdn" runat="server" Visible="false" OnCommand="btnRefreshHdn_Command" CssClass="HiddenButton" />
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />