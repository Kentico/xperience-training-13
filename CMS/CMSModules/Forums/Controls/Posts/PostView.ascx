<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Posts_PostView"
     Codebehind="PostView.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Posts/ForumPost.ascx" TagName="ForumPost"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Posts/PostEdit.ascx" TagName="PostEdit"
    TagPrefix="cms" %>

<asp:PlaceHolder ID="plcManager" runat="server" />
<asp:Panel ID="PanelNewThread" runat="server" Visible="false">
    <cms:PageTitle ID="NewThreadTitle" runat="server" HideTitle="true" />
    <div class="ForumFlat">
        <cms:PostEdit ID="PostEdit1" runat="server" />
    </div>
</asp:Panel>
<asp:Panel ID="pnlBody" runat="server">
    <asp:Panel runat="server" ID="pnlMove" Visible="false" CssClass="ForumMoveThreadArea">
        <asp:PlaceHolder runat="server" ID="plcThreadMove"></asp:PlaceHolder>
    </asp:Panel>
    <asp:Panel ID="pnlContent" runat="server" Visible="true">
        <div class="ForumFlat">
            <cms:ForumPost ID="ForumPost1" runat="server" EnableSignature="true" />
        </div>
        <br />
        <asp:Button ID="btnDelete" runat="server" CssClass="HiddenButton" EnableViewState="false"
            OnClick="btnDelete_Click" />
        <asp:Button ID="btnApprove" runat="server" CssClass="HiddenButton" EnableViewState="false"
            OnClick="btnApprove_Click" />
        <asp:Button ID="btnApproveSubTree" runat="server" CssClass="HiddenButton" EnableViewState="false"
            OnClick="btnApproveSubTree_Click" />
        <asp:Button ID="btnRejectSubTree" runat="server" CssClass="HiddenButton" EnableViewState="false"
            OnClick="btnRejectSubTree_Click" />
        <asp:Button ID="btnStickThread" runat="server" CssClass="HiddenButton" EnableViewState="false"
            OnClick="btnStickThread_Click" />
        <asp:Button ID="btnSplitThread" runat="server" CssClass="HiddenButton" EnableViewState="false"
            OnClick="btnSplitThread_Click" />
        <asp:Button ID="btnLockThread" runat="server" CssClass="HiddenButton" EnableViewState="false"
            OnClick="btnLockThread_Click" />
        <asp:Button runat="server" ID="btnMoveThread" CssClass="HiddenButton" EnableViewState="false"
            OnClick="btnMoveThread_Click" />
    </asp:Panel>
    <asp:Panel ID="pnlAttachmentTitle" runat="server">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="ForumPost_View.PostAttachmentTitle"></cms:LocalizedHeading>
    </asp:Panel>
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />

    <asp:Panel ID="pnlAttachmentContent" runat="server" Visible="true">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-value-cell">
                    <cms:CMSFileUpload runat="server" ID="upload" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell">
                    <cms:LocalizedButton ResourceString="general.upload" OnClick="btnUpload_Click" ValidationGroup="NewPostforum"
                                    runat="server" ID="btnUpload" ButtonStyle="Default" />
                </div>
            </div>
            <cms:UniGrid runat="server" ID="UniGrid" GridName="~/CMSModules/Forums/Controls/Posts/AttachmentList.xml"
                Visible="false" OrderBy="AttachmentFileName" />
        </div>
    </asp:Panel>
</asp:Panel>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
