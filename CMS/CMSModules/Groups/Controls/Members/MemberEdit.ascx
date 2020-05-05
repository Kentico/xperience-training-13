<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Controls_Members_MemberEdit"  Codebehind="MemberEdit.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="UserSelector" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblFullNameLabel" runat="server" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel ID="lblFullName" runat="server" CssClass="form-control-text"/>
            <cms:UserSelector ID="userSelector" runat="server" Visible="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblComment" runat="server" AssociatedControlID="txtComment" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextArea ID="txtComment" runat="server" Rows="4" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcEdit" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblMemberJoinedLabel" runat="server" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizedLabel ID="lblMemberJoined" runat="server" CssClass="form-control-text"/>
            </div>
        </div>
        <div class="form-group" runat="server" id="rowApproved">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblMemberApprovedLabel" runat="server" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizedLabel ID="lblMemberApproved" runat="server" CssClass="form-control-text"/>
            </div>
        </div>
        <div class="form-group" runat="server" id="rowRejected">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblMemberRejectedLabel" runat="server" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizedLabel ID="lblMemberRejected" runat="server" CssClass="form-control-text"/>
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcNew" runat="server" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblMemberApprove" runat="server" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkApprove" runat="server" Checked="true" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnSave" runat="server" ButtonStyle="Primary" />
            <cms:CMSButton ID="btnApprove" runat="server" EnableViewState="true" ButtonStyle="Primary" />
            <cms:CMSButton ID="btnReject" runat="server" ButtonStyle="Primary" />
        </div>
    </div>
</div>
<cms:LocalizedHeading runat="server" ID="headRoles" EnableViewState="False" Level="4" CssClass="listing-title" DisplayColon="true" />
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniSelector ID="usRoles" runat="server" IsLiveSite="false" ObjectType="cms.role"
            SelectionMode="Multiple" ResourcePrefix="addroles" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<cms:LocalizedLabel ID="lblRole" runat="server" ResourceString="group.member.role" />